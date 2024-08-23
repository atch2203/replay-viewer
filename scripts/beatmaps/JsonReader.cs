using System;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Godot;
using Microsoft.VisualBasic;
using static BeatMap;
using static MapInfo;

public static class JsonReader {

  private static Regex versionRegex = new Regex("version\": ?\"(\\d\\.\\d\\.\\d)\"", RegexOptions.Compiled);
  public static readonly JsonSerializerOptions options = new JsonSerializerOptions{IncludeFields = true};

  public static string readFile(string filename) {
    using FileAccess file = FileAccess.Open(filename, FileAccess.ModeFlags.Read);
    string content = file.GetAsText();
    return content;
  }

  public static MapFolder makeMapFolder(string folder){
    MapFolder mapFolder = new MapFolder{
      mapInfo = parseMapInfo(readFile($"{folder}/Info.dat"))
    };
    foreach(MapInfo.DifficultyBeatmap difficultyBeatmap in mapFolder.mapInfo.difficultyBeatmaps){
      difficultyBeatmap.map = parseBeatMap(readFile($"{folder}/{difficultyBeatmap.beatmapDataFilename}"));
      difficultyBeatmap.bpm = mapFolder.mapInfo.audio.bpm;
    }
    return mapFolder;
  }

  private static string getVersion(string json){
    string res;
    try{
      Match match = versionRegex.Match(json);
      res = match.Groups[1].ToString();
    }catch(Exception){
      GD.Print("bad json versioning format");
      return null;
    }
    return res;
  }

  public static MapInfo parseMapInfo(string info) {
    bool isv4 = getVersion(info)[0] == '4';
    MapInfo mapInfo;
    if(isv4){
      mapInfo = JsonSerializer.Deserialize<MapInfo>(info, options);
    }else{
      mapInfo = JsonSerializer.Deserialize<MapInfov2>(info, options).toMapInfo();
    }
    return mapInfo;
  }

  public static BeatMap parseBeatMap(string beatmap){
    BeatMap res;
    switch(getVersion(beatmap)[0]){
      case '2':
        res = JsonSerializer.Deserialize<BeatMapv2>(beatmap, options).toBeatMap();
        break;
      case '3':
        res = JsonSerializer.Deserialize<BeatMap>(beatmap, options);
        break;
      case '4': //TODO? implement v4 parsing
        res = JsonSerializer.Deserialize<BeatMapv4>(beatmap, options).toBeatMap();
        break;
      default:
        GD.Print("bad beatmap");
        res = null;
        break;
    }
    return res;
  }

  // public static BeatMapDiffWrapper parseBeatmap(string json, DifficultyBeatmap beatmap) {
  //   string filename = beatmap._beatmapFilename;

  //   BeatMapDiff diff;

  //   Match match = VersionRx.Match(json);
  //   string versionNumber = match.Value.Split('"').Last();
  //   if(versionNumber[0] == '3'){
  //     BeatmapDiffv3Json v3Json = JsonSerializer.Deserialize<BeatmapDiffv3Json>(json);
  //   }
  //   diff = 
  // }
}