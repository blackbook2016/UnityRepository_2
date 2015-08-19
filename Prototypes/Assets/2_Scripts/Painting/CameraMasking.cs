using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMasking : MonoBehaviour
{
	public GameObject Dust;
	Rect ScreenRect;
	RenderTexture rt;
	Texture2D tex;
	public Material EraserMaterial;
	private bool firstFrame;
	private Vector2? newHolePosition;
	[Range(1, 10)]
	public int brushSize = 1;
	private Rect brushRect;

	private List<Rect> paintingQuads = new List<Rect>();

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}

	public IEnumerator Start()
	{
		firstFrame = true;
		//Get Erase effect boundary area
		ScreenRect.x = Dust.GetComponent<Renderer>().bounds.min.x;
		ScreenRect.y = Dust.GetComponent<Renderer>().bounds.min.y;
		ScreenRect.width = Dust.GetComponent<Renderer>().bounds.size.x;
		ScreenRect.height = Dust.GetComponent<Renderer>().bounds.size.y;

		brushRect.width = EraserMaterial.mainTexture.width;
		brushRect.height = EraserMaterial.mainTexture.height;

		//Create new render texture for camera target texture
		rt = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
		yield return rt.Create();
		Graphics.Blit(tex, rt);
		GetComponent<Camera>().targetTexture = rt;
		//Set Mask Texture to dust material to Generate Dust erase effect
		Dust.GetComponent<Renderer>().material.SetTexture("_MaskTex", rt);

		GenerateRects ();
	}
	public void Update()
	{
		newHolePosition = null;

		if (Input.GetMouseButton(0)) //Check if MouseDown
		{
			Vector2 v = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
			Rect worldRect = ScreenRect;
			if (worldRect.Contains(v))
			{
				//Get MousePosition for eraser
				newHolePosition = new Vector2(1024 * (v.x - worldRect.xMin) / worldRect.width,
				                              614 * (v.y - worldRect.yMin) / worldRect.height);
			}
		}
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
	
	private void EraseBrush(Vector2 imageSize, Vector2 imageLocalPosition)
	{
		Rect textureRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f); //this will get erase material texture	part
		Rect positionRect = new Rect(
			(imageLocalPosition.x - 0.5f * EraserMaterial.mainTexture.width) / imageSize.x,
			(imageLocalPosition.y - 0.5f * EraserMaterial.mainTexture.height) / imageSize.y,
			EraserMaterial.mainTexture.width / imageSize.x,
			EraserMaterial.mainTexture.height / imageSize.y
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
//		CheckFilledPercentage();
	}

//	private void CheckFilledPercentage()
//	{
//		int countFilledPixels = 0;
//		for (int y = 0; y < tmptex.height; y++) 
//		{
//			for (int x = 0; x < tmptex.width; x++) 
//			{
//				if(tmptex.GetPixel(x,y).a == 0)
//					countFilledPixels++;
//			}
//		}
//		print (countFilledPixels);
//	}

	private void GenerateRects()
	{		
//		print (ScreenRect.width * 100 + " " + ScreenRect.height * 100);
//		print (brushRect.width + " " + brushRect.height);
//		int width = 0;
//		int height = 0;
//		int valuew = ScreenRect.width * 100 / brushRect.width;
//		if((ScreenRect.width * 100) % brushRect.width)
//			valuew = 

//		width = 
//		paintingQuads
	}
}
