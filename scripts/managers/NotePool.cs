using System.Collections.Generic;
using Godot;

//keeps track of, updates, and removes notes
public partial class NotePool: Node{
  Dictionary<BeatMap.Note, NoteObject> notes;
  // Dictionary<BeatMap.Note, NoteObject> notesToAdd;
  MapInfo.DifficultyBeatmap difficultyBeatmap;

  public NotePool(MapInfo.DifficultyBeatmap difficultyBeatmap){
    this.difficultyBeatmap = difficultyBeatmap;
    notes = new Dictionary<BeatMap.Note, NoteObject>();
    // notesToAdd = new Dictionary<BeatMap.Note, NoteObject>();
  }

  public void update(float time, Vector3 headPos){
    foreach(BeatMap.Note n in notes.Keys){
      NoteObject note = notes.GetValueOrDefault(n, null);
      if(note.isDone(time) || note.beforeSpawn(time)){
        note.QueueFree();
        RemoveChild(note);
        notes.Remove(n);
        // if(notesToAdd.ContainsKey(n)) notesToAdd.Remove(n);
      }else{
        note.update(time, headPos);
      }
    }
  }

  public void addNote(BeatMap.Note n){
    if(notes.ContainsKey(n)){
      return;
    }
    PackedScene noteScene = ResourceLoader.Load<PackedScene>("res://gameobjects/beat.tscn");
		NoteObject noteObj = (NoteObject)noteScene.Instantiate();
    noteObj.initialize(difficultyBeatmap, n);
    notes.Add(n, noteObj);
    AddChild(noteObj);
  }
}