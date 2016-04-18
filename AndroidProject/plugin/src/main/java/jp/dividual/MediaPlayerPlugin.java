package jp.dividual;

import android.app.Activity;
import android.content.res.AssetFileDescriptor;
import android.media.MediaPlayer;
import android.util.Log;

import java.io.IOException;


public class MediaPlayerPlugin extends MediaPlayer {

    private Activity _activity;



    public MediaPlayerPlugin( Activity activity ){
        super();
        _activity = activity;
    }


    public void load( String soundName ){
        AssetFileDescriptor afd;

        try {
            afd = _activity.getAssets().openFd( soundName );
            setDataSource(afd.getFileDescriptor(), afd.getStartOffset(), afd.getLength());// 音楽ファイルをmediaplayerに設定
            prepare();

            // レイテンシを下げるためにプリロード
			start();
			pause();
        } catch( IOException e ) {
            Log.e("LLASP", "File does not exist!");
            Log.e("LLASP", e.toString());
        }
    }



}
