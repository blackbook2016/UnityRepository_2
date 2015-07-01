using FoV2;
using UnityEngine;
using System.Collections;

namespace FoVDemo {

	public class TowerFOVController : MonoBehaviour {

		public float speed = 25;
		
		FoV fov;
		
		Transform player;
		
		bool playerDetected;
		
		public Color fovDefaultColor;
		public Color fovPlayerDetectedColor;

		public Texture fovGreen;
		public Texture fovRed;

		public bool rotate = false;
		
		void Start () {
		
			fov = GetComponentInChildren<FoV>();
			player = GameObject.FindGameObjectWithTag("Player").transform;

		}
		
		// Update is called once per frame
		void Update () {
		
			DetectPlayer();
			UpdateFoVColor();
			
			if(!playerDetected) {
				
				if(rotate) fov.transform.Rotate(Vector3.up, speed * Time.deltaTime);
				
			}

		}

		void DetectPlayer() {
			
			if(!playerDetected && fov.GetDetectedObjects().Contains(player)) playerDetected = true;
			else if(playerDetected && !fov.GetDetectedObjects().Contains(player)) playerDetected = false;
			
		}
		
		void UpdateFoVColor() {

			/*
			if(playerDetected) fov.GetComponent<Renderer>().material.color = fovPlayerDetectedColor;
			else fov.GetComponent<Renderer>().material.color = fovDefaultColor;
			*/

			if(playerDetected) fov.GetComponent<Renderer>().material.mainTexture = fovRed;
			else fov.GetComponent<Renderer>().material.mainTexture = fovGreen;
			
		}

	}

}