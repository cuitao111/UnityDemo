using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject pasueMenu;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ShowUI()
    {
        GameObject.Find("Canvas/Mainmenu/UI").SetActive(true);
    }

    public void Pause()
    {
        gameObject.SetActive(false);
    }
}
