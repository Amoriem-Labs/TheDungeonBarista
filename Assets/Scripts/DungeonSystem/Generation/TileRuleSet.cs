using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "TDB/Dungeon/Tile Rule Set")]
public class TileRuleSet : ScriptableObject
{
    [Tooltip("Higher priority rules are evaluated first. For ties, list order is used.")]
    public List<TileRule> rules = new List<TileRule>();

    public bool TryMatch(TileNeighborMask neighborMask, out TileRule matchedRule, out int rotationSteps)
    {
        matchedRule = null;
        rotationSteps = 0;

        if (rules == null || rules.Count == 0)
            return false;

        int bestPriority = int.MinValue;
        int bestIndex = int.MaxValue;

        for (int index = 0; index < rules.Count; index++)
        {
            TileRule rule = rules[index];
            if (rule == null) continue;

            int maxRotations = rule.allowRotation ? 4 : 1;
            for (int rot = 0; rot < maxRotations; rot++)
            {
                TileNeighborMask filled = RotateMaskClockwise(rule.requiredFilled, rot);
                TileNeighborMask empty = RotateMaskClockwise(rule.requiredEmpty, rot);

                if ((neighborMask & filled) != filled)
                    continue;

                if ((neighborMask & empty) != TileNeighborMask.None)
                    continue;

                bool higherPriority = rule.priority > bestPriority;
                bool samePriorityEarlier = rule.priority == bestPriority && index < bestIndex;

                if (higherPriority || samePriorityEarlier)
                {
                    bestPriority = rule.priority;
                    bestIndex = index;
                    matchedRule = rule;
                    rotationSteps = rot;
                }

                break;
            }
        }

        return matchedRule != null;
    }

    private static TileNeighborMask RotateMaskClockwise(TileNeighborMask mask, int steps)
    {
        steps = ((steps % 4) + 4) % 4;
        TileNeighborMask rotated = mask;

        for (int i = 0; i < steps; i++)
        {
            TileNeighborMask next = TileNeighborMask.None;

            if ((rotated & TileNeighborMask.North) != 0) next |= TileNeighborMask.East;
            if ((rotated & TileNeighborMask.East) != 0) next |= TileNeighborMask.South;
            if ((rotated & TileNeighborMask.South) != 0) next |= TileNeighborMask.West;
            if ((rotated & TileNeighborMask.West) != 0) next |= TileNeighborMask.North;

            if ((rotated & TileNeighborMask.NorthEast) != 0) next |= TileNeighborMask.SouthEast;
            if ((rotated & TileNeighborMask.SouthEast) != 0) next |= TileNeighborMask.SouthWest;
            if ((rotated & TileNeighborMask.SouthWest) != 0) next |= TileNeighborMask.NorthWest;
            if ((rotated & TileNeighborMask.NorthWest) != 0) next |= TileNeighborMask.NorthEast;

            rotated = next;
        }

        return rotated;
    }
}

[Serializable]
public class TileRule
{
    [Tooltip("Only for organization in the inspector.")]
    public TileShape shape = TileShape.Custom;

    [Tooltip("Higher priority rules are checked first.")]
    public int priority = 0;

    [Tooltip("Tile displayed when this rule matches. Leave null to clear the tile at this cell.")]
    public TileBase outputTile;

    [Tooltip("Neighbors that must be present to match this rule.")]
    public TileNeighborMask requiredFilled = TileNeighborMask.None;

    [Tooltip("Neighbors that must be absent to match this rule.")]
    public TileNeighborMask requiredEmpty = TileNeighborMask.None;

    [Tooltip("If enabled, this rule can rotate by 90-degree increments to find a match.")]
    public bool allowRotation = true;
}

public enum TileShape
{
    Full,
    Half,
    Quarter,
    ThreeQuarter,
    Diagonal,
    Empty,
    Custom
}

[Flags]
public enum TileNeighborMask
{
    None = 0,
    North = 1 << 0,
    East = 1 << 1,
    South = 1 << 2,
    West = 1 << 3,
    NorthEast = 1 << 4,
    SouthEast = 1 << 5,
    SouthWest = 1 << 6,
    NorthWest = 1 << 7,
    Cardinals = North | East | South | West,
    Diagonals = NorthEast | SouthEast | SouthWest | NorthWest,
    All = Cardinals | Diagonals
}
