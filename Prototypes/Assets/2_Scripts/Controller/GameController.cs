namespace TheVandals {
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class GameController : MonoBehaviour 
	{
		#region Properties
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

		[SerializeField]
		private List<IAController> listIA = new List<IAController>();

		[SerializeField]
		private GameObject endZone;
		[SerializeField]
		private GameObject PaintingCreationZone;
		#endregion

		#region Unity
		void Start()
		{
			Profiler.enabled = false;
			endZone.SetActive(false);
			PaintingCreationZone.SetActive(false);
		}
		#endregion

		#region Handlers
		public void AddEnemyToList(IAController iaController)
		{
			if(!listIA.Contains(iaController))
				listIA.Add(iaController);
		}

		public void Reset()
		{
			ResetEnemies();

			endZone.SetActive(false);
			PaintingCreationZone.SetActive(false);
		}

		public void PlayerShouted()
		{
			foreach(IAController tempIA in listIA)
				tempIA.heardSound = true;
		}

		public void PlayerCapturedAllPaintings()
		{			
			PaintingCreationZone.SetActive(true);
		}

		public void CreateOeuvre()
		{
			StartCoroutine("CreateOeuvreCrtn");
		}

		public void FinishLevel()
		{
			StartCoroutine("FinishedLevelCrtn");
		}
		#endregion

		#region Private
		private void ResetEnemies()
		{
			foreach(IAController tempIA in listIA)
				tempIA.isReset = true;
		}

		private IEnumerator CreateOeuvreCrtn()
		{
			PaintingCreationZone.SetActive(false);

			foreach(IAController tempIA in listIA)
				tempIA.Freeze();

			yield return CameraController.instance.StartCoroutine("PlayCinematique", EthanController.instance.transform);

			yield return new WaitForSeconds(3.0f);

			CameraController.instance.disablePlayingCinematique();

			endZone.SetActive(true);
			endZone.transform.position = EthanController.instance.GetPlayerInitPos();

			EthanController.instance.Resume();

			foreach(IAController tempIA in listIA)
				tempIA.Resume();
		}

		private IEnumerator FinishedLevelCrtn()
		{
			PaintingManager.instance.SetActiveGameOverText(true);
			Reset();
			foreach(IAController tempIA in listIA)
				tempIA.Freeze();

			yield return new WaitForSeconds(3.0f);
			CameraController.instance.Reset();
			PaintingManager.instance.Reset();
			EthanController.instance.reset();
			foreach(IAController tempIA in listIA)
				tempIA.Resume();
		}
		#endregion

	}
}
