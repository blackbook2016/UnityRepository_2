using UnityEngine;
using System.Collections;

namespace RG_GameCamera.Demo
{
    /// <summary>
    /// simple zombie/worm spawner
    /// </summary>
    public class ZombieHUD : MonoBehaviour
    {
        public static ZombieHUD Instance { get; private set; }

        public Texture HUD;
        public Vector2 Position;
        public Vector2 Size;

        public Vector2 Pos2;
        public float Size2;

        public GUIText Text;

        private int zombies;
        private int worms;

        public void ZombieKilled()
        {
            zombies++;
        }

        public void WormKilled()
        {
            worms++;
        }

        void Awake()
        {
            Instance = this;
            if (!Text)
            {
                Text = gameObject.GetComponent<GUIText>();
            }
        }

        void OnGUI()
        {
            var x = Screen.width*Position.x;
            var y = Screen.height*Position.y;

            var width = Screen.width*Size.x;
            var height = Screen.height*Size.y;

            GUI.DrawTexture(new Rect(x, y, width, height), HUD, ScaleMode.ScaleToFit);

            x = Screen.width*Pos2.x;
            y = Screen.height*Pos2.y;
            var s = Screen.height*Size2;

            Text.pixelOffset = new Vector2(x, y);
            Text.fontSize = (int) s;
            Text.text = worms.ToString() + "\n\n" + zombies.ToString();
        }
    }
}
