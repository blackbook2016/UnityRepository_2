// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config
{
    public class DebugConfig : Config
    {
        /// <summary>
        /// initialize default config values
        /// </summary>
        public override void LoadDefault()
        {
            var defaultParams = new Dictionary<string, Param>
            {
                {"FOV",                 new RangeParam {value = 60.0f, min = 20.0f, max = 100.0f }}, // field of view
                {"Orthographic",        new BoolParam { value = false }}, // enable orthographic projection
                {"RotationSpeedX", new RangeParam {value = 8.0f, min = 0.0f, max = 10.0f}}, // speed of rotating in X, higher is faster
                {"RotationSpeedY", new RangeParam {value = 5.0f, min = 0.0f, max = 10.0f}}, // speed of rotating in Y, higher is faster
                {"MoveSpeed", new RangeParam {value = 0.5f, min = 0.0f, max = 1.0f}}, // speed of movement
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
