using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using Godot.Collections;

public partial class MapFolder {
  // [JsonInclude]
  // public System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary> beatmaps;
  [JsonInclude]
  public MapInfo mapInfo;
  [JsonInclude]
  public string folder;

  public override string ToString() {
    return JsonSerializer.Serialize(this);
  }
}
