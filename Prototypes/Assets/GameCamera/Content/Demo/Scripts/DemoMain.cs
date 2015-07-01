using RG_GameCamera.CharacterController;
using RG_GameCamera.Effects;
using RG_GameCamera.Modes;
using UnityEngine;
using System.Collections;

namespace RG_GameCamera.Demo
{
    public class DemoMain : MonoBehaviour
    {
        public Player Player;

        public Vector2 effectsGUIPos;
        public Vector2 gameModesGUIPos;
        private bool displayEffects;
        private bool showGameModes;

        void Awake()
        {
            Application.targetFrameRate = 60;
            Random.seed = System.DateTime.Now.TimeOfDay.Milliseconds;
        }

        void Start()
        {
            if (!Player)
            {
                if (CameraManager.Instance.CameraTarget)
                {
                    Player = CameraManager.Instance.CameraTarget.gameObject.GetComponent<Player>();
                }
            }

            Utils.Debug.Assert(Player, "Missing player!");
            SetupThirdPerson();
            //SetupRPG();
            //SetupRTS();
            //SetupOrbit();
        }

        void SetupThirdPerson()
        {
            CameraManager.Instance.SetMode(Modes.Type.ThirdPerson);
            CameraManager.Instance.SetCameraTarget(Player.gameObject.transform);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(RG_GameCamera.Input.InputPreset.ThirdPerson);
            Extras.RTSProjector.Instance.Disable();

            // enable player input & deactivate ai controller
            Player.EnableCrosshairAim = true;
            Player.SetInput(Player.InputSource.Direct);
        }

        void SetupFPS()
        {
            CameraManager.Instance.SetMode(Modes.Type.FPS);
            CameraManager.Instance.SetCameraTarget(Player.gameObject.transform);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(RG_GameCamera.Input.InputPreset.FPS);
            Extras.RTSProjector.Instance.Disable();

            // enable player input & deactivate ai controller
            Player.EnableCrosshairAim = true;
            Player.SetInput(Player.InputSource.Direct);
        }

        void SetupRTS()
        {
            CameraManager.Instance.SetMode(Modes.Type.RTS);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(RG_GameCamera.Input.InputPreset.RTS);
            Extras.RTSProjector.Instance.Enable();
            CameraManager.Instance.SetCameraTarget(Player.gameObject.transform);

            TargetManager.Instance.HideCrosshair = true;

            // disable player input & activate ai controller
            Player.SetInput(Player.InputSource.AI);
        }

        void SetupRPG()
        {
            CameraManager.Instance.SetMode(Modes.Type.RPG);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(RG_GameCamera.Input.InputPreset.RPG);
            Extras.RTSProjector.Instance.Enable();
            CameraManager.Instance.SetCameraTarget(Player.gameObject.transform);

            TargetManager.Instance.HideCrosshair = true;

            // disable player input & activate ai controller
            Player.SetInput(Player.InputSource.AI);
        }

        void SetupOrbit()
        {
            CameraManager.Instance.SetMode(Modes.Type.Orbit);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(RG_GameCamera.Input.InputPreset.Orbit);

            // disable all the player specific stuff
            Extras.RTSProjector.Instance.Disable();
            TargetManager.Instance.HideCrosshair = true;
            Player.SetInput(Player.InputSource.AI);
        }

        void SetupLookAt()
        {
            var lookAt = CameraManager.Instance.SetMode(Modes.Type.LookAt) as LookAtCameraMode;
            var playerPos = Player.transform.position;

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

            if (GUI.Button(new Rect(offsetX, offsetY + 290, 100, 30), "Sprint"))
            {
                var shake = em.Create<SprintShake>();
                shake.Play();
            }
        }

        void OnGUI()
        {
            displayEffects = GUI.Toggle(new Rect(effectsGUIPos.x, effectsGUIPos.y, 150, 30), displayEffects, "Camera effects");

            if (displayEffects)
            {
                DisplayEffects();
            }

            showGameModes = GUI.Toggle(new Rect(gameModesGUIPos.x, gameModesGUIPos.y, 150, 30), showGameModes, "Game modes");

            if (showGameModes)
            {
                ShowGameModes();
            }
        }
    }
}
