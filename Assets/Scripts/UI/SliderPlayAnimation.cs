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
		//ToolBox.GetInstance().GetManager<DrawManager>().PauseAvatar(isPaused);
	}

	void Update()
	{
        if (drawManager.numberFrames >= drawManager.secondNumberFrames) slider.value = drawManager.frameN;
        else slider.value = drawManager.secondFrameN;

        if(drawManager.cntAvatar > 1)
        {
            if (drawManager.numberFrames >= drawManager.secondNumberFrames)
            {
                if (drawManager.timeElapsed > 0 && !drawManager.isPaused)
                    textChrono.text = ToolBox.GetInstance().GetManager<DrawManager>().frameN * 0.02 + " s";
                //                    textChrono.text = string.Format("{0:0.0}", drawManager.timeElapsed) + " s";
            }
            else
            {
                if (drawManager.secondTimeElapsed > 0 && !drawManager.secondPaused)
                    textChrono.text = ToolBox.GetInstance().GetManager<DrawManager>().secondFrameN * 0.02 + " s";
                //                    textChrono.text = string.Format("{0:0.0}", drawManager.secondTimeElapsed) + " s";
            }
        }
        else
        {
            if (drawManager.timeElapsed > 0 && !drawManager.isPaused)
            {
                textChrono.text = ToolBox.GetInstance().GetManager<DrawManager>().frameN * 0.02 + " s";
                //                textChrono.text = string.Format("{0:0.0}", drawManager.timeElapsed) + " s";
            }
        }

        /*		if (slider.value > 100)
                {
        //			worldCanvas.SetActive(false);
                    result.SetActive(true);
                    result.GetComponent<Animator>().Play("Panel In");
                    ToolBox.GetInstance().GetManager<DrawManager>().PlayEnd();
                }*/
    }


	///===///  OnClick() functions
	#region		<-- TOP

	public void OnPlayAnimationSlider()
	{
        if (ToolBox.GetInstance().GetManager<DrawManager>().isEditing) return;

        slider.minValue = 0f;

        if (Input.GetMouseButton(0) && ToolBox.GetInstance().GetManager<UIManager>().IsOnGameObject(slider.gameObject))
        {
            pauseButton.isOn = false;

            if (!ToolBox.GetInstance().GetManager<DrawManager>().isPaused)
            {
                ToolBox.GetInstance().GetManager<DrawManager>().isPaused = true;

                if (drawManager.cntAvatar>1 && !drawManager.secondPaused)
                    ToolBox.GetInstance().GetManager<DrawManager>().secondPaused = true;
            }

            if (pauseBackground.activeSelf)
                pauseBackground.SetActive(false);
        }

        if (drawManager.cntAvatar > 1)
        {
            if (drawManager.numberFrames >= drawManager.secondNumberFrames)
            {
                //                slider.maxValue = (float)drawManager.numberFrames;
                //                drawManager.frameN = (int)slider.value;
                slider.maxValue = (float)drawManager.numberFrames - 1;
                drawManager.frameN = (int)slider.value;

                if (slider.value >= drawManager.secondNumberFrames) drawManager.secondFrameN = drawManager.secondNumberFrames;
                else drawManager.secondFrameN = (int)slider.value;

                textChrono.text = ToolBox.GetInstance().GetManager<DrawManager>().frameN * 0.02 + " s";
            }
            else
            {
                //                slider.maxValue = (float)drawManager.secondNumberFrames;
                //                drawManager.secondFrameN = (int)slider.value;
                slider.maxValue = (float)drawManager.secondNumberFrames - 1;
                drawManager.secondFrameN = (int)slider.value;

                if (slider.value >= drawManager.numberFrames) drawManager.frameN = drawManager.numberFrames;
                else drawManager.frameN = (int)slider.value;

                textChrono.text = ToolBox.GetInstance().GetManager<DrawManager>().secondFrameN * 0.02 + " s";
            }
        }
        else
        {
            slider.maxValue = (float)drawManager.numberFrames -1;
            drawManager.frameN = (int)slider.value;

            textChrono.text = ToolBox.GetInstance().GetManager<DrawManager>().frameN * 0.02 + " s";
        }
    }

    #endregion		<-- BOTTOM

}