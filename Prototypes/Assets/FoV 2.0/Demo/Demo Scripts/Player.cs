using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace FoVDemo {

	public class Player : MonoBehaviour {

		NavMeshAgent navAgent;

		// Use this for initialization
		void Start () {
		
			navAgent = GetComponent<NavMeshAgent>();

		}
		
		// Update is called once per frame
		void Update () {
		
			if (Input.GetMouseButton (0)) {

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				
				RaycastHit[] hits = Physics.RaycastAll(ray, 200);

				foreach(RaycastHit hit in hits) {
				
					if(hit.collider.name == "Floor") navAgent.SetDestination(hit.point);

				}

			}

		}

	}

}