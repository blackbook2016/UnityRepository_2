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
		
		public WaypointManager waypointManager = null;

		private EnemyEntity enemy = new EnemyEntity();	
		private Transform player;		
		private NavMeshAgent navAgent;		
		private Animator anim;

		private float defaultAlpha;
		
		private Vector3 initPos;		
		private Vector3 targetPos;		
		

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

			if(enemyType != enemy.Type)
				SetEnemy();

			DetectPlayer();

			UpdateFoVColor();

			if(enemy.PlayerDetected) 
			{				
				enemy.MoveState = State.Run;
				targetPos = player.position;
				navAgent.SetDestination(targetPos);

				if((targetPos - transform.position).magnitude <= stopDistance)
				{
					player.GetComponent<EthanController>().isCaught();
					navAgent.SetDestination(transform.position);
				}				
			}
			else
			{				
				if(navAgent.desiredVelocity.magnitude != 0)
					enemy.MoveState = State.Walk;
				else
					enemy.MoveState = State.Idle;
			}
//			else 
//				if(navAgent.remainingDistance <= stopDistance) 
//			{
//				
//				navAgent.SetDestination(initPos);
//				
//			}

			if(enemy.Type == EnemyType._RoamingRandom  && navAgent.desiredVelocity.magnitude == 0)
			{
				Vector3 point;
				if (RandomPoint(transform.position, range, out point))
				{
					navAgent.SetDestination(point);
					Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
				}
			}

			if(enemy.Type == EnemyType._RoamingPath && Vector3.Distance(transform.position,navAgent.destination) <= 0.1F && waypointManager != null)
			{
				waypointManager.SetNextPoint();

				navAgent.SetDestination(waypointManager.NextPoint.transform.position);
			}
						
			anim.SetInteger("MoveState",(int)enemy.MoveState);
		}

		
		void UpdateFoVColor() {
			
			if(enemy.PlayerDetected) 
				enemy.Fov.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0, defaultAlpha); // Color RED
			else 
				enemy.Fov.GetComponent<MeshRenderer>().material.color = new Color(0, 1, 0, defaultAlpha); // Color Green
			
		}

		void DetectPlayer() {
			
			if(enemy.Fov.GetDetectedObjects().Contains(player)) {
				
				enemy.PlayerDetected = true;
				targetPos = player.position;
				
			} else {
				
				enemy.PlayerDetected = false;
				
			}
			
		}

		void SetEnemy()
		{
			enemy.SwitchType(enemyType);
			enemy.Fov = GetComponentInChildren<FieldOfView>();
			enemy.Fov.canSearch = enemy.CanSearch;	
			
			initPos = transform.position;

			if(enemy.Type == EnemyType._RoamingRandom)
			{
				Vector3 point;
				if (RandomPoint(transform.position, range, out point))
				{
					navAgent.SetDestination(point);
					Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
				}
			}
			if(enemy.Type == EnemyType._RoamingPath && waypointManager != null)
			{
				navAgent.SetDestination(waypointManager.NextPoint.transform.position);
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