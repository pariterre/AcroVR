using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void ClosedMissionBannerCallback();

public class MissionBanner : MonoBehaviour
{
    protected Animator animator; 
    protected Text text;
    protected Button continueButton;
    public bool IsShown { get; protected set; } = false;

    public ClosedMissionBannerCallback closedCallback;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        text = GetComponentInChildren<Text>();
        var allButtons = GetComponentsInChildren<Button>();
        continueButton = allButtons[0];
    }

    public void SetText(string _newText){
        text.text = _newText;
    }

    public void Show(ClosedMissionBannerCallback _closedCallback = null){
        animator.Play("Panel In");
        IsShown = true;
        closedCallback = _closedCallback;
    }

    public void Hide(){
        animator.Play("Panel Out");
        IsShown = false;
        if (closedCallback != null)
            closedCallback();

    }

    public void ClickedContinue(){
        Debug.Log("Coucou");
        if (!IsShown) return;
        Hide();
    }
}
