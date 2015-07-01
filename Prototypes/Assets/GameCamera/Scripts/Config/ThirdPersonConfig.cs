// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config
{
    public class ThirdPersonConfig : Config
    {
        /// <summary>
        /// initialize default config values
        /// </summary>
        public override void LoadDefault()
        {
            var defaultParams = new Dictionary<string, Param>
            {
                {"FOV",             new RangeParam {value = 60.0f, min = 20.0f, max = 100.0f }}, // field of view
                {"Distance",        new RangeParam {value = 2.0f, min = 0.0f, max = 10.0f }}, // distance from target
                {"RotationSpeedX",  new RangeParam {value = 8.0f, min = 0.0f, max = 10.0f }}, // speed of rotating in X, higher is faster
                {"RotationSpeedY",  new RangeParam {value = 5.0f, min = 0.0f, max = 10.0f }}, // speed of rotating in Y, higher is faster
                {"RotationYMax",    new RangeParam {value = 80, min = 0, max = 80}},          // rotation limit in z-up axix
                {"RotationYMin",    new RangeParam {value = -80, min = -80, max = 0}},        // rotation limit in z-down axis
                {"TargetOffset",    new Vector3Param { value = Vector3.zero }},               // offset from the target
                {"FollowCoef",      new RangeParam { value = 1.0f, min = 0.0f, max = 10.0f}}, // how much the camera will follow the player
                {"Spring",          new RangeParam { value = 0.0f, min = 0.0f, max = 1.0f }}, // spring movement  
                {"DefaultYRotation",new RangeParam { value = 0.0f, min = -80.0f, max = 80.0f }}, // default Y rotation
                {"AutoYRotation",   new RangeParam { value = 0.0f, min = 0.0f, max = 1.0f }}, // automatic Y angle rotation
                {"DeadZone",        new Vector2Param { value = Vector2.zero }},               // ellipsoid zone with now movement
                {"Orthographic",    new BoolParam { value = false }}, // enable orthographic projection
            };

            var crouch = new Dictionary<string, Param>
            {
                {"FOV",             new RangeParam {value = 60.0f, min = 20.0f, max = 100.0f }}, // field of view
                {"Distance",        new RangeParam {value = 2.0f, min = 0.0f, max = 10.0f }}, // distance from target
                {"RotationSpeedX",  new RangeParam {value = 8.0f, min = 0.0f, max = 10.0f }}, // speed of rotating in X, higher is faster
                {"RotationSpeedY",  new RangeParam {value = 5.0f, min = 0.0f, max = 10.0f }}, // speed of rotating in Y, higher is faster
                {"RotationYMax",    new RangeParam {value = 80, min = 0, max = 80}},          // rotation limit in z-up axix
                {"RotationYMin",    new RangeParam {value = -80, min = -80, max = 0}},        // rotation limit in z-down axis
                {"TargetOffset",    new Vector3Param { value = Vector3.zero }},               // offset from the target
                {"FollowCoef",      new RangeParam { value = 1.0f, min = 0.0f, max = 10.0f}}, // how much the camera will follow the player
                {"Spring",          new RangeParam { value = 0.0f, min = 0.0f, max = 1.0f }}, // spring movement  
                {"DefaultYRotation",new RangeParam { value = 0.0f, min = -80.0f, max = 80.0f }}, // default Y rotation
                {"AutoYRotation",   new RangeParam { value = 0.0f, min = 0.0f, max = 1.0f }}, // automatic Y angle rotation
                {"DeadZone",        new Vector2Param { value = Vector2.zero }},               // ellipsoid zone with now movement
                {"Orthographic",    new BoolParam { value = false }}, // enable orthographic projection
            };

            var aim = new Dictionary<string, Param>
            {
                {"FOV",             new RangeParam {value = 40.0f, min = 20.0f, max = 100.0f }}, // field of view
                {"Distance",        new RangeParam {value = 2.0f, min = 0.0f, max = 10.0f }}, // distance from target
                {"RotationSpeedX",  new RangeParam {value = 8.0f, min = 0.0f, max = 10.0f }}, // speed of rotating in X, higher is faster
                {"RotationSpeedY",  new RangeParam {value = 5.0f, min = 0.0f, max = 10.0f }}, // speed of rotating in Y, higher is faster
                {"RotationYMax",    new RangeParam {value = 80, min = 0, max = 80}},          // rotation limit in z-up axix
                {"RotationYMin",    new RangeParam {value = -80, min = -80, max = 0}},        // rotation limit in z-down axis
                {"TargetOffset",    new Vector3Param { value = Vector3.zero }},               // offset from the target
                {"FollowCoef",      new RangeParam { value = 0.0f, min = 0.0f, max = 10.0f}}, // how much the camera will follow the player
                {"Spring",          new RangeParam { value = 0.0f, min = 0.0f, max = 1.0f }}, // spring movement  
                {"DefaultYRotation",new RangeParam { value = 0.0f, min = -80.0f, max = 80.0f }}, // default Y rotation
                {"AutoYRotation",   new RangeParam { value = 0.0f, min = 0.0f, max = 1.0f }}, // automatic Y angle rotation
                {"DeadZone",        new Vector2Param { value = Vector2.zero }},               // ellipsoid zone with now movement
                {"Orthographic",    new BoolParam { value = false }}, // enable orthographic projection
            };

            var sprint = new Dictionary<string, Param>
            {
                {"FOV",             new RangeParam {value = 60.0f, min = 20.0f, max = 100.0f }}, // field of view
                {"Distance",        new RangeParam {value = 2.0f, min = 0.0f, max = 10.0f }}, // distance from target
                {"RotationSpeedX",  new RangeParam {value = 8.0f, min = 0.0f, max = 10.0f }}, // speed of rotating in X, higher is faster
                {"RotationSpeedY",  new RangeParam {value = 5.0f, min = 0.0f, max = 10.0f }}, // speed of rotating in Y, higher is faster
                {"RotationYMax",    new RangeParam {value = 80, min = 0, max = 80}},          // rotation limit in z-up axix
                {"RotationYMin",    new RangeParam {value = -80, min = -80, max = 0}},        // rotation limit in z-down axis
                {"TargetOffset",    new Vector3Param { value = Vector3.zero }},               // offset from the target
                {"FollowCoef",      new RangeParam { value = 1.0f, min = 0.0f, max = 10.0f}}, // how much the camera will follow the player
                {"Spring",          new RangeParam { value = 0.0f, min = 0.0f, max = 1.0f }}, // spring movement  
                {"DefaultYRotation",new RangeParam { value = 0.0f, min = -80.0f, max = 80.0f }}, // default Y rotation
                {"AutoYRotation",   new RangeParam { value = 0.0f, min = 0.0f, max = 1.0f }}, // automatic Y angle rotation
                {"DeadZone",        new Vector2Param { value = Vector2.zero }},               // ellipsoid zone with now movement
                {"Orthographic",    new BoolParam { value = false }}, // enable orthographic projection
            };

            Params = new Dictionary<string, Dictionary<string, Param>>
            { 
                { "Default", defaultParams },
                { "Crouch", crouch },
                { "Aim", aim },
                { "Sprint", sprint },
            };

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
