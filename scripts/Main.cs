using Godot;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public partial class Main : Node3D {

	Ui UI;
	Replay r;
	float t = 0;
	int frame = 0;
	float speed = 1F;
	MapFolder b;
	Node3D headset;


	Previewer previewer;
	bool isReplayMode = false;
	TimeManager timeManager = new TimeManager();
	PackedScene bombScene = ResourceLoader.Load<PackedScene>("res://gameobjects/bomb.tscn");

	public override void _Ready() {
		headset = GetNode("Quest") as Node3D;
		headset.Position = new Vector3(0, 1.5F, 0);
		// headset.QueueFree();
		// RemoveChild(headset);

		b = JsonReader.makeMapFolder("C:\\Users\\atch2\\Documents\\ReplayViewer\\maps\\298b5 (Last Wish - BSWC Team)");

		UI = (Ui)ResourceLoader.Load<PackedScene>("res://ui.tscn").Instantiate();
		AddChild(UI);
		UI.initializeTimeManager(timeManager);
		UI.initializeAudio("res://maps/298b5 (Last Wish - BSWC Team)/song.ogg");
		UI.songManager.setSpeed(1F);
		GD.Print(b.mapInfo.difficultyBeatmaps[3].getHJD());
		b.mapInfo.difficultyBeatmaps[3].hjd = 3;
		previewer = new Previewer(b.mapInfo.difficultyBeatmaps[3]);
		AddChild(previewer);

		// using Task<Replay> fetcher = ReplayLoader.ReplayFromDirectory("C:\\Users\\atch2\\Documents\\ReplayViewer\\maps\\76561198246352688-Last Wish-Expert-Standard-C86336B3CA84CD03BC3995FADFD7CFDDE2FD00C0-1723756781.bsor");

		r = ReplayDecoder.Decode(File.ReadAllBytes("C:\\Users\\atch2\\Documents\\ReplayViewer\\maps\\76561198246352688-Last Wish-Expert-Standard-C86336B3CA84CD03BC3995FADFD7CFDDE2FD00C0-1723756781.bsor"));

		UI.TogglePause();
	}

	public override void _Process(double delta) {
		if (isReplayMode) {
			t += (float)delta / 3;
			while(r.frames[frame].time < t) frame++;
			GD.Print($"frametime {r.frames[frame].time} time {t}");
			// noteObj.update(r.frames[frame].time);
		}else{
			previewer.goTo(timeManager.time);
			// noteObj.update(timeManager.time);
		}
	}

	public override void _Input(InputEvent @event) {
		if (@event.IsActionPressed("togglePause")) {
			UI.TogglePause();
		}
		if(Input.IsKeyPressed(Key.Equal)){
			speed += 0.1F;
			UI.songManager.setSpeed(speed);
		}
		if(Input.IsKeyPressed(Key.Minus)){
			speed -= 0.1F;
			UI.songManager.setSpeed(speed);
		}
	}
}
