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
		private GameObject paintingCreationObject;
		[SerializeField]
		private GameObject paintingCreationTrigger;
		
		public bool playcinematique = false;
		#endregion
		
		#region Unity
		void Start()
		{
			Profiler.enabled = false;
			endZone.SetActive(false);
			
			//			paintingCreationTrigger = paintingCreationObject.GetComponentInChildren<Collider>().gameObject;
			
			paintingCreationTrigger.SetActive(true); 
		}
		
		void Update()
		{
			if(playcinematique)
			{
				playcinematique = false;
				StartDrawOeuvre();
			}
		}
		#endregion
		
		#region Handlers
		public void AddEnemyToList(IAController iaController)
		{
			if(!listIA.Contains(iaController))
				listIA.Add(iaController);
		}
		//is called at the end of the playerdeath animator
		public void Reset()
		{
			ResetEnemies();
			
			endZone.SetActive(false);
			CreateOeuvre.instance.Reset();
			paintingCreationTrigger.SetActive(true); 
		}
		
		public void PlayerShouted()
		{
			foreach(IAController tempIA in listIA)
				tempIA.heardSound = true;
		}
		
		public void PlayerCapturedAllPaintings()
		{			
			//			paintingCreationTrigger.SetActive(true); 
		}
		
		public void StartDrawOeuvre()
		{
			StartCoroutine("DrawOeuvreCrtn");
		}
		
		public void FinishDrawOeuvre()
		{
			CameraController.instance.disablePlayingCinematique();
			
			endZone.SetActive(true);
			endZone.transform.position = EthanController.instance.GetPlayerInitPos();
			
			EthanController.instance.Resume();
			
			foreach(IAController tempIA in listIA)
				tempIA.Resume();
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
		
		private IEnumerator DrawOeuvreCrtn()
		{
			paintingCreationTrigger.SetActive(false); 
			
			foreach(IAController tempIA in listIA)
				tempIA.Freeze();
			
			yield return CameraController.instance.StartCoroutine("PlayCinematique", paintingCreationObject.transform);
			
			CreateOeuvre.instance.StartPainting();
		}
		
		private IEnumerator FinishedLevelCrtn()
		{
			PaintingManager.instance.SetActiveGameOverText(true);
			ResetEnemies();
			
			endZone.SetActive(false);
			
			foreach(IAController tempIA in listIA)
				tempIA.Freeze();
			
			yield return new WaitForSeconds(3.0f);
			
			CameraController.instance.Reset();
			PaintingManager.instance.Reset();
			EthanController.instance.reset();			
			CreateOeuvre.instance.Reset();

			paintingCreationTrigger.SetActive(true); 
			
			foreach(IAController tempIA in listIA)
				tempIA.Resume();
		}
		#endregion
		
	}
}
