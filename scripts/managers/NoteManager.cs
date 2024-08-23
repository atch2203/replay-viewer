using Godot;
using static MapInfo;

public partial class NoteManager: StaticBody3D{
  
  public SingleNoteMovementManager singleNoteMovementManager;
  
  public void initialize(DifficultyBeatmap difficultyBeatmap, BeatMap.Note note){
    singleNoteMovementManager = new SingleNoteMovementManager(difficultyBeatmap, note);
  }


  public void update(float time, float headX=0, float headY=1.7F, float headZ=0){
    PositionData res = singleNoteMovementManager.updatePosition(time, new Vector3(headX, headY, headZ), Position, Quaternion.FromEuler(Rotation));
    Position = res.position;
    Rotation = res.rotation.GetEuler();
  }
}