using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject bgTilePrefab;

    public Gem[] gems;
    public SuperGem[] superGems;
    public Gem[,] allGems;

    public float gemSpeed;

    [HideInInspector]
    public MatchFinder matchFinder;

    public FallManager fallManager;
    public MonsterManager monsterManager;
    public SwapManager swapManager;
    public enum BoardState {  wait, move}
    public BoardState currentState = BoardState.move;

    public Gem bomb;
    public float bombChance = 2f;

    [HideInInspector]
    public RoundManager roundManager;
    private BoardLayout boardLayout;
    private Gem[,] layoutStore;
    private float bonusMulti;
    public float bonusAmount = .5f;

    private StateSaver stateSaver;
    private void Awake()
    {
        matchFinder = FindObjectOfType<MatchFinder>();
        roundManager = FindObjectOfType<RoundManager>();
        boardLayout = GetComponent<BoardLayout>();
        stateSaver = GetComponent<StateSaver>();
    }

    void Start()
    {
        allGems = new Gem[width,height];

        layoutStore = new Gem[width, height];

        
        
        fallManager.SetBoard(this);
        swapManager.SetBoard(this);
        Setup();
    }

    public void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.S))
        {
            ShuffleBoard();
        }
    }

    private void Setup()
    {

        if(boardLayout != null)
        {
            layoutStore = boardLayout.GetLayout();
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new(x, y);
                GameObject bgTile = Instantiate(bgTilePrefab, pos, Quaternion.identity);
                bgTile.transform.parent = transform;
                bgTile.name = $"BG Tile - {x} , {y}";

                if (layoutStore[x, y] != null)
                {
                    SpawnGem(new Vector2Int(x, y), layoutStore[x, y]);
                }
                else
                {
                    int gemToUse = Random.Range(0, gems.Length);

                    while (MatchesAt(new Vector2Int(x, y), gems[gemToUse]))
                    {
                        gemToUse = Random.Range(0, gems.Length);
                    }
                    SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
                }
            }
        }
    }

    private void SpawnGem(Vector2Int pos, Gem gemToSpawn)
    {
        if(Random.Range(0f, 100f) < bombChance)
        {
            gemToSpawn = bomb;
        }
        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity);
        gem.transform.parent = transform;
        gem.name = $"Gem - {pos.x} , {pos.y}";
        allGems[pos.x, pos.y] = gem;
        gem.SetupGem(pos, this);
        
    }

    public void SetupSuperGems()
    {
        foreach (var superGem in matchFinder.superGemOnBoard)
        {
            SuperGem superGemToSpawn = FindSuperGemToSpawn(superGem);
            if (superGemToSpawn != null) 
            { 
                SuperGem sgem = Instantiate(superGemToSpawn,
                    new Vector3(superGem.posIndex.x, superGem.posIndex.y + height, 0f),
                    Quaternion.identity);
                sgem.transform.parent = transform;
                sgem.name = $"SGem - {superGem.posIndex.x} , {superGem.posIndex.y}";
                Destroy(allGems[superGem.posIndex.x, superGem.posIndex.y].gameObject);
                allGems[superGem.posIndex.x, superGem.posIndex.y] = sgem;
                sgem.SetupGem(superGem.posIndex, this);
            }
        }
        matchFinder.superGemOnBoard.Clear();
    }

    private SuperGem FindSuperGemToSpawn(SuperGem superGem)
    {
        foreach (var superGemToSpawn in superGems)
        {
            if(superGemToSpawn.type == superGem.type && superGemToSpawn.Boost == superGem.Boost)
            {
                return superGemToSpawn;
            }
        }
        return null;
    }
    bool MatchesAt(Vector2Int positionToCheck, Gem gemToCheck)
    {
        if (positionToCheck.x > 1)
        {
            if (allGems[positionToCheck.x - 1, positionToCheck.y].type == gemToCheck.type &&
                allGems[positionToCheck.x - 2, positionToCheck.y].type == gemToCheck.type)
            {
                return true;
            }
        }
        if (positionToCheck.y > 1)
        {
            if (allGems[positionToCheck.x, positionToCheck.y - 1].type == gemToCheck.type &&
                allGems[positionToCheck.x, positionToCheck.y - 2].type == gemToCheck.type)
            {
                return true;
            }
        }
        return false;
    }
    

    private void DestroyMatchedGemAt(Vector2Int position)
    {
        if (allGems[position.x, position.y] != null)
        {
            if (allGems[position.x, position.y].isMatched)
            {

                if (allGems[position.x, position.y].type  == Gem.GemType.bomb)
                {
                    SFXManager.instance.PlayExplode();
                }
                else if (allGems[position.x, position.y].type == Gem.GemType.stone)
                {
                    SFXManager.instance.PlayStoneBreak();
                }
                else
                {
                    SFXManager.instance.PlayGemBreak();
                }

                Instantiate(allGems[position.x, position.y].destroyEffect, 
                    new Vector2(position.x, position.y), Quaternion.identity);

                MoveAndDestroyGemOnMonster(position);

            }
        }
    }

    private void MoveAndDestroyGemOnMonster(Vector2Int position)
    {

        var monsterPosition = monsterManager.Monster.transform.position;
        var gem = allGems[position.x, position.y];
        allGems[position.x, position.y] = null;
        DOTween.Sequence().Append(gem.transform.DOMove(monsterPosition, 0.3f))
            .Join(gem.transform.DOScale(0.5f, 0.3f))
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                Instantiate(monsterManager.Monster.destroyEffect, monsterPosition, Quaternion.identity);
                Destroy(gem.gameObject);
            });
    }


    public void DestroyMatches()
    {
        for (int i = 0; i < matchFinder.currentMatches.Count; i++)
        {
            if (matchFinder.currentMatches[i] != null)
            {
                scoreCheck(matchFinder.currentMatches[i]);
                DestroyMatchedGemAt(matchFinder.currentMatches[i].posIndex);
            }
        }
        SetupSuperGems();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(.2f);

        int nullCounter = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x,y ] == null)
                {
                    nullCounter++;
                }
                else if(nullCounter > 0)
                {
                    allGems[x, y].posIndex.y -= nullCounter;
                    allGems[x, y - nullCounter] = allGems[x, y];
                    allGems[x, y] = null;
                }
            }
            nullCounter = 0;
        }

        StartCoroutine(FillBoardCo());
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(.5f);

        RefillBoard();

        yield return new WaitForSeconds(.5f);

        matchFinder.FindAllMatches();
        if(matchFinder.currentMatches.Count > 0)
        {
            bonusMulti++;

            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            currentState = BoardState.move;

            bonusMulti = 0;
        }
    }

    private void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    int gemToUse = Random.Range(0, gems.Length);
                    SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
                }
            }
        }

        CheckMisplacedGems();
    }

    private void CheckMisplacedGems()
    {
        List<Gem> foundGems = new();

        foundGems.Add(FindObjectOfType<Gem>());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (foundGems.Contains(allGems[x, y]))
                {
                    foundGems.Remove(allGems[x, y]);
                }
            }
        }

        foreach (var gem in foundGems)
        {
            Destroy(gem.gameObject);
        }
    }

    public void ShuffleBoard()
    {
        if(currentState != BoardState.wait)
        {
            currentState = BoardState.wait;

            List<Gem> gemsFromBoard = new();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gemsFromBoard.Add(allGems[x, y]);
                    allGems[x, y] = null;

                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int gemToUse = Random.Range(0, gemsFromBoard.Count);

                    int iterations = 0;
                    while(MatchesAt(new Vector2Int(x,y), gemsFromBoard[gemToUse]) 
                        && iterations < 100 && gemsFromBoard.Count > 1)
                    {
                        gemToUse = Random.Range(0, gemsFromBoard.Count);
                        iterations++;
                    }

                    gemsFromBoard[gemToUse].SetupGem(new Vector2Int(x, y), this);
                    allGems[x, y] = gemsFromBoard[gemToUse];
                    gemsFromBoard.RemoveAt(gemToUse);
                }
            }
            StartCoroutine(FillBoardCo());
            
        }

    }

    public void scoreCheck(Gem gemToCheck)
    {
        float score = gemToCheck.scoreValue;
        if (bonusMulti > 0)
        {
            score += gemToCheck.scoreValue * bonusMulti * bonusAmount;
        }

        roundManager.currentScore += Mathf.RoundToInt(score);
        monsterManager.DoDamage(Mathf.RoundToInt(score));
    }

    public void SaveBoard()
    {
        var time = roundManager.roundTime;
        var score = roundManager.currentScore;
        
    }
}

