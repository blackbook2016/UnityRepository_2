// Version 1.1.5
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace RG_GameCamera.Input
{
    [System.Serializable]
    public class CustomInput : GameInput
    {
        /// <summary>
        /// type
        /// </summary>
        public override InputPreset PresetType
        {
            get { return InputPreset.Custom; }
        }

        protected override void Awake()
        {
            ResetInputArray = false;
        }

        /// <summary>
        /// update input array
        /// </summary>
        public override void UpdateInput(Input[] inputs)
        {
        }

        /// <summary>
        /// panning camera
        /// </summary>
        /// <param name="pos">position in world-space</param>
        public void OnPan(Vector2 pos)
        {
            InputManager.Instance.GetInputArray()[(int) InputType.Pan].Valid = true;
            InputManager.Instance.GetInputArray()[(int) InputType.Pan].Value = pos;
        }

        /// <summary>
        /// zoom camera
        /// </summary>
        /// <param name="delta">delta zoom value</param>
        public void OnZoom(float delta)
        {
            InputManager.Instance.GetInputArray()[(int)InputType.Zoom].Valid = true;
            InputManager.Instance.GetInputArray()[(int)InputType.Zoom].Value = delta;
        }

        /// <summary>
        /// rotate camera
        /// </summary>
        /// <param name="rot">rotate axis</param>
        public void OnRotate(Vector2 rot)
        {
            InputManager.Instance.GetInputArray()[(int)InputType.Rotate].Valid = true;
            InputManager.Instance.GetInputArray()[(int)InputType.Rotate].Value = rot;
        }

        /// <summary>
        /// move camera
        /// </summary>
        /// <param name="axis">movement axis</param>
        public void OnMove(Vector2 axis)
        {
            InputManager.Instance.GetInputArray()[(int)InputType.Move].Valid = true;
            InputManager.Instance.GetInputArray()[(int)InputType.Move].Value = axis;
        }

        /// <summary>
        /// aim mode (valid for character controller)
        /// </summary>
        public void OnAim(bool aim)
        {
            InputManager.Instance.GetInputArray()[(int)InputType.Aim].Valid = true;
            InputManager.Instance.GetInputArray()[(int)InputType.Aim].Value = aim;
        }

        /// <summary>
        /// fire mode (valid for character controller)
        /// </summary>
        public void OnFire(bool fire)
        {
            InputManager.Instance.GetInputArray()[(int)InputType.Fire].Valid = true;
            InputManager.Instance.GetInputArray()[(int)InputType.Fire].Value = fire;
        }

        /// <summary>
        /// crouch mode (valid for character controller)
        /// </summary>
        public void OnCrouch(bool val)
        {
            InputManager.Instance.GetInputArray()[(int)InputType.Crouch].Valid = true;
            InputManager.Instance.GetInputArray()[(int)InputType.Crouch].Value = val;
        }

        /// <summary>
        /// walk mode (valid for character controller)
        /// </summary>
        public void OnWalk(bool val)
        {
            InputManager.Instance.GetInputArray()[(int)InputType.Walk].Valid = true;
            InputManager.Instance.GetInputArray()[(int)InputType.Walk].Value = val;
        }

        /// <summary>
        /// sprint mode (valid for character controller)
        /// </summary>
        public void OnSprint(bool val)
        {
            InputManager.Instance.GetInputArray()[(int)InputType.Sprint].Valid = true;
            InputManager.Instance.GetInputArray()[(int)InputType.Sprint].Value = val;
        }

        /// <summary>
        /// jump mode (valid for character controller)
        /// </summary>
        public void OnJump(bool val)
        {
            InputManager.Instance.GetInputArray()[(int)InputType.Jump].Valid = true;
            InputManager.Instance.GetInputArray()[(int)InputType.Jump].Value = val;
        }

        /// <summary>
        /// die (valid for character controller)
        /// </summary>
        public void OnDie(bool val)
        {
            InputManager.Instance.GetInputArray()[(int)InputType.Die].Valid = true;
            InputManager.Instance.GetInputArray()[(int)InputType.Die].Value = val;
        }

        /// <summary>
        /// go to waypoint (valid for character controller in RTS/RPG)
        /// </summary>
        /// <param name="mousePos">mouse position, position on ground will be calculated</param>
        public void OnWaypoint(Vector3 mousePos)
        {
            Vector3 pos;
            if (FindWaypointPosition(UnityInput.mousePosition, out pos))
            {
                InputManager.Instance.GetInputArray()[(int)InputType.WaypointPos].Valid = true;
                InputManager.Instance.GetInputArray()[(int)InputType.WaypointPos].Value = pos;
            }
        }
    }
}
