// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.IO;
using RG_GameCamera.Utils;
using UnityEditor;
using UnityEngine;

namespace RG_GameCamera.Editor
{
    static class Utils
    {
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
            EditorGUILayout.BeginHorizontal();
            var oldValue = value;
            value = EditorGUILayout.FloatField(label, value);
            value = GUILayout.HorizontalSlider(value, min, max);
            EditorGUILayout.EndHorizontal();

            value = Mathf.Clamp(value, min, max);

            return oldValue != value;
        }

        /// <summary>
        /// create slider with edit box for Vector2 values
        /// </summary>
        /// <param name="label0">name of the first slider</param>
        /// <param name="label1">name of the second slider</param>
        /// <param name="min">minimum value</param>
        /// <param name="max">maximum value</param>
        /// <param name="value">current value</param>
        /// <returns>true if value has been changed</returns>
        public static bool SliderEdit(string label0, string label1, float min0, float max0, float min1, float max1, ref Vector2 value)
        {
            var oldValue = value;
            EditorGUILayout.BeginHorizontal();
            value.x = EditorGUILayout.FloatField(label0, value.x);
            value.x = GUILayout.HorizontalSlider(value.x, min0, max0);
            EditorGUILayout.EndHorizontal();
            value.x = Mathf.Clamp(value.x, min0, max0);

            EditorGUILayout.BeginHorizontal();
            value.y = EditorGUILayout.FloatField(label1, value.y);
            value.y = GUILayout.HorizontalSlider(value.y, min1, max1);
            EditorGUILayout.EndHorizontal();
            value.y = Mathf.Clamp(value.y, min1, max1);

            return oldValue != value;
        }

        public static bool Button(string label)
        {
            GUILayout.BeginHorizontal();
            var val = GUILayout.Button(label);
            GUILayout.EndHorizontal();
            return val;
        }

        public static bool ButtonWithSpace(string label)
        {
            GUILayout.BeginHorizontal();
            var val = GUILayout.Button(label);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.EndHorizontal();
            return val;
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
            EditorGUILayout.BeginHorizontal();
            var oldValue = value;
            value = EditorGUILayout.IntField(label, value);
            value = (int)GUILayout.HorizontalSlider(value, min, max);
            EditorGUILayout.EndHorizontal();

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

            EditorGUILayout.BeginHorizontal();
            value = EditorGUILayout.Toggle(label, value);
            EditorGUILayout.EndHorizontal();

            return oldValue != value;
        }

