using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LowLatencyAudioSource : MonoBehaviour {

	// ループするか？現状、途中での変更は対応していません
	public bool loop = false;
	public bool playOnAwake = false;


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

	#region delegates

	void Awake(){
//		print ("Awake");
		if( onAndroidDevice () ){
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
//		print ("OnDestroy");
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
				float currentTime = Time.time;
				int currentPosition = mediaPlayer.Call<int> ("getCurrentPosition");
				// 再生開始前はマイナスの大きな値が返ってくるので補正
				if( currentPosition < 0 ){
					currentPosition = 0;
				}
					
				if( currentPosition < prevFramePosition ){
					Debug.LogWarning ("時間が巻き戻ってるので調整します! "+ prevFramePosition +" => "+ currentPosition );
					float gapTime = currentTime - prevFrameTime;
					currentPosition = prevFramePosition + (int)(gapTime * 1000);
				}
				print ("経過時間:"+ (currentPosition - prevFramePosition));

				prevFramePosition = currentPosition;
				prevFrameTime = currentTime;
				return currentPosition / 1000f;
			} else {
				return audioSource.time;
			}
		}
	}

	#endregion







	#region public

	public void Play(){
		if( clip == null ){
			Debug.LogWarning ("clipが登録されていません");
			return;
		}

		if( onAndroidDevice() == false ){
			audioSource.Play ();
			return;
		}

		if( clip.length > 10 ){
			mediaPlayer.Call ("start");
		} else {
			PlayOneShot (clip);
		}
	}



	public void PlayOneShot( AudioClip clip ){
		PlayOneShot (clip, 1);
	}

	/// ボリュームを指定して再生
	public void PlayOneShot(AudioClip clip, float volume){
		if( onAndroidDevice () ){
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
		if( onAndroidDevice () == false ){
			return;
		}
		if( soundIds.ContainsKey (clip.name) ){
			print ("すでに"+ clip.name +"は登録されています。");
			return;
		}
		string path = "Resources/Sounds/" + clip.name + ".mp3";
		int soundId = soundObj.Call<int>( "loadSound", new object[] { path } );
		soundIds.Add (clip.name, soundId);
	}


	void loadMusic(AudioClip clip){
		if( onAndroidDevice () == false ){
			return;
		}
		string path = "Resources/Sounds/" + clip.name + ".mp3";
		print ("MediaPlayerに"+ path +"をロードします");
		mediaPlayer.Call( "load", new object[] { path } );
	}




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
