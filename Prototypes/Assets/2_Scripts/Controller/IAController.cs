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
		float searchTime = 15f;

		[SerializeField]
		EnemyActions publicAction = EnemyActions.Idle;

		[SerializeField]
		AlertLevel publicAlert = AlertLevel.None;
		
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

		private bool isLowAlertStarted = false;
		private bool isMediumAlertStarted = false;

		private bool isWatchingPlayer = false;
		private bool isSearching = false;
		private bool isReturningToInit = false;
		private bool isFollowingPlayerPoint = false;
		private bool isWaiting = false;
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

		}
		
		void Start()
		{
			InitEnemy();
			InvokeRepeating ("UpdateEnemy", 0, 0.01f);			
		}		
		
		void OnAnimatorMove ()
		{ 
			if (enemy.MoveState != State.Idle && enemy.MoveState != State.Climb)
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
			if(enemyType != enemy.Type)
				InitEnemy();
			
			if(publicAction != enemy.Action)
				SwitchAction(publicAction);
			
			UpdateIsDestinationReached();
			
			UpdateAlertLevel();
			
			AlertManager();
			
			ActionManager();
			
			AnimatorManager();
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
			if(enemy.Fov.GetDetectedObjects().Contains(enemy.Player))
			{								
				enemy.PlayerDetected = true;				
				enemy.setTargetPos();
				
				float distance = Vector3.Distance(enemy.Player.position, transform.position);				
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
					SwitchAlertLevel( AlertLevel.None);
				
				if(enemy.AlertLVL == AlertLevel.High)
					SwitchAlertLevel(AlertLevel.Medium);
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
				NoneAlert();
				break;
			}
			case AlertLevel.Low:
			{
				if(!isLowAlertStarted)
				{					
					StartCoroutine("LowAlert");
					print ("started " + isLowAlertStarted);
				}
				break;
			}
			case AlertLevel.Medium:
			{
				if(!isMediumAlertStarted)
				{					
					StartCoroutine("MediumAlert");
					print ("started " + isMediumAlertStarted);
				}
				break;
			}
			case AlertLevel.High:
			{
				if(Vector3.Distance(transform.position,enemy.Player.position) < 1)
				{
					CaughtPlayer();
				}
				else
					if(enemy.Action != EnemyActions.runTowardPlayer)
						SwitchAction(EnemyActions.runTowardPlayer);
				
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
					if(enemy.PlayerDetected)
					{
						isFollowDone = false;
						SwitchAction(EnemyActions.FollowPlayerPoint);
					}
					else 
					{
						if(!isSearchDone)
						{
							if(!isSearching)
								SwitchAction(EnemyActions.Search);
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
						if(!isSearchDone)
						{
							if(!isSearching)
								SwitchAction(EnemyActions.Search);
						}else
							SwitchAction(EnemyActions.ReturnToInitialPosition);
						
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
						enemy.GoToRandomPoint(transform);
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
						enemy.GoToPathPoint(transform);	
				}		
				break;
			} 
			case EnemyActions.WatchPlayer:
			{
				if(!isWatchingPlayer)
					StartCoroutine("WatchPlayer");
				break;
			}
			case EnemyActions.Search:
			{
				if(!isSearching)			
					StartCoroutine("Search");
				break;
			}
			case EnemyActions.FollowPlayerPoint:
			{			
				if(!isFollowingPlayerPoint)
				{
					StartCoroutine("FollowPlayerPoint");
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
			
			enemy.enableProjector(true);

			float waitingTime = Time.time + 2.0f;
			while(Time.time < waitingTime)
			{
				enemy.EnemyProjector.Project(enemy.TargetPos,Color.red);
				enemy.WatchPlayer(transform);
				yield return null;
			}
			isWatchDone = true;
			isWatchingPlayer = false;
		}

		IEnumerator Search()
		{			
			isSearching = true;

			enemy.Fov.canSearch = true;

			float sequenceTime = searchTime / 6.0f;
//			float initFovRotSpeed = enemy.Fov.rotateSpeed;
//			enemy.Fov.rotateSpeed = sequenceTime / (Mathf.PI * 2);

			yield return new WaitForSeconds(sequenceTime);
			
			float rotateTime = Time.time + (sequenceTime); 
			Vector3 temp_rotation = transform.eulerAngles;
			temp_rotation.y += 120;
			while(Time.time <= rotateTime)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(temp_rotation), Time.time /sequenceTime);
				yield return null;	
			}
			transform.rotation = Quaternion.Euler(temp_rotation);

			yield return new WaitForSeconds(sequenceTime);	
			
			rotateTime = Time.time + (sequenceTime); 
			temp_rotation.y += 120;
			while(Time.time <= rotateTime)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(temp_rotation), Time.time);
				yield return null;	
			}
			transform.rotation = Quaternion.Euler(temp_rotation);
			
			yield return new WaitForSeconds(sequenceTime);	
			
			rotateTime = Time.time + (sequenceTime); 
			temp_rotation.y += 120;
			while(Time.time <= rotateTime)
			{
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(temp_rotation), Time.deltaTime);
				yield return null;	
			}
			transform.rotation = Quaternion.Euler(temp_rotation);
			
//			enemy.Fov.rotateSpeed = initFovRotSpeed;
			
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

			isReturnDone = true;
			isReturningToInit = false;
		}

		IEnumerator FollowPlayerPoint()
		{
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
				case EnemyActions.Search:
				{
					StopCoroutine("Search");
					isSearching = false;
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
				publicAlert = alertLevel;
			}
		}
		public void CaughtPlayer()
		{
			enemy.Player.GetComponent<EthanController>().isCaught();
		}
		#endregion

	}
	
}