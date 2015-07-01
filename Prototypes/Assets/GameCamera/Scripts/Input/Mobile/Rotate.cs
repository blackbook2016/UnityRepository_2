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
    /// mobile button is a rectangural button with custom textures and toggle option
    /// </summary>
    public class Rotate : BaseControl
    {
        public float RotateAngle;
        public float Sensitivity = 1.0f;

        private Rect rect;
        private Vector3 lastVector;

        public override ControlType Type
        {
            get { return ControlType.Rotate; }
        }

        public override void Init(TouchProcessor processor)
        {
            base.Init(processor);
            rect = new Rect();
            UpdateRect();
            RotateAngle = 0.0f;
            Side = ControlSide.Arbitrary;

            Priority = 2;
        }

        public override Dictionary<string, object> SerializeJSON()
        {
            var baseDic = base.SerializeJSON();
            baseDic.Add("Sensitivity", Sensitivity);

            return baseDic;
        }

        public override void DeserializeJSON(Dictionary<string, object> jsonDic)
        {
            base.DeserializeJSON(jsonDic);
            Sensitivity = Convert.ToSingle(jsonDic["Sensitivity"]);
        }

        public bool ContainPoint(Vector2 point)
        {
            point.y = Screen.height - point.y;
            return rect.Contains(point);
        }

        public override bool AbortUpdateOtherControls()
        {
            return false;
        }

        protected override void DetectTouches()
        {
            var touches = touchProcessor.GetActiveTouchCount();
            var endPress = false;

            if (touches > 1)
            {
                if (!Active)
                {
                    // check for button press
                    for (var i = 0; i < touches; i++)
                    {
                        var touch = touchProcessor.GetTouch(i);

                        if (ContainPoint(touch.StartPosition))
                        {
                            if (touch.Status != TouchStatus.Invalid)
                            {
                                if (TouchIndex == -1)
                                {
                                    TouchIndex = i;
                                }
                                else if (TouchIndexAux == -1)
                                {
                                    TouchIndexAux = i;
                                }
                            }
                        }
                    }

                    Active = TouchIndex != -1 && TouchIndexAux != -1;
                }
                else
                {
                    var t0 = touchProcessor.GetTouch(TouchIndex);
                    var t1 = touchProcessor.GetTouch(TouchIndexAux);

                    if (t0.Status != TouchStatus.Invalid && t1.Status != TouchStatus.Invalid)
                    {
                        var v1 = (t1.Position - t0.Position).normalized;
                        var v0 = lastVector;

                        var t = 5.0f;

                        if (lastVector.x == float.MaxValue)
                        {
                            v0 = v1;
                            t = float.MaxValue;
                        }

                        var rot = (Mathf.Atan2(v1.y, v1.x) - Mathf.Atan2(v0.y, v0.x)) * 20 * Sensitivity;

                        RotateAngle = Mathf.Lerp(RotateAngle, rot, Time.deltaTime * 2.0f);

                        if (t == float.MaxValue)
                        {
                            RotateAngle = rot;
                        }

                        RotateAngle = rot;

                        lastVector = v1;
                    }
                }
            }
            else
            {
                endPress = true;
            }

            if (endPress)
            {
                lastVector.x = float.MaxValue;
                Active = false;
                TouchIndex = -1;
                TouchIndexAux = -1;
                RotateAngle = 0.0f;
            }
        }

        public override void GameUpdate()
        {
            base.GameUpdate();
            DetectTouches();
        }

        public override void Draw()
        {
            UpdateRect();

            if (HideGUI)
            {
                return;
            }

            GUI.Box(rect, "Rotate area");
        }

        public void UpdateRect()
        {
            rect.x = Position.x * Screen.width;
            rect.y = Position.y * Screen.height;
            rect.width = Size.x * Screen.width;
            rect.height = Size.y * Screen.height;
        }
    }
}
