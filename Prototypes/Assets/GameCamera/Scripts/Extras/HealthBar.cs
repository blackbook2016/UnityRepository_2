// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Extras
{
    public class HealthBar : MonoBehaviour
    {
        public static HealthBar Instance;

        public float PosX;
        public float PosY;

        public Texture2D HealthBarFull;
        public Texture2D HealthBarEmpty;

        private float health;

        void Awake()
        {
            Instance = this;
        }

        public void SetHealth(float newHealth)
        {
            health = newHealth;

            if (health < 0.0f)
            {
                health = 0.1f;
            }
        }

        void OnGUI()
        {
            var x = PosX*Screen.width;
            var y = PosY*Screen.height;

            const int height = 200;
            var healthHeight = (health/100.0f)*height;
            var offset = (height - healthHeight);

            var offsetHack = 0.0f;
            
            if (health > 0.1f)
            {
                offsetHack = Mathf.Clamp01(1.0f - health / 100.0f) * 5;
            }

            GUI.DrawTexture(new Rect(x, y, 50, 200), HealthBarEmpty, ScaleMode.StretchToFill);
            GUI.DrawTexture(new Rect(x, y + offset, 50, 200-offset-offsetHack), HealthBarFull, ScaleMode.StretchToFill);
        }
    }
}
