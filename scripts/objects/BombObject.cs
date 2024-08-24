using Godot;
using static MapInfo;

public partial class BombObject: NoteBombObject<BeatMap.Bomb>{

  public override void initialize(DifficultyBeatmap difficultyBeatmap, BeatMap.Bomb note){
    base.initialize(difficultyBeatmap, note);
  }
}