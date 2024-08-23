using System;
using System.Collections.Generic;
using Godot;
using static BeatMap;

public class MovementCalculator { //TODO clean this up
  public MapInfo.DifficultyBeatmap difficultyBeatmap;
  public BeatMap.Object obj;
  public MovementData movementData;

  // credit to [NSGolova](https://github.com/NSGolova) for these values + outline for position/movement code

  #region consts
  public static float[] Y_LAYER_POS = { 0.25F, 0.85F, 1.45F };
  public static float[] HIGHEST_Y_POS = { 0.85F, 1.4F, 1.9F };
  public static float[] X_LAYER_POS = { -1.5F * 0.6F, -0.5F * 0.6F, 0.5F * 0.6F, 1.5F * 0.6F }; //flipped for some reason????
  public static Dictionary<BeatMap.CutDirection, float> ROTATION_ANGLE = new Dictionary<CutDirection, float> {
    // [CutDirection.UP] = 0F,
    // [CutDirection.DOWN] = 180F,
    // [CutDirection.LEFT] = 90F,
    // [CutDirection.RIGHT] = 270F,
    // [CutDirection.UPRIGHT] = 315F,
    // [CutDirection.DOWNRIGHT] = 225F,
    // [CutDirection.UPLEFT] = 45F,
    // [CutDirection.DOWNLEFT] = 135F
    [CutDirection.UP] = 180F,
    [CutDirection.DOWN] = 0F,
    [CutDirection.LEFT] = 270F,
    [CutDirection.RIGHT] = 900F,
    [CutDirection.UPRIGHT] = 135F,
    [CutDirection.DOWNRIGHT] = 45F,
    [CutDirection.UPLEFT] = 225F,
    [CutDirection.DOWNLEFT] = 315F,
    [CutDirection.ANY] = 180F
  };
  public static float REPLAY_HEIGHT { get; set; } = 1.7F;

  public const float SWORD_OFFSET = 0.9F;

  public static Vector3[] RANDOM_ROTATIONS = {
  new Vector3(deg2rad(-0.954387F), deg2rad(-0.1183784F), deg2rad(0.2741019F)),
  new Vector3(deg2rad(0.7680854F), deg2rad(-0.08805521F), deg2rad(0.6342642F)),
  new Vector3(deg2rad(-0.6780157F), deg2rad(0.306681F), deg2rad(-0.6680131F)),
  new Vector3(deg2rad(0.1255014F), deg2rad(0.9398643F), deg2rad(0.3176546F)),
  new Vector3(deg2rad(0.365105F), deg2rad(-0.3664974F), deg2rad(-0.8557909F)),
  new Vector3(deg2rad(-0.8790653F), deg2rad(-0.06244748F), deg2rad(-0.4725934F)),
  new Vector3(deg2rad(0.01886305F), deg2rad(-0.8065798F), deg2rad(0.5908241F)),
  new Vector3(deg2rad(-0.1455435F), deg2rad(0.8901445F), deg2rad(0.4318099F)),
  new Vector3(deg2rad(0.07651193F), deg2rad(0.9474725F), deg2rad(-0.3105508F)),
  new Vector3(deg2rad(0.1306983F), deg2rad(-0.2508438F), deg2rad(-0.9591639F))
  };
  public static int NUM_RANDOM_ROTATIONS = RANDOM_ROTATIONS.Length;

  bool rotateTowardPlayer = true;
  Quaternion worldRot = new Quaternion(0, 0, 0, 1), inverseWorldRot = new Quaternion(0, 0, 0, 1), worldPlayerRot = new Quaternion(0, 0, 0, 1);
  static float endDistOffset = 500;
  #endregion

  #region unchanging vars
  public float noteTime, moveStartTime, jumpStartTime, jumpEndTime, startVertVelo;
  Quaternion startRotation, midRotation, endRotation;
  #endregion

