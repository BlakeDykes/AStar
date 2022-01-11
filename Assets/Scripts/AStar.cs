using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class AStar
{
    public int diagonalCost = 14;
    public int straightCost = 10;
    AStarNode Start;
    Vector3Int Goal;
    AStarNode GoalNode;
    TileMap TM;
    List<AStarNode> OpenNodes;
    List<AStarNode> ClosedNodes;
    private bool Found = false;
    private int Iterations = 0;

    public AStar(Vector3Int start, Vector3Int goal, TileMap tm)
    {
        start.z = 0;
        this.Goal = goal;
        this.Start = new AStarNode(null, 0, start, goal);
        this.TM = tm;
        OpenNodes = new List<AStarNode>();
        ClosedNodes = new List<AStarNode>();

        OpenNodes.Add(this.Start);
    }

    public IEnumerator FindAstar(float waitTime)
    {
        //yield return new WaitForSeconds(1.0f);
        float startTime = Time.realtimeSinceStartup;
        while (!Found)
        {
            //yield return new WaitForSeconds(waitTime);
            if(OpenNodes.Count == 0)
            {
                Debug.Log("Goal Unreachable");
                Debug.Log("total time - " + (Time.realtimeSinceStartup - startTime));
                yield break;
            }
            this.ReachableNodes(OpenNodes.First());
            Iterations++;
        }
        CreatePath();
        Debug.Log("total time - " + (Time.realtimeSinceStartup - startTime));
        yield return null;
    }


    public void ReachableNodes(AStarNode current)
    {
        Vector3Int currentPos = current.Pos;
        ClosedNodes.Add(current);
        if(currentPos == Goal)
        {
            Found = true;
            return;
        }
        TM.map.SetTile(current.Pos, TM.scanned);

        OpenNodes.Remove(current);

        int possibleCost;
        for(int i = -1 ; i <= 1; i++)
        {
            for(int j = -1 ; j <= 1; j++)
            {
                if (j == 0 && i == 0)
                    continue;
                
                Vector3Int possiblePos = new Vector3Int(currentPos.x + i, currentPos.y + j, currentPos.z);
                TileBase tile = TM.map.GetTile(possiblePos);
                if (tile == TM.white)
                {
                    possibleCost = Mathf.Abs(i + j) == 1 ? straightCost : diagonalCost;
                    TM.map.SetTile(possiblePos, TM.frontier);
                    AStarNode newNode = new AStarNode(current, current.Cost + possibleCost, possiblePos, this.Goal);
                    if (newNode.Pos == Goal)
                        GoalNode = newNode;
                    if (OpenNodes.Count <= 0)
                    {
                        OpenNodes.Add(newNode);
                        TM.map.SetTile(newNode.Pos, TM.frontier);
                    }
                    else
                    {
                        if(newNode.FScore >= OpenNodes[(OpenNodes.Count -1)].FScore)
                        {
                            OpenNodes.Add(newNode);
                        }
                        else
                        {
                            for (int k = 0; k < OpenNodes.Count; k++)
                            {
                                if (OpenNodes[k].FScore > newNode.FScore)
                                {
                                    OpenNodes.Insert(k, newNode);
                                    break;
                                }
                            }
                        }
                    }
                }
                else if(tile == TM.frontier)
                {
                    AStarNode openNode = OpenNodes.Find(x => x.Pos == possiblePos);
                    possibleCost = Mathf.Abs(i + j) == 1 ? straightCost : diagonalCost;
                    if (openNode != null)
                    {
                        if(openNode.Cost > current.Cost + possibleCost)
                        {
                            openNode.Parent = current;
                            openNode.Cost = current.Cost + possibleCost;
                            openNode.SetFScore();
                            OpenNodes.Remove(openNode);

                            if (openNode.FScore >= OpenNodes[(OpenNodes.Count - 1)].FScore)
                            {
                                OpenNodes.Add(openNode);
                            }
                            else
                            {
                                for (int k = 0; k < OpenNodes.Count; k++)
                                {
                                    if (OpenNodes[k].FScore > openNode.FScore)
                                    {
                                        OpenNodes.Insert(k, openNode);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void CreatePath()
    {
        Debug.Log("Found the goal at " + GoalNode.Pos.x + ", " + GoalNode.Pos.y + " in " + Iterations + " iterations");
        AStarNode cur = GoalNode;
        while (cur.Pos != Start.Pos)
        {
            TM.map.SetTile(cur.Pos, TM.path);
            cur = cur.Parent;
        }
    }

}
