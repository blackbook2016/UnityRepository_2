namespace TheVandals {

	using UnityEngine;
	using System.Collections;

	public enum State {
		Idle,
		Walk,
		Run,
		Climb,
		Dead,
		Shout,
	}	
	
	public enum PlayerState {
		Free,
		Caught,
		CaptureOeuvre,
	}

	public enum EnemyType {
		_Fixe,
		_RoamingRandom,
		_RoamingPath,
		_Busy,
		_Camera,
	}

	public enum EnemyBeHaviour {
		Idle,
		Curious,
		Alert,
		Searching,
	}
	
	public enum EnemyActions {
		Idle,
		GoToRandomPoint,
		GoToPathPoint,
		WatchPlayer,
		FollowPlayerPoint,
		SearchL,
		SearchM,
		ReturnToInitialPosition,
		runTowardPlayer,
	}

	public enum AlertLevel {
		None,
		Low,
		Medium,
		High
	}

	public enum AreaMask {
		
		All = -1,
		Nothing = 0,
		Area1 = 1 << 3,
		Area2 = 1 << 4,
		Area3 = 1 << 5,
		Area4 = 1 << 6,
	}

	public struct TargetDestination
	{
		public Vector3 position;
		public Vector3 eulerAngles;
		
		public TargetDestination(Vector3 position,Vector3 eulerAngles)
		{
			this.position = position;
			this.eulerAngles = eulerAngles;
		}
	}

	public struct Shout
	{
		public GameObject obj;
		public float radius;

		public Shout (GameObject obj, float radius)
		{
			this.obj = obj;
			this.radius = radius;
		}
	}

	public enum CaptureOeuvreType {
		TypeFPS,
		TypeRTS,
	}

//	public struct AreaMask {
//
//		int All;
//		int Nothing;
//		int Area1;
//		int Area2;
//		int Area3;
//		int Area4;
//
//		public AreaMask ()
//		{
//			this.All = -1;
//			this.Nothing = 0;
//			this.Area1 = 1 << 3;
//			this.Area2 = 1 << 4;
//			this.Area3 = 1 << 5;
//			this.Area4 = 1 << 6;
//		}
//	}
}
