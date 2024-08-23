using Godot;

public partial class TimeManager {
  public float time;
  public bool paused;
  
  public TimeManager(){
    time = 0;
    paused = false;
  }

  public void togglePause(){
    paused = !paused;
  }
  public float getTime(){
    return time;
  }
}