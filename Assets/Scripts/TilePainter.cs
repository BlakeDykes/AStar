using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TilePainter : MonoBehaviour
{
    public Camera MainCamera;
    public GameObject ShanePrefab;
    public GameObject SkullPrefab;
    public int MaxWallDistance = 10;
    public int MaxWalls = 30;
    public int WallOffset = 1;
    private int x, y, maxX, maxY;
    

    public TileMap TM;
    IEnumerator Pathfind;


    public void Start()
    {

        TM = GetComponent<TileMap>();

        Vector3 max = MainCamera.ScreenToWorldPoint(new Vector3(MainCamera.pixelWidth, MainCamera.pixelHeight));
        Vector3 min = MainCamera.ScreenToWorldPoint(new Vector3(0, 0));
        
        x = Mathf.RoundToInt(min.x);
        y = Mathf.RoundToInt(min.y);
        maxX = Mathf.RoundToInt(max.x);
        maxY = Mathf.RoundToInt(max.y);
        TileBase setTile = TM.white;



        int whiteTiles = 0;

        for(int i = y; i <= maxY; i++)
        {
            for(int j = x; j <= maxX; j++)
            {
                setTile = TM.white;
                if (j <= x + WallOffset || j >= maxX - WallOffset || i <= y + WallOffset || i >= maxY - WallOffset)
                    setTile = TM.wall;
                TM.map.SetTile(new Vector3Int(j, i), setTile);
                if(setTile == TM.white)
                    ++whiteTiles;
            }
        }

        Vector3Int characterPos = new Vector3Int(0, 0, -1);
        GameObject character = Instantiate(ShanePrefab, characterPos, Quaternion.identity);
        GameObject goal = Instantiate(SkullPrefab, new Vector3Int(Random.Range(x + WallOffset + 1, maxX - WallOffset - 1), Random.Range(y + WallOffset + 1, maxY - WallOffset - 1), -1), Quaternion.identity);
        Vector3Int goalPos = new Vector3Int(Mathf.RoundToInt(goal.transform.position.x), Mathf.RoundToInt(goal.transform.position.y), 0);
        int wallTiles = 0;
        for (int i = 0; i <= MaxWalls; i++)
        {
            Vector3Int positionStart = new Vector3Int(Random.Range(x + WallOffset, maxX - MaxWallDistance), Random.Range(y + WallOffset, maxY - MaxWallDistance));
            Vector3Int positionEnd = new Vector3Int(Random.Range(positionStart.x, positionStart.x + MaxWallDistance), Random.Range(positionStart.y, positionStart.y + MaxWallDistance));
            wallTiles += positionEnd.x - positionStart.x + positionEnd.y - positionStart.y;
            for (int j = positionStart.y; j <= positionEnd.y; j++)
            {
                for (int h = positionStart.x; h <= positionEnd.x; h++)
                {
                    Vector3Int pos = new Vector3Int(h, j);
                    if (TM.map.GetTile(pos) != TM.wall && new Vector3Int(0, 0, 0) != pos && goalPos != pos)
                    {
                        TM.map.SetTile(new Vector3Int(h, j), TM.wall);
                        whiteTiles--;
                    }
                }
            }
        }

        AStar aStar = new AStar(characterPos, goalPos, TM);

        Pathfind = aStar.FindAstar(0.025f);
        StartCoroutine(Pathfind);
    }

}
