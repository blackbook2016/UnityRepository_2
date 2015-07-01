// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.IO;
using UnityEngine;

namespace RG_GameCamera.Utils
{
    static class GUIUtils
    {
        private static float labelMaxWidth = 130;

        /// <summary>
        /// create slider with edit box for float values
        /// </summary>
        /// <param name="label">name of the slider</param>
        /// <param name="min">minimum value</param>
        /// <param name="max">maximum value</param>
        /// <param name="value">current value</param>
        /// <returns>true if value has been changed</returns>
        public static bool SliderEdit(string label, float min, float max, ref float value)
        {
            GUILayout.BeginHorizontal();
            var oldValue = value;

            GUILayout.Label(label, GUILayout.Width(labelMaxWidth));
            value = GUILayout.HorizontalSlider(value, min, max);

            GUILayout.EndHorizontal();
            value = Mathf.Clamp(value, min, max);
            return oldValue != value;
        }

        /// <summary>
        /// create slider with edit box for int values
        /// </summary>
        /// <param name="label">name of the slider</param>
        /// <param name="min">minimum value</param>
        /// <param name="max">maximum value</param>
        /// <param name="value">current value</param>
        /// <returns>true if value has been changed</returns>
        public static bool SliderEdit(string label, int min, int max, ref int value)
        {
            GUILayout.BeginHorizontal();
            var oldValue = value;
            GUILayout.Label(label);
            value = (int)GUILayout.HorizontalSlider(value, min, max);
            GUILayout.EndHorizontal();

            value = Mathf.Clamp(value, min, max);

            return oldValue != value;
        }

        /// <summary>
        /// create toggle control
        /// </summary>
        /// <param name="label">name of the toggle</param>
        /// <param name="value">bool value of toggle</param>
        /// <returns>true if value has been changed</returns>
        public static bool Toggle(string label, ref bool value)
        {
            var oldValue = value;

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(labelMaxWidth));
            value = GUILayout.Toggle(value, string.Empty);
            GUILayout.EndHorizontal();

            return oldValue != value;
        }

        public static bool Selection(string label, string[] labels, ref int index)
        {
//            var oldValue = index;
//            GUILayout.BeginHorizontal();
//            index = GUILayout.Popup(label, index, labels);
//            GUILayout.EndHorizontal();
//            return index != oldValue;
            return false;
        }

        public static System.Enum EnumSelection(string label, System.Enum selected, ref bool changed)
        {
//            GUILayout.BeginHorizontal();
//            var newValue = GUILayout.EnumPopup(label, selected);
//            GUILayout.EndHorizontal();
//            changed |= newValue != selected;
//            return newValue;
            return null;
        }

        public static bool String(string label, ref string input)
        {
            var oldValue = input;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(labelMaxWidth));
            input = GUILayout.TextField(input);
            GUILayout.EndHorizontal();
            return input != oldValue;
        }

        public static bool Vector2(string label, ref Vector2 input)
        {
            var oldValue = input;
            GUILayout.BeginHorizontal();

            GUILayout.Label(label, GUILayout.Width(labelMaxWidth));
            var inputX = input.x.ToString();
            var x = GUILayout.TextField(inputX);
            var inputY = input.y.ToString();
            var y = GUILayout.TextField(inputY);

            try
            {
                input.x = System.Convert.ToSingle(x);
                input.y = System.Convert.ToSingle(y);
            }
            catch { }

            GUILayout.EndHorizontal();
            return input != oldValue;
        }

        public static bool Vector3(string label, ref Vector3 input)
        {
            var oldValue = input;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(labelMaxWidth));
            var inputX = input.x.ToString();
            var x = GUILayout.TextField(inputX);
            var inputY = input.y.ToString();
            var y = GUILayout.TextField(inputY);
            var inputZ = input.z.ToString();
            var z = GUILayout.TextField(inputZ);

            try
            {
                input.x = System.Convert.ToSingle(x);
                input.y = System.Convert.ToSingle(y);
                input.z = System.Convert.ToSingle(z);
            }
            catch { }

            GUILayout.EndHorizontal();
            return input != oldValue;
        }

        public static void Separator(string label, float height)
        {
            GUILayout.Box(label, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(height) });
        }
    }
}
