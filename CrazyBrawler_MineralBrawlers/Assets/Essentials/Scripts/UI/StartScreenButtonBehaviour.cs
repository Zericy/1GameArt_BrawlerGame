using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenButtonBehaviour : MonoBehaviour
{
    public void GoToScene(string SceneNameToLoad)
    {
        SceneManager.LoadScene(SceneNameToLoad);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
