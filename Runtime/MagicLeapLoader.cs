using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

using UnityEngine.XR;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.InteractionSubsystems;
using UnityEngine.XR.MagicLeap.Meshing;
using UnityEngine.XR.MagicLeap.Rendering;
using UnityEngine.XR.Management;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEngine.Rendering;
#endif //UNITY_EDITOR

#if UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
#endif //UNITY_INPUT_SYSTEM

#if UNITY_2020_1_OR_NEWER
using XRTextureLayout = UnityEngine.XR.XRDisplaySubsystem.TextureLayout;
#endif // UNITY_2020_1_OR_NEWER

namespace UnityEngine.XR.MagicLeap
{
#if UNITY_EDITOR && XR_MANAGEMENT_3_2_0_OR_NEWER
    [XRSupportedBuildTarget(BuildTargetGroup.Android)]
    [XRSupportedBuildTarget(BuildTargetGroup.Standalone, new BuildTarget[]{BuildTarget.StandaloneOSX, BuildTarget.StandaloneWindows, BuildTarget.StandaloneWindows64})]
#endif // UNITY_EDITOR && XR_MANAGEMENT_3_2_0_OR_NEWER
    /// <summary>
    /// Magic Leap XR Loader.
    /// Part of the XR Management system for loading the Magic Leap Provider
    /// </summary>
    public sealed class MagicLeapLoader : XRLoaderHelper
#if UNITY_EDITOR
        , IXRLoaderPreInit
#endif
    {
 #if UNITY_EDITOR
        public string GetPreInitLibraryName(BuildTarget buildTarget, BuildTargetGroup buildTargetGroup)
        {
            return "UnityMagicLeap";
        }
#endif
        
        enum Privileges : uint
        {
            ControllerPose = 263,
            GesturesConfig = 269,
            GesturesSubscribe = 268,
            LowLatencyLightwear = 59,
            WorldReconstruction = 33
        }
        const string kLogTag = "MagicLeapLoader";
        // Integrated Subsystems
        static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();
        static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();
        static List<XRMeshSubsystemDescriptor> s_MeshSubsystemDescriptor = new List<XRMeshSubsystemDescriptor>();
        static List<XRGestureSubsystemDescriptor> s_GestureSubsystemDescriptors = new List<XRGestureSubsystemDescriptor>();

        /// <summary>
        /// Display Subsystem property.
        /// Use this to determine the loaded Display Subsystem.
        /// </summary>
        public XRDisplaySubsystem displaySubsystem => GetLoadedSubsystem<XRDisplaySubsystem>();
        /// <summary>
        /// XR Input Subsystem property.
        /// Use this to determine the loaded XR Input Subsystem.
        /// </summary>
        public XRInputSubsystem inputSubsystem => GetLoadedSubsystem<XRInputSubsystem>();
        /// <summary>
        /// XR Meshing Subsystem property.
        /// Use this to determine the loaded XR Meshing Subsystem.
        /// </summary>
        public XRMeshSubsystem meshSubsystem => GetLoadedSubsystem<XRMeshSubsystem>();
        /// <summary>
        /// XR Gesture Subsystem property.
        /// Use this to determine the loaded XR Gesture Subsystem.
        /// </summary>
        public XRGestureSubsystem gestureSubsystem => GetLoadedSubsystem<XRGestureSubsystem>();

        // ARSubsystems
        static List<XRSessionSubsystemDescriptor> s_SessionSubsystemDescriptors = new List<XRSessionSubsystemDescriptor>();
        static List<XRPlaneSubsystemDescriptor> s_PlaneSubsystemDescriptors = new List<XRPlaneSubsystemDescriptor>();
        static List<XRAnchorSubsystemDescriptor> s_AnchorSubsystemDescriptors = new List<XRAnchorSubsystemDescriptor>();
        static List<XRRaycastSubsystemDescriptor> s_RaycastSubsystemDescriptors = new List<XRRaycastSubsystemDescriptor>();
        static List<XRImageTrackingSubsystemDescriptor> s_ImageTrackingSubsystemDescriptors = new List<XRImageTrackingSubsystemDescriptor>();

        /// <summary>
        /// XR Session Subsystem property.
        /// Use this to determine the loaded XR Session Subsystem.
        /// </summary>
        public XRSessionSubsystem sessionSubsystem => GetLoadedSubsystem<XRSessionSubsystem>();
        /// <summary>
        /// XR Plane Subsystem property.
        /// Use this to determine the loaded XR Plane Subsystem.
        /// </summary>
        public XRPlaneSubsystem planeSubsystem => GetLoadedSubsystem<XRPlaneSubsystem>();
        /// <summary>
        /// XR Plane Subsystem property.
        /// Use this to determine the loaded XR Anchor Subsystem.
        /// </summary>
        public XRAnchorSubsystem anchorSubsystem => GetLoadedSubsystem<XRAnchorSubsystem>();
        /// <summary>
        /// XR Raycast Subsystem property.
        /// Use this to determine the loaded XR Raycast Subsystem.
        /// </summary>
        public XRRaycastSubsystem raycastSubsystem => GetLoadedSubsystem<XRRaycastSubsystem>();
        /// <summary>
        /// XR Image Tracking Subsystem property.
        /// Use this to determine the loaded XR Image Tracking Subsystem.
        /// </summary>
        public XRImageTrackingSubsystem imageTrackingSubsystem => GetLoadedSubsystem<XRImageTrackingSubsystem>();

#if UNITY_EDITOR
        /// <summary>
        /// Location of the Magic Leap Loader asset.
        /// </summary>
        public static MagicLeapLoader assetInstance => (MagicLeapLoader)AssetDatabase.LoadAssetAtPath("Packages/com.unity.xr.magicleap/XR/Loaders/Magic Leap Loader.asset", typeof(MagicLeapLoader));
#endif // UNITY_EDITOR

        private bool m_DisplaySubsystemRunning = false;
        private int m_MeshSubsystemRefcount = 0;

        internal bool DisableValidationChecksOnEnteringPlaymode = false;

        /// <summary>
        /// Initialize all the Magic Leap subsystems.
        /// </summary>
        /// <returns>true if successfully created all subsystems.</returns>
        public override bool Initialize()
        {
#if UNITY_EDITOR
            if (!DisableValidationChecksOnEnteringPlaymode)
            {
                // This will only work when "Magic Leap Zero Iteration" is selected as the Editor's XR Plugin
                if (MagicLeapProjectValidation.LogPlaymodeValidationIssues())
                    return false;
            }
#endif
            ApplySettings();

            // Display Subsystem depends on Input Subsystem, so initialize that first.
            //CheckForInputRelatedPermissions();
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRInputSubsystem>(MagicLeapConstants.kInputSubsystemId));
            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(s_DisplaySubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRDisplaySubsystem>(MagicLeapConstants.kDisplaySubsytemId));
            CreateSubsystem<XRGestureSubsystemDescriptor, XRGestureSubsystem>(s_GestureSubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRGestureSubsystem>(MagicLeapConstants.kGestureSubsystemId));

            CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(s_MeshSubsystemDescriptor, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRMeshSubsystem>(MagicLeapConstants.kMeshSubsystemId));

            // Now that subsystem creation is strictly handled by the loaders we must create the following subsystems
            // that live in ARSubsystems
            CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(s_SessionSubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRSessionSubsystem>(MagicLeapConstants.kSessionSubsystemId));
            CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(s_PlaneSubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRPlaneSubsystem>(MagicLeapConstants.kPlanesSubsystemId));
            //CreateSubsystem<XRAnchorSubsystemDescriptor, XRAnchorSubsystem>(s_AnchorSubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRAnchorSubsystem>(MagicLeapConstants.kAnchorSubsystemId));
            CreateSubsystem<XRRaycastSubsystemDescriptor, XRRaycastSubsystem>(s_RaycastSubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRRaycastSubsystem>(MagicLeapConstants.kRaycastSubsystemId));
            CreateSubsystem<XRImageTrackingSubsystemDescriptor, XRImageTrackingSubsystem>(s_ImageTrackingSubsystemDescriptors, MagicLeapSettings.Subsystems.GetSubsystemOverrideOrDefault<XRImageTrackingSubsystem>(MagicLeapConstants.kImageTrackingSubsystemId));

            return true;
        }

        /// <summary>
        /// Start all subsystems.
        /// </summary>
        /// <returns>true if all subsystems have successfully started.</returns>
        public override bool Start()
        {
            StartSubsystem<XRInputSubsystem>();
            StartSubsystem<XRGestureSubsystem>();

            if (!isLegacyDeviceActive)
            {
                var settings = MagicLeapSettings.currentSettings;
#if UNITY_2020_1_OR_NEWER
                if (settings != null && settings.forceMultipass)
                {
                    displaySubsystem.textureLayout = XRTextureLayout.SeparateTexture2Ds;
                    RenderingSettings.UnityMagicLeap_RenderingSetParameter("SinglePassEnabled", 0.0f);
                }
                else
                {
                    displaySubsystem.textureLayout = XRTextureLayout.Texture2DArray;
                    RenderingSettings.UnityMagicLeap_RenderingSetParameter("SinglePassEnabled", 1.0f);
                }
#else
                if (settings != null && settings.forceMultipass)
                    displaySubsystem.singlePassRenderingDisabled = true;
                else
                    displaySubsystem.singlePassRenderingDisabled = false;
#endif // UNITY_2020_1_OR_NEWER
                StartSubsystem<XRDisplaySubsystem>();
                m_DisplaySubsystemRunning = true;
            }
            return true;
        }

