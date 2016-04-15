androidPath="/Users/noughts/android-sdk-macosx/platforms/android-23/android.jar"
unityClassesPath="/Applications/Unity/PlaybackEngines/AndroidPlayer/Variations/mono/Release/Classes/classes.jar"

javac -source 1.7 -target 1.7 MyMainActivity.java -bootclasspath ${androidPath} -classpath ${unityClassesPath} -d .
jar cvf Assets/Plugins/Android/MyMainActivity.jar jp
