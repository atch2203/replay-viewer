using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using Godot;
using Godot.Collections;

public partial class Mapfile {
  System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<BeatMap.Difficulty, BeatMap>> beatmaps;
  string version, folder;
  string song_name, song_subname, song_author, song_file, cover_image;
  float bpm, song_length, song_offset, swing_shuffle, swing_shuffle_period, preview_start_time, preview_duration;



  public Mapfile(string folder) {
    this.folder = folder;
    Dictionary data = Json.ParseString(readFile($"{folder}/Info.dat")).As<Dictionary>();
    this.version = data["_version"].As<string>();
    this.song_name = data["_songName"].As<string>();
    this.song_subname = data["_songSubName"].As<string>();
    this.song_author = data["_songAuthorName"].As<string>();
    this.song_file = data["_songFilename"].As<string>();
    this.cover_image = data["_coverImageFilename"].As<string>();
    this.bpm = data["_beatsPerMinute"].As<float>();
    this.song_offset = data["_songTimeOffset"].As<float>();
    this.swing_shuffle = data["_shuffle"].As<float>();
    this.swing_shuffle_period = data["_shufflePeriod"].As<float>();
    this.preview_start_time = data["_previewStartTime"].As<float>();
    this.preview_duration = data["_previewDuration"].As<float>();
    Godot.Collections.Array beatmapSets = data["_difficultyBeatmapSets"].As<Godot.Collections.Array>();
    this.beatmaps = getBeatmaps(beatmapSets);


    // GD.Print(this.ToString());
    // GD.Print(this.beatmaps["Standard"][BeatMap.Difficulty.Easy]);
    // GD.Print(this.beatmaps["Standard"][BeatMap.Difficulty.Easy].beats[1].x);
    // GD.Print(this.beatmaps["Standard"][BeatMap.Difficulty.Easy].beats[1].y);
  }


  public override string ToString() {
    return
    $"version {this.version}\n" +
    $"song_name {this.song_name}\n" +
    $"song_subname {this.song_subname}\n" +
    $"song_author {this.song_author}\n" +
    $"song_file {this.song_file}\n" +
    $"cover_image {this.cover_image}\n" +
    $"bpm {this.bpm}\n" +
    $"song_offset {this.song_offset}\n" +
    $"swing_shuffle {this.swing_shuffle}\n" +
    $"swing_shuffle_period {this.swing_shuffle_period}\n" +
    $"preview_start_time {this.preview_start_time}\n" +
    $"preview_duration {this.preview_duration}\n" +
    $"maps {JsonSerializer.Serialize(this.beatmaps)}";
  }

  private System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<BeatMap.Difficulty, BeatMap>> getBeatmaps(Godot.Collections.Array d) {
    beatmaps = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<BeatMap.Difficulty, BeatMap>>();
    foreach (Dictionary beatmapset in d) {
      System.Collections.Generic.Dictionary<BeatMap.Difficulty, BeatMap> maps = new System.Collections.Generic.Dictionary<BeatMap.Difficulty, BeatMap>();
      string characteristic = beatmapset["_beatmapCharacteristicName"].As<string>();
      foreach (Dictionary map in beatmapset["_difficultyBeatmaps"].As<Godot.Collections.Array>()) {
        maps[Enum.Parse<BeatMap.Difficulty>(map["_difficulty"].As<string>())] = new BeatMap(version, folder, bpm, map);
      }
      beatmaps[characteristic] = maps;
    }
    return beatmaps;
  }

  public static string readFile(string filename) {
    using FileAccess file = FileAccess.Open(filename, FileAccess.ModeFlags.Read);
    string content = file.GetAsText();
    return content;
  }

  //TODO 
  public int getSongLength(){
    return 0;
  }

  public float timeToBeat(float time){
    return time * bpm / 60;
  }
  public float beatToTime(float beat){
    return beat * 60 / bpm;
  }
}
