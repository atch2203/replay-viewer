using Godot;
using static MapInfo;

public partial class BombObject: NoteBombObject{

  public void initialize(DifficultyBeatmap difficultyBeatmap, BeatMap.Bomb note){
    base.initialize(difficultyBeatmap, note);
  }
}