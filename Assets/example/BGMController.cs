using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class BGMController : MonoBehaviour {
	
	public LowLatencyAudioSource audioSource;
	public Text status_txt;

	float prevTime = -1;

	// Use this for initialization
	void Start () {
	
	}
	

	void Update(){
		float currentTime = audioSource.time;
		if( currentTime < prevTime ){
			print ("timeが巻き戻ってます!!!");
			print (prevTime + " => "+ currentTime);
		}
		status_txt.text = "time="+ currentTime;
		prevTime = currentTime;
	}
		
	public void playMusic(){
		audioSource.Play ();
	}

}
