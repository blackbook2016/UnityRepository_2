namespace TheVandals
{
	using System.Collections;
	using UnityEngine;

	public class PaintingEntity 
	{		
		#region Properties
		private string title;
		private string artist;
		private string info;
		private string spriteName;
		private string textureName;

		private GameObject paintingObject;
		private bool isCaptured;
		#endregion

		#region Getters/Setters
		public string Title {
			get {
				return this.title;
			}
			set {
				title = value;
			}
		}
		
		public string Artist {
			get {
				return this.artist;
			}
			set {
				artist = value;
			}
		}
		
		public string Info {
			get {
				return this.info;
			}
			set {
				info = value;
			}
		}
		
		public string SpriteName {
			get {
				return this.spriteName;
			}
			set {
				spriteName = value;
			}
		}
		
		public string TextureName {
			get {
				return this.textureName;
			}
			set {
				textureName = value;
			}
		}		
		
		public GameObject PaintingObject {
			get {
				return this.paintingObject;
			}
			set {
				paintingObject = value;
			}
		}

		public bool IsCaptured {
			get {
				return this.isCaptured;
			}
			set {
				isCaptured = value;
			}
		}
		#endregion

		#region Constructors
		public PaintingEntity (string title, string artist, string info, string spriteName, string textureName)
		{
			this.title = title;
			this.artist = artist;
			this.info = info;
			this.spriteName = spriteName;
			this.textureName = textureName;
			this.paintingObject = null;
			this.isCaptured = false;
		}
		public PaintingEntity (PaintingEntity pe)
		{
			this.title = pe.Title;
			this.artist = pe.Artist;
			this.info = pe.Info;
			this.spriteName = pe.SpriteName;
			this.textureName = pe.TextureName;
			this.paintingObject = pe.paintingObject;
			this.isCaptured = pe.isCaptured;
		}
		public PaintingEntity (){}
		#endregion

		#region Public
		public void Clear()
		{
			this.title = "";
			this.artist = "";
			this.info = "";
			this.spriteName = "";
			this.textureName = "";

			this.paintingObject = null;
			isCaptured = false;
		}
		#endregion
	}
}