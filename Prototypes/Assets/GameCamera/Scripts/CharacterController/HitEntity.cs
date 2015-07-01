// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using RG_GameCamera.Extras;
using UnityEngine;

namespace RG_GameCamera.CharacterController
{
    /// <summary>
    /// basic entity in the demo with Health atribute
    /// </summary>
    public class HitEntity : MonoBehaviour
    {
        /// <summary>
        /// immortality flag
        /// </summary>
        public bool Immortal;

        /// <summary>
        /// healt points
        /// </summary>
        public float Health;

        /// <summary>
        /// dead flag
        /// </summary>
        protected bool Dead;

        /// <summary>
        /// is this entity dead?
        /// </summary>
        public bool IsDead { get { return Dead; } }

        /// <summary>
        /// is this enemy?
        /// </summary>
        public bool Enemy;

        /// <summary>
        /// unity start
        /// </summary>
        protected virtual void Start()
        {
            EntityManager.Instance.Register(this);
        }

        /// <summary>
        /// unity update
        /// </summary>
        protected virtual void Update()
        {
        }

        /// <summary>
        /// unity fixed update
        /// </summary>
        protected virtual void FixedUpdate()
        {
        }

        /// <summary>
        /// reset health and dead status
        /// </summary>
        public void Resurect()
        {
            if (Dead)
            {
                Health = 100;
                Dead = false;
                EntityManager.Instance.Register(this);
            }
        }

        /// <summary>
        /// force die
        /// </summary>
        public void Die()
        {
            OnHit(null, Health*2.0f, Vector3.zero);
        }

        /// <summary>
        /// callback called when somebody shoot/hit/damage this entity
        /// </summary>
        public virtual void OnHit(HitEntity owner, float damage, Vector3 hitPos)
        {
            if (Dead || owner == this)
            {
                return;
            }

            if (!Immortal)
            {
                Health -= damage;

                if (Health < 0.0f)
                {
                    Dead = true;
                    OnDie();
                }
            }

            if (Extras.BloodSFX.Instance)
            {
                Extras.BloodSFX.Instance.Emit(hitPos);
            }
        }

        public virtual bool OnHealthPickUp(float hp)
        {
            if (!Dead)
            {
                if (Health < 100)
                {
                    Health += hp;

                    if (Health > 100.0f)
                    {
                        Health = 100.0f;
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnDie()
        {
            EntityManager.Instance.OnDeath(this);
        }
    }
}
