using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallManager : MonoBehaviour
{
    private Board board;

    public bool isCanFall = true;
    public void SetBoard(Board board)
    {
        this.board = board;
    }
    public void MoveAllGems(Gem[,] allGems)
    {
        if (board != null)
        {
            foreach (var gem in board.allGems)
            {
                if (gem == null) continue;
                if (Vector2.Distance(gem.transform.position, gem.posIndex) > .01f)
                {
                    gem.transform.position = Vector2.Lerp(gem.transform.position, gem.posIndex, board.gemSpeed * Time.deltaTime);
                }
                else
                {
                    gem.transform.position = new Vector3(gem.posIndex.x, gem.posIndex.y, 0f);
                    board.allGems[gem.posIndex.x, gem.posIndex.y] = gem;
                }
            }
        }
    }

    public void MoveGem(Gem gem)
    {
        if (board != null && gem != null)
        {
            if (Vector2.Distance(gem.transform.position, gem.posIndex) > .01f)
            {
                gem.transform.position = Vector2.Lerp(gem.transform.position, gem.posIndex, board.gemSpeed * Time.deltaTime);
            }
            else
            {
                gem.transform.position = new Vector3(gem.posIndex.x, gem.posIndex.y, 0f);
                board.allGems[gem.posIndex.x, gem.posIndex.y] = gem;
            }
            
        }
    }


    private void Update()
    {

        if (board != null && isCanFall)
        {
            foreach (var gem in board.allGems)
            {
                if (gem == null) continue;
                if (Vector2.Distance(gem.transform.position, gem.posIndex) > .01f)
                {
                    gem.transform.position = Vector2.Lerp(gem.transform.position, gem.posIndex, board.gemSpeed * Time.deltaTime);
                }
                else
                {
                    gem.transform.position = new Vector3(gem.posIndex.x, gem.posIndex.y, 0f);
                    board.allGems[gem.posIndex.x, gem.posIndex.y] = gem;
                }
            }
        }
    }
}
