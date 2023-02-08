using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;
using System;

public class MatchFinder : MonoBehaviour
{
    private Board board;

    public List<Gem> currentMatches = new();
    public List<SuperGem> superGemOnBoard = new();
    private void Awake()
    {
            board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        currentMatches.Clear();
        superGemOnBoard.Clear();
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                Gem currentGem = board.allGems[x, y];
                if(currentGem != null && currentGem.type != Gem.GemType.stone)
                {
                    if (x > 0 && x < board.width - 1)
                    {
                        Gem leftGem = board.allGems[x - 1, y];
                        Gem rightGem = board.allGems[x + 1, y];

                        List<Gem> farGems = new();
                        if (x < board.width - 2)
                        {
                            farGems.Add(board.allGems[x + 2, y]);
                        }
                        if (x < board.width - 3)
                        {
                            farGems.Add(board.allGems[x + 3, y]);
                        }
                        WorkWithLine(leftGem, currentGem, rightGem, farGems, SuperGem.BoostType.horizontal);
                    }

                    if (y > 0 && y < board.height - 1)
                    {
                        Gem aboveGem = board.allGems[x, y + 1];
                        Gem belowGem = board.allGems[x, y - 1];

                        List<Gem> farGems = new();

                        if (y < board.height - 2) //смущает такая конструкция, может есть какой-то другой вариант
                        {
                            farGems.Add(board.allGems[x, y + 2]);
                        }
                        if (y < board.height - 3)
                        {
                            farGems.Add(board.allGems[x, y + 3]);
                        }
                        WorkWithLine(belowGem, currentGem, aboveGem, farGems, SuperGem.BoostType.vertical);
                    }
                }
            }
        }

        if(currentMatches.Count > 0)
        {
            currentMatches = currentMatches.Distinct().ToList();
        }

        CheckForBombs();
        CheckForSuperGem();
        board.SetupSuperGems();
    }

    private bool IsAvailableForChecking(Gem firstNeighbour, Gem secondNeighbour)
    {
        return firstNeighbour != null && firstNeighbour.type != Gem.GemType.stone
            && secondNeighbour != null && secondNeighbour.type != Gem.GemType.stone;
    }

    private void WorkWithLine(Gem previousGem, Gem currentGem, Gem nextGem, List<Gem>farGems, SuperGem.BoostType boostType) 
        //можно было бы собрать одним списком, но тогда будет не понятно отличие и расположение кристаллов
    {
        if(IsAvailableForChecking(previousGem, nextGem)) 
        {
            if( previousGem.type == currentGem.type && currentGem.type == nextGem.type)
            {
                MarkGemsinLine(new Gem[3] {previousGem, currentGem, nextGem});
                if((farGems.Count == 1 && farGems[0].type == currentGem.type) || 
                   (farGems.Count == 2 && farGems[0].type == currentGem.type && farGems[1].type != currentGem.type))
                {
                    SuperGem superGem = new(currentGem, boostType);
                    superGemOnBoard.Add(superGem);
                }
            }
        }
    }

    public void CheckForSuperGem()
    {
        List<Gem> gemsToMark = new List<Gem>();
        foreach (var gem in currentMatches)
        {
            if(gem is SuperGem superGem)
            {
                if (superGem.Boost == SuperGem.BoostType.horizontal)
                {
                    for (int i = 0; i < board.width; i++)
                    {
                        gemsToMark.Add(board.allGems[i, superGem.posIndex.y]);
                    }
                }
                else if (superGem.Boost == SuperGem.BoostType.vertical)
                {
                    for (int i = 0; i < board.height; i++)
                    {
                        gemsToMark.Add(board.allGems[superGem.posIndex.x, i]);
                    }
                }
                else
                {
                    throw new Exception("Не опознанный буст");
                }
            }
        }
        MarkGemsinLine(gemsToMark.ToArray());
    }
    public void CheckForBombs()
    {
        for (int i = 0; i < currentMatches.Count; i++)
        {
            Gem gem = currentMatches[i];

            int x = gem.posIndex.x;
            int y = gem.posIndex.y;

            if(gem.posIndex.x > 0)
            {
                CheckForBombOnCoord(x - 1, y);
            }

            if (gem.posIndex.x < board.width - 1)
            {
                CheckForBombOnCoord(x + 1, y);
            }

            if (gem.posIndex.y > 0)
            {
                CheckForBombOnCoord(x, y - 1);
            }

            if (gem.posIndex.y < board.height - 1)
            {
                CheckForBombOnCoord(x, y + 1);
            }
        }
    }

    private void CheckForBombOnCoord(int bombX, int bombY)
    {
        if (board.allGems[bombX, bombY] != null && board.allGems[bombX, bombY].type == Gem.GemType.bomb)
        {
            MarkBombArea(new Vector2Int(bombX, bombY), board.allGems[bombX, bombY]);
        }
    }
    public void MarkGemsinLine(Gem[] gems)
    {
        foreach (var gem in gems)
        {
            gem.isMatched = true;
            currentMatches.Add(gem);
        }
        currentMatches = currentMatches.Distinct().ToList();
    }

    public void MarkBombArea(Vector2Int bombPosition, Gem theBomb)
    {
        for (int x = bombPosition.x - theBomb.blastSize; x <= bombPosition.x + theBomb.blastSize; x++)
        {
            for (int y = bombPosition.y - theBomb.blastSize; y <= bombPosition.y + theBomb.blastSize; y++)
            {
                if(x >= 0 && x < board.width && y >= 0 && y < board.height)
                {
                    if (board.allGems[x,y] != null)
                    {
                        board.allGems[x, y].isMatched = true;
                        currentMatches.Add(board.allGems[x, y]);
                    }
                }
            }
        }
        currentMatches = currentMatches.Distinct().ToList();
    }
}
