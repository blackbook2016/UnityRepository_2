namespace TheVandals {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class GameController : MonoBehaviour 
	{
		private static GameController _instance;
		public static GameController instance
		{
			get
			{
				if(_instance == null)
					_instance = GameObject.FindObjectOfType<GameController>();
				return _instance;
			}
		}

		public  List<IAController> listIA = new List<IAController>();

		void Awake()
		{
			if (listIA.Count == 0)
			{
				GameObject[] ia = GameObject.FindGameObjectsWithTag("Enemy");
				foreach(GameObject tempIA in ia)
					listIA.Add(tempIA.GetComponent<IAController>());
			}
		}

		public void ResetEnemies()
		{
			foreach(IAController tempIA in listIA)
			{
				print(tempIA.gameObject);
				tempIA.isReset = true;
			}
		}
	}
}
