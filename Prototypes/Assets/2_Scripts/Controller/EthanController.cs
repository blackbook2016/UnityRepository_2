namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	
	[RequireComponent (typeof (NavMeshAgent), typeof (Animator))]
	public class EthanController : MonoBehaviour {
		
		#region Properties
		[Header("Configuration")]
		[Tooltip("Double Click Delays")]
		float delay = 0.25F;
		[SerializeField]
		State state = State.Idle;
		[SerializeField]
		PlayerState plState = PlayerState.Free;
		[SerializeField]
		GroundMarker groundMarker = null;

		public GameObject shoutMarkerPrefab = null;
		private  List<Shout> listShoutMarkers = new List<Shout>();	

		private float runningSoundTimer = 0f;

		public float soundDistance = 5.0f;
		
		private float lastClickTimeL = 0F;
		private float lastClickTimeR = 0F;

		private Vector3 linkStart_ ;
		private Vector3 linkEnd_ ;
		private Quaternion linkRot_;

		private NavMeshAgent agent;
		private Animator animator;
		private TargetDestination initDes;

		private Vector3 padVelocity ;
		[SerializeField]
		private bool isSafe = false;
		public  List<IAController> listAlertedGuards = new List<IAController>();

		private bool doubleClicked = false;

		private static EthanController _instance;
		public static EthanController instance
		{
			get
			{
				if(_instance == null)
					_instance = GameObject.FindObjectOfType<EthanController>();
				return _instance;
			}
		}

		#endregion
		
		#region Unity
		void OnDrawGizmos()
		{
//			if(Application.isPlaying && shoutMarker)
//			{
//				Gizmos.DrawWireSphere(shoutMarker.position,shoutMarker.localScale.magnitude/4);
//			}
		}

		void Awake()
		{
			agent = GetComponent<NavMeshAgent>();
			animator = GetComponent<Animator>();
		}
		void Start () 
		{
			agent.autoTraverseOffMeshLink = false;
			
			initDes.position = transform.position;
			initDes.eulerAngles = transform.eulerAngles;
		}	
		
		void Update () 
		{
			if(plState == PlayerState.Free)
			{
				if(state != State.Climb && state != State.Dead && !CameraController.instance.getIsPlayingCinematique())
				{
					if(Input.GetButtonDown("Shout"))
					{					
						StartCoroutine("ShoutCoroutine", soundDistance);
					}
					if(Input.GetAxis("Horizontal_R") != 0 || Input.GetAxis("Vertical_R") != 0)
					{
						if(Input.GetAxis("RunJoystick")!=0)
							state = State.Run;
						else
							if(state != State.Walk)
								state = State.Walk;

						float angleTranslation = Mathf.Atan2(Input.GetAxis("Horizontal_R"),Input.GetAxis("Vertical_R")) * (180 / Mathf.PI);

						transform.rotation = Quaternion.Euler(new Vector3(0,transform.rotation.y + angleTranslation,0));
					}
					else
					{
						if(state != State.Idle && agent.remainingDistance == 0)
						{
							state = State.Idle;
							agent.Warp(transform.position);
						}
					}
					
					if(Input.GetKeyUp(KeyCode.Mouse0))
						doubleClicked = false;

					if(Input.GetKey(KeyCode.Mouse0) && !CameraController.instance.getIsPlayingCinematique())	
						MovePlayer();

					if(Input.GetKeyDown(KeyCode.Mouse1))
						lastClickTimeR = Time.time;
					
					if(Input.GetKeyUp(KeyCode.Mouse1) && Time.time < lastClickTimeR + delay)
						StopPlayer();	
										
					if (agent.hasPath)
					{		
						if(state == State.Idle)
						{
							if(doubleClicked)
								state = State.Run;
							else
								state = State.Walk;
						}
					}
					if(state == State.Run)
					{
						runningSoundTimer += Time.deltaTime;
						if(runningSoundTimer > 0.5f)
						{
							StartCoroutine("ShoutCoroutine",soundDistance / 2);
							runningSoundTimer -= 0.5f;
						}
					}
				}
				else
					if(!CameraController.instance.getIsPlayingCinematique())
						StopPlayer();

				if(agent.isOnOffMeshLink && state != State.Climb)
				{
					StopCoroutine(SelectLinkAnimation());
					StartCoroutine(SelectLinkAnimation());
				}
			}
			
			animator.SetInteger("MoveState", (int)state);
		}
		
		void OnAnimatorMove ()
		{
			if (state != State.Idle && state != State.Climb && state != State.Dead && plState == PlayerState.Free)
			{
				agent.velocity = animator.deltaPosition / Time.deltaTime;
				if(agent.desiredVelocity != Vector3.zero)
				{
					Quaternion lookRotation = Quaternion.LookRotation(agent.desiredVelocity);
					transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, agent.angularSpeed * Time.deltaTime);
				}
//				transform.rotation = Quaternion.LookRotation(agent.desiredVelocity);

			}		
		}

		void OnTriggerEnter(Collider other) 
		{
			if(other.tag == "StreetArt")
			{
				string paintingTexture = other.GetComponentInParent<MeshRenderer>().material.mainTexture.name;
				PaintingManager.instance.setPainting(paintingTexture, other.transform.parent.transform);
			}

			if(other.tag == "SafeZone" && listAlertedGuards.Count == 0)
			{
				isSafe = true;
			}
		}

		void OnTriggerExit(Collider other) 
		{
			if(other.tag == "StreetArt")
				PaintingManager.instance.RemoveText();

			if(other.tag == "SafeZone")
			{
				isSafe = false;
			}
		}

		#endregion
		
		#region Private
		
		private void MovePlayer()
		{
			if(RetrieveMousePosition() != transform.position)
			{
				agent.SetDestination(RetrieveMousePosition());

				if(Input.GetKeyDown(KeyCode.Mouse0))
				{							
					if(state != State.Run && Time.time - lastClickTimeL < delay )
					{
						state = State.Run;
						doubleClicked = true;
					}
					else if(state != State.Walk && Time.time - lastClickTimeL >= delay && Input.GetKeyDown(KeyCode.Mouse0))
					{
						state = State.Walk;
					}
					lastClickTimeL = Time.time;
				}
			}
		}
		
		public  void StopPlayer()
		{
			agent.SetDestination(transform.position);
			state = State.Idle;
			agent.Warp(transform.position);
		}

		public void isCaught()
		{
			StopPlayer();
			plState = PlayerState.Caught;
			state = State.Dead;
			animator.Play("Dying");	
		}

		public void reset()
		{
			state = State.Idle;
			animator.Play("Idle");	

			agent.Warp(initDes.position);
			
			doubleClicked = false;
			GameController.instance.ResetEnemies();
			
			CameraController.instance.Reset();
			plState = PlayerState.Free;
		}

		private Vector3 RetrieveMousePosition()
		{			
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if(Physics.SphereCast (mouseRay, 1.0f, out hit, Mathf.Infinity, 1<<8 | 1<<9) && hit.collider.tag == "StreetArt")
			{				
				return transform.position;
			}
			
			if (Physics.Raycast(mouseRay, out hit,Mathf.Infinity,1 << 8 | 1 << 11))
			{
				if(groundMarker)
					groundMarker.Project(hit.point,Color.white);
				else
					print ("Pointer Projector not defined");

				return hit.point;
			}
			return transform.position;
		}

		private string SelectLinkAnimation() {
			OffMeshLinkData link  = agent.currentOffMeshLinkData;
			float distS  = (transform.position - link.startPos).magnitude;
			float distE  = (transform.position - link.endPos).magnitude;
			if(distS < distE) {
				linkStart_ = link.startPos;
				linkEnd_ = link.endPos;
			} else {
				linkStart_ = link.endPos;
				linkEnd_ = link.startPos;
			}
		
			Vector3 alignDir  = linkEnd_ - linkStart_;
			alignDir.y = 0;
			linkRot_ = Quaternion.LookRotation(alignDir);

			if(link.linkType == OffMeshLinkType.LinkTypeManual && linkStart_.y < linkEnd_.y) {
				return "Locomotion_ClimbAnimation";
			} else {
				return "Locomotion_JumpAnimation";
			}
		}
		#endregion
		
		#region Editor API

		public void StartCaptureOeuvreMode()
		{
			plState = PlayerState.CaptureOeuvre;
			StopPlayer();
		}
		public void EndCaptureOeuvreMode()
		{
			plState = PlayerState.Free;
		}

		public bool IsInSafeZone()
		{
			return isSafe;
		}

		public void PlayerIsDetected(IAController guard)
		{
			if(listAlertedGuards.Contains(guard))
				return;
			else
				listAlertedGuards.Add(guard);
		}
		public void PlayerIsNotDetected(IAController guard)
		{
			if(listAlertedGuards.Contains(guard))
				listAlertedGuards.Remove(guard);
		}

		
		
		public bool CheckShoutDistance(Vector3 enemyPos, out Transform tempShout)
		{
			for(int i = listShoutMarkers.Count-1; i > -1; i--)
			{
				tempShout = listShoutMarkers[i].obj.transform;
				float distance = Vector3.Distance(tempShout.position, enemyPos);	
				if(distance <= tempShout.localScale.magnitude / 4)
					return true;
			}
			tempShout = null;
			return false;
		}
		#endregion 

		#region Action

		private IEnumerator ShoutCoroutine(float radius)
		{
			float shoutTimer = 0.0f;

			GameObject currentShout = Instantiate(shoutMarkerPrefab) as GameObject;
			Shout tempShout = new Shout(currentShout, radius);
			listShoutMarkers.Add(tempShout);
			currentShout.SetActive(false);	

			currentShout.transform.localScale = Vector3.one;
			Vector3 shoutPosition = transform.position;
			shoutPosition.y = currentShout.transform.position.y;
			currentShout.transform.position = shoutPosition;
			currentShout.SetActive(true);			
			SoundController.instance.PlayClip("shoutClip");
			float startingTime = Time.time;

			while(shoutTimer <= 1)
			{
				shoutTimer = (Time.time - startingTime) * 1  ;
				shoutTimer /= 0.6f * (Time.time - startingTime + 0.5f);
				currentShout.transform.localScale = Vector3.one * shoutTimer * radius * 2;
				GameController.instance.PlayerShouted();
				yield return null;
			}

			listShoutMarkers.Remove(tempShout);			
			Destroy(currentShout);
		}

		private IEnumerator Locomotion_ClimbAnimation() 
		{
			agent.Stop();
			
			state = State.Climb;		
			animator.SetInteger("MoveState", (int)state);
			
			animator.Play("Idle_ToJumpUpHigh");	
			
			Quaternion startRot = transform.rotation;
			Vector3 startPos = transform.position;
			float blendTime = 0.2F;
			float tblend = 0F;
			
			
			
			do {
				transform.position = Vector3.Lerp(startPos, linkStart_, tblend/blendTime);
				transform.rotation = Quaternion.Slerp(startRot, linkRot_, tblend/blendTime);
				yield return null;
				tblend += Time.deltaTime;
			} while(tblend < blendTime);
			transform.position = linkStart_;
			
			//			anim_.CrossFade(linkAnimationName, 0.1, PlayMode.StopAll);
			//			agent.ActivateCurrentOffMeshLink(false);
			do {
				transform.rotation = linkRot_;
				transform.position += new Vector3(animator.deltaPosition.x,animator.deltaPosition.y * 2.1F / 3,animator.deltaPosition.z);
				
				yield return null;
			} while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1);
			
			//			agent.ActivateCurrentOffMeshLink(true);
			
			state = State.Idle;
			animator.Play("Idle");
			
			agent.CompleteOffMeshLink();
			agent.Resume();
		}
		
		private IEnumerator Locomotion_JumpAnimation() 
		{
			agent.Stop();
			state = State.Climb;
			
			do{
				transform.position = Vector3.Slerp(transform.position, linkEnd_,agent.speed);
				yield return null;
			} while(transform.position != linkEnd_);
			
			agent.CompleteOffMeshLink();
			agent.Resume();
			
			state = State.Idle;
			animator.Play("Idle");
		}
		#endregion
	}
}
