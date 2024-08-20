using Godot;
using System;
using System.Text.Json;

public partial class Main : Node3D
{

	[Export]
	public PackedScene BombScene {get; set;}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Mapfile b = new Mapfile("C:\\Users\\atch2\\Documents\\ReplayViewer\\maps\\298b5 (Last Wish - BSWC Team)");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void OnTimerTimeout(){
		Bomb bomb = BombScene.Instantiate<Bomb>();

		bomb.initialize(0);

		AddChild(bomb);
	}
}
