// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config
{
    public class RPGConfig : Config
    {
        /// <summary>
        /// initialize default config values
        /// </summary>
        public override void LoadDefault()
        {
            var defaultParams = new Dictionary<string, Param>
            {
                {"FOV",             new RangeParam {value = 60.0f, min = 20.0f, max = 100.0f }}, // field of view
                {"Distance",        new RangeParam {value = 10.0f, min = 0.0f, max = 100.0f }}, // distance from target
                {"DistanceMin",     new RangeParam {value = 2.0f, min = 0.0f, max = 100.0f }}, // distance from target
                {"DistanceMax",     new RangeParam {value = 50.0f, min = 0.0f, max = 100.0f }}, // distance from target
                {"ZoomSpeed",       new RangeParam {value = 0.5f, min = 0.0f, max = 10.0f }},// speed of zoom
                {"EnableZoom",      new BoolParam  {value = true }}, // enable zoom?
                {"DefaultAngleX",   new RangeParam {value = 45, min = -180.0f, max = 180.0f }},// speed of zoom
                {"EnableRotation",  new BoolParam  {value = true }}, // enable rotation?
                {"RotationSpeed",   new RangeParam {value = 8.0f, min = 0.0f, max = 10.0f  }}, // speed of rotating in X, higher is faster
                {"AngleY",          new RangeParam {value = 50.0f, min = 0.0f, max = 85.0f }}, // angle of Y rotation
                {"AngleZoomMin",    new RangeParam {value = 50.0f, min = 0.0f, max = 85.0f }}, // angle of Y rotation
                {"TargetOffset",    new Vector3Param { value = Vector3.zero }},               // offset from the target
                {"Spring",          new RangeParam { value = 0.0f, min = 0.0f, max = 1.0f }}, // spring movement
                {"DeadZone",        new Vector2Param { value = Vector2.zero }},               // ellipsoid zone with now movement
                {"Orthographic",    new BoolParam { value = false }}, // enable orthographic projection
                {"OrthoMin",        new RangeParam {value = 1.0f,  min = 0.0f, max = 100.0f }}, // minimal orthographic size
                {"OrthoMax",        new RangeParam {value = 50.0f, min = 0.0f, max = 100.0f }}, // maximal orthographic size
            };

            var interiorParams = new Dictionary<string, Param>
            {
                {"FOV",             new RangeParam {value = 60.0f, min = 20.0f, max = 100.0f }}, // field of view
                {"Distance",        new RangeParam {value = 10.0f, min = 0.0f, max = 100.0f }}, // distance from target
                {"DistanceMin",     new RangeParam {value = 2.0f, min = 0.0f, max = 100.0f }}, // distance from target
                {"DistanceMax",     new RangeParam {value = 50.0f, min = 0.0f, max = 100.0f }}, // distance from target
                {"ZoomSpeed",       new RangeParam {value = 0.5f, min = 0.0f, max = 10.0f }},// speed of zoom
                {"EnableZoom",      new BoolParam  {value = true }}, // enable zoom?
                {"DefaultAngleX",   new RangeParam {value = 45, min = -180.0f, max = 180.0f }},// speed of zoom
                {"EnableRotation",  new BoolParam  {value = true }}, // enable rotation?
                {"RotationSpeed",   new RangeParam {value = 8.0f, min = 0.0f, max = 10.0f  }}, // speed of rotating in X, higher is faster
                {"AngleY",          new RangeParam {value = 50.0f, min = 0.0f, max = 85.0f }}, // angle of Y rotation
                {"AngleZoomMin",    new RangeParam {value = 50.0f, min = 0.0f, max = 85.0f }}, // angle of Y rotation
                {"TargetOffset",    new Vector3Param { value = Vector3.zero }},               // offset from the target
                {"Spring",          new RangeParam { value = 0.0f, min = 0.0f, max = 1.0f }}, // spring movement
                {"DeadZone",        new Vector2Param { value = Vector2.zero }},               // ellipsoid zone with now movement
                {"Orthographic",    new BoolParam { value = false }}, // enable orthographic projection
                {"OrthoMin",        new RangeParam {value = 1.0f,  min = 0.0f, max = 100.0f }}, // minimal orthographic size
                {"OrthoMax",        new RangeParam {value = 50.0f, min = 0.0f, max = 100.0f }}, // maximal orthographic size
            };

            Params = new Dictionary<string, Dictionary<string, Param>>
            {
                { "Default", defaultParams },
                { "Interior", interiorParams },
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
