using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public enum TileType { Number, Operator }

    public TileType type;
    public string value;
}
