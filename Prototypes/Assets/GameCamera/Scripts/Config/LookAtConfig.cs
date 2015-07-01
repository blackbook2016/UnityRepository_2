// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config
{
    public class LookAtConfig : Config
    {
        /// <summary>
        /// initialize default config values
        /// </summary>
        public override void LoadDefault()
        {
            var defaultParams = new Dictionary<string, Param>
            {
                {"FOV",                 new RangeParam {value = 60.0f, min = 20.0f, max = 100.0f }}, // field of view
                {"InterpolateTarget",  new BoolParam { value = true }},  // always look at target during interpolation
                {"Orthographic",        new BoolParam { value = false }}, // enable orthographic projection
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
