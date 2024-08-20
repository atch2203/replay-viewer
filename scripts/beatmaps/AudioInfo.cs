using Godot;

public class AudioInfo{
  public string version;
  public string songChecksum;
  public int songSampleCount;
  public int songFrequency;
  public BPMData[] bpmData;
  public LUFSData[] lufsData;

  public class BPMData{
    public int si, ei, sb, eb;
  }
  public class LUFSData{
    public int si, ei;
    public float l;
  }
}