using System;
using System.IO;
using System.Runtime.InteropServices;

using UnityEditor;

namespace UnityEditor.XR.MagicLeap
{
    internal static class SDKUtility
    {
        const string kManifestPath = ".metadata/sdk.manifest";
        const string kRemoteLauncher = "VirtualDevice/bin/UIFrontend/MLRemote";

        static class Native
        {
            const string Library = "UnityMagicLeap";

            [DllImport("UnityMagicLeap", EntryPoint = "UnityMagicLeap_PlatformGetAPILevel")]
            public static extern uint GetAPILevel();
        }

        internal static string hostBinaryExtension
        {
            get
            {
#if UNITY_EDITOR_WIN
                return ".exe";
#else
                return "";
#endif
            }
        }

        internal static bool isCompatibleSDK
        {
            get
            {
                var min = pluginAPILevel;
                var max = sdkAPILevel;
                return min <= max;
            }
        }
        internal static int pluginAPILevel
        {
            get
            {
                return (int)Native.GetAPILevel();
            }
        }
        internal static bool remoteLauncherAvailable
        {
            get
            {
                if (!sdkAvailable)
                    return false;
                var launcher = Path.ChangeExtension(Path.Combine(sdkPath, kRemoteLauncher), hostBinaryExtension);
                return File.Exists(launcher);
            }
        }
        internal static int sdkAPILevel
        {
            get
            {
                return PrivilegeParser.ParsePlatformLevelFromHeader(Path.Combine(SDKUtility.sdkPath, PrivilegeParser.kPlatformHeaderPath));
            }
        }
        internal static bool sdkAvailable
        {
            get
            {
                if (string.IsNullOrEmpty(sdkPath)) return false;
                return File.Exists(Path.Combine(sdkPath, kManifestPath));
            }
        }
        internal static string sdkPath
        {
            get
            {
                return EditorPrefs.GetString("LuminSDKRoot", null);
            }
        }
    }
}