using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void ClosedMissionBannerCallback(bool _toNext);

public class MissionBanner : MonoBehaviour
{
    protected Animator animator; 
    protected Text text;
    protected Button continueButton;
    protected Button redoButton;
    protected Vector3 redoButtonOriginalPosition;

    public ClosedMissionBannerCallback closedCallback;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        text = GetComponentInChildren<Text>();
        var allButtons = GetComponentsInChildren<Button>();
        continueButton = allButtons[0];
        redoButton = allButtons[1];
        redoButtonOriginalPosition = redoButton.transform.localPosition;
    }

    public void SetText(string _newText){
        text.text = _newText;
    }

    public void Show(
        bool _withContinueButton,
        bool _withRedoButton, 
        ClosedMissionBannerCallback _closedCallback = null
    ){
        StartCoroutine(WaitThenShow(_withContinueButton, _withRedoButton, _closedCallback));
    }

    IEnumerator WaitThenShow(
        bool _withContinueButton,
        bool _withRedoButton, 
        ClosedMissionBannerCallback _closedCallback = null
    ){
        // We have to delay one frame to make sure the current frame did not setup a hiding process
        // otherwise the former overrides the Show process
        yield return null;

        // Prepare button
        continueButton.gameObject.SetActive(_withContinueButton);
        redoButton.gameObject.SetActive(_withRedoButton);
        redoButton.transform.localPosition = _withContinueButton && _withContinueButton 
            ? redoButtonOriginalPosition
            : continueButton.transform.localPosition;

        closedCallback = _closedCallback;

        animator.Play("Panel In");
        if (!_withContinueButton && !_withRedoButton)
            StartCoroutine(WaitThenHide(5));
    }
 
    public void Hide(){
        animator.Play("Panel Out");
        if (closedCallback != null)
            closedCallback(false);
    }
    
    IEnumerator WaitThenHide(float _waitingTime){
        var _startingTime = Time.time;
        while (Time.time - _startingTime < _waitingTime){
            if (Input.anyKeyDown){
                break;
            }
            yield return 0;
        }
        Hide();
    }

    public void ClickedContinue(){
        Hide();
        if (closedCallback != null)
            closedCallback(true);
    }

    public void ClickedRedo(){
        Hide();
        if (closedCallback != null)
            closedCallback(false);
    }
}
