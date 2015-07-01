// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using RG_GameCamera.Utils;
using RG_GameCamera.Events;
using UnityEditor;
using EventType = RG_GameCamera.Events.EventType;

namespace RG_GameCamera.Editor.Events
{
    [CustomEditor(typeof (CameraEvent))]
    public class EditorTriggerEvent : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var cameraEvent = this.target as CameraEvent;

            if (cameraEvent)
            {
                Utils.Separator("Select camera event", 20);

                var change = false;
                cameraEvent.Type = (RG_GameCamera.Events.EventType) Utils.EnumSelection("Events", cameraEvent.Type, ref change);

                switch (cameraEvent.Type)
                {
                    case EventType.ConfigMode:
                    {
                        change |= Utils.String("Config mode name", ref cameraEvent.StringParam0);
                        change |= Utils.Toggle("Restore on exit", ref cameraEvent.RestoreOnExit);

                        change |= Utils.Toggle("Restore on timeout", ref cameraEvent.RestoreOnTimeout);
                        if (cameraEvent.RestoreOnTimeout)
                        {
                            change |= Utils.SliderEdit("Restore timeout", 0, 10, ref cameraEvent.RestoreTimeout);
                        }
                    }
                    break;

                    case EventType.ConfigParam:
                    {
                        change |= Utils.String("Config parameter name", ref cameraEvent.StringParam0);

                        cameraEvent.ConfigParamValueType = (RG_GameCamera.Config.Config.ConfigValue)Utils.EnumSelection("Type", cameraEvent.ConfigParamValueType, ref change);
                        var floatParam = false;

                        switch (cameraEvent.ConfigParamValueType)
                        {
                            case RG_GameCamera.Config.Config.ConfigValue.Bool:
                                change |= Utils.Toggle("Config param value", ref cameraEvent.ConfigParamBool);
                                break;

                            case RG_GameCamera.Config.Config.ConfigValue.Range:
                                const float valmin = -100.0f;
                                const float valmax = 100.0f;
                                change |= Utils.SliderEdit("Config parameter value", valmin, valmax, ref cameraEvent.ConfigParamFloat);
                                floatParam = true;
                                break;

                            case RG_GameCamera.Config.Config.ConfigValue.Selection:
                                change |= Utils.String("Config parameter value", ref cameraEvent.StringParam1);
                                break;

                            case RG_GameCamera.Config.Config.ConfigValue.String:
                                change |= Utils.String("Config parameter value", ref cameraEvent.StringParam1);
                                break;

                            case RG_GameCamera.Config.Config.ConfigValue.Vector2:
                                change |= Utils.Vector2("Config parameter value", ref cameraEvent.ConfigParamVector2);
                                floatParam = true;
                                break;

                            case RG_GameCamera.Config.Config.ConfigValue.Vector3:
                                change |= Utils.Vector3("Config parameter value", ref cameraEvent.ConfigParamVector3);
                                floatParam = true;
                                break;
                        }

                        if (floatParam)
                        {
                            change |= Utils.Toggle("Smooth", ref cameraEvent.SmoothFloatParams);
                            if (cameraEvent.SmoothFloatParams)
                            {
                                change |= Utils.SliderEdit("Smooth time", 0.0f, 10.0f, ref cameraEvent.SmoothTimeout);
                            }
                        }
                    }
                    break;

                    case EventType.CustomMessage:
                    {
                        change |= Utils.GameObjectSelection("GameObject", ref cameraEvent.CustomObject);
                        change |= Utils.String("Function name on Enter", ref cameraEvent.StringParam0);
                        change |= Utils.String("Function name on Exit", ref cameraEvent.StringParam1);
                    }
                    break;

                    case EventType.Effect:
                    {
                        cameraEvent.CameraEffect = (Effects.Type)Utils.EnumSelection("Camera effect", cameraEvent.CameraEffect, ref change);
                    }
                    break;

                    case EventType.LookAt:
                    {
                        change |= Utils.Toggle("From", ref cameraEvent.LookAtFrom);
                        if (cameraEvent.LookAtFrom)
                        {
                            change |= Utils.TransformSelection("From Transform", ref cameraEvent.LookAtFromObject);
                        }
                        change |= Utils.Toggle("To", ref cameraEvent.LookAtTo);
                        if (cameraEvent.LookAtTo)
                        {
                            change |= Utils.TransformSelection("To Transform", ref cameraEvent.LookAtToObject);
                        }

                        if (cameraEvent.LookAtFrom || cameraEvent.LookAtTo)
                        {
                            change |= Utils.SliderEdit("Time", 0.0f, 10.0f, ref cameraEvent.SmoothTimeout);
                            change |= Utils.Toggle("Restore on exit", ref cameraEvent.RestoreOnExit);
                            change |= Utils.Toggle("Restore on timeout", ref cameraEvent.RestoreOnTimeout);
                            if (cameraEvent.RestoreOnTimeout)
                            {
                                change |= Utils.SliderEdit("Restore timeout", 0, 10, ref cameraEvent.RestoreTimeout);
                            }

                            if (cameraEvent.RestoreOnTimeout || cameraEvent.RestoreOnExit)
                            {
                                change |= Utils.Toggle("Restore configuration", ref cameraEvent.RestoreConfiguration);
                                if (cameraEvent.RestoreConfiguration)
                                {
                                    change |= Utils.String("Configuration name", ref cameraEvent.RestoreConfigurationName);
                                }
                            }
                        }
                    }
                    break;
                }

                if (change)
                {
                    EditorUtility.SetDirty(cameraEvent);
                }

                EditorGUILayout.Separator();
            }
        }
    }
}
