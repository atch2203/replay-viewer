using Godot;
using static MapInfo;

public abstract partial class NoteBombObject: StaticBody3D{
  
  public MovementCalculator movement;
  
  public virtual void initialize(DifficultyBeatmap difficultyBeatmap, BeatMap.Object note){
    movement = new MovementCalculator(difficultyBeatmap, note);
  }

  public void update(float time, Vector3 headPos){
    PositionData res = movement.updatePosition(time, headPos, Position, Quaternion.FromEuler(Rotation));
    Position = res.position;
    Rotation = res.rotation.GetEuler();
  }

  public bool beforeSpawn(float time){
    return time < movement.moveStartTime;
  }

  public bool isDone(float time){
    bool isAtEnd = time > movement.jumpEndTime;
    bool isHit = false;
    return isAtEnd || isHit;
  }
}