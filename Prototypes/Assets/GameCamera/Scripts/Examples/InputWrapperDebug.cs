// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

//#define WRAPPER_DEBUG
#if WRAPPER_DEBUG

using UnityEngine;

namespace RG_GameCamera.Input
{
    public class InputWrapper
    {
        /// <summary>
        /// flag whether we are using mobile input
        /// </summary>
        public static bool Mobile;
        
        /// <summary>
        /// get status of the button
        /// </summary>
        public static bool GetButton(string key)
        {
            if (Mobile)
            {
                var btnState = RG_GameCamera.Input.Mobile.MobileControls.Instance.GetButton(key);
                return btnState || UnityEngine.Input.GetButton(key);
            }

            return UnityEngine.Input.GetButton(key);
        }

        /// <summary>
        /// get status of the zoom area (this is valid only for mobile controls)
        /// </summary>
        public static float GetZoom(string key)
        {
            if (Mobile)
            {
                return RG_GameCamera.Input.Mobile.MobileControls.Instance.GetZoom(key);
            }

            return 0.0f;
        }

        /// <summary>
        /// get status of the axis
        /// </summary>
        public static float GetAxis(string key)
        {
            if (Mobile)
            {
                var axis = Mathf.Abs(RG_GameCamera.Input.Mobile.MobileControls.Instance.GetAxis(key)) < Mathf.Epsilon;
                return !axis ? RG_GameCamera.Input.Mobile.MobileControls.Instance.GetAxis(key) : UnityEngine.Input.GetAxis(key);
            }

            return UnityEngine.Input.GetAxis(key);
        }

        /// <summary>
        /// get status of the down button event
        /// </summary>
        public static bool GetButtonDown(string buttonName)
        {
            if (Mobile)
            {
                var btnState = RG_GameCamera.Input.Mobile.MobileControls.Instance.GetButtonDown(buttonName);
                return btnState || UnityEngine.Input.GetButtonDown(buttonName);
            }

            return UnityEngine.Input.GetButtonDown(buttonName);
        }

        /// <summary>
        /// get status of up button event
        /// </summary>
        public static bool GetButtonUp(string buttonName)
        {
            if (Mobile)
            {
                var btnState = RG_GameCamera.Input.Mobile.MobileControls.Instance.GetButtonUp(buttonName);
                return btnState || UnityEngine.Input.GetButton(buttonName);
            }

            return UnityEngine.Input.GetButton(buttonName);
        }
    }
}

#endif