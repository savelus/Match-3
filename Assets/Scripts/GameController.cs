using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Board Board { get; set; }
    public FallManager FallManager { get; set; }
    public SwapManager SwapManager { get; set; }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        //SwapManager
    }

    public void SetBoard(Board board)
    {
        Board = board;
        FallManager.SetBoard(board);
        SwapManager.SetBoard(board);
    }
    
}
