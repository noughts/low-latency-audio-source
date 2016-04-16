using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour {

	public LowLatencyAudioSource audioSource;
	public AudioClip sound;



	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 60;
	}



	public void loadSound(){
		audioSource.load (sound);
	}


	public void playSound(){
		audioSource.PlayOneShot (sound);
	}




	public void reloadScene(){
		SceneManager.LoadScene ("main");
	}

}
