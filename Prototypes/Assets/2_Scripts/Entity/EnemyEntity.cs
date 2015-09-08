namespace TheVandals
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;

	public class EnemyEntity  
	{
		private EnemyType type;
		private EnemyBeHaviour behaviour;
		private State moveState;
		private EnemyState setState;
		private FieldOfView fov;
		private bool playerDetected;
		private bool canRoam;
		private bool canSearch; 
		private bool canFollowPath;

		private EnemyActions action;		
		private NavMeshAgent navAgent;	
		private Transform player;	
		private Animator anim;		
		private AlertLevel alertLVL;
		
		private TargetDestination init;

		private WaypointManager waypointManager;
		private GroundMarker enemyProjector;
		private AreaMask layer;

		private bool isDestinationReached;

		private Vector3 targetPos;
		private Transform distractionObject;

		#region getter/setter
		public EnemyBeHaviour _Behaviour {
			get {
				return this.behaviour;
			}
			set {
				behaviour = value;
			}
		}
		public EnemyType Type { 
			get {
				return this.type;
			}
			set {
				type = value;
			}
		}
		
		public FieldOfView Fov {
			get {
				return this.fov;
			}
			set {
				fov = value;
			}
		}
		
		public State MoveState {
			get {
				return this.moveState;
			}
			set {
				moveState = value;
			}
		}	

		public bool PlayerDetected {
			get {
				return this.playerDetected;
			}
			set {
				playerDetected = value;
			}
		}
		
		public bool CanRoam {
			get {
				return this.canRoam;
			}
			set {
				canRoam = value;
			}
		}
		
		public bool CanSearch {
			get {
				return this.canSearch;
			}
			set {
				canSearch = value;
			}
		} 		
		public bool CanFollowPath {
			get {
				return this.canFollowPath;
			}
			set {
				canFollowPath = value;
			}
		}
		public AreaMask Layer {
			get {
				return this.layer;
			}
			set {
				layer = value;
			}
		}	

		public EnemyActions Action {
			get {
				return this.action;
			}
			set {
				action = value;
			}
		}
		
		public NavMeshAgent NavAgent {
			get {
				return this.navAgent;
			}
			set {
				navAgent = value;
			}
		}
		
		public Transform Player {
			get {
				return this.player;
			}
			set {
				player = value;
			}
		}
		
		public Animator Anim {
			get {
				return this.anim;
			}
			set {
				anim = value;
			}
		}
		
		public TargetDestination Init {
			get {
				return this.init;
			}
			set {
				init = value;
			}
		}

		public WaypointManager WaypointManager {
			get {
				return this.waypointManager;
			}
			set {
				waypointManager = value;
			}
		}
		
		public GroundMarker EnemyProjector {
			get {
				return this.enemyProjector;
			}
			set {
				enemyProjector = value;
			}
		}
		
		public AlertLevel AlertLVL {
			get {
				return this.alertLVL;
			}
			set {
				alertLVL = value;
			}
		}	

		public bool IsDestinationReached {
			get {
				return this.isDestinationReached;
			}
			set {
				isDestinationReached = value;
			}
		}

		public Vector3 TargetPos {
			get {
				return this.targetPos;
			}
			set {
				targetPos = value;
			}
		}

		public void setTargetPos() {
			this.targetPos = player.position;
		}	
		
		public	EnemyState SetState {
			get {
				return this.setState;
			}
			set {
				setState = value;
			}
		}

		public Transform DistractionObject {
			get {
				return this.distractionObject;
			}
			set {
				distractionObject = value;
			}
		}	
		#endregion

		#region Constructor
		public EnemyEntity()
		{
			SwitchType(EnemyType._Busy);
		}

		public EnemyEntity(EnemyType type)
		{
			SwitchType(type);
		}
		#endregion

		#region functions
		public EnemyType SwitchType(EnemyType tp)
		{
			switch (tp)
			{
				case EnemyType._Busy:
				{				
					this.canRoam = false;
					this.canSearch = false;
					this.canFollowPath = false;
					break;
				}
				case EnemyType._Fixe:
				{			
					this.canRoam = false;
					this.canSearch = true;
					this.canFollowPath = false;
					break;
				}
				case EnemyType._RoamingRandom:
				{			
					this.canRoam = true;
					this.canSearch = true;
					this.canFollowPath = false;
					break;
				}
				case EnemyType._RoamingPath:
				{		
					this.canRoam = true;
					this.canSearch = true;
					this.canFollowPath = true;
					break;
				}
			}
			this.type = tp;
			this.setState = EnemyState.Free;		
			this.behaviour = EnemyBeHaviour.Idle;
			this.playerDetected = false;

			return this.type;
		}
		#endregion

		#region Actions
		public void GoToPosition(Vector3 position)
		{
//			moveState = State.Walk;
			navAgent.SetDestination(position);
		}

		public void GoToRandomPoint(Transform tr)
		{				
			Vector3 destination;
			if (GenerateRandomPoint(tr.position, 10.0f, out destination))
			{
				setInit(destination, tr.eulerAngles);
				GoToPosition(destination);
				
				Debug.DrawRay(destination, Vector3.up, Color.blue, 1.0f);
			}
		}

		public void GoToPathPoint(Transform tr)
		{				
			waypointManager.SetNextPoint();
			Vector3 destination = waypointManager.NextPoint.transform.position;

			setInit(destination, tr.eulerAngles);			
			GoToPosition(destination);
		}

		public void setInit(Vector3 position, Vector3 eulerAngles)
		{
			init.position = position;
			init.eulerAngles = eulerAngles;
		}

		public bool GenerateRandomPoint(Vector3 center, float range, out Vector3 result)
		{
			for (int i = 0; i < 30; i++)
			{
				Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
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

		public void enableProjector(bool temp)
		{
			enemyProjector.gameObject.SetActive(temp);
		}

		public void ResetNavAgent(Transform tr)
		{
			navAgent.SetDestination(tr.position);
		}

		public void WatchPlayer(Transform tr)
		{			
			if(fov.canSearch)
			{
				fov.canSearch = false;
				fov.DrawFoV();
			}
			
			fov.isCentered = false;
//			tr.LookAt(targetPos);
			Quaternion rotation = Quaternion.LookRotation(targetPos - tr.position);

			tr.rotation = Quaternion.Slerp(tr.rotation, rotation, Time.deltaTime * 2.0f);

			rotation = Quaternion.LookRotation(targetPos - fov.transform.position);
			fov.transform.rotation = Quaternion.Slerp(fov.transform.rotation, rotation, Time.deltaTime * 2.0f);
		}
		
		public void ProximityWatchPlayer(Transform tr, float elapsed)
		{
			Vector3 rotation = Vector3.zero;	
//			rotation.y = Quaternion.LookRotation(targetPos - tr.position).eulerAngles.y;	
//			
//			if(Mathf.Abs(tr.eulerAngles.y -rotation.y) < 0.1f)
//			{
//				tr.rotation = Quaternion.Euler(rotation);
//				print ("finished");
//			}
//			else
//			{
			rotation.y = Mathf.Lerp(tr.rotation.eulerAngles.y,Quaternion.LookRotation(targetPos - tr.position).y,elapsed / 50.0f);
			tr.rotation = Quaternion.Euler(rotation);
//				tr.rotation = Quaternion.Lerp(tr.rotation,Quaternion.LookRotation(targetPos - tr.position),elapsed / 50.0f);
				//					print (transform.eulerAngles.y + "/" + rotation.y);
				//					if(transform.eulerAngles.y - rotation.y)
				//					{
				//						rotation.y = transform.eulerAngles.y + (Time.deltaTime * 10.0f);
				//						print ("+");
				//					}
				//					else
				//					{
				//						rotation.y = transform.eulerAngles.y - (Time.deltaTime * 10.0f);
				//						print ("-");
				//					}
				//					transform.eulerAngles = rotation;
//			}
		}

		public void ReturnToInitialPosition()
		{
			GoToPosition(init.position);
			fov.canSearch = canSearch;
		}

		public void runTowardPlayer()
		{
			if(fov.canSearch)
			{
				fov.canSearch = false;
				fov.DrawFoV();
			}
			if(!enemyProjector.gameObject.activeSelf)
				enableProjector(true);

			moveState = State.Run;
			enemyProjector.Project(targetPos,Color.red);
			GoToPosition(targetPos);
		}
		#endregion
	}
}
