namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	public class GroundMarker : MonoBehaviour {

		public float AnimTimeout;
		public float Distance;
		public float FovMax;
		public float FovMin;
		
		private SpriteRenderer sprite;
		private float timeout;
		
		void Awake()
		{
			sprite = GetComponent<SpriteRenderer>();
		}
		
		public void Enable()
		{
			sprite.enabled = true;
		}
		
		public void Disable()
		{
			sprite.enabled = false;
		}
		
		public void Project(Vector3 pos, Color color)
		{
			sprite.material.color = color;
			Enable();
			transform.localScale = Vector3.one * FovMax;
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
				float scale = FovMin + (FovMax - FovMin)*tt;
				transform.localScale = Vector3.one * scale;
			}
			else
			{
				transform.localScale = Vector3.zero;
			}
		}
	}
}
