using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LowLatencyAudioSource : MonoBehaviour {

	/// iOS、エディタ用のaudioSource;
	AudioSource audioSource;
	AndroidJavaObject soundObj;
	AndroidJavaObject mediaPlayer;

	Dictionary<string,int> soundIds = new Dictionary<string, int> ();

	/// 再開時に再生を再開するべきか？
	bool shouldResumeOnFocus = false;


	#region delegates

	void Awake(){
//		print ("Awake");
		if( onAndroidDevice () ){
			AndroidJavaClass unityActivityClass =  new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
			AndroidJavaObject activityObj = unityActivityClass.GetStatic<AndroidJavaObject>( "currentActivity" );
			soundObj = new AndroidJavaObject( "com.catsknead.androidsoundfix.AudioCenter", 20, activityObj );
			mediaPlayer = new AndroidJavaObject( "jp.dividual.MediaPlayerPlugin", activityObj );
		} else {
			audioSource = gameObject.AddComponent<AudioSource> ();
		}
	}

	// アプリが中断/再開されたとき。標準のAudioSourceはいい感じに処理するのでそれを真似る
	void OnApplicationFocus( bool status ){
		print ("onFocus=" + status);
		if( onAndroidDevice ()==false){
			return;
		}
		if( status == false ){
			print ("アプリが中断しました");
			if( isPlaying ){
				print ("現在再生中なので、再開時に再生を再開するようフラグを立てます");
				shouldResumeOnFocus = true;
				mediaPlayer.Call( "pause" );
			}
		} else {
			print ("アプリが再開しました shouldResumeOnFocus="+ shouldResumeOnFocus);
			if( shouldResumeOnFocus ){
				shouldResumeOnFocus = false;
				mediaPlayer.Call( "start" );
			}
		}
	}



	void OnDestroy(){
		print ("OnDestroy");
		if( onAndroidDevice () == false ){
			return;
		}
		print ("このAudioSourceにロード済みの全てのサウンドをunloadします");
		foreach (KeyValuePair<string, int> pair in soundIds) {
			soundObj.Call( "unloadSound", new object[] { pair.Value } );
		}
		mediaPlayer.Call( "stop" );
	}

	#endregion





	#region AudioSourceと同じ機能

	public bool isPlaying{
		get{
			if( onAndroidDevice () ){
				return mediaPlayer.Call<bool> ("isPlaying");
			} else {
				return audioSource.isPlaying;
			}
		}
	}

	public float time{
		get{
			if( onAndroidDevice () ){
				int val = mediaPlayer.Call<int> ("getCurrentPosition");
				// 再生開始前はマイナスの大きな値が返ってくるので補正
				if( val < 0 ){
					val = 0;
				}
				return val / 1000f;
			} else {
				return audioSource.time;
			}
		}
	}

	#endregion







	#region public

	// Android用にクリップをロード
	public void load(AudioClip clip){
		if( onAndroidDevice () == false ){
			return;
		}
		if( soundIds.ContainsKey (clip.name) ){
			print ("すでに"+ clip.name +"は登録されています。");
			return;
		}
		int soundId = soundObj.Call<int>( "loadSound", new object[] { "Resources/Sounds/" +  clip.name + ".mp3" } );
		soundIds.Add (clip.name, soundId);
	}

	public void loadMusic(AudioClip clip){
		if( onAndroidDevice () == false ){
			return;
		}
		mediaPlayer.Call( "load", new object[] { "Resources/Sounds/" +  clip.name + ".mp3" } );
	}
	public void playMusic( AudioClip clip ){
		if (onAndroidDevice ()) {
			mediaPlayer.Call( "start" );
		} else {
			audioSource.clip = clip;
			audioSource.Play ();// playOneShotを使うとtimeがとれないのでこうしてます。
		}
	}


	/// 再生
	public void play(AudioClip clip){
		play (clip, 1);
	}

	/// ボリュームを指定して再生
	public void play(AudioClip clip, float volume){
		if( onAndroidDevice () ){
			if( soundIds.ContainsKey (clip.name) == false ){
				print (clip.name +"はまだロードされていません");
				return;
			}
			int soundId = soundIds [clip.name];
			soundObj.Call( "playSound", new object[] { soundId, volume } );
		} else {
			audioSource.PlayOneShot (clip, volume);
		}
	}



	#endregion







	bool onAndroidDevice(){
		#if UNITY_EDITOR
		return false;
		#endif

		if( Application.platform == RuntimePlatform.Android ){
			return true;
		} else {
			return false;
		}
	}

}
