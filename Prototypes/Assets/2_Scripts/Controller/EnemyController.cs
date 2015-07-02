namespace TheVandals {

	using UnityEngine;
	using System.Collections;

	[RequireComponent (typeof (NavMeshAgent), typeof (Animator))]
	public class EnemyController : MonoBehaviour {


		#region Properties		
		[Header("Configuration")]
		[SerializeField]
		float stopDistance = 1.0F;
		[SerializeField]
		EnemyType enemyType = EnemyType._Busy;
		[SerializeField]
		float range = 10.0f;
		[SerializeField]
		AreaMask layer = AreaMask.All;
		[SerializeField]
		float WaitInSec = 1.0f;		
		[SerializeField]
		float searchingWaitingTime = 3.0f;		
		[SerializeField]
		PointerProjector enemyProjector = null;

		public WaypointManager waypointManager = null;

		private EnemyEntity enemy = new EnemyEntity();	
		private Transform player;		
		private NavMeshAgent navAgent;		
		private Animator anim;

		private float defaultAlpha;
		
		private TargetDestination init;		
		private Vector3 targetPos;		

		private bool returnedToInitialPos = true;
		private bool isSearchWaitting = false;
		private bool isSettingDestination = false;

		#endregion

		#region Unity
		void Start () {
			
			player = GameObject.FindGameObjectWithTag("Player").transform;

			anim = GetComponent<Animator>();
			
			navAgent = GetComponent<NavMeshAgent>();

			defaultAlpha = GetComponentInChildren<MeshRenderer>().material.color.a;		

			if(waypointManager == null)
				print("Pls set waypointManager of the enemy to use Path");
			
			SetEnemy();
			
			InvokeRepeating ("UpdateFoV", 0, 0.01f);
		}

		void OnAnimatorMove ()
		{ 
			if (enemy.MoveState != State.Idle && enemy.MoveState != State.Climb)
			{
				navAgent.velocity = anim.deltaPosition / Time.deltaTime;
				
				if(navAgent.desiredVelocity != Vector3.zero)
				{
					Quaternion lookRotation = Quaternion.LookRotation(navAgent.desiredVelocity);
					transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, navAgent.angularSpeed * Time.deltaTime);
				}
			}		
		}
		#endregion

		#region Private
		void UpdateFoV () {
			Vector3 enemyPosOnNavMesh = transform.position;
			enemyPosOnNavMesh.y = navAgent.destination.y;

			if(enemyType != enemy.Type)
				SetEnemy();

			DetectPlayer();

			UpdateFoVColor();

			if(enemy._Behaviour != EnemyBeHaviour.Idle) 
			{				
				switch(enemy._Behaviour)
				{
				case EnemyBeHaviour.Curious:
				{
					if(Vector3.Distance(enemyPosOnNavMesh,navAgent.destination) <= 0.1F)
					{
						if(!isSearchWaitting)
						{
							if(enemyProjector)
							{
								enemyProjector.gameObject.SetActive(false);
							}
							
							enemy.MoveState = State.Idle;

							StopCoroutine("SearchingWait");
							StartCoroutine("SearchingWait",searchingWaitingTime);
						}
					}
					else
					{
						enemy.MoveState = State.Walk;
						enemy.Fov.canSearch = false;
					}
					break;
				}
				case EnemyBeHaviour.Alert:
				{
					enemy.MoveState = State.Run;
					enemy.Fov.canSearch = false;
					break;
				}
				case EnemyBeHaviour.Searching:
				{		
					if(Vector3.Distance(enemyPosOnNavMesh,navAgent.destination) <= 0.1F)
					{
						if(!isSearchWaitting)
						{
							if(enemyProjector)
							{
								enemyProjector.gameObject.SetActive(false);
							}

							enemy.MoveState = State.Idle;

							StopCoroutine("SearchingWait");
							StartCoroutine("SearchingWait",searchingWaitingTime);
						}
					}
					else
						enemy.MoveState = State.Walk;
					break;
				}
				}
				
				
				if((player.position - transform.position).magnitude <= stopDistance)
				{
					if(enemyProjector)
					{
						enemyProjector.gameObject.SetActive(false);
					}
					player.GetComponent<EthanController>().isCaught();
					navAgent.SetDestination(transform.position);
					enemy._Behaviour = EnemyBeHaviour.Idle;
				}
				returnedToInitialPos = false;				
			}
			else
			{				
				if(!returnedToInitialPos)
				{
					navAgent.SetDestination(init.position);
					returnedToInitialPos = true;
					enemy.Fov.canSearch = enemy.CanSearch;
				}
				else if(Vector3.Distance(enemyPosOnNavMesh,navAgent.destination) <= 0.1F)
				{
					if(enemy.Type == EnemyType._Fixe || enemy.Type == EnemyType._Busy)
					{
						transform.rotation = Quaternion.Euler(init.eulerAngles);
					}
					else
					if(isSettingDestination == false)
					{
						StopCoroutine("SetDestination");
						StartCoroutine("SetDestination",WaitInSec);
					}
				}

				if(Vector3.Distance(enemyPosOnNavMesh,navAgent.destination) <= 0.1F)
				{
					enemy.MoveState = State.Idle;
				}
				else
					enemy.MoveState = State.Walk;
			}						
			anim.SetInteger("MoveState",(int)enemy.MoveState);
		}

		IEnumerator SetDestination(int sec)
		{
			isSettingDestination = true;

			yield return new WaitForSeconds(sec);

			if(enemy.Type == EnemyType._RoamingRandom  && navAgent.desiredVelocity.magnitude == 0)
			{
				Vector3 point;
				if (RandomPoint(transform.position, range, out point))
				{
					init.position = point;
					init.eulerAngles = transform.eulerAngles;
					navAgent.SetDestination(point);
					Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
				}
			}
			
			if(enemy.Type == EnemyType._RoamingPath && Vector3.Distance(transform.position,navAgent.destination) <= 0.1F && waypointManager != null)
			{
				waypointManager.SetNextPoint();
				init.position = waypointManager.NextPoint.transform.position;
				init.eulerAngles = transform.eulerAngles;
				navAgent.SetDestination(init.position);
			}
			isSettingDestination = false;
		}

		IEnumerator SearchingWait(int sec)
		{
			isSearchWaitting = true;
			enemy.Fov.canSearch = true;

			yield return new WaitForSeconds(sec);

			enemy.Fov.canSearch = enemy.CanSearch;
			isSearchWaitting = false;

			enemy._Behaviour = EnemyBeHaviour.Idle;
		}

		void UpdateFoVColor() {

			switch(enemy._Behaviour)
			{
			case EnemyBeHaviour.Idle:
			{
				enemy.Fov.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0, defaultAlpha); // Color Green
				break;
			}
			case EnemyBeHaviour.Curious:
			{
				enemy.Fov.GetComponent<MeshRenderer>().material.color = new Color(1, 0.5f, 0, defaultAlpha); // Color Orange
				break;
			}
			case EnemyBeHaviour.Alert:
			{
				enemy.Fov.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, defaultAlpha); // Color RED
				break;
			}
			case EnemyBeHaviour.Searching:
			{
				enemy.Fov.GetComponent<MeshRenderer>().material.color = new Color(0, 0, 1, defaultAlpha); // Color Blue
				break;
			}
			}			
		}
		
		void DetectPlayer() {
			
			if(enemy.Fov.GetDetectedObjects().Contains(player))
			{								
				enemy.PlayerDetected = true;

				StopAllCoroutines();
				isSettingDestination = false;
				isSearchWaitting = false;

				targetPos = player.position;				
				navAgent.SetDestination(targetPos);

				if(enemyProjector)
				{
					enemyProjector.gameObject.SetActive(true);
					enemyProjector.Project(targetPos,Color.red);
				}
				else
					print ("Pointer Projector not defined");

				float distance = Vector3.Distance(player.position, transform.position);

				if(distance >= enemy.Fov.fovDepth * 2 / 3 && enemy._Behaviour != EnemyBeHaviour.Alert)
				{
					if(enemy._Behaviour == EnemyBeHaviour.Searching)
						enemy._Behaviour = EnemyBeHaviour.Alert;
					else
						enemy._Behaviour = EnemyBeHaviour.Curious;
				}
				else
				{
					enemy._Behaviour = EnemyBeHaviour.Alert;
				}
			}
			else 
			{				
				enemy.PlayerDetected = false;	

				if(enemy._Behaviour == EnemyBeHaviour.Alert)
					enemy._Behaviour = EnemyBeHaviour.Searching;
			}			
		}

		void SetEnemy()
		{
			if(enemyProjector)
			{
				enemyProjector.gameObject.SetActive(false);
			}
			enemy.SwitchType(enemyType);
			enemy.Fov = GetComponentInChildren<FieldOfView>();
			enemy.Fov.canSearch = enemy.CanSearch;	
			
			init.position = transform.position;
			init.eulerAngles = transform.eulerAngles;

			if(enemy.Type == EnemyType._RoamingRandom)
			{
				Vector3 point;
				if (RandomPoint(transform.position, range, out point))
				{
					navAgent.SetDestination(point);
					Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
					init.position = point;
					init.eulerAngles = transform.eulerAngles;
				}
			}
			if(enemy.Type == EnemyType._RoamingPath && waypointManager != null)
			{
				init.position = waypointManager.NextPoint.transform.position;
				navAgent.SetDestination(init.position);
				init.eulerAngles = transform.eulerAngles;
			}
		}

		bool RandomPoint(Vector3 center, float range, out Vector3 result)
		{
			for (int i = 0; i < 30; i++)
			{
				Vector3 randomPoint = center + Random.insideUnitSphere * range;
				NavMeshHit hit;

				if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, (int)layer)) 
				{
					result = hit.position;
					return true;
				}
			}

			result = Vector3.zero;
			return false;
		}
		#endregion
		
	}
	
}