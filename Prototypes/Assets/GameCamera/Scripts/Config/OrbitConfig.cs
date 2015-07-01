// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config
{
    public class OrbitConfig : Config
    {
        /// <summary>
        /// initialize default config values
        /// </summary>
        public override void LoadDefault()
        {
            var defaultParams = new Dictionary<string, Param>
            {
                {"FOV",             new RangeParam {value = 60.0f, min = 20.0f, max = 100.0f }}, // field of view
                {"ZoomSpeed", new RangeParam {value = 2.0f, min = 0.0f, max = 10.0f}},      // speed of zooming, higher is faster
                {"RotationSpeedX", new RangeParam {value = 8.0f, min = 0.0f, max = 10.0f}}, // speed of rotating in X, higher is faster
                {"RotationSpeedY", new RangeParam {value = 5.0f, min = 0.0f, max = 10.0f}}, // speed of rotating in Y, higher is faster
                {"PanSpeed", new RangeParam {value = 1.0f, min = 0.0f, max = 10.0f}},       // speed of panning

                {"RotationYMax", new RangeParam {value = 90, min = 0, max = 90}},           // rotation limit in z-up axix
                {"RotationYMin", new RangeParam {value = -90, min = -90, max = 0}},         // rotation limit in z-down axis

                {"DisablePan", new BoolParam {value = false}},
                {"DisableZoom", new BoolParam {value = false}},
                {"DisableRotation", new BoolParam {value = false}},
                {"TargetInterpolation", new RangeParam { value = 0.5f, min = 0.0f, max = 1.0f }},
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
