namespace TheVandals
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public enum WaypointType{
		None,
		ClosedLoop,
		PingPong,
		RandomPath,
	}

	public class WaypointManager : MonoBehaviour 
	{
		#region Properties
		public WaypointType wpType = WaypointType.None;
		public List<Waypoint> points;
		
		private Waypoint giznextPoint;
		private Waypoint nextPoint = null;
		private Waypoint StartPoint = null;
		private Waypoint EndPoint = null;

		private  bool direction = true;
		private int currentPosition;

		#endregion
		#region Editor API
		void OnDrawGizmos()
		{
			Refresh();
			Gizmos.color = Color.blue;
			Waypoint currentpoint = StartPoint;

			switch (wpType)
			{
				case WaypointType.None:
				{
					while(currentpoint != EndPoint)
					{
					giznextPoint = points[points.IndexOf(currentpoint) + 1];
					Gizmos.DrawLine(currentpoint.transform.position, giznextPoint.transform.position);
					currentpoint = giznextPoint;
					}
					break;
				}
				case WaypointType.ClosedLoop:
				{								
					while(currentpoint != EndPoint)
					{
					giznextPoint = points[points.IndexOf(currentpoint) + 1];
					Gizmos.DrawLine(currentpoint.transform.position, giznextPoint.transform.position);
					currentpoint = giznextPoint;
					}
					Gizmos.DrawLine(StartPoint.transform.position, EndPoint.transform.position);
					break;
				}
				case WaypointType.PingPong:
				{
					while(currentpoint != EndPoint)
					{
					giznextPoint = points[points.IndexOf(currentpoint) + 1];
					Gizmos.DrawLine(currentpoint.transform.position, giznextPoint.transform.position);
					currentpoint = giznextPoint;
					}
					break;
				}
				case WaypointType.RandomPath:
				{
					for(int i = points.IndexOf(StartPoint); i < points.IndexOf(EndPoint); i++)
					{
						for(int y = i+1; y <= points.IndexOf(EndPoint); y++)
							Gizmos.DrawLine(points[i].transform.position, points[y].transform.position);
					}
					break;
				}
			}

		}
		#endregion
		#region Unity
		void Awake() 
		{
			Refresh();
		}
		#endregion

		#region API
		public Waypoint NextPoint
		{
			get
			{
				return nextPoint;
			}
		}
		
		public void SetNextPoint()
		{
			if(EndPoint == null || StartPoint == null)
			{
				print ("Path endpoint or startpoint aren't defined");
			}
			if(nextPoint == null)
				nextPoint = StartPoint;
			else
			{
				currentPosition = points.IndexOf(nextPoint);

				switch (wpType)
				{
					case WaypointType.None:
					{
						if(currentPosition == points.IndexOf(EndPoint))
							nextPoint = EndPoint;
						else 
							nextPoint = points[points.IndexOf(nextPoint) + 1];
						break;
					}
					case WaypointType.ClosedLoop:
					{			
						if (nextPoint == EndPoint)
						{
							nextPoint = points[0];
						}				
						else 
							nextPoint = points[points.IndexOf(nextPoint) + 1];
						break;
					}
					case WaypointType.PingPong:
					{
						if(currentPosition == points.IndexOf(EndPoint) || (currentPosition == 0 && !direction))
						{
							direction = !direction;
						}
						if(direction)
							nextPoint = points[currentPosition+1];
						else
							nextPoint = points[currentPosition-1];
						break;
					}
					case WaypointType.RandomPath:
					{
						while(points.IndexOf(nextPoint) == currentPosition)
							nextPoint = points[Random.Range(0, points.IndexOf(EndPoint) + 1)];
						break;
					}
				}
			}
		}
		
		public void reset()
		{
			nextPoint = null;
		}
		#endregion		

		#region Private
		private void Refresh()
		{
			points.Clear();

			Waypoint[] waypoints = GetComponentsInChildren<Waypoint>();
			
			foreach(Waypoint waypoint in waypoints)
			{
				points.Add(waypoint);
				if(waypoint.pointType == PointType.Start)
					StartPoint = waypoint;
				else if(waypoint.pointType == PointType.End)
					EndPoint = waypoint;
			}			

			giznextPoint = StartPoint;
		}
		#endregion		
	}
}
