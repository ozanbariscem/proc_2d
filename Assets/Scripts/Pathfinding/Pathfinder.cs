#region License
// ====================================================
// Project Porcupine Copyright(C) 2016 Team Porcupine
// This program comes with ABSOLUTELY NO WARRANTY; This is free software,
// and you are welcome to redistribute it under certain conditions; See
// file LICENSE, which is part of this source code package, for details.
// ====================================================
#endregion

using UnityEngine;

public static class Pathfinder
{
    public delegate float PathfindingHeuristic(Cell cell);

    public static PathfindingHeuristic DefaultDistanceHeuristic(Cell goalCell)
    {
        return ManhattanDistance(goalCell);
    }

    public static PathfindingHeuristic ManhattanDistance(Cell goalCell)
    {
        return tile => Mathf.Abs(tile.TrueX - goalCell.TrueX) + Mathf.Abs(tile.TrueY - goalCell.TrueY);
    }
}