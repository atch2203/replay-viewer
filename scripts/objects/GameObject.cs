using Godot;
using static MapInfo;

public abstract partial class GameObject<O>: StaticBody3D where O: BeatMap.Object{
  
  public abstract void initialize(DifficultyBeatmap difficultyBeatmap, O obj);

  public abstract void update(float time, Vector3 headPos);

  public abstract bool beforeSpawn(float time);

  public abstract bool isDone(float time);
}