namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	[RequireComponent (typeof (IAController))]
	public class StateMachine : MonoBehaviour 
	{
		#region Properties
		private EnemyEntity enemy;
		#endregion

		#region Unity
		void Start()
		{
			enemy = GetComponent<IAController>().ReturnEnemy();
		}
		#endregion


	}
}
