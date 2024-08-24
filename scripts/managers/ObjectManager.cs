using Godot;
using static MapInfo;

//holds pools for and adds all objects
public partial class ObjectManager : Node {
  public NotePool notePool;
  public BeatMap.Note[] notesList;

  public BombPool bombPool;
  public BeatMap.Bomb[] bombsList;

  public DifficultyBeatmap difficultyBeatmap;

  public ObjectManager(DifficultyBeatmap difficultyBeatmap) {
    this.difficultyBeatmap = difficultyBeatmap;
    notePool = new NotePool(difficultyBeatmap);
    notesList = difficultyBeatmap.map.colorNotes;
    bombPool = new BombPool(difficultyBeatmap);
    bombsList = difficultyBeatmap.map.bombNotes;
    AddChild(notePool);
    AddChild(bombPool);
  }

  public void update(float time, Vector3 headPos) {
    foreach (BeatMap.Note n in notesList) {//todo use more efficient algo
      float moveStart = difficultyBeatmap.getNoteBombMoveTime(n.b);
      float jumpEnd = difficultyBeatmap.getNoteBombJumpEnd(n.b);
      if (time < jumpEnd && time > moveStart) {
        notePool.addNote(n);
      }
    }

    foreach (BeatMap.Bomb b in bombsList) {//todo use more efficient algo
      float moveStart = difficultyBeatmap.getNoteBombMoveTime(b.b);
      float jumpEnd = difficultyBeatmap.getNoteBombJumpEnd(b.b);
      if (time < jumpEnd && time > moveStart) {
        bombPool.addNote(b);
      }
    }

    notePool.update(time, headPos);
    bombPool.update(time, headPos);
  }

}
