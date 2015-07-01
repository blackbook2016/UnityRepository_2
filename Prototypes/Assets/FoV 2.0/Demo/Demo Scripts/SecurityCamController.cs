using FoV2;
using UnityEngine;
using System.Collections;

namespace FoVDemo {

	public class SecurityCamController : MonoBehaviour {

		int dir = 1;

		public float speed = 1;
		public float fovSpeed = 1;

		DoubleFoV fov;

		Transform player;
		Transform cameraBody;

		public bool playerDetected;

		public Color fovDefaultColor;
		public Color fovPlayerDetectedColor;

		public bool rotateCam = true;
		public bool rotateFov = true;

		void Start() {

			fov = GetComponentInChildren<DoubleFoV>();
			player = GameObject.FindGameObjectWithTag("Player").transform;
			cameraBody = transform.FindChild("Body");

		}

		// Update is called once per frame
		void Update () {
		
			DetectPlayer();
			UpdateFoVColor();

			if(!playerDetected) {

				if(rotateCam) cameraBody.Rotate(Vector3.up, dir * speed * Time.deltaTime);
				if(rotateFov) fov.transform.Rotate(Vector3.forward, -dir * fovSpeed * Time.deltaTime);
				if(dir == 1 && cameraBody.rotation.eulerAngles.y >= 345 && cameraBody.rotation.eulerAngles.y <= 355) dir = -1;
				else if(dir == -1 && cameraBody.rotation.eulerAngles.y >= 175 && cameraBody.rotation.eulerAngles.y <= 185) dir = 1;

			} else {

				cameraBody.LookAt(player);
				fov.transform.LookAt(player);

			}

		}

		void DetectPlayer() {

			if(!playerDetected && fov.GetDetectedObjects().Contains(player)) playerDetected = true;
			else if(playerDetected && !fov.GetDetectedObjects().Contains(player)) playerDetected = false;

		}

		void UpdateFoVColor() {

			if(playerDetected) fov.GetComponent<Renderer>().material.color = fovPlayerDetectedColor;
			else fov.GetComponent<Renderer>().material.color = fovDefaultColor;

		}

	}

}