        /// <summary>
        /// Stop all Magic Leap Subsystems.
        /// </summary>
        /// <returns>always returns true.</returns>
        public override bool Stop()
        {
            if (m_DisplaySubsystemRunning)
            {
                StopSubsystem<XRDisplaySubsystem>();
                m_DisplaySubsystemRunning = false;
            }
            if (m_MeshSubsystemRefcount > 0)
            {
                m_MeshSubsystemRefcount = 0;
                StopSubsystem<XRMeshSubsystem>();
            }
            StopSubsystem<XRPlaneSubsystem>();
            StopSubsystem<XRGestureSubsystem>();
            //StopSubsystem<XRAnchorSubsystem>();
            StopSubsystem<XRRaycastSubsystem>();
            StopSubsystem<XRImageTrackingSubsystem>();
            StopSubsystem<XRInputSubsystem>();
            StopSubsystem<XRSessionSubsystem>();
            return true;
        }

        /// <summary>
        /// Teardown (Deinitialize) all subsystems.
        /// </summary>
        /// <returns>Always returns true</returns>
        public override bool Deinitialize()
        {
            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRMeshSubsystem>();
            DestroySubsystem<XRPlaneSubsystem>();
            DestroySubsystem<XRGestureSubsystem>();
            DestroySubsystem<XRImageTrackingSubsystem>();
            DestroySubsystem<XRRaycastSubsystem>();
            //DestroySubsystem<XRAnchorSubsystem>();
            DestroySubsystem<XRInputSubsystem>();
            DestroySubsystem<XRSessionSubsystem>();
            return true;
        }

        internal static bool isLegacyDeviceActive
        {
            get { return XRSettings.enabled && (XRSettings.loadedDeviceName == "Lumin"); }
        }

        internal void StartMeshSubsystem()
        {
            //MagicLeapLogger.Debug(kLogTag, "m_MeshSubsystemRefcount: {0}", m_MeshSubsystemRefcount);
            m_MeshSubsystemRefcount += 1;
            //MagicLeapLogger.Debug(kLogTag, "m_MeshSubsystemRefcount: {0}", m_MeshSubsystemRefcount);
            if (m_MeshSubsystemRefcount == 1)
            {
                MagicLeapLogger.Debug(kLogTag, "Starting Mesh Subsystem");
                StartSubsystem<XRMeshSubsystem>();
            }
        }

        internal void StopMeshSubsystem()
        {
            //MagicLeapLogger.Debug(kLogTag, "m_MeshSubsystemRefcount: {0}", m_MeshSubsystemRefcount);
            if (m_MeshSubsystemRefcount == 0)
                return;

            m_MeshSubsystemRefcount -= 1;
            //MagicLeapLogger.Debug(kLogTag, "m_MeshSubsystemRefcount: {0}", m_MeshSubsystemRefcount);
            if (m_MeshSubsystemRefcount == 0)
            {
                MagicLeapLogger.Debug(kLogTag, "Stopping Mesh Subsystem");
                StopSubsystem<XRMeshSubsystem>();
            }
        }

        private void ApplySettings()
        {
            var settings = MagicLeapSettings.currentSettings;
            if (settings != null)
            {
                // set depth buffer precision
                MagicLeapLogger.Debug(kLogTag, $"Setting Depth Precision: {settings.depthPrecision}");
                Rendering.RenderingSettings.depthPrecision = settings.depthPrecision;
                MagicLeapLogger.Debug(kLogTag, $"Setting Headlocked: {settings.headlockGraphics}");
                Rendering.RenderingSettings.headlocked = settings.headlockGraphics;
            }
        }
    }
#if UNITY_EDITOR
    internal static class XRMangementEditorExtensions
    {
        internal static bool IsEnabledForPlatform(this XRLoader loader, BuildTargetGroup group)
        {
            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(group);
            return settings?.Manager?.activeLoaders?.Contains(loader) ?? false;
        }

        internal static bool IsEnabledForPlatform(this XRLoader loader, BuildTarget target)
        {
            return loader.IsEnabledForPlatform(BuildPipeline.GetBuildTargetGroup(target));
        }
    }
#endif // UNITY_EDITOR
}
