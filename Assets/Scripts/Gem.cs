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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isMousePressed && Input.GetMouseButtonUp(0))
        {
            isMousePressed = false;

            if (board.currentState == Board.BoardState.move && board.roundManager.roundTime > 0)
            {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngel();
            }
        }
    }

    public void SetupGem(Vector2Int pos, Board board)
    {
        posIndex = pos;
        this.board = board;
    }

    private void OnMouseDown()
    {
        if (board.currentState == Board.BoardState.move && board.roundManager.roundTime > 0)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isMousePressed = true;
        }
    }

    private void CalculateAngel()
    {
        swipeAngel = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y,
                                 finalTouchPosition.x - firstTouchPosition.x);
        swipeAngel = swipeAngel * 180 / Mathf.PI;
        Debug.Log(swipeAngel);
        if(Vector3.Distance(firstTouchPosition, finalTouchPosition) > .5f)
        {
            MovePieces();
        }
        
    }

    private void MovePieces()
    {
        previousPosition = posIndex;
        if(swipeAngel < 45 && swipeAngel >= -45 && posIndex.x < board.width - 1)
        {
            otherGem = board.allGems[posIndex.x + 1, posIndex.y];
            otherGem.posIndex.x--;
            posIndex.x++;
        }
        else if (swipeAngel > 45 && swipeAngel <= 135 && posIndex.y < board.height- 1)
        {
            otherGem = board.allGems[posIndex.x, posIndex.y + 1];
            otherGem.posIndex.y--;
            posIndex.y++;
        }
        else if (swipeAngel < -45 && swipeAngel >= -135 && posIndex.y > 0)
        {
            otherGem = board.allGems[posIndex.x, posIndex.y - 1];
            otherGem.posIndex.y++;
            posIndex.y--;
        }
        else if (swipeAngel > 135 || swipeAngel < -135 && posIndex.x > 0)
        {
            otherGem = board.allGems[posIndex.x - 1, posIndex.y];
            otherGem.posIndex.x++;
            posIndex.x--;
        }

        board.allGems[posIndex.x, posIndex.y] = this;
        board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

        StartCoroutine(CheckMoveCo());
    }

    public IEnumerator CheckMoveCo()
    {
        board.currentState = Board.BoardState.wait;
        yield return new WaitForSeconds(.5f);

        board.matchFinder.FindAllMatches();

        if(otherGem != null) 
        {
            if (!isMatched && !otherGem.isMatched)
            {
                otherGem.posIndex = posIndex;
                posIndex = previousPosition;

                board.allGems[posIndex.x, posIndex.y] = this;
                board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

                yield return new WaitForSeconds(.5f);
                board.currentState= Board.BoardState.move;
            }
            else
            {
                board.DestroyMatches();
            }
        }
    }
}
