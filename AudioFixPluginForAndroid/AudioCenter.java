package com.catsknead.androidsoundfix;

import android.app.Activity;
import android.content.res.AssetFileDescriptor;
import android.media.AudioManager;
import android.media.SoundPool;
import android.util.Log;

import java.io.IOException;
import java.util.HashSet;
import java.util.Set;

public class AudioCenter extends SoundPool
{
    private final Activity activity;
    private Set<Integer> soundsSet = new HashSet<Integer>();

    public AudioCenter( int maxStreams, Activity activity )
    {
        super( maxStreams, AudioManager.STREAM_MUSIC, 0 );
        this.activity = activity;

        setOnLoadCompleteListener( new OnLoadCompleteListener() {
            @Override
            public void onLoadComplete( SoundPool soundPool, int sampleId, int status ) {
                soundsSet.add( sampleId );
            }
        } );
    }

    public void play( int soundID )
    {
        if( soundsSet.contains( soundID ) ) {
            play( soundID, 1, 1, 1, 0, 1f );
        }
    }

    public void playSound( int soundId )
    {
        if( ( !soundsSet.contains( soundId ) ) || ( soundId == 0 ) ) {
            Log.e( "SoundPluginUnity", "File has not been loaded!" );
            return;
        }

        final int sKey = soundId;

        activity.runOnUiThread( new Runnable() {
            public void run() {
                play( sKey );
            }
        } );
    }
	
	public void playSound( int soundId, float volume ) {
		if( ( !soundsSet.contains( soundId ) ) || ( soundId == 0 ) ) {
			Log.e( "SoundPluginUnity", "File has not been loaded!" );
			return;
		}
 
		final int sKey = soundId;
		final float sVolume = volume;
 
		activity.runOnUiThread( new Runnable() {
			public void run() {
				 play( sKey, sVolume, sVolume, 1, 0, 1f );
			}
		} );
	}
   
	public void playSound( int soundId, float leftVolume, float rightVolume, int priority, int loop, float rate ) {
	   if( ( !soundsSet.contains( soundId ) ) || ( soundId == 0 ) ) {
			Log.e( "SoundPluginUnity", "File has not been loaded!" );
			return;
		}
 
		final int sKey = soundId;
		final float lVolume = leftVolume;
		final float rVolume = rightVolume;
		final int sPriority = priority;
		final int sLoop = loop;
		final float sRate = rate;
 
		activity.runOnUiThread( new Runnable() {
			public void run() {
				 play( sKey, lVolume, rVolume, sPriority, sLoop, sRate );
			}
		} );
	}

    public int loadSound( String soundName )
    {
        AssetFileDescriptor afd;

        try {
            afd = activity.getAssets().openFd( soundName );
        } catch( IOException e ) {
            Log.e( "SoundPluginUnity", "File does not exist!" );
            return -1;
        }

        int soundId = load( afd, 1 );

        soundsSet.add( soundId );

        return soundId;
    }

    public void unloadSound( int soundId )
    {
        if( unload( soundId ) ) {
            soundsSet.remove( soundId );
        } else {
            Log.e( "SoundPluginUnity", "File has not been loaded!" );
        }
    }
}
