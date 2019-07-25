using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPanel : MonoBehaviour
{
    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Play()
    {
        Time.timeScale = 1;
    }

    public void Reset()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}
