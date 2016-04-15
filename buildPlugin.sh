cd androidProject
./gradlew jar

from="/Users/noughts/Dropbox/works/libs/unity/LowLatencyAudioSource/androidProject/lowlatencyaudiosourceplugin/build/libs/LowLatencyAudioSourcePlugIn-1.0.0.jar"
to="/Users/noughts/Dropbox/works/libs/unity/LowLatencyAudioSource/Assets/Plugins/Android/"
cp ${from} ${to}

cd ..
