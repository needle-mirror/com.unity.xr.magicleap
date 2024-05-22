using System;
using Unity.Collections;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap.Internal;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// The Magic Leap implementation of the <c>XRSessionSubsystem</c>. Do not create this directly.
    /// Use <c>MagicLeapSessionSubsystemDescriptor.Create()</c> instead.
    /// </summary>
    [Preserve]
    public sealed class MagicLeapSessionSubsystem : XRSessionSubsystem
    {
#if !UNITY_2020_2_OR_NEWER
        /// <summary>
        /// Create a Magic Leap provider
        /// </summary>
        /// <returns>Magic Leap implementation of a Session Subsystem.</returns>
        protected override Provider CreateProvider() => new SessionProvider();
#endif

        public class SessionProvider : Provider
        {
            private PerceptionHandle m_PerceptionHandle;

            /// <summary>
            /// Constructor for SessionProvider
            /// </summary>
            public SessionProvider()
            {
                m_PerceptionHandle = PerceptionHandle.Acquire();
            }

            /// <summary>
            /// Get the session's availability
            /// </summary>
            /// <returns>Session availability promise</returns>
            public override Promise<SessionAvailability> GetAvailabilityAsync()
            {
                var availability =
#if UNITY_ANDROID
                SessionAvailability.Installed | SessionAvailability.Supported;
#else
                SessionAvailability.None;
#endif // UNITY_ANDROID
                return Promise<SessionAvailability>.CreateResolvedPromise(availability);
            }

            /// <summary>
            /// Tracking state of the HMD
            /// </summary>
            /// <returns>Tracking state status</returns>
            public override TrackingState trackingState
            {
                get
                {
                    var device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
                    if (device.TryGetFeatureValue(CommonUsages.trackingState, out InputTrackingState inputTrackingState))
                    {
                        if (inputTrackingState == InputTrackingState.None)
                            return TrackingState.None;
                        else if (inputTrackingState == (InputTrackingState.Position | InputTrackingState.Rotation))
                            return TrackingState.Tracking;
                        else
                            return TrackingState.Limited;
                    }
                    else
                    {
                        return TrackingState.None;
                    }
                }
            }

            /// <summary>
            /// Magic Leap Requested Features
            /// </summary>
            /// <returns>Features requested for use at runtime</returns>
            public override Feature requestedFeatures => MagicLeapFeatures.requestedFeatures;

            /// <summary>
            /// Get the configuration of available feature descriptions
            /// </summary>
            /// <returns>Array of Configuration descriptors</returns>
            public override NativeArray<ConfigurationDescriptor> GetConfigurationDescriptors(Allocator allocator)
                => MagicLeapFeatures.AcquireConfigurationDescriptors(allocator);

            /// <summary>
            /// Get/Set the tracking mode of the device
            /// </summary>
            public override Feature requestedTrackingMode
            {
                get => MagicLeapFeatures.requestedFeatures.Intersection(Feature.AnyTrackingMode);
                set
                {
                    MagicLeapFeatures.SetFeatureRequested(Feature.AnyTrackingMode, false);
                    MagicLeapFeatures.SetFeatureRequested(value, true);
                }
            }

            /// <summary>
            /// The current tracking mode feature flag.
            /// </summary>
            /// <remarks>
            /// Magic Leap will always try to use 6DoF tracking but will automatically switch to
            /// 3DoF if it doesn't have a sufficient tracking environment. This will report which
            /// of the two modes is currently active and
            /// <c>UnityEngine.XR.ARSubsystems.Feature.None</c> otherwise.
            /// </remarks>
            public override Feature currentTrackingMode
            {
                get
                {
                    switch (trackingState)
                    {
                        case TrackingState.Tracking:
                            return Feature.PositionAndRotation;
                        case TrackingState.Limited:
                            return Feature.RotationOnly;
                        default:
                            return Feature.None;
                    }
                }
            }

            /// <summary>
            /// Update requested features
            /// </summary>
            public override void Update(XRSessionUpdateParams updateParams, Configuration configuration)
            {
                // Magic Leap supports almost everything working at the same time except Point Clouds and Meshing
                if (configuration.features.HasFlag(Feature.Meshing | Feature.PointCloud))
                {
                    // TODO (5/26/2020): Move MLSpatialMapper specific features to shared XRMeshSubsystem extensions
                    // Currently, the MLSpatialMapper component is required to do PointClouds on magic leap.  So
                    // if meshing is detected at all then simply request a start to the subsystem because it will be
                    // handled either by ARMeshManager or the MLSpatialMapper.
                    var loader = (MagicLeapLoader)XRGeneralSettings.Instance.Manager.activeLoader;
                    if (loader.meshSubsystem != null && !loader.meshSubsystem.running && configuration.features.HasFlag(Feature.Meshing))
                    {
                        loader.StartMeshSubsystem();
                        loader.meshSubsystem.SetMeshingFeature(Feature.Meshing);
                    }
                }

                MagicLeapFeatures.currentFeatures = configuration.features;
            }

            /// <summary>
            /// Destroy/Dispose of the current Perception instance
            /// </summary>
            public override void Destroy()
            {
                m_PerceptionHandle.Dispose();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
#if UNITY_ANDROID
            XRSessionSubsystemDescriptor.RegisterDescriptor(new XRSessionSubsystemDescriptor.Cinfo
            {
                id = "MagicLeap-Session",
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(MagicLeapSessionSubsystem.SessionProvider),
                subsystemTypeOverride = typeof(MagicLeapSessionSubsystem),
#else
                subsystemImplementationType = typeof(MagicLeapSessionSubsystem),
#endif
                supportsInstall = false
            });
#endif // UNITY_ANDROID
        }
    }
}
