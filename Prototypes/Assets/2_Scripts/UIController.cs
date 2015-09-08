namespace TheVandals
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using System.Collections.Generic;
	using UnityStandardAssets.ImageEffects;

	public class UIController : Singleton<UIController>
	{
		#region properties
		[Header("Panel PaintInfo")]
		public Text text_Title;
		public Text text_Info;
		public Image paintingToShow;
		[Header("Panel GameInfo")]
		public Text text_CapturedPainting;
		public Image text_GameOver;
		public Image image_CursorThrow;
		public Button button_Throw;
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


		[SerializeField]
		private bool isThrowingRock = false;
		public bool IsThrowingRock {
			get {
				return this.isThrowingRock;
			}
			set {
				isThrowingRock = value;
				button_Throw.gameObject.SetActive(!value);
				image_CursorThrow.enabled = value;
			}
		}
		#endregion

		void Start()
		{
		}

		void Update()
		{
			if(isThrowingRock)
				UI_ThrowRock();
		}

		private void UI_ThrowRock()
		{			
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(mouseRay, out hit,Mathf.Infinity) && hit.collider.tag == "Floor")
			{
				Vector3 p = hit.point;
				p.y = image_CursorThrow.rectTransform.rect.height / 2;
				image_CursorThrow.transform.position = p;
				image_CursorThrow.transform.LookAt(Camera.main.transform.position);

				if(!image_CursorThrow.enabled)
					image_CursorThrow.enabled = true;
			}
			else
				image_CursorThrow.enabled = false;
		}
	}
}
