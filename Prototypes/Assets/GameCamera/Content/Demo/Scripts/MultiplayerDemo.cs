using RG_GameCamera.CharacterController;
using RG_GameCamera.Effects;
using RG_GameCamera.Input;
using RG_GameCamera.Modes;
using RG_GameCamera.Utils;
using UnityEngine;
using System.Collections.Generic;

namespace RG_GameCamera.Demo
{
    public class MultiplayerDemo : MonoBehaviour
     {
        public Transform playersParent;
        public Vector2 effectsGUIPos;
        public Vector2 gameModesGUIPos;
        public GUISkin skin;

        private List<CharacterController.Player> players;
        private Player currentPlayer;
        private Modes.Type currentCameraType;
        private bool switchPlayers;
        private bool showGameModes;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Random.seed = System.DateTime.Now.TimeOfDay.Milliseconds;

            players = new List<Player>(4);

            for (var i = 0; i < playersParent.childCount; i++)
            {
                var child = playersParent.GetChild(i);

                if (child)
                {
                    var character = child.GetComponent<CharacterController.Player>();

                    if (character)
                    {
                        character.Remote = true;
                        players.Add(character);
                    }
                }
            }

            if (players.Count > 0)
            {
                currentPlayer = players[0];
                currentPlayer.Remote = false;
            }
        }

        void Start()
        {
            if (currentPlayer)
            {
                RG_GameCamera.CameraManager.Instance.SetCameraTarget(currentPlayer.transform);
                currentCameraType = RG_GameCamera.CameraManager.Instance.GetCameraMode().Type;
            }
        }

        void SetupThirdPerson()
        {
            CameraManager.Instance.SetMode(Modes.Type.ThirdPerson);
            CameraManager.Instance.SetCameraTarget(currentPlayer.transform);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.ThirdPerson);
            CursorLocking.Lock();
            EffectManager.Instance.StopAll();
        }

        void SetupFPS()
        {
            CameraManager.Instance.SetMode(Modes.Type.FPS);
            CameraManager.Instance.SetCameraTarget(currentPlayer.transform);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.FPS);
            CursorLocking.Lock();
            EffectManager.Instance.StopAll();
        }

        void SetupRTS()
        {
            CameraManager.Instance.SetMode(Modes.Type.RTS);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.RTS);
            CameraManager.Instance.SetCameraTarget(currentPlayer.transform);
            CursorLocking.Unlock();
            EffectManager.Instance.StopAll();
        }

        void SetupRPG()
        {
            CameraManager.Instance.SetMode(Modes.Type.RPG);
            RG_GameCamera.Input.InputManager.Instance.SetInputPreset(InputPreset.RPG);
            CameraManager.Instance.SetCameraTarget(currentPlayer.transform);
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

        void SetupLookAt()
        {
            EffectManager.Instance.StopAll();
            var lookAt = CameraManager.Instance.SetMode(Modes.Type.LookAt) as LookAtCameraMode;
            var playerPos = currentPlayer.transform.position;

            var randVec = Random.insideUnitSphere;
            randVec.y = 0.6f;
            lookAt.LookFrom(playerPos + randVec * Random.Range(1.0f, 20.0f), 2.0f);
        }

        private void ShowGameModes()
        {
            var offsetY = gameModesGUIPos.y + 30;
            var offsetX = gameModesGUIPos.x;

            if (GUI.Button(new Rect(offsetX, offsetY + 10, 120, 30), "ThirdPerson"))
            {
                SetupThirdPerson();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 50, 120, 30), "RTS"))
            {
                SetupRTS();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 90, 120, 30), "RPG"))
            {
                SetupRPG();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 130, 120, 30), "Orbit"))
            {
                SetupOrbit();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 170, 120, 30), "LookAt"))
            {
                SetupLookAt();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 210, 120, 30), "Dead"))
            {
                SetupDead();
            }

            if (GUI.Button(new Rect(offsetX, offsetY + 250, 120, 30), "FPS"))
            {
                SetupFPS();
            }

            currentCameraType = RG_GameCamera.CameraManager.Instance.GetCameraMode().Type;
        }

        void SwitchPlayers()
        {
            var offsetY = effectsGUIPos.y + 30;
            var offsetX = effectsGUIPos.x;

            if (GUI.Button(new Rect(offsetX, offsetY + 10, 120, 30), "Select None"))
            {
                currentPlayer.Remote = true;
                var lookAt = RG_GameCamera.CameraManager.Instance.SetMode(RG_GameCamera.Modes.Type.LookAt) as LookAtCameraMode;
                lookAt.LookAt(currentPlayer.transform.position + new Vector3(10.0f, 10.0f, 10.0f), currentPlayer.transform.position, 1.0f);
            }

            var y = 50;
            var i = 0;

            foreach (var player in players)
            {
                i++;

                if (GUI.Button(new Rect(offsetX, offsetY + y, 120, 30), "Select Player " + i.ToString()))
                {
                    currentPlayer.Remote = true;
                    currentPlayer = player;
                    currentPlayer.Remote = false;
                    RG_GameCamera.CameraManager.Instance.SetCameraTarget(currentPlayer.transform);
                    RG_GameCamera.CameraManager.Instance.SetMode(currentCameraType);
                }

                y += 40;
            }
        }

        void OnGUI()
        {
            GUI.skin = skin;

            switchPlayers = GUI.Toggle(new Rect(effectsGUIPos.x, effectsGUIPos.y, 150, 30), switchPlayers, "Switch players");

            if (switchPlayers)
            {
                SwitchPlayers();
            }

            showGameModes = GUI.Toggle(new Rect(gameModesGUIPos.x, gameModesGUIPos.y, 150, 30), showGameModes, "Camera modes");

            if (showGameModes)
            {
                ShowGameModes();
            }
        }
    }
}
