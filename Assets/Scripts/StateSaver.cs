using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class StateSaver : MonoBehaviour
{
    private Board board;
    private RoundManager roundManager;
    private string sceneName;
    public float GetScore()
    {
        return roundManager.currentScore;
    }

    public float GetTime()
    {
        return roundManager.roundTime;
    }

    public Gem[,] GetAllGems()
    {
        return board.allGems;
    }

    public void SaveState(Board board, RoundManager roundManager)
    {
        sceneName = SceneManager.GetActiveScene().name;
        State stateToSave = new State(sceneName, roundManager.currentScore, roundManager.roundTime, board.allGems);
        string serializedState = JsonConvert.SerializeObject(stateToSave);
        string fullPath = GetFullPath();
        File.WriteAllText(fullPath, serializedState);
        
    }

    public bool LoadState()
    {
        try
        {
            string serializedState = File.ReadAllText(GetFullPath());
            KeyValuePair<Board, RoundManager> boardAndManager = JsonConvert.DeserializeObject<KeyValuePair<Board, RoundManager>>(serializedState);
            board = boardAndManager.Key;
            roundManager= boardAndManager.Value;
            return true;
        }
        catch (Exception)
        {
            return false;
        }

    }

    private string GetFullPath()
    {
        var baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appStorageFolder = Path.Combine(baseFolder, "Match 3");
        
        var fullPath = Path.Combine(appStorageFolder, sceneName);

        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
        return fullPath;
    }
}
[Serializable]
public class State
{
    public string SceneName { get; private set; }
    public int Score { get; private set; }
    public float Time { get; private set; }

    public Gem[,] AllGems { get; private set; }
    public State(string sceneName, int score, float time, Gem[,] allGems) { 
        
        SceneName = sceneName;
        Score = score;
        Time = time;
        AllGems = allGems;
    }
}
