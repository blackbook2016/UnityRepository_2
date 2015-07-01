// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RG_GameCamera.Editor.Installation
{
    /// <summary>
    /// game camera installation dialog
    /// </summary>
    [InitializeOnLoad]
    public class GameCameraInstaller : EditorWindow
    {
        static string GetVersion()
        {
            return "1.1.12";
        }

        static GameCameraInstaller()
        {
            EditorApplication.update -= OpenNextUpdate;
            EditorApplication.update += OpenNextUpdate;
        }

        static void OpenNextUpdate()
        {
            if (CanShow())
            {
                Install();
            }

            EditorApplication.update -= OpenNextUpdate;
        }

        static bool CanShow()
        {
            if (string.IsNullOrEmpty(Application.dataPath))
            {
                return true;
            }

            return EditorPrefs.GetBool("GameCameraInstall"+GetVersion(), true);
        }

        static void SetHidden(bool status)
        {
            EditorPrefs.SetBool("GameCameraInstall"+GetVersion(), status);
        }

        static void SetUnity5Upgraded()
        {
            EditorPrefs.SetBool("GameCameraInstallUnity5Upgrade", true);
        }

        static bool IsUnity5Upgraded()
        {
            return EditorPrefs.GetBool("GameCameraInstallUnity5Upgrade", false);
        }

        private bool yes = true;
        private bool showOnLoad = true;
        static GameCameraInstaller window;

        [MenuItem("Window/GameCamera/Installer")]
        public static void Install()
        {
            window = EditorWindow.GetWindow<GameCameraInstaller>(true);
            window.minSize = new Vector2(600, 400);
            window.title = "Game Camera Installer";
            window.position = new Rect(200, 200, window.minSize.x, window.minSize.y);
            window.ShowUtility();
            window.showOnLoad = !CanShow();
        }

        [MenuItem("Window/GameCamera/Documentation")]
        public static void Documentation()
        {
            Application.OpenURL("https://docs.google.com/document/d/1TcXc4Onkk13gWRlKgrFCWSY4-UjW7XUJIF4Cfsyaqk8/pub?embedded=true");
        }

//#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
//        [MenuItem("Window/GameCamera/Unity5 Upgrade")]
//        public static void UpgradeUnity5()
//        {
//            if (EditorUtility.DisplayDialog("Unity5 Upgrade",
//                "UNITY 5 UPGRADE: This will replace all demo scenes, Navigational mesh and Mecanim data with Unity5 updated version.", "Ok", "Cancel"))
//            {
//                CopyUnity5Data();
//            }
//        }
//#endif

        static void CopyUnity5Data()
        {
            string[] src =
            {
                Application.dataPath + "/GameCamera/Content/Unity5.Update/BasicScene.unity",
                Application.dataPath + "/GameCamera/Content/Unity5.Update/EventsScene.unity",
                Application.dataPath + "/GameCamera/Content/Unity5.Update/ZombieScene.unity",
				Application.dataPath + "/GameCamera/Content/Unity5.Update/MultiplayerScene.unity",
                Application.dataPath + "/GameCamera/Content/Unity5.Update/BasicScene/NavMesh.asset",
                Application.dataPath + "/GameCamera/Content/Unity5.Update/EventsScene/NavMesh.asset",
                Application.dataPath + "/GameCamera/Content/Unity5.Update/ZombieScene/NavMesh.asset",
				Application.dataPath + "/GameCamera/Content/Unity5.Update/MultiplayerScene/NavMesh.asset",
                Application.dataPath + "/GameCamera/Content/Unity5.Update/Animator/Third Person Animator Controller.controller",
            };

            string[] dst =
            {
                Application.dataPath + "/GameCamera/Content/BasicScene.unity",
                Application.dataPath + "/GameCamera/Content/EventsScene.unity",
                Application.dataPath + "/GameCamera/Content/ZombieScene.unity",
				Application.dataPath + "/GameCamera/Content/MultiplayerScene.unity",
                Application.dataPath + "/GameCamera/Content/BasicScene/NavMesh.asset",
                Application.dataPath + "/GameCamera/Content/EventsScene/NavMesh.asset",
                Application.dataPath + "/GameCamera/Content/ZombieScene/NavMesh.asset",
				Application.dataPath + "/GameCamera/Content/MultiplayerScene/NavMesh.asset",
                Application.dataPath + "/GameCamera/Content/Characters/Third Person Character/Animator/Third Person Animator Controller.controller",
            };

            for (var i = 0; i < src.Length; i++)
            {
                if (System.IO.File.Exists(src[i]) && System.IO.File.Exists(dst[i]))
                {
                    System.IO.File.Copy(src[i], dst[i], true);
                    UnityEngine.Debug.Log("GameCamera Unity5 Upgrade: " + dst[i]);
                }
                else
                {
                    UnityEngine.Debug.LogWarning("GameCamera Unity5 Upgrade File Not Found: " + src[i]);
                }
            }

            // refresh asset
            UnityEditor.AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        void OnGUI()
        {
            GUILayout.Space(20);

            var style = GUIStyle.none;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 16;
            style.fontStyle = FontStyle.Bold;

            GUILayout.Box("GameCamera Installation", style, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(20) });
            GUILayout.Space(40);

            style.fontSize = 12;
            style.fontStyle = FontStyle.Normal;
            style.alignment = TextAnchor.MiddleLeft;

            GUILayout.Label("     GameCamera package includes predefined input controls in InputManager.asset,\n" +
                            "     it is required for proper working of GameCamera. However it will overwrite current\n" +
                            "     input settings.", style);

            GUILayout.Space(20);
            GUILayout.Label("     Would you like to use GameCamera InputManager.asset?", style);
            GUILayout.Space(30);

            style = new GUIStyle("toggle");
            style.fontSize = 12;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleLeft;

            GUILayout.BeginHorizontal();
            GUILayout.Space(80);
            yes = GUILayout.Toggle(yes, "Yes, this will overwrite current input settings.", style);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.Space(80);
            yes = !GUILayout.Toggle(!yes, "No, I will configure game camera controls manually.", style);
            GUILayout.EndHorizontal();

            if (!yes)
            {
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.Space(50);
                EditorGUILayout.HelpBox(
                    "Please read the Input chapter in GameCamera documentation how to manually\nconfigure InputManager.",
                    MessageType.Info);
                GUILayout.Space(10);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);
            if (GUILayout.Button("Continue", GUILayout.Height(30)))
            {
                if (yes)
                {
                    CopyInputManager(true);

#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6)
//                    UpgradeUnity5();
//                    if (!IsUnity5Upgraded())
//                    {
//                        CopyUnity5Data();
//                        SetUnity5Upgraded();
//                    }
#endif
                }
                this.Close();
            }
            GUILayout.Space(50);
            GUILayout.EndHorizontal();

            if (yes)
            {
                GUILayout.Space(40);
            }

            bool checkbox = GUILayout.Toggle(showOnLoad, "Don't show this again.");

            if (checkbox != showOnLoad)
            {
                showOnLoad = checkbox;
                SetHidden(!showOnLoad);

                if (showOnLoad)
                {
                    EditorUtility.DisplayDialog("Confirmation",
                        "You can always open this window from Editor menu: Window->GameCamera->Installer.\n", "Ok");
                }
            }
        }

        private static void CopyInputManager(bool backup)
        {
            var src = Application.dataPath + "/GameCamera/Content/InputManager/InputManager.install";
            var dst = Application.dataPath + "/../ProjectSettings/InputManager.asset";
            var bck = Application.dataPath + "/../ProjectSettings/InputManager.backup";

            if (System.IO.File.Exists(src) && System.IO.File.Exists(dst))
            {
                if (backup)
                {
                    System.IO.File.Copy(dst, bck, true);
                }

                System.IO.File.Copy(src, dst, true);

                UnityEngine.Debug.Log("GameCamera has successfully installed InputManager.asset!");

                // refresh asset
                UnityEditor.AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            else
            {
                UnityEngine.Debug.LogError("GameCamera import error: " + src + " or " + dst + " not found!");
            }
        }
    }
}
