// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Input.Mobile
{
    /// <summary>
    /// camera panel is a rectangle on screen (usually half of the mobile screen) used for camera movement
    /// </summary>
    public class CameraPanel : BaseControl
    {
        public Vector2 Sensitivity = new Vector2(0.5f, 0.5f);
        private Rect rect;
        private InputFilter cameraFilter;
        private Vector2 input;

        public override ControlType Type
        {
            get { return ControlType.CameraPanel; }
        }

        public override void Init(TouchProcessor processor)
        {
            base.Init(processor);
            cameraFilter = new InputFilter(10, 0.5f);
            rect = new Rect();
            UpdateRect();
        }

        public override Dictionary<string, object> SerializeJSON()
        {
            var baseDic = base.SerializeJSON();
            baseDic.Add("SensitivityX", Sensitivity.x);
            baseDic.Add("SensitivityY", Sensitivity.y);

            return baseDic;
        }

        public override void DeserializeJSON(Dictionary<string, object> jsonDic)
        {
            base.DeserializeJSON(jsonDic);
            Sensitivity.x = Convert.ToSingle(jsonDic["SensitivityX"]);
            Sensitivity.y = Convert.ToSingle(jsonDic["SensitivityY"]);
        }

        public override void GameUpdate()
        {
            DetectTouches();

            input = Vector2.zero;

            if (TouchIndex != -1)
            {
                var cameraTouch = touchProcessor.GetTouch(TouchIndex);

                if (cameraTouch.Status != TouchStatus.Invalid)
                {
                    var sample = cameraTouch.DeltaPosition;
                    sample.x *= Sensitivity.x;
                    sample.y *= Sensitivity.y;

                    cameraFilter.AddSample(sample);
                    input = cameraFilter.GetValue();
                }
                else
                {
                    TouchIndex = -1;
                }
            }
        }

        public override Vector2 GetInputAxis()
        {
            return input;
        }

        public void UpdateRect()
        {
            rect.x = Position.x * Screen.width;
            rect.y = Position.y * Screen.height;
            rect.width = Position.x * Screen.width;
            rect.height = Position.y * Screen.height;
        }

        public override void Draw()
        {
            if (HideGUI)
            {
                return;
            }
        }
    }
}
