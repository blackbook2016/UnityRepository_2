using RG_GameCamera.Modes;
using UnityEngine;

namespace RG_GameCamera.Demo
{
    public class HelpScreen : MonoBehaviour
    {
        public Vector2 guiPos;
        public Vector2 showLiveGUIPos;
        public GUISkin skin;
        private bool show = true;
        private bool showLiveGUI;

        private void Show(bool dieInfo)
        {
            var offsetY = guiPos.y + 30;
            var offsetX = guiPos.x;

            var mode = CameraManager.Instance.GetCameraMode().Type;

            var style = new GUIStyle("box");
            style.fontSize = 12;
            style.alignment = TextAnchor.MiddleLeft;

            var help = string.Empty;

            if (dieInfo)
            {
                help += "Press 'R' to resurect player.";
            }
            else
            {
                help += "Press Tab to lock mouse cursor\nPress Escape to unlock it\n";
                help += "Press 'H' to show/hide this window\n\n";

                switch (mode)
                {
                    case Type.ThirdPerson:
                        help += "WASD - move around the scene\n";
                        help += "Right mouse button - Aim\n";
                        help += "Left mouse button - Shoot\n";
                        help += "Space - jump\n";
                        help += "C - crouch\n";
                        help += "LShift - Sprint\n";
                        help += "CapsLock - Walk\n";
                        help += "----------------------------\n";
                        help += "You can use gamepad as well!\n";
                        break;
                    case Type.RPG:
                        help += "Use Right mouse button to set waypoint position.\n";
                        help += "Use Right mouse button to attack enemies.\n";
                        help += "Use Mouse scrollwheel to zoom the camera.\n";
                        help += "Use '[' and ']' to rotate the camera.\n";
                        break;
                    case Type.RTS:
                        help += "Use Right mouse button to set waypoint position.\n";
                        help += "Use Right mouse button to attack enemies.\n";
                        help += "To move the camera move your mouse to screen.\nborder, use WSAD or drag the scene.\n";
                        help += "Use Mouse scrollwheel to zoom the camera.\n";
                        help += "Use '[' and ']' to rotate the camera.\n";
                        break;
                    case Type.Orbit:
                        help += "Use Right mouse button to rotate the camera.\n";
                        help += "Use Left right mouse button to pan the camera.\n";
                        help += "Use Mouse scrollwheel to zoom the camera.\n";
                        help += "Use Middle mouse double-click button to reset camera target\n";
                        break;
                    case Type.LookAt:
                        help += "Randomly choose camera position and target.\n";
                        help += "You can click on LookAt button again to repeat\nthe process.\n";
                        break;
                    case Type.Dead:
                        help += "Camera without controls, just rotating around\ncharacter.\n";
                        break;
                }
            }

            GUI.Box(new Rect(offsetX, offsetY, 300, dieInfo ? 50 : 200), help, style);
        }

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.H))
            {
                show = !show;
            }
        }

        void OnGUI()
        {
            GUI.skin = skin;

            show = GUI.Toggle(new Rect(guiPos.x, guiPos.y, 100, 30), show, "Help");
            showLiveGUI = GUI.Toggle(new Rect(showLiveGUIPos.x, showLiveGUIPos.y, 150, 30), showLiveGUI, "Live configuration");

            if (show)
            {
                Show(false);
            }

            CameraManager.Instance.GetCameraMode().EnableLiveGUI = showLiveGUI;

            var player = CameraManager.Instance.CameraTarget;

            var isDead = player && player.GetComponent<CharacterController.Player>() &&
                         player.GetComponent<CharacterController.Player>().IsDead;

            if (isDead)
            {
                Show(true);
            }
        }
    }
}
