using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CameraMasking : MonoBehaviour
{
	#region Properties
	public GameObject Dust;
	private Rect ScreenRect;
	private float dustDepth = 0.0f;

	public RenderTexture rt;

	public Material EraserMaterial;
	
	private bool firstFrame;
	private Vector2? newHolePosition;
	
	[Range(0, 1)]
	public float brushSize = 1.0f;

	[Range(0, 5)]
	public float cursorTextureSize = 1.0f;
	private Rect brushRect;

	public Transform cursorTexture;

	public int percentage = 15; 

	private int maxQuads = 0; 
	
	private List<Rect> paintingQuads = new List<Rect>();
	private List<Rect> paintingQuadsToRemove = new List<Rect>();

	private bool isFadeInPainting = false;
	private bool isPlayingSound = false;

	private AudioSource audioSource;
	[Tooltip("Audio Clip SprayingSound")]
	public AudioClip ac_Spraying;

	[Tooltip("Audio Clip SprayRechargeSound")]
	public AudioClip ac_SprayRecharge;

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

	public Image image_SprayCapacity;

	private Vector2? mouseLastPosition = null;
	private float mouseLastDrawClick;
	private bool playerClicked = false;

	private List<Rect> list_mousePos = new List<Rect>();
	private List<Rect> list_mousePosGizmos = new List<Rect>();

	private bool canDrawPercGiz = false;
	private bool canDrawMousePathGiz = false;
	private bool canDrawMouseShakeGiz = false;

	private List<Vector2> list_ShakePoints = new List<Vector2>();

	#endregion

	#region Unity
	void OnDrawGizmos()
	{
		if(canDrawPercGiz)
		{
			Gizmos.color = Color.red;
			foreach(Rect cb in paintingQuads)
			{
				Gizmos.DrawCube(cb.center, cb.size);
			}
		}

		if(canDrawMousePathGiz)
		{
			Rect lastPoint = new Rect();
			int i = 0;
			Gizmos.color = Color.black;

			foreach(Rect point in list_mousePosGizmos)
			{
				if(i!=0)
					Gizmos.DrawLine(lastPoint.min,point.min);
	//			Gizmos.DrawSphere(new Vector3 (point.x, point.y, 0), 0.1f);
				i++;
				lastPoint = point;
			}
			
			Gizmos.DrawLine(brushRect.min, new Vector2(brushRect.xMin,brushRect.yMax));
			Gizmos.DrawLine(brushRect.min, new Vector2(brushRect.xMax,brushRect.yMin));
			Gizmos.DrawLine(brushRect.max, new Vector2(brushRect.xMin,brushRect.yMax));
			Gizmos.DrawLine(brushRect.max, new Vector2(brushRect.xMax,brushRect.yMin));
		}

		if(canDrawMouseShakeGiz)
		{
			Vector2? lastPointShake = null;
			Gizmos.color = Color.black;
			
			foreach(Vector2 point in list_ShakePoints)
			{
				if(lastPointShake != null)
					Gizmos.DrawLine(lastPointShake.Value,point);

				Gizmos.DrawSphere(new Vector3 (point.x, point.y, 0), 0.1f);
				lastPointShake = point;
			}
		}
	}

	void OnGUI()
	{
		GUI.Label(new Rect(0, 0, 160, 20),"PaintPercentage: " + ((maxQuads - paintingQuads.Count) * 100 /maxQuads)+"%");
		GUI.Label(new Rect(0, 40, 120, 20),"ShakeIterations: " + shakeIterations + "/4");

//		if(isFadeInPainting)
//		{
			if(GUI.Button(new Rect(0, 80, 120, 20), "Restart"))
			{
				StopAllCoroutines();
				StartCoroutine("ResetPainting");
			}
//		}

		if(Application.isPlaying && !isFadeInPainting && Input.GetMouseButton(0) && image_SprayCapacity.fillAmount > 0)
		{			
			Event e = Event.current;
			GenerateListMousePosition(e.mousePosition);
		}
//#if UNITY_EDITOR
		if(GUI.Toggle(new Rect(0, 120, 200, 20),false, "Show Percentage Gizmos"))
			canDrawPercGiz = !canDrawPercGiz;
		
		if(GUI.Toggle(new Rect(0, 160, 200, 20),false, "Show Mouse Draw Points Gizmos"))
			canDrawMousePathGiz = !canDrawMousePathGiz;

		if(GUI.Toggle(new Rect(0, 200, 200, 20),false, "Show Mouse Shake Points Gizmos"))
			canDrawMouseShakeGiz = !canDrawMouseShakeGiz;
//#endif
	}
	
	public void Start()
	{
//		Cursor.visible = false;
		firstFrame = true;

		//Get Erase effect boundary area
		ScreenRect.x = Dust.GetComponent<Renderer>().bounds.min.x;
		ScreenRect.y = Dust.GetComponent<Renderer>().bounds.min.y;
		ScreenRect.width = Dust.GetComponent<Renderer>().bounds.size.x;
		ScreenRect.height = Dust.GetComponent<Renderer>().bounds.size.y;

		dustDepth = Dust.transform.position.z - transform.position.z;

		brushRect.width = EraserMaterial.mainTexture.width / 100.0f * brushSize;
		brushRect.height = EraserMaterial.mainTexture.height / 100.0f * brushSize;

		rt = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);

		GetComponent<Camera>().targetTexture = rt;

		Dust.GetComponent<Renderer>().material.SetTexture("_MaskTex", rt);
		
		GenerateRects ();

		audioSource = GetComponent<AudioSource>();

		dirCursor = CursorDirection.None;

		shakeDistance = Mathf.Sqrt((Screen.width * Screen.width) + (Screen.height * Screen.height)) * 5f / 100;
		
		cursorTexture.localScale = Vector3.one * cursorTextureSize * brushSize;
	}

	public void Update()
	{
		brushRect.width = EraserMaterial.mainTexture.width / 100.0f * brushSize;
		brushRect.height = EraserMaterial.mainTexture.height / 100.0f * brushSize;

		newHolePosition = null;

		if(!isFadeInPainting && !isRechargingSpray)
		{				
			if (Input.GetMouseButton(0) && image_SprayCapacity.fillAmount > 0)
			{				
				//Reset Shake Points
				if(minCursor != null)
				{
					list_ShakePoints.Clear();
					minCursor = null;				
					shakeIterations = 0;
				}

				if(!isPlayingSound)
					PlaySound(ac_Spraying, true);

				image_SprayCapacity.fillAmount -= Time.deltaTime / 3;

				Vector2 v = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dustDepth));

				if (ScreenRect.Contains(v))
				{						
					//Get MousePosition for eraser
					newHolePosition = new Vector2(100 * (v.x - ScreenRect.xMin),
					                              100 * (v.y - ScreenRect.yMin));
				}
				else
					if(playerClicked)
				{
					CancelInvoke();
					playerClicked = false;
				}
			}
			else
			{
				mouseLastPosition = null;

				if(playerClicked)
				{
					CancelInvoke();
					playerClicked = false;
				}

				StopSound();

				if(!Input.GetMouseButton(0))
					if((image_SprayCapacity.fillAmount < 1 && CheckShake()) || image_SprayCapacity.fillAmount == 0 )
						StartCoroutine("RecharcheSpray");
			}
		}
		//		Cursor Texture
