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
		[SerializeField]
		Transform cameraTarget;

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
		
		#region Unity
		void Start()
		{
			td.position = transform.position;
			transform.rotation = Quaternion.LookRotation(cameraTarget.position - transform.position, Vector3.up);
			td.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
//			td.eulerAngles = transform.eulerAngles;
			td.eulerAngles.x = Mathf.Clamp(transform.eulerAngles.x, PanAngleMin, PanAngleMax);

			init = td;
			
		}
		
		void Update()
		{
			if(!isplayingCinematique)
			{
				if(playCinematique && cinematiqueTarget )
				{
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

		#region API
		public void Reset()
		{
			td = init;
			disablePlayingCinematique();
		}

		public IEnumerator PlayCinematique(Transform target)
		{
			Vector3 pos = target.position;
			Vector3 goToPosition;

			if(target.tag == "Player")
				pos.y += 2.0f;
			goToPosition = pos + target.forward * 2;

			isplayingCinematique = true;

			float t = 0;
			float distance = Vector3.Distance(transform.position, target.position);
			float maxdistance = distance;
			while(distance > 0.1f * maxdistance /10)
			{
				SmoothLookAt(pos, distance/maxdistance);
				t += Time.deltaTime * 1/5;
				transform.position = Vector3.Lerp(transform.position, goToPosition, Mathf.SmoothStep(0.0f, 1.0f, t));
				distance = Vector3.Distance(transform.position, goToPosition);
				yield return null;
			}
			transform.position = goToPosition;
			transform.LookAt(pos);
			playCinematique = false;
		}
		
		public void enablePlayingCinematique()
		{
			isplayingCinematique = true;
		}
		
		public void disablePlayingCinematique()
		{
			isplayingCinematique = false;
		}
		
		public bool getIsPlayingCinematique()
		{
			return isplayingCinematique;
		}
		#endregion
		
		#region Private
		private void UpdateCamera()
		{
			Vector3 translation = Vector3.zero;
			Vector2 trRot = Vector2.zero;

			float angle = transform.rotation.eulerAngles.y;

			Vector3 right = new Vector3(Mathf.Sin(Mathf.Deg2Rad * (angle + 90)), 0,Mathf.Cos(Mathf.Deg2Rad * (angle + 90)) );
			Vector3 forward = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0,Mathf.Cos(Mathf.Deg2Rad * angle) );

			/////////////////////////////////////////////////////   MOVE CAMERA      ////////////////////////////////////////////////////////////////

			// Move camera with arrow keys
//			translation += Input.GetAxis("Horizontal") * right;
//			translation += Input.GetAxis("Vertical") * forward;

			trRot.x -= Input.GetAxis("Horizontal");
			trRot.y -= Input.GetAxis("Vertical");
			// Move camera with mouse
//			if (Input.GetKey(KeyCode.Mouse1) && draggable) 
//			{
//				// Hold button and drag camera around
//				translation -= Input.GetAxis("Mouse X") * DragSpeed * Time.deltaTime * right;
//				translation -= Input.GetAxis("Mouse Y") * DragSpeed * Time.deltaTime * forward;
//			}
//			else if (mouseBorders)
//			{
//				// Move camera if mouse pointer reaches screen borders
//				if (Input.mousePosition.x < ScrollArea)
//				{
//					translation += right * -ScrollSpeed * Time.deltaTime;
//				}
//				
//				if (Input.mousePosition.x >= Screen.width - ScrollArea)
//				{
//					translation += right * ScrollSpeed * Time.deltaTime;
//				}
//				
//				if (Input.mousePosition.y < ScrollArea)
//				{
//					translation += forward * -ScrollSpeed * Time.deltaTime;
//				}
//				
//				if (Input.mousePosition.y > Screen.height - ScrollArea)
//				{
//					translation += forward * ScrollSpeed * Time.deltaTime;
//				}
//			}

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

//					translation -= transform.up * Input.GetAxis("Mouse Y") * RotSpeed * 0.1f;
					trRot.x += Input.GetAxis("Mouse X");
					trRot.y += Input.GetAxis("Mouse Y");
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

			if((trRot.x !=0 || trRot.y != 0 || Input.GetKey(KeyCode.Mouse2)) && NewCameraRotation)
			{
//				GenerateCameraTarget();

				GameObject dummy = new GameObject();
				
				transform.rotation = Quaternion.LookRotation(cameraTarget.position - transform.position, Vector3.up);
				td.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
				transform.rotation = Quaternion.Lerp (Quaternion.Euler(transform.eulerAngles), Quaternion.Euler(td.eulerAngles), Time.deltaTime * smooth);

				dummy.transform.position = transform.position;
				dummy.transform.rotation = transform.rotation;
				
				dummy.transform.RotateAround(cameraTarget.position,-transform.right, trRot.y * RotSpeed * Time.deltaTime);
				dummy.transform.RotateAround(cameraTarget.position,Vector3.up, trRot.x * RotSpeed * Time.deltaTime);


				if(dummy.transform.eulerAngles.z > 100 && dummy.transform.eulerAngles.z < 200) {}
				else if(dummy.transform.eulerAngles.x >= PanAngleMin && dummy.transform.eulerAngles.x <= PanAngleMax)
				{
					transform.position = dummy.transform.position;
					transform.rotation = dummy.transform.rotation;
					//transform.RotateAround(rotationTarget,-transform.right, Input.GetAxis("Mouse Y") * RotSpeed * Time.deltaTime);
				}
				else
					transform.RotateAround(cameraTarget.position,Vector3.up, trRot.x * RotSpeed * Time.deltaTime);

				if(transform.eulerAngles.z > 10)
					print (dummy.transform.eulerAngles + "/" + transform.eulerAngles);

				td.position = new Vector3(Mathf.Clamp(transform.position.x, -LevelArea, LevelArea),
				                          Mathf.Clamp(transform.position.y, ZoomMin, ZoomMax), 
				                          Mathf.Clamp(transform.position.z, -LevelArea, LevelArea));
				td.eulerAngles = transform.localEulerAngles;
				Destroy(dummy);
			}
			else
			{
				transform.position = Vector3.Lerp (transform.position, td.position, Time.deltaTime * smooth);

				transform.rotation = Quaternion.LookRotation(cameraTarget.position - transform.position, Vector3.up);
				td.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
				transform.rotation = Quaternion.Lerp (Quaternion.Euler(transform.eulerAngles), Quaternion.Euler(td.eulerAngles), Time.deltaTime * smooth);
			}
  		}

		private void SmoothLookAt (Vector3 target, float smooth)
		{
			//Direction from cameraPos to targetPos
			Vector3 direction = target - transform.position;
			//Creates a rotation with the specified forward and upwards directions.
			Quaternion lookAtRotation = Quaternion.LookRotation(direction, Vector3.up);			
			// smooth goes from 1 to 0 depending on the distance remaining
			transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, (smooth - 1) * -1);
		}

		private void GenerateCameraTarget()
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
#region oldCamera
//
//namespace TheVandals
//{
//	using UnityEngine;
//	using System.Collections;
//	
//	public class CameraController : MonoBehaviour
//	{
//		
//		#region Properties
//		[Header("Configuration")]
//		[SerializeField]
//		[Tooltip("Camera limits in width and length")]
//		int LevelArea = 50;
//		[SerializeField]
//		[Tooltip("Move camera if mouse pointer reaches screen borders")]
//		int ScrollArea = 50;
//		[SerializeField]
//		int ScrollSpeed = 100;
//		[SerializeField]
//		int DragSpeed = 100;
//		
//		[SerializeField]
//		int ZoomSpeed = 25;
//		[SerializeField]
//		int ZoomMin = 1;
//		[SerializeField]
//		int ZoomMax = 60;
//		
//		[SerializeField]
//		int PanSpeed = 40;
//		[SerializeField]
//		int PanAngleMin = 10;
//		[SerializeField]
//		int PanAngleMax = 80;
//		
//		[SerializeField]
//		int RotSpeed = 100;
//		[SerializeField]
//		float smooth = 9f;
//		
//		[Header("Function")]
//		[SerializeField]
//		bool draggable = true;
//		[SerializeField]
//		bool mouseBorders = true;
//		[SerializeField]
//		bool playCinematique = false;
//		[SerializeField]
//		Transform cinematiqueTarget;
//		[SerializeField]
//		bool NewCameraRotation = false;
//		
//		private TargetDestination td = new TargetDestination(Vector3.zero,Vector3.zero);
//		private bool isplayingCinematique = false;
//		private Vector3 rotationTarget = Vector3.zero;
//		private TargetDestination init;
//		
//		private static CameraController _instance;
//		public static CameraController instance
//		{
//			get
//			{
//				if(_instance == null)
//					_instance = GameObject.FindObjectOfType<CameraController>();
//				return _instance;
//			}
//		}
//		#endregion
//		
//		#region Unity
//		void Start()
//		{
//			td.position = transform.position;
//			td.eulerAngles = transform.eulerAngles;
//			td.eulerAngles.x = Mathf.Clamp(transform.eulerAngles.x, PanAngleMin, PanAngleMax);
//			init = td;
//			
//		}
//		
//		void Update()
//		{
//			if(!isplayingCinematique)
//			{
//				if(playCinematique && cinematiqueTarget )
//				{
//					StopCoroutine("PlayCinematique");
//					StartCoroutine("PlayCinematique", cinematiqueTarget);
//				}
//				else
//					UpdateCamera();
//			}
//		}
//		
//		public void Message1()
//		{
//			Debug.Log ("Message 1 received");
//		}
//		
//		public void Message2()
//		{
//			Debug.Log ("Message 2 received");
//		}
//		#endregion
//		
//		#region API
//		public void Reset()
//		{
//			td = init;
//			disablePlayingCinematique();
//		}
//		
//		public IEnumerator PlayCinematique(Transform target)
//		{
//			Vector3 pos = target.position;
//			Vector3 goToPosition;
//			
//			if(target.tag == "Player")
//				pos.y += 2.0f;
//			goToPosition = pos + target.forward * 2;
//			
//			isplayingCinematique = true;
//			
//			float t = 0;
//			float distance = Vector3.Distance(transform.position, target.position);
//			float maxdistance = distance;
//			while(distance > 0.1f * maxdistance /10)
//			{
//				SmoothLookAt(pos, distance/maxdistance);
//				t += Time.deltaTime * 1/5;
//				transform.position = Vector3.Lerp(transform.position, goToPosition, Mathf.SmoothStep(0.0f, 1.0f, t));
//				distance = Vector3.Distance(transform.position, goToPosition);
//				yield return null;
//			}
//			transform.position = goToPosition;
//			transform.LookAt(pos);
//			playCinematique = false;
//		}
//		
//		public void enablePlayingCinematique()
//		{
//			isplayingCinematique = true;
//		}
//		
//		public void disablePlayingCinematique()
//		{
//			isplayingCinematique = false;
//		}
//		
//		public bool getIsPlayingCinematique()
//		{
//			return isplayingCinematique;
//		}
//		#endregion
//		
//		#region Private
//		private void UpdateCamera()
//		{
//			Vector3 translation = Vector3.zero;
//			
//			float angle = transform.rotation.eulerAngles.y;
//			
//			Vector3 right = new Vector3(Mathf.Sin(Mathf.Deg2Rad * (angle + 90)), 0,Mathf.Cos(Mathf.Deg2Rad * (angle + 90)) );
//			Vector3 forward = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0,Mathf.Cos(Mathf.Deg2Rad * angle) );
//			
//			/////////////////////////////////////////////////////   MOVE CAMERA      ////////////////////////////////////////////////////////////////
//			
//			// Move camera with arrow keys
//			translation += Input.GetAxis("Horizontal") * right;
//			translation += Input.GetAxis("Vertical") * forward;
//			
//			// Move camera with mouse
//			if (Input.GetKey(KeyCode.Mouse1) && draggable) 
//			{
//				// Hold button and drag camera around
//				translation -= Input.GetAxis("Mouse X") * DragSpeed * Time.deltaTime * right;
//				translation -= Input.GetAxis("Mouse Y") * DragSpeed * Time.deltaTime * forward;
//			}
//			else if (mouseBorders)
//			{
//				// Move camera if mouse pointer reaches screen borders
//				if (Input.mousePosition.x < ScrollArea)
//				{
//					translation += right * -ScrollSpeed * Time.deltaTime;
//				}
//				
//				if (Input.mousePosition.x >= Screen.width - ScrollArea)
//				{
//					translation += right * ScrollSpeed * Time.deltaTime;
//				}
//				
//				if (Input.mousePosition.y < ScrollArea)
//				{
//					translation += forward * -ScrollSpeed * Time.deltaTime;
//				}
//				
//				if (Input.mousePosition.y > Screen.height - ScrollArea)
//				{
//					translation += forward * ScrollSpeed * Time.deltaTime;
//				}
//			}
//			
//			/////////////////////////////////////////////////////   ROTATE CAMERA     ///////////////////////////////////////////////////////////////////////////
//			
//			if(NewCameraRotation)
//			{
//				//Zoom In/Out
//				if(Input.GetAxis("Mouse ScrollWheel")!=0)
//				{
//					float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;
//					
//					
//					if( Input.GetAxis("Mouse ScrollWheel") > 0 && td.position.y + (transform.forward.y * ZoomSpeed * zoomDelta) <= ZoomMin){}								
//					else
//						translation += transform.forward * ZoomSpeed * zoomDelta;
//				}
//				
//				if(Input.GetKey(KeyCode.Mouse2))
//				{
//					translation = Vector2.zero;
//					//translation -= transform.right * Input.GetAxis("Mouse X") * RotSpeed * 0.1f;
//					
//					translation -= transform.up * Input.GetAxis("Mouse Y") * RotSpeed * 0.1f;
//				}
//			}
//			else
//			{
//				//Zoom In/Out
//				if(Input.GetAxis("Mouse ScrollWheel")!=0)
//				{
//					float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;
//					
//					translation -= Vector3.up * ZoomSpeed * zoomDelta;	
//					
//					// Start panning camera if zooming in close to the ground or if just zooming out.
//					float pan = transform.eulerAngles.x - zoomDelta * PanSpeed;
//					pan = Mathf.Clamp(pan, PanAngleMin, PanAngleMax);
//					
//					if (zoomDelta < 0 || td.position.y < (ZoomMin+((ZoomMax-ZoomMin)/2)))
//					{
//						//camera.transform.eulerAngles = new Vector3(pan, 0, 0);
//						td.eulerAngles.x = (int)pan;
//					}		
//				}
//				
//				if(Input.GetKey(KeyCode.Mouse2))
//				{
//					td.eulerAngles.y += Input.GetAxis("Mouse X") * RotSpeed;
//				}
//			}
//			
//			/////////////////////////////////////////////////////   CHECK CAMERA BOUNDARIES     ///////////////////////////////////////////////////////////////////
//			
//			// Keep camera within level and zoom area
//			Vector3 desiredPosition = td.position + translation;
//			if (desiredPosition.x < -LevelArea || LevelArea < desiredPosition.x)
//			{
//				translation.x = 0;
//			}
//			if (desiredPosition.y < ZoomMin || ZoomMax < desiredPosition.y)
//			{
//				translation.y = 0;
//			}
//			if (desiredPosition.z < -LevelArea || LevelArea < desiredPosition.z)
//			{
//				translation.z = 0;
//			}
//			
//			td.position += translation;
//			
//			if(Input.GetKey(KeyCode.Mouse2) && NewCameraRotation)
//			{
//				//				GenerateCameraTarget();
//				
//				GameObject dummy = new GameObject();
//				
//				dummy.transform.position = transform.position;
//				dummy.transform.rotation = transform.rotation;
//				
//				dummy.transform.RotateAround(rotationTarget,-transform.right, Input.GetAxis("Mouse Y") * RotSpeed * Time.deltaTime);
//				dummy.transform.RotateAround(rotationTarget,Vector3.up, Input.GetAxis("Mouse X") * RotSpeed * Time.deltaTime);
//				
//				
//				if(dummy.transform.eulerAngles.z > 100 && dummy.transform.eulerAngles.z < 200) {}
//				else if(dummy.transform.eulerAngles.x >= PanAngleMin && dummy.transform.eulerAngles.x <= PanAngleMax)
//				{
//					transform.position = dummy.transform.position;
//					transform.rotation = dummy.transform.rotation;
//					//transform.RotateAround(rotationTarget,-transform.right, Input.GetAxis("Mouse Y") * RotSpeed * Time.deltaTime);
//				}
//				else
//					transform.RotateAround(rotationTarget,Vector3.up, Input.GetAxis("Mouse X") * RotSpeed * Time.deltaTime);
//				
//				if(transform.eulerAngles.z > 10)
//					print (dummy.transform.eulerAngles + "/" + transform.eulerAngles);
//				
//				td.position = new Vector3(Mathf.Clamp(transform.position.x,-LevelArea, LevelArea),
//				                          Mathf.Clamp(transform.position.y,ZoomMin, ZoomMax), 
//				                          Mathf.Clamp(transform.position.z,-LevelArea, LevelArea));
//				td.eulerAngles = transform.localEulerAngles;
//				Destroy(dummy);
//			}
//			else
//			{
//				transform.position = Vector3.Lerp (transform.position, td.position, Time.deltaTime * smooth);
//				transform.rotation = Quaternion.Lerp (Quaternion.Euler(transform.eulerAngles), Quaternion.Euler(td.eulerAngles), Time.deltaTime * smooth);
//			}
//		}
//		
//		private void SmoothLookAt (Vector3 target, float smooth)
//		{
//			//Direction from cameraPos to targetPos
//			Vector3 direction = target - transform.position;
//			//Creates a rotation with the specified forward and upwards directions.
//			Quaternion lookAtRotation = Quaternion.LookRotation(direction, Vector3.up);			
//			// smooth goes from 1 to 0 depending on the distance remaining
//			transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, (smooth - 1) * -1);
//		}
//		
//		private void GenerateCameraTarget()
//		{
//			Ray ray = new Ray(transform.position,transform.forward);
//			// create a plane at 0,0,0 whose normal points to +Y:
//			Plane hPlane = new Plane(Vector3.up, Vector3.zero);
//			// Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
//			float distance = 0; 
//			// if the ray hits the plane...
//			if (hPlane.Raycast(ray, out distance)){
//				// get the hit point:
//				rotationTarget = ray.GetPoint(distance);
//			}
//		}
//		
//		#endregion
//	}
//}
#endregion
