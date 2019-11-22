using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.XR.MagicLeap
{
    internal static class PlatformLevelSelector
    {
        public static int SelectorGUI(int value)
        {
            return EditorGUILayout.IntPopup("Minimum API Level",
                EnsureValidValue(value),
                GetChoices().Select(c => $"API Level {c}").ToArray(),
                GetChoices().ToArray());
        }

        public static IEnumerable<int> GetChoices()
        {
            int min = SDKUtility.pluginAPILevel;
            int max = SDKUtility.sdkAPILevel;
            for (int i = min; i <= max; i++)
                yield return i;
        }

        public static int EnsureValidValue(int input)
        {
            var max = GetChoices().Max();
            var min = GetChoices().Min();
            if (input < min)
                return min;
            if (input > max)
                return max;
            return input;
        }
    }
}