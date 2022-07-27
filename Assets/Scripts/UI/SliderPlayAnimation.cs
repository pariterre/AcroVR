using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

/// <summary>
///	 
/// </summary>

public class SliderPlayAnimation : MonoBehaviour
{
    // Variables
    DrawManager drawManager;
    public Slider slider;

    public Text textChrono;
    public Button playButton;
    public Button pauseButton;

    void Start()
    {
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
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


    public void showPlayButton()
    {
        playButton.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

    public void showPauseButton()
    {
        playButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }

    public void OnPlayAnimationSlider()
    {
        if (drawManager.isEditing) return;

        slider.minValue = 0f;

        if (Input.GetMouseButton(0) && ToolBox.GetInstance().GetManager<UIManager>().IsOnGameObject(slider.gameObject))
        {
            showPlayButton();

            if (!drawManager.isPaused)
            {
                drawManager.isPaused = true;

                if (drawManager.cntAvatar>1 && !drawManager.secondPaused)
                    drawManager.secondPaused = true;
            }
        }

        if (drawManager.cntAvatar > 1)
        {
            if (drawManager.numberFrames >= drawManager.secondNumberFrames)
            {
                slider.maxValue = (float)drawManager.numberFrames - 1;
                drawManager.setFrameN((int)slider.value);

                if (slider.value >= drawManager.secondNumberFrames)
                    drawManager.setSecondFrameN(drawManager.secondNumberFrames);
                else 
                    drawManager.setSecondFrameN((int)slider.value);

                textChrono.text = drawManager.frameNtime + " s";
            }
            else
            {
                slider.maxValue = (float)drawManager.secondNumberFrames - 1;
                drawManager.setSecondFrameN((int)slider.value);

                if (slider.value >= drawManager.numberFrames)
                    drawManager.setFrameN(drawManager.numberFrames);
                else 
                    drawManager.setFrameN((int)slider.value);

                textChrono.text = drawManager.secondFrameNtime + " s";
            }
        }
        else
        {
            slider.maxValue = (float)drawManager.numberFrames -1;
            drawManager.setFrameN((int)slider.value);

            textChrono.text = drawManager.frameNtime + " s";
        }

        if ((int)slider.value == (int)slider.maxValue)
        {
            showPlayButton();
        }
    }
}