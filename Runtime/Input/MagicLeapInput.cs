using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.XR;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// Monobehaviour for a Magic Leap Controller
    /// </summary>
    [AddComponentMenu("AR/Magic Leap/MagicLeap Input")]
    public sealed class MagicLeapInput : MonoBehaviour
    {
    }

    /// <summary>
    /// Input extensions for Magic Leap
    /// </summary>
    public static class MagicLeapInputExtensions
    {
        static class Native
        {
#if UNITY_ANDROID
            const string Library = "UnityMagicLeap";

            [DllImport(Library, EntryPoint="UnityMagicLeap_InputGetControllerTrackerActive")]
            [return: MarshalAs(UnmanagedType.I1)]
            /// <summary>
            /// C# interface to native method.
            /// Check to see if the controller is active
            /// </summary>
            public static extern bool GetControllerActive();

            [DllImport(Library, EntryPoint="UnityMagicLeap_InputSetControllerTrackerActive")]
            /// <summary>
            /// C# interface to native method.
            /// Set the active state of a controller
            /// </summary>
            public static extern void SetControllerActive([MarshalAs(UnmanagedType.I1)]bool value);

            [DllImport(Library, EntryPoint="UnityMagicLeap_InputGetEyeTrackerActive")]
            [return: MarshalAs(UnmanagedType.I1)]
            /// <summary>
            /// C# interface to native method.
            /// Get the active state of the Eye tracker
            /// </summary>
            public static extern bool GetEyeTrackerActive();

            [DllImport(Library, EntryPoint="UnityMagicLeap_InputSetEyeTrackerActive")]
            /// <summary>
            /// C# interface to native method.
            /// Set the active state of the eye tracker
            /// </summary>
            public static extern void SetEyeTrackerActive([MarshalAs(UnmanagedType.I1)]bool value);
#else
            /// <summary>
            /// default, unbound method
            /// Always returns false
            /// </summary>
            public static bool GetControllerActive() => false;
            /// <summary>
            /// default, unbound method
            /// Noop
            /// </summary>
            public static void SetControllerActive(bool value) { /* Dummy for non-Android Compilation */ }
            /// <summary>
            /// default, unbound method
            /// Always returns false
            /// </summary>
            public static bool GetEyeTrackerActive() => false;
            /// <summary>
            /// default, unbound method
            /// Noop
            /// </summary>
            public static void SetEyeTrackerActive(bool value) { /* Dummy for non-Android Compilation */ }

#endif // UNITY_ANDROID
        }
        
        /// <summary>
        /// True if the controller is/should be enabled
        /// </summary>
        public static bool controllerEnabled
        {
            get
            {
                return Native.GetControllerActive();
            }
            set
            {
                Native.SetControllerActive(value);
            }
        }

        /// <summary>
        /// True if eye tracking is/should be enabled
        /// </summary>
        public static bool eyeTrackingEnabled
        {
            get
            {
                return Native.GetEyeTrackerActive();
            }
            set
            {
                Native.SetEyeTrackerActive(value);
            }
        }
    }

    /// <summary>
    /// Utility static class
    /// </summary>
    public static class MagicLeapInputUtility
    {
        /// <summary>
        /// Parse a byte array, aligned on 4 byte boundary, as a list of floatd
        /// </summary>
        /// <param name="input">4 byte aligned stream of bytes</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">No input detected.</exception>
        /// <exception cref="ArgumentException">Byte stream does not align on a 4 byte boundary.</exception>
        public static float[] ParseData(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if ((input.Length % 4) != 0)
                throw new ArgumentException("malformed input array; incorrect number of bytes");

            var list = new List<float>();

            for (int i = 0; i < input.Length; i += 4)
            {
                list.Add(BitConverter.ToSingle(input, i));
            }
            return list.ToArray();
        }
    }
}
