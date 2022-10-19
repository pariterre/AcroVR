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

    public bool IsShown = false;
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
        if (IsShown) yield break;

        // Prepare button
        IsShown = true;
        continueButton.gameObject.SetActive(_withContinueButton);
        redoButton.gameObject.SetActive(_withRedoButton);
        redoButton.transform.localPosition = _withContinueButton && _withContinueButton 
            ? redoButtonOriginalPosition
            : continueButton.transform.localPosition;

        closedCallback = _closedCallback;

        animator.Play("Panel In");
        if (!_withContinueButton && !_withRedoButton){ 
            // If no button are shown
            StartCoroutine(WaitThenHide(5));
        }
        StartCoroutine(WaitClickOnBannerThenHide(_withRedoButton));
    }
 
    public void Hide(bool _continue = false){
        if (!IsShown) return;

        IsShown = false;
        animator.Play("Panel Out");
        if (closedCallback != null)
            closedCallback(_continue);
    }
    
    IEnumerator WaitThenHide(float _waitingTime){
        var _startingTime = Time.time;
        while (Time.time - _startingTime < _waitingTime){
            if (!IsShown) yield break;
            
            yield return 0;
        }
        Hide();
    }

    IEnumerator WaitClickOnBannerThenHide(bool _hasRedo){
        while (IsShown){
            if (Input.GetMouseButtonUp(0)){
                yield return 0;  // Make sure to finish treating clicked on button if it was

                if (_hasRedo)
                    ClickedRedo();
                else
                    ClickedContinue();
                break;
            }
            yield return 0;
        }
    }

    public void ClickedContinue(){
        Hide(true);
    }

    public void ClickedRedo(){
        Hide(false);
    }
}
