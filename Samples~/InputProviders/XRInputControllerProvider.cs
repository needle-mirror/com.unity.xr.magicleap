using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
#if LIH_2_OR_NEWER
using UnityEngine.SpatialTracking;
#endif
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;

namespace UnityEngine.XR.MagicLeap.Samples
{
    [AddComponentMenu("AR/Magic Leap/Samples/XR Input Controller Provider")]
    public class XRInputControllerProvider : BasePoseProvider
    {
        const string kLogTag = "XRInputControllerProvider";
        [Range(0,1)]
        public int controllerIndex;

        [SerializeField]
        private bool m_LogTrackedPosition;

        public bool logTrackedPosition
        {
            get { return m_LogTrackedPosition; }
            set { m_LogTrackedPosition = value; }
        }

#if LIH_2_OR_NEWER
        public override PoseDataFlags GetPoseFromProvider(out Pose output)
        {
            output = default(Pose);
            var device = default(InputDevice);
            var flags = PoseDataFlags.NoData;

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand, devices);
            if (devices.Count == 0 && devices.Count <= controllerIndex)
            {
                MagicLeapLogger.Debug(kLogTag, "Unable to find a valid controller device!");
                return flags;
            }
            device = devices.ElementAt(controllerIndex);

            if (!device.isValid)
                return flags;

            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out output.position))
                flags = flags | PoseDataFlags.Position;
            if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out output.rotation))
                flags = flags | PoseDataFlags.Rotation;

            if (logTrackedPosition)
                MagicLeapLogger.Debug(kLogTag, $"pos: {output.position}, rot: {output.rotation}");

            return flags;
        }
#else
        public override bool TryGetPoseFromProvider(out Pose output)
        {
            output = default(Pose);
            var device = default(InputDevice);
            var flags = false;

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HeldInHand, devices);
            if (devices.Count == 0 && devices.Count <= controllerIndex)
            {
                MagicLeapLogger.Debug(kLogTag, "Unable to find a valid controller device!");
                return flags;
            }
            device = devices.ElementAt(controllerIndex);

            if (!device.isValid)
                return flags;

            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out output.position))
                flags = true;
            if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out output.rotation))
                flags = true;

            if (logTrackedPosition)
                MagicLeapLogger.Debug(kLogTag, $"pos: {output.position}, rot: {output.rotation}");

            return flags;
        }
#endif // LIH_2_OR_NEWER
    }
}
