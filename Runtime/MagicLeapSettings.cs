using UnityEngine;
using UnityEngine.XR.Management;

using System;

namespace UnityEngine.XR.MagicLeap
{
    [Serializable]
    [XRConfigurationData("Magic Leap Settings", MagicLeapConstants.kSettingsKey)]
    public class MagicLeapSettings : ScriptableObject
    {
        [SerializeField]
        bool m_EnableOpenGLCache;

        [SerializeField]
        Rendering.FrameTimingHint m_FrameTimingHint;

        public bool enableOpenGLCache
        {
            get { return m_EnableOpenGLCache; }
            set { m_EnableOpenGLCache = value; }
        }
        public Rendering.FrameTimingHint frameTimingHint
        {
            get { return m_FrameTimingHint; }
            set { m_FrameTimingHint = value; }
        }
    }
}