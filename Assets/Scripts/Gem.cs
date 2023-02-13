using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    //[HideInInspector]
    public Vector2Int posIndex;
    //[HideInInspector]
    public Board board;

    public bool isMatched;
    public enum GemType { blue, green, red, yellow, purple, bomb, stone}

    public GemType type;

    public GameObject destroyEffect;

    public int blastSize = 2;

    public int scoreValue = 10;

    public void SetupGem(Vector2Int pos, Board board)
    {
        posIndex = pos;
        this.board = board;
    }

}
