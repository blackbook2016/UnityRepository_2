using FoV2;
using UnityEngine;
using System.Collections;

namespace FoVDemo {

	public class EnemyMobile : MonoBehaviour {

		FoV fov;

		Transform player;

		bool playerDetected;

		NavMeshAgent navAgent;

		public Texture fovGreen;
		public Texture fovRed;

		Vector3 initPos;

		Vector3 targetPos;

		// Use this for initialization
		void Start () {
		
			fov = GetComponentInChildren<FoV>();

			player = GameObject.FindGameObjectWithTag("Player").transform;

			playerDetected = false;

			navAgent = GetComponent<NavMeshAgent>();

			initPos = transform.position;

			InvokeRepeating ("UpdateFoV", 0, 0.1f);

		}

		// Update is called once per frame
		void UpdateFoV () {

			DetectPlayer();

			UpdateFoVColor();

			if(playerDetected) {

				navAgent.SetDestination(targetPos);

			} else if(navAgent.remainingDistance <= 1) {

				navAgent.SetDestination(initPos);

			}

		}

		void DetectPlayer() {

			if(fov.GetDetectedObjects().Contains(player)) {

				playerDetected = true;
				targetPos = player.position;

			} else {

				playerDetected = false;

			}

		}

		void UpdateFoVColor() {

			if(playerDetected) fov.GetComponent<Renderer>().material.mainTexture = fovRed;
			else fov.GetComponent<Renderer>().material.mainTexture = fovGreen;

		}

	}

}