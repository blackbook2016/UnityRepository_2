namespace TheVandals
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using System.Collections.Generic;

	public class PaintingManager : MonoBehaviour 
	{
		private static PaintingManager _instance;
		public static PaintingManager instance
		{
			get
			{
				if(_instance == null)
					_instance = GameObject.FindObjectOfType<PaintingManager>();
				return _instance;
			}
		}

		public List<Sprite> paintingsList = new List<Sprite>();
		public Text text_Title;
		public Text text_Info;

		private List<PaintingEntity> paintings = new List<PaintingEntity>();
		private TextAsset paintingInfo;

		void Awake() 
		{
			ReadJSON rj = GetComponent<ReadJSON>();
			paintings = rj.ReadJson();
		}
		void Start()
		{ 
			ShowText("banksy");
		}

		public Sprite PaintingSprite(string paintingName)
		{
			return paintingsList.Find(x => x.name.Equals(paintingName +"_Sprite"));
		}

		public void ShowText(string paintingName)
		{
			PaintingEntity tempP;
			try {
				tempP = new PaintingEntity(paintings.Find(x => x.SpriteName.Equals(paintingName)));
			} catch {
				print("Couldn't find painting Check Json name!");
				return;
			}

			text_Title.text = tempP.Title;
			text_Info.text = tempP.Info;
			text_Info.transform.parent.gameObject.SetActive(true);
		}

		public void RemoveText()
		{
			text_Info.transform.parent.gameObject.SetActive(false);
		}
	}
}
