using System;
using System.Diagnostics.Contracts;
using System.Linq;

public class BeatMap{ //TODO lightshow https://bsmg.wiki/mapping/map-format/lightshow.html

  public Note[] colorNotes;
  public Bomb[] bombNotes;
  public Slider[] sliders;
  public Chain[] chains;
  public RotationEvent[] rotationEvents;
  public BPMEvent[] bpmEvents;

  public class Object{
    public float b;
    public int x, y;
  }

  public class Note: Object{
    public SaberColor c;
    public CutDirection d;
    public float a;
  }

  public class Bomb: Object{}

  public class Wall: Object{
    public float d;
    public int w, h;
  }

  public class Slider: Object{
    public SaberColor c;
    public CutDirection d;
    public int m;
    public float mu, tmu;
    public float tb;
    public int tx, ty;
    public int tc; // more aptly td
  }

  public class Chain: Object{
    public SaberColor c;
    public CutDirection d;
    public float tb;
    public int tx, ty;
    public int sc;
    public float s;
  }

  public class RotationEvent{
    public float b;
    public int e;
    public float r;
  }

  public class BPMEvent{
    public float b, m;
  }


  public enum SaberColor{LEFT=0,RIGHT=1}
  public enum CutDirection{UP=0,DOWN=1,LEFT=2,RIGHT=3,UPLEFT=4,UPRIGHT=5,DOWNLEFT=6,DOWNRIGHT=7,ANY=8}

  public class BeatMapv4{ // todo? use this  https://converter.stormpacer.xyz/

    public Note[] colorNotes;
    public NoteData[] colorNotesData;
    public Bomb[] bombNotes;
    public BombData[] bombNotesData;
    public Wall[] obstacles;
    public WallData[] obstaclesData;
    public Slider[] arcs;
    public SliderData[] arcsData;
    public Chain[] chains;
    public ChainData[] chainsData;
    public RotationEvent[] spawnRotations;
    public RotationEventData[] spawnRotationsData;


    public class Object{
      public float b;
      public int r;
      public int i;
    }

    public class ObjectData{
      public int x, y;
    }

    public class Note: Object{}
    public class NoteData: ObjectData{
      public int c, d;
      public float a;
    }

    public class Bomb: Object{}
    public class BombData: ObjectData{}

    public class Wall: Object{}
    public class WallData: ObjectData{
      public float d;
      public int w, h;
    }

    public class Slider{
      public float hb, tb;
      public int hr, tr;
      public int hi, ti, ai;
    }
    public class SliderData{
      public float m, tm;
      public int a;
    }

    public class Chain{
      public int hr, tr;
      public float hb, tb;
      public int i, ci;
    }
    public class ChainData{
      public int tx, ty;
      public int c;
      public float s;
    }

    public class RotationEvent{
      public float b;
      public int i;
    }
    public class RotationEventData{
      public int t;
      public float r;
    }
    //bpm events are in audio.dat file

    public BeatMap toBeatMap(){
      return null;
    }
  }

  public class BeatMapv2{
    public Note[] _notes;
    public Wall[] _obstacles;
    public Slider[] _sliders;
    public Event[] _events;

    public class Object{
      public float _time;
      public int _lineIndex, _lineLayer;
      public int _type;
    }

    public class Note: Object{
      public int _cutDirection;
    }

    public class Wall: Object{
      public float _duration;
      public int _width, _height;
    }

    public class Slider{
      public int _colorType, _sliderMidAnchorMode;
      public float _headTime, _tailTime;
      public int _headLineIndex, _headLineLayer, _tailLineIndex, _tailLineLayer;
      public int _headCutDirection, _tailCutDirection;
      public float _headControlPointLengthMultiplier, _tailControlPointLengthMultiplier;
    }

    public class Event{
      public float _time;
      public int _type;
      public int _value;
      public float _floatValue;
    }

    public BeatMap toBeatMap(){
      BeatMap.Note[] colorNotes = _notes is null ? null : _notes
      .Where(n => n._type != 3)
      .Select(n => new BeatMap.Note{
        b=n._time,
        x=n._lineIndex,
        y=n._lineLayer,
        c=(SaberColor)n._type,
        d=(CutDirection)n._cutDirection,
        a=0
      }).ToArray();

      BeatMap.Bomb[] bombs = _notes is null ? null : _notes
      .Where(n => n._type == 3)
      .Select(n => new BeatMap.Bomb{
        b=n._time,
        x=n._lineIndex,
        y=n._lineLayer
      }).ToArray();

      BeatMap.Slider[] sliders = _sliders is null ? null : _sliders
      .Select(s => new BeatMap.Slider{
        c=(SaberColor)s._colorType,
        b=s._headTime,
        x=s._headLineIndex,
        y=s._headLineLayer,
        d=(CutDirection)s._headCutDirection,
        mu=s._headControlPointLengthMultiplier,
        tb=s._tailTime,
        tx=s._tailLineIndex,
        ty=s._tailLineLayer,
        tc=s._tailCutDirection,
        tmu=s._tailControlPointLengthMultiplier,
        m=s._sliderMidAnchorMode
      }).ToArray();

      BeatMap.RotationEvent[] rotationEvents = _events is null ? null : _events
      .Where(e => e._type == 14 || e._type == 15)
      .Select(e => new BeatMap.RotationEvent{
        b=e._time,
        e=e._type-14,
        r=e._value,
      }).ToArray();

      BeatMap.BPMEvent[] bpmEvents = _events is null ? null : _events
      .Where(e => e._type == 100)
      .Select(e => new BeatMap.BPMEvent{
        b=e._time,
        m=e._floatValue
      }).ToArray();

      BeatMap res = new BeatMap{
        colorNotes=colorNotes,
        bombNotes=bombs,
        sliders=sliders,
        chains=null,
        rotationEvents=rotationEvents,
        bpmEvents=bpmEvents
      };
      return res;
    }
  }
}