using System.Collections.Generic;
using Godot;

//keeps track of, updates, and removes notes
public partial class NotePool: ObjectPool<BeatMap.Note, NoteObject>{
  public NotePool(MapInfo.DifficultyBeatmap difficultyBeatmap): base(difficultyBeatmap, "res://gameobjects/beat.tscn"){
    objs = new Dictionary<BeatMap.Note, NoteObject>();
  }
}

public partial class BombPool: ObjectPool<BeatMap.Bomb, BombObject>{
  public BombPool(MapInfo.DifficultyBeatmap difficultyBeatmap): base(difficultyBeatmap, "res://gameobjects/bomb.tscn"){
    objs = new Dictionary<BeatMap.Bomb, BombObject>();
  }
}

// public partial class WallPool: ObjectPool<BeatMap.Wall, WallObject>{
//   public WallPool(MapInfo.DifficultyBeatmap difficultyBeatmap): base(difficultyBeatmap, ){

//   }
// }