using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LowLatencyAudioSource : MonoBehaviour {

	// ループするか？現状、途中での変更は対応していません
	public bool loop = false;
	public bool playOnAwake = false;

	/// ネイティブプラグインを使わない
	public bool forceUnitySoundSystem = false;


	public AudioClip clip;

	/// iOS、エディタ用のaudioSource;
	AudioSource audioSource;
	AndroidJavaObject soundObj;
	AndroidJavaObject mediaPlayer;

	Dictionary<string,int> soundIds = new Dictionary<string, int> ();

	/// 再開時に再生を再開するべきか？
	bool shouldResumeOnFocus = false;

	// mediaPlayerのcurrentPositionは不安定なことがあるので、それを補正するための変数
	int prevFramePosition = 0;
	float prevFrameTime = 0;
	float startTime = 0;

	#region delegates

	void Awake(){
//		print ("Awake");
		if( shouldUseAndroidPlugin () ){
			AndroidJavaClass unityActivityClass =  new AndroidJavaClass( "com.unity3d.player.UnityPlayer" );
			AndroidJavaObject activityObj = unityActivityClass.GetStatic<AndroidJavaObject>( "currentActivity" );
			soundObj = new AndroidJavaObject( "com.catsknead.androidsoundfix.AudioCenter", 20, activityObj );
			mediaPlayer = new AndroidJavaObject( "jp.dividual.MediaPlayerPlugin", activityObj );
			mediaPlayer.Call ("setLooping", new object[] { loop });
		} else {
			audioSource = gameObject.AddComponent<AudioSource> ();
			audioSource.loop = loop;
			audioSource.clip = clip;
		}

		if( clip != null ){
			load (clip);
		}
		if( playOnAwake ){
			Play ();
		}
	}

	// アプリが中断/再開されたとき。標準のAudioSourceはいい感じに処理するのでそれを真似る
	void OnApplicationFocus( bool status ){
//		print ("onFocus=" + status);
		if( shouldUseAndroidPlugin ()==false){
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
//		print ("OnDestroy");
		if( shouldUseAndroidPlugin () == false ){
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

	public void Stop(){
		if( shouldUseAndroidPlugin() ){
			mediaPlayer.Call ("stop");
		} else {
			audioSource.Stop ();
		}
	}

	public void Play(){
		if( clip == null ){
			Debug.LogWarning ("clipが登録されていません");
			return;
		}

		if( shouldUseAndroidPlugin() == false ){
			audioSource.Play ();
			return;
		}

		if( clip.length > 10 ){
			mediaPlayer.Call ("start");
			print ("Play!!!!!");
			startTime = Time.time;
		} else {
			PlayOneShot (clip);
		}
	}

	public void PlayOneShot( AudioClip clip ){
		PlayOneShot (clip, 1);
	}

	/// ボリュームを指定して再生
	public void PlayOneShot(AudioClip clip, float volume){
		if( shouldUseAndroidPlugin () ){
			if( soundIds.ContainsKey (clip.name) == false ){
				Debug.LogWarning (clip.name +"はまだロードされていません");
				return;
			}
			int soundId = soundIds [clip.name];
			soundObj.Call( "playSound", new object[] { soundId, volume } );
		} else {
			audioSource.PlayOneShot (clip, volume);
		}
	}


	public bool isPlaying{
		get{
			if( shouldUseAndroidPlugin () ){
				return mediaPlayer.Call<bool> ("isPlaying");
			} else {
				return audioSource.isPlaying;
			}
		}
	}

	public float time{
		get{
			if( shouldUseAndroidPlugin () ){
				if( isPlaying == false ){
					return 0;
				}

				// loopingの時に、ループしたタイミングでstartTimeをリセット
				int currentPosition = mediaPlayer.Call<int> ("getCurrentPosition");
				int gap = currentPosition - prevFramePosition;
				if( gap < 0 - clip.length*1000*0.8 ){
					print ("巻き戻します");
					startTime = Time.time;
				}
				prevFramePosition = currentPosition;
				float result = Time.time - startTime;

//				print (currentPosition + " / "+ (result*1000) +" gap=>"+ (currentPosition-(result*1000)));

				return result;
				/*
				float currentTime = Time.time;
				int currentPosition = mediaPlayer.Call<int> ("getCurrentPosition");
//				currentPosition = currentPosition + 50;// iOSの感覚と合わせる
				// 再生開始前はマイナスの大きな値が返ってくるので補正
				if( currentPosition < 0 ){
					currentPosition = 0;
				}

				int gap = currentPosition - prevFramePosition;
				if( -1000 < gap && gap < 0 ){
					currentPosition = prevFramePosition;
				}
				prevFramePosition = currentPosition;
				prevFrameTime = currentTime;
				return currentPosition / 1000f;
				*/
			} else {
				return audioSource.time;
			}
		}
	}


	#endregion







	#region public







	// SoundPoolは10秒以上の音声ファイルはロードできないので、長さによって使うモジュールを分ける
	public void load(AudioClip clip){
		if( clip.length > 10){
			loadMusic (clip);
		} else {
			loadSound (clip);
		}
	}





	#endregion


	// Android用にクリップをロード
	void loadSound(AudioClip clip){
		if( shouldUseAndroidPlugin () == false ){
			return;
		}
		if( soundIds.ContainsKey (clip.name) ){
			print ("すでに"+ clip.name +"は登録されています。");
			return;
		}
		string path = "Resources/Sounds/" + clip.name + ".wav";
		int soundId = soundObj.Call<int>( "loadSound", new object[] { path } );
		soundIds.Add (clip.name, soundId);
	}


	void loadMusic(AudioClip clip){
		if( shouldUseAndroidPlugin () == false ){
			return;
		}
		string path = "Resources/Sounds/" + clip.name + ".wav";
		print ("MediaPlayerに"+ path +"をロードします");
		mediaPlayer.Call( "load", new object[] { path } );
	}




	bool shouldUseAndroidPlugin(){
		if( forceUnitySoundSystem ){
			return false;
		}

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
