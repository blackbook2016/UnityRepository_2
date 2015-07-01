// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.CharacterController
{
    /// <summary>
    /// simple audio manager for playing soundeffects like shots, foot steps, etc.
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        public AudioClip ShootClip;
        public AudioClip HitClip;
        public AudioClip FootStepClip;
        public AudioClip ReloadClip;
        public AudioClip HealthPickUpClip;

        public static SoundManager Instance
        {
            get { return instance; }
        }

        private static SoundManager instance;

        private void Awake()
        {
            instance = this;
        }

        public void PlayShoot(AudioSource source)
        {
            source.PlayOneShot(ShootClip);
        }

        public void PlayFootStep(AudioSource source)
        {
            source.PlayOneShot(FootStepClip);
        }

        public void PlayReload(AudioSource source)
        {
            source.PlayOneShot(ReloadClip);
        }

        public void PlayHit(AudioSource source)
        {
            source.PlayOneShot(HitClip);
        }

        public void PlayHealthPickUp(AudioSource source)
        {
            if (HealthPickUpClip)
            {
                source.PlayOneShot(HealthPickUpClip);
            }
        }
    }
}
