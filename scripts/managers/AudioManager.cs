using Godot;

public partial class AudioManager: AudioStreamPlayer{

  public float speed;
  public AudioEffectPitchShift shift;

  public AudioManager(string songfile){
		this.Stream = ResourceLoader.Load(songfile) as AudioStream;
		shift = new AudioEffectPitchShift();
		AudioServer.AddBus(1);
		AudioServer.SetBusName(1, "Song");
		AudioServer.AddBusEffect(1, shift);
		this.Bus = "Song";
    setSpeed(1F);
  }

  public void setSpeed(float f){
    this.speed = f;
    this.PitchScale = f;
    this.shift.PitchScale = 1/f;
  }


  //seek/stop are in base class
  // use song.StreamPaused = true; to pause
}