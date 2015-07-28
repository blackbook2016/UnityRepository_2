namespace TheVandals
{
	using UnityEngine;
	using System.Collections;

	[RequireComponent (typeof (AudioSource), typeof (AudioListener))]
	public class SoundController : MonoBehaviour {

		#region Properties
		private static SoundController _instance;
		public static SoundController instance
		{
			get
			{
				if(_instance == null)
					_instance = GameObject.FindObjectOfType<SoundController>();
				return _instance;
			}
		}

		[SerializeField]
		private AudioClip shoutClip;
		[SerializeField]
		private AudioClip runClip;
		[SerializeField]
		private AudioClip takePicClip;

		private AudioSource audioSource;
//		private AudioListener audioListener;
		#endregion

		#region Unity
		void Awake()
		{
			audioSource = GetComponent<AudioSource>();
//			audioListener = GetComponent<AudioListener>();
		}
		#endregion

		#region API
		public void PlayClip(string clipName)
		{
			SetClip(clipName);
			audioSource.Play();
		}
		#endregion

		#region Private
		private void SetClip(string clipName)
		{			
			switch(clipName)
			{
			case "shoutClip":
			{
				audioSource.clip = shoutClip;
				break;
			}
			case "runClip":
			{
				audioSource.clip = runClip;
				break;
			}
			case "takePicClip":
			{
				audioSource.clip = takePicClip;
				break;
			}
			default:
				print ("Incorrect SoundClip: " + clipName);
				break;
			}
		}
		#endregion

	}
}
