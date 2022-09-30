using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;

public enum SceneState
{
    Logo,
    MainMenu,
    EndScreen,
    Profile,
    Training,
}

public class LevelManager : MonoBehaviour
{
    public SceneState currentState = 0;
    Stack<GameObject> sceneStack = new Stack<GameObject>();

    public bool IsLevelScene()
    {
        if (currentState == SceneState.MainMenu || currentState == SceneState.EndScreen)
            return false;
        else
            return true;
    }

    private void Awake()
    {
        currentState = SceneState.Logo;
    }

    public void Init()
    {
        currentState = SceneState.MainMenu;
    }

    public void NextLevel()
    {
        if (currentState == SceneState.EndScreen) Init();
        else
        {
            currentState++;
        }

        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(currentState.ToString());
        while (!op.isDone)
        {
            yield return null;
        }
    }

    public void CurrentScreen()
    {
        StartCoroutine(LoadScene());
    }

    public void GotoScreen(string screen)
    {
        if (screen == "Profile") currentState = SceneState.Profile;
        if (screen == "MainMenu") currentState = SceneState.MainMenu;
        if (screen == "Training") currentState = SceneState.Training;
        StartCoroutine(LoadScene());
    }
}
