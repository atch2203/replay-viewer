using Godot;
using Godot.Collections;
using System;

public partial class Bomb : BeatObject {

	float ticks = 5;

	public Bomb(float beat, int x, int y) : base(beat, 0, x, y) {
	}
	public Bomb() : base(0, 0, 0, 0) {

	}

	public static Bomb makeBomb(bool version3, Dictionary json) {
		Bomb b = version3 ?
			new Bomb(json["b"].As<float>(), json["x"].As<int>(), json["y"].As<int>()) :
			new Bomb(json["_time"].As<float>(), json["_lineIndex"].As<int>(), json["_lineLayer"].As<int>());
		return b;
	}

	public static Bomb a() {
		return new Bomb(1, 1, 1);
	}

	public void initialize(int x) {
		ticks = x;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		Position = new Vector3(0, 0, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		Vector3 newPos = Position;
		newPos.X += ticks / 100;
		Position = newPos;
		ticks++;
		if (ticks > 100) {
			QueueFree();
		}
	}
}
