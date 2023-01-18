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
                if(currentGem != null)
                {
                    if( x > 0 && x < board.width - 1)
                    {
                        Gem leftGem = board.allGems[x - 1, y];
                        Gem rightGem = board.allGems[x + 1, y];
                        if (leftGem != null && rightGem != null)
                        {
                            if (leftGem.type == currentGem.type && rightGem.type == currentGem.type && currentGem.type != Gem.GemType.stone)
                            {
                                MarkGemsinLine(new Gem[] { leftGem, currentGem, rightGem });
                                if(x < board.width - 2 && currentGem.type == board.allGems[x + 2, y].type)
                                {
                                    if (x + 3 < board.width && currentGem.type != board.allGems[x+3, y].type || x + 3 >= board.width)
                                    {
                                        
                                        SuperGem superGem = new(currentGem, SuperGem.BoostType.horizontal);
                                        superGemOnBoard.Add(superGem);
                                    }

                                }
                            }
                        }

                        
                    }

                    if (y > 0 && y < board.height - 1)
                    {
                        Gem aboveGem = board.allGems[x, y + 1];
                        Gem belowGem = board.allGems[x, y - 1];
                        if (aboveGem != null && belowGem != null)
                        {
                            if (aboveGem.type == currentGem.type && belowGem.type == currentGem.type && currentGem.type != Gem.GemType.stone)
                            {
                                MarkGemsinLine(new Gem[] { aboveGem, currentGem, belowGem });
                                if (y < board.height - 2 && currentGem.type == board.allGems[x, y + 2].type)
                                {
                                    if (y + 3 < board.width && currentGem.type != board.allGems[x, y + 3].type || y + 3 >= board.height)
                                    {
                                        SuperGem superGem = new(currentGem, SuperGem.BoostType.vertical);
                                        superGemOnBoard.Add(superGem);
                                    }

                                }
                            }
                        }
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
                    throw new Exception("�� ���������� ����");
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
                if (board.allGems[x - 1, y] != null)
                {
                    if(board.allGems[x - 1, y].type == Gem.GemType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x-1, y), board.allGems[x - 1, y]);
                    }
                }
            }

            if (gem.posIndex.x < board.width - 1)
            {
                if (board.allGems[x + 1, y] != null)
                {
                    if (board.allGems[x + 1, y].type == Gem.GemType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x + 1, y), board.allGems[x + 1, y]);
                    }
                }
            }

            if (gem.posIndex.y > 0)
            {
                if (board.allGems[x, y - 1] != null)
                {
                    if (board.allGems[x, y - 1].type == Gem.GemType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x, y - 1), board.allGems[x, y - 1]);
                    }
                }
            }

            if (gem.posIndex.y < board.height - 1)
            {
                if (board.allGems[x, y + 1] != null)
                {
                    if (board.allGems[x, y + 1].type == Gem.GemType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x, y + 1), board.allGems[x, y + 1]);
                    }
                }
            }
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
