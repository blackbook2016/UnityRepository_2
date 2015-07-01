// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RG_GameCamera.Editor.Input
{
    [CustomEditor(typeof(RG_GameCamera.Input.Mobile.MobileControls))]
    public class EditorMobileControls : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var controls = this.target as RG_GameCamera.Input.Mobile.MobileControls;

            if (controls)
            {
                if (ShowPresetPanel(controls))
                {
                    EditorUtility.SetDirty(controls);
                    return;
                }

                var change = false;

                change |= ShowPanelControls(controls, RG_GameCamera.Input.Mobile.ControlSide.Left, ref controls.LeftPanelIndex);
                change |= ShowPanelControls(controls, RG_GameCamera.Input.Mobile.ControlSide.Right, ref controls.RightPanelIndex);
                change |= ShowButtonControls(controls);
                change |= ShowZoomControls(controls);
                change |= ShowRotateControls(controls);
                change |= ShowPanControls(controls);

                if (change)
                {
                    EditorUtility.SetDirty(controls);
                }

                EditorGUILayout.Separator();
            }
        }

        private bool ShowPresetPanel(RG_GameCamera.Input.Mobile.MobileControls controls)
        {
            Utils.Separator("Presets", 20);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Load"))
            {
                var fileName = EditorUtility.OpenFilePanel("Load mobile input preset", "Assets/GameCamera/Resources/Config/MobileLayouts/", "json");

                if (!string.IsNullOrEmpty(fileName))
                {
                    var content = RG_GameCamera.Utils.IO.ReadTextFile(fileName);
                    var dic = RG_GameCamera.ThirdParty.Json.Deserialize(content) as Dictionary<string, object>;
                    controls.Deserialize(dic);

                    return true;
                }
            }

            if (GUILayout.Button("Save"))
            {
                var fileName = EditorUtility.SaveFilePanel("Save mobile input preset", "Assets/GameCamera/Resources/Config/MobileLayouts/", "MobileInput", "json");

                if (!string.IsNullOrEmpty(fileName))
                {
                    var content = controls.Serialize();
                    var json = RG_GameCamera.ThirdParty.Json.Serialize(content);
                    RG_GameCamera.Utils.IO.WriteTextFile(fileName, json);
                }
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            return false;
        }

        bool ShowZoomControls(RG_GameCamera.Input.Mobile.MobileControls controls)
        {
            var change = false;

            Utils.Separator("ZoomArea", 20);

            var controlsList = controls.GetControls();
            if (controlsList != null)
            {
                foreach (var control in controlsList)
                {
                    if (!control)
                    {
                        continue;
                    }

                    var btnChange = false;

                    if (control.Type == RG_GameCamera.Input.Mobile.ControlType.Zoom)
                    {
                        btnChange |= Utils.String("Name", ref control.InputKey0);

                        var cPos = new Vector2(control.Position.x * Screen.width, control.Position.y * Screen.height);
                        btnChange |= Utils.SliderEdit("X", "Y", 0, Screen.width, 0, Screen.height, ref cPos);
                        control.Position = new Vector2(cPos.x / Screen.width, cPos.y / Screen.height);

                        var size = control.Size * 100;
                        btnChange |= Utils.SliderEdit("Width", "Height", 0, 100, 0, 100, ref size);
                        control.Size = size / 100;

                        btnChange |= Utils.SliderEdit("Sensitivity", 0.0f, 2.0f, ref ((RG_GameCamera.Input.Mobile.Zoom)control).Sensitivity);
                        btnChange |= Utils.Toggle("ReverseZoom", ref ((RG_GameCamera.Input.Mobile.Zoom)control).ReverseZoom);
                        btnChange |= Utils.Toggle("HideGUI", ref control.HideGUI);

                        if (Utils.ButtonWithSpace("Remove"))
                        {
                            controls.RemoveControl(control);
                            change = true;
                            break;
                        }

                        Utils.Separator(string.Empty, 1);
                    }

                    if (btnChange)
                    {
                        EditorUtility.SetDirty(control);
                        change = true;
                        break;
                    }
                }
            }

            if (Utils.Button("Create zoom area"))
            {
                var controlLen = controls.GetControls() != null ? controls.GetControls().Length : 0;

                RG_GameCamera.Input.Mobile.Zoom lastButton = null;
                if (controls.GetControls().Length > 0)
                {
                    var ctrls = controls.GetControls();
                    foreach (var baseControl in ctrls)
                    {
                        if (baseControl.Type == RG_GameCamera.Input.Mobile.ControlType.Zoom)
                        {
                            lastButton = (RG_GameCamera.Input.Mobile.Zoom)baseControl;
                        }
                    }
                }

                var btn = controls.CreateZoom("ZoomArea" + controlLen);

                if (lastButton)
                {
                    controls.DuplicateBasicValues(btn, lastButton);
                }
            }

            return change;
        }

        bool ShowRotateControls(RG_GameCamera.Input.Mobile.MobileControls controls)
        {
            var change = false;

            Utils.Separator("RotationArea", 20);

            var controlsList = controls.GetControls();
            if (controlsList != null)
            {
                foreach (var control in controlsList)
                {
                    if (!control)
                    {
                        continue;
                    }

                    var btnChange = false;

                    if (control.Type == RG_GameCamera.Input.Mobile.ControlType.Rotate)
                    {
                        btnChange |= Utils.String("Name", ref control.InputKey0);

                        var cPos = new Vector2(control.Position.x * Screen.width, control.Position.y * Screen.height);
                        btnChange |= Utils.SliderEdit("X", "Y", 0, Screen.width, 0, Screen.height, ref cPos);
                        control.Position = new Vector2(cPos.x / Screen.width, cPos.y / Screen.height);

                        var size = control.Size * 100;
                        btnChange |= Utils.SliderEdit("Width", "Height", 0, 100, 0, 100, ref size);
                        control.Size = size / 100;

                        btnChange |= Utils.SliderEdit("Sensitivity", 0.0f, 2.0f, ref ((RG_GameCamera.Input.Mobile.Rotate)control).Sensitivity);
                        btnChange |= Utils.Toggle("HideGUI", ref control.HideGUI);

                        if (Utils.ButtonWithSpace("Remove"))
                        {
                            controls.RemoveControl(control);
                            change = true;
                            break;
                        }

                        Utils.Separator(string.Empty, 1);
                    }

                    if (btnChange)
                    {
                        EditorUtility.SetDirty(control);
                        change = true;
                        break;
                    }
                }
            }

            if (Utils.Button("Create rotation area"))
            {
                var controlLen = controls.GetControls() != null ? controls.GetControls().Length : 0;

                RG_GameCamera.Input.Mobile.Rotate lastButton = null;
                if (controls.GetControls().Length > 0)
                {
                    var ctrls = controls.GetControls();
                    foreach (var baseControl in ctrls)
                    {
                        if (baseControl.Type == RG_GameCamera.Input.Mobile.ControlType.Rotate)
                        {
                            lastButton = (RG_GameCamera.Input.Mobile.Rotate)baseControl;
                        }
                    }
                }

                var btn = controls.CreateRotation("RotateArea" + controlLen);

                if (lastButton)
                {
                    controls.DuplicateBasicValues(btn, lastButton);
                }
            }

            return change;
        }

        bool ShowPanControls(RG_GameCamera.Input.Mobile.MobileControls controls)
        {
            var change = false;

            Utils.Separator("PanArea", 20);

            var controlsList = controls.GetControls();
            if (controlsList != null)
            {
                foreach (var control in controlsList)
                {
                    if (!control)
                    {
                        continue;
                    }

                    var btnChange = false;

                    if (control.Type == RG_GameCamera.Input.Mobile.ControlType.Pan)
                    {
                        btnChange |= Utils.String("Name", ref control.InputKey0);

                        var cPos = new Vector2(control.Position.x * Screen.width, control.Position.y * Screen.height);
                        btnChange |= Utils.SliderEdit("X", "Y", 0, Screen.width, 0, Screen.height, ref cPos);
                        control.Position = new Vector2(cPos.x / Screen.width, cPos.y / Screen.height);

                        var size = control.Size * 100;
                        btnChange |= Utils.SliderEdit("Width", "Height", 0, 100, 0, 100, ref size);
                        control.Size = size / 100;

                        btnChange |= Utils.SliderEdit("Sensitivity", 0.0f, 2.0f, ref ((RG_GameCamera.Input.Mobile.Pan)control).Sensitivity);
                        btnChange |= Utils.Toggle("Double Pan", ref ((RG_GameCamera.Input.Mobile.Pan)control).DoublePan);
                        btnChange |= Utils.Toggle("HideGUI", ref control.HideGUI);

                        if (Utils.ButtonWithSpace("Remove"))
                        {
                            controls.RemoveControl(control);
                            change = true;
                            break;
                        }

                        Utils.Separator(string.Empty, 1);
                    }

                    if (btnChange)
                    {
                        EditorUtility.SetDirty(control);
                        change = true;
                        break;
                    }
                }
            }

            if (Utils.Button("Create pan area"))
            {
                var controlLen = controls.GetControls() != null ? controls.GetControls().Length : 0;

                RG_GameCamera.Input.Mobile.Pan lastButton = null;
                if (controls.GetControls().Length > 0)
                {
                    var ctrls = controls.GetControls();
                    foreach (var baseControl in ctrls)
                    {
                        if (baseControl.Type == RG_GameCamera.Input.Mobile.ControlType.Pan)
                        {
                            lastButton = (RG_GameCamera.Input.Mobile.Pan)baseControl;
                        }
                    }
                }

                var btn = controls.CreatePan("PanArea" + controlLen);

                if (lastButton)
                {
                    controls.DuplicateBasicValues(btn, lastButton);
                }
            }

            return change;
        }

        bool ShowButtonControls(RG_GameCamera.Input.Mobile.MobileControls controls)
        {
            var change = false;

            Utils.Separator("Buttons", 20);

            var controlsList = controls.GetControls();
            if (controlsList != null)
            {
                foreach (var control in controlsList)
                {
                    if (!control)
                    {
                        continue;
                    }

                    var btnChange = false;

                    if (control.Type == RG_GameCamera.Input.Mobile.ControlType.Button)
                    {
                        btnChange |= Utils.String("Name", ref control.InputKey0);

                        var cPos = new Vector2(control.Position.x * Screen.width, control.Position.y * Screen.height);
                        btnChange |= Utils.SliderEdit("X", "Y", 0, Screen.width, 0, Screen.height, ref cPos);
                        control.Position = new Vector2(cPos.x / Screen.width, cPos.y / Screen.height);

                        var size = control.Size * 100;
                        btnChange |= Utils.SliderEdit("Width", "Height", 0, 100, 0, 100, ref size);
                        control.Size = size / 100;

                        btnChange |= Utils.Toggle("Preserve texture ratio", ref control.PreserveTextureRatio);
                        btnChange |= Utils.Toggle("Toggle", ref ((RG_GameCamera.Input.Mobile.Button)control).Toggle);

                        btnChange |= Utils.Toggle("Invalidate on drag", ref ((RG_GameCamera.Input.Mobile.Button)control).InvalidateOnDrag);

                        btnChange |= Utils.Toggle("HideGUI", ref control.HideGUI);

                        if (Utils.TextureSelection("Default texture",
                                                   ref ((RG_GameCamera.Input.Mobile.Button)control).TextureDefault))
                        {
                            btnChange = true;
                            if (control.Size.sqrMagnitude < Mathf.Epsilon)
                            {
                                var tex = ((RG_GameCamera.Input.Mobile.Button)control).TextureDefault;
                                if (tex)
                                {
                                    control.Size.x = tex.width / (float)Screen.width;
                                    control.Size.y = tex.height / (float)Screen.height;
                                }
                            }
                        }

                        btnChange |= Utils.TextureSelection("Pressed texture",
                                                            ref
                                                                    ((RG_GameCamera.Input.Mobile.Button)control)
                                                                .TexturePressed);

                        var inputGroup = new[] { "CameraMove", "Character", "All", "None" };
                        btnChange |= Utils.Selection("Disable Input Group", inputGroup, ref control.DisableInputGroup);

                        if (Utils.ButtonWithSpace("Remove"))
                        {
                            controls.RemoveControl(control);
                            change = true;
                            break;
                        }

                        if (control.PreserveTextureRatio)
                        {
                            if (control.Size.sqrMagnitude < Mathf.Epsilon)
                            {
                                var tex = ((RG_GameCamera.Input.Mobile.Button)control).TextureDefault;
                                if (tex)
                                {
                                    control.Size.x = tex.width / (float)Screen.width;
                                    control.Size.y = tex.height / (float)Screen.height;
                                    btnChange = true;
                                }
                            }
                            else
                            {
                                var y = control.Size.x * Screen.height / Screen.width;
                                if (y > 1.0f)
                                {
                                    y = 1.0f;
                                }

                                if (Mathf.Abs(control.Size.y - y) > Mathf.Epsilon)
                                {
                                    control.Size.y = y;
                                    btnChange = true;
                                }
                            }
                        }

                        Utils.Separator(string.Empty, 1);
                    }

                    if (btnChange)
                    {
                        EditorUtility.SetDirty(control);
                        change = true;
                        break;
                    }
                }
            }

            if (Utils.Button("Create button"))
            {
                var controlLen = controls.GetControls() != null ? controls.GetControls().Length : 0;

                RG_GameCamera.Input.Mobile.Button lastButton = null;
                if (controls.GetControls().Length > 0)
                {
                    var ctrls = controls.GetControls();
                    foreach (var baseControl in ctrls)
                    {
                        if (baseControl.Type == RG_GameCamera.Input.Mobile.ControlType.Button)
                        {
                            lastButton = (RG_GameCamera.Input.Mobile.Button)baseControl;
                        }
                    }
                }

                var btn = controls.CreateButton("button" + controlLen);

                if (lastButton)
                {
                    controls.DuplicateButtonValues(btn, lastButton);
                }
            }

            return change;
        }

        bool ShowPanelControls(RG_GameCamera.Input.Mobile.MobileControls controls, RG_GameCamera.Input.Mobile.ControlSide side, ref int index)
        {
            Utils.Separator(side.ToString() + " panel", 20);

            var change = false;

            var modeNames = new[] { "None", "Stick", "Camera panel" };
            if (Utils.Selection("Master control", modeNames, ref index))
            {
                switch (index)
                {
                    case 1:
                        controls.CreateMasterControl("Horizontal", "Vertical", RG_GameCamera.Input.Mobile.ControlType.Stick, side);
                        break;

                    case 2:
                        controls.CreateMasterControl("Horizontal_R", "Vertical_R", RG_GameCamera.Input.Mobile.ControlType.CameraPanel, side);
                        break;

                    case 0:
                        controls.RemoveMasterControl(side);
                        break;
                }

                change = true;
            }

            var controlsList = controls.GetControls();
            if (controlsList != null)
            {
                foreach (var control in controlsList)
                {
                    if (!control)
                    {
                        continue;
                    }

                    var btnChange = false;

                    if (control.Side == side)
                    {
                        switch (control.Type)
                        {
                            case RG_GameCamera.Input.Mobile.ControlType.Stick:
                                {
                                    btnChange |= Utils.String("StickAxis0" + control.Side, ref control.InputKey0);
                                    btnChange |= Utils.String("StickAxis1" + control.Side, ref control.InputKey1);
                                    btnChange |= Utils.SliderEdit("CircleSize", 0, 512, ref ((RG_GameCamera.Input.Mobile.Stick)control).CircleSize);
                                    btnChange |= Utils.SliderEdit("HitSize", 0, 512, ref ((RG_GameCamera.Input.Mobile.Stick)control).HitSize);
                                    btnChange |= Utils.SliderEdit("Sensitivity", 0, 1, ref ((RG_GameCamera.Input.Mobile.Stick)control).Sensitivity);
                                    btnChange |= Utils.TextureSelection("ControlCircle",
                                                                        ref ((RG_GameCamera.Input.Mobile.Stick)control).MoveControlsCircle);
                                    btnChange |= Utils.TextureSelection("HitPoint",
                                                                        ref ((RG_GameCamera.Input.Mobile.Stick)control).MoveControlsHit);
                                    btnChange |= Utils.Toggle("HideGUI", ref control.HideGUI);
                                }
                                break;

                            case RG_GameCamera.Input.Mobile.ControlType.CameraPanel:
                                {
                                    btnChange |= Utils.String("CameraAxis0" + control.Side, ref control.InputKey0);
                                    btnChange |= Utils.String("CameraAxis1" + control.Side, ref control.InputKey1);
                                    btnChange |= Utils.SliderEdit("Sensitivity Horizontal", "Sensitivity Vertical", 0, 1,
                                                                  0, 1, ref ((RG_GameCamera.Input.Mobile.CameraPanel)control).Sensitivity);
                                }
                                break;
                        }
                    }

                    if (btnChange)
                    {
                        EditorUtility.SetDirty(control);
                        change = true;
                        break;
                    }
                }
            }

            return change;
        }
    }
}
