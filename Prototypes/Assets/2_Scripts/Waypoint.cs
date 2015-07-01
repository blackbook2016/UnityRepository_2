namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	public enum PointType{
		Path,
		Start,
		End,
	}

	public class Waypoint : MonoBehaviour 
	{
		public PointType pointType = PointType.Path;

		#region Editor API
		void OnDrawGizmos()
		{
			if(pointType == PointType.Path)
				Gizmos.color = Color.blue;
			else
				Gizmos.color = Color.red;

			Gizmos.DrawWireSphere(transform.position, 0.5f);
		}
		#endregion
	}
}
