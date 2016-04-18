using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class BGMController : MonoBehaviour {
	
	public LowLatencyAudioSource audioSource;
	public Text status_txt;
	public GameObject cube;


	// Use this for initialization
	void Start () {
	
	}
	

	void Update(){
		float currentTime = audioSource.time;
		status_txt.text = "time="+ currentTime;

		float posX = currentTime*5 % 10;
		cube.transform.position = new Vector3 (posX,0);
	}
		
	public void playMusic(){
		audioSource.Play ();
	}

}
