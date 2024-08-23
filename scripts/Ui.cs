using Godot;
using System;

public partial class Ui : Node2D
{
  HSlider slider;
  bool dragging;
  public TimeManager timeManager;
  public AudioManager audioManager;

  public void initializeAudio(string songfile){
    timeManager = new TimeManager();
    audioManager = new AudioManager(songfile);
    slider = GetNode("Slider") as HSlider;
    slider.MinValue = 0;
    slider.MaxValue = audioManager.Stream.GetLength();
    AddChild(audioManager);
    audioManager.Play();
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
        timeManager.time = audioManager.GetPlaybackPosition();
        slider.Value = timeManager.time;
      }
    }
	}

  public void SliderDragStarted(){
    dragging = true;
    audioManager.StreamPaused = true;
  }

  public void SliderDragEnded(bool a){
    dragging = false;
    timeManager.time = (float)slider.Value;
    audioManager.Play(timeManager.getTime());
    if(timeManager.paused){
      audioManager.StreamPaused = true;
    }
  }

  public void TogglePause(){
    if(!dragging){
      timeManager.togglePause();
      audioManager.StreamPaused = !audioManager.StreamPaused;
    }
  }
}