//			cursorTexture.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,0));
		cursorTexture.position = Input.mousePosition;
		cursorTexture.localScale = Vector3.one * cursorTextureSize * brushSize;

	}
	public void OnPostRender()
	{
		//Start It will clear Graphics buffer
		if (firstFrame)
		{
			firstFrame = false;
			GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
		}
		//Generate GL quad according to eraser material texture
		if (newHolePosition != null || list_mousePos.Count != 0)
			EraseBrush(new Vector2(1024, 614));
	}
	#endregion

	#region Private
	private void EraseBrush(Vector2 imageSize)
	{
//		print ("DrawCalls: ");
		Rect textureRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f); //this will get erase material texture	part

		//This will Generate position of eraser according to mouse position and size of eraser texture
		//Draw Graphics Quad using GL library to render in target render texture of camera to generate effect
		if(list_mousePos.Count == 0)
		{
			Rect positionRect  = new Rect();
			if(newHolePosition != null)
			{
				positionRect = new Rect(
					(newHolePosition.Value.x - 0.5f * -brushRect.width * 100) /  imageSize.x,
					(newHolePosition.Value.y - 0.5f * -brushRect.height * 100) / imageSize.y,
					brushRect.width * 100 / imageSize.x,
					brushRect.height * 100 / imageSize.y
					); 

				GL.PushMatrix();
				GL.LoadOrtho();
				EraserMaterial.SetPass(0);
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
				GL.PopMatrix();
			}


		}
		else
		{
			GL.PushMatrix();
			GL.LoadOrtho();
			for (int i = 0; i < list_mousePos.Count; i++)
			{
				EraserMaterial.SetPass(0);
				GL.Begin(GL.QUADS);
				GL.Color(Color.white);
				GL.TexCoord2(textureRect.xMin, textureRect.yMax);
				GL.Vertex3(list_mousePos[i].xMin, list_mousePos[i].yMax, 0.0f);
				GL.TexCoord2(textureRect.xMax, textureRect.yMax);
				GL.Vertex3(list_mousePos[i].xMax, list_mousePos[i].yMax, 0.0f);
				GL.TexCoord2(textureRect.xMax, textureRect.yMin);
				GL.Vertex3(list_mousePos[i].xMax, list_mousePos[i].yMin, 0.0f);
				GL.TexCoord2(textureRect.xMin, textureRect.yMin);
				GL.Vertex3(list_mousePos[i].xMin, list_mousePos[i].yMin, 0.0f);
				GL.End();
			}
			GL.PopMatrix();
		}

		list_mousePos.Clear();
	}

	private void GenerateListMousePosition(Vector2 mousePos)
	{
		Vector3 pos = new Vector3(mousePos.x, Screen.height - mousePos.y - 1.0f, dustDepth);
		Vector2 v = Camera.main.ScreenToWorldPoint(pos);
		if (ScreenRect.Contains(v))
		{	
			Vector2 imageLocalPosition = new Vector2(
				100 * (v.x - ScreenRect.xMin),
				100 * (v.y - ScreenRect.yMin)
				);

			Rect positionRect = new Rect(
				(imageLocalPosition.x - 0.5f * brushRect.width * 100) / 1024,
				(imageLocalPosition.y - 0.5f * brushRect.height * 100) / 614,
				brushRect.width * 100 / 1024,
				brushRect.height * 100 / 614
				); 

			if(!list_mousePosGizmos.Contains(positionRect))
			{
				list_mousePos.Add(positionRect);
				list_mousePosGizmos.Add(new Rect(v.x,v.y,1,1));

				EraseRects(v);

				if(mouseLastPosition != null)
				{
					while(Vector2.Distance(mouseLastPosition.Value,mousePos) > 15)
					{
						Vector2 dir = 10 * new Vector2(mousePos.x - mouseLastPosition.Value.x, mousePos.y -mouseLastPosition.Value.y).normalized;
						mouseLastPosition += dir;

						pos = new Vector3(mouseLastPosition.Value.x, Screen.height - mouseLastPosition.Value.y - 1.0f, dustDepth);
						v = Camera.main.ScreenToWorldPoint(pos);

						if (ScreenRect.Contains(v))
						{
							imageLocalPosition = new Vector2(
								100 * (v.x - ScreenRect.xMin),
								100 * (v.y - ScreenRect.yMin)
								);

							positionRect = new Rect(
								(imageLocalPosition.x - 0.5f * brushRect.width * 100) / 1024,
								(imageLocalPosition.y - 0.5f * brushRect.height * 100) / 614,
								brushRect.width * 100 / 1024,
								brushRect.height * 100 / 614
								); 

							if(!list_mousePos.Contains(positionRect))
							{
								list_mousePos.Add(positionRect);
								list_mousePosGizmos.Add(new Rect(v.x,v.y,1,1));

								EraseRects(v);
							}
						}
					}
				}

				mouseLastPosition = mousePos;
				return;
			}

		}
	}
	
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

	private void EraseRects(Vector3 v)
	{
		brushRect.center = v;

		//Check Rectangle close to mouse position
		foreach(Rect rt in paintingQuads)
			if(brushRect.Contains(rt.center))
				paintingQuadsToRemove.Add(rt);
		
		//Remove rectangles from the list
		foreach(Rect tc in paintingQuadsToRemove)
			paintingQuads.Remove(tc);
		
		paintingQuadsToRemove.Clear();
		
		if(paintingQuads.Count * 100 / maxQuads < percentage)
		{
			StopSound();
			image_SprayCapacity.fillAmount = 1;
			StartCoroutine("FadeInPainting");
		}
	}
	
	private int HowManyIntegerInInteger(int segment, int maxSegment)
	{
		int value = 0;
		value = maxSegment / segment;
		
		float rest = maxSegment % segment;		
		if(rest != 0)
		{
			if(rest > 0)
				value++;
			else
				value--;
		}
		return value;
	}

	private IEnumerator FadeInPainting()
	{
		isFadeInPainting = true;
		float alpha = 0.0f;
		Material mat = Dust.GetComponent<Renderer>().material;
		
//		GL.Clear(true, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
		while(alpha != 1)
		{			
			alpha += Time.deltaTime * 0.5f;
			alpha = Mathf.Clamp(alpha,0,1);
			mat.SetFloat("_Alpha", alpha);
			yield return null;
		}
//		StartCoroutine("ResetPainting");
	}

	private IEnumerator ResetPainting()
	{		
		isFadeInPainting = false;

		rt.Release();
		GenerateRects();

		list_mousePosGizmos.Clear();

		Dust.GetComponent<Renderer>().material.SetFloat("_Alpha", 0);
		yield return null;
	}

	private void PlaySound(AudioClip clip, bool loop)
	{
		audioSource.loop = loop;
		audioSource.clip = clip;
		audioSource.Play();
		
		isPlayingSound = true;
	}

	private void StopSound()
	{
		audioSource.Stop();
		isPlayingSound = false;
	}

	private IEnumerator RecharcheSpray()
	{
		isRechargingSpray = true;
		PlaySound(ac_SprayRecharge, false);
		float startingTime = Time.time;
		float capacityLeft = image_SprayCapacity.fillAmount;
		float capacityTofill = 1 - capacityLeft;
		float rechargeTime = 1.70f * capacityTofill;
		float soundTimer = startingTime + rechargeTime;
//		float soundTimer = startingTime + audioSource.clip.length;

		while((Time.time <= soundTimer || image_SprayCapacity.fillAmount != 1) && !Input.GetMouseButton(0))
		{
			image_SprayCapacity.fillAmount =  capacityLeft + (capacityTofill * ((Time.time - startingTime) / rechargeTime));
//			image_SprayCapacity.fillAmount =  capacityLeft + ((1 - capacityLeft) * ((Time.time - startingTime) / audioSource.clip.length));
			yield return null;
		}
		isPlayingSound = false;
		isRechargingSpray = false;
	}

	private bool CheckShake()
	{
		if(Input.mousePosition.x >= 0 && Input.mousePosition.x <= Screen.width 
		   && Input.mousePosition.y >= 0 && Input.mousePosition.y <= Screen.height)
		{	
			if(minCursor == null)
			{
				minCursor =  Input.mousePosition;	

				list_ShakePoints.Add(Camera.main.ScreenToWorldPoint(new Vector3(minCursor.Value.x, minCursor.Value.y, dustDepth)));
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
					{
						shakeIterations ++;
	//					print ("Shake Iteration From " + dirCursor + " To " + tempDirCursor + " / " + shakeIterations);
						
						list_ShakePoints.Add(Camera.main.ScreenToWorldPoint(new Vector3(minCursor.Value.x, minCursor.Value.y, dustDepth)));
						list_ShakePoints.Add(Camera.main.ScreenToWorldPoint(new Vector3(maxCursor.x, maxCursor.y, dustDepth)));
					}
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

}
