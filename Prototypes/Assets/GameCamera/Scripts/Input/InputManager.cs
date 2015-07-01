// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using RG_GameCamera.Input.Mobile;
using RG_GameCamera.Utils;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace RG_GameCamera.Input
{
    public enum InputType
    {
        Pan     = 0,
        Zoom,
        Rotate,
        Move,
        Reset,
        Aim,
        Fire,
        Crouch,
        Walk,
        Sprint,
        Jump,
        Die,
        WaypointPos,

        InputLast,
    }

    public enum InputGroup
    {
        CameraMove,
        Character,
        All,
        None,
    }

    public class Input
    {
        public bool Valid;
        public InputType Type;
        public object Value;
        public bool Enabled;
    }

    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance
        {
            get
            {
                if (!instance)
                {
                    instance = Utils.CameraInstance.CreateInstance<InputManager>("CameraInput");
                }
                return instance;
            }
        }

        public static float DoubleClickTimeout = 0.25f;
        public bool FilterInput = true;
        private static InputManager instance;
        public InputPreset InputPreset;

        public bool MobileInput = false;

        [HideInInspector]
        public bool IsValid = false;
        private Input[] inputs;
        private GameInput[] GameInputs;
        private GameInput currInput;

        /// <summary>
        /// get input value by type
        /// </summary>
        /// <param name="type">type of the input</param>
        /// <returns>value of input</returns>
        public Input GetInput(InputType type)
        {
            return inputs[(int) type];
        }

        /// <summary>
        /// return input value (template version)
        /// if input is valid it returns casted value
        /// else it returns default value
        /// </summary>
        /// <typeparam name="T">type of input value</typeparam>
        /// <param name="type">name of input parameter</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>value of type T</returns>
        public T GetInput<T>(InputType type, T defaultValue)
        {
            var input = inputs[(int) type];

            if (input.Valid)
            {
                return (T) input.Value;
            }

            return defaultValue;
        }

        /// <summary>
        /// select input preset by type
        /// </summary>
        /// <param name="preset">type of preset</param>
        public void SetInputPreset(InputPreset preset)
        {
            foreach (var gameInput in GameInputs)
            {
                if (gameInput.PresetType == preset)
                {
                    currInput = gameInput;
                    InputPreset = preset;
                    return;
                }
            }

            Utils.Debug.Assert(false, "Input Preset not found: " + preset.ToString());
        }

        public Input[] GetInputArray()
        {
            return inputs;
        }

        public GameInput GetInputPresetCurrent()
        {
            return currInput;
        }
        
        public void EnableInputGroup(InputGroup inputGroup, bool status)
        {
            switch (inputGroup)
            {
                case InputGroup.All:
                {
                    foreach (var input in inputs)
                    {
                        input.Enabled = status;
                    }
                }
                break;

                case InputGroup.CameraMove:
                {
                    inputs[(int)InputType.Pan].Enabled = status;
                    inputs[(int)InputType.Zoom].Enabled = status;
                    inputs[(int)InputType.Rotate].Enabled = status;
                    inputs[(int)InputType.Move].Enabled = status;
                    inputs[(int)InputType.Reset].Enabled = status;
                    inputs[(int)InputType.Aim].Enabled = status;
                }
                break;

                case InputGroup.Character:
                {
                    inputs[(int)InputType.Fire].Enabled = status;
                    inputs[(int)InputType.Crouch].Enabled = status;
                    inputs[(int)InputType.Walk].Enabled = status;
                    inputs[(int)InputType.Sprint].Enabled = status;
                    inputs[(int)InputType.Jump].Enabled = status;
                    inputs[(int)InputType.Die].Enabled = status;
                    inputs[(int)InputType.WaypointPos].Enabled = status;
                }
                break;

                default:
                    break;
            }
        }

        void Awake()
        {
            instance = this;

            inputs = new Input[(int) InputType.InputLast+1];
            var i = 0;
            foreach (var type in (InputType[])Enum.GetValues(typeof(InputType)))
            {
                inputs[i++] = new Input {Type = type, Valid = false, Value = null};
            }

            GameInputs = gameObject.GetComponents<GameInput>();

            Utils.Debug.Assert(GameInputs != null && GameInputs.Length > 0, "No game inputs found!");

            SetInputPreset(InputPreset);

            EnableInputGroup(InputGroup.All, true);
        }

        void Start()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            if (!Application.isEditor)
            {
                MobileInput = true;
                Utils.Debug.SetActive(Mobile.MobileControls.Instance.gameObject, true);
                Mobile.MobileControls.Instance.enabled = true;
            }
#endif
        }

        /// <summary>
        /// sets all input values to invalid state (no input)
        /// </summary>
        public void ResetInputArray()
        {
            foreach (var t in inputs)
            {
                t.Valid = false;
            }
        }

        /// <summary>
        /// input manager is updated from camera because it is important to update it before camera modes
        /// </summary>
        public void GameUpdate()
        {
            InputWrapper.Mobile = MobileInput;

            IsValid = true;

            // reset input data
            if (currInput.ResetInputArray)
            {
                foreach (var t in inputs)
                {
                    t.Valid = false;
                }
            }

            if (currInput.PresetType != InputPreset)
            {
                SetInputPreset(InputPreset);
            }

            //
            // update input
            //
            currInput.UpdateInput(inputs);
        }
    }
}