  public MovementCalculator(MapInfo.DifficultyBeatmap difficultyBeatmap, BeatMap.Object note) {
    if (note is Bomb) rotateTowardPlayer = false;

    this.difficultyBeatmap = difficultyBeatmap;
    obj = note;
    this.movementData = new MovementData(difficultyBeatmap, note);

    noteTime = obj.b * 60F / difficultyBeatmap.bpm;
    moveStartTime = noteTime - MovementData.MOVE_TIME - movementData.jumpDuration / 2F;
    jumpStartTime = noteTime - movementData.jumpDuration / 2F;
    jumpEndTime = noteTime + movementData.jumpDuration / 2F;
    startVertVelo = movementData.jumpGravity * movementData.jumpDuration / 2F;
    // GD.Print($"jump {jumpStartTime}");
    // GD.Print($"end {noteTime + movementData.jumpDuration / 2F}");

    startRotation = new Quaternion(0, 0, 0, 1);
    endRotation = Quaternion.FromEuler(new Vector3(0, 0, movementData.endRotation));

    Vector3 midEulerRot = new Vector3(0, 0, 0);
    int rotIndex = Math.Abs(((int)Math.Round(noteTime * 10 + movementData.jumpEnd.X * 2 + movementData.jumpEnd.Y * 2)) % NUM_RANDOM_ROTATIONS);
    midEulerRot += RANDOM_ROTATIONS[rotIndex] * 20;
    midRotation = Quaternion.FromEuler(midEulerRot+endRotation.GetEuler()).Normalized();

  }

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

  public PositionData updatePosition(float time, Vector3 frameHeadPos, Vector3 curPos, Quaternion curRot) {
    PositionData positionData = new PositionData { rotation = new Quaternion(0, 0, 0, 1) };
    //floor movement
    float relativeMoveTime = time - moveStartTime;
    if (relativeMoveTime < 0) throw new Exception("note shouldn't have spawned yet");

    if (relativeMoveTime < MovementData.MOVE_TIME) { // floor/initial movement
      // GD.Print($"move step {relativeMoveTime / MovementData.MOVE_TIME}");
      positionData.position = movementData.moveStart.Lerp(movementData.moveEnd, relativeMoveTime / MovementData.MOVE_TIME);
      return positionData;
    }

    //jump movement
    float relativeJumpTime = time - jumpStartTime;
    float jumpPercent = relativeJumpTime / movementData.jumpDuration;
    // GD.Print($"jump step {jumpPercent}");

    Vector3 headPos = new Vector3(frameHeadPos.X, frameHeadPos.Y, frameHeadPos.Z);
    positionData.position = rotate(calculatePosition(jumpPercent, startVertVelo, relativeJumpTime, headPos,
    endDistOffset, inverseWorldRot), worldRot);

    // float yAvoidance = 0;
    //if y_avoidance != 0 and percentage_of_jump < 0.25: since y_avoidance is 0 not used
    //local_pos.y += (0.5 - cos(percentage_of_jump * 8 * π) * 0.5) * y_avoidance

    positionData.rotation = calculateRotation(jumpPercent,
    startRotation, midRotation, endRotation, curRot,
    headPos, positionData.position, worldPlayerRot, inverseWorldRot, rotateTowardPlayer);

    return positionData;
  }

  #region calcthings
  private Vector3 calculatePosition(float jumpPercent, float startVertVelo, float relativeJumpTime,
  Vector3 headPos, float endDistOffset, Quaternion inverseWorldRot) {
    Vector3 localPos = new Vector3(0, 0, 0);

    if (movementData.moveEnd.X == movementData.jumpEnd.X) localPos.X = movementData.moveEnd.X;
    else if (jumpPercent >= 0.25) localPos.X = movementData.jumpEnd.X;
    else localPos.X = lerpUnclamped(movementData.moveEnd.X, movementData.jumpEnd.X, quadInOut(jumpPercent));

    localPos.Y = movementData.moveEnd.Y + startVertVelo * relativeJumpTime - movementData.jumpGravity * relativeJumpTime * relativeJumpTime / 2F;

    localPos.Z = moveTowardHead(movementData.moveEnd.Z, movementData.jumpEnd.Z, inverseWorldRot, jumpPercent, headPos);
    if (jumpPercent >= 0.75) {//end of jump: accelerate off to endDistOffset
      float rpp = (jumpPercent - 0.75F) / 0.25F;
      localPos.Z += lerpUnclamped(0, endDistOffset, rpp * rpp * rpp);
    }

    return localPos;
  }

  private Quaternion calculateRotation(float jumpPercent,
  Quaternion startRotation, Quaternion midRotation, Quaternion endRotation, Quaternion curRot,
  Vector3 headPos, Vector3 localPos,
  Quaternion worldPlayerRot, Quaternion inverseWorldRot,
  bool rotateTowardPlayer) {
    Quaternion res, rot;
    if(jumpPercent < 0.5){
      if (jumpPercent >= 0.125) {
        rot = midRotation.Slerp(endRotation, (float)Math.Sin((jumpPercent - 0.125) / 3 * Math.PI * 4)); //not the same as sabersim?
        // rot = midRotation.Slerp(endRotation, (float)Math.Sin((jumpPercent - 0.125) * Math.PI * 4)); 
      } else {
        rot = startRotation.Slerp(midRotation, (float)Math.Sin(jumpPercent * Math.PI * 4));
      }
    }else{
      rot = endRotation;
    }
    if (jumpPercent < 0.5) {
      if (rotateTowardPlayer) {
        headPos.Y = lerp(headPos.Y, localPos.Y, 0.8F);
        Vector3 normalized = (localPos - rotate(headPos, inverseWorldRot)).Normalized();
        Vector3 rotatedUp = rotate(Vector3.Up, rot.Normalized());

        rotatedUp = rotate(rotatedUp, worldPlayerRot);
        Quaternion b = lookingAt(normalized, rotate(rotatedUp, inverseWorldRot));
        res = rot.Slerp(b, jumpPercent * 2);
      } else {
        res = rot;
      }
    } else {
      res = curRot;
    }
    return res;
  }
  #endregion

