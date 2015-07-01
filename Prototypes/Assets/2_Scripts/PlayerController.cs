namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	[ExecuteInEditMode]
	public class PlayerController : MonoBehaviour {
		
		#region Properties
		[Header("Configuration")]
		[SerializeField]
		float walkSpeed = 3;
		[SerializeField]
		float runSpeed = 15;
		[Tooltip("Double Click Delays")]
		float delay = 0.25F;
		[SerializeField]
		State state = State.Idle;

		private float lastClickTimeL = 0F;
		private float lastClickTimeR = 0F;
		//private float pathDistance = 0F;
		private NavMeshAgent agent;
		#endregion

		#region Unity
		void Start () 
		{
			agent = GetComponent<NavMeshAgent>();
		}	

		void Update () 
		{
			if(state != State.Idle && agent.remainingDistance == 0)
				state = State.Idle;

			if(Input.GetKeyDown(KeyCode.Mouse0))	
				MovePlayer();

			if(Input.GetKeyDown(KeyCode.Mouse1))
				lastClickTimeR = Time.time;

			if(Input.GetKeyUp(KeyCode.Mouse1) && Time.time < lastClickTimeR + delay)
				StopPlayer();



			if (agent.hasPath)
			{		
				if(state == State.Idle)
					state = State.Walk;
				print ("walk 1");
				//agent.updateRotation = false;
				//set the rotation in the direction of movement
				//transform.rotation = Quaternion.LookRotation(agent.desiredVelocity);
				//set the navAgent's velocity to the velocity of the animation clip currently playing
				//print(agent.desiredVelocity);

				////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				agent.velocity = agent.desiredVelocity;
				if(agent.remainingDistance < 3)
					agent.velocity = Vector3.Normalize(agent.desiredVelocity) * agent.speed * agent.remainingDistance / 3;
				////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//				if(double.IsNaN(pathDistance) || double.IsInfinity(pathDistance) || pathDistance <= 0.5F)
//				{
//					if(!double.IsNaN(agent.remainingDistance) && !double.IsInfinity(agent.remainingDistance))
//					{
//						pathDistance = agent.remainingDistance;
//						agent.velocity = Vector3.Normalize(agent.desiredVelocity) * agent.speed * agent.remainingDistance / pathDistance;
//					}
//				}else
//					if(!double.IsNaN(agent.remainingDistance) && !double.IsInfinity(agent.remainingDistance))
//					{
//						print(agent.remainingDistance + "  " + pathDistance);
//						agent.velocity = Vector3.Normalize(agent.desiredVelocity) * agent.speed * agent.remainingDistance / pathDistance;
//					}
			}
		}
		#endregion

		#region Private

		void MovePlayer()
		{
			agent.SetDestination(RetrieveMousePosition());
			//agent.Resume();

			if(state != State.Run && Time.time - lastClickTimeL < delay)
			{
				//agent.velocity = Vector3.forward * runSpeed;
				state = State.Run;
				agent.speed = runSpeed;
			}
			else if(state != State.Walk && Time.time - lastClickTimeL >= delay)
			{
				//agent.velocity = Vector3.forward * walkSpeed;
				state = State.Walk;
				agent.speed = walkSpeed;
			}
			lastClickTimeL = Time.time;
		}

		void StopPlayer()
		{
			agent.SetDestination(transform.position);
			state = State.Idle;
		}
		
		Vector3 RetrieveMousePosition()
		{			
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(mouseRay, out hit,Mathf.Infinity,1<<8))
			{
				PointerProjector.Instance.Project(hit.point,Color.white);
				return hit.point;
			}
			return transform.position;
		}
		#endregion

		#region Editor API
//		void OnGUI()
//		{
//			int speed = (int)agent.speed;
//			#if UNITY_EDITOR
//			GUI.Label(new Rect(25, 5, 100, 100),"PlayerSpeed: " + speed.ToString());
//			agent.speed = GUI.HorizontalSlider(new Rect(25, 25, 100, 30), agent.speed, 0.0F, 10.0F);
//
//			#endif
//		}
		#endregion 
	}
}
