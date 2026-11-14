using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
   public void PlayGame()
    {
        
    }
    

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
