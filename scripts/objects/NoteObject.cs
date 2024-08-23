
using Godot;
using static MapInfo;

public partial class NoteObject: NoteBombObject{

  public static StandardMaterial3D blue = new StandardMaterial3D{AlbedoColor=new Color(0,0,1,1)};
  public static StandardMaterial3D red = new StandardMaterial3D{AlbedoColor=new Color(1,0,0,1)};

  public void initialize(DifficultyBeatmap difficultyBeatmap, BeatMap.Note note){
    if(note is BeatMap.Note n){
      if(n.d == BeatMap.CutDirection.ANY){
        GetNode("Arrow").QueueFree();
      }else{
        GetNode("Dot").QueueFree();
      }
      if(n.c == BeatMap.SaberColor.RIGHT){
        ((GetNode("Beat") as GeometryInstance3D).MaterialOverride ) = blue;
      }else{
        ((GetNode("Beat") as GeometryInstance3D).MaterialOverride ) = red;
      }
      base.initialize(difficultyBeatmap, note);
    }
  }
}