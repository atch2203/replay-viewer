using Godot;
using static MapInfo;

public partial class NoteManager: StaticBody3D{
  
  public SingleNoteMovementManager singleNoteMovementManager;
  
  public void initialize(DifficultyBeatmap difficultyBeatmap, BeatMap.Note note){
    singleNoteMovementManager = new SingleNoteMovementManager(difficultyBeatmap, note);
  }


  public void update(Frame f){
    PositionData res = singleNoteMovementManager.updatePosition(f, Position, Quaternion.FromEuler(Rotation));
    Position = res.position;
    Rotation = res.rotation.GetEuler();
  }
}