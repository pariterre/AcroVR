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
        if (drawManager.numberFrames >= drawManager.secondNumberFrames)
            slider.value = drawManager.frameN;
        else 
            slider.value = drawManager.secondFrameN;

        if(drawManager.cntAvatar > 1)
        {
            if (drawManager.numberFrames >= drawManager.secondNumberFrames)
            {
                if (drawManager.timeElapsed > 0 && !drawManager.isPaused)
                    textChrono.text = drawManager.frameNtime + " s";
            }
            else
            {
                if (drawManager.secondTimeElapsed > 0 && !drawManager.secondPaused)
                    textChrono.text = drawManager.secondFrameNtime + " s";
            }
        }
        else
        {
            if (drawManager.timeElapsed > 0 && !drawManager.isPaused)
            {
                textChrono.text = drawManager.frameNtime + " s";
            }
        }
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
        if (drawManager.isEditing) return;

        slider.maxValue = (float)drawManager.numberFrames - 1;
        var currentFrame = (int)slider.value;
        drawManager.setFrameN(currentFrame);
        textChrono.text = drawManager.frameNtime + " s";

        if (drawManager.cntAvatar > 1)
        {
            var secondCurrentFrame = currentFrame;
            if (secondCurrentFrame >= drawManager.secondNumberFrames)
                secondCurrentFrame = drawManager.secondNumberFrames - 1;
            drawManager.setSecondFrameN(secondCurrentFrame);
        }

        if ((int)slider.value == 0 || (int)slider.value == (int)slider.maxValue)
        {
            drawManager.setCanResumeAnimation(false);
            ShowPlayButton();
        }

        if (Input.GetMouseButton(0) && uiManager.IsOnGameObject(slider.gameObject))
        {
            ShowPlayButton();
            drawManager.Pause();

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