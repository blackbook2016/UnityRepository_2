// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Config
{
    public class CollisionConfig : Config
    {
        /// <summary>
        /// initialize default config values
        /// </summary>
        public override void LoadDefault()
        {
            var defaultParams = new Dictionary<string, Param>
            {
                {"MinDistance", new RangeParam {value = 0.5f, min = 0.0f, max = 10.0f}},
                {"ReturnSpeed", new RangeParam {value = 0.4f, min = 0.0f, max = 1.0f}},
                {"ClipSpeed", new RangeParam {value = 0.05f, min = 0.0f, max = 1.0f}},
                {"IgnoreCollisionTag", new StringParam { value = "Player" }},
                {"TransparentCollisionTag", new StringParam { value = "CameraTransparent"}},
                {"TargetSphereRadius", new RangeParam {value = 0.5f, min = 0, max = 1}},
                {"RaycastTolerance", new RangeParam { value = 0.5f, min = 0.0f, max = 1.0f}},
                {"TargetClipSpeed", new RangeParam { value = 0.1f, min = 0.0f, max = 1.0f}},
                {"ReturnTargetSpeed", new RangeParam { value = 0.1f, min = 0.0f, max = 1.0f}},
                {"SphereCastRadius", new RangeParam { value = 0.1f, min = 0.0f, max = 1.0f}},
                {"SphereCastTolerance", new RangeParam { value = 0.5f, min = 0.0f, max = 1.0f}},
                {"CollisionAlgorithm", new SelectionParam { index = 0, value = new [] {"Simple", "Spherical", "Volumetric"}}},
                {"ConeRadius", new Vector2Param { value = new Vector2(0.5f, 1.0f)}},
                {"ConeSegments", new RangeParam { value = 3.0f, min = 3.0f, max = 10.0f }},
                {"HeadOffset", new RangeParam { value = 1.6f, min = 0.0f, max = 10.0f }},
                {"NearClipPlane", new RangeParam { value = 0.01f, min = 0.0f, max = 1.0f}}
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
