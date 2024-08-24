using Godot;
using static MapInfo;

public abstract partial class NoteBombObject<O>: GameObject<O> where O: BeatMap.Object{
  
  public NoteBombMovement movement;
  
  public override void initialize(DifficultyBeatmap difficultyBeatmap, O obj){
    movement = new NoteBombMovement(difficultyBeatmap, obj);
  }

  public override void update(float time, Vector3 headPos){
    PositionData res = movement.updatePosition(time, headPos, Position, Quaternion.FromEuler(Rotation));
    Position = res.position;
    Rotation = res.rotation.GetEuler();
  }

  public override bool beforeSpawn(float time){
    return time < movement.moveStartTime;
  }

  public override bool isDone(float time){
    bool isAtEnd = time > movement.jumpEndTime;
    bool isHit = false;
    return isAtEnd || isHit;
  }
}