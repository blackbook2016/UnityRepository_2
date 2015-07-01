// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace RG_GameCamera.Input.Mobile
{
    [ExecuteInEditMode]
    public class MobileControls : MonoBehaviour
    {
        public static MobileControls Instance
        {
            get
            {
                if (!instance)
                {
                    Utils.CameraInstance.CreateInstance<MobileControls>("MobileControls");
                }
                return instance;
            }
        }

        public enum Layout
        {
            Empty,
            FPS,
            Orbit,
            RPG,
            RTS,
            ThirdPerson,
        }

		private enum ControlPriority
		{
			Pan,
			ZoomRotate,
		}

        private static MobileControls instance;
		private ControlPriority controlPriority;

        public int LeftPanelIndex = 0;
        public int RightPanelIndex = 0;

        private TouchProcessor touchProcessor;

        private bool isPanning;

        void Awake()
        {
            Init();
            isPanning = false;
        }

        public Dictionary<string, object> Serialize()
        {
            var controls = gameObject.GetComponents<BaseControl>();

            var dic = new Dictionary<string, object>();
            var counter = 0;

            foreach (var baseControl in controls)
            {
                dic.Add(counter++.ToString(), baseControl.SerializeJSON());
            }

            return dic;
        }

        public void Deserialize(Dictionary<string, object> dic)
        {
            LeftPanelIndex = 0;
            RightPanelIndex = 0;

            var controls = gameObject.GetComponents<BaseControl>();

            foreach (var control in controls)
            {
                RemoveControl(control);
            }

            foreach (Dictionary<string, object> control in dic.Values)
            {
                var type = (ControlType)Convert.ToInt32(control["Type"]);

                switch (type)
                {
                    case ControlType.Button:
                        var btn = CreateButton("");
                        btn.DeserializeJSON(control);
                        break;

                    case ControlType.Stick:
                    case ControlType.CameraPanel:
                        var cam = DeserializeMasterControl(type);
                        cam.DeserializeJSON(control);
                        break;

                    case ControlType.Zoom:
                        var zoom = CreateZoom("");
                        zoom.DeserializeJSON(control);
                        break;

                    case ControlType.Rotate:
                        var rot = CreateRotation("");
                        rot.DeserializeJSON(control);
                        break;

                    case ControlType.Pan:
                        var pan = CreatePan("");
                        pan.DeserializeJSON(control);
                        break;
                }
            }
        }

        public void LoadLayout(Layout layout)
        {
            const string resDir = "Config/MobileLayouts/";

            var resName = layout.ToString() + "Layout";

            var asset = Resources.Load<TextAsset>(resDir + resName);

            if (asset && !string.IsNullOrEmpty(asset.text))
            {
                var dic = RG_GameCamera.ThirdParty.Json.Deserialize(asset.text) as Dictionary<string, object>;
                Deserialize(dic);
            }
        }

        void Init()
        {
            if (instance == null)
            {
                instance = this;
                touchProcessor = new TouchProcessor(2);

                var controls = GetControls();
                if (controls != null)
                {
                    foreach (var control in controls)
                    {
                        control.Init(touchProcessor);
                    }
                }
            }
        }

        public BaseControl[] GetControls()
        {
            var baseControls = gameObject.GetComponents<BaseControl>();
            Array.Sort(baseControls, delegate(BaseControl a, BaseControl b)
            {
                //return b.Priority.CompareTo(a.Priority);
                return a.OperationTimer.CompareTo(b.Operations);
            });

            return baseControls;
        }

        public Button CreateButton(string btnName)
        {
            var btn = gameObject.AddComponent<Button>();
            btn.Init(touchProcessor);
            btn.InputKey0 = btnName;
            return btn;
        }

        public Zoom CreateZoom(string btnName)
        {
            var btn = gameObject.AddComponent<Zoom>();
            btn.Init(touchProcessor);
            btn.InputKey0 = btnName;
            return btn;
        }

        public Rotate CreateRotation(string btnName)
        {
            var btn = gameObject.AddComponent<Rotate>();
            btn.Init(touchProcessor);
            btn.InputKey0 = btnName;
            return btn;
        }

        public Pan CreatePan(string btnName)
        {
            var btn = gameObject.AddComponent<Pan>();
            btn.Init(touchProcessor);
            btn.InputKey0 = btnName;
            return btn;
        }

        public void DuplicateButtonValues(Button target, Button source)
        {
            target.Position = source.Position;
            target.Size = source.Size;
            target.PreserveTextureRatio = source.PreserveTextureRatio;
            target.Toggle = source.Toggle;
            target.TextureDefault = source.TextureDefault;
            target.TexturePressed = source.TexturePressed;
        }

        public void DuplicateBasicValues(BaseControl target, BaseControl source)
        {
            target.Position = source.Position;
            target.Size = source.Size;
        }

        public void RemoveControl(BaseControl button)
        {
            Utils.Debug.Destroy(button, true);
        }

        private BaseControl DeserializeMasterControl(ControlType type)
        {
            BaseControl btn = null;
            switch (type)
            {
                case ControlType.CameraPanel:
                    btn = gameObject.AddComponent<CameraPanel>();
                    break;

                case ControlType.Stick:
                    btn = gameObject.AddComponent<Stick>();
                    break;
            }

            if (btn != null)
            {
                btn.Init(touchProcessor);
            }

            return btn;
        }

        public BaseControl CreateMasterControl(string axis0, string axis1, ControlType type, ControlSide side)
        {
            RemoveMasterControl(side);

            BaseControl btn = null;
            switch (type)
            {
                case ControlType.CameraPanel:
                    btn = gameObject.AddComponent<CameraPanel>();
                    break;

                case ControlType.Stick:
                    btn = gameObject.AddComponent<Stick>();
                    break;
            }

            if (btn != null)
            {
                btn.Init(touchProcessor);
                btn.Side = side;
                btn.InputKey0 = axis0;
                btn.InputKey1 = axis1;
            }

            return btn;
        }

        public void RemoveMasterControl(ControlSide side)
        {
            var controls = GetControls();
            if (controls != null)
            {
                foreach (var baseControl in controls)
                {
                    if (baseControl.Side == side)
                    {
                        RemoveControl(baseControl);
                    }
                }
            }
        }

        /// <summary>
        /// get status of the button
        /// </summary>
        public bool GetButton(string key)
        {
            BaseControl btn;
            if (TryGetControl(key, out btn))
            {
                return (btn.Type == ControlType.Button && ((Button) btn).IsPressed());
            }

            return false;
        }

        /// <summary>
        /// get status of the zoom (pinch gesture defined over area)
        /// </summary>
        public float GetZoom(string key)
        {
			if (!isPanning)
			{
	            BaseControl btn;
	            if (TryGetControl(key, out btn))
	            {
	                if (btn.Type == ControlType.Zoom && btn.Active)
	                {
	                    return ((Zoom) btn).ZoomDelta;
	                }
	            }
			}

            return 0.0f;
        }

        /// <summary>
        /// get status of the rotation
        /// </summary>
        public float GetRotation(string key)
        {
			if (!isPanning)
			{
	            BaseControl btn;
	            if (TryGetControl(key, out btn))
	            {
	                if (btn.Type == ControlType.Rotate && btn.Active)
	                {
	                    return ((Rotate) btn).RotateAngle;
	                }
	            }
			}

            return 0.0f;
        }

        /// <summary>
        /// get status of the pan
        /// </summary>
        public Vector2 GetPan(string key)
        {
			BaseControl btn;
			if (TryGetControl(key, out btn))
			{
			    if (btn.Type == ControlType.Pan && btn.Active && btn.Operations > 3)
			    {
			        isPanning = true;
			        return ((Pan) btn).PanPosition;
			    }
	        }

            isPanning = false;
            return Vector2.zero;
        }

        /// <summary>
        /// get status of the axis
        /// </summary>
        public float GetAxis(string key)
        {
            BaseControl btn;
            if (TryGetControl(key, out btn))
            {
                if (btn.Type == ControlType.Stick || btn.Type == ControlType.CameraPanel)
                {
                    var axis = btn.GetInputAxis();

                    if (key == btn.InputKey0)
                    {
                        return axis.x;
                    }
                    if (key == btn.InputKey1)
                    {
                        return axis.y;
                    }

                    Utils.Debug.Assert(false);
                    return 0.0f;
                }
            }

            return 0;
        }

        /// <summary>
        /// get status of the down button event
        /// </summary>
        public bool GetButtonDown(string buttonName)
        {
            BaseControl btn;
            if (TryGetControl(buttonName, out btn))
            {
                return (btn.Type == ControlType.Button && ((Button)btn).State == Button.ButtonState.Begin);
            }

            return false;
        }

        /// <summary>
        /// get status of up button event
        /// </summary>
        public bool GetButtonUp(string buttonName)
        {
            BaseControl btn;
            if (TryGetControl(buttonName, out btn))
            {
                return (btn.Type == ControlType.Button && ((Button)btn).State == Button.ButtonState.End);
            }

            return false;
        }

        private bool TryGetControl(string key, out BaseControl ctrl)
        {
            var controls = GetControls();
            if (controls != null)
            {
                foreach (var control in controls)
                {
                    if (control.InputKey0 == key || control.InputKey1 == key)
                    {
                        ctrl = control;
                        return true;
                    }
                }
            }

            ctrl = null;
            return false;
        }

        private void Update()
        {
            // hack - fix for re-creating the structures when the code is recompiled
            Init();

            touchProcessor.ScanInput();

            var controls = GetControls();
            if (controls != null)
            {
//				Utils.Debug.Log("-----------------------------------------");

                foreach (var control in controls)
                {
                    control.GameUpdate();

//					Utils.Debug.Log("Type {0} Timer {1} Hits {2}", control.Type, control.OperationTimer, control.Operations);
                }
            }
        }

        private void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            var controls = GetControls();
            if (controls != null)
            {
                foreach (var control in controls)
                {
                    control.Draw();
                }
            }
        }
    }
}
