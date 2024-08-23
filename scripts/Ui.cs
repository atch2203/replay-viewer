using Godot;
using System;

public partial class Ui : Node2D
{
  HSlider slider;
  bool dragging;
  public TimeManager timeManager;
  public SongManager songManager;

  public void initializeTimeManager(TimeManager timeManager){
    this.timeManager = timeManager;
  }

  public void initializeAudio(string songfile){
    songManager = new SongManager(songfile);
    slider = GetNode("Slider") as HSlider;
    slider.MinValue = 0;
    slider.MaxValue = songManager.Stream.GetLength();
    AddChild(songManager);
    songManager.Play();
  }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    if(dragging){
      timeManager.time = (float)slider.Value;
    }else{
      if(!timeManager.paused){
        timeManager.time = songManager.GetPlaybackPosition();
        slider.Value = timeManager.time;
      }
    }
	}

  public void SliderDragStarted(){
    dragging = true;
    songManager.StreamPaused = true;
  }

  public void SliderDragEnded(bool a){
    dragging = false;
    timeManager.time = (float)slider.Value;
    songManager.Play(timeManager.getTime());
    if(timeManager.paused){
      songManager.StreamPaused = true;
    }
  }

  public void TogglePause(){
    if(!dragging){
      timeManager.togglePause();
      songManager.StreamPaused = !songManager.StreamPaused;
    }
  }
}
