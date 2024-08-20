using Godot;
using Godot.Collections;
using System;

public partial class Beat : BeatObject
{
	public enum Beat_Color
	{
		LEFT = 0, RIGHT = 1
	}
	public enum Beat_Direction
	{
		UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3, UPLEFT = 4, UPRIGHT = 5, DOWNLEFT = 6, DOWNRIGHT = 7, ANY = 8
	}

	Beat_Color color;
	Beat_Direction dir;
	float angle_offset;
	float scoring_type;

	public Beat(float beat, int x, int y, Beat_Color color, Beat_Direction direction, float angle_offset, float scoring_type): base(beat, 0, x, y){
		this.color = color;
		this.dir = direction;
		this.angle_offset = angle_offset;
		this.scoring_type = scoring_type;
	}

	public static Beat makeBeat(bool version3, Dictionary json){
		Beat b = version3 ?
			new Beat(json["b"].As<float>(), json["x"].As<int>(), json["y"].As<int>(), json["c"].As<Beat_Color>(), json["d"].As<Beat_Direction>(), json["a"].As<float>(), 0) :
			new Beat(json["_time"].As<float>(), json["_lineIndex"].As<int>(), json["_lineLayer"].As<int>(), json["_type"].As<Beat_Color>(), json["_cutDirection"].As<Beat_Direction>(), 0, 0);
		return b;
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
