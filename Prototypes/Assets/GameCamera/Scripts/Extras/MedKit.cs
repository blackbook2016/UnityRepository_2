// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using RG_GameCamera.CharacterController;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace RG_GameCamera.Extras
{
    [RequireComponent(typeof(BoxCollider))]
    public class MedKit : MonoBehaviour
    {
        public float HP = 20;

        /// <summary>
        /// Unity callback when something enters MedKit collider
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other && other.gameObject)
            {
                var human = other.gameObject.GetComponent<Human>();

                if (human)
                {
                    if (human.OnHealthPickUp(HP))
                    {
                        SoundManager.Instance.PlayHealthPickUp(human.GetComponent<AudioSource>());
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
