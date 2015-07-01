// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Collections.Generic;
using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace RG_GameCamera.CharacterController
{
    public enum TargetType
    {
        Enemy,
        Default,
        UseObject,
        None,
    }

    /// <summary>
    /// target manager is used for determining game object in aim direction from camera
    /// in the demo it is used for detection of enemy entities
    /// </summary>
    public class TargetManager : MonoBehaviour
    {
        public static TargetManager Instance
        {
            get { return instance; }
        }

        private static TargetManager instance;

        public GameObject TargetObject;
        public TargetType TargetType;
        public Vector3 TargetPosition;
        public bool HideCrosshair;
        private Vector3 hitPos;

        public GUITexture CrosshairGun = null;
        public GUITexture CrosshairHand = null;
        public Camera Camera = null;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            Utils.Debug.SetActive(CrosshairGun.gameObject, true);
            Utils.Debug.SetActive(CrosshairHand.gameObject, true);

            if (!Camera)
            {
                Camera = CameraManager.Instance.UnityCamera;
            }
        }

        private void Update()
        {
            // run raycast against objects in the scene
            var mouseRay = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            TargetObject = null;
            TargetType = TargetType.None;
            TargetPosition = Vector3.zero;

            var hits =
                new List<RaycastHit>(Physics.RaycastAll(mouseRay, Mathf.Infinity));
            GameObject hitObject = null;

            if (hits.Count > 0)
            {
                hits.Sort(delegate(RaycastHit a, RaycastHit b)
                {
                    return (Camera.transform.position - a.point).sqrMagnitude.CompareTo(
                            (Camera.transform.position - b.point).sqrMagnitude);
                });

                hitObject = hits[0].collider.gameObject;
                TargetPosition = hits[0].point;
                hitPos = hits[0].point;
            }

            if (hitObject != null)
            {
                TargetObject = hitObject;
                TargetType = TargetType.Default;

                var hitEntity = TargetObject.GetComponent<HitEntity>();
                if (hitEntity && !hitEntity.IsDead && hitEntity.Enemy)
                {
                    TargetType = TargetType.Enemy;
                }
            }

            switch (TargetType)
            {
                case TargetType.Enemy:
                    CrosshairHand.enabled = false;
                    CrosshairGun.enabled = true;
                    var color = Color.red;
                    CrosshairGun.color = color;
                    break;

                case TargetType.UseObject:
                    CrosshairHand.enabled = true;
                    CrosshairGun.enabled = false;
                    break;

                case TargetType.Default:
                case TargetType.None:
                    CrosshairHand.enabled = false;
                    CrosshairGun.enabled = true;
                    var clr = Color.white;
                    clr.a = CrosshairGun.color.a;
                    CrosshairGun.color = clr;
                    break;
            }

            if (HideCrosshair)
            {
                CrosshairGun.enabled = false;
                CrosshairHand.enabled = false;
            }

            // activate use object
            if (UnityInput.GetKeyDown(KeyCode.E))
            {
                if (TargetType == TargetType.UseObject)
                {
//                    var useObject = TargetObject.GetComponent<UseObject>();
//
//                    if (useObject)
//                    {
//                        useObject.Use();
//                    }
                }
            }
        }

        public void Shoot(HitEntity owner, float damage)
        {
            if (TargetType == TargetType.Enemy)
            {
                var hitEntity = TargetObject.GetComponent<HitEntity>();

                if (hitEntity)
                {
                    hitEntity.OnHit(owner, damage, hitPos);
                }
            }
        }

        public void ShootAt(Vector3 aimVector, HitEntity owner, float damage)
        {
            // run raycast against objects in the scene
            var ray = new Ray(owner.transform.position + Vector3.up, aimVector);

            var hits = new List<RaycastHit>(Physics.RaycastAll(ray, Mathf.Infinity));
            GameObject hitObject = null;

            if (hits.Count > 0)
            {
                hits.Sort(delegate(RaycastHit a, RaycastHit b)
                {
                    return (Camera.transform.position - a.point).sqrMagnitude.CompareTo(
                            (Camera.transform.position - b.point).sqrMagnitude);
                });

                hitObject = hits[0].collider.gameObject;
                hitPos = hits[0].point;
            }

            if (hitObject != null)
            {
                var hitEntity = hitObject.GetComponent<HitEntity>();
                if (hitEntity && !hitEntity.IsDead && hitEntity.Enemy)
                {
                    hitEntity.OnHit(owner, damage, hitPos);
                }
            }
        }
    }
}
