// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config
{
    public class FPSConfig : Config
    {
        /// <summary>
        /// initialize default config values
        /// </summary>
        public override void LoadDefault()
        {
            var defaultParams = new Dictionary<string, Param>
            {
                {"FOV",             new RangeParam {value = 60.0f, min = 20.0f, max = 100.0f }}, // field of view
                {"RotationSpeedX", new RangeParam {value = 8.0f, min = 0.0f, max = 10.0f}}, // speed of rotating in X, higher is faster
                {"RotationSpeedY", new RangeParam {value = 5.0f, min = 0.0f, max = 10.0f}}, // speed of rotating in Y, higher is faster
                {"RotationYMax", new RangeParam {value = 80, min = 0, max = 80}},           // rotation limit in z-up axix
                {"RotationYMin", new RangeParam {value = -80, min = -80, max = 0}},         // rotation limit in z-down axis
                {"TargetOffset",    new Vector3Param { value = Vector3.zero }},             // offset from the target
                {"Orthographic",    new BoolParam { value = false }},                       // enable orthographic projection
                {"HideTarget",      new BoolParam { value = true}},                         // hide the target (character)
            };

            var crouch = new Dictionary<string, Param>
            {
                {"FOV",             new RangeParam {value = 60.0f, min = 20.0f, max = 100.0f }}, // field of view
                {"RotationSpeedX", new RangeParam {value = 8.0f, min = 0.0f, max = 10.0f}}, // speed of rotating in X, higher is faster
                {"RotationSpeedY", new RangeParam {value = 5.0f, min = 0.0f, max = 10.0f}}, // speed of rotating in Y, higher is faster
                {"RotationYMax", new RangeParam {value = 80, min = 0, max = 80}},           // rotation limit in z-up axix
                {"RotationYMin", new RangeParam {value = -80, min = -80, max = 0}},         // rotation limit in z-down axis
                {"TargetOffset",    new Vector3Param { value = Vector3.zero }},             // offset from the target
                {"Orthographic",    new BoolParam { value = false }},                       // enable orthographic projection
                {"HideTarget",      new BoolParam { value = true}},                         // hide the target (character)
            };

            Params = new Dictionary<string, Dictionary<string, Param>> {{"Default", defaultParams}, {"Crouch", crouch}};
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
