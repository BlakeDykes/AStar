using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMap : MonoBehaviour
{
    public TileBase white;
    public TileBase wall;
    public TileBase frontier;
    public TileBase scanned;
    public TileBase path;
    public Tilemap map;
}
