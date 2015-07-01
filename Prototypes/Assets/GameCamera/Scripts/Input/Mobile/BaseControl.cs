// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections.Generic;
using RG_GameCamera.Utils;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RG_GameCamera.Input.Mobile
{
    /// <summary>
    /// type of the mobil control
    /// </summary>
    public enum ControlType
    {
        None,
        Stick,
        CameraPanel,
        Button,
        Zoom,
        Rotate,
        Pan,
    }

    /// <summary>
    /// position side of the control on the screen
    /// </summary>
    public enum ControlSide
    {
        Left,
        Right,
        Arbitrary,
    }

    /// <summary>
    /// base class for all mobile controls
    /// </summary>
    public abstract class BaseControl : MonoBehaviour
    {
        /// <summary>
        /// relative screen position of the control
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// relative screen size of the control
        /// </summary>
        public Vector2 Size;

        /// <summary>
        /// true if this control will have the same ratio as texture dimension
        /// </summary>
        public bool PreserveTextureRatio = true;

        /// <summary>
        /// position side on the screen
        /// </summary>
        public ControlSide Side;

        /// <summary>
        /// type of control
        /// </summary>
        public abstract ControlType Type { get; }

        /// <summary>
        /// filter other input when this control is active
        /// </summary>
        public int DisableInputGroup = (int)InputGroup.None;

        /// <summary>
        /// index of touch
        /// </summary>
        public int TouchIndex;

        /// <summary>
        /// secondary index of touch (for zoom)
        /// </summary>
        public int TouchIndexAux;

        /// <summary>
        /// name of the input
        /// </summary>
        public string InputKey0;

        /// <summary>
        /// name of secondary input
        /// </summary>
        public string InputKey1;

        /// <summary>
        /// don't show gui (button/stick)
        /// </summary>
        public bool HideGUI;

        /// <summary>
        /// update priority
        /// </summary>
        public int Priority = 1;

        /// <summary>
        /// timer or active (moving) input
        /// </summary>
        public float OperationTimer = 0.0f;

        /// <summary>
        /// is the controls active (not necessary moving, but it is touch-ready)
        /// </summary>
        public bool Active;

        /// <summary>
        /// the control is activelly running (panning/rotating)
        /// </summary>
        public bool Operating;

		public int Operations;

        protected TouchProcessor touchProcessor;

        public virtual void Init (TouchProcessor processor)
        {
            hideFlags = HideFlags.HideInInspector;
            touchProcessor = processor;
            TouchIndex = -1;
        }

        public virtual Dictionary<string, object> SerializeJSON()
        {
            var baseDic = new Dictionary<string, object>();

            baseDic.Add("PositionX", Position.x);
            baseDic.Add("PositionY", Position.y);
            baseDic.Add("SizeX", Size.x);
            baseDic.Add("SizeY", Size.y);
            baseDic.Add("PreserveTextureRatio", PreserveTextureRatio);
            baseDic.Add("Side", (int)Side);
            baseDic.Add("Type", (int)Type);
            baseDic.Add("InputGroup", (int)DisableInputGroup);

            baseDic.Add("TouchIndex", TouchIndex);
            baseDic.Add("TouchIndexAux", TouchIndexAux);
            baseDic.Add("InputKey0", InputKey0);
            baseDic.Add("InputKey1", InputKey1);
            baseDic.Add("HideGUI", HideGUI);
            baseDic.Add("Priority", Priority);

            return baseDic;
        }

        public virtual void DeserializeJSON(Dictionary<string, object> jsonDic)
        {
            Position.x = Convert.ToSingle(jsonDic["PositionX"]);
            Position.y = Convert.ToSingle(jsonDic["PositionY"]);
            Size.x = Convert.ToSingle(jsonDic["SizeX"]);
            Size.y = Convert.ToSingle(jsonDic["SizeY"]);
            PreserveTextureRatio = Convert.ToBoolean(jsonDic["PreserveTextureRatio"]);
            Side = (ControlSide)Convert.ToInt32(jsonDic["Side"]);
            DisableInputGroup = Convert.ToInt32(jsonDic["InputGroup"]);

            TouchIndex = Convert.ToInt32(jsonDic["TouchIndex"]);
            TouchIndexAux = Convert.ToInt32(jsonDic["TouchIndexAux"]);
            InputKey0 = Convert.ToString(jsonDic["InputKey0"]);
            InputKey1 = Convert.ToString(jsonDic["InputKey1"]);
            HideGUI = Convert.ToBoolean(jsonDic["HideGUI"]);
            Priority = Convert.ToInt32(jsonDic["Priority"]);
        }

        public Texture2D FindTexture(string name)
        {
            const string resDir = "MobileResources/";
            var asset = Resources.Load<Texture2D>(resDir + name);

            if (asset)
            {
                return asset;
            }

#if UNITY_EDITOR

            var guids = AssetDatabase.FindAssets(name + " t:texture2D");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                var fileName = IO.GetFileNameWithoutExtension(path);

                if (fileName == name)
                {
                    return AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
                }
            }
#endif

            return null;
        }

        /// <summary>
        /// update controls called every frame
        /// </summary>
        public virtual void GameUpdate()
        {
        }

        /// <summary>
        /// gui draw callback
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// return axis of the control in case of stick or camera panel
        /// </summary>
        public virtual Vector2 GetInputAxis()
        {
            return Vector2.zero;
        }

        /// <summary>
        /// if true this control will stop updating other controls
        /// </summary>
        /// <returns></returns>
        public virtual bool AbortUpdateOtherControls()
        {
            return false;
        }

        /// <summary>
        /// detect touch index
        /// </summary>
        protected virtual void DetectTouches()
        {
            var touches = touchProcessor.GetActiveTouchCount();

            if (touches == 0)
            {
                TouchIndex = -1;
            }
            else
            {
                if (TouchIndex == -1)
                {
                    for (var i = 0; i < touches; i++)
                    {
                        var touch = touchProcessor.GetTouch(i);

                        if (touch.Status != TouchStatus.Invalid)
                        {
                            // detect screen position
                            if (IsSide(touch.StartPosition))
                            {
                                if (TouchIndex == -1)
                                {
                                    TouchIndex = i;
                                }
                            }
                        }
                    }
                }
            }
        }

        protected bool IsSide(Vector2 pos)
        {
            if (Side == ControlSide.Arbitrary)
            {
                return true;
            }

            if (pos.x < Screen.width*0.5f)
            {
                return Side == ControlSide.Left;
            }

            return Side == ControlSide.Right;
        }

        protected virtual void OnTouchDown()
        {
            if (Application.isEditor && !Application.isPlaying)
                return;

            InputManager.Instance.EnableInputGroup((InputGroup)DisableInputGroup, false);
        }

        protected virtual void OnTouchUp()
        {
            if (Application.isEditor && !Application.isPlaying)
                return;

            InputManager.Instance.EnableInputGroup((InputGroup)DisableInputGroup, true);
        }
    }
}
