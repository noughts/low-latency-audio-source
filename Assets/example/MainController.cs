using UnityEngine;
using System.Collections;

public class MainController : MonoBehaviour {

	public LowLatencyAudioSource audioSource;
	public AudioClip clip;

	// Use this for initialization
	void Start () {
		audioSource.load (clip);
	}

	// Update is called once per frame
	void Update () {
	
	}


	public void playSound(){
		audioSource.play (clip);
	}


}
