using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
//    public GameObject mainCanvas;
    public Animator animatorComponent;

    private void Start()
    {
        animatorComponent.GetComponent<Animator>();
        Invoke("AutoToMainMenu", 2.0f);
    }

    void ToMainMenu()
    {
        ToolBox.GetInstance().GetManager<LevelManager>().NextLevel();
        Destroy(this.gameObject);
    }

    void AutoToMainMenu()
    {
        animatorComponent.Play("Curaphic Splash Fade-out");
        Invoke("ToMainMenu", 0.5f);
    }

    void Update()
    { 
        if (Input.anyKeyDown)
        {
            animatorComponent.Play("Curaphic Splash Fade-out");
            CancelInvoke();
            Invoke("ToMainMenu", 0.5f);
        }
    }
}
