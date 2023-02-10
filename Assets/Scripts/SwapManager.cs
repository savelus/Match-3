using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SwapManager : MonoBehaviour
{
    public FallManager fallManager;
    private Board board;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;

    private Vector2Int firstGemPosition;
    private Vector2Int finalGemPosition;

    private bool isMousePressed;
    private float swipeAngel = 0;

    private Gem firstGem;
    private Gem finalGem;
    public void SetBoard(Board board)
    {
        this.board = board;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fallManager.isCanFall= false;

            if (board.currentState == Board.BoardState.move && board.roundManager.roundTime > 0)
            {
                firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                isMousePressed = true;

                firstGemPosition = CalculateGemCoord(firstTouchPosition);
                firstGem = board.allGems[firstGemPosition.x, firstGemPosition.y];

                Debug.Log(firstTouchPosition);
                Debug.Log(firstGemPosition);
                
            }
        }
        else if (isMousePressed && (Input.GetMouseButtonUp(0)))
        {
            isMousePressed = false;

            if (board.currentState == Board.BoardState.move && board.roundManager.roundTime > 0)
            {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                CalculateAngel();
            }
        }
        else return;
        fallManager.isCanFall = true;
    }


    private Vector2Int CalculateGemCoord( Vector2 localPosition)
    {
        var x = (int)(localPosition.x + 0.5);
        var y = (int)(localPosition.y + 0.5);
        return new Vector2Int(x, y);
    }
    private void CalculateAngel()
    {
        swipeAngel = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y,
                                 finalTouchPosition.x - firstTouchPosition.x);
        swipeAngel = swipeAngel * 180 / Mathf.PI;
        Debug.Log(swipeAngel);
        if (Vector3.Distance(firstTouchPosition, finalTouchPosition) > .5f)
        {
            MovePieces();
        }
    }

    private void MovePieces()
    {
        if (swipeAngel < 45 && swipeAngel >= -45 && firstGemPosition.x < board.width - 1)
        {
            finalGem = board.allGems[firstGemPosition.x + 1, firstGemPosition.y];
            finalGemPosition = finalGem.posIndex;
            finalGem.posIndex = firstGemPosition;
        }
        else if (swipeAngel >= 45 && swipeAngel <= 135 && firstGemPosition.y < board.height - 1)
        {
            finalGem = board.allGems[firstGemPosition.x, firstGemPosition.y + 1];
            finalGemPosition = finalGem.posIndex;
            finalGem.posIndex = firstGemPosition;
        }
        else if (swipeAngel < -45 && swipeAngel >= -135 && firstGemPosition.y > 0)
        {
            finalGem = board.allGems[firstGemPosition.x, firstGemPosition.y - 1];
            finalGemPosition = finalGem.posIndex;
            finalGem.posIndex = firstGemPosition;
        }
        else if (swipeAngel > 135 || swipeAngel < -135 && firstGemPosition.x > 0)
        {
            finalGem = board.allGems[firstGemPosition.x - 1, firstGemPosition.y];
            finalGemPosition = finalGem.posIndex;
            finalGem.posIndex = firstGemPosition;
        }
        
        firstGem.posIndex = finalGemPosition;
        board.allGems[finalGemPosition.x, finalGemPosition.y] = firstGem;
        board.allGems[firstGemPosition.x, firstGemPosition.y] = finalGem;
        
        StartCoroutine(CheckMoveCo());
    }

    public IEnumerator CheckMoveCo()
    {
        board.currentState = Board.BoardState.wait;
        yield return new WaitForSeconds(.5f);

        board.matchFinder.FindAllMatches();

        if (finalGem != null)
        {
            if (!firstGem.isMatched && !finalGem.isMatched)
            {
                finalGem.posIndex = finalGemPosition;
                firstGem.posIndex = firstGemPosition;

                board.allGems[firstGemPosition.x, firstGemPosition.y] = firstGem;
                board.allGems[finalGemPosition.x, finalGemPosition.y] = finalGem;

                //fallManager.MoveGem(firstGem);
                //fallManager.MoveGem(finalGem);

                yield return new WaitForSeconds(.5f);
                board.currentState = Board.BoardState.move;
            }
            else
            {
                board.DestroyMatches();
            }
        }
    }
}
