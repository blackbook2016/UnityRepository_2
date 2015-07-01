// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using RG_GameCamera.Utils;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace RG_GameCamera.Input
{
    [System.Serializable]
    public class RPGInput : GameInput
    {
        /// <summary>
        /// direction of mouse panning up or down in screenspace to rotate the camera
        /// </summary>
        public MousePanRotDirection MouseRotateDirection;

        /// <summary>
        /// option for moving the character (with stick or waypoint)
        /// </summary>
        public MoveMethod MoveOption;

        /// <summary>
        /// type
        /// </summary>
        public override InputPreset PresetType
        {
            get { return InputPreset.RPG; }
        }

        /// <summary>
        /// update input array
        /// </summary>
        public override void UpdateInput(Input[] inputs)
        {
            mouseFilter.AddSample(UnityInput.mousePosition);

            //
            // rotate camera by dragging the mouse
            //
            if (InputWrapper.GetButton("RotatePan"))
            {
                var hor = MouseRotateDirection == MousePanRotDirection.Horizontal_L ||
                                 MouseRotateDirection == MousePanRotDirection.Horizontal_R;

                var sign = Mathf.Sign((int)MouseRotateDirection);
                var axis0 = InputWrapper.GetAxis("Mouse X");
                var axis1 = InputWrapper.GetAxis("Mouse Y");

                SetInput(inputs, InputType.Rotate, new Vector2(hor ? axis0 * sign : axis1 * sign, 0.0f));
            }

            //
            // zoom camera
            //
            var scrollWheel = InputWrapper.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scrollWheel) > Mathf.Epsilon)
            {
                SetInput(inputs, InputType.Zoom, scrollWheel);
            }
            else
            {
                //
                // zoom camera from gamepad
                //
                var zoomIn = InputWrapper.GetAxis("ZoomIn") * 0.1f;
                var zoomOut = InputWrapper.GetAxis("ZoomOut") * 0.1f;

                var zoom = zoomIn - zoomOut;

                if (Mathf.Abs(zoom) > Mathf.Epsilon)
                {
                    SetInput(inputs, InputType.Zoom, zoom);
                }
            }

            //
            // mobile controls
            //
            if (InputManager.Instance.MobileInput)
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
            }
            else
            {
                //
                // rotate camera
                //
                var gamePadInput = new Vector2(InputWrapper.GetAxis("Horizontal_R"), InputWrapper.GetAxis("Vertical_R"));
                if (gamePadInput.sqrMagnitude > Mathf.Epsilon)
                {
                    SetInput(inputs, InputType.Rotate, gamePadInput);
                }
                else
                {
                    var rot = ((InputWrapper.GetButton("RotateLeft") ? 1.0f : 0.0f) - (InputWrapper.GetButton("RotateRight") ? 1.0f : 0.0f));
                    if (Mathf.Abs(rot) > Mathf.Epsilon)
                    {
                        SetInput(inputs, InputType.Rotate, new Vector2(rot, 0.0f));
                    }
                }
            }

            SetInput(inputs, InputType.Reset, UnityEngine.Input.GetKey(KeyCode.R));

            doubleClickTimeout += Time.deltaTime;

            if (MoveOption == MoveMethod.Stick)
            {
                //
                // movement
                //
                var horizontal = InputWrapper.GetAxis("Horizontal");
                var vertical = InputWrapper.GetAxis("Vertical");

                var move = new Vector2(horizontal, vertical);
                padFilter.AddSample(move);
                SetInput(inputs, InputType.Move, padFilter.GetValue());
            }
            else if (MoveOption == MoveMethod.Waypoint)
            {
                //
                // calculate waypoint position
                //
                if (InputWrapper.GetButton("Waypoint"))
                {
                    Vector3 pos;
                    if (FindWaypointPosition(UnityInput.mousePosition, out pos))
                    {
                        SetInput(inputs, InputType.WaypointPos, pos);
                    }
                }
            }
        }
    }
}
