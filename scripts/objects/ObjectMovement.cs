using System;
using System.Collections.Generic;
using Godot;
using static BeatMap;

public abstract class ObjectMovement  { //TODO clean this up
  public MapInfo.DifficultyBeatmap difficultyBeatmap;
  public BeatMap.Object obj;

  // credit to [NSGolova](https://github.com/NSGolova) for these values + outline for position/movement code

  #region consts
  public static float REPLAY_HEIGHT { get; set; } = 1.7F;

  public const float SWORD_OFFSET = 0.9F;

  protected Quaternion worldRot = new Quaternion(0, 0, 0, 1), inverseWorldRot = new Quaternion(0, 0, 0, 1), worldPlayerRot = new Quaternion(0, 0, 0, 1);
  protected static float endDistOffset = 500;
  #endregion

  #region mathfuncs
  public static float lerp(float start, float end, float t) {
    if (t < 0) return start;
    if (t > 1) return end;
    return start + (end - start) * t;
  }
  public static float lerpUnclamped(float start, float end, float t) {
    return start + (end - start) * t;
  }
  public static float quadInOut(float t) {
    if (t < 0.5) return 2 * t * t;
    return (4 - 2 * t) * t - 1;
  }
  public static float clamp(float v, float min, float max) {
    if (v < min) return min;
    if (v > max) return max;
    return v;
  }
  public static float deg2rad(float d) {
    return (float)(Math.PI * d / 180);
  }
  #endregion

  public abstract PositionData updatePosition(float time, Vector3 frameHeadPos, Vector3 curPos, Quaternion curRot);
   
  #region positionutils
  protected Quaternion lookingAt(Vector3 forward, Vector3 up) {
    Quaternion c = Transform3D.Identity.LookingAt(forward, up).Basis.GetRotationQuaternion();
    Quaternion d = new Quaternion(-c.Z, c.W, c.X, -c.Y);
    return c;
  }
  protected float moveTowardHead(float start, float end, Quaternion invWorldRot, float jumpPercent, Vector3 headPos) {
    float headOffsetZ = rotate(headPos, invWorldRot).Z; //usually negligible
    return lerpUnclamped(start + headOffsetZ * Math.Min(1, jumpPercent * 2), end + headOffsetZ, jumpPercent);
  }
  protected static Vector3 rotate(Vector3 v, Quaternion q) {
    Quaternion quatForm = q * new Quaternion(v.X, v.Y, v.Z, 0) * q.Inverse();
    return new Vector3(quatForm.X, quatForm.Y, quatForm.Z);
  }
  #endregion

}
