using Godot;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public partial class Main : Node3D {

	Ui UI;
	NoteManager noteMan;
	Replay r;
	float t= 0;
	int frame = 0;
	MapFolder b;
	Node3D headset;

	// Called when the node enters the scene tree for the first time.
	public override async void _Ready() {
		headset = GetNode("Quest") as Node3D;
		b = JsonReader.makeMapFolder("C:\\Users\\atch2\\Documents\\ReplayViewer\\maps\\298b5 (Last Wish - BSWC Team)");
		// AudioManager song = new AudioManager("res://maps/31d13 (Luminency - Fnyt)/song.ogg");
		// AddChild(song);
		// song.Play();
		UI = (Ui)ResourceLoader.Load<PackedScene>("res://ui.tscn").Instantiate();
		AddChild(UI);
		UI.initializeAudio("res://maps/31d13 (Luminency - Fnyt)/song.ogg");

		GD.Print(JsonSerializer.Serialize(b.mapInfo.difficultyBeatmaps[0].map.colorNotes[0].d, JsonReader.options));
		GD.Print(b.mapInfo.difficultyBeatmaps[0].map.colorNotes[0].d);
		BeatMap.Note testNote = new BeatMap.Note{
			b=5,
			x=0,
			y=2,
			a=0,
			c=BeatMap.SaberColor.LEFT,
			d=BeatMap.CutDirection.UP
		};
		// using Task<Replay> fetcher = ReplayLoader.ReplayFromDirectory("C:\\Users\\atch2\\Documents\\ReplayViewer\\maps\\76561198246352688-Last Wish-Expert-Standard-C86336B3CA84CD03BC3995FADFD7CFDDE2FD00C0-1723756781.bsor");

		r = ReplayDecoder.Decode(File.ReadAllBytes("C:\\Users\\atch2\\Documents\\ReplayViewer\\maps\\76561198246352688-Last Wish-Expert-Standard-C86336B3CA84CD03BC3995FADFD7CFDDE2FD00C0-1723756781.bsor"));
		// GD.Print(r);
		noteMan = (NoteManager)ResourceLoader.Load<PackedScene>("res://gameobjects/arrowbeat.tscn").Instantiate();
		noteMan.initialize(b.mapInfo.difficultyBeatmaps[0], testNote);
		// NoteManager noteManager = new NoteManager(b.mapInfo.difficultyBeatmaps[0], testNote);
		AddChild(noteMan);
		UI.TogglePause();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		t += (float)delta/10;
		while(r.frames[frame].time < t) frame++;
		GD.Print($"frametime {r.frames[frame].time} time {t}");
		if(t > noteMan.singleNoteMovementManager.self.b * 60 / b.mapInfo.audio.bpm + noteMan.singleNoteMovementManager.movementData.jumpDuration/2){
			t = 0;
			frame = 0;
		}
		GD.Print($"leftX {r.frames[frame].leftHand.position.Z} rightX {r.frames[frame].rightHand.position.Z}");
		// headset.Position = r.frames[frame].head.position;
		headset.Rotation = r.frames[frame].head.rotation.GetEuler();

		noteMan.update(r.frames[frame].time);
	}

	public override void _Input(InputEvent @event) {
		if(@event.IsActionPressed("togglePause")){
			UI.TogglePause();
		}
	}
}
