namespace TheVandals
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	[ExecuteInEditMode, RequireComponent (typeof(MeshFilter)), RequireComponent (typeof(MeshRenderer))]
	public class FieldOfView : MonoBehaviour
	{
		//
		// Fields
		//
		private float angle_offset;
		
		private float angle_end;
		
		private float angle_curr;
		
		private Vector3 pos_curr_min;
		
		private float angle_next;
		
		private float angle_start;
		
		private int minMeshQuality = 18;
		
		private float maxRange = 180f;
		
		private int maxMeshQuality = 2500;
		
		private float angle_lookat;
		
		protected bool isSecondFoV;
		
		private Vector3 pos_curr_max;
		
		private Vector3 sphere_next;
		
		private Vector3 sphere_curr;
		
		private Vector3 tempRayVectorMiddle;
		
		private bool nonInteractiveInit;
		
		private float rayActualAngleMiddle;
		
		private Vector3 origin;
		
		private Vector3 pos_next_max;
		
		private Vector3 pos_next_min;
		
		private Vector3[] vertices;
		
		private float rayActualAngle;
		
		private int[] triangles;
		
		private int previousMeshQuality = 250;
		
		public float fovDepth = 10f;
		
		private float previousFovVertexOffset;
		
		private float previousFovDepth = 10f;
		
		private float previousFovRange = 45f;
		
		public float fovRange = 45f;
		
		public bool interactiveFOV = true;
		
		public bool executeInEditMode = true;
		
		private bool previousInteractiveFOV = true;
		
		public float fovVertexOffset;
		
		public int meshQuality = 250;
		
		public float heightDetectionOffset;
		
		private MeshFilter meshFilter;
		
		private Mesh mesh;
		
		private float minVertexOffset;
		
		private float minRange = 0.1f;
		
		private float maxDepth = float.PositiveInfinity;
		
		public List<Transform> detectedObjects;
		
		public float updateRate = 0.01f;
		
		public LayerMask layerFOV;
		
		private float minUpdateRate = 0.01f;
		
		private float previousUpdateRate = 0.01f;
		
		private float maxUpdateRate = float.PositiveInfinity;

		[HideInInspector]
		public  bool canSearch = false;

		public float rotationAngle = 45.0f;

		public float rotateSpeed = 1.0f;

		private Transform player = null;

		private float timer = 0.0f;

		private EnemyController enemyController = null;

		private IAController iaController = null;

		private MeshRenderer meshRenderer;

		private float defaultAlpha = 0.0f;
		//
		// Methods
		//
		private void DrawFoV ()
		{
			if(Application.isPlaying)
			{
				UpdateFoVColor();
				if(canSearch)
				{
					RotateFOV();
				}
				else
				{
					timer = 0.0f;
					if(transform.rotation.y != 0)
						transform.rotation = Quaternion.Euler(0.0f, transform.parent.transform.rotation.eulerAngles.y , 0.0f);
				}
			}

			this.angle_lookat = 0f;
			this.angle_start = this.angle_lookat - this.fovRange;
			this.angle_end = this.angle_lookat + this.fovRange;
			this.angle_offset = (this.angle_end - this.angle_start) / (float)this.meshQuality;
			this.angle_curr = this.angle_start;
			this.angle_next = this.angle_start + this.angle_offset;
			this.pos_curr_min = Vector3.zero;
			this.pos_curr_max = Vector3.zero;
			this.pos_next_min = Vector3.zero;
			this.pos_next_max = Vector3.zero;
			this.vertices = new Vector3[4 * this.meshQuality];
			this.triangles = new int[6 * this.meshQuality];
			this.rayActualAngle = -this.fovRange;
			this.origin = transform.position;
			this.origin.y = this.origin.y + this.heightDetectionOffset;
			CheckPlayerInFoV();
			if (this.detectedObjects != null)
			{
				this.detectedObjects.Clear ();
			}
			for (int i = 0; i < this.meshQuality; i++)
			{
				this.sphere_curr = new Vector3 (Mathf.Sin (0.0174532924f * this.angle_curr), 0f, Mathf.Cos (0.0174532924f * this.angle_curr));
				this.sphere_next = new Vector3 (Mathf.Sin (0.0174532924f * this.angle_next), 0f, Mathf.Cos (0.0174532924f * this.angle_next));
				this.rayActualAngleMiddle = this.rayActualAngle + this.angle_offset / 2f;
				this.tempRayVectorMiddle = Quaternion.AngleAxis (this.rayActualAngleMiddle, transform.up) * transform.forward;
				if (this.fovDepth > 0f)
				{
					float distance = this.fovDepth;
					RaycastHit raycastHit;
					if (Physics.Raycast (new Ray (this.origin, this.tempRayVectorMiddle), out raycastHit, this.fovDepth, this.layerFOV))
					{
						distance = raycastHit.distance;
						if (!this.isSecondFoV && !this.detectedObjects.Contains (raycastHit.collider.transform) && raycastHit.distance > this.fovVertexOffset)
						{
							this.detectedObjects.Add (raycastHit.collider.transform);
						}
						if (this.interactiveFOV && raycastHit.distance > this.fovVertexOffset)
						{
							this.pos_curr_min = this.sphere_curr * this.fovVertexOffset;
							this.pos_curr_max = this.sphere_curr * distance;
							this.pos_next_min = this.sphere_next * this.fovVertexOffset;
							this.pos_next_max = this.sphere_next * distance;
						}
						else
						{
							this.pos_curr_min = this.sphere_curr * this.fovVertexOffset;
							this.pos_curr_max = this.sphere_curr * this.fovDepth;
							this.pos_next_min = this.sphere_next * this.fovVertexOffset;
							this.pos_next_max = this.sphere_next * this.fovDepth;
						}
					}
					else
					{
						this.pos_curr_min = this.sphere_curr * this.fovVertexOffset;
						this.pos_curr_max = this.sphere_curr * this.fovDepth;
						this.pos_next_min = this.sphere_next * this.fovVertexOffset;
						this.pos_next_max = this.sphere_next * this.fovDepth;
					}
				}
				int num = 4 * i;
				int num2 = 4 * i + 1;
				int num3 = 4 * i + 2;
				int num4 = 4 * i + 3;
				this.vertices [num] = this.pos_curr_min;
				this.vertices [num2] = this.pos_curr_max;
				this.vertices [num3] = this.pos_next_max;
				this.vertices [num4] = this.pos_next_min;
				this.triangles [6 * i] = num;
				this.triangles [6 * i + 1] = num2;
				this.triangles [6 * i + 2] = num3;
				this.triangles [6 * i + 3] = num3;
				this.triangles [6 * i + 4] = num4;
				this.triangles [6 * i + 5] = num;
				this.angle_curr += this.angle_offset;
				this.angle_next += this.angle_offset;
				this.rayActualAngle += this.angle_offset;
			}
			if (!this.interactiveFOV)
			{
				if (this.nonInteractiveInit || this.meshQuality != this.previousMeshQuality || this.fovVertexOffset != this.previousFovVertexOffset || this.fovRange != this.previousFovRange || this.fovDepth != this.previousFovDepth)
				{
					if (this.nonInteractiveInit)
					{
						this.nonInteractiveInit = false;
					}
					this.previousMeshQuality = this.meshQuality;
					this.previousFovVertexOffset = this.fovVertexOffset;
					this.previousFovRange = this.fovRange;
					this.previousFovDepth = this.fovDepth;
					this.mesh.Clear ();
					this.mesh.vertices = vertices;
					this.mesh.triangles = triangles;
					this.mesh.RecalculateNormals ();
					this.mesh.Optimize ();
					this.meshFilter.sharedMesh = mesh;
					return;
				}
			}
			else
			{
				this.mesh.Clear ();
				this.mesh.vertices = vertices;
				this.mesh.triangles = triangles;
				this.mesh.RecalculateNormals ();
				this.mesh.Optimize ();
				this.meshFilter.sharedMesh = mesh;
			}
		}
		private void CheckPlayerInFoV()
		{

//			transform.LookAt(player.position);
//			print( player.position + "  /  " +Vector3.Angle(transform.forward, player.position - transform.position));
//			float angle = (transform.position - player.position).y;
//			print(angle + "/" + angle_start + "/" + angle_end);
		}

		private void RotateFOV()
		{
			timer += Time.deltaTime;
			float requiredRotation = transform.parent.transform.rotation.eulerAngles.y + (rotationAngle * Mathf.Sin(timer * rotateSpeed));
			transform.rotation = Quaternion.Euler(0.0f, requiredRotation, 0.0f);
		}
		
		public List<Transform> GetDetectedObjects ()
		{
			return this.detectedObjects;
		}

		public void Reset()
		{
			this.detectedObjects.Clear();
		}

		public virtual void LateUpdate ()
		{
			this.LimitsController ();
			if (Application.isPlaying && this.previousUpdateRate != this.updateRate)
			{
				this.previousUpdateRate = this.updateRate;
				CancelInvoke ();
				InvokeRepeating ("DrawFoV", 0f, this.updateRate);
			}
			if (this.previousInteractiveFOV != this.interactiveFOV)
			{
				this.previousInteractiveFOV = this.interactiveFOV;
				this.nonInteractiveInit = true;
			}
			if (this.isSecondFoV && transform.name != "SecondFoV")
			{
				transform.set_name ("SecondFoV");
			}
			if (!Application.isPlaying && this.executeInEditMode)
			{
				this.DrawFoV ();
			}
		}
		
		private void LimitsController ()
		{
			if (this.fovVertexOffset < this.fovDepth)
			{
				this.fovDepth = Mathf.Clamp (this.fovDepth, this.fovVertexOffset, this.maxDepth);
			}
			else
			{
				this.fovDepth = Mathf.Clamp (this.fovDepth, 0f, this.maxDepth);
			}
			if (this.fovDepth > 0f)
			{
				this.fovVertexOffset = Mathf.Clamp (this.fovVertexOffset, this.minVertexOffset, this.fovDepth - 0.5f);
			}
			else
			{
				if (this.fovDepth == 0f)
				{
					this.fovVertexOffset = 0f;
				}
			}
			this.fovRange = Mathf.Clamp (this.fovRange, this.minRange, this.maxRange);
			this.meshQuality = Mathf.Clamp (this.meshQuality, this.minMeshQuality, this.maxMeshQuality);
			this.updateRate = Mathf.Clamp (this.updateRate, this.minUpdateRate, this.maxUpdateRate);
		}
		
		private void OnDrawGizmos ()
		{
			Vector3 position = transform.position;
			position.y += this.heightDetectionOffset;
			Vector3 vector = position + transform.forward * this.fovDepth;
			Gizmos.color = Color.red;
			if (this.heightDetectionOffset != 0f)
			{
				Gizmos.DrawLine (position, vector);
				Gizmos.DrawLine (position, transform.position);
			}
//			Gizmos.DrawWireSphere(transform.position, fovDepth);
		}
		
		public void SetIsSecondFoV (bool isSecond)
		{
			this.isSecondFoV = isSecond;
		}

		void Awake()
		{

		}

		public virtual void Start ()
		{			
			player = GameObject.FindGameObjectWithTag("Player").transform;
			enemyController = gameObject.GetComponentInParent<EnemyController>();
			iaController = gameObject.GetComponentInParent<IAController>();
			
			this.meshRenderer = GetComponent<MeshRenderer> ();
			this.meshFilter = GetComponent<MeshFilter> ();
			this.mesh = new Mesh ();
			this.meshFilter.sharedMesh = mesh;
			this.isSecondFoV = false;
			this.minVertexOffset = 0f;
			this.maxDepth = float.PositiveInfinity;
			this.minRange = 0.1f;
			this.maxRange = 180f;
			this.minMeshQuality = 18;
			this.maxMeshQuality = 2500;
			this.minUpdateRate = 0.01f;
			this.detectedObjects = new List<Transform> ();
			this.previousMeshQuality = this.meshQuality;
			this.previousFovVertexOffset = this.fovVertexOffset;
			this.previousFovRange = this.fovRange;
			this.previousFovDepth = this.fovDepth;
			this.previousUpdateRate = this.updateRate;
			this.previousInteractiveFOV = this.interactiveFOV;
			this.nonInteractiveInit = true;
			if (Application.isPlaying)
			{
				InvokeRepeating ("DrawFoV", 0f, this.updateRate);
			}
		}

		void UpdateFoVColor() {
			
			defaultAlpha = meshRenderer.material.color.a;
			if(enemyController)
			{
				switch(enemyController.Enemy._Behaviour)
				{
				case EnemyBeHaviour.Idle:
				{
					meshRenderer.material.color = new Color(0, 1, 0, defaultAlpha); // Color Green
					break;
				}
				case EnemyBeHaviour.Curious:
				{
					meshRenderer.material.color = new Color(1, 0.5f, 0, defaultAlpha); // Color Orange
					break;
				}
				case EnemyBeHaviour.Alert:
				{
					meshRenderer.material.color = new Color(1, 0, 0, defaultAlpha); // Color RED
					break;
				}
				case EnemyBeHaviour.Searching:
				{
					meshRenderer.material.color = new Color(0, 0, 1, defaultAlpha); // Color Blue
					break;
				}
				}
			}else
			{
//				print ("FOV doesn't have an Enemy Parent");
				if(iaController)
				{
					switch(iaController.ReturnEnemy().AlertLVL)
					{
					case AlertLevel.None:
					{
						meshRenderer.material.color = new Color(0, 1, 0, defaultAlpha); // Color Green
						break;
					}
					case AlertLevel.Low:
					{
						meshRenderer.material.color = new Color(1, 0.5f, 0, defaultAlpha); // Color Orange
						break;
					}
					case AlertLevel.Medium:
					{
						meshRenderer.material.color = new Color(0, 0, 1, defaultAlpha); // Color Blue
						break;
					}
					case AlertLevel.High:
					{
						meshRenderer.material.color = new Color(1, 0, 0, defaultAlpha); // Color Red
						break;
					}
					}
				}
			}
		}

	}
}
