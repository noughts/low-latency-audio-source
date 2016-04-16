using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class BGMController : MonoBehaviour {
	
	public LowLatencyAudioSource audioSource;
	public Text status_txt;

	// Use this for initialization
	void Start () {
	
	}
	

	void Update(){
		status_txt.text = "time="+ audioSource.time;
	}
		
	public void playMusic(){
		audioSource.Play ();
	}

}
