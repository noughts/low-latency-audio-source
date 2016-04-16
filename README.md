# HOW TO INSTALL

1. Unity プロジェクトの Assets/ に package.json を作成します。
2. 以下を記述します。
```json
{
	"dependencies": {
		"low-latency-audio-source": "noughts/low-latency-audio-source"
	}
}
```
3. Assets/ で npm install します。



# HOW TO USE

AudioSource と同じように使います。
エディタや iOS は Resources/Sounds/ 以下を使い、
Android は StreamingAssets/Resources/Sounds/ 以下の mp3 を使うので、
Resources/Sounds/ 内の音楽ファイルをすべて mp3 に変換し、StreamingAssets/Resources/Sounds/ に置きましょう。
