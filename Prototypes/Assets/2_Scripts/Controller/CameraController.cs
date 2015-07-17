namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	public class CameraController : MonoBehaviour
	{

		#region Properties
		[Header("Configuration")]
		[SerializeField]
		[Tooltip("Camera limits in width and length")]
		int LevelArea = 50;
		[SerializeField]
		[Tooltip("Move camera if mouse pointer reaches screen borders")]
		int ScrollArea = 50;
		[SerializeField]
		int ScrollSpeed = 100;
		[SerializeField]
		int DragSpeed = 100;
		
		[SerializeField]
		int ZoomSpeed = 25;
		[SerializeField]
		int ZoomMin = 1;
		[SerializeField]
		int ZoomMax = 60;
		
		[SerializeField]
		int PanSpeed = 40;
		[SerializeField]
		int PanAngleMin = 10;
		[SerializeField]
		int PanAngleMax = 80;

		[SerializeField]
		int RotSpeed = 100;
		[SerializeField]
		float smooth = 9f;

		[Header("Function")]
		[SerializeField]
		bool draggable = true;
		[SerializeField]
		bool mouseBorders = true;
		[SerializeField]
		bool playCinematique = false;
		[SerializeField]
		Transform cinematiqueTarget;
		[SerializeField]
		bool NewCameraRotation = false;

		private TargetDestination td = new TargetDestination(Vector3.zero,Vector3.zero);
		private bool isplayingCinematique = false;
		private Vector3 rotationTarget = Vector3.zero;
		private TargetDestination init;

		private static CameraController _instance;
		public static CameraController instance
		{
			get
			{
				if(_instance == null)
					_instance = GameObject.FindObjectOfType<CameraController>();
				return _instance;
			}
		}
		#endregion

		#region Events
		#endregion
		
		#region Editor API
		void OnGUI()
		{

		}
		#endregion
		
		#region API
		public void Reset()
		{
			td = init;
		}
		#endregion

		#region Unity

		void Start()
		{
			td.position = transform.position;
			td.eulerAngles.x = Mathf.Clamp(transform.eulerAngles.x, PanAngleMin, PanAngleMax);
			init = td;

		}

		void Update()
		{
			if(!isplayingCinematique)
			{
				if(playCinematique && cinematiqueTarget )
				{
					isplayingCinematique = true;
					StopCoroutine("PlayCinematique");
					StartCoroutine("PlayCinematique", cinematiqueTarget);
				}
				else
					UpdateCamera();
			}
		}

		public void Message1()
		{
			Debug.Log ("Message 1 received");
		}
		
		public void Message2()
		{
			Debug.Log ("Message 2 received");
		}

		#endregion

		#region Handlers
		#endregion
		
		#region Private

		void UpdateCamera()
		{
			Vector3 translation = Vector3.zero;

			float angle = transform.rotation.eulerAngles.y;

			Vector3 right = new Vector3(Mathf.Sin(Mathf.Deg2Rad * (angle + 90)), 0,Mathf.Cos(Mathf.Deg2Rad * (angle + 90)) );
			Vector3 forward = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0,Mathf.Cos(Mathf.Deg2Rad * angle) );

			/////////////////////////////////////////////////////   MOVE CAMERA      ////////////////////////////////////////////////////////////////

			// Move camera with arrow keys
			translation += Input.GetAxis("Horizontal") * right;
			translation += Input.GetAxis("Vertical") * forward;

			// Move camera with mouse
			if (Input.GetKey(KeyCode.Mouse1) && draggable) 
			{
				// Hold button and drag camera around
				translation -= Input.GetAxis("Mouse X") * DragSpeed * Time.deltaTime * right;
				translation -= Input.GetAxis("Mouse Y") * DragSpeed * Time.deltaTime * forward;
			}
			else if (mouseBorders)
			{
				// Move camera if mouse pointer reaches screen borders
				if (Input.mousePosition.x < ScrollArea)
				{
					translation += right * -ScrollSpeed * Time.deltaTime;
				}
				
				if (Input.mousePosition.x >= Screen.width - ScrollArea)
				{
					translation += right * ScrollSpeed * Time.deltaTime;
				}
				
				if (Input.mousePosition.y < ScrollArea)
				{
					translation += forward * -ScrollSpeed * Time.deltaTime;
				}
				
				if (Input.mousePosition.y > Screen.height - ScrollArea)
				{
					translation += forward * ScrollSpeed * Time.deltaTime;
				}
			}

			/////////////////////////////////////////////////////   ROTATE CAMERA     ///////////////////////////////////////////////////////////////////////////

			if(NewCameraRotation)
			{
				//Zoom In/Out
				if(Input.GetAxis("Mouse ScrollWheel")!=0)
				{
					float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;


					if( Input.GetAxis("Mouse ScrollWheel") > 0 && td.position.y + (transform.forward.y * ZoomSpeed * zoomDelta) <= ZoomMin){}								
					else
						translation += transform.forward * ZoomSpeed * zoomDelta;
				}

				if(Input.GetKey(KeyCode.Mouse2))
				{
					translation = Vector2.zero;
					//translation -= transform.right * Input.GetAxis("Mouse X") * RotSpeed * 0.1f;

					translation -= transform.up * Input.GetAxis("Mouse Y") * RotSpeed * 0.1f;
				}
			}
			else
			{
				//Zoom In/Out
				if(Input.GetAxis("Mouse ScrollWheel")!=0)
				{
					float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;

					translation -= Vector3.up * ZoomSpeed * zoomDelta;	

					// Start panning camera if zooming in close to the ground or if just zooming out.
					float pan = transform.eulerAngles.x - zoomDelta * PanSpeed;
					pan = Mathf.Clamp(pan, PanAngleMin, PanAngleMax);
					
					if (zoomDelta < 0 || td.position.y < (ZoomMin+((ZoomMax-ZoomMin)/2)))
					{
						//camera.transform.eulerAngles = new Vector3(pan, 0, 0);
						td.eulerAngles.x = (int)pan;
					}		
				}

				if(Input.GetKey(KeyCode.Mouse2))
				{
					td.eulerAngles.y += Input.GetAxis("Mouse X") * RotSpeed;
				}
			}

			/////////////////////////////////////////////////////   CHECK CAMERA BOUNDARIES     ///////////////////////////////////////////////////////////////////

			// Keep camera within level and zoom area
			Vector3 desiredPosition = td.position + translation;
			if (desiredPosition.x < -LevelArea || LevelArea < desiredPosition.x)
			{
				translation.x = 0;
			}
			if (desiredPosition.y < ZoomMin || ZoomMax < desiredPosition.y)
			{
				translation.y = 0;
			}
			if (desiredPosition.z < -LevelArea || LevelArea < desiredPosition.z)
			{
				translation.z = 0;
			}

			td.position += translation;

			if(Input.GetKey(KeyCode.Mouse2) && NewCameraRotation)
			{
				GenerateCameraTarget();

				GameObject dummy = new GameObject();

				dummy.transform.position = transform.position;
				dummy.transform.rotation = transform.rotation;
				
				dummy.transform.RotateAround(rotationTarget,-transform.right, Input.GetAxis("Mouse Y") * RotSpeed * Time.deltaTime);
				dummy.transform.RotateAround(rotationTarget,Vector3.up, Input.GetAxis("Mouse X") * RotSpeed * Time.deltaTime);


				if(dummy.transform.eulerAngles.z > 100 && dummy.transform.eulerAngles.z < 200) {}
				else if(dummy.transform.eulerAngles.x >= PanAngleMin && dummy.transform.eulerAngles.x <= PanAngleMax)
				{
					transform.position = dummy.transform.position;
					transform.rotation = dummy.transform.rotation;
					//transform.RotateAround(rotationTarget,-transform.right, Input.GetAxis("Mouse Y") * RotSpeed * Time.deltaTime);
				}
				else
					transform.RotateAround(rotationTarget,Vector3.up, Input.GetAxis("Mouse X") * RotSpeed * Time.deltaTime);

				if(transform.eulerAngles.z > 10)
					print (dummy.transform.eulerAngles + "/" + transform.eulerAngles);

				td.position = new Vector3(Mathf.Clamp(transform.position.x,-LevelArea, LevelArea),
				                          Mathf.Clamp(transform.position.y,ZoomMin, ZoomMax), 
				                          Mathf.Clamp(transform.position.z,-LevelArea, LevelArea));
				td.eulerAngles = transform.localEulerAngles;
				Destroy(dummy);
			}
			else
			{
				transform.position = Vector3.Lerp (transform.position, td.position, Time.deltaTime * smooth);
				//transform.rotation = Quaternion.Lerp (Quaternion.Euler(transform.eulerAngles), Quaternion.Euler(td.eulerAngles), Time.deltaTime * smooth);
			}
  		}

		IEnumerator PlayCinematique(Transform target)
		{
			//TargetDestination initial = new TargetDestination(transform.position,transform.eulerAngles);

			float distance = Vector3.Distance(transform.position, target.position);
			float maxdistance = distance;
			while(distance> 2 * maxdistance /10)
			{
				SmoothLookAt(target);
				transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * distance / maxdistance);
				distance = Vector3.Distance(transform.position, target.position);
				yield return null;
			}
			
			print ("WaitForSeconds");
			yield return new WaitForSeconds(3f);
			
			print ("finished");
			playCinematique = false;
			isplayingCinematique = false;
		}

		void SmoothLookAt (Transform target)
		{
			// Create a vector from the camera towards the player.
			Vector3 relPlayerPosition = target.position - transform.position;
			
			// Create a rotation based on the relative position of the player being the forward vector.
			Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);
			
			// Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
			transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
		}

		void GenerateCameraTarget()
		{
			Ray ray = new Ray(transform.position,transform.forward);
			// create a plane at 0,0,0 whose normal points to +Y:
			Plane hPlane = new Plane(Vector3.up, Vector3.zero);
			// Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
			float distance = 0; 
			// if the ray hits the plane...
			if (hPlane.Raycast(ray, out distance)){
				// get the hit point:
				rotationTarget = ray.GetPoint(distance);
			}
		}

		#endregion
	}
}