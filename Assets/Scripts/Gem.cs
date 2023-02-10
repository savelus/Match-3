using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    //[HideInInspector]
    public Vector2Int posIndex;
    //[HideInInspector]
    public Board board;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;

    private bool isMousePressed;
    private float swipeAngel = 0;

    private Gem otherGem;

    public bool isMatched;

    private Vector2Int previousPosition;
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
