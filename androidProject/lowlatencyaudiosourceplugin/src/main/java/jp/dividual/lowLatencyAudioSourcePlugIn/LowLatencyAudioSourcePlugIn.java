package jp.dividual.lowLatencyAudioSourcePlugIn;

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
		} catch( IOException e ) {
			Log.e( "SoundPluginUnity", "File does not exist!" );
			return;
		}
	}
}
