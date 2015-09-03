namespace TheVandals
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using System.Collections.Generic;

	public class CreateOeuvre : MonoBehaviour 
	{		
		private static CreateOeuvre _instance;
		public static CreateOeuvre instance
		{
			get
			{
				if(_instance == null)
					_instance = GameObject.FindObjectOfType<CreateOeuvre>();
				return _instance;
			}
		}

		[Header("Links To Painting")]
		public Renderer painting_Renderer;
		public RenderTexture rt;	
		public Material brush_Material;
		public GameObject Panel_DrawTexture;
		public Transform cursorTexture;		

		public Image image_Fill;

		[Header("Configuration")]
		[Range(0, 1)]
		public float brushSize = 1.0f;	
		[Range(0, 5)]
		public float cursorTextureSize = 1.0f;
		public int percentage = 15; 

		private Vector2 brush_Position;
		private Vector2? mouseLastPosition = null;
		private Rect brush_Rect;
		private Rect ScreenRect;

		private bool firstFrame = true;
		private bool canPlayerPaint = false;

		private List<Rect> paintingQuads = new List<Rect>();
		private List<Rect> paintingQuadsToRemove = new List<Rect>();
		private int maxQuads = 0;

		private List<Vector2> list_mousePos = new List<Vector2>();

		private enum CursorDirection
		{
			None,
			RightUp,
			LeftUp,
			RightDown,
			LeftDown
		}
		private CursorDirection dirCursor;
		private Vector2? minCursor = null;
		private Vector2 maxCursor = Vector2.zero;	
		private bool isRechargingSpray = false;
		private float shakeDistance;		
		private int shakeIterations = 0;

		#region Unity
		void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			foreach(Rect cb in paintingQuads)
			{
				Gizmos.DrawCube(cb.center, cb.size);
			}
			Gizmos.color = Color.black;
			Gizmos.DrawLine(brush_Rect.min, new Vector2(brush_Rect.xMin,brush_Rect.yMax));
			Gizmos.DrawLine(brush_Rect.min, new Vector2(brush_Rect.xMax,brush_Rect.yMin));
			Gizmos.DrawLine(brush_Rect.max, new Vector2(brush_Rect.xMin,brush_Rect.yMax));
			Gizmos.DrawLine(brush_Rect.max, new Vector2(brush_Rect.xMax,brush_Rect.yMin));
		}

		void OnGUI()
		{
			if(GUI.Button(new Rect(0, 80, 120, 20), "Restart"))
			{				
				GL.Clear(true, true, new Color(1.0f, 1.0f, 1.0f, 1.0f));
				rt.Release();
				GenerateRects();
				
				list_mousePos.Clear();
				mouseLastPosition = null;
				
				painting_Renderer.material.SetFloat("_Alpha", 0);
			}
		}
		
		public void Start()
		{
			if(!painting_Renderer)
			{
				print ("Please set a painting to draw to the gameController");
				gameObject.SetActive(false);
			}
		
			//Get Brush effect boundary area on painting with a rotation zero and position zero
			Vector3 painting_size = Quaternion.AngleAxis(-painting_Renderer.transform.eulerAngles.y, Vector3.up) * painting_Renderer.bounds.size;
			
			ScreenRect.width = Mathf.Abs(painting_size.x);
			ScreenRect.height = Mathf.Abs(painting_size.y);		
			
			ScreenRect.x = -ScreenRect.width / 2;
			ScreenRect.y = -ScreenRect.height / 2;

			brush_Rect.width = brush_Material.mainTexture.width / 100.0f * brushSize;
			brush_Rect.height = brush_Material.mainTexture.height / 100.0f * brushSize;

			rt = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
			GetComponent<Camera>().targetTexture = rt;
			painting_Renderer.material.SetTexture("_MaskTex", rt);

			GenerateRects ();

			cursorTexture.localScale = Vector3.one * cursorTextureSize * brushSize;
			shakeDistance = Mathf.Sqrt((Screen.width * Screen.width) + (Screen.height * Screen.height)) * 5f / 100;

			firstFrame = true;
			canPlayerPaint = false;		
			Panel_DrawTexture.SetActive(false);
		}
		
		public void Update()
		{
			if(canPlayerPaint)
			{
				if (Input.GetMouseButton(0) && image_Fill.fillAmount > 0 && !isRechargingSpray)
				{		
					image_Fill.fillAmount -= Time.deltaTime / 3;

					if(!SoundController.instance.IsPlaying())
						SoundController.instance.PlayClip("sprayClip");

					if(minCursor != null)
					{
						minCursor = null;				
						shakeIterations = 0;
					}

					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.collider.tag == "Painting")					
						GenerateListMousePosition(hit);
				}
				else
				{
					mouseLastPosition = null;

					if(!isRechargingSpray)
						SoundController.instance.StopClip();
					
					if(image_Fill.fillAmount == 0 || 
					   (!Input.GetMouseButton(0) && image_Fill.fillAmount < 1 && CheckShake()))
							StartCoroutine("RecharcheSpray");
				}

				cursorTexture.position = Input.mousePosition;
				cursorTexture.localScale = Vector3.one * cursorTextureSize * brushSize;
			}
		}
		
		public void OnPostRender()
		{
			//clear Graphics buffer 
			if (firstFrame)
			{
				firstFrame = false;
				GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
			}

			if (list_mousePos.Count != 0)
			{
				foreach(Vector2 imageLocalPosition in list_mousePos)
				{
					DrawBrushOnPainting(new Vector2(1024.0f, 614), imageLocalPosition);
				}
				list_mousePos.Clear();
			}
		}
		#endregion

		#region Private 
		#region Functions		
		private void GenerateRects()
		{		
			paintingQuads.Clear();
			
			int widthQuadsCount = 10;
			int heightQuadsCount = 10;
			
			
			float paintingQuadsWidth = ScreenRect.width / widthQuadsCount;
			float paintingQuadsHeight = -ScreenRect.height / heightQuadsCount;
			
			for(int i = 0; i < widthQuadsCount; i++)
			{
				for(int y = 0; y < heightQuadsCount; y++)
				{
					paintingQuads.Add(new Rect((i * paintingQuadsWidth) + ScreenRect.x,
					                           (y * paintingQuadsHeight) - ScreenRect.y,
					                           paintingQuadsWidth,
					                           paintingQuadsHeight));
				}
			}
			maxQuads = paintingQuads.Count;
		}
		
		private void RemoveRects(Vector3 v)
		{
			brush_Rect.center = v;
			
			//Check Rectangle close to mouse position
			foreach(Rect rt in paintingQuads)
				if(brush_Rect.Contains(rt.center))
					paintingQuadsToRemove.Add(rt);
			
			//Remove rectangles from the list
			foreach(Rect tc in paintingQuadsToRemove)
				paintingQuads.Remove(tc);
			
			paintingQuadsToRemove.Clear();
			
			if(paintingQuads.Count * 100 / maxQuads < percentage)
			{			
				StartCoroutine("FadeInPainting");
			}
		}

		private void GenerateListMousePosition(RaycastHit hit)
		{
			// Le screenrect qui est la taille et position du graffiti a la position vector3.zero
			// On va deplacer et faire une rotation du hit point du raycast pour avoir sa position a la position vector0 du screenRect
			Vector2 v = Quaternion.AngleAxis(-hit.transform.eulerAngles.y, Vector3.up) * (hit.point - hit.transform.position);
			
			if (ScreenRect.Contains(v)) 
			{
				//Transpose mouse position from world into pixels
				brush_Position = new Vector2(1024 * (v.x - ScreenRect.xMin) / ScreenRect.width,
				                             614 * (v.y - ScreenRect.yMin) / ScreenRect.height);
				if(!list_mousePos.Contains(brush_Position))
				{
					list_mousePos.Add(brush_Position);				
					RemoveRects(v);
					
					if(mouseLastPosition != null)
					{
						while(Vector2.Distance(mouseLastPosition.Value,brush_Position) > 15)
						{
							Vector2 dir = 10 * new Vector2(brush_Position.x - mouseLastPosition.Value.x, brush_Position.y -mouseLastPosition.Value.y).normalized;
							mouseLastPosition += dir;
							
							if(!list_mousePos.Contains(mouseLastPosition.Value))
							{
								list_mousePos.Add(mouseLastPosition.Value);
								//On recupere le v initial pour le removeRect
								Vector2 temprect = new Vector2(
									(ScreenRect.width * mouseLastPosition.Value.x / 1024) + ScreenRect.xMin,
									(ScreenRect.height * mouseLastPosition.Value.y / 614) + ScreenRect.yMin);
								RemoveRects(temprect);
							}
						}
					}					
					mouseLastPosition = brush_Position;
					return;
				}
			}
		}
		
		private void DrawBrushOnPainting(Vector2 imageSize, Vector2 imageLocalPosition)
		{
			Rect textureRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f); //this will get brush material texture part
			Rect positionRect = new Rect(
				(imageLocalPosition.x - 0.5f * brush_Rect.width * 100) /  imageSize.x,
				(imageLocalPosition.y - 0.5f * brush_Rect.height * 100) / imageSize.y,
				brush_Rect.width * 100 / imageSize.x,
				brush_Rect.height * 100 / imageSize.y
				);  //This will Generate position of the brush according to mouse position and size of brush texture
			
			//Draw Graphics Quad using GL library to render in target render texture of camera to generate effect
			GL.PushMatrix();
			GL.LoadOrtho();
			for (int i = 0; i < brush_Material.passCount; i++)
			{
				brush_Material.SetPass(i);
				GL.Begin(GL.QUADS);
				GL.Color(Color.white);
				GL.TexCoord2(textureRect.xMin, textureRect.yMax);
				GL.Vertex3(positionRect.xMin, positionRect.yMax, 0.0f);
				GL.TexCoord2(textureRect.xMax, textureRect.yMax);
				GL.Vertex3(positionRect.xMax, positionRect.yMax, 0.0f);
				GL.TexCoord2(textureRect.xMax, textureRect.yMin);
				GL.Vertex3(positionRect.xMax, positionRect.yMin, 0.0f);
				GL.TexCoord2(textureRect.xMin, textureRect.yMin);
				GL.Vertex3(positionRect.xMin, positionRect.yMin, 0.0f);
				GL.End();
			}
			GL.PopMatrix();
		}

		private bool CheckShake()
		{
			if(Input.mousePosition.x >= 0 && Input.mousePosition.x <= Screen.width 
			   && Input.mousePosition.y >= 0 && Input.mousePosition.y <= Screen.height)
			{	
				if(minCursor == null)
				{
					minCursor =  Input.mousePosition;	
					return false;
				}
				
				maxCursor = Input.mousePosition;
				Vector2 mouseDir = maxCursor - minCursor.Value;
				
				if(Vector2.Distance(minCursor.Value,maxCursor) > shakeDistance)
				{
					CursorDirection tempDirCursor;
					if(mouseDir.x >= 0)
						tempDirCursor = mouseDir.y >= 0 ? CursorDirection.RightUp : CursorDirection.LeftUp;
					else
						tempDirCursor = mouseDir.y >= 0 ? CursorDirection.RightDown : CursorDirection.LeftDown;
					
					if(dirCursor != tempDirCursor)
					{
						if(dirCursor != CursorDirection.None)						
							shakeIterations ++;

						minCursor =  Input.mousePosition;
						dirCursor = tempDirCursor;
					}
					
					if(shakeIterations == 2)
					{
						shakeIterations = 0;
						return true;
					}
				}
			}
			return false;
		}
		#endregion
		#region IEnumerators
		private IEnumerator FadeInPainting()
		{
			SoundController.instance.StopClip();
			image_Fill.fillAmount = 1;

			canPlayerPaint = false;
			Panel_DrawTexture.SetActive(false);
			Cursor.visible = true;

			float alpha = 0.0f;

			while(alpha != 1)
			{			
				alpha += Time.deltaTime * 0.5f;
				alpha = Mathf.Clamp(alpha,0,1);
				painting_Renderer.material.SetFloat("_Alpha", alpha);
				yield return null;
			}
			GameController.instance.FinishDrawOeuvre();
		}

		private IEnumerator RecharcheSpray()
		{
			isRechargingSpray = true;
			SoundController.instance.PlayClip("rechargeSprayClip");

			float startingTime = Time.time;
			float capacityLeft = image_Fill.fillAmount;
			float capacityTofill = 1 - capacityLeft;
			float rechargeTime = 1.70f * capacityTofill;
			float soundTimer = startingTime + rechargeTime;
			
			while((Time.time <= soundTimer || image_Fill.fillAmount != 1) && !Input.GetMouseButton(0))
			{
				image_Fill.fillAmount =  capacityLeft + (capacityTofill * ((Time.time - startingTime) / rechargeTime));
				yield return null;
			}
//			isPlayingSound = false;
			isRechargingSpray = false;
		}
        #endregion	
		#endregion

		#region Public
		public void StartPainting()
		{
			canPlayerPaint = true;

			Cursor.visible = false;
			cursorTexture.position = Input.mousePosition;
			cursorTexture.localScale = Vector3.one * cursorTextureSize * brushSize;
			Panel_DrawTexture.SetActive(true);
		}

		public void Reset()
		{		
			canPlayerPaint = false;

			GL.Clear(true, true, new Color(1.0f, 1.0f, 1.0f, 1.0f));
			rt.Release();
			GenerateRects();
			
			list_mousePos.Clear();
			mouseLastPosition = null;
			
			painting_Renderer.material.SetFloat("_Alpha", 0);
			image_Fill.fillAmount = 1.0f;
		}	
		#endregion
	}
}