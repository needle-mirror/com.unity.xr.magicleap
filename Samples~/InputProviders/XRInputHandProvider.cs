using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;

namespace UnityEngine.XR.MagicLeap.Samples
{
    [AddComponentMenu("AR/Magic Leap/Samples/XR Input Hand Provider")]
    public class XRInputHandProvider : BasePoseProvider
    {
        public enum Hand
        {
            Left,
            Right
        }

        public Hand hand = Hand.Left;

        private InputDeviceCharacteristics m_Characteristics => InputDeviceCharacteristics.HandTracking | ((hand == Hand.Left) ? InputDeviceCharacteristics.Left : InputDeviceCharacteristics.Right);

        public bool TryGetHandDevice(out InputDevice device)
        {
            device = default(InputDevice);

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(m_Characteristics, devices);
            if (devices.Count == 0)
            {
                MagicLeapLogger.Debug("XRInputHandProvider", "Unable to find hand tracking device!");
                return false;
            }
            device = devices.First();
            return true;
        }

        public override bool TryGetPoseFromProvider(out Pose output)
        {
            output = default(Pose);
            var result = false;

            if (!TryGetHandDevice(out var device) || !device.isValid)
                return result;

            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out output.position))
                result = true;
            if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out output.rotation))
                result = true;
            return result;
        }
    }
}