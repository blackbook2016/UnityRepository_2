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
	
	void Update () 
	{	
		// get mouse pos
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
		RaycastHit hit ;
		if (Physics.Raycast (ray,out hit, Mathf.Infinity)) 
		{
			print("here");
			//		renderer.material.SetVector("_ObjPos", Vector4(obj.position.x,obj.position.y,obj.position.z,0));
//			GetComponent<Renderer>().material.SetVector("_ObjPos", new Vector4(transform.position.x,transform.position.y,transform.position.z,0));
			
			// convert hit.point to our plane local coordinates, not sure how to do in shader.. IN.pos.. ??
			Vector3 hitlocal = transform.InverseTransformPoint(hit.point);
					GetComponent<Renderer>().material.SetVector("_ObjPos",new Vector4(hitlocal.x,hitlocal.y,hitlocal.z,0));
			
		}
		
		
		// box rotation for testing..
		if (Input.GetKey ("q"))
		{
			transform.Rotate(new Vector3(0,30,0) * Time.deltaTime);
		}
		if (Input.GetKey ("d"))
		{
			transform.Rotate(new Vector3(0,-30,0) * Time.deltaTime);
		}
		
		// mousewheel for radius
		if (Input.GetAxis("Mouse ScrollWheel")!=0)
		{
			radius += Input.GetAxis("Mouse ScrollWheel") * 0.8f;
			GetComponent<Renderer>().material.SetFloat( "_Radius", radius);
		}
	}
}
