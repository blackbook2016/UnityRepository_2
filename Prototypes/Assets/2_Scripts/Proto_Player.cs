namespace TheVandals
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityStandardAssets.ImageEffects;
	using System.Collections;

	public class Proto_Player : MonoBehaviour {


		public Transform uiLoading;
		public Image iconLoading;
		public Image paintingToShow;	
		public Blur blur;

		public float rayDistance = 10;
		public float blurSpreadMax = 0.5F;

		public float blurValue;

		private  Transform cam;
		private bool isPaintingShown = false;
		private bool isLoadingPainting = false;

		void Awake()
		{		
			cam = Camera.main.transform;
		}
		void Start () 
		{
			RemovePaintingInfo();
		}

		void Update () 
		{
			if(Input.GetButton("Fire2"))
			{
				RaycastHit hit;
//				if(Physics.Raycast (camPosition.position, cam.forward.normalized, out hit, rayDistance, 1<<19|1<<20))
//					Debug.DrawLine(camPosition.position, hit.point,Color.red);
				if(Physics.Raycast (cam.position, cam.forward, out hit, rayDistance, 1<<19 | 1<<20) && hit.collider.tag == "StreetArt")
				{

					if(!isPaintingShown)
					{
						uiLoading.position = hit.collider.transform.position + (hit.normal * 0.1F) ;
						uiLoading.rotation = hit.collider.transform.rotation;
						uiLoading.gameObject.SetActive(true);

						iconLoading.fillAmount +=  Time.deltaTime;
						isLoadingPainting = true;
					}		
					if(iconLoading.fillAmount == 1 || isPaintingShown)
					{
						isPaintingShown = true;
						ShowPaintingInfo(PaintingManager.instance.PaintingSprite(hit.collider.GetComponent<MeshRenderer>().material.mainTexture.name));
					}
				}	
				else
				{
					if(isPaintingShown || isLoadingPainting)
						RemovePaintingInfo();
				}
			}
			else
			{
				if(isPaintingShown || isLoadingPainting)
					RemovePaintingInfo();
			}
			
		}
		
		void ShowPaintingInfo(Sprite painting)
		{
			blur.enabled = true;
			paintingToShow.sprite = painting;
			paintingToShow.enabled = true;
			paintingToShow.CrossFadeAlpha(1, blurSpreadMax,false);
			if(blur.blurSpread < blurSpreadMax)
			{
				blur.blurSpread += Time.deltaTime * blurSpreadMax;
			}
			PaintingManager.instance.ShowText();
//			PaintingManager.instance
		}

		void RemovePaintingInfo()
		{
			blur.enabled = false;
			paintingToShow.enabled = false;	
			paintingToShow.CrossFadeAlpha(0F,2.0f,false);
			iconLoading.fillAmount = 0;	
			blur.blurSpread = 0;

			uiLoading.gameObject.SetActive(false);

			isLoadingPainting = false;
			isPaintingShown = false;
			PaintingManager.instance.RemoveText();
		}
	}
}
