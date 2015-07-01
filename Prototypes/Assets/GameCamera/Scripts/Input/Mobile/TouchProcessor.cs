// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;

namespace RG_GameCamera.Input.Mobile
{
    public class TouchProcessor
    {
        private readonly SimTouch[] touches;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="numberOfTouches">maximum number of touches to recognize</param>
        public TouchProcessor(int numberOfTouches)
        {
            touches = new SimTouch[numberOfTouches];

            for (int i = 0; i < touches.Length; i++)
            {
                touches[i] = new SimTouch(i, KeyCode.LeftAlt);
            }
        }

        /// <summary>
        /// return all touches
        /// </summary>
        public SimTouch[] GetTouches()
        {
            return touches;
        }

        /// <summary>
        /// get number of all touches (include not active)
        /// </summary>
        public int GetTouchCount()
        {
            return touches.Length;
        }

        /// <summary>
        /// get number of all active touches
        /// </summary>
        public int GetActiveTouchCount()
        {
            var activeTouches = 0;

            foreach (var touch in touches)
            {
                if (touch.Status != TouchStatus.Invalid)
                {
                    activeTouches++;
                }
            }

            return activeTouches;
        }

        /// <summary>
        /// get specified touch
        /// </summary>
        /// <param name="index">index of the touch</param>
        public SimTouch GetTouch(int index)
        {
            return touches[index];
        }

        /// <summary>
        /// scan for input touches
        /// </summary>
        public void ScanInput()
        {
            for (int i = 0; i < touches.Length; i++)
            {
                touches[i].ScanInput();
            }
        }

        /// <summary>
        /// show touch icon and display information
        /// </summary>
        /// <param name="status">show or hide</param>
        public void ShowDebug(bool status)
        {
        }
    }
}
