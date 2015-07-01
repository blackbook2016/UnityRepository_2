using UnityEngine;
using System.Collections;

public class CamLookAt : MonoBehaviour {

	public Transform target;

	// Update is called once per frame
	void Update () {
	
		if (target != null) transform.LookAt (target);

	}

}