namespace TheVandals
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using System.Collections.Generic;
	using UnityStandardAssets.ImageEffects;

	public class PaintingManager : MonoBehaviour 
	{
		#region Properties
		private static PaintingManager _instance;
		public static PaintingManager instance
		{
			get
			{
				if(_instance == null)
					_instance = GameObject.FindObjectOfType<PaintingManager>();
				return _instance;
			}
		}
		[Header("Configuration")]
		public CaptureOeuvreType captureType = CaptureOeuvreType.TypeRTS;
		public List<Sprite> listPaintingsSprite = new List<Sprite>();

		[Header("Panel PaintInfo")]
		public Text text_Title;
		public Text text_Info;
		public Image paintingToShow;
		[Header("Panel GameInfo")]
		public Text text_CapturedPainting;
		public Text text_GameOver;
		[Header("Panel CaptureOeuvreRTS")]
		public Image iconCapture;
		public Image iconLoading;
		public Image iconBackground;
		[Header("Panel CaptureOeuvreFPS")]
		public Button button_Capture;
		public Image iconCameraViewer;
		[Header("Scene Cameras")]
		public GameObject cameraRTS;
		public GameObject cameraFPS;
		public GameObject cameraUI;
		
		private CaptureOeuvreType captureTypeCurrent;
		private List<PaintingEntity> listPaintingsInJson = new List<PaintingEntity>();
		private string painting;
		private int paintingsInScene = 0;
		private int paintingsCaptured = 0;

		private GameObject panel_GameInfo;
		private GameObject Panel_PaintInfo;		
		private GameObject panel_CaptureOeuvreRTS;
		private GameObject panel_CaptureOeuvreFPS;
		private Blur blur;
		
		private bool isCameraFPS = false;
		#endregion

		#region Unity
		void Awake() 
		{
			ReadJSON rj = new ReadJSON();
			listPaintingsInJson = rj.ReadJson();

			SetPaintingsCountInScene();

			Panel_PaintInfo = text_Info.transform.parent.transform.parent.gameObject;
			panel_CaptureOeuvreRTS = iconCapture.transform.parent.transform.gameObject;
			panel_CaptureOeuvreFPS = button_Capture.transform.parent.transform.gameObject;
			blur = cameraRTS.GetComponent<Blur>();
		}

		void Start()
		{ 
			captureTypeCurrent = captureType;
			SetCaptureType();

			blur.blurSpread = 0;
			blur.enabled = false;		

			RemoveText();
			SetActiveGameOverText(false);
		}

		void Update()
		{
			if(captureTypeCurrent !=captureType)
			{
				captureTypeCurrent = captureType;
				SetCaptureType();
			}
		}

//		void OnGUI () 
//		{
//			if(GUI.Button(new Rect(20,160,100,40), "FPS/RTS Capture")) {
//				if(captureType == CaptureOeuvreType.TypeRTS)
//					captureType = CaptureOeuvreType.TypeFPS;
//				else
//					captureType = CaptureOeuvreType.TypeRTS;
//				SetCaptureType();
//			}
//		}
		#endregion

		#region API
		public void setPainting(string paintingName, Transform paintingTarget)
		{
			this.painting = paintingName;

			paintingToShow.sprite = GetPaintingSprite(paintingName);

			if(captureType == CaptureOeuvreType.TypeRTS)
			{
				panel_CaptureOeuvreRTS.SetActive(true);
				StopCoroutine ("CaptureOeuvreCoroutineRTS");
				StartCoroutine("CaptureOeuvreCoroutineRTS", paintingTarget);
			}
		}
		public void CaptureOeuvre()
		{
			SwitchCamera();
			button_Capture.gameObject.SetActive(false);
			iconCameraViewer.material.color = Color.white;
			iconCameraViewer.enabled = true;
			StopCoroutine ("CaptureOeuvreCoroutineFPS");
			StartCoroutine("CaptureOeuvreCoroutineFPS");
		}

		public void EndCaptureOeuvre()
		{
			paintingToShow.CrossFadeAlpha(0F,2.0f,false);
			iconLoading.fillAmount = 0;	
			blur.blurSpread = 0;
			blur.enabled = false;

			if(captureType == CaptureOeuvreType.TypeFPS)
				SetCaptureType();
			RemoveText();
  		}

		public void RemoveText()
		{
			EthanController.instance.EndCaptureOeuvreMode();
			panel_CaptureOeuvreRTS.SetActive(false);
			Panel_PaintInfo.SetActive(false);
			StopCoroutine ("CaptureOeuvreCoroutineRTS");
			StopCoroutine ("CaptureOeuvreCoroutineFPS");		
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			CameraController.instance.disablePlayingCinematique();
			Time.timeScale = 1.0f;
			cameraUI.GetComponent<ScreenOverlay>().intensity = 0.0f;
			cameraFPS.GetComponent<ScreenOverlay>().intensity = 0.0f;
		}

		public void Reset()
		{
			paintingsCaptured = 0;
			text_CapturedPainting.text = paintingsCaptured + "/" + paintingsInScene;

			foreach(PaintingEntity pe in listPaintingsInJson)
			{
				pe.IsCaptured = false;
			}
			SetActiveGameOverText(false);
		}

		public void SetActiveGameOverText(bool value)
		{
			text_GameOver.enabled = value;
		}
		#endregion

		#region Private
		
		private void SetPaintingsCountInScene()
		{
			foreach (Transform child in transform)
			{
				if(child.gameObject.activeSelf && child.tag == "StreetArt")
					paintingsInScene += 1;
			}
			text_CapturedPainting.text = paintingsCaptured + "/" + paintingsInScene;
		}

		private Sprite GetPaintingSprite(string paintingName)
		{
			return listPaintingsSprite.Find(x => x.name.Equals(paintingName +"_Sprite"));
		}

		private IEnumerator CaptureOeuvreCoroutineRTS(Transform paintingTransform)
		{
			bool captured = false;			
			RaycastHit hit;
			iconLoading.fillAmount = 0;
			Vector3 paintingScreenPosition;
			
			while(!captured)
			{
				paintingScreenPosition = Camera.main.WorldToViewportPoint(paintingTransform.position);
				iconCapture.rectTransform.anchorMax = paintingScreenPosition;
				iconCapture.rectTransform.anchorMin = paintingScreenPosition;
				iconLoading.rectTransform.anchorMax = paintingScreenPosition;
				iconLoading.rectTransform.anchorMin = paintingScreenPosition;
				iconBackground.rectTransform.anchorMax = paintingScreenPosition;
				iconBackground.rectTransform.anchorMin = paintingScreenPosition;
				
				if(Input.GetButton("Fire2"))
				{
					Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
					if(Physics.SphereCast (mouseRay, 1.0f,out hit, Mathf.Infinity, 1<<8 | 1<<9) && hit.collider.tag == "StreetArt")
					{
						iconLoading.fillAmount +=  Time.deltaTime;
						if(iconLoading.fillAmount == 1)
						{
							captured = true;
							ShowText();
						}
					}	
				}
				else
				{
					iconLoading.fillAmount = 0;
				}
				yield return null;
			}			
			EthanController.instance.StartCaptureOeuvreMode();
			panel_CaptureOeuvreRTS.SetActive(false);
			
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			
			yield return CameraController.instance.StartCoroutine("PlayCinematique",paintingTransform);
			
			Time.timeScale = 0;
			
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			
			Panel_PaintInfo.SetActive(true);
			iconLoading.fillAmount = 0;
			
			blur.enabled = true; 
			
			paintingToShow.CrossFadeAlpha(1, 0.5f,false);
			if(blur.blurSpread < 0.5f)
			{
				blur.blurSpread += Time.deltaTime * 0.5f;
			}
		}
		
		private IEnumerator CaptureOeuvreCoroutineFPS()
		{
			FPSCameraController.instance.enableMouseControl(true);
			CameraController.instance.enablePlayingCinematique();
			Transform cam;
			bool captured = false;			
			RaycastHit hit;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			
			while(!captured)
			{
				if(Input.GetKeyDown(KeyCode.Escape))
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					SetCaptureType();
					RemoveText();
					yield break;
				}

				cam =  Camera.main.transform;
				if(Physics.Raycast (cam.position, cam.forward, out hit, Mathf.Infinity, 1<<8 | 1<<9) && hit.collider.tag == "StreetArt")
				{
					iconCameraViewer.CrossFadeAlpha(1f, 0.5f, false);

					if(hit.distance <= 12.0f)
					{
						iconCameraViewer.color = Color.white;

						if(Input.GetButton("Fire2"))
						{
							captured = true;

							string paintingName = hit.collider.GetComponent<MeshRenderer>().material.mainTexture.name;
							GameObject paintingObject = hit.collider.gameObject;
							
							setPainting(paintingName, paintingObject.transform);
							UpdateCapturedPaintings(paintingName, paintingObject);

							FPSCameraController.instance.enableMouseControl(false);
							SoundController.instance.PlayClip("takePicClip");
						}
					}
					else
						iconCameraViewer.color = Color.red;

				}
				else
				{
					iconCameraViewer.color = Color.white;
					iconCameraViewer.CrossFadeAlpha(0.2f, 0.5f, false);
				}
				
				yield return null;
			}
			
			float opacity = 0.1f;
			while(opacity <= 1.0f)
			{
				cameraFPS.GetComponent<ScreenOverlay>().intensity = opacity;
				opacity += Time.deltaTime * 10.0f;
				yield return null;
			}						
			
			cameraFPS.SetActive(false);
			cameraRTS.SetActive(true);
			isCameraFPS = false;
			
			iconCameraViewer.enabled = false;
			panel_CaptureOeuvreFPS.SetActive(false);
			Panel_PaintInfo.SetActive(true);			
			ShowText();
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;			
			blur.enabled = true; 
			
			opacity = 2.0f;
			while(opacity >= 0.0f)
			{
				cameraUI.GetComponent<ScreenOverlay>().intensity = opacity;
				opacity -= Time.deltaTime * 3.0f;
				yield return null;
			}
			cameraUI.GetComponent<ScreenOverlay>().intensity = 0.0f;
			cameraFPS.GetComponent<ScreenOverlay>().intensity = 0.0f;
			//			yield return new WaitForSeconds(1.0f);
			
			Time.timeScale = 0;
			
			
			
			paintingToShow.CrossFadeAlpha(1, 0.5f,false);
			if(blur.blurSpread < 0.5f)
			{
				blur.blurSpread += Time.deltaTime * 0.5f;
			}
		}
		
		private void ShowText()
		{
			PaintingEntity tempP;
			try {
				tempP = new PaintingEntity(listPaintingsInJson.Find(x => x.TextureName.Equals(painting)));
			} catch {
				print("Couldn't find painting Check Json name!");
				return;
			}
			
			text_Title.text = tempP.Title;
			text_Info.text = tempP.Info;
		}
		
		private void SwitchCamera()
		{
			if(!isCameraFPS)
			{
				cameraRTS.SetActive(false);
				cameraFPS.SetActive(true);
				isCameraFPS = true;
				EthanController.instance.StartCaptureOeuvreMode();
			}
			else
			{
				cameraFPS.SetActive(false);
				cameraRTS.SetActive(true);
				isCameraFPS = false;
			}
		}
		
		private void SetCaptureType()
		{
			switch(captureType)
			{
			case CaptureOeuvreType.TypeRTS:
			{
				cameraRTS.SetActive(true);
				cameraFPS.SetActive(false);
				panel_CaptureOeuvreFPS.SetActive(false);
				break;
			}
			case CaptureOeuvreType.TypeFPS:
			{
				cameraRTS.SetActive(true);
				cameraFPS.SetActive(false);
				isCameraFPS = false;
				
				button_Capture.gameObject.SetActive(true);
				iconCameraViewer.enabled = false;
				panel_CaptureOeuvreFPS.SetActive(true);
				break;
			}
			}
		}

		private void UpdateCapturedPaintings(string paintingName, GameObject paintingObject)
		{
			PaintingEntity tempP;
			try {
		
				tempP = listPaintingsInJson.Find(x => x.TextureName.Equals(painting));
			} catch {
				print("Couldn't find painting Check Json name!");
				return;
			}
			if(!tempP.IsCaptured)
			{
				tempP.PaintingObject = paintingObject;
				tempP.IsCaptured = true;
				paintingsCaptured += 1;
				text_CapturedPainting.text = paintingsCaptured + "/" + paintingsInScene;

				if(paintingsCaptured == paintingsInScene)
					GameController.instance.PlayerCapturedAllPaintings();

			}
		}
		#endregion
	}
}
