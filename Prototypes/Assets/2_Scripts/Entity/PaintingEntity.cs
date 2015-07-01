namespace TheVandals
{
	using System.Collections;

	public class PaintingEntity 
	{		
		private string title;
		private string artist;
		private string info;
		private string spriteName;
		private string textureName;
		
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

		public PaintingEntity (string title, string artist, string info, string spriteName, string textureName)
		{
			this.title = title;
			this.artist = artist;
			this.info = info;
			this.spriteName = spriteName;
			this.textureName = textureName;
		}
		public PaintingEntity (PaintingEntity pe)
		{
			this.title = pe.Title;
			this.artist = pe.Artist;
			this.info = pe.Info;
			this.spriteName = pe.SpriteName;
			this.textureName = pe.TextureName;
		}
		public PaintingEntity (){}

		public void Clear()
		{
			this.title = "";
			this.artist = "";
			this.info = "";
			this.spriteName = "";
			this.textureName = "";
		}
	}
}