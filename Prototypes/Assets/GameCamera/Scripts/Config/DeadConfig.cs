// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config
{
    public class DeadConfig : Config
    {
        /// <summary>
        /// initialize default config values
        /// </summary>
        public override void LoadDefault()
        {
            var defaultParams = new Dictionary<string, Param>
            {
                {"FOV",             new RangeParam {value = 60.0f, min = 20.0f, max = 100.0f }}, // field of view
                {"Distance",        new RangeParam {value = 2.0f, min = 0.0f, max = 10.0f }},  // distance from target
                {"RotationSpeed",   new RangeParam { value = 0.5f, min = -10.0f, max = 10.0f }}, // rotation speed around the player
                {"Angle",           new RangeParam { value = 50.0f, min = 0.0f, max = 80.0f }},// angle looking at player
                {"TargetOffset",    new Vector3Param { value = Vector3.zero }}, // offset from the target
                {"Collision",       new BoolParam  { value = true }}, // use camera collision
                {"Orthographic",    new BoolParam { value = false }}, // enable orthographic projection
            };

            Params = new Dictionary<string, Dictionary<string, Param>> { { "Default", defaultParams } };
            Transitions = new Dictionary<string, float>();
            foreach (var param in Params)
            {
                Transitions.Add(param.Key, 0.25f);
            }
            Deserialize(DefaultConfigPath);

            base.LoadDefault();
        }

        protected override void Awake()
        {
            base.Awake();
            LoadDefault();
        }
    }
}
