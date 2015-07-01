// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;
using System.Collections;

namespace RG_GameCamera.CharacterController
{
    /// <summary>
    /// shooting particles from gun
    /// taken from Bootcamp demo
    /// </summary>
    public class GunSFX : MonoBehaviour
    {
        private ParticleEmitter[] emmiters;

        void Awake()
        {
            emmiters = GetComponentsInChildren<ParticleEmitter>();
        }

        public void Emit()
        {
            foreach (var emitter in emmiters)
            {
                emitter.Emit();
            }
        }
    }
}
