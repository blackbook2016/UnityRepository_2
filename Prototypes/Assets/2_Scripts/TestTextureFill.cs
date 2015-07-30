using UnityEngine;
using System.Collections;

public class TestTextureFill : MonoBehaviour {
//	public Texture2D texture;
//	void Start ()
//	{
//		texture = GetComponent<MeshRenderer>().material.mainTexture  as Texture2D ;
//		texture.FloodFillBorder(10,10,Color.red,Color.blue);
//		texture.Apply();
//	}

//	private float radius = 2;	
//	void Start()
//	{		
//		transform.localScale = Vector3.one * radius;
//	}
//	
//	void Update () 
//	{	
//		// get mouse pos
//		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
//		RaycastHit hit ;
//		if (Physics.Raycast (ray,out hit, Mathf.Infinity)) 
//		{
//			GetComponent<Renderer>().material.SetVector("_MousePos",new Vector4(hit.point.x,hit.point.y,hit.point.z,0));
//		}
//		if (Input.GetKey ("q"))
//		{
//			transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
//		}
//		if (Input.GetKey ("d"))
//		{
//			transform.Rotate(new Vector3(0, -30, 0) * Time.deltaTime);
//		}
//	}
	// the texture that i will paint with and the original texture (for saving)

	public Texture2D sprayTexture, originalTexture ;
	//temporary texture
	private Texture2D tmpTexture;
	// colors used to tint the first 3 mip levels
	Color[] colors = new Color[3];
	int mipCount ;
	
	// tint each mip level

	// actually apply all SetPixels, don't recalculate mip levels

	void Start ()
	{
		//setting temp texture width and height 
		tmpTexture = new Texture2D (originalTexture.width, originalTexture.height);
		
		colors[0] = Color.red;
		colors[1] = Color.green;
		colors[2] = Color.blue;
		mipCount = Mathf.Min(3, tmpTexture.mipmapCount);
		for( var mip = 0; mip < mipCount; ++mip ) {
			var cols = tmpTexture.GetPixels( mip );
			for( var i = 0; i < cols.Length; ++i ) {
				cols[i] = Color.Lerp(cols[i], colors[mip], 0.33f);
			}
			tmpTexture.SetPixels( cols, mip );
		}
		//fill the new texture with the original one (to avoid "empty" pixels)
		for (int y =0; y<tmpTexture.height; y++) {
			for (int x = 0; x<tmpTexture.width; x++) {
				tmpTexture.SetPixel (x, y, originalTexture.GetPixel (x, y));
			}
		}
//		print (tmpTexture.height);
		//filling a part of the temporary texture with the target texture 
//		for (int y =0; y<tmpTexture.height; y++) 
//		{
//			for (int x = 0; x<tmpTexture.width; x++) 
//			{
//				if(y<tmpTexture.height-300)
//				{
//					tmpTexture.SetPixel (x, y, targetTexture.GetPixel (x, y));				
//				}
//				else
//				tmpTexture.SetPixel(x,y,new Color(0,0,0,0));
//			}
//		}
		//Apply 
		tmpTexture.Apply (false);
		//change the object main texture 
		GetComponent<Renderer>().material.mainTexture = tmpTexture;
	}

	void Update()
	{
		// get mouse pos
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
		RaycastHit hit ;
		if (Physics.Raycast (ray,out hit, Mathf.Infinity)) 
		{
			print ("here" + hit.collider);
			Vector2 pixelUV = hit.textureCoord;
			pixelUV.x *= tmpTexture.width;
			pixelUV.y *= tmpTexture.height;
			tmpTexture.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.black);
			tmpTexture.Apply();
			GetComponent<Renderer>().material.mainTexture = tmpTexture;
		}
		if (Input.GetKey ("q"))
		{
			transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
		}
		if (Input.GetKey ("d"))
		{
			transform.Rotate(new Vector3(0, -30, 0) * Time.deltaTime);
		}
	}

}
