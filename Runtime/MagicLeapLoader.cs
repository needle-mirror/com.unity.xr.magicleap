using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine.Experimental;
using UnityEngine.Experimental.XR;
using UnityEngine.XR;
using UnityEngine.XR.Management;

#if UNITY_EDITOR
using System;
using System.Runtime.InteropServices;

using UnityEditor.XR.MagicLeap.Remote;
#endif

namespace UnityEngine.XR.MagicLeap
{
    public sealed class MagicLeapLoader : XRLoaderHelper
    {
        enum Privileges : uint
        {
            ControllerPose = 263,
            GesturesConfig = 269,
            GesturesSubscribe = 268,
            LowLatencyLightwear = 59,
            WorldReconstruction = 33
        }
        const string kLogTag = "MagicLeapLoader";
        static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();
        static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();
        static List<XRMeshSubsystemDescriptor> s_MeshSubsystemDescriptor = new List<XRMeshSubsystemDescriptor>();

        public XRDisplaySubsystem displaySubsystem => GetLoadedSubsystem<XRDisplaySubsystem>();
        public XRInputSubsystem inputSubsystem => GetLoadedSubsystem<XRInputSubsystem>();
        public XRMeshSubsystem meshSubsystem => GetLoadedSubsystem<XRMeshSubsystem>();

        private bool m_DisplaySubsystemRunning = false;
        private int m_MeshSubsystemRefcount = 0;

        public override bool Initialize()
        {
#if UNITY_EDITOR
            if (!MagicLeapRemoteManager.Initialize())
                return false;
#endif // UNITY_EDITOR
            MagicLeapPrivileges.Initialize();

            // TODO :: Handle settings

            // Display Subsystem depends on Input Subsystem, so initialize that first.
            CheckForInputRelatedPermissions();
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, "MagicLeap-Input");
            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(s_DisplaySubsystemDescriptors, "MagicLeap-Display");

            if (CanCreateMeshSubsystem())
                CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(s_MeshSubsystemDescriptor, "MagicLeap-Mesh");

            return true;
        }

        public override bool Start()
        {
            StartSubsystem<XRInputSubsystem>();

            if (!isLegacyDeviceActive)
            {
                StartSubsystem<XRDisplaySubsystem>();
                m_DisplaySubsystemRunning = true;
            }
            return true;
        }

        public override bool Stop()
        {
            if (m_DisplaySubsystemRunning)
            {
                StopSubsystem<XRDisplaySubsystem>();
                m_DisplaySubsystemRunning = false;
            }
            StopSubsystem<XRInputSubsystem>();
            return true;
        }

        public override bool Deinitialize()
        {
            DestroySubsystem<XRMeshSubsystem>();
            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRInputSubsystem>();
            MagicLeapPrivileges.Shutdown();
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
            m_MeshSubsystemRefcount -= 1;
            //MagicLeapLogger.Debug(kLogTag, "m_MeshSubsystemRefcount: {0}", m_MeshSubsystemRefcount);
            if (m_MeshSubsystemRefcount == 0)
            {
                MagicLeapLogger.Debug(kLogTag, "Stopping Mesh Subsystem");
                StopSubsystem<XRMeshSubsystem>();
            }
        }

        [Conditional("DEVELOPMENT_BUILD")]
        private void CheckForInputRelatedPermissions()
        {
            if (!MagicLeapPrivileges.IsPrivilegeApproved((uint)Privileges.ControllerPose))
                Debug.LogWarning("No controller privileges specified; Controller data will not be available via XRInput Subsystem!");
            if (!(MagicLeapPrivileges.IsPrivilegeApproved((uint)Privileges.GesturesConfig) && MagicLeapPrivileges.IsPrivilegeApproved((uint)Privileges.GesturesSubscribe)))
                Debug.LogWarning("No gestures privileges specified; Gesture and Hand data will not be available via XRInput Subsystem!");
        }

        private bool CanCreateMeshSubsystem()
        {
            if (MagicLeapPrivileges.IsPrivilegeApproved((uint)Privileges.WorldReconstruction))
                return true;
#if DEVELOPMENT_BUILD
            Debug.LogError("Unable to create Mesh Subsystem due to missing 'WorldReconstruction' privilege. Please add to manifest");
#endif // DEVELOPMENT_BUILD
            return false;
        }
    }
}