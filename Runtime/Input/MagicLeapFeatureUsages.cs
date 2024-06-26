using UnityEngine.XR;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// Static class for Input Features for Head usages
    /// </summary>
    public static class MagicLeapHeadUsages
    {
        /// <summary>
        /// Input Feature Usage for MLHeadConfidence
        /// </summary>
        public static readonly InputFeatureUsage<float> Confidence = new InputFeatureUsage<float>("MLHeadConfidence");

        /// <summary>
        /// Input Feature Usage for MLFixationConfidence
        /// </summary>
        public static readonly InputFeatureUsage<float> FixationConfidence = new InputFeatureUsage<float>("MLFixationConfidence");
        /// <summary>
        /// Input Feature Usage for MLEyeLeftVenterConfidence
        /// </summary>
        public static readonly InputFeatureUsage<float> EyeLeftCenterConfidence = new InputFeatureUsage<float>("MLEyeLeftCenterConfidence");
        /// <summary>
        /// Input Feature Usage for MLEyeRightConfidence
        /// </summary>
        public static readonly InputFeatureUsage<float> EyeRightCenterConfidence = new InputFeatureUsage<float>("MLEyeRightCenterConfidence");

        /// <summary>
        /// Input Feature Usage for MLEyeCalibrationStatus
        /// </summary>
        public static readonly InputFeatureUsage<uint> EyeCalibrationStatus = new InputFeatureUsage<uint>("MLEyeCalibrationStatus");
    }

    /// <summary>
    /// Static class for Input Features for Controllers
    /// </summary>
    public static class MagicLeapControllerUsages
    {
        /// <summary>
        /// Input Feature Usage for MLControllerType
        /// </summary>
        public static readonly InputFeatureUsage<uint> ControllerType = new InputFeatureUsage<uint>("MLControllerType");
        /// <summary>
        /// Input Feature Usage for MLControllerDOF
        /// </summary>
        public static readonly InputFeatureUsage<uint> ControllerDOF = new InputFeatureUsage<uint>("MLControllerDOF");
        /// <summary>
        /// Input Feature Usage for MLControllerCalibrationAccuracy
        /// </summary>
        public static readonly InputFeatureUsage<uint> ControllerCalibrationAccuracy = new InputFeatureUsage<uint>("MLControllerCalibrationAccuracy");

        /// <summary>
        /// Input Feature Usage for MLControllerTouch1Force
        /// </summary>
        public static readonly InputFeatureUsage<float> ControllerTouch1Force = new InputFeatureUsage<float>("MLControllerTouch1Force");
        /// <summary>
        /// Input Feature Usage for MLControllerTouch2Force
        /// </summary>
        public static readonly InputFeatureUsage<float> ControllerTouch2Force = new InputFeatureUsage<float>("MLControllerTouch2Force");

        /// <summary>
        /// Input Feature Usage for Secondary2DAxisTouch
        /// </summary>
        public static readonly InputFeatureUsage<bool> secondary2DAxisTouch = new InputFeatureUsage<bool>("Secondary2DAxisTouch");
    }

    /// <summary>
    /// Static class for Input Features for hands
    /// </summary>
    public static class MagicLeapHandUsages
    {
        /// <summary>
        /// Input Feature Usage for MLHandConfidence
        /// </summary>
        public static readonly InputFeatureUsage<float> Confidence = new InputFeatureUsage<float>("MLHandConfidence");
        /// <summary>
        /// Input Feature Usage for MLHandNormalizedCenter
        /// </summary>
        public static readonly InputFeatureUsage<Vector3> NormalizedCenter = new InputFeatureUsage<Vector3>("MLHandNormalizedCenter");

        /// <summary>
        /// Input Feature Usage for MLHandWristCenter
        /// </summary>
        public static readonly InputFeatureUsage<Vector3> WristCenter = new InputFeatureUsage<Vector3>("MLHandWristCenter");
        /// <summary>
        /// Input Feature Usage for MLHandWristUlnar
        /// </summary>
        public static readonly InputFeatureUsage<Vector3> WristUlnar = new InputFeatureUsage<Vector3>("MLHandWristUlnar");
        /// <summary>
        /// Input Feature Usage for MLHandWristRadial
        /// </summary>
        public static readonly InputFeatureUsage<Vector3> WristRadial = new InputFeatureUsage<Vector3>("MLHandWristRadial");

        /// <summary>
        /// Input Feature Usage for MLHandKeyPoseConfidence
        /// </summary>
        public static readonly InputFeatureUsage<byte[]> KeyPoseConfidence = new InputFeatureUsage<byte[]>("MLHandKeyPoseConfidence");
        /// <summary>
        /// Input Feature Usage for KeyPoseConfidenceFiltered
        /// </summary>
        public static readonly InputFeatureUsage<byte[]> KeyPoseConfidenceFiltered = new InputFeatureUsage<byte[]>("KeyPoseConfidenceFiltered");
        /// <summary>
        /// Input Feature Usage for KeyPointsMask
        /// </summary>
        public static readonly InputFeatureUsage<byte[]> KeyPointsMask = new InputFeatureUsage<byte[]>("KeyPointsMask");

        /// <summary>
        /// Input Feature Usage for IsHoldingControl
        /// </summary>
        public static readonly InputFeatureUsage<bool> IsHoldingControl = new InputFeatureUsage<bool>("IsHoldingControl");
    }
}