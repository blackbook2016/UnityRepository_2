// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections.Generic;
using RG_GameCamera.ThirdParty;
using UnityEngine;

namespace RG_GameCamera.Input.Mobile
{
    /// <summary>
    /// mobile button is a rectangural button with custom textures and toggle option
    /// </summary>
    public class Zoom : BaseControl
    {
        public float ZoomDelta;
        public float Sensitivity = 1.0f;
        public bool ReverseZoom;

        private Rect rect;
        private float lastDistance;

        public override ControlType Type
        {
            get { return ControlType.Zoom; }
        }

        public override void Init(TouchProcessor processor)
        {
            base.Init(processor);
            rect = new Rect();
            UpdateRect();
            ZoomDelta = 0.0f;
            Side = ControlSide.Arbitrary;

            Priority = 2;
        }

        public override Dictionary<string, object> SerializeJSON()
        {
            var baseDic = base.SerializeJSON();
            baseDic.Add("Sensitivity", Sensitivity);
            baseDic.Add("ReverseZoom", ReverseZoom);

            return baseDic;
        }

        public override void DeserializeJSON(Dictionary<string, object> jsonDic)
        {
            base.DeserializeJSON(jsonDic);
            Sensitivity = Convert.ToSingle(jsonDic["Sensitivity"]);
            ReverseZoom = Convert.ToBoolean(jsonDic["ReverseZoom"]);
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

                                    var t0 = touchProcessor.GetTouch(TouchIndex);
                                    var t1 = touchProcessor.GetTouch(TouchIndexAux);
                                    lastDistance = (t0.Position - t1.Position).magnitude;
                                }
                            }
                        }
                    }

                    Active = TouchIndex != -1 && TouchIndexAux != -1;
                    OperationTimer = 0.0f;
                }
                else
                {
                    var t0 = touchProcessor.GetTouch(TouchIndex);
                    var t1 = touchProcessor.GetTouch(TouchIndexAux);

                    if (t0.Status != TouchStatus.Invalid && t1.Status != TouchStatus.Invalid)
                    {
                        var dist = Mathf.Lerp(lastDistance, (t0.Position - t1.Position).magnitude, Time.deltaTime*10.0f);

                        if (lastDistance > 0)
                        {
                            ZoomDelta = (lastDistance - dist) * 0.01f * Sensitivity;

                            if (ReverseZoom)
                            {
                                ZoomDelta = -ZoomDelta;
                            }
                        }
                        else
                        {
                            ZoomDelta = 0.0f;
                        }

                        lastDistance = dist;
                    }
                }
            }
            else
            {
                endPress = true;
            }

            if (endPress)
            {
                lastDistance = 0.0f;
                Active = false;
                TouchIndex = -1;
                TouchIndexAux = -1;
                ZoomDelta = 0.0f;
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

            GUI.Box(rect, "Zoom area");
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
