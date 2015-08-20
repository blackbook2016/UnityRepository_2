using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMasking : MonoBehaviour
{
	#region Properties
	public GameObject Dust;
	private Rect ScreenRect;
	private float dustDepth = 0.0f;

	private RenderTexture rt;

	public Material EraserMaterial;
	
	private bool firstFrame;
	private Vector2? newHolePosition;
	
	[Range(-5, 5)]
	public int brushSize = 1;

	public Transform cursorTexture;

	public int percentage = 15; 

	private Rect brushRect;
	private int maxQuads = 0; 
	
	private List<Rect> paintingQuads = new List<Rect>();
	private List<Rect> paintingQuadsToRemove = new List<Rect>();

	private bool isFadeInPainting = false;
	#endregion

	#region Unity
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		foreach(Rect cb in paintingQuads)
		{
			Gizmos.DrawCube(cb.center, cb.size);
		}
	}
	void OnGUI()
	{
		GUI.Label(new Rect(0, 0, 30, 20),(maxQuads - paintingQuads.Count) * 100 /maxQuads+"%");

		if(isFadeInPainting)
		{
			if(GUI.Button(new Rect(0, 40, 120, 20), "Restart"))
			{
				StopAllCoroutines();
				StartCoroutine("ResetPainting");
			}
		}
	}
	
	public IEnumerator Start()
	{
		Cursor.visible = false;
		firstFrame = true;
		//Get Erase effect boundary area
		ScreenRect.x = Dust.GetComponent<Renderer>().bounds.min.x;
		ScreenRect.y = Dust.GetComponent<Renderer>().bounds.min.y;
		ScreenRect.width = Dust.GetComponent<Renderer>().bounds.size.x;
		ScreenRect.height = Dust.GetComponent<Renderer>().bounds.size.y;

		dustDepth = Dust.transform.position.z - transform.position.z;

		brushRect.width = EraserMaterial.mainTexture.width / 100.0f;
		brushRect.height = EraserMaterial.mainTexture.height / 100.0f;

		if(brushSize >= 0)
		{
			brushRect.width *= Mathf.Abs(brushSize);
			brushRect.height *= Mathf.Abs(brushSize);
		}
		else
		{
			brushRect.width /= Mathf.Abs(brushSize);
			brushRect.height /= Mathf.Abs(brushSize);
		}

		rt = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
		yield return rt.Create();

		GetComponent<Camera>().targetTexture = rt;

		Dust.GetComponent<Renderer>().material.SetTexture("_MaskTex", rt);
		
		GenerateRects ();

	}
	public void Update()
	{
		if(!isFadeInPainting)
		{
			newHolePosition = null;
			
			if (Input.GetMouseButton(0)) //Check if MouseDown
			{		
				Vector2 v = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dustDepth));
				brushRect.center = v;

				if (ScreenRect.Contains(v))
				{
					//Get MousePosition for eraser
					print (ScreenRect );
					newHolePosition = new Vector2(100 * (v.x - ScreenRect.xMin),
					                              100 * (v.y - ScreenRect.yMin));

					//Check Rectangle close to mouse position
					foreach(Rect rt in paintingQuads)
						if(brushRect.Contains(rt.center))
							paintingQuadsToRemove.Add(rt);

					//Remove rectangles from the list
					foreach(Rect tc in paintingQuadsToRemove)
						paintingQuads.Remove(tc);

					paintingQuadsToRemove.Clear();

					if(paintingQuads.Count * 100 / maxQuads < percentage)
						StartCoroutine("FadeInPainting");
				}
			}
		}
		cursorTexture.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dustDepth- 0.5f));
		cursorTexture.localScale = brushSize >= 0 ? Vector3.one * brushSize : Vector3.one / brushSize;
		
//		Material mat = Dust.GetComponent<Renderer>().material;
//		mat.SetFloat("_Alpha", Mathf.Sin (Mathf.Abs(Time.time)));
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
		if (newHolePosition != null)
			EraseBrush(new Vector2(1024, 614), newHolePosition.Value);
	}
	#endregion

	#region Private
	private void EraseBrush(Vector2 imageSize, Vector2 imageLocalPosition)
	{
		Rect textureRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f); //this will get erase material texture	part
		float brushWidth = 0.0f;
		float brushHeight = 0.0f;
		if(brushSize >= 0)
		{
			brushWidth = EraserMaterial.mainTexture.width * brushSize;
			brushHeight = EraserMaterial.mainTexture.height * brushSize;
		}
		else
		{
			brushWidth = EraserMaterial.mainTexture.width / brushSize;
			brushHeight = EraserMaterial.mainTexture.height / brushSize;
		}
		Rect positionRect = new Rect(
			(imageLocalPosition.x - 0.5f * brushWidth) / imageSize.x,
			(imageLocalPosition.y - 0.5f * brushHeight) / imageSize.y,
			brushWidth / imageSize.x,
			brushHeight / imageSize.y
			); 
		//This will Generate position of eraser according to mouse position and size of eraser texture
		//Draw Graphics Quad using GL library to render in target render texture of camera to generate effect
		GL.PushMatrix();
		GL.LoadOrtho();
		for (int i = 0; i < EraserMaterial.passCount; i++)
		{
			EraserMaterial.SetPass(i);
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
		while(alpha != 1)
		{			
			alpha += Time.deltaTime * 0.5f;
			alpha = Mathf.Clamp(alpha,0,1);
			mat.SetFloat("_Alpha", alpha);
			yield return null;
		}
	}

	private IEnumerator ResetPainting()
	{		
		isFadeInPainting = false;

		rt.Release();
		GenerateRects();

		Dust.GetComponent<Renderer>().material.SetFloat("_Alpha", 0);
		yield return null;
	}
	#endregion

}
