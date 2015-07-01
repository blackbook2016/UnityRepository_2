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
    public class ThirdPersonInput : GameInput
    {
        /// <summary>
        /// type
        /// </summary>
        public override InputPreset PresetType
        {
            get { return InputPreset.ThirdPerson; }
        }

        /// <summary>
        /// update input array
        /// </summary>
        public override void UpdateInput(Input[] inputs)
        {
            //
            // rotate camera
            //
            var gamePadInput = new Vector2(InputWrapper.GetAxis("Horizontal_R"), InputWrapper.GetAxis("Vertical_R"));
            SetInput(inputs, InputType.Rotate, gamePadInput);

            if (gamePadInput.sqrMagnitude < Mathf.Epsilon && CursorLocking.IsLocked)
            {
                SetInput(inputs, InputType.Rotate, new Vector2(InputWrapper.GetAxis("Mouse X"), InputWrapper.GetAxis("Mouse Y")));
            }

//            SetInput(inputs, InputType.Reset, UnityEngine.Input.GetKey(KeyCode.R));
            SetInput(inputs, InputType.Reset, InputWrapper.GetButton("Reset"));

            //
            // movement
            //
            var horizontal = InputWrapper.GetAxis("Horizontal");
            var vertical = InputWrapper.GetAxis("Vertical");

            var move = new Vector2(horizontal, vertical);
            padFilter.AddSample(move);
            SetInput(inputs, InputType.Move, padFilter.GetValue());

            //
            // aim && fire
            //
            var aimAxis = InputWrapper.GetAxis("Aim");
            var fireAxis = InputWrapper.GetAxis("Fire");
            var aimButton = InputWrapper.GetButton("Aim");
            var fireButton = InputWrapper.GetButton("Fire");

            SetInput(inputs, InputType.Aim, aimAxis > 0.5f || aimButton);
            SetInput(inputs, InputType.Fire, fireAxis > 0.5f || fireButton);

            //
            // crouch
            //
            SetInput(inputs, InputType.Crouch, UnityEngine.Input.GetKey(KeyCode.C) || InputWrapper.GetButton("Crouch"));

            //
            // walk toggle
            //
            SetInput(inputs, InputType.Walk, InputWrapper.GetButton("Walk"));

            //
            // jump toggle
            //
            SetInput(inputs, InputType.Jump, InputWrapper.GetButton("Jump"));

            //
            // sprint toggle
            //
            SetInput(inputs, InputType.Sprint, InputWrapper.GetButton("Sprint"));
        }
    }
}
