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

	private float radius = 2;	
	void Start()
	{		
		transform.localScale = Vector3.one * radius;
	}
	
	void Update () 
	{	
		// get mouse pos
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
		RaycastHit hit ;
		if (Physics.Raycast (ray,out hit, Mathf.Infinity)) 
		{
			GetComponent<Renderer>().material.SetVector("_ObjPos",new Vector4(hit.point.x,hit.point.y,hit.point.z,0));
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
