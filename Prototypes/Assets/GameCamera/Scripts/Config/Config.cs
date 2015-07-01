// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System;
using System.Collections.Generic;
using RG_GameCamera.ThirdParty;
using UnityEngine;

namespace RG_GameCamera.Config
{
    public abstract partial class Config : MonoBehaviour
    {
        /// <summary>
        /// list of all config parameters per camera mode
        /// key is a name of camera mode, eg.: Stand, Aim, Run, Sprint
        /// value is a dictionary of parameters:
        /// key = name of the parameter
        /// value = can be bool or array[3] of float
        /// in case of float first value is current value, second value is min, third value is max
        /// examples:
        /// 
        /// Param = { "Aim",   {"Distance", new [] { 5.0f, 0.0f, 100.0f } }
        /// Param = { "Stand", {"FollowTarget", true } }
        ///
        /// </summary>
        public Dictionary<string, Dictionary<string, Param>> Params;

        /// <summary>
        /// transition table with interpolation times between modes
        /// </summary>
        public Dictionary<string, float> Transitions;

        /// <summary>
        /// initialize default config values
        /// </summary>
        public virtual void LoadDefault()
        {
            currentMode = "Default";
            CopyParams(Params[currentMode], ref currParams);
            CopyParams(Params[currentMode], ref oldParams);
        }

        /// <summary>
        /// path to default configuration file
        /// </summary>
        public string DefaultConfigPath  { get { return ResourceDir + GetType().Name + ".json"; } }

        /// <summary>
        /// resource directory with all the config files
        /// </summary>
        public string ResourceDir { get { return Application.dataPath + "/GameCamera/Resources/Config/"; } }

        /// <summary>
        /// resource directory relative to assets folder
        /// </summary>
        public string ResourceDirRel { get { return "Config/"; } }

        /// <summary>
        /// config file as resource asset, resource text asset is created automatically on saving/loading new config
        /// </summary>
        public TextAsset ResourceAsset;

        /// <summary>
        /// transition callback
        /// </summary>
        public delegate void OnTransitMode(string newMode, float t);

        /// <summary>
        /// transition start callback
        /// </summary>
        public delegate void OnTransitionStart(string oldMode, string newMode);

        /// <summary>
        /// progress transition callback
        /// </summary>
        public OnTransitMode TransitCallback;

        /// <summary>
        /// start of transition callback
        /// </summary>
        public OnTransitionStart TransitionStartCallback;

        /// <summary>
        /// index of selected editor gui item for camera mode
        /// </summary>
        public int ModeIndex;

        /// <summary>
        /// current mode of the config
        /// </summary>
        protected string currentMode;

        private float transitionTime;
        private Dictionary<string, Param> currParams = new Dictionary<string, Param>();
        private Dictionary<string, Param> oldParams = new Dictionary<string, Param>();

        /// <summary>
        /// unity c-tor
        /// </summary>
        protected virtual void Awake()
        {
        }

        /// <summary>
        /// set camera mode by name
        /// </summary>
        /// <param name="mode">name of the camera mode</param>
        public bool SetCameraMode(string mode)
        {
            if (Params.ContainsKey(mode) && mode != currentMode)
            {
                if (TransitionStartCallback != null)
                {
                    TransitionStartCallback(currentMode, mode);
                }

                currentMode = mode;
                transitionTime = 0.0f;
                CopyParams(currParams, ref oldParams);

                return true;
            }

            return false;
        }

        /// <summary>
        /// return current camera config mode
        /// </summary>
        public string GetCurrentMode()
        {
            return currentMode;
        }

        /// <summary>
        /// get normalized transition time for key
        /// </summary>
        /// <param name="key">name of transition</param>
        float GetTransitionTime(string key)
        {
            var t = transitionTime/Transitions[key];
            if (t > 1.0f)
            {
                t = 1.0f;
            }
            return t;
        }

        void Update()
        {
            // update transitions between modes
            transitionTime += Time.deltaTime;

            var newParam = Params[currentMode];

            var t = GetTransitionTime(currentMode);

            if (t > 0.0f && t < 1.0f)
            {
                if (TransitCallback != null)
                {
                    TransitCallback(currentMode, t);
                }
            }

            foreach (var modes in Params.Values)
            {
                foreach (var key in modes.Keys)
                {
                    currParams[key].Interpolate(oldParams[key], newParam[key], t);
                }
                break;
            }
        }

        void CopyParams(Dictionary<string, Param> src, ref Dictionary<string, Param> dst)
        {
            foreach (var param in src)
            {
                Param val;
                if (dst.TryGetValue(param.Key, out val))
                {
                    val.Set(param.Value);
                }
                else
                {
                    dst.Add(param.Key, param.Value.Clone());
                }
            }
        }

        /// <summary>
        /// get bool parameter by name
        /// </summary>
        /// <param name="mode">camera mode</param>
        /// <param name="key">name of parameter</param>
        /// <returns>value of parameter</returns>
        public bool GetBool(string mode, string key)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    return ((BoolParam)value).value;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
            return false;
        }

