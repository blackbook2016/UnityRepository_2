namespace TheVandals
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using System.Collections.Generic;
	using UnityStandardAssets.ImageEffects;
	using UnityEngine.EventSystems;
	
	public class PaintingManager_Second : MonoBehaviour 
	{
		#region OldCapture
		#region Properties
		private static PaintingManager_Second _instance;
		public static PaintingManager_Second instance
		{
			get
			{
				if(_instance == null)
					_instance = GameObject.FindObjectOfType<PaintingManager_Second>();
				return _instance;
			}
		}
		
		public List<Sprite> paintingsList = new List<Sprite>();
		public Text text_Title;
		public Text text_Info;
		public Image paintingToShow;
		public Button button_Capture;
		public GameObject Panel_CaptureOeuvre;
		public Image iconLoading;
		
		private List<PaintingEntity> paintings = new List<PaintingEntity>();
		private string painting;
		private GameObject Panel_PaintInfo;
		
		private bool isCameraFPS = false;
		public GameObject cameraRTS;
		public GameObject cameraFPS;
		private Blur blur;
		#endregion
		
		#region Unity
		void Awake() 
		{
			ReadJSON rj = new ReadJSON();
			paintings = rj.ReadJson();
			Panel_PaintInfo = text_Info.transform.parent.transform.parent.gameObject;
			blur = cameraFPS.GetComponent<Blur>();
		}
		void Start()
		{ 
			blur.blurSpread = 0;
			blur.enabled = false;
			isCameraFPS = false;
			RemoveText();
		}
		#endregion
		
		#region API
		public Sprite PaintingSprite(string paintingName)
		{
			return paintingsList.Find(x => x.name.Equals(paintingName +"_Sprite"));
		}
		
		public void setPainting(string paintingName)
		{
			painting = paintingName;
			button_Capture.gameObject.SetActive(true);
		}
		public void ShowText()
		{
			PaintingEntity tempP;
			try {
				tempP = new PaintingEntity(paintings.Find(x => x.TextureName.Equals(painting)));
			} catch {
				print("Couldn't find painting Check Json name! : "+ painting);
				return;
			}
			
			text_Title.text = tempP.Title;
			text_Info.text = tempP.Info;
		}
		public void SwitchCamera()
		{
			if(!isCameraFPS)
			{
				cameraRTS.SetActive(false);
				cameraFPS.SetActive(true);
				isCameraFPS = true;
				Second_EthanController.instance.StartFPSMode();
			}
			else
			{
				cameraFPS.SetActive(false);
				cameraRTS.SetActive(true);
				isCameraFPS = false;
				Second_EthanController.instance.EndFPSMode();
			}
		}
		
		public void CaptureOeuvre()
		{
			button_Capture.gameObject.SetActive(false);
			SwitchCamera();

			Panel_CaptureOeuvre.SetActive(true);	
			StartCoroutine("CaptureOeuvreCoroutine");
		}
		
		IEnumerator CaptureOeuvreCoroutine()
		{
			FPSCameraController.instance.enableMouseControl(true);
			Transform cam;
			bool captured = false;			
			RaycastHit hit;
//			Cursor.visible = false;
			
			while(!captured)
			{
				if(Input.GetButton("Fire2"))
				{
					cam =  Camera.main.transform;
					if(Physics.Raycast (cam.position, cam.forward, out hit, 10.0f, 1<<8 | 1<<9) && hit.collider.tag == "StreetArt")
					{
						//							iconLoading.fillAmount +=  Time.deltaTime;
						//							
						//							if(iconLoading.fillAmount == 1)
						//							{
						captured = true;
						//							}
					}	
				}
				else
				{
					iconLoading.fillAmount = 0;
				}
				yield return null;
			}
//			Cursor.visible = true;
			Panel_CaptureOeuvre.SetActive(false);
			Panel_PaintInfo.SetActive(true);
			
			ShowText();
			iconLoading.fillAmount = 0;
			FPSCameraController.instance.enableMouseControl(false);
			
			blur.enabled = true; 
			
			paintingToShow.CrossFadeAlpha(1, 0.5f,false);
			if(blur.blurSpread < 0.5f)
			{
				blur.blurSpread += Time.deltaTime * 0.5f;
			}
		}
		
		
		public void EndCaptureOeuvre()
		{
			paintingToShow.CrossFadeAlpha(0F,2.0f,false);
			iconLoading.fillAmount = 0;	
			blur.blurSpread = 0;
			blur.enabled = false;
			
			RemoveText();
			SwitchCamera();
		}
		
		
		public void RemoveText()
		{
			button_Capture.gameObject.SetActive(false);
			Panel_PaintInfo.SetActive(false);
			Panel_CaptureOeuvre.SetActive(false);
		}
		#endregion
		#endregion
//		#region Properties
//		private static PaintingManager _instance;
//		public static PaintingManager instance
//		{
//			get
//			{
//				if(_instance == null)
//					_instance = GameObject.FindObjectOfType<PaintingManager>();
//				return _instance;
//			}
//		}
//		
//		public List<Sprite> paintingsList = new List<Sprite>();
//		public Text text_Title;
//		public Text text_Info;
//		public Image paintingToShow;
//		public Image iconCapture;
//		public Image iconLoading;
//		public Image iconBackground;
//		
//		private List<PaintingEntity> paintings = new List<PaintingEntity>();
//		private string painting;
//		private GameObject Panel_PaintInfo;
//		private Transform Panel_CaptureOeuvre;
//		private Blur blur;
//		private Transform paintingTransform;
//		private Vector3 Paner_PaintInfoPosition;
//		#endregion
//		
//		#region Unity
//		void Awake() 
//		{
//			ReadJSON rj = new ReadJSON();
//			paintings = rj.ReadJson();
//			
//			Panel_PaintInfo = text_Info.transform.parent.transform.parent.gameObject;
//			Panel_CaptureOeuvre = iconCapture.transform.parent.transform;
//			blur = Camera.main.GetComponent<Blur>();
//		}
//		void Start()
//		{ 
//			blur.blurSpread = 0;
//			blur.enabled = false;
//			
//			RemoveText();
//		}
//		#endregion
//		
//		#region API
//		public Sprite PaintingSprite(string paintingName)
//		{
//			return paintingsList.Find(x => x.name.Equals(paintingName +"_Sprite"));
//		}
//		
//		public void setPainting(string paintingName, Transform paintingTarget)
//		{
//			this.painting = paintingName;
//			this.paintingTransform = paintingTarget;
//			
//			paintingToShow.sprite = PaintingSprite(paintingName);
//			Panel_CaptureOeuvre.gameObject.SetActive(true);
//			
//			StopCoroutine ("CaptureOeuvreCoroutine");
//			StartCoroutine("CaptureOeuvreCoroutine");
//		}
//		
//		IEnumerator CaptureOeuvreCoroutine()
//		{
//			Transform cam;
//			bool captured = false;			
//			RaycastHit hit;
//			Vector3 normal;
//			iconLoading.fillAmount = 0;
//			Vector3 paintingScreenPosition;
//			
//			while(!captured)
//			{
//				paintingScreenPosition = Camera.main.WorldToViewportPoint(paintingTransform.position);
//				iconCapture.rectTransform.anchorMax = paintingScreenPosition;
//				iconCapture.rectTransform.anchorMin = paintingScreenPosition;
//				iconLoading.rectTransform.anchorMax = paintingScreenPosition;
//				iconLoading.rectTransform.anchorMin = paintingScreenPosition;
//				iconBackground.rectTransform.anchorMax = paintingScreenPosition;
//				iconBackground.rectTransform.anchorMin = paintingScreenPosition;
//				
//				if(Input.GetButton("Fire2"))
//				{
//					Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
//					cam =  Camera.main.transform;
//					if(Physics.SphereCast (mouseRay, 1.0f,out hit, Mathf.Infinity, 1<<8 | 1<<9) && hit.collider.tag == "StreetArt")
//					{
//						iconLoading.fillAmount +=  Time.deltaTime;
//						normal = hit.normal;
//						if(iconLoading.fillAmount == 1)
//						{
//							captured = true;
//							ShowText();
//						}
//					}	
//				}
//				else
//				{
//					iconLoading.fillAmount = 0;
//				}
//				yield return null;
//			}
//			Panel_CaptureOeuvre.gameObject.SetActive(false);
//			yield return CameraController.instance.StartCoroutine("PlayCinematique",paintingTransform);
//			
//			Panel_PaintInfo.SetActive(true);
//			iconLoading.fillAmount = 0;
//			
//			blur.enabled = true; 
//			
//			paintingToShow.CrossFadeAlpha(1, 0.5f,false);
//			if(blur.blurSpread < 0.5f)
//			{
//				blur.blurSpread += Time.deltaTime * 0.5f;
//			}
//		}
//		
//		public void ShowText()
//		{
//			PaintingEntity tempP;
//			try {
//				tempP = new PaintingEntity(paintings.Find(x => x.TextureName.Equals(painting)));
//			} catch {
//				print("Couldn't find painting Check Json name!");
//				return;
//			}
//			
//			text_Title.text = tempP.Title;
//			text_Info.text = tempP.Info;
//		}
//		
//		public void EndCaptureOeuvre()
//		{
//			paintingToShow.CrossFadeAlpha(0F,2.0f,false);
//			iconLoading.fillAmount = 0;	
//			blur.blurSpread = 0;
//			blur.enabled = false;
//			
//			RemoveText();
//		}
//		
//		
//		public void RemoveText()
//		{
//			Panel_CaptureOeuvre.gameObject.SetActive(false);
//			Panel_PaintInfo.SetActive(false);
//			StopCoroutine ("CaptureOeuvreCoroutine");
//			CameraController.instance.disablePlayingCinematique();
//		}
//		#endregion
	}

}
