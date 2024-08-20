using Godot;
using Godot.Collections;
using System;

public partial class Wall : BeatObject {

  int width, height;

  public Wall(float beat, float duration, int x, int y, int width, int height) : base(beat, duration, x, y) {
    this.width = width;
    this.height = height;
  }

  public static Wall makeWall(bool version3, Dictionary json) {
    Wall w;
    if (version3) {
      w = new Wall(json["b"].As<float>(), json["d"].As<float>(), json["x"].As<int>(), json["y"].As<int>(), json["w"].As<int>(), json["h"].As<int>());
    } else {
      int y, h;
      if (json.ContainsKey("_type")) {
        switch (json["_type"].As<int>()) {
          case 0:
            y = 0;
            h = 5;
            break;
          case 1:
            y = 2;
            h = 3;
            break;
          default:
            y = json["_lineLayer"].As<int>();
            h = json["_height"].As<int>();
            break;
        }
      } else {
        y = json["_lineLayer"].As<int>();
        h = json["_height"].As<int>();
      }
      w = new Wall(json["_time"].As<float>(), json["_duration"].As<float>(), json["_lineIndex"].As<int>(), y, json["_width"].As<int>(), h);
    }
    return w;
  }

  // Called when the node enters the scene tree for the first time.
  public override void _Ready() {

  }

  // Called every frame. 'delta' is the elapsed time since the previous frame.
  public override void _Process(double delta) {
  }
}
