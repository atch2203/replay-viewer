using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text.Json;
using System.Text.Json.Serialization;

public class MapInfo {

  public string version;
  public Song song;
  public Audio audio;
  public string songPreviewFilename, coverImageFilename;
  public string[] environmentNames;
  public ColorScheme[] colorSchemes;
  public DifficultyBeatmap[] difficultyBeatmaps;
  public struct Song {
    public string title, subTitle, author;
  }

  public struct Audio {
    public string songFilename, audioDataFilename;
    public float bpm, lufs, songDuration, previewStartTime, previewDuration;
  }

  public class DifficultyBeatmap {
    public int environmentNameIdx, beatmapColorSchemeIdx;
    public float noteJumpMovementSpeed, noteJumpStartBeatOffset;
    public string difficulty, beatmapDataFilename, lightshowDataFilename, characteristic;
    public Authors beatmapAuthors;
    public struct Authors {
      public string[] mappers, lighters;
    }
    public BeatMap map;
    public float bpm;
    public float hjd = -1, njd = -1;
    
    public float getHJD(){
      if(hjd != -1) return hjd;
      float hj = 4F;//just a coefficient
      float n = 60.0F/bpm; // seconds/beat
      while(noteJumpMovementSpeed*n*hj > 18.0F) hj /= 2.0F; // checks whether distance/beat is too high
      hj += noteJumpStartBeatOffset;
      if(hj < 0.25F) hj = 0.25F;
      hjd = hj;
      return hj;
    }

    public float getNJD(){
      // if(njd != -1) return njd;
      njd = noteJumpMovementSpeed * getHJD() * 60.0F / bpm;
      return njd;
    }
    
    public float getJumpDuration(){
      return getHJD() * 60F / bpm;
    }

    public float getNoteBombMoveTime(float beat){
      return beat * 60F / bpm - NoteBombMovement.MovementData.MOVE_TIME - getJumpDuration() / 2F;
    }
    public float getNoteBombJumpEnd(float beat){
      return beat * 60F / bpm + getJumpDuration() / 2F;
    }
  }
  public struct ColorScheme {
    public bool useOverride;
    public string colorSchemeName;
    public string saberAColor;
    public string saberBColor;
    public string obstaclesColor;
    public string environmentColor0;
    public string environmentColor1;
    public string environmentColor0Boost;
    public string environmentColor1Boost;
  }

 


  public override string ToString() {
    return JsonSerializer.Serialize(this, new JsonSerializerOptions { IncludeFields = true });
  }

  public struct MapInfov2 {

    //taken from allpoland beatmapinfov2.cs
    public string _version;
    public string _songName;
    public string _songSubName;
    public string _songAuthorName;
    public string _levelAuthorName;
    public float _beatsPerMinute;
    public string _songFilename;
    public string _coverImageFilename;
    public string _environmentName;
    public string _allDirectionsEnvironmentName;
    public float _songTimeOffset; // depreciated
    public float _shuffle, _shufflePeriod;//depreciated
    public float _previewStartTime, _previewDuration;
    public DifficultyBeatmapSet[] _difficultyBeatmapSets;

    public string[] _environmentNames;
    public ColorScheme[] _colorSchemes;

    public struct DifficultyBeatmapSet {
      public string _beatmapCharacteristicName;
      public DifficultyBeatmap[] _difficultyBeatmaps;
    }

    public struct DifficultyBeatmap {
      public string _difficulty;
      public int _difficultyRank;
      public string _beatmapFilename;
      public float _noteJumpMovementSpeed;
      public float _noteJumpStartBeatOffset;

      public int _beatmapColorSchemeIdx;
      public int _environmentNameIdx;
      // public DifficultyBeatmapCustomDataV2 _customData;
    }
    public class ColorScheme {
      public bool userOverride;
      public InnerColorScheme colorScheme;
      public class InnerColorScheme {
        public string colorSchemeId;
        public Color saberAColor;
        public Color saberBColor;
        public Color obstaclesColor;
        public Color environmentColor0;
        public Color environmentColor1;
        public Color environmentColor0Boost;
        public Color environmentColor1Boost;
        public class Color {
          public float r, g, b, a;
          public string toHex() {
            return $"#{((int)(r * 255)).ToString("X")}{((int)(g * 255)).ToString("X")}{((int)(b * 255)).ToString("X")}{((int)(a * 255)).ToString("X")}";
          }
        }
      }
    }

    public MapInfo toMapInfo() {
      //see https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.selectmany?view=net-8.0
      MapInfo.DifficultyBeatmap[] difficultyBeatmaps = _difficultyBeatmapSets
      .SelectMany(set => set._difficultyBeatmaps, (set, difficultyBeatmap) => new { c = set._beatmapCharacteristicName, d = difficultyBeatmap })
      .Select(cdp => new MapInfo.DifficultyBeatmap {
        environmentNameIdx = cdp.d._environmentNameIdx,
        beatmapColorSchemeIdx = cdp.d._beatmapColorSchemeIdx,
        noteJumpMovementSpeed = cdp.d._noteJumpMovementSpeed,
        noteJumpStartBeatOffset = cdp.d._noteJumpStartBeatOffset,
        difficulty = cdp.d._difficulty,
        beatmapDataFilename = cdp.d._beatmapFilename,
        characteristic = cdp.c
      }).ToArray();

      MapInfo.ColorScheme[] colorSchemes = _colorSchemes is null ? null : _colorSchemes.
      Select(c => new MapInfo.ColorScheme {
        useOverride = c.userOverride,
        colorSchemeName = c.colorScheme.colorSchemeId,
        saberAColor = c.colorScheme.saberAColor.toHex(),
        saberBColor = c.colorScheme.saberBColor.toHex(),
        obstaclesColor = c.colorScheme.obstaclesColor.toHex(),
        environmentColor0 = c.colorScheme.environmentColor0.toHex(),
        environmentColor1 = c.colorScheme.environmentColor1.toHex(),
        environmentColor0Boost = c.colorScheme.environmentColor0Boost.toHex(),
        environmentColor1Boost = c.colorScheme.environmentColor1Boost.toHex(),
      }).ToArray();

      MapInfo res = new MapInfo {
        version = this._version,
        song = new Song {
          title = _songName,
          subTitle = _songSubName,
          author = _songAuthorName
        },
        audio = new Audio {
          songFilename = _songFilename,
          bpm = _beatsPerMinute
        },
        songPreviewFilename = _songFilename,
        coverImageFilename = _coverImageFilename,
        environmentNames = _environmentNames,
        colorSchemes = colorSchemes,
        difficultyBeatmaps = difficultyBeatmaps
      };
      return res;
    }
  }
}