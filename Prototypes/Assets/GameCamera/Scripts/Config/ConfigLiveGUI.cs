// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using RG_GameCamera.Utils;
using UnityEngine;

namespace RG_GameCamera.Config
{
    public abstract partial class Config
    {
        /// <summary>
        /// enable/disable live GUI in game window
        /// </summary>
        public void EnableLiveGUI(bool status)
        {
            enableLiveGUI = status;
        }

        private bool enableLiveGUI = false;
        private Vector2 scrolling;
        private Vector2 WindowPos = new Vector2(10, 10);
        private Vector2 WindowSize = new Vector2(400, 800);
        private int modeIndex;
        private bool showTransitions;

        void OnGUI()
        {
            if (!enableLiveGUI)
            {
                return;
            }

            var height = WindowSize.y;
            var width = WindowSize.x;

            if (height > Screen.height)
            {
                height = Screen.height;
            }

            if (width > Screen.width)
            {
                width = Screen.width;
            }

            // apply gui skin
            var skin = CameraManager.Instance.GuiSkin;

            if (skin)
            {
                GUI.skin = skin;
            }

            GUILayout.Window(0, new Rect(Screen.width - width - WindowPos.x, WindowPos.y, width, height), GUIWindow, "Live GUI");
        }

        private void GUIWindow(int id)
        {
            if (Params != null)
            {
                scrolling = GUILayout.BeginScrollView(scrolling);

                var modeNames = new string[Params.Keys.Count + 1];
                modeNames[0] = "All";
                Params.Keys.CopyTo(modeNames, 1);

                GUIUtils.Selection("Show modes", modeNames, ref modeIndex);

                foreach (var mode in Params)
                {
                    var change = false;

                    // filter not selected modes
                    if (modeNames[modeIndex] != "All" && modeNames[modeIndex] != mode.Key)
                    {
                        continue;
                    }

                    GUIUtils.Separator(mode.Key, 23);

                    foreach (var param in mode.Value)
                    {
                        var key = param.Key;
                        var value = param.Value;
                        Utils.Debug.Assert(value != null);

                        switch (value.Type)
                        {
                            case ConfigValue.Bool:
                            {
                                var val = (Config.BoolParam)value;
                                if (GUIUtils.Toggle(key, ref val.value))
                                {
                                    mode.Value[key] = val;
                                    change = true;
                                }
                            }
                            break;

                            case Config.ConfigValue.Range:
                            {
                                var val = (Config.RangeParam)value;
                                if (GUIUtils.SliderEdit(param.Key, val.min, val.max, ref val.value))
                                {
                                    mode.Value[key] = val;
                                    change = true;
                                }
                            }
                            break;

                            case Config.ConfigValue.Selection:
                            {
                                var val = (Config.SelectionParam)value;
                                if (GUIUtils.Selection(param.Key, val.value, ref val.index))
                                {
                                    mode.Value[key] = val;
                                    change = true;
                                }
                            }
                            break;

                            case Config.ConfigValue.String:
                            {
                                var val = (Config.StringParam)value;
                                if (GUIUtils.String(param.Key, ref val.value))
                                {
                                    mode.Value[key] = val;
                                    change = true;
                                }
                            }
                            break;

                            case Config.ConfigValue.Vector2:
                            {
                                var val = (Config.Vector2Param)value;
                                if (GUIUtils.Vector2(param.Key, ref val.value))
                                {
                                    mode.Value[key] = val;
                                    change = true;
                                }
                            }
                            break;

                            case Config.ConfigValue.Vector3:
                            {
                                var val = (Config.Vector3Param)value;
                                if (GUIUtils.Vector3(param.Key, ref val.value))
                                {
                                    mode.Value[key] = val;
                                    change = true;
                                }
                            }
                            break;
                        }

                        if (change)
                        {
                            break;
                        }
                    }
                }

                // only show transitions if there are more than one camara modes
                if (Params.Count > 1)
                {
                    if (!showTransitions)
                    {
                        GUIUtils.Separator("", 1);
                    }

                    // filter not selected modes
                    GUIUtils.Toggle("Show transitions", ref showTransitions);
                    if (showTransitions)
                    {
                        GUIUtils.Separator("Transitions", 23);
                        foreach (var transition in Transitions)
                        {
                            var val = transition.Value;
                            if (GUIUtils.SliderEdit(transition.Key, 0.0f, 2.0f, ref val))
                            {
                                Transitions[transition.Key] = val;
                                break;
                            }
                        }
                    }
                }

                GUILayout.EndScrollView();
            }
        }
    }
}
