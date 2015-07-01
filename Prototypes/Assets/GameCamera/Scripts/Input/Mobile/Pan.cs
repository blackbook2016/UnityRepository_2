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
    public class Pan : BaseControl
    {
        public Vector2 PanPosition;
        public float Sensitivity = 1.0f;
        public bool DoublePan = true;

        private Rect rect;
        private Vector2 offset;
        private Vector2 start0, start1;
        private float negTimeout;

        public override ControlType Type
        {
            get { return ControlType.Pan; }
        }

        public override void Init(TouchProcessor processor)
        {
            base.Init(processor);
            rect = new Rect();
            UpdateRect();
            Side = ControlSide.Arbitrary;

            Priority = 2;
        }

        public override Dictionary<string, object> SerializeJSON()
        {
            var baseDic = base.SerializeJSON();
            baseDic.Add("Sensitivity", Sensitivity);
            baseDic.Add("DoublePan", DoublePan);

            return baseDic;
        }

        public override void DeserializeJSON(Dictionary<string, object> jsonDic)
        {
            base.DeserializeJSON(jsonDic);
            Sensitivity = Convert.ToSingle(jsonDic["Sensitivity"]);
            DoublePan = Convert.ToBoolean(jsonDic["DoublePan"]);
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

            if (touches > 0)
            {
                var touch0 = touchProcessor.GetTouch(0);
                var touch1 = touchProcessor.GetTouch(1);

                if (touch0.Status == TouchStatus.Start)
                {
                    if (ContainPoint(touch0.StartPosition))
                    {
                        TouchIndex = 0;
                        offset = Vector2.zero;
                        start0 = touch0.Position;
                        start1 = touch1.Position;
                    }
                }
                else if (touch0.Status == TouchStatus.End)
                {
                    TouchIndex = -1;

                    if (TouchIndexAux != -1)
                    {
                        offset = -touch1.Position + touch0.Position;
                    }
                }

                if (touch1.Status == TouchStatus.Start)
                {
                    if (ContainPoint(touch0.StartPosition))
                    {
                        TouchIndexAux = 1;
                        start0 = touch0.Position;
                        start1 = touch1.Position;
                    }
                }
                else if (touch1.Status == TouchStatus.End)
                {
                    TouchIndexAux = -1;
                }

                Active = TouchIndex != -1 || TouchIndexAux != -1;

                if (Active)
                {
                    var t0 = touchProcessor.GetTouch(TouchIndex != -1 ? TouchIndex : TouchIndexAux);
                    var pos = t0.Position + offset;

                    Operating = (pos - PanPosition).sqrMagnitude > Mathf.Epsilon;

                    if (TouchIndex != -1 && TouchIndexAux != -1)
                    {
                        if (DoublePan)
                        {
                            if (negTimeout > 0)
                            {
                                start0 = touch0.Position;
                                start1 = touch1.Position;
                                offset = Vector2.zero;
                            }
                            else
                            {
                                start0 = Vector2.Lerp(start0, touch0.Position, Time.deltaTime * 10.0f);
                                start1 = Vector2.Lerp(start1, touch1.Position, Time.deltaTime * 10.0f);

                                if (offset.sqrMagnitude > Mathf.Epsilon)
                                {
                                    offset = -start1 + start0;
                                }
                            }

                            var v0 = (touch0.Position - start0).normalized;
                            var v1 = (touch1.Position - start1).normalized;
                            var dot = Vector2.Dot(v0, v1);

                            if (dot < 0.8f)
                            {
                                Operating = false;
                            }

                            if (dot < 0)
                            {
                                negTimeout = 1.0f;
                            }

                            if (Operations == 0)
                            {
                                start0 = touch0.Position;
                                start1 = touch1.Position;
                                offset = Vector2.zero;
                            }
                        }
                        else
                        {
                            Operations = 0;
                        }
                    }
                    else
                    {
                        Operations = 4;
                    }

                    PanPosition = pos;
                }
            }
            else
            {
                endPress = true;
            }

            if (endPress)
            {
                Active = false;
                Operating = false;
                TouchIndex = -1;
                TouchIndexAux = -1;
                PanPosition = Vector2.zero;
                offset = Vector2.zero;
            }
        }

        public override void GameUpdate()
        {
            if (Active)
            {
                OperationTimer += Time.deltaTime;

                if (Operating)
                {
                    Operations++;

                    if (Operations > 20)
                    {
                        Operations = 20;
                    }
                }

                if (OperationTimer > 0.1f)
                {
                    OperationTimer = 0.0f;
                    Operations--;

                    if (Operations < 0)
                    {
                        Operations = 0;
                    }
                }

                negTimeout -= Time.deltaTime;

                if (negTimeout > 0.0f)
                {
                    Operations = 0;
                }
            }
            else
            {
                OperationTimer = 0.0f;
                Operations = 0;
            }

            DetectTouches();

//            Utils.Debug.Log("Timer {0} Operations {1}", OperationTimer, Operations);
        }

        public override void Draw()
        {
            UpdateRect();

            if (HideGUI)
            {
                return;
            }

            GUI.Box(rect, "Pan area");
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
