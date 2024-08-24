using System.Collections.Generic;
using Godot;

//keeps track of, updates, and removes objects
public abstract partial class ObjectPool<B, G>: Node where B: BeatMap.Object where G : GameObject<B>{
  protected Dictionary<B, G> objs;
  protected MapInfo.DifficultyBeatmap difficultyBeatmap;
  protected string sceneLoc;

  public ObjectPool(MapInfo.DifficultyBeatmap difficultyBeatmap, string sceneLoc){
    this.difficultyBeatmap = difficultyBeatmap;
    this.sceneLoc = sceneLoc;
  }

  public void update(float time, Vector3 headPos){
    foreach(B n in objs.Keys){
      GameObject<B> obj = objs.GetValueOrDefault(n, null);
      if(obj.isDone(time) || obj.beforeSpawn(time)){
        obj.QueueFree();
        RemoveChild(obj);
        objs.Remove(n);
      }else{
        obj.update(time, headPos);
      }
    }
  }

  public void addNote(B b){
    if(objs.ContainsKey(b)){
      return;
    }
    PackedScene scene = ResourceLoader.Load<PackedScene>(sceneLoc);
		G gameObj = (G)scene.Instantiate();
    gameObj.initialize(difficultyBeatmap, b);
    objs.Add(b, gameObj);
    AddChild(gameObj);
  }
}