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

	public Texture2D tmpTexture;
	private Vector2[] vecArray;

	private float radius = 2;	
	private int vecArrayIndex = 0;

	void Start()
	{		
		vecArrayIndex = 0;
		vecArray = new Vector2[200];

		transform.localScale = Vector3.one * radius;

		Material mat = GetComponent<Renderer>().sharedMaterial;

		for(int i=-30; i< 31; i++, vecArrayIndex++)
		{
			for(int y=-30; y< 31; y++, vecArrayIndex++)
			{
				if(vecArrayIndex < vecArray.Length)
				{
					vecArray[vecArrayIndex].x = (tmpTexture.width/2) + i;
					vecArray[vecArrayIndex].y = (tmpTexture.height/2) + y;
					
					print (vecArrayIndex + "/" +vecArray[vecArrayIndex]);
				}
				else 
				{
					vecArrayIndex -= 2;
					i = y = 31;
				}
			}
		}
//		for(int i = 0; i < vecArrayIndex; i++)
//		{
//			mat.SetFloat("myarray"+i, vecArray[i].x);
//		}
		mat.SetFloat("myarrayx", vecArray[0].x);
		mat.SetFloat("myarrayx", vecArray[0].x);

		ComputeBuffer cb = new ComputeBuffer(200,16);
		cb.SetData(vecArray);
	}
	
//	void Update()
//	{
//		// get mouse pos
//		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
//		RaycastHit hit ;
//		if (Physics.Raycast (ray,out hit, Mathf.Infinity)) 
//		{	
//			Vector2 pixelUV = hit.textureCoord;			
//			pixelUV.x *= tmpTexture.width;
//			pixelUV.y *= tmpTexture.height;
//
//			for(int i=-5; i< 6; i++, vecArrayIndex++)
//			{
//				for(int y=-5; y< 6; y++, vecArrayIndex++)
//				{
//					vecArray[vecArrayIndex].x = pixelUV.x + x;
//					vecArray[vecArrayIndex].y = pixelUV.y + y;
//				}
//			}
//		}
//	}
	void SendToShader()
	{

//		GetComponent<Renderer>().material.SetVector("_MousePos",new Vector4(hit.point.x,hit.point.y,hit.point.z,0));
//		vecArray[vecArrayIndex].x = tmpTexture.width + i;
//		vecArray[vecArrayIndex].y = tmpTexture.height + y;
//		GetComponent<Renderer>().sharedMaterial.SetInt("_MousePos",0);
//		Material mat = GetComponent<Renderer>().sharedMaterial;
	}

	#region SetPixel	
	//	public Texture2D sprayTexture, originalTexture ;
	//	//temporary texture
	//	private Texture2D tmpTexture;
	//
	//
	//	void Start ()
	//	{
	//		//setting temp texture width and height 
	//		tmpTexture = new Texture2D (originalTexture.width, originalTexture.height);
	//
	//		for (int y =0; y<tmpTexture.height; y++) {
	//					for (int x = 0; x<tmpTexture.width; x++) {
	////				tmpTexture.SetPixel (x, y, originalTexture.GetPixel (x, y));
	//				tmpTexture.SetPixel(x,y,new Color(0,0,0,0));
	//			}
	//		}
	//
	//		tmpTexture.Apply ();
	//
	//		GetComponent<Renderer>().material.mainTexture = tmpTexture;
	//	}
	//
	//	void Update()
	//	{
	//		// get mouse pos
	//		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
	//		RaycastHit hit ;
	//		if (Physics.Raycast (ray,out hit, Mathf.Infinity)) 
	//		{			
	//			Vector2 pixelUV = hit.textureCoord;			
	//			pixelUV.x *= tmpTexture.width;
	//			pixelUV.y *= tmpTexture.height;
	//			Vector2 currentPixel;
	//			int pixelcount = 0;
	//			for (int y =0; y<tmpTexture.height; y++) {
	//				for (int x = 0; x<tmpTexture.width; x++) {
	//					currentPixel.x = x;
	//					currentPixel.y = y;
	//					if(Vector2.Distance(currentPixel, pixelUV) < 50f)
	//					{
	//						tmpTexture.SetPixel (x, y, originalTexture.GetPixel (x, y));
	//						pixelcount++;
	//					}
	//				}
	//			}
	//			tmpTexture.Apply();
	//			GetComponent<Renderer>().material.mainTexture = tmpTexture;
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

	#endregion
	#region RenderTexture
//	void Update () {		
//		if(Input.GetMouseButton(0)) {			
//			RaycastHit hitInfo;
//			if( Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo )) {
//				
//				if(hitInfo.collider.tag == "Draw") {
//					
//					Vector2 point = hitInfo.textureCoord2;
//					Texture2D tex = (Texture2D)hitInfo.collider.gameObject.renderer.material.mainTexture;
//					point.x *= tex.width;
//					point.y *= tex.height;
//					
//					AddToStencil(_stencil, _brush, point, brushSize);
//				}
//				else if(hitInfo.collider.tag == "Stencil") {
//					
//					Vector2 point = hitInfo.textureCoord2;
//					Texture2D tex = (Texture2D)hitInfo.collider.gameObject.renderer.material.mainTexture;
//					point.x *= tex.width;
//					point.y *= tex.height;
//					
//					AddToTexture(tex, hardBrush, point, brushSize);
//				}
//			}
//		}
//		
//	}
//
//	void AddToTexture(Texture2D stencil, Texture2D brush, Vector2 brushPosition, int brushSizePixels)
//	{
//		int width = stencil.width;
//		int height = stencil.height;
//		
//		float bs2 = brushSizePixels / 2f;
//		
//		
//		//For loop goes through texture pixels and replaces them with brush pixels
//		for(int i = (int)(brushPosition.y - bs2); i < (int)(brushPosition.y - bs2) + brush.height; i++) {
//			
//			for(int j = (int)(brushPosition.x - bs2); j < (int)(brushPosition.x - bs2) + brush.width; j++) {
//				
//				int x = j - (int)(brushPosition.x - bs2);
//				int y = i - (int)(brushPosition.y - bs2);
//				
//				Color pix = brush.GetPixel(x,y);
//				
//				if(pix != Color.clear) {
//					
//					stencil.SetPixel(j, i, pix);
//				}
//			}
//		}
//		
//		stencil.Apply();
//	}
	#endregion
}
