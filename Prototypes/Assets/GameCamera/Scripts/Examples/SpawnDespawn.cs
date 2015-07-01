using RG_GameCamera;
using RG_GameCamera.Modes;
using UnityEngine;
using System.Collections;

namespace RG_GameCamera.Examples
{
    /// <summary>
    /// example of spawning/despawning player and deactivation of camera
    /// this can be used for example in network games where the player can be despawned and camera deactivated
    /// </summary>
    public class SpawnDespawn : MonoBehaviour
    {
        /// <summary>
        /// prefab of player character
        /// </summary>
        public GameObject CharacterControllerPrefab;

        /// <summary>
        /// reference to current player
        /// </summary>
        public GameObject CharacterControllerCurrent;

        private CameraManager cameraManager;
        private Vector3 lastPos;
        private bool spawned;

        private void Start()
        {
            spawned = CharacterControllerCurrent != null;
            cameraManager = CameraManager.Instance;
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 100, 300, 30), spawned ? "Despawn" : "Spawn"))
            {
                spawned = !spawned;

                if (spawned)
                {
                    Spawn();
                }
                else
                {
                    Despawn();
                }
            }
        }

        /// <summary>
        /// this add a character to the hierarchy and activate the camera
        /// </summary>
        private void Spawn()
        {
            // instantiate new player
            CharacterControllerCurrent =
                Instantiate(CharacterControllerPrefab, lastPos, Quaternion.identity) as GameObject;

            // set a new camera target
            cameraManager.SetCameraTarget(CharacterControllerCurrent.transform);

            // start 3rd camera with a new player
            cameraManager.SetMode(Type.ThirdPerson);
        }

        /// <summary>
        /// this deactivate camera and remove the character from the hierarchy
        /// </summary>
        private void Despawn()
        {
            // remember last position just for easier test
            lastPos = CharacterControllerCurrent.transform.position;

            // destroy current character controller
            Destroy(CharacterControllerCurrent.gameObject);

            // deactivate game camera
            cameraManager.SetMode(Type.None);
        }
    }
}
