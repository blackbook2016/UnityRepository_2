// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;
using System.Collections;

namespace RG_GameCamera.Extras
{
    public class BloodSFX : MonoBehaviour
    {
        public static BloodSFX Instance;

        private ParticleEmitter[] emmiters;

        void Awake()
        {
            emmiters = GetComponentsInChildren<ParticleEmitter>();
            Instance = this;
        }

        public void Emit(Vector3 pos)
        {
            transform.position = pos;

            foreach (var emitter in emmiters)
            {
                emitter.Emit();
            }
        }
    }
}
