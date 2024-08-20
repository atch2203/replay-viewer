using System;
using Godot;
using Godot.Collections;

public partial class BeatMap {
  public enum Difficulty {
    Easy = 1, Normal = 3, Hard = 5, Expert = 7, ExpertPlus = 9
  }

  Difficulty difficulty;
  string beatmapFile;
  float njs, njoffset;
  float bpm;
  string version, folder;
  Array<Beat> beats;
  Array<Bomb> bombs;
  Array<Wall> walls;
  float hjd;

  public BeatMap(string version, string folder, float bpm, Dictionary json) {
    Enum.TryParse((String)json["_difficulty"], out this.difficulty);
    this.beatmapFile = (String)json["_beatmapFilename"];
    this.njs = (float)json["_noteJumpMovementSpeed"];
    this.njoffset = (float)json["_noteJumpStartBeatOffset"];
    this.version = version;
    this.folder = folder;
    this.bpm = bpm;
    this.beats = new Array<Beat>();
    this.bombs = new Array<Bomb>();
    this.walls = new Array<Wall>();

    Dictionary beatmapjson = Json.ParseString(Mapfile.readFile($"{folder}/{this.beatmapFile}")).As<Dictionary>();
    if (version[0] == '3') {
      Array<Dictionary> beatarray = beatmapjson["colorNotes"].As<Godot.Collections.Array<Dictionary>>();
      foreach (Dictionary d in beatarray) {
        beats.Add(Beat.makeBeat(true, d));
      }
      Array<Dictionary> bombarray = beatmapjson["bombNotes"].As<Godot.Collections.Array<Dictionary>>();
      foreach (Dictionary d in bombarray) {
        bombs.Add(Bomb.makeBomb(true, d));
      }
      Array<Dictionary> wallarray = beatmapjson["obstacles"].As<Godot.Collections.Array<Dictionary>>();
      foreach (Dictionary d in wallarray) {
        walls.Add(Wall.makeWall(true, d));
      }
    } else {
      Array<Dictionary> beatbombarray = beatmapjson["_notes"].As<Godot.Collections.Array<Dictionary>>();
      foreach (Dictionary d in beatbombarray) {
        if (d["_type"].As<int>() == 3) {
          bombs.Add(Bomb.makeBomb(false, d));
        } else {
          beats.Add(Beat.makeBeat(false, d));
        }
      }
      Array<Dictionary> wallarray = beatmapjson["_obstacles"].As<Godot.Collections.Array<Dictionary>>();
      foreach (Dictionary d in wallarray) {
        walls.Add(Wall.makeWall(false, d));
      }
    }
    hjd = calc_hjd();
  }
  private float calc_hjd() {
    float hj = 4;
    float n = 60 / bpm;
    while (njs * n * hj > 18) hj /= 2;

    hj += njoffset;
    if (hj < 0.25) hj = 0.25F;

    return hj;
  }

  public float get_njd() {
    return njs * hjd * 60 / bpm;
  }

}
