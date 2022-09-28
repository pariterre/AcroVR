using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

/// <summary>
///	 
/// </summary>

public class SliderPlayAnimation : MonoBehaviour
{
    // Variables
    protected DrawManager drawManager;
    protected UIManager uiManager;
    public Slider slider;

    public Text textChrono;
    public Button playButton;
    public Button pauseButton;

    void Start()
    {
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();
        slider.minValue = 0f;

        drawManager.RegisterSliderAnimation(this);
    }

    void Destroy()
    {
        drawManager.UnregisterSliderAnimation();
    }

    void Update()
    {
        if (drawManager.timeElapsed > 0 && !drawManager.isPaused)
            textChrono.text = drawManager.frameNtime + " s";
    }

    public void SetSlider(int value)
    {
        slider.value = value;
    }

    public void ShowPlayButton()
    {
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

    public void ShowPauseButton()
    {
        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }
    
    public void OnPlayAnimationSlider()
    {
        if (drawManager.IsEditing) return;
        
        slider.maxValue = (float)drawManager.numberFrames - 1;
        var currentFrame = (int)slider.value;
        drawManager.SetFrameN(currentFrame);
        textChrono.text = drawManager.frameNtime + " s";

        if (drawManager.cntAvatar > 1)
        {
            var secondCurrentFrame = currentFrame;
            if (secondCurrentFrame >= drawManager.secondNumberFrames)
                secondCurrentFrame = drawManager.secondNumberFrames - 1;
            drawManager.SetSecondFrameN(secondCurrentFrame);
        }

        if (!drawManager.isPaused && ((int)slider.value == 0 || (int)slider.value == (int)slider.maxValue || !drawManager.ShouldContinuePlaying()) )
        {
            drawManager.SetCanResumeAnimation(false);
            ShowPlayButton();
        }

        if (Input.GetMouseButton(0))
        {
            ShowPlayButton();
            drawManager.Pause();
            drawManager.PlayOneFrame();

            if (drawManager.cntAvatar > 1)
                drawManager.secondPaused = true;
        }
    }

    public void EnableSlider()
    {
        slider.interactable = true;
        playButton.interactable = true;
        pauseButton.interactable = true;
    }

    public void DisableSlider()
    {
        slider.interactable = false;
        playButton.interactable = false;
        pauseButton.interactable = false;
    }

}