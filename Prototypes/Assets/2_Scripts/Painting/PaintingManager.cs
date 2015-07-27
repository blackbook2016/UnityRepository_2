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

		public List<Sprite> paintingsList = new List<Sprite>();
		public Text text_Title;
		public Text text_Info;
		public Image paintingToShow;
		public Image iconCapture;
		public Image iconLoading;
		public Image iconBackground;
		public Button button_Capture;
		public Image iconCameraViewer;
		public CaptureOeuvreType captureType = CaptureOeuvreType.TypeRTS;

		private CaptureOeuvreType captureTypeCurrent;
		private List<PaintingEntity> paintings = new List<PaintingEntity>();
		private string painting;
		private GameObject Panel_PaintInfo;
		private Transform Panel_CaptureOeuvreRTS;
		private Transform Panel_CaptureOeuvreFPS;
		private Blur blur;
		private Blur blurFPS;
		private Transform paintingTransform;
		private Vector3 Paner_PaintInfoPosition;

		private bool isCameraFPS = false;
		public GameObject cameraRTS;
		public GameObject cameraFPS;
		public GameObject cameraUI;
		#endregion

		#region Unity
		void Awake() 
		{
			ReadJSON rj = new ReadJSON();
			paintings = rj.ReadJson();

			Panel_PaintInfo = text_Info.transform.parent.transform.parent.gameObject;
			Panel_CaptureOeuvreRTS = iconCapture.transform.parent.transform;
			Panel_CaptureOeuvreFPS = button_Capture.transform.parent.transform;
			blur = cameraRTS.GetComponent<Blur>();
			blurFPS = cameraFPS.GetComponent<Blur>();
		}
		void Start()
		{ 
			captureTypeCurrent = captureType;
			SetCaptureType();

			blur.blurSpread = 0;
			blur.enabled = false;	

			blurFPS.blurSpread = 0;
			blurFPS.enabled = false;	

			RemoveText();
		}
		void Update()
		{
			if(captureTypeCurrent !=captureType)
			{
				captureTypeCurrent = captureType;
				SetCaptureType();
			}
		}
		void OnGUI () 
		{
			if(GUI.Button(new Rect(20,160,100,40), "FPS/RTS Capture")) {
				if(captureType == CaptureOeuvreType.TypeRTS)
					captureType = CaptureOeuvreType.TypeFPS;
				else
					captureType = CaptureOeuvreType.TypeRTS;
				SetCaptureType();
			}
		}
		#endregion

		#region API
		public Sprite PaintingSprite(string paintingName)
		{
			return paintingsList.Find(x => x.name.Equals(paintingName +"_Sprite"));
		}

		public void setPainting(string paintingName, Transform paintingTarget)
		{
			this.painting = paintingName;
			this.paintingTransform = paintingTarget;

			paintingToShow.sprite = PaintingSprite(paintingName);

			if(captureType == CaptureOeuvreType.TypeRTS)
			{
				Panel_CaptureOeuvreRTS.gameObject.SetActive(true);
				StopCoroutine ("CaptureOeuvreCoroutineRTS");
				StartCoroutine("CaptureOeuvreCoroutineRTS");
			}
		}
		public void CaptureOeuvre()
		{
			SwitchCamera();
			button_Capture.gameObject.SetActive(false);
			iconCameraViewer.enabled = true;
			StopCoroutine ("CaptureOeuvreCoroutineFPS");
			StartCoroutine("CaptureOeuvreCoroutineFPS");
		}

		IEnumerator CaptureOeuvreCoroutineRTS()
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
			Panel_CaptureOeuvreRTS.gameObject.SetActive(false);

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

		IEnumerator CaptureOeuvreCoroutineFPS()
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
				if(Input.GetButton("Fire2"))
				{
					cam =  Camera.main.transform;
					if(Physics.Raycast (cam.position, cam.forward, out hit, Mathf.Infinity, 1<<8 | 1<<9) && hit.collider.tag == "StreetArt")
					{
						captured = true;
						setPainting(hit.collider.GetComponent<MeshRenderer>().material.mainTexture.name, hit.collider.transform.parent.transform);
						FPSCameraController.instance.enableMouseControl(false);
						SoundController.instance.PlayClip("takePicClip");
					}	
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
			Panel_CaptureOeuvreFPS.gameObject.SetActive(false);
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

		public void ShowText()
		{
			PaintingEntity tempP;
			try {
				tempP = new PaintingEntity(paintings.Find(x => x.TextureName.Equals(painting)));
			} catch {
				print("Couldn't find painting Check Json name!");
				return;
			}

			text_Title.text = tempP.Title;
			text_Info.text = tempP.Info;
		}

		public void EndCaptureOeuvre()
		{
			paintingToShow.CrossFadeAlpha(0F,2.0f,false);
			iconLoading.fillAmount = 0;	
			blur.blurSpread = 0;
			blur.enabled = false;

			blurFPS.blurSpread = 0;
			blurFPS.enabled = false;

			if(captureType == CaptureOeuvreType.TypeFPS)
				SetCaptureType();
			RemoveText();
		}

		public void SwitchCamera()
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
//				EthanController.instance.EndFPSMode();
			}
		}
		
		public void RemoveText()
		{
			EthanController.instance.EndCaptureOeuvreMode();
			Panel_CaptureOeuvreRTS.gameObject.SetActive(false);
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

		void SetCaptureType()
		{
			switch(captureType)
			{
			case CaptureOeuvreType.TypeRTS:
			{
				cameraRTS.SetActive(true);
				cameraFPS.SetActive(false);
				Panel_CaptureOeuvreFPS.gameObject.SetActive(false);
				break;
			}
			case CaptureOeuvreType.TypeFPS:
			{
				cameraRTS.SetActive(true);
				cameraFPS.SetActive(false);
				isCameraFPS = false;

				button_Capture.gameObject.SetActive(true);
				iconCameraViewer.enabled = false;
				Panel_CaptureOeuvreFPS.gameObject.SetActive(true);
				break;
			}
			}
		}
		#endregion
	}
}
