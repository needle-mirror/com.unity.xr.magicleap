using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;

namespace UnityEngine.XR.MagicLeap.Samples
{
    [AddComponentMenu("AR/Magic Leap/Samples/XR Input Eye Tracking Provider")]
    public class XRInputEyeTrackingProvider : BasePoseProvider
    {
        public enum TrackingInformation
        {
            FixationPoint,
            LeftEye,
            RightEye
        }

        public TrackingInformation trackingInformation;

        public override bool TryGetPoseFromProvider(out Pose output)
        {
            output = default(Pose);
            var device = default(InputDevice);
            var result = false;

            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.EyeTracking | InputDeviceCharacteristics.HeadMounted, devices);
            if (devices.Count == 0)
            {
                MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to find a valid eye tracking device!");
                return result;
            }
            device = devices.First();

            if (!device.isValid)
                return result;

            var eyes = default(Eyes);
            if (!device.TryGetFeatureValue(CommonUsages.eyesData, out eyes))
            {
                MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read eye tracking data!");
                return result;
            }

            switch (trackingInformation)
            {
                case TrackingInformation.FixationPoint:
                {
                    if (eyes.TryGetFixationPoint(out output.position))
                        result = true;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read fixation point!");
                    break;
                }
                case TrackingInformation.LeftEye:
                {
                    if (eyes.TryGetLeftEyePosition(out output.position))
                        result = true;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read left eye position!");
                    if (eyes.TryGetLeftEyeRotation(out output.rotation))
                        result = true;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read left eye rotation!");
                    break;
                }
                case TrackingInformation.RightEye:
                {
                    if (eyes.TryGetRightEyePosition(out output.position))
                        result = true;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read right eye position!");
                    if (eyes.TryGetRightEyeRotation(out output.rotation))
                        result = true;
                    else
                        MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Unable to read right eye rotation!");
                    break;
                }
            }

            return result;
        }

        void OnEnable()
        {
            // enable ML Eye tracking if it's not already enabled
            var list = new List<XRInputSubsystem>();
            SubsystemManager.GetInstances(list);
            if (list.Count > 0)
            {
                if (!list[0].IsEyeTrackingApiEnabled())
                {
                    MagicLeapLogger.Debug("XRInputEyeTrackingProvider", "Enabling Eye Tracking!");
                    list[0].SetEyeTrackingApiEnabled(true);
                }
            }
        }
    }
}