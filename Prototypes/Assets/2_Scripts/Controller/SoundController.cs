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
		[SerializeField]
		private AudioClip sprayClip;
		[SerializeField]
		private AudioClip rechargeSprayClip;

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
		public void PlayClip(string clipName, bool loop)
		{
			audioSource.loop = loop;
			SetClip(clipName);
			audioSource.Play();
		}
		public void PlayClip(string clipName)
		{
			audioSource.loop = false;
			SetClip(clipName);
			audioSource.Play();
		}
		public void StopClip()
		{
			audioSource.Stop();
		}

		public bool IsPlaying()
		{
			return audioSource.isPlaying;
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
			case "sprayClip":
			{
				audioSource.clip = sprayClip;
				break;
			}
			case "rechargeSprayClip":
			{
				audioSource.clip = rechargeSprayClip;
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
