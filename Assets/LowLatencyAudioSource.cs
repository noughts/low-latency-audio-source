using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LowLatencyAudioSource : MonoBehaviour {

	/// iOS、エディタ用のaudioSource;
	AudioSource audioSource;
	AndroidJavaObject soundObj;

	Dictionary<string,int> soundIds = new Dictionary<string, int> ();



	void Awake(){
		if( Application.platform == RuntimePlatform.Android ){
			AndroidJavaClass unityActivityClass =  new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
			AndroidJavaObject activityObj = unityActivityClass.GetStatic<AndroidJavaObject>( "currentActivity" );
			soundObj = new AndroidJavaObject( "com.catsknead.androidsoundfix.AudioCenter", 20, activityObj );
		} else {
			audioSource = gameObject.AddComponent<AudioSource> ();
		}
	}



	// Android用にクリップをロード
	public void load(AudioClip clip){
		if( soundIds.ContainsKey (clip.name) ){
			print ("すでに"+ clip.name +"は登録されています。");
			return;
		}
		int soundId = soundObj.Call<int>( "loadSound", new object[] { "Resources/Sounds/" +  clip.name + ".wav" } );
		soundIds.Add (clip.name, soundId);
	}


	/// 再生
	public void play(AudioClip clip){
		if( Application.platform == RuntimePlatform.Android ){
			if( soundIds.ContainsKey (clip.name) == false ){
				print (clip.name +"はまだロードされていません");
				return;
			}
			int soundId = soundIds [clip.name];
			soundObj.Call( "playSound", new object[] { soundId } );
		} else {
			audioSource.PlayOneShot (clip);
		}
	}


}
