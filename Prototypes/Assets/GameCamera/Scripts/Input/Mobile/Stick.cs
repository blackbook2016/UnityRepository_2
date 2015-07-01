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
    /// mobile stick is a simulation of a joystick handle, very useful for character movement
    /// </summary>
    public class Stick : BaseControl
    {
        public float CircleSize = 160.0f;
        public float HitSize = 32.0f;
        public Texture2D MoveControlsCircle;
        public Texture2D MoveControlsHit;
        public float Sensitivity = 1.0f;

        private Rect rect;
        private bool pressed;
        private Vector2 input;

        public override ControlType Type
        {
            get { return ControlType.Stick; }
        }

        public override Dictionary<string, object> SerializeJSON()
        {
            var baseDic = base.SerializeJSON();

            baseDic.Add("CircleSize", CircleSize);
            baseDic.Add("HitSize", HitSize);
            baseDic.Add("Sensitivity", Sensitivity);
            if (MoveControlsCircle)
            {
                baseDic.Add("MoveControlsCircle", MoveControlsCircle.name);
            }
            if (MoveControlsHit)
            {
                baseDic.Add("MoveControlsHit", MoveControlsHit.name);
            }

            return baseDic;
        }

        public override void DeserializeJSON(Dictionary<string, object> jsonDic)
        {
            base.DeserializeJSON(jsonDic);
            CircleSize = Convert.ToSingle(jsonDic["CircleSize"]);
            HitSize = Convert.ToSingle(jsonDic["HitSize"]);
            Sensitivity = Convert.ToSingle(jsonDic["Sensitivity"]);

            if (jsonDic.ContainsKey("MoveControlsCircle"))
            {
                MoveControlsCircle = FindTexture(Convert.ToString(jsonDic["MoveControlsCircle"]));
            }

            if (jsonDic.ContainsKey("MoveControlsHit"))
            {
                MoveControlsHit = FindTexture(Convert.ToString(jsonDic["MoveControlsHit"]));
            }
        }

        public override void GameUpdate()
        {
            DetectTouches();

            input = Vector2.zero;

            if (TouchIndex != -1)
            {
                var moveTouch = touchProcessor.GetTouch(TouchIndex);

                if (moveTouch.Status != TouchStatus.Invalid)
                {
                    var m = moveTouch.Position - moveTouch.StartPosition;
                    var size = m.magnitude;

                    if (Sensitivity < 1.0f)
                    {
                        var to = Quaternion.FromToRotation(m, Vector2.up);
                        var rot = Quaternion.Slerp(Quaternion.identity, to, 1.0f - Sensitivity);

                        m = rot*m;
                    }

                    if (size > Mathf.Epsilon)
                    {
                        var radius = CircleSize / 2 - HitSize / 2;
                        var ratio = size / radius;

                        var movement = m * ratio;

                        movement.x = Mathf.Clamp(movement.x, -radius, radius);
                        movement.y = Mathf.Clamp(movement.y, -radius, radius);

                        input = movement / radius;
                    }
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

        public override void Draw()
        {
            if (HideGUI)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // display movement circle only for camera movement
                var offset = -CircleSize * 0.5f;
                Vector2 pos;

                pos.y = Screen.height*0.3f;
                pos.x = Side == ControlSide.Left ? Screen.width*0.2f : Screen.width - Screen.width*0.2f;

                if (MoveControlsCircle)
                {
                    var circlePos = new Rect(offset + pos.x, offset + (Screen.height - pos.y), CircleSize, CircleSize);
                    GUI.DrawTexture(circlePos, MoveControlsCircle, ScaleMode.StretchToFill);
                }

                if (MoveControlsHit)
                {
                    offset = -HitSize * 0.5f;
                    var hitPos = new Rect(offset + pos.x, offset + (Screen.height - pos.y), HitSize, HitSize);
                    GUI.DrawTexture(hitPos, MoveControlsHit, ScaleMode.StretchToFill);
                }

                return;
            }
#endif

            if (TouchIndex != -1)
            {
                var touch = touchProcessor.GetTouch(TouchIndex);

                // display movement circle only for camera movement
                var offset = -CircleSize * 0.5f;

                if (MoveControlsCircle)
                {
                    var circlePos = new Rect(offset + touch.StartPosition.x, offset + (Screen.height - touch.StartPosition.y), CircleSize, CircleSize);
                    GUI.DrawTexture(circlePos, MoveControlsCircle, ScaleMode.StretchToFill);
                }

                if (MoveControlsHit)
                {
                    offset = -HitSize * 0.5f;
                    var hitPos = new Rect(offset + touch.Position.x, offset + (Screen.height - touch.Position.y), HitSize, HitSize);
                    GUI.DrawTexture(hitPos, MoveControlsHit, ScaleMode.StretchToFill);
                }
            }
        }
    }
}
