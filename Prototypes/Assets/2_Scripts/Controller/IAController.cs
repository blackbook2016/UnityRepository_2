namespace TheVandals {

	using UnityEngine;
	using System.Collections;

	[RequireComponent (typeof (NavMeshAgent), typeof (Animator))]
	public class IAController : MonoBehaviour 
	{
		#region Properties
		
		[SerializeField]
		EnemyType enemyType = EnemyType._Busy;

		[SerializeField]
		AreaMask layer = AreaMask.All;

		[SerializeField]
		float searchTime = 12;

		[SerializeField]
		EnemyActions publicAction = EnemyActions.Idle;
		
		public WaypointManager waypointManager = null;
		public PointerProjector enemyProjector = null;
		
		private EnemyEntity enemy = new EnemyEntity();
		
		private bool isDestinationReached = false;
		private bool isSearchDone = false;
		private bool isFollowDone = false;
		private bool isReturnDone = false;

		private bool isWaitDone = false;
		private bool isWatchDone = false;

		private float lowTimer = 0.0f;
		private float maxlowTimer= 3.0f;
		private float fovRotationSpeed = 1.0f;

		private bool isLowAlertStarted = false;
		private bool isMediumAlertStarted = false;

		private bool isWatchingPlayer = false;
		private bool isSearching = false;
		private bool isReturningToInit = false;
		private bool isFollowingPlayerPoint = false;
		private bool isWaiting = false;

		private bool isplayerCaught = false;

		private TargetDestination starting;

		[HideInInspector]
		public bool isReset = false; 
		[HideInInspector]
		public bool heardSound = false;
		#endregion

		#region Unity
		void Awake()
		{
			if(!waypointManager)
				print ("Please Assign a waypoint manager to the StateMachine");
			if(!enemyProjector)
				print ("Please Assign an enemy projector to the StateMachine");
			
			enemy.NavAgent = GetComponent<NavMeshAgent>();
			enemy.Player = GameObject.FindGameObjectWithTag("Player").transform;
			enemy.Anim = GetComponent<Animator>();			
			enemy.Fov = GetComponentInChildren<FieldOfView>();
			enemy.WaypointManager = waypointManager;
			enemy.EnemyProjector = enemyProjector;
			enemy.Layer = layer;

			GameController.instance.AddEnemyToList(this);
		}
		
		void Start()
		{
			starting.position = transform.position;
			starting.eulerAngles = transform.eulerAngles;

			InitEnemy();
			InvokeRepeating ("UpdateEnemy", 0, 0.01f);			
		}		
		
		void OnAnimatorMove ()
		{ 
			if (enemy.MoveState != State.Idle && enemy.MoveState != State.Climb && Time.timeScale == 1.0f)
			{
				enemy.NavAgent.velocity = enemy.Anim.deltaPosition / Time.deltaTime;
				
				if(enemy.NavAgent.desiredVelocity != Vector3.zero)
				{
					Quaternion lookRotation = Quaternion.LookRotation(enemy.NavAgent.desiredVelocity);
					transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, enemy.NavAgent.angularSpeed * Time.deltaTime);
				}
			}		
		}
		#endregion

		#region Handlers
		void InitEnemy()
		{
			enemy.enableProjector(false);
			
			enemy.SwitchType(enemyType);
			enemy.Fov.canSearch = enemy.CanSearch;	
			
			enemy.setInit(transform.position, transform.eulerAngles);			

			SwitchAlertLevel(AlertLevel.None);
		}

		void UpdateEnemy()
		{
			if(enemy.SetState == EnemyState.Free)
			{
				if(enemyType != enemy.Type)
					InitEnemy();
				
				if(publicAction != enemy.Action)
					SwitchAction(publicAction);

				if(isReset)
					Reset();
				
				UpdateIsDestinationReached();

				UpdateAlertLevel();

				AlertManager();
				
				ActionManager();
				
				AnimatorManager();
			}
		}

		void UpdateIsDestinationReached()
		{			
			Vector3 enemyPosOnNavMesh = transform.position;
			enemyPosOnNavMesh.y = enemy.NavAgent.destination.y;
			
			if(Vector3.Distance(enemyPosOnNavMesh,enemy.NavAgent.destination) <= 0.1F)
				isDestinationReached = true;
			else
				isDestinationReached = false;
			
			enemy.IsDestinationReached = isDestinationReached;
		}
		
		void UpdateAlertLevel()
		{						
			float distance = Vector3.Distance(EthanController.instance.transform.position, transform.position);	
			if(enemy.Fov.GetDetectedObjects().Contains(enemy.Player) && !CameraController.instance.getIsPlayingCinematique() && !EthanController.instance.IsInSafeZone())
			{				
				EthanController.instance.PlayerIsDetected(this);
				enemy.PlayerDetected = true;				
				enemy.setTargetPos();	
					
				if(enemy.AlertLVL != AlertLevel.High  && enemy.AlertLVL != AlertLevel.Medium && distance >= enemy.Fov.fovDepth * 1 / 3)
				{						
					lowTimer += 0.01f;
					
					if(lowTimer > maxlowTimer)
					{
						lowTimer = 0;
						SwitchAlertLevel( AlertLevel.High);
					}
					else
						SwitchAlertLevel( AlertLevel.Low);				
				}
				else
					SwitchAlertLevel( AlertLevel.High);
			}
			else 
			{			
				enemy.PlayerDetected = false;
				if((enemy.AlertLVL == AlertLevel.Low || enemy.AlertLVL == AlertLevel.Medium) && isReturnDone)
				{
					SwitchAlertLevel( AlertLevel.None);
					EthanController.instance.PlayerIsNotDetected(this);
				}
				
				if(enemy.AlertLVL == AlertLevel.High && !isplayerCaught)
					SwitchAlertLevel(AlertLevel.Medium);

				if(heardSound)
				{
					if(enemy.AlertLVL != AlertLevel.High  && enemy.AlertLVL != AlertLevel.Medium)
					{		
						Transform tempShout;
						if(EthanController.instance.CheckShoutDistance(transform.position, out tempShout))
						{			
							isFollowDone = false;
							enemy.TargetPos = tempShout.position;	
							SwitchAction(EnemyActions.FollowPlayerPoint);	
							enemy.PlayerDetected = true;	
							SwitchAlertLevel( AlertLevel.Low);	
						}			
					}
					if(enemy.Action != EnemyActions.SearchL && enemy.Action != EnemyActions.SearchM && enemy.Action != EnemyActions.ReturnToInitialPosition)
						heardSound = false;
				}
			}	
		}

		void AnimatorManager()
		{
			UpdateIsDestinationReached();

			if(isDestinationReached)
				enemy.MoveState = State.Idle;
			else
				if(enemy.MoveState != State.Run)
					enemy.MoveState = State.Walk;
			enemy.Anim.SetInteger("MoveState",(int)enemy.MoveState);
		}
		#endregion

		#region Alerts
		void AlertManager()
		{
			switch(enemy.AlertLVL)
			{
			case AlertLevel.None:
			{				
//				if(Vector3.Distance(enemy.Player.position, transform.position) < 1.5f)
//					SwitchAction(EnemyActions.WatchPlayer);
//				else
//					if(!isWatchingPlayer)
						NoneAlert();
				break;
			}
			case AlertLevel.Low:
			{
				if(!isLowAlertStarted)
					StartCoroutine("LowAlert");
				break;
			}
			case AlertLevel.Medium:
			{
				if(!isMediumAlertStarted)
					StartCoroutine("MediumAlert");
				break;
			}
			case AlertLevel.High:
			{
				if(!isplayerCaught)
				{
					if(Vector3.Distance(enemy.Player.position, transform.position) < 1.5f)
					{
						CaughtPlayer();
						SwitchAction(EnemyActions.Idle);
					}
					else
					if(enemy.Action != EnemyActions.runTowardPlayer)
						SwitchAction(EnemyActions.runTowardPlayer);				
				}
				break;
			}
			}
		}

		void NoneAlert()
		{
			switch(enemy.Type)
			{
			case EnemyType._Fixe:
			{
				if(enemy.Action != EnemyActions.Idle)
					SwitchAction(EnemyActions.Idle);
				break;
			}
			case EnemyType._Busy:
			{
				if(enemy.Action != EnemyActions.Idle)
					SwitchAction(EnemyActions.Idle);
				break;
			}
			case EnemyType._Camera:
			{
				if(enemy.Action != EnemyActions.Idle)
					SwitchAction(EnemyActions.Idle);
				break;
			}
			case EnemyType._RoamingPath:
			{
				if(enemy.Action != EnemyActions.GoToPathPoint)
					SwitchAction(EnemyActions.GoToPathPoint);
				break;
			}
			case EnemyType._RoamingRandom:
			{
				if(enemy.Action != EnemyActions.GoToRandomPoint)
					SwitchAction(EnemyActions.GoToRandomPoint);		
				break;
			}
			}			
		}
		
		IEnumerator LowAlert()
		{
			isLowAlertStarted = true;
			while(!isWatchDone)
			{
				SwitchAction(EnemyActions.WatchPlayer);
				yield return null;
			}

			isFollowDone = false;
			SwitchAction(EnemyActions.FollowPlayerPoint);

			isReturnDone = false;
			while(!isReturnDone)
			{
				if(isFollowDone)
				{
					if(enemy.PlayerDetected || heardSound)
					{
						Transform tempShout;
						if(EthanController.instance.CheckShoutDistance(transform.position, out tempShout))
						{			
							isFollowDone = false;
							enemy.TargetPos = tempShout.position;								
							SwitchAction(EnemyActions.FollowPlayerPoint);		
						}
							if(enemy.PlayerDetected)
						{
							isFollowDone = false;
							SwitchAction(EnemyActions.FollowPlayerPoint);
						}
						heardSound = false;
					}
					else 
					{
						if(!isSearchDone)
						{
							if(!isSearching)
								SwitchAction(EnemyActions.SearchL);
						}else
							SwitchAction(EnemyActions.ReturnToInitialPosition);

					}
				}
				yield return null;
			}
//			isLowAlertStarted = false;
		}

		IEnumerator MediumAlert()
		{
			isMediumAlertStarted = true;

			SwitchAction(EnemyActions.FollowPlayerPoint);
			
			isReturnDone = false;
			while(!isReturnDone)
			{
				if(isFollowDone)
				{
					if(enemy.PlayerDetected || heardSound)
					{
						Transform tempShout;
						if(EthanController.instance.CheckShoutDistance(transform.position, out tempShout))
						{			
							isFollowDone = false;
							enemy.TargetPos = tempShout.position;		
							SwitchAction(EnemyActions.FollowPlayerPoint);		
						}
						else
							if(enemy.PlayerDetected)
						{
							isFollowDone = false;
							SwitchAction(EnemyActions.FollowPlayerPoint);
						}
						heardSound = false;
					}
					else 
					{
						if(!isSearchDone)
						{
							if(!isSearching)
								SwitchAction(EnemyActions.SearchM);
						}else
							SwitchAction(EnemyActions.ReturnToInitialPosition);
					}						
				}
				yield return null;
			}
		}
		#endregion

		#region Actions
		void ActionManager()
		{
			switch(enemy.Action)
			{
			case EnemyActions.Idle:
			{
				enemy.NavAgent.Stop();
//				print ("here");
				break;
			}
			case EnemyActions.GoToRandomPoint:
			{
				if(isDestinationReached)
				{
					if(!isWaitDone)
					{
						if(!isWaiting)
							StartCoroutine("WaitSeconds");
					}
					else
					{
						enemy.GoToRandomPoint(transform);
						isWaitDone = false;
					}
				}
				break;
			}
			case EnemyActions.GoToPathPoint:
			{
				if(isDestinationReached)
				{
					if(!isWaitDone)
					{
						if(!isWaiting)
							StartCoroutine("WaitSeconds");
					}
					else
					{
						enemy.GoToPathPoint(transform);	
						isWaitDone = false;
					}
				}		
				break;
			} 
			case EnemyActions.WatchPlayer:
			{
				if(!isWatchingPlayer)
					StartCoroutine("WatchPlayer");
				break;
			}
			case EnemyActions.SearchL:
			{
				if(!isSearching && !isSearchDone)			
					StartCoroutine("SearchL");
				break;
			}
			case EnemyActions.SearchM:
			{
				if(!isSearching && !isSearchDone)		
					StartCoroutine("SearchM");
				break;
			}
			case EnemyActions.FollowPlayerPoint:
			{			
				if(!isFollowingPlayerPoint)
				{
					if(enemy.AlertLVL == AlertLevel.Low)
						StartCoroutine("FollowPlayerPoint", State.Walk);
					else
						StartCoroutine("FollowPlayerPoint", State.Run);
				}
				break;
			}
			case EnemyActions.ReturnToInitialPosition:
			{		
				if(!isReturningToInit && !isReturnDone)
					StartCoroutine("ReturntoInit");
				break;
			}
			case EnemyActions.runTowardPlayer:
			{		
				enemy.runTowardPlayer();
				break;
			}
			}
		}

		IEnumerator WatchPlayer()
		{
			isWatchingPlayer = true;
			enemy.Fov.canSearch = false;
			enemy.enableProjector(true);
			enemy.ResetNavAgent(transform);
			enemy.Fov.DrawFoV();

//			if(enemy.AlertLVL == AlertLevel.None)
//			{
//				float elapsed = 0;
//				while(transform.rotation != Quaternion.LookRotation(enemy.TargetPos - transform.position))
//				{					
//					elapsed += Time.deltaTime;
//					enemy.ProximityWatchPlayer(transform, elapsed);
//
//					if(Vector3.Distance(enemy.Player.position, transform.position) < 1.5f)
//						enemy.setTargetPos();	
//
//					enemy.EnemyProjector.Project(enemy.TargetPos,Color.red);
//					yield return null;
//				}
//			}
//			else
//			{
				float waitingTime = Time.time + 2.0f;
				while(Time.time < waitingTime)
				{
					enemy.EnemyProjector.Project(enemy.TargetPos,Color.red);
					enemy.WatchPlayer(transform);
					yield return null;
				}
//			}
			isWatchDone = true;
			isWatchingPlayer = false;
		}
		
		IEnumerator SearchM()
		{			
			isSearching = true;
			
			fovRotationSpeed = 3.0f;
			float initFovRotSpeed = enemy.Fov.rotateSpeed;
			enemy.Fov.rotateSpeed *= fovRotationSpeed;			
			enemy.Fov.canSearch = true;

			float sequenceTime = searchTime / 6.0f;

			yield return new WaitForSeconds(sequenceTime);
			
			float rotateTime;
			Vector3 temp_rotation = transform.eulerAngles;
			for(int i =0; i < 3; i++)
			{
				rotateTime = Time.time + (sequenceTime); 
				temp_rotation.y += 120;
				while(Time.time <= rotateTime)
				{
					transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(temp_rotation), Time.deltaTime * 2);
					yield return null;	
				}
				transform.rotation = Quaternion.Euler(temp_rotation);

				yield return new WaitForSeconds(sequenceTime);	
			}
			enemy.Fov.rotateSpeed = initFovRotSpeed;
			
			enemy.Fov.canSearch = enemy.CanSearch;

			isSearchDone = true;
			isSearching = false;

			fovRotationSpeed = 1.0f;
		}

		IEnumerator SearchL()
		{			
			isSearching = true;
			
			enemy.Fov.canSearch = true;
			
			float sequenceTime = searchTime / 5.0f;
			
			yield return new WaitForSeconds(sequenceTime);


			float rotateTime = Time.time + (sequenceTime); 

			float initRotation = transform.eulerAngles.y;
			Vector3 temp_rotation = transform.eulerAngles;
			temp_rotation.y += Random.Range(90,270);

			while(Time.time <= rotateTime)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(temp_rotation), Time.deltaTime * 2);
				yield return null;	
			}
			transform.rotation = Quaternion.Euler(temp_rotation);
			
			yield return new WaitForSeconds(sequenceTime);	

			rotateTime = Time.time + (sequenceTime); 
			temp_rotation.y = initRotation;
			while(Time.time <= rotateTime)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(temp_rotation), Time.deltaTime * 2);
				yield return null;	
			}
			transform.rotation = Quaternion.Euler(temp_rotation);
				
			yield return new WaitForSeconds(sequenceTime);	
			
			enemy.Fov.canSearch = enemy.CanSearch;
			
			isSearchDone = true;
			isSearching = false;
		}
		IEnumerator ReturntoInit()
		{
			isReturningToInit = true;
			enemy.ReturnToInitialPosition();

			do
			{
				UpdateIsDestinationReached();
				yield return null;
			}while(!isDestinationReached);

			while(transform.eulerAngles != enemy.Init.eulerAngles)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(enemy.Init.eulerAngles), Time.deltaTime * 50);
				yield return null;	
			}
			isReturnDone = true;
			isReturningToInit = false;
		}

		IEnumerator FollowPlayerPoint(State st)
		{
			enemy.MoveState = st;
			isFollowingPlayerPoint = true;
			isSearchDone = false;

			enemy.Fov.canSearch = false;
			enemy.enableProjector(true);

			do
			{				
				enemy.EnemyProjector.Project(enemy.TargetPos,Color.red);
				enemy.GoToPosition(enemy.TargetPos);
				UpdateIsDestinationReached();
				yield return null;
			}while(!isDestinationReached);
			isFollowDone = true;
			isFollowingPlayerPoint = false;
		}

		IEnumerator WaitSeconds()
		{
			isWaiting = true;
			yield return new WaitForSeconds(2.0f);
			isWaitDone = true;
			isWaiting = false;
		}
		#endregion

		#region Public
		public EnemyEntity ReturnEnemy()
		{
			return this.enemy;
		}
		
		public void SwitchAction(EnemyActions action)
		{
			if(enemy.Action != action)
			{
				switch(enemy.Action)
				{
				case EnemyActions.Idle:
				{
					enemy.NavAgent.Resume();
					break;
				}
				case EnemyActions.GoToRandomPoint:
				{
					StopCoroutine("WaitSeconds");
					isWaiting = false;
					enemy.ResetNavAgent(transform);
					break;
				}
				case EnemyActions.GoToPathPoint:
				{
					StopCoroutine("WaitSeconds");
					isWaiting = false;
					enemy.ResetNavAgent(transform);
					break;
				}
				case EnemyActions.WatchPlayer:
				{
					StopCoroutine("WatchPlayer");
					isWatchingPlayer = false;
					break;
				}
				case EnemyActions.FollowPlayerPoint:
				{
					StopCoroutine("FollowPlayerPoint");
					isFollowingPlayerPoint = false;
					enemy.enableProjector(false);
					break;
				}
				case EnemyActions.SearchL:
				{
					StopCoroutine("SearchL");
					isSearching = false;
					break;
				}
				case EnemyActions.SearchM:
				{
					StopCoroutine("SearchM");
					isSearching = false;
					enemy.Fov.rotateSpeed = 1.0f;
					break;
				}

				case EnemyActions.ReturnToInitialPosition:
				{
					StopCoroutine("ReturntoInit");
					isReturningToInit = false;
					break;
				}
				}
				
				enemy.Action = action;
				publicAction = action;
			}
		}

		public void SwitchAlertLevel(AlertLevel alertLevel)
		{			
			if(alertLevel != enemy.AlertLVL)
			{				
//				print ("Switch: "  + enemy.AlertLVL + "/" + alertLevel + "/" + enemy.PlayerDetected);
				switch(enemy.AlertLVL)
				{
				case AlertLevel.None:
				{				
					break;
				}
				case AlertLevel.Low:
				{
					StopCoroutine("LowAlert");	
					lowTimer = 0;
					isLowAlertStarted = false;					
					isWatchDone = false;
					isSearchDone = false;
					isReturnDone = false;
					isFollowDone = false;
					break;
				}
				case AlertLevel.Medium:
				{					
					StopCoroutine("LowAlert");	
					isMediumAlertStarted = false;	
					lowTimer = 0;
					isWatchDone = false;
					isSearchDone = false;
					isReturnDone = false;
					isFollowDone = false;
					break;
				}
				case AlertLevel.High:
				{
					isWatchDone = true;
					isSearchDone = false;
					isReturnDone = false;
					isFollowDone = false;
					break;
				}
				}
				enemy.AlertLVL = alertLevel;
			}
		}

		public void CaughtPlayer()
		{
			isplayerCaught = true;
			enemy.Player.GetComponent<EthanController>().isCaught();
			enemy.NavAgent.SetDestination(transform.position);
			enemy.NavAgent.Stop();
		}

		public void Reset()
		{
			enemy.Fov.Reset();
			StopAllCoroutines();
			enemy.Init = starting;
			enemy.NavAgent.Warp(enemy.Init.position);
			transform.rotation = Quaternion.Euler(enemy.Init.eulerAngles);			
			enemy.NavAgent.SetDestination(transform.position);

			enemy.Fov.canSearch = enemy.CanSearch;

			enemy.enableProjector(false);
			enemy.WaypointManager.reset();
			isplayerCaught = false;

			EthanController.instance.PlayerIsNotDetected(this);
			SwitchAlertLevel(AlertLevel.None);
			SwitchAction(EnemyActions.Idle);
			isWatchDone = false;
			isReset = false;
			UpdateAlertLevel();
		}

		public void Freeze()
		{
			enemy.NavAgent.Stop();
			enemy.Anim.speed = 0.0f;
			enemy.Fov.isfrozen = true;
			enemy.SetState = EnemyState.Frozen;	
		}

		public void Resume()
		{
			enemy.NavAgent.Resume();
			enemy.Anim.speed = 1.0f;
			enemy.Fov.isfrozen = false;
			enemy.SetState = EnemyState.Free;
		}
		#endregion

	}
	
}