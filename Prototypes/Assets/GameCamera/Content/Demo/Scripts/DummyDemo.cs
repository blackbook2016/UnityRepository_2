using RG_GameCamera.CharacterController;
using RG_GameCamera.Effects;
using RG_GameCamera.Input;
using RG_GameCamera.Modes;
using RG_GameCamera.Utils;
using UnityEngine;
using System.Collections;

namespace RG_GameCamera.Demo
{
    public class DummyDemo : MonoBehaviour
    {
        public Transform player;
        public Vector2 effectsGUIPos;
        public Vector2 gameModesGUIPos;
        public GUISkin skin;

        public bool showEffects = true;

        private bool displayEffects;
        private bool showGameModes;

        void Awake()
        {
            Application.targetFrameRate = 60;
            Random.seed = System.DateTime.Now.TimeOfDay.Milliseconds;
        }

		void Update()
		{
//			if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
//			{
//				CameraManager.Instance.GetCameraMode().Reset();
//			}
		}

        void Start()
        {
            //SetupThirdPerson();
            //SetupRPG();
            //SetupRTS();
            //SetupOrbit();
        }

        void SetupThirdPerson()
        {
            CameraManager.Instance.SetMode(Modes.Type.ThirdPerson);
            CameraManager.Instance.SetCameraTarget(player.transform);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.ThirdPerson);
            CursorLocking.Lock();
            EffectManager.Instance.StopAll();
        }

        void SetupFPS()
        {
            CameraManager.Instance.SetMode(Modes.Type.FPS);
            CameraManager.Instance.SetCameraTarget(player.transform);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.FPS);
            CursorLocking.Lock();
            EffectManager.Instance.StopAll();
        }

        void SetupRTS()
        {
            CameraManager.Instance.SetMode(Modes.Type.RTS);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.RTS);
            CameraManager.Instance.SetCameraTarget(player.transform);
            CursorLocking.Unlock();
            EffectManager.Instance.StopAll();
        }

        void SetupRPG()
        {
            CameraManager.Instance.SetMode(Modes.Type.RPG);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.RPG);
            CameraManager.Instance.SetCameraTarget(player.transform);
            CursorLocking.Unlock();
            EffectManager.Instance.StopAll();
        }

        void SetupOrbit()
        {
            CameraManager.Instance.SetMode(Modes.Type.Orbit);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.Orbit);
            CursorLocking.Unlock();
            EffectManager.Instance.StopAll();
        }

        void SetupDead()
        {
            CameraManager.Instance.SetMode(Modes.Type.Dead);
            EffectManager.Instance.StopAll();
        }

        void SetupDebug()
        {
            CameraManager.Instance.SetMode(Modes.Type.Debug);
        }

        void SetupLookAt()
        {
            EffectManager.Instance.StopAll();
            var lookAt = CameraManager.Instance.SetMode(Modes.Type.LookAt) as LookAtCameraMode;
            var playerPos = player.position;

            var randVec = Random.insideUnitSphere;
            randVec.y = 0.6f;
            lookAt.LookFrom(playerPos + randVec*Random.Range(1.0f, 20.0f), 2.0f);
        }

        private void ShowGameModes()
        {
            var offsetY = gameModesGUIPos.y + 30;
            var offsetX = gameModesGUIPos.x;

            if (GUI.Button(new Rect(offsetX, offsetY + 10, 100, 30), "ThirdPerson"))
            {
                SetupThirdPerson();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 50, 100, 30), "RTS"))
            {
                SetupRTS();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 90, 100, 30), "RPG"))
            {
                SetupRPG();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 130, 100, 30), "Orbit"))
            {
                SetupOrbit();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 170, 100, 30), "LookAt"))
            {
                SetupLookAt();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 210, 100, 30), "Dead"))
            {
                SetupDead();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 250, 100, 30), "FPS"))
            {
                SetupFPS();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 290, 100, 30), "Debug"))
            {
                SetupDebug();
            }
        }

        void DisplayEffects()
        {
            var em = EffectManager.Instance;

            var offsetY = effectsGUIPos.y + 30;
            var offsetX = effectsGUIPos.x;

            if (GUI.Button(new Rect(offsetX, offsetY + 10, 100, 30), "Earthquake"))
            {
                var eartquake = em.Create<Earthquake>();
                eartquake.Play();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 50, 100, 30), "Yes"))
            {
                var yes = em.Create<Yes>();
                yes.Play();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 90, 100, 30), "No"))
            {
                var no = em.Create<No>();
                no.Play();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 130, 100, 30), "FireKick"))
            {
                var fireKick = em.Create<FireKick>();
                fireKick.Play();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 170, 100, 30), "Stomp"))
            {
                var stomp = em.Create<Stomp>();
                stomp.Play();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 210, 100, 30), "Fall"))
            {
                var fall = em.Create<Fall>();
                fall.ImpactVelocity = 2.0f;
                fall.Play();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 250, 100, 30), "Explosion"))
            {
                var explosion = em.Create<Explosion>();
                explosion.position = CameraManager.Instance.UnityCamera.transform.position + Random.insideUnitSphere * 2;
                explosion.position.y = 0.0f;
                explosion.Play();
            }
        }

        void OnGUI()
        {
            GUI.skin = skin;

            if (showEffects)
            {
                displayEffects = GUI.Toggle(new Rect(effectsGUIPos.x, effectsGUIPos.y, 150, 30), displayEffects, "Camera effects");

                if (displayEffects)
                {
                    DisplayEffects();
                }
            }

            showGameModes = GUI.Toggle(new Rect(gameModesGUIPos.x, gameModesGUIPos.y, 150, 30), showGameModes, "Camera modes");

            if (showGameModes)
            {
                ShowGameModes();
            }
        }
    }
}
