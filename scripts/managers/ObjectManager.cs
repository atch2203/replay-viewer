using Godot;
using static MapInfo;

//holds pools for and adds all objects
public partial class ObjectManager: Node{
  public NotePool notePool;
  public BeatMap.Note[] notesList;
  public DifficultyBeatmap difficultyBeatmap;

  public ObjectManager(DifficultyBeatmap difficultyBeatmap){
    this.difficultyBeatmap = difficultyBeatmap;
    notePool = new NotePool(difficultyBeatmap);
    notesList = difficultyBeatmap.map.colorNotes;
    AddChild(notePool);
  }

  public void update(float time, Vector3 headPos){
    foreach(BeatMap.Note n in notesList){//todo use more efficient algo
      float moveStart = n.b * 60F / difficultyBeatmap.bpm - MovementCalculator.MovementData.MOVE_TIME - difficultyBeatmap.getJumpDuration()/2F;
      float jumpEnd = n.b * 60F / difficultyBeatmap.bpm + difficultyBeatmap.getJumpDuration()/2F;
      if(time < jumpEnd && time > moveStart){
        notePool.addNote(n);
      }
    }
    notePool.update(time, headPos);
  }

}