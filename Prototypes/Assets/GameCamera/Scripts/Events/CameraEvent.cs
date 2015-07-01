// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using RG_GameCamera.Modes;
using RG_GameCamera.Effects;
using UnityEngine;
using Type = RG_GameCamera.Modes.Type;

namespace RG_GameCamera.Events
{
    public enum EventType
    {
        /// <summary>
        /// run camera effect
        /// </summary>
        Effect,

        /// <summary>
        /// change single config parameter in current camera mode
        /// for example change distance in third person camera from current to 100
        /// </summary>
        ConfigParam,

        /// <summary>
        /// change config mode in current camera
        /// for example change third person camera mode Default to Aim
        /// </summary>
        ConfigMode,

        /// <summary>
        /// change camera mode to LookAt and use this mode for looking at some point
        /// </summary>
        LookAt,

        /// <summary>
        /// send message to other game object
        /// </summary>
        CustomMessage,
    }

    /// <summary>
    /// camera event is a area with box collider that triggers various camera event (effect, change of paramater, etc)
    /// if the object (character) with camera trigger component hits it
    /// </summary>
    [RequireComponent(typeof (BoxCollider))]
    public partial class CameraEvent : MonoBehaviour
    {
        public EventType Type;
        public Modes.Type CameraMode;
        public string StringParam0;
        public string StringParam1;
        public Config.Config.ConfigValue ConfigParamValueType;
        public bool ConfigParamBool;
        public string ConfigParamString;
        public float ConfigParamFloat;
        public Vector2 ConfigParamVector2;
        public Vector3 ConfigParamVector3;
        public Effects.Type CameraEffect;
        public GameObject CustomObject;
        public bool RestoreOnExit;
        public bool SmoothFloatParams;
        public float SmoothTimeout;
        public bool LookAtFrom;
        public bool LookAtTo;
        public Transform LookAtFromObject;
        public Transform LookAtToObject;
        public bool RestoreOnTimeout;
        public float RestoreTimeout;
        public bool RestoreConfiguration;
        public string RestoreConfigurationName;

        private Collider cameraTrigger;
        private object oldParam0;
        private object oldParam1;
        private object oldParam2;
        private float restorationTimeout;

        private bool paramChanged;

        /// <summary>
        /// Unity callback when some game object entered this collider
        /// </summary>
        /// <param name="other">other collider</param>
        private void OnTriggerEnter(Collider other)
        {
            if (other && other.gameObject)
            {
                var trigger = other.gameObject.GetComponent<CameraTrigger>();

                if (trigger)
                {
                    if (cameraTrigger)
                    {
                        return;
                    }

                    cameraTrigger = other;

                    switch (Type)
                    {
                        case EventType.ConfigMode:
                        {
                            var cfg = CameraManager.Instance.GetCameraMode().Configuration;

                            if (cfg && !string.IsNullOrEmpty(StringParam0))
                            {
                                oldParam0 = cfg.GetCurrentMode();
                                if ((string) oldParam0 != StringParam0)
                                {
                                    paramChanged = cfg.SetCameraMode(StringParam0);
                                }
                            }
                        }
                        break;

                        case EventType.ConfigParam:
                        {
                            var cfg = CameraManager.Instance.GetCameraMode().Configuration;
                            var cfgMode = cfg.GetCurrentMode();
                            oldParam2 = cfgMode;

                            if (cfg && !string.IsNullOrEmpty(StringParam0))
                            {
                                oldParam0 = StringParam0;

                                switch (ConfigParamValueType)
                                {
                                    case Config.Config.ConfigValue.Bool:
                                        oldParam1 = cfg.GetBool(cfgMode, StringParam0);
                                        cfg.SetBool(cfgMode, StringParam0, ConfigParamBool);
                                        break;

                                    case Config.Config.ConfigValue.Range:
                                        oldParam1 = cfg.GetFloat(cfgMode, StringParam0);
                                        if (SmoothFloatParams)
                                        {
                                            SmoothParam(cfgMode, StringParam0, (float) oldParam1, ConfigParamFloat,
                                                        SmoothTimeout);
                                        }
                                        else
                                        {
                                            cfg.SetFloat(cfgMode, StringParam0, ConfigParamFloat);
                                        }
                                        break;

                                    case Config.Config.ConfigValue.Selection:
                                        oldParam1 = cfg.GetSelection(cfgMode, StringParam0);
                                        cfg.SetSelection(cfgMode, StringParam0, StringParam1);
                                        break;

                                    case Config.Config.ConfigValue.String:
                                        oldParam1 = cfg.GetString(cfgMode, StringParam0);
                                        cfg.SetString(cfgMode, StringParam0, StringParam1);
                                        break;

                                    case Config.Config.ConfigValue.Vector2:
                                        oldParam1 = cfg.GetVector2(cfgMode, StringParam0);
                                        if (SmoothFloatParams)
                                        {
                                            SmoothParam(cfgMode, StringParam0, (Vector2) oldParam1,
                                                        ConfigParamVector2, SmoothTimeout);
                                        }
                                        else
                                        {
                                            cfg.SetVector2(cfgMode, StringParam0, ConfigParamVector2);
                                        }
                                        break;

                                    case Config.Config.ConfigValue.Vector3:
                                        oldParam1 = cfg.GetVector3(cfgMode, StringParam0);
                                        if (SmoothFloatParams)
                                        {
                                            SmoothParam(cfgMode, StringParam0, (Vector3) oldParam1,
                                                        ConfigParamVector3, SmoothTimeout);
                                        }
                                        else
                                        {
                                            cfg.SetVector2(cfgMode, StringParam0, ConfigParamVector2);
                                        }
                                        break;
                                }
                            }
                        }
                        break;

                        case EventType.Effect:
                        {
                            var effect = EffectManager.Instance.Create(CameraEffect);
                            effect.Play();
                        }
                        break;

                        case EventType.CustomMessage:
                        {
                            if (CustomObject && !string.IsNullOrEmpty(StringParam0))
                            {
                                CustomObject.SendMessage(StringParam0);
                            }
                        }
                        break;

                        case EventType.LookAt:
                        {
                            if (LookAtFrom && !LookAtFromObject)
                                break;

                            if (LookAtTo && !LookAtToObject)
                                break;

                            if (LookAtTo || LookAtFrom)
                            {
                                oldParam0 = CameraManager.Instance.GetCameraMode().Type;

                                var lookAt = CameraManager.Instance.SetMode(Modes.Type.LookAt) as Modes.LookAtCameraMode;

                                if (LookAtFrom)
                                {
                                    if (LookAtTo)
                                    {
                                        lookAt.LookAt(LookAtFromObject.position, LookAtToObject.position, SmoothTimeout);
                                    }
                                    else
                                    {
                                        lookAt.LookFrom(LookAtFromObject.position, SmoothTimeout);
                                    }
                                }
                                else
                                {
                                    lookAt.LookAt(LookAtToObject.position, SmoothTimeout);
                                }
                            }
                        }
                        break;
                    }
                }

                if (RestoreOnTimeout)
                {
                    restorationTimeout = RestoreTimeout;
                }
            }
        }

