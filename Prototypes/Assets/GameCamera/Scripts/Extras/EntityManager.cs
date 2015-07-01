// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using RG_GameCamera.CharacterController;
using UnityEngine;

namespace RG_GameCamera.Extras
{
    /// <summary>
    /// simple manager for enumerating all enemies and it's positions
    /// </summary>
    class EntityManager
    {
        public static EntityManager Instance { get { return instance ?? (instance = new EntityManager()); } }
        private static EntityManager instance;

        public HitEntity Player
        {
            get
            {
                foreach (Human player in players)
                {
                    if (!player.Remote && player)
                        return player;
                }

                return null;
            }
        }

        private readonly HashSet<HitEntity> enemies;

        private readonly HashSet<HitEntity> players;

        private EntityManager()
        {
            enemies = new HashSet<HitEntity>();
            players = new HashSet<HitEntity>();
        }

        /// <summary>
        /// registration
        /// </summary>
        /// <param name="enemy">register new entity on born</param>
        public void Register(HitEntity enemy)
        {
            enemies.Add(enemy);
        }

        /// <summary>
        /// register player entity
        /// </summary>
        public void RegisterPlayer(HitEntity player)
        {
            players.Add(player);
        }

        /// <summary>
        /// removes enemy from the list
        /// </summary>
        public void OnDeath(HitEntity enemy)
        {
            enemies.Remove(enemy);
        }

        /// <summary>
        /// find closest enemy in position within radius
        /// </summary>
        /// <param name="pos">position to look from</param>
        /// <param name="radius">radius of search</param>
        /// <param name="ignoreTag">ignore entities with specified tag</param>
        /// <returns>returns enemy if found, null otherwise</returns>
        public HitEntity Find(Vector3 pos, float radius, string ignoreTag)
        {
            var r2 = radius*radius;
            HitEntity closest = null;
            var dist2 = float.MaxValue;

            foreach (var hitEntity in enemies)
            {
                if (!hitEntity)
                    continue;

                // ignore entity with ignoreTag
                if (hitEntity.gameObject.CompareTag(ignoreTag))
                {
                    continue;
                }

                var dist = (pos - hitEntity.transform.position).sqrMagnitude;

                if (dist < r2 && dist < dist2)
                {
                    dist2 = dist;
                    closest = hitEntity;
                }
            }

            return closest;
        }
    }
}
