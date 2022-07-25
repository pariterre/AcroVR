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
    //	public PlayController playController;
    //	public GameObject result;
    //	public GameObject worldCanvas;

    public Text textChrono;
    public Toggle pauseButton;
    public GameObject pauseBackground;

    void Start()
    {
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        //slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        //isPaused = !isPaused;
        //drawManager.PauseAvatar(isPaused);
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


    public void OnPlayAnimationSlider()
    {
        if (drawManager.isEditing) return;

        slider.minValue = 0f;

        if (Input.GetMouseButton(0) && ToolBox.GetInstance().GetManager<UIManager>().IsOnGameObject(slider.gameObject))
        {
            pauseButton.isOn = false;

            if (!drawManager.isPaused)
            {
                drawManager.isPaused = true;

                if (drawManager.cntAvatar>1 && !drawManager.secondPaused)
                    drawManager.secondPaused = true;
            }

            if (pauseBackground.activeSelf)
                pauseBackground.SetActive(false);
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
    }
}