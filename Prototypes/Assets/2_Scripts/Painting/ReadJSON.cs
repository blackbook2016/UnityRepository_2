namespace TheVandals
{
	using System.Collections.Generic;
	using System.Collections;
	using System.IO;
	using SimpleJSON;
	using UnityEngine;

	public class ReadJSON  {

		private string fileName = "PaintingInfo";
		private string data;

		private List<PaintingEntity> paintings = new List<PaintingEntity>();

		public ReadJSON ()
		{
		}
		

		public List<PaintingEntity> ReadJson () 
		{
//			StreamReader sr = new StreamReader(Application.dataPath + "/Resources/" + fileName);
//			data = sr.ReadToEnd();
//			sr.Close();
			TextAsset data= Resources.Load(fileName) as TextAsset;
			JSONNode json = JSON.Parse(data.text);
//			print (json.Count);



			for(int i = 0; i < json.Count; i++)
			{
				PaintingEntity tempPainting = new PaintingEntity();

				tempPainting.Title = json[i]["Title"].Value;
				tempPainting.Artist = json[i]["Artist"].Value;
				tempPainting.Info = json[i]["Info"].Value;
				tempPainting.SpriteName = json[i]["Sprite"].Value;
				tempPainting.TextureName = json[i]["Texture"].Value;

				paintings.Add(tempPainting);
			}
			return paintings;
		}
	}
}