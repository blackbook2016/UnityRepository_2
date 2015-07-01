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
    public class Button : BaseControl
    {
        public bool Toggle;
        public bool HoldDrag;
        public bool InvalidateOnDrag;
        public float HoldTimeout = 0.3f;
        public Texture2D TextureDefault;
        public Texture2D TexturePressed;

        public enum ButtonState
        {
            Pressed,
            Begin,
            End,
            None,
        }

        public ButtonState State;

        private Rect rect;
        private bool pressed;
        private Vector2 startTouch;

        public override ControlType Type
        {
            get { return ControlType.Button; }
        }

        public override void Init(TouchProcessor processor)
        {
            base.Init(processor);
            rect = new Rect();
            UpdateRect();
            State = ButtonState.None;
            Side = ControlSide.Arbitrary;
        }

        public override Dictionary<string, object> SerializeJSON()
        {
            var baseDic = base.SerializeJSON();

            baseDic.Add("Toggle", Toggle);
            baseDic.Add("HoldDrag", HoldDrag);
            baseDic.Add("HoldTimeout", HoldTimeout);
            baseDic.Add("InvalidateOnDrag", InvalidateOnDrag);
            if (TextureDefault)
            {
                baseDic.Add("TextureDefault", TextureDefault.name);
            }
            if (TextureDefault)
            {
                baseDic.Add("TexturePressed", TexturePressed.name);
            }

            return baseDic;
        }

        public override void DeserializeJSON(Dictionary<string, object> jsonDic)
        {
            base.DeserializeJSON(jsonDic);

            Toggle = Convert.ToBoolean(jsonDic["Toggle"]);
            HoldDrag = Convert.ToBoolean(jsonDic["HoldDrag"]);
            HoldTimeout = Convert.ToSingle(jsonDic["HoldTimeout"]);

            if (jsonDic.ContainsKey("InvalidateOnDrag"))
            {
                InvalidateOnDrag = Convert.ToBoolean(jsonDic["InvalidateOnDrag"]);
            }

            if (jsonDic.ContainsKey("TextureDefault"))
            {
                TextureDefault = FindTexture(Convert.ToString(jsonDic["TextureDefault"]));
            }

            if (jsonDic.ContainsKey("TexturePressed"))
            {
                TexturePressed = FindTexture(Convert.ToString(jsonDic["TexturePressed"]));
            }
        }

        public bool ContainPoint(Vector2 point)
        {
            point.y = Screen.height - point.y;
            return rect.Contains(point);
        }

        public void Press()
        {
            if (Toggle)
            {
                pressed = !pressed;
            }
            else
            {
                pressed = true;
            }

            OnTouchDown();
        }

        public bool IsPressed()
        {
            return pressed;
        }

        public void Reset()
        {
            pressed = false;
            OnTouchUp();
        }

        private void CheckForMove(Vector2 touch)
        {
            if (InvalidateOnDrag)
            {
                if ((touch - startTouch).sqrMagnitude > 10.0f)
                {
                    State = ButtonState.None;
                    pressed = false;
                }
            }
        }

        protected override void DetectTouches()
        {
            var touches = touchProcessor.GetActiveTouchCount();
            var endPress = false;

            if (touches > 0)
            {
                // check for button press
                for (var i = 0; i < touches; i++)
                {
                    var touch = touchProcessor.GetTouch(i);

                    if (ContainPoint(touch.StartPosition))
                    {
                        switch (touch.Status)
                        {
                            case TouchStatus.Start:
                            {
                                Press();
                                State = ButtonState.Begin;
                                startTouch = touch.StartPosition;
                                TouchIndex = i;
                            }
                            break;
                        }
                    }

                    if (TouchIndex == i)
                    {
                        switch (touch.Status)
                        {
                            case TouchStatus.Stationary:
                            case TouchStatus.Moving:
                                State = ButtonState.Pressed;
                                CheckForMove(touch.Position);
                                break;

                            case TouchStatus.End:
                                State = ButtonState.End;
                                CheckForMove(touch.Position);
                                endPress = true;
                                break;

                            case TouchStatus.Invalid:
                                endPress = true;
                                break;
                        }
                    }
                }
            }
            else
            {
                endPress = true;
            }

            if (endPress)
            {
                if (TouchIndex == -1)
                {
                    State = ButtonState.None;
                }
                else if (!HoldDrag)
                {
                    if (IsHoldDrag())
                    {
                        State = ButtonState.None;
                    }
                }
                TouchIndex = -1;
                if (!Toggle)
                {
                    Reset();
                }
            }
        }

        public override void GameUpdate()
        {
            DetectTouches();
        }

        public override void Draw()
        {
            UpdateRect();

            if (HideGUI)
            {
                return;
            }

            var tex = pressed ? TexturePressed : TextureDefault;
            if (tex)
            {
                GUI.DrawTexture(rect, tex);
            }
        }

        public void UpdateRect()
        {
            rect.x = Position.x * Screen.width;
            rect.y = Position.y * Screen.height;
            rect.width = Size.x * Screen.width;
            rect.height = Size.y * Screen.height;
        }

        bool IsHoldDrag()
        {
            if (TouchIndex != -1)
            {
                var touch = touchProcessor.GetTouch(TouchIndex);
                return (touch.PressTime > HoldTimeout);
            }
            return false;
        }
    }
}
