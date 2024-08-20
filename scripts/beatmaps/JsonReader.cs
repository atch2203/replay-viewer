using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Godot;
using static MapInfo;

public static class JsonReader {

  public static Regex VersionRx = new Regex(@"version""\s*:\s*""(\d\.?)*", RegexOptions.Compiled);
  public static string readFile(string filename) {
    using FileAccess file = FileAccess.Open(filename, FileAccess.ModeFlags.Read);
    string content = file.GetAsText();
    return content;
  }

  public static MapFolder makeMapFolder(string folder){
    MapFolder mapFolder = new MapFolder{
      mapInfo = parseMapInfo(readFile($"{folder}/Info.dat"))
    };
    return mapFolder;
  }

  public static MapInfo parseMapInfo(string info) {
    bool isv4 = false;
    MapInfo mapInfo;
    JsonSerializerOptions options = new JsonSerializerOptions{IncludeFields = true};
    if(isv4){
      mapInfo = JsonSerializer.Deserialize<MapInfo>(info, options);
    }else{
      mapInfo = JsonSerializer.Deserialize<MapInfov2>(info, options).toMapInfo();
    }
    return mapInfo;
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