        /// <summary>
        /// get bool parameter by name from current mode
        /// </summary>
        public bool GetBool(string key)
        {
            return GetBool(currentMode, key);
        }

        /// <summary>
        /// check wheater there is a boolean parameter
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>true if there is a boolean parameter</returns>
        public bool IsBool(string key)
        {
            if (Params.ContainsKey(currentMode))
            {
                return Params[currentMode].ContainsKey(key);
            }
            else
            {
                Utils.Debug.Log("ISBool error: " + currentMode.ToString());
                return false;
            }
        }

        /// <summary>
        /// set bool parameter by name
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <param name="inputValue">input bool</param>
        public void SetBool(string mode, string key, bool inputValue)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    var val = ((BoolParam) value);
                    val.value = inputValue;
                    camMode[key] = val;
                    return;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
        }

        /// <summary>
        /// get float parameter by name
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <returns>value of parameter</returns>
        public float GetFloat(string mode, string key)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    return ((RangeParam) value).value;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
            return -1;
        }

        /// <summary>
        /// get float parameter by name from current mode
        /// </summary>
        public float GetFloat(string key)
        {
            return ((RangeParam) currParams[key]).value;
        }

        public bool IsFloat(string key)
        {
            if (currParams != null && currParams.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// get float minimum parameter by name
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <returns>value of parameter</returns>
        public float GetFloatMin(string mode, string key)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    return ((RangeParam) value).min;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
            return -1;
        }

        /// <summary>
        /// get float minimum parameter by name from current mode
        /// </summary>
        public float GetFloatMin(string key)
        {
            return GetFloatMin(currentMode, key);
        }

        /// <summary>
        /// get float maximum parameter by name
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <returns>value of parameter</returns>
        public float GetFloatMax(string mode, string key)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    return ((RangeParam) value).max;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
            return -1;
        }

        /// <summary>
        /// get float maximum parameter by name from current mode
        /// </summary>
        public float GetFloatMax(string key)
        {
            return GetFloatMax(currentMode, key);
        }

        /// <summary>
        /// set float parameter by name
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <param name="inputValue">input float</param>
        public void SetFloat(string mode, string key, float inputValue)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    var val = ((RangeParam) value);
                    val.value = inputValue;
                    camMode[key] = val;
                    return;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
        }

        /// <summary>
        /// get Vector3 parameter by name
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <returns>value of parameter</returns>
        public Vector3 GetVector3(string mode, string key)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    return ((Vector3Param) value).value;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
            return Vector3.zero;
        }

        /// <summary>
        /// get Vector3 parameter by name from current mode
        /// </summary>
        public Vector3 GetVector3(string key)
        {
            return ((Vector3Param)currParams[key]).value;
        }

        public bool IsVector3(string key)
        {
            if (currParams != null && currParams.ContainsKey(key))
            {
                return true;
            }

            return false;
        }

        public Vector3 GetVector3Direct(string key)
        {
            return ((Vector3Param) (Params[currentMode][key])).value;
        }

        /// <summary>
        /// set Vector3 parameter by name
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <param name="inputValue">input Vector3</param>
        public void SetVector3(string mode, string key, Vector3 inputValue)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    var val = ((Vector3Param) value);
                    val.value = inputValue;
                    camMode[key] = val;
                    return;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
        }

        /// <summary>
        /// get Vector2 parameter by name
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <returns>value of parameter</returns>
        public Vector2 GetVector2(string mode, string key)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    return ((Vector2Param) value).value;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
            return Vector2.zero;
        }

        /// <summary>
        /// get Vector2 parameter by name from current mode
        /// </summary>
        public Vector2 GetVector2(string key)
        {
            return ((Vector2Param)currParams[key]).value;
        }

        /// <summary>
        /// set Vector2 parameter by name
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <param name="inputValue">input Vector3</param>
        public void SetVector2(string mode, string key, Vector2 inputValue)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    var val = ((Vector2Param) value);
                    val.value = inputValue;
                    camMode[key] = val;
                    return;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
        }

        /// <summary>
        /// get string parameter by name
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <returns>value of parameter</returns>
        public string GetString(string mode, string key)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    return ((StringParam) value).value;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
            return null;
        }

        /// <summary>
        /// get string parameter by name from current mode
        /// </summary>
        public string GetString(string key)
        {
            return GetString(currentMode, key);
        }

        /// <summary>
        /// set Vector2 parameter by name
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <param name="inputValue">input string</param>
        public void SetString(string mode, string key, string inputValue)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    var val = ((StringParam) value);
                    val.value = inputValue;
                    camMode[key] = val;
                    return;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
        }

        /// <summary>
        /// get selection (string) parameter by name
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <returns>value of parameter</returns>
        public string GetSelection(string mode, string key)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    var sel = ((SelectionParam) value);

                    return sel.value[sel.index];
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
            return null;
        }

        /// <summary>
        /// get selection parameter by name from current mode
        /// </summary>
        public string GetSelection(string key)
        {
            return GetSelection(currentMode, key);
        }

        /// <summary>
        /// set selection (string) parameter by index
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <param name="inputValue">input index</param>
        public void SetSelection(string mode, string key, int inputValue)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    var val = ((SelectionParam) value);
                    val.index = inputValue;
                    camMode[key] = val;
                    return;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
        }

        /// <summary>
        /// set selection (string) parameter by name
        /// </summary>
        /// <param name="key">name of parameter</param>
        /// <param name="mode">camera mode</param>
        /// <param name="inputValue">input index</param>
        public void SetSelection(string mode, string key, string inputValue)
        {
            Dictionary<string, Param> camMode;
            if (Params.TryGetValue(mode, out camMode))
            {
                Param value;
                if (camMode.TryGetValue(key, out value))
                {
                    var val = ((SelectionParam)value);
                    var index = val.Find(inputValue);
                    if (index != -1)
                    {
                        val.index = index;
                        camMode[key] = val;
                    }
                    return;
                }
            }

            Utils.Debug.Assert(false, "Not a valid config key: " + key);
        }

        /// <summary>
        /// create new configuration mode
        /// </summary>
        /// <param name="cfgName">name of configuration</param>
        public void AddMode(string cfgName)
        {
            var def = Params["Default"];
            var newParams = new Dictionary<string, Param>(def.Count);
            CopyParams(def, ref newParams);
            Params.Add(cfgName, newParams);
            Transitions.Add(cfgName, 0.25f);
        }

        /// <summary>
        /// remove configuration mode by name
        /// </summary>
        /// <param name="cfgName">name of configuration to be removed</param>
        public void DeleteMode(string cfgName)
        {
            Params.Remove(cfgName);
            Transitions.Remove(cfgName);
            ModeIndex = 0;
        }

        /// <summary>
        /// save parameters to file in JSON format
        /// </summary>
        /// <param name="file">destination file</param>
        public void Serialize(string file)
        {
            var serConfig = new Dictionary<string, object>(Params.Count);

            foreach (var param in Params)
            {
                var serParams = new Dictionary<string, object>(param.Value.Count);

                foreach (var configItem in param.Value)
                {
                    var itemSerialized = configItem.Value.Serialize();
                    serParams.Add(configItem.Key, itemSerialized);
                }

                serConfig.Add(param.Key, serParams);
            }

            if (Params.Count > 1)
            {
                serConfig.Add("Transitions", Transitions);
            }

            var jsonString = Json.Serialize(serConfig);

            if (!string.IsNullOrEmpty(jsonString))
            {
                Utils.IO.WriteTextFile(file, jsonString);
            }
        }

        /// <summary>
        /// load parameters from JSON file
        /// </summary>
        /// <param name="file">source file</param>
        public void Deserialize(string file)
        {
            var fileContent = Utils.IO.ReadTextFile(file);

            // try to load config from resources
            if (string.IsNullOrEmpty(fileContent))
            {
                if (!ResourceAsset)
                {
                    RefreshResourceAsset();
                }

                if (ResourceAsset)
                {
                    fileContent = ResourceAsset.text;
                }
            }

            if (!string.IsNullOrEmpty(fileContent))
            {
                var deserialized = Json.Deserialize(fileContent) as Dictionary<string, object>;

                if (deserialized != null)
                {
                    foreach (var mode in deserialized)
                    {
                        var cfgParams = mode.Value as Dictionary<string, object>;

                        if (cfgParams != null)
                        {
                            foreach (var param in cfgParams)
                            {
                                if (mode.Key == "Transitions")
                                {
                                    Transitions[param.Key] = Convert.ToSingle(param.Value);
                                    continue;
                                }

                                var value = param.Value as List<object>;
                                Utils.Debug.Assert(value != null);
                                var array = value.ToArray();

                                var type = (ConfigValue)Enum.Parse(typeof(ConfigValue), value[0] as string);

                                Param desValue = null;

                                switch (type)
                                {
                                    case ConfigValue.Bool:
                                        desValue = new BoolParam();
                                        break;

                                    case ConfigValue.Range:
                                        desValue = new RangeParam();
                                        break;

                                    case ConfigValue.Selection:
                                        desValue = new SelectionParam();
                                        break;

                                    case ConfigValue.String:
                                        desValue = new StringParam();
                                        break;

                                    case ConfigValue.Vector2:
                                        desValue = new Vector2Param();
                                        break;

                                    case ConfigValue.Vector3:
                                        desValue = new Vector3Param();
                                        break;
                                }

                                Utils.Debug.Assert(desValue != null);
                                desValue.Deserialize(array);

                                if (!Params.ContainsKey(mode.Key))
                                {
                                    Params[mode.Key] = new Dictionary<string, Param>();
                                }

                                Params[mode.Key][param.Key] = desValue;
                            }
                        }
                    }
                }
            }
        }

        public void RefreshResourceAsset()
        {
            ResourceAsset = Resources.Load<TextAsset>(ResourceDirRel + GetType().Name);
            Utils.Debug.Assert(ResourceAsset != null, "Resource asset is null!");
        }
    }
}
