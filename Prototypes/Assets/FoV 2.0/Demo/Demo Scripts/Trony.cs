using UnityEngine;
using System.Collections;

namespace FoVDemo {

	public class Trony : MonoBehaviour {
		
		Transform electron1;
		Transform electron2;
		
		float rotationSpeed;

		float trailMinTime;
		float trailMaxTime;

		NavMeshAgent navAgent;

		void Start() {
			
			navAgent = GetComponent<NavMeshAgent>();

			electron1 = transform.FindChild("Electron1");
			electron2 = transform.FindChild("Electron2");

			trailMinTime = 0.1f;
			trailMaxTime = 0.75f;

			rotationSpeed = 250;

		}

		// Update is called once per frame
		void Update () {

			electron1.RotateAround(transform.localPosition, transform.up + transform.right, rotationSpeed * Time.deltaTime);
			electron2.RotateAround(transform.localPosition, transform.up - transform.right, rotationSpeed * Time.deltaTime);

			if(navAgent.remainingDistance < 0.5f) {
				
				electron1.GetComponent<TrailRenderer>().time = trailMaxTime;
				electron2.GetComponent<TrailRenderer>().time = trailMaxTime;
				
			} else {
				
				electron1.GetComponent<TrailRenderer>().time = trailMinTime;
				electron2.GetComponent<TrailRenderer>().time = trailMinTime;
				
			}
			
		}

	}

}