using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AStarNode
{
    public AStarNode Parent { get; set; }
    public int Cost { get; set; }
    public int DistanceFromGoal { get; set; }
    public int FScore { get; set; }
    public Vector3Int Pos { get; set; }

    public void SetDistance(Vector3Int goal)
    {
        this.DistanceFromGoal = 10 * Mathf.Abs(goal.y - this.Pos.y) + Mathf.Abs(goal.x - this.Pos.x);

    }

    public void SetFScore()
    {
        this.FScore = DistanceFromGoal + Cost;
    }

    public AStarNode(AStarNode parent, int cost, Vector3Int pos, Vector3Int goal)
    {
        this.Parent = parent;
        this.Pos = pos;
        this.Cost = cost;
        this.DistanceFromGoal = 10 * (Mathf.Abs(goal.x - pos.x) + Mathf.Abs(goal.y - pos.y));
        this.FScore = this.Cost + this.DistanceFromGoal;
    }
}

