using UnityEngine;
using System.Collections;

public class LowLatencyAudioSource : MonoBehaviour {

	/// iOS、エディタ用のaudioSource;
	AudioSource audioSource;

	void Awake(){
		audioSource = gameObject.AddComponent<AudioSource> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	

	public void play(AudioClip clip){
		audioSource.PlayOneShot (clip);
	}


}