  #region positionutils
  private Quaternion lookingAt(Vector3 forward, Vector3 up) {
    Quaternion c = Transform3D.Identity.LookingAt(forward, up).Basis.GetRotationQuaternion();
    Quaternion d = new Quaternion(-c.Z, c.W, c.X, -c.Y);
    return c;
  }
  private float moveTowardHead(float start, float end, Quaternion invWorldRot, float jumpPercent, Vector3 headPos) {
    float headOffsetZ = rotate(headPos, invWorldRot).Z; //usually negligible
    return lerpUnclamped(start + headOffsetZ * Math.Min(1, jumpPercent * 2), end + headOffsetZ, jumpPercent);
  }
  private static Vector3 rotate(Vector3 v, Quaternion q) {
    Quaternion quatForm = q * new Quaternion(v.X, v.Y, v.Z, 0) * q.Inverse();
    return new Vector3(quatForm.X, quatForm.Y, quatForm.Z);
  }
  #endregion



  public class MovementData {
    public const float MOVE_TIME = 0.5F, MOVE_SPEED = 100, Z_OFFSET = -0.25F;
    public static Vector3 CENTER = new Vector3(0, 0, -0.65F);

    public float jumpDuration, moveDistance, jumpGravity, endRotation, jumpOffsetY;
    public Vector3 jumpEnd, moveEnd, moveStart;

    public MovementData(MapInfo.DifficultyBeatmap difficultyBeatmap, BeatMap.Object note) {
      if (!(note is Note || note is Bomb)) throw new System.Exception("Only notes and bombs are allowed here");

      // float njs = difficultyBeatmap.noteJumpMovementSpeed; // in njs???
      // float time = note.b * 60F / difficultyBeatmap.bpm; // in seconds
      jumpDuration = difficultyBeatmap.getHJD() * 60F / difficultyBeatmap.bpm; // in seconds
      float jumpDistance = difficultyBeatmap.getNJD();
      GD.Print(jumpDistance);
      moveDistance = MOVE_TIME * MOVE_SPEED;
      int yBeforeJump = 0;

      //important positions
      moveEnd = CENTER + Vector3.Forward * (jumpDistance * 0.5F); // "start" of note jump, in time with spawn time
      jumpEnd = CENTER - Vector3.Forward * (jumpDistance * 0.5F); // end of jump
      moveStart = CENTER + Vector3.Forward * (moveDistance + jumpDistance * 0.5F); // "spawning animation" start
      jumpOffsetY = getYHeightOffset(REPLAY_HEIGHT);
      Vector3 jumpOffset = new Vector3(getXOffset(note.x), yBeforeJump, Z_OFFSET);
      jumpEnd += jumpOffset;
      if (note is Note) {
        Vector3 moveOffset = new Vector3(getXOffset(note.x), yBeforeJump, Z_OFFSET);
        moveStart += moveOffset;
        moveEnd += moveOffset;
      } else {
        moveStart += jumpOffset;
        moveEnd += jumpOffset;
      }

      if (note is Note n) endRotation = deg2rad(ROTATION_ANGLE[n.d] + n.a);
      else if (note is Bomb b) endRotation = 0;

      jumpGravity = getGravity(note.y, yBeforeJump);
    }


    #region offsetfuncs
    private float getYHeightOffset(float playerHeight) {
      return clamp((playerHeight - 1.8F) * 0.5F, -0.2F, 0.6F);
    }
    private float getXOffset(int x) {
      return X_LAYER_POS[x];
    }
    private float getGravity(int y, int yBeforeJump) {
      float n = jumpDuration / 2;
      float highest = HIGHEST_Y_POS[y] + jumpOffsetY;
      float end = Y_LAYER_POS[yBeforeJump]; // why not y?
      return (2 * (highest - end) / (n * n));
    }
    #endregion

  }
}
