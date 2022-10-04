using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void EndOfMissionCallback(); //

public class MissionBanner : MonoBehaviour
{
    protected Animator animator; 
    protected Text text;
    public bool IsShown { get; protected set; } = false;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        text = GetComponentInChildren<Text>();
    }

    public void SetText(string _newText){
        text.text = _newText;
    }

    public void Show(EndOfMissionCallback _closedCallback = null){
        animator.Play("Panel In");
        IsShown = true;
        StartCoroutine(WaitClickToCloseBanner(_closedCallback));
    }

    public void Hide(){
        animator.Play("Panel Out");
        IsShown = false;
    }

    IEnumerator WaitClickToCloseBanner(EndOfMissionCallback _closedCallback){
        while (true){
            if (Input.anyKeyDown)
            {
                Hide();
                if (_closedCallback != null)
                    _closedCallback();
                break;
            }
            yield return null;
        }
    }
}
