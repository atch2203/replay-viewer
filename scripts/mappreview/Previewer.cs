using Godot;
using static MapInfo;

//keeps track of timing + ???
public partial class Previewer : Node {
  
  ObjectManager objectManager;
  public Previewer(DifficultyBeatmap difficultyBeatmap) {
    objectManager = new ObjectManager(difficultyBeatmap);
    AddChild(objectManager);
  }
  public void goTo(float time){
    objectManager.update(time, new Vector3(0, 1.7F, 0));
  }
}