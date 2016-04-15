package jp.dividual;

import android.app.Activity;
import android.content.res.AssetFileDescriptor;
import android.media.MediaPlayer;
import android.util.Log;
import android.widget.Toast;
import com.unity3d.player.UnityPlayer;

import java.io.IOException;

public class LowLatencyAudioSourcePlugin {

	private Activity _activity;
	private MediaPlayer _mediaPlayer;

	public LowLatencyAudioSourcePlugin(Activity activity ){
		_activity = activity;
		_mediaPlayer = new MediaPlayer();
	}


	public void loadSound( String soundName ){
		AssetFileDescriptor afd;

		try {
			afd = _activity.getAssets().openFd( soundName );
			_mediaPlayer.setDataSource(afd.getFileDescriptor(), afd.getStartOffset(), afd.getLength());// 音楽ファイルをmediaplayerに設定

			// レイテンシを下げるためにプリロード
			_mediaPlayer.start();
			_mediaPlayer.pause();
		} catch( IOException e ) {
			Log.e("LLASP", "File does not exist!");
		}
	}

	public void start(){
		_mediaPlayer.start();
	}

	public void stop(){
		_mediaPlayer.stop();
	}

}
