// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using RG_GameCamera.Input.Mobile;
using RG_GameCamera.Utils;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace RG_GameCamera.Input
{
    [System.Serializable]
    public class OrbitInput : GameInput
    {
        /// <summary>
        /// type
        /// </summary>
        public override InputPreset PresetType
        {
            get { return InputPreset.Orbit; }
        }

        /// <summary>
        /// update input array
        /// </summary>
        public override void UpdateInput(Input[] inputs)
        {
            mouseFilter.AddSample(UnityInput.mousePosition);

            //
            // mobile controls
            //
            if (InputManager.Instance.MobileInput)
            {
                //
                // pan camera
                //
                var mobilePan = InputWrapper.GetPan("Pan");

                if (mobilePan.sqrMagnitude > Mathf.Epsilon)
                {
                    SetInput(inputs, InputType.Pan, mobilePan);
                }
                else
                {
                    //
                    // zoom camera from touch input
                    //
                    var mobileZoom = InputWrapper.GetZoom("Zoom");

                    if (Mathf.Abs(mobileZoom) > Mathf.Epsilon)
                    {
                        SetInput(inputs, InputType.Zoom, mobileZoom);
                    }

                    //
                    // rotate camera
                    //
                    var mobileRotation = InputWrapper.GetRotation("Rotate");

                    if (Mathf.Abs(mobileRotation) > Mathf.Epsilon)
                    {
                        SetInput(inputs, InputType.Rotate, new Vector2(mobileRotation, 0.0f));
                    }
                    else
                    {
                        //
                        // rotate camera by axis
                        //
                        var axisInput = new Vector2(InputWrapper.GetAxis("Horizontal_R"), InputWrapper.GetAxis("Vertical_R"));

                        if (axisInput.sqrMagnitude > Mathf.Epsilon)
                        {
                            SetInput(inputs, InputType.Rotate, new Vector2(axisInput.x, axisInput.y));
                        }
                    }
                }
            }
            else
            {
                var input = RG_GameCamera.Input.InputManager.Instance.FilterInput ? (Vector2)mouseFilter.GetValue() : new Vector2(UnityInput.mousePosition.x, UnityInput.mousePosition.y);

                //
                // pan camera
                //
                if (InputWrapper.GetButton("Pan"))
                {
                    SetInput(inputs, InputType.Pan, input);
                }

                //
                // zoom camera
                //
                var scrollWheel = InputWrapper.GetAxis("Mouse ScrollWheel");

                if (Mathf.Abs(scrollWheel) > Mathf.Epsilon)
                {
                    SetInput(inputs, InputType.Zoom, scrollWheel);
                }

                //
                // rotate camera
                //
                var gamePadInput = new Vector2(InputWrapper.GetAxis("Horizontal_R"), InputWrapper.GetAxis("Vertical_R"));

                // gamepad version
                if (gamePadInput.sqrMagnitude > Mathf.Epsilon)
                {
                    SetInput(inputs, InputType.Rotate, new Vector2(gamePadInput.x, gamePadInput.y));
                }

                // mouse version
                if (UnityInput.GetMouseButton(1))
                {
                    SetInput(inputs, InputType.Rotate, new Vector2(InputWrapper.GetAxis("Mouse X"), InputWrapper.GetAxis("Mouse Y")));
                }

                SetInput(inputs, InputType.Reset, UnityEngine.Input.GetKey(KeyCode.R));

                doubleClickTimeout += Time.deltaTime;

                if (UnityEngine.Input.GetMouseButtonDown(2))
                {
                    if (doubleClickTimeout < InputManager.DoubleClickTimeout)
                    {
                        SetInput(inputs, InputType.Reset, true);
                    }

                    doubleClickTimeout = 0.0f;
                }

                //
                // movement
                //
                var horizontal = InputWrapper.GetAxis("Horizontal");
                var vertical = InputWrapper.GetAxis("Vertical");

                var move = new Vector2(horizontal, vertical);
                padFilter.AddSample(move);
                SetInput(inputs, InputType.Move, padFilter.GetValue());
            }
        }
    }
}
