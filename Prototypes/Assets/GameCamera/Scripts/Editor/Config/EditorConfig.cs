// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEditor;
using UnityEngine;
using Debug = RG_GameCamera.Utils.Debug;

namespace RG_GameCamera.Editor.Config
{
    public class EditorConfig : UnityEditor.Editor
    {
        private bool showTransitions;
        private bool valueChanged;
        private bool createOptions;
        private string configName = string.Empty;
        private int removeIndex;

        public override void OnInspectorGUI()
        {
            var config = this.target as RG_GameCamera.Config.Config;

            if (config)
            {
                RG_GameCamera.Utils.Debug.Assert(config);

                if (config.Params == null)
                {
                    config.LoadDefault();
                }

                if (config.Params != null)
                {
                    var modeNames = new string[config.Params.Keys.Count+1];
                    modeNames[0] = "All";
                    config.Params.Keys.CopyTo(modeNames, 1);
                    if (Utils.Selection("Configuration", modeNames, ref config.ModeIndex))
                    {
                        EditorUtility.SetDirty(config);
                    }

                    Utils.Separator("", 1);

                    foreach (var mode in config.Params)
                    {
                        var change = false;

                        // filter not selected modes
                        if (modeNames[config.ModeIndex] != "All" && modeNames[config.ModeIndex] != mode.Key)
                        {
                            continue;
                        }

                        Utils.Separator(mode.Key, 20);

                        foreach (var param in mode.Value)
                        {
                            var key = param.Key;
                            var value = param.Value as RG_GameCamera.Config.Config.Param;
                            RG_GameCamera.Utils.Debug.Assert(value != null);

                            switch (value.Type)
                            {
                                case RG_GameCamera.Config.Config.ConfigValue.Bool:
                                {
                                    var val = (RG_GameCamera.Config.Config.BoolParam)value;
                                    if (Utils.Toggle(key, ref val.value))
                                    {
                                        mode.Value[key] = val;
                                        change = true;
                                    }
                                }
                                break;

                                case RG_GameCamera.Config.Config.ConfigValue.Range:
                                {
                                    var val = (RG_GameCamera.Config.Config.RangeParam)value;
                                    if (Utils.SliderEdit(param.Key, val.min, val.max, ref val.value))
                                    {
                                        mode.Value[key] = val;
                                        change = true;
                                    }
                                }
                                break;

                                case RG_GameCamera.Config.Config.ConfigValue.Selection:
                                {
                                    var val = (RG_GameCamera.Config.Config.SelectionParam)value;
                                    if (Utils.Selection(param.Key, val.value, ref val.index))
                                    {
                                        mode.Value[key] = val;
                                        change = true;
                                    }
                                }
                                break;

                                case RG_GameCamera.Config.Config.ConfigValue.String:
                                {
                                    var val = (RG_GameCamera.Config.Config.StringParam)value;
                                    if (Utils.String(param.Key, ref val.value))
                                    {
                                        mode.Value[key] = val;
                                        change = true;
                                    }
                                }
                                break;

                                case RG_GameCamera.Config.Config.ConfigValue.Vector2:
                                {
                                    var val = (RG_GameCamera.Config.Config.Vector2Param)value;
                                    if (Utils.Vector2(param.Key, ref val.value))
                                    {
                                        mode.Value[key] = val;
                                        change = true;
                                    }
                                }
                                break;

                                case RG_GameCamera.Config.Config.ConfigValue.Vector3:
                                {
                                    var val = (RG_GameCamera.Config.Config.Vector3Param)value;
                                    if (Utils.Vector3(param.Key, ref val.value))
                                    {
                                        mode.Value[key] = val;
                                        change = true;
                                    }
                                }
                                break;
                            }

                            if (change)
                            {
                                valueChanged = true;
                                break;
                            }
                        }

                        Utils.Separator("", 1);
                    }

                    // only show transitions if there are more than one camara modes
                    if (config.Params.Count > 1)
                    {
                        // filter not selected modes
                        Utils.Toggle("Show transitions", ref showTransitions);
                        if (showTransitions)
                        {
                            Utils.Separator("Transitions", 20);
                            foreach (var transition in config.Transitions)
                            {
                                var val = transition.Value;
                                if (Utils.SliderEdit(transition.Key, 0.0f, 2.0f, ref val))
                                {
                                    config.Transitions[transition.Key] = val;
                                    valueChanged = true;
                                    break;
                                }
                            }
                        }
                    }

                    EditorGUILayout.Separator();

                    //Utils.String("Default config file", ref config.DefaultConfigPath);

                    EditorGUILayout.BeginHorizontal();
                    var saved = Utils.SaveDefault(config, valueChanged);
                    var loaded = Utils.LoadDefault(config);
                    EditorGUILayout.EndHorizontal();

                    if (saved || loaded)
                    {
                        valueChanged = false;
                    }

                    EditorGUILayout.BeginHorizontal();
                    Utils.SaveAs(config);
                    Utils.LoadNew(config);
                    EditorGUILayout.EndHorizontal();

                    Utils.Separator(string.Empty, 1);

                    Utils.Toggle("Change configurations", ref createOptions);

                    if (createOptions)
                    {
                        Utils.Separator("Change configurations", 20);

                        EditorGUILayout.BeginHorizontal();
                        if (Utils.Button("Add configuration"))
                        {
                            var valid = !string.IsNullOrEmpty(configName);

                            foreach (var param in config.Params)
                            {
                                if (param.Key == configName)
                                {
                                    valid = false;
                                    break;
                                }
                            }

                            if (!valid)
                            {
                                EditorUtility.DisplayDialog("Error", "Configuration name must be unique and non-empty string!", "OK");
                            }
                            else
                            {
                                valueChanged = true;
                                config.AddMode(configName);
                                configName = string.Empty;
                            }
                        }
                        Utils.String("Name", ref configName);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        var cfgs = new string[config.Params.Keys.Count];
                        config.Params.Keys.CopyTo(cfgs, 0);
                        if (Utils.Button("Remove configuration"))
                        {
                            if (removeIndex > 0)
                            {
                                config.DeleteMode(cfgs[removeIndex]);
                                removeIndex = 0;
                                valueChanged = true;
                            }
                            else
                            {
                                UnityEditor.EditorUtility.DisplayDialog("Error",
                                                                        "Default configurations cannot be removed.", "OK");
                            }
                        }
                        Utils.Selection("", cfgs, ref removeIndex);
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }
    }
}
