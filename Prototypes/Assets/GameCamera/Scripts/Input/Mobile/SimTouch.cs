// Version 1.1.12
// ©2013 Reindeer Games
// All rights reserved
// Redistribution of source code without permission not allowed

using UnityEngine;
using UnityInput = UnityEngine.Input;

namespace RG_GameCamera.Input.Mobile
{
    /// <summary>
    /// status of the touch
    /// </summary>
    public enum TouchStatus
    {
        Invalid = 0,
        Start = 1,
        Stationary = 2,
        Moving = 3,
        End = 4,
    }

    /// <summary>
    /// this class represents single touch input (android devices use UnityInput.Touch, other platforms are simulated by mouse)
    /// </summary>
    public class SimTouch
    {
        /// <summary>
        /// id of the touch
        /// </summary>
        public int FingerId;

        /// <summary>
        /// position of the touch in screen space
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// position of first touch
        /// </summary>
        public Vector2 StartPosition;

        /// <summary>
        /// status of the touch
        /// </summary>
        public TouchStatus Status = TouchStatus.Invalid;

        /// <summary>
        /// number of tap count
        /// </summary>
        public int TapCount;

        /// <summary>
        /// delta time from last touch input
        /// </summary>
        public float DeltaTime;

        /// <summary>
        /// delta position from last touch input
        /// </summary>
        public Vector2 DeltaPosition;

        /// <summary>
        /// maximal time between taps
        /// </summary>
        public float TapTimeWindow = 0.3f;

        /// <summary>
        /// time of overall touch press
        /// </summary>
        public float PressTime;

        /// <summary>
        /// dead zone distance for moving
        /// </summary>
        public float MoveThreshold = 0.5f;

        private Vector2 lastPosition;
        private static KeyCode SimulationKey = KeyCode.None;
        private float tapTimeout;

        enum MouseStatus
        {
            StartDown = 0,
            Down = 1,
            Up = 2,
            None = 3,
        }

        private MouseStatus lastMouseStatus;

        /// <summary>
        /// constructor
        /// </summary>
        public SimTouch(int fingerID, KeyCode simKey)
        {
            FingerId = fingerID;
            SimulationKey = simKey;
            lastMouseStatus = MouseStatus.None;
        }

        /// <summary>
        /// scan for touch input, updates every frame
        /// </summary>
        public void ScanInput()
        {
#if UNITY_IPHONE || UNITY_ANDROID
//            if (Application.isEditor)
//            {
//                UpdateTouchSim();
//            }
//            else
            {
                UpdateTouchInput();
            }
#else
            UpdateTouchSim();
#endif
        }

        private MouseStatus GetMouseStatus()
        {
            bool updateMouse0 = true;

            if (UnityInput.GetKey(SimulationKey))
            {
                if (FingerId > 0)
                {
                    if (lastMouseStatus == MouseStatus.None || lastMouseStatus == MouseStatus.StartDown)
                    {
                        updateMouse0 = false;

                        if (UnityInput.GetMouseButtonDown(0))
                        {
                            return MouseStatus.StartDown;
                        }
                        if (UnityInput.GetMouseButton(0))
                        {
                            return MouseStatus.Down;
                        }
                    }
                    else
                    {
                        DeltaPosition = Vector3.zero;
                        Position = lastPosition;
                        return lastMouseStatus;
                    }
                }
            }
            else
            {
                if (FingerId > 0)
                {
                    return MouseStatus.None;
                }
            }

            if (updateMouse0)
            {
                if (UnityInput.GetMouseButtonDown(0))
                {
                    return MouseStatus.StartDown;
                }
                if (UnityInput.GetMouseButton(0))
                {
                    return MouseStatus.Down;
                }
                if (UnityInput.GetMouseButtonUp(0))
                {
                    return MouseStatus.Up;
                }
            }

            return MouseStatus.None;
        }

        void UpdateTouchSim()
        {
            DeltaTime = Time.deltaTime;
            Position = UnityInput.mousePosition;
            DeltaPosition = new Vector2(UnityInput.mousePosition.x, UnityInput.mousePosition.y) - lastPosition;

            var mouseStatus = GetMouseStatus();
            lastMouseStatus = mouseStatus;

            switch (mouseStatus)
            {
                case MouseStatus.StartDown:
                {
                    Status = TouchStatus.Start;
                    StartPosition = UnityInput.mousePosition;
                    DeltaPosition = Vector2.zero;
                    lastPosition = UnityInput.mousePosition;
                    tapTimeout = TapTimeWindow;
                }
                break;

                case MouseStatus.Down:
                {
                    Status = TouchStatus.Stationary;

                    var mag2 = DeltaPosition.sqrMagnitude;

                    if (mag2 > MoveThreshold * MoveThreshold)
                    {
                        Status = TouchStatus.Moving;
                        lastPosition = Position;
                    }
                }
                break;

                case MouseStatus.Up:
                {
                    Status = TouchStatus.End;

                    if (tapTimeout > 0.0f)
                    {
                        TapCount = TapCount + 1;
                    }
                }
                break;

                case MouseStatus.None:
                {
                    if (Status != TouchStatus.Invalid && Status != TouchStatus.End)
                    {
                        Status = TouchStatus.End;
                    }
                    else
                    {
                        if (Status == TouchStatus.End)
                        {
                            Status = TouchStatus.Invalid;
                        }
                    }
                }
                break;
            }

            tapTimeout -= Time.deltaTime;

            if (tapTimeout < 0.0f)
            {
                TapCount = 1;
            }
        }

        bool GetTouchByID(int id, out Touch touch)
        {
            for (var i = 0; i < UnityInput.touchCount; i++)
            {
                var t = UnityInput.GetTouch(i);

                if (t.fingerId == id)
                {
                    touch = t;
                    return true;
                }
            }

            touch = new Touch();
            return false;
        }

        void UpdateTouchInput()
        {
            Touch touch;

            if (GetTouchByID(FingerId, out touch))
            {
                DeltaPosition = touch.position - lastPosition;
                DeltaTime = touch.deltaTime;
                Position = touch.position;
                TapCount = touch.tapCount;

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                    {
                        StartPosition = touch.position;
                        DeltaPosition = Vector2.zero;
                        lastPosition = Position;
                        Status = TouchStatus.Start;
                        PressTime = 0.0f;
                    }
                    break;

                    case TouchPhase.Stationary:
                    case TouchPhase.Moved:
                    {
                        Status = TouchStatus.Stationary;

                        var mag2 = DeltaPosition.sqrMagnitude;

                        if (mag2 > MoveThreshold * MoveThreshold)
                        {
                            Status = TouchStatus.Moving;
                            lastPosition = Position;
                        }

                        PressTime += Time.deltaTime;
                    }
                    break;

                    case TouchPhase.Ended:
                    {
                        Status = TouchStatus.End;
                    }
                    break;

                    default:
                    {
                        Status = TouchStatus.Invalid;
                    }
                    break;
                }
            }
            else
            {
                if (Status != TouchStatus.Invalid && Status != TouchStatus.End)
                {
                    Status = TouchStatus.End;
                }
                else
                {
                    if (Status == TouchStatus.End)
                    {
                        Status = TouchStatus.Invalid;
                    }
                }
            }
        }
    }
}
