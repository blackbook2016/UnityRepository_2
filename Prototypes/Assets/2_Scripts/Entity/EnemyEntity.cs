namespace TheVandals
{
	using System;
	using System.Collections.Generic;

	public class EnemyEntity  
	{
		private EnemyType type;
		private EnemyBeHaviour behaviour;
		private FieldOfView fov;
		private bool playerDetected;
		private State moveState;
		private bool canRoam;
		private bool canSearch; 
		private bool canFollowPath;
		private int layerArea;

		#region getter/setter
		public EnemyBeHaviour _Behaviour {
			get {
				return this.behaviour;
			}
			set {
				behaviour = value;
			}
		}
		public EnemyType Type { 
			get {
				return this.type;
			}
			set {
				type = value;
			}
		}
		
		public FieldOfView Fov {
			get {
				return this.fov;
			}
			set {
				fov = value;
			}
		}
		
		public State MoveState {
			get {
				return this.moveState;
			}
			set {
				moveState = value;
			}
		}	

		public bool PlayerDetected {
			get {
				return this.playerDetected;
			}
			set {
				playerDetected = value;
			}
		}
		
		public bool CanRoam {
			get {
				return this.canRoam;
			}
			set {
				canRoam = value;
			}
		}
		
		public bool CanSearch {
			get {
				return this.canSearch;
			}
			set {
				canSearch = value;
			}
		} 		
		public bool CanFollowPath {
			get {
				return this.canFollowPath;
			}
			set {
				canFollowPath = value;
			}
		}
		public int LayerArea {
			get {
				return this.layerArea;
			}
			set {
				layerArea = value;
			}
		}	
		#endregion

		#region Constructor
		public EnemyEntity()
		{
			SwitchType(EnemyType._Busy);
		}

		public EnemyEntity(EnemyType type)
		{
			SwitchType(type);
		}
		#endregion

		#region functions
		public EnemyType SwitchType(EnemyType tp)
		{
			switch (tp)
			{
				case EnemyType._Busy:
				{
					this.type = tp;				
					this.behaviour = EnemyBeHaviour.Idle;
					this.playerDetected = false;
					this.canRoam = false;
					this.canSearch = false;
					this.canFollowPath = false;
					break;
				}
				case EnemyType._Fixe:
				{
					this.type = tp;				
					this.behaviour = EnemyBeHaviour.Idle;
					this.playerDetected = false;
					this.canRoam = false;
					this.canSearch = true;
					this.canFollowPath = false;
					break;
				}
				case EnemyType._RoamingRandom:
				{
					this.type = tp;				
					this.behaviour = EnemyBeHaviour.Idle;
					this.playerDetected = false;
					this.canRoam = true;
					this.canSearch = true;
					this.canFollowPath = false;
					break;
				}
				case EnemyType._RoamingPath:
				{
					this.type = tp;				
					this.behaviour = EnemyBeHaviour.Idle;
					this.playerDetected = false;
					this.canRoam = true;
					this.canSearch = true;
					this.canFollowPath = true;
					break;
				}
			}
			return this.type;
		}
		public EnemyType returntype()
		{
			return this.type;
		}
		#endregion
	}
}