        /// <summary>
        /// create game object selection box
        /// </summary>
        /// <param name="label">name of the box</param>
        /// <param name="obj">game object to select</param>
        /// <returns>true if value has been changed</returns>
        public static bool GameObjectSelection(string label, ref GameObject obj)
        {
            var oldValue = obj;
            EditorGUILayout.BeginHorizontal();
            obj = (GameObject)EditorGUILayout.ObjectField(label, obj, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            return oldValue != obj;
        }

        /// <summary>
        /// create game object selection box
        /// </summary>
        /// <param name="label">name of the box</param>
        /// <param name="obj">game object to select</param>
        /// <returns>true if value has been changed</returns>
        public static bool TransformSelection(string label, ref Transform obj)
        {
            var oldValue = obj;
            EditorGUILayout.BeginHorizontal();
            obj = (Transform)EditorGUILayout.ObjectField(label, obj, typeof(Transform), true);
            EditorGUILayout.EndHorizontal();
            return oldValue != obj;
        }

        /// <summary>
        /// create game object selection box
        /// </summary>
        /// <param name="label">name of the box</param>
        /// <param name="obj">game object to select</param>
        /// <returns>true if value has been changed</returns>
        public static bool TextureSelection(string label, ref Texture2D obj)
        {
            var oldValue = obj;
            EditorGUILayout.BeginHorizontal();
            obj = (Texture2D)EditorGUILayout.ObjectField(label, obj, typeof(Texture2D), true);
            EditorGUILayout.EndHorizontal();
            return oldValue != obj;
        }

        public static bool Selection(string label, string[] labels, ref int index)
        {
            var oldValue = index;
            EditorGUILayout.BeginHorizontal();
            index = EditorGUILayout.Popup(label, index, labels);
            EditorGUILayout.EndHorizontal();
            return index != oldValue;
        }

        public static System.Enum EnumSelection(string label, System.Enum selected, ref bool changed)
        {
            EditorGUILayout.BeginHorizontal();
            var newValue = EditorGUILayout.EnumPopup(label, selected);
            EditorGUILayout.EndHorizontal();
            changed |= newValue != selected;
            return newValue;
        }

        public static bool String(string label, ref string input)
        {
            var oldValue = input;
            EditorGUILayout.BeginHorizontal();
            input = EditorGUILayout.TextField(label, input);
            EditorGUILayout.EndHorizontal();
            return input != oldValue;
        }

        public static bool Vector2(string label, ref Vector2 input)
        {
            var oldValue = input;
            EditorGUILayout.BeginHorizontal();
            input = EditorGUILayout.Vector2Field(label, input);
            EditorGUILayout.EndHorizontal();
            return input != oldValue;
        }

        public static bool Vector3(string label, ref Vector3 input)
        {
            var oldValue = input;
            EditorGUILayout.BeginHorizontal();
            input = EditorGUILayout.Vector3Field(label, input);
            EditorGUILayout.EndHorizontal();
            return input != oldValue;
        }

        public static bool SaveDefault(RG_GameCamera.Config.Config config, bool warning)
        {
            GUILayout.BeginHorizontal();
            var saved = false;

            var defaultColor = GUI.color;
            if (warning)
            {
                GUI.color = Color.red;
            }

            if (GUILayout.Button("Save"))
            {
                config.Serialize(config.DefaultConfigPath);
                CopyConfigToResource(config, config.DefaultConfigPath);
//                config.RefreshResourceAsset();
                saved = true;
            }

            GUI.color = defaultColor;

            GUILayout.EndHorizontal();

            return saved;
        }

        public static bool LoadDefault(RG_GameCamera.Config.Config config)
        {
            var loaded = false;
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Load"))
            {
                config.Deserialize(config.DefaultConfigPath);
                CopyConfigToResource(config, config.DefaultConfigPath);
                config.RefreshResourceAsset();
                loaded = true;
            }

            GUILayout.EndHorizontal();
            return loaded;
        }

        public static void LoadNew(RG_GameCamera.Config.Config config)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Load new"))
            {
                var path = EditorUtility.OpenFilePanel("Load config", "", "json");

                if (path.Length != 0)
                {
                    config.Deserialize(path);
                    CopyConfigToResource(config, path);
                    config.RefreshResourceAsset();
                }
            }

            GUILayout.EndHorizontal();
        }

        public static void SaveAs(RG_GameCamera.Config.Config config)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save as"))
            {
                var path = EditorUtility.SaveFilePanel("Save config", "", config.GetType().Name + ".json", "json");

                if (path.Length != 0)
                {
                    config.Serialize(path);
                }
            }

            GUILayout.EndHorizontal();
        }

        public static void Separator(string label, float height)
        {
            GUILayout.Box(label, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(height) });
        }

        public static void Separator(string label, float height, GUIStyle style)
        {
            GUILayout.Box(label, style, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(height) });
        }

        public static void CopyConfigToResource(RG_GameCamera.Config.Config cfg, string file)
        {
            var destFile = cfg.ResourceDir + IO.GetFileName(file);

            if (file != destFile)
            {
                if (!Directory.Exists(cfg.ResourceDir))
                {
                    Directory.CreateDirectory(cfg.ResourceDir);
                }

                IO.CopyFile(file, destFile, true);
            }

            // refresh unity editor
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}
