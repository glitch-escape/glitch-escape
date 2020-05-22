using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility class for getting glitch escape physics layers. Added this so that if we ever change how we're using layers
/// all layer masks can be put (and updated) in one location.
/// </summary>
public class LayerMasks {
    public static int FloorGeometry => LayerMask.GetMask("Default", "BlueMaze", "PinkMaze");
    public static int Walls => LayerMask.GetMask("Wall");
    public static int FloorAndWalls => LayerMask.GetMask("Default", "Wall", "BlueMaze", "PinkMaze");
}
