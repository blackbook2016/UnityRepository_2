// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config
{
    public class RTSConfig : Config
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
                {"EnableRotation",  new BoolParam  {value = true }}, // enable rotation?
                {"DefaultAngleX",   new RangeParam {value = 45, min = -180.0f, max = 180.0f }},// speed of zoom
                {"RotationSpeed",   new RangeParam {value = 8.0f, min = 0.0f, max = 10.0f  }}, // speed of rotating in X, higher is faster
                {"GroundOffset",    new RangeParam {value = 0.0f, min = -100.0f, max = 100.0f  }}, // offset from the ground plane (game plane)
                {"AngleY",          new RangeParam {value = 50.0f, min = 0.0f, max = 85.0f }}, // angle of Y rotation
                {"AngleZoomMin",    new RangeParam {value = 50.0f, min = 0.0f, max = 85.0f }}, // angle of Y rotation
                {"FollowTargetY",   new BoolParam  {value = true }}, // follow target Y
                {"DraggingMove",    new BoolParam  {value = true }}, // enable moving camera by mouse dragging
                {"ScreenBorderMove",new BoolParam  {value = true }}, // enable moving camera by mouse moving mouse on the border of the screen
                {"ScreenBorderOffset", new RangeParam {value = 10.0f, min = 1.0f, max = 500.0f }}, // offset from camera border
                {"ScreenBorderSpeed", new RangeParam  {value = 1.0f, min = 0.0f, max = 10.0f }}, // offset from camera border
                {"KeyMove",         new BoolParam  {value = true }}, // enable moving camera by keyboard or joystick
                {"MoveSpeed",       new RangeParam {value = 1.0f, min = 0.0f, max = 10.0f }}, // speed of camera movement
                {"MapCenter",       new Vector2Param {value = Vector2.zero }}, // center of the game map
                {"MapSize",         new Vector2Param {value = new Vector2(100, 100) }}, // size of the game map (width/height)
                {"SoftBorder",      new RangeParam { value = 5.0f, min = 0.0f, max = 20.0f}},
                {"DisableHorizontal", new BoolParam {value = false}},
                {"DisableVertical", new BoolParam {value = false}},
                {"DragMomentum",    new RangeParam {value = 1.0f, min = 0.0f, max = 3.0f}},
                {"Orthographic",    new BoolParam {value = false }}, // enable orthographic projection
                {"OrthoMin",        new RangeParam {value = 1.0f,  min = 0.0f, max = 100.0f }}, // minimal orthographic size
                {"OrthoMax",        new RangeParam {value = 50.0f, min = 0.0f, max = 100.0f }}, // maximal orthographic size
            };

            Params = new Dictionary<string, Dictionary<string, Param>>
            {
                { "Default", defaultParams },
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
