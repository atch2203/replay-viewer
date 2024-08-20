using Godot;
using System;

public partial class BeatObject : StaticBody3D
{

	float beat, duration;
	int x, y;

	protected BeatObject(float beat, float duration, int x, int y){
		this.beat = beat;
		this.duration = duration;
		this.x = x;
		this.y = y;
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
