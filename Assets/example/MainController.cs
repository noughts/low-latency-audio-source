using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour {

	public LowLatencyAudioSource audioSource;
	public AudioClip clip;

	// Use this for initialization
	void Start () {
		
	}


	public void loadSound(){
		audioSource.load (clip);
	}


	public void playSound(){
		audioSource.play (clip);
	}


	public void reloadScene(){
		SceneManager.LoadScene ("main");
	}

}
