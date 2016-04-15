﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour {

	public LowLatencyAudioSource audioSource;
	public AudioClip sound;
	public AudioClip music;

	// Use this for initialization
	void Start () {
		
	}


	public void loadSound(){
		audioSource.load (sound);
	}


	public void playSound(){
		audioSource.play (sound);
	}


	public void loadMusic(){
		audioSource.loadMusic (music);
	}

	public void playMusic(){
		audioSource.playMusic (music);
	}


	public void reloadScene(){
		SceneManager.LoadScene ("main");
	}

}
