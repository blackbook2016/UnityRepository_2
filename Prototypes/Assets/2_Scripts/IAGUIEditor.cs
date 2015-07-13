using UnityEngine;
using System.Collections;

public class IAGUIEditor : MonoBehaviour {

	bool activ = true;
	private GameObject player;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void OnGUI () 
	{
		if(GUI.Button(new Rect(20,40,100,40), "Enable/Disable")) {
			activ = !activ;
			player.SetActive(activ);
		}
	}
}
