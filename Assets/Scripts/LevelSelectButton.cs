using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelSelectButton : MonoBehaviour
{

    public string levelToLoad;

    public GameObject star1, star2, star3;
    void Start()
    {
        
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
