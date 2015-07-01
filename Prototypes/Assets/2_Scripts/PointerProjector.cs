namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	[RequireComponent(typeof(Projector))]
	public class PointerProjector : MonoBehaviour
	{
		public static PointerProjector Instance;
		public float AnimTimeout;
		public float Distance;
		public float FovMax;
		public float FovMin;
		
		private Projector projector;
		private float timeout;
		
		void Awake()
		{
			Instance = this;
			projector = GetComponent<Projector>();
			if(projector == null)
				print("Missing projector componnent!");
		}
		
		public void Enable()
		{
			projector.enabled = true;
		}
		
		public void Disable()
		{
			projector.enabled = false;
		}
		
		public void Project(Vector3 pos, Color color)
		{
			projector.material.color = color;
			Enable();
			projector.fieldOfView = FovMax;
			timeout = AnimTimeout;
			transform.position = pos + Vector3.up*Distance;
		}
		
		void Update()
		{
			timeout -= Time.deltaTime;
			
			if (timeout > 0.0f)
			{
				var t = (timeout/AnimTimeout);
				
				float tt;
				
				if (t > 0.5)
				{
					tt = (t - 0.5f)/0.5f;
				}
				else
				{
					tt = t/0.5f;
				}
				
				projector.fieldOfView = FovMin + (FovMax - FovMin)*tt;
			}
			else
			{
				projector.fieldOfView = FovMax;
			}
		}
	}
}
