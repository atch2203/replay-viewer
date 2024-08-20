using Godot;
using System;
using System.Text.Json;

public partial class Main : Node3D
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MapFolder b = JsonReader.makeMapFolder("C:\\Users\\atch2\\Documents\\ReplayViewer\\maps\\298b5 (Last Wish - BSWC Team)");
		// GD.Print(b.mapInfo);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
