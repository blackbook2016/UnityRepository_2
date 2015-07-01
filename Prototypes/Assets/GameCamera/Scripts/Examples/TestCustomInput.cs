using RG_GameCamera.Input;
using UnityEngine;

namespace RG_GameCamera.Examples
{
    /// <summary>
    /// this example shows how to use custom input preset with GameCamera
    /// you can use it in combination with some other third party input plugins
    /// </summary>
    public class TestCustomInput : MonoBehaviour
    {
        private void Update()
        {
            //
            // this is necessary to call on your custom Input script to make sure all the inputs are invalid before the update
            //
            RG_GameCamera.Input.InputManager.Instance.ResetInputArray();

            //
            // get custom input preset
            //
            var customInput = RG_GameCamera.Input.InputManager.Instance.GetInputPresetCurrent() as CustomInput;

            if (customInput)
            {
                //
                // handle zoom
                //
                customInput.OnZoom(UnityEngine.Input.GetAxis("Mouse ScrollWheel"));

                //
                // handle go to waypoint (RTS/RPG mode)
                //
                if (UnityEngine.Input.GetMouseButtonUp(0))
                {
                    Vector3 pos;
                    if (GameInput.FindWaypointPosition(UnityEngine.Input.mousePosition, out pos))
                    {
                        customInput.OnWaypoint(pos);
                    }
                }

                //
                // handle pan
                //
                if (UnityEngine.Input.GetMouseButton(0))
                {
                    customInput.OnPan(UnityEngine.Input.mousePosition);
                }
            }
        }
    }
}