        private void Exit(bool onTimeout, Collider other)
        {
            var exitProcedure = false;

            if (onTimeout)
            {
                exitProcedure = RestoreOnTimeout;
            }
            else
            {
                exitProcedure = RestoreOnExit && cameraTrigger == other;
            }

            if (!RestoreOnExit && !RestoreOnTimeout)
            {
                cameraTrigger = null;
            }

            if (exitProcedure)
            {
                cameraTrigger = null;

                switch (Type)
                {
                    case EventType.ConfigMode:
                        {
                            if (paramChanged)
                            {
                                var cfg = CameraManager.Instance.GetCameraMode().Configuration;
                                if (cfg && !string.IsNullOrEmpty((string) oldParam0))
                                {
                                    if ((string) oldParam0 != cfg.GetCurrentMode())
                                    {
                                        cfg.SetCameraMode((string) oldParam0);
                                    }
                                }
                            }
                        }
                        break;

                    case EventType.ConfigParam:
                        {
                            var cfg = CameraManager.Instance.GetCameraMode().Configuration;

                            if (cfg && !string.IsNullOrEmpty((string) oldParam0) && oldParam1 != null &&
                                !string.IsNullOrEmpty((string) oldParam2))
                            {
                                switch (ConfigParamValueType)
                                {
                                    case Config.Config.ConfigValue.Bool:
                                        cfg.SetBool((string) oldParam2, (string) oldParam0, (bool) oldParam1);
                                        break;

                                    case Config.Config.ConfigValue.Range:
                                        var curr = cfg.GetFloat((string) oldParam2, (string) oldParam0);
                                        if (SmoothFloatParams)
                                        {
                                            SmoothParam((string) oldParam2, (string) oldParam0, curr,
                                                        (float) oldParam1, SmoothTimeout);
                                        }
                                        else
                                        {
                                            cfg.SetFloat((string) oldParam2, (string) oldParam0, (float) oldParam1);
                                        }
                                        break;

                                    case Config.Config.ConfigValue.Selection:
                                        cfg.SetSelection((string) oldParam2, (string) oldParam0, (string) oldParam1);
                                        break;

                                    case Config.Config.ConfigValue.String:
                                        cfg.SetString((string) oldParam2, (string) oldParam0, (string) oldParam1);
                                        break;

                                    case Config.Config.ConfigValue.Vector2:
                                        var currv2 = cfg.GetVector2((string) oldParam2, (string) oldParam0);
                                        if (SmoothFloatParams)
                                        {
                                            SmoothParam((string) oldParam2, (string) oldParam0, currv2,
                                                        (Vector2) oldParam1, SmoothTimeout);
                                        }
                                        else
                                        {
                                            cfg.SetVector2((string) oldParam2, (string) oldParam0,
                                                           (Vector2) oldParam1);
                                        }
                                        break;

                                    case Config.Config.ConfigValue.Vector3:
                                        var currv3 = cfg.GetVector3((string) oldParam2, (string) oldParam0);
                                        if (SmoothFloatParams)
                                        {
                                            SmoothParam((string) oldParam2, (string) oldParam0, currv3,
                                                        (Vector3) oldParam1, SmoothTimeout);
                                        }
                                        else
                                        {
                                            cfg.SetVector3((string) oldParam2, (string) oldParam0,
                                                           (Vector3) oldParam1);
                                        }
                                        break;
                                }
                            }
                        }
                        break;

                    case EventType.CustomMessage:
                        if (CustomObject && !string.IsNullOrEmpty(StringParam1))
                        {
                            CustomObject.SendMessage(StringParam1);
                        }
                        break;

                    case EventType.LookAt:
                        {
                            if (oldParam0 is Modes.Type)
                            {
                                if (RestoreConfiguration && !string.IsNullOrEmpty(RestoreConfigurationName))
                                {
                                    CameraManager.Instance.SetDefaultConfiguration((Modes.Type) oldParam0, RestoreConfigurationName);
                                }
                                CameraManager.Instance.SetMode((Modes.Type) oldParam0);
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Unity callback when some game object left this collider
        /// </summary>
        /// <param name="other">other collider</param>
        private void OnTriggerExit(Collider other)
        {
            Exit(false, other);
        }
    }
}
