using UnityEngine;
using System.Collections;

namespace RG_GameCamera.Demo
{
    /// <summary>
    /// simple zombie/worm spawner
    /// </summary>
    public class ZombieSpawner : MonoBehaviour
    {
        public GameObject ZombiePrefab;
        public GameObject WormPrefab;

        public static ZombieSpawner Instance { get; private set; }
        private bool spawnHell;

        /// <summary>
        /// spawn zombie at position
        /// </summary>
        public void SpawnZombieAt(Vector3 position)
        {
            Instantiate(ZombiePrefab, position, Quaternion.identity);
        }

        /// <summary>
        /// spawn worm at position
        /// </summary>
        public void SpawnWormAt(Vector3 position)
        {
            Instantiate(WormPrefab, position, Quaternion.identity);
        }

        /// <summary>
        /// spawn zombie on position of this gameobject
        /// </summary>
        public void SpawnZombie()
        {
            SpawnZombieAt(gameObject.transform.position);
        }

        /// <summary>
        /// spawn worm on position of this gameobject
        /// </summary>
        public void SpawnWorm()
        {
            SpawnWormAt(gameObject.transform.position);
        }

        /// <summary>
        /// spawn 10 zombies and 10 worms
        /// </summary>
        public void SpawnHell()
        {
            for (int i = 0; i < 10; i++)
            {
                SpawnZombie();
                SpawnWorm();
            }
        }

        /// <summary>
        /// spawn 10 zombies and 10 worms
        /// </summary>
        public void SpawnHellOnce()
        {
            if (!spawnHell)
            {
                SpawnHell();
                spawnHell = true;
            }
        }

        void Awake()
        {
            Instance = this;
        }
    }
}
