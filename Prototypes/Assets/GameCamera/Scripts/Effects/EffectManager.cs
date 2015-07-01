// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;

namespace RG_GameCamera.Effects
{
    /// <summary>
    /// effect manager responsible for updating camera effects
    /// </summary>
    class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance { get { return instance; } }

        private static EffectManager instance;
        private List<Effect> effects;

        void Awake()
        {
            instance = this;
            effects = new List<Effect>();
        }

        /// <summary>
        /// effect factory
        /// </summary>
        public void Register(Effect effect)
        {
            if (effect != null)
            {
                effects.Add(effect);
            }
        }

        /// <summary>
        /// stop all effects
        /// </summary>
        public void StopAll()
        {
            foreach (var effect in effects)
            {
                effect.Stop();
            }
        }

        /// <summary>
        /// create and return new effect
        /// </summary>
        public T Create<T>() where T : Effect
        {
            var effect = gameObject.GetComponent<T>();

            if (!effect)
            {
                effect = gameObject.AddComponent<T>();

                if (effect)
                {
                    Register(effect);
                    effect.Init();
                }
            }

            return effect;
        }

        /// <summary>
        /// create and return new effect by type
        /// </summary>
        /// <param name="effectType">type of effect</param>
        /// <returns>new effect</returns>
        public Effect Create(Type effectType)
        {
            switch (effectType)
            {
                case Type.Earthquake:
                    return Create<Earthquake>();
                case Type.Explosion:
                    return Create<Explosion>();
                case Type.No:
                    return Create<No>();
                case Type.FireKick:
                    return Create<FireKick>();
                case Type.Stomp:
                    return Create<Stomp>();
                case Type.Yes:
                    return Create<Yes>();
                case Type.SprintShake:
                    return Create<SprintShake>();
            }

            Utils.Debug.Assert(false);
            return null;
        }

        /// <summary>
        /// delete effect
        /// </summary>
        public void Delete(Effect effect)
        {
            if (effects.Contains(effect))
            {
                effects.Remove(effect);
            }
        }

        /// <summary>
        /// update all active effects
        /// </summary>
        public void PostUpdate()
        {
            foreach (var effect in effects)
            {
                if (effect.Playing)
                {
                    effect.PostUpdate();
                }
            }
        }

        void OnGUI()
        {
        }
    }
}
