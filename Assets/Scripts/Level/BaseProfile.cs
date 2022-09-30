using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class BaseProfile : LevelBase
{
    protected LevelManager levelManager;
    protected GameManager gameManager;
    protected DrawManager drawManager;
    protected AniGraphManager aniGraphManager;
    protected UIManager uiManager;


    public Dropdown dropDownCondition;
    public Dropdown dropDownDDLNames;
    public Camera AvatarCamera;

    public Dropdown dropDownPlaySpeed;

    public GameObject ErrorObject;
    public GameObject TutorialObject;
    public GameObject NodeNameObject;
    protected GameObject CurrentTabContent;

    public Text endFrameText;

    public GameObject[] cameraList;

    public InputField somersaultPosition;
    public InputField tiltPosition;
    public InputField twistPosition;
    public InputField horizontalPosition;
    public InputField verticalPosition;
    public InputField somersaultSpeed;
    public InputField tiltSpeed;
    public InputField twistSpeed;
    public InputField horizontalSpeed;
    public InputField verticalSpeed;

    public InputField simulationDuration;
    public SliderPlayAnimation sliderPlay;

    public GameObject Floor;

    public Text fileName;

    public enum CameraView
    {
        FirstPOV,
        FrontPOV,
    }

    public CameraView camView;

    public override void SetPrefab(GameObject _prefab)
    {
    }

    public override void CreateLevel()
    {
    }

    public void ToggleCameraFirstOrThird(){
        if (camView != CameraView.FirstPOV){
            FirstPOVCamera();
        } else {
            FrontCameraPOV(drawManager.CheckPositionAvatar());
        }

    }

    public void SwitchCameraView()
    {
        if (drawManager.girl1 == null)
        {
            if (MainParameters.Instance.languages.Used.toolTipButtonQuit == "Quit")
            {
                ErrorMessage("Please load files first");
            }
            else
            {
                ErrorMessage("SVP charger d'abord les fichiers");
            }
            return;
        }

        if (camView == CameraView.FirstPOV)
            FirstPOVCamera();
        else
            FrontCameraPOV(drawManager.CheckPositionAvatar());
    }

    public void FrontCameraPOV(float _v)
    {
        if (cameraList == null) return;

        if(cameraList[15] == null)
            cameraList[15] = drawManager.GetFirstViewTransform();

        for (int i = 0; i < cameraList.Length; i++)
        {
            cameraList[i].gameObject.SetActive(false);

            if(cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt == null)
            {
                if(drawManager.girl1 != null)
                    cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt = drawManager.girl1.transform.Find("Petra.002/hips").gameObject.transform;
            }
        }


        if(_v >= 0 && _v < 2)
            cameraList[0].gameObject.SetActive(true);
        else if(_v >= 2 && _v < 4)
            cameraList[1].gameObject.SetActive(true);
        else if(_v >= 4 && _v < 6)
            cameraList[2].gameObject.SetActive(true);
        else if (_v >= 6 && _v < 8)
            cameraList[3].gameObject.SetActive(true);
        else
            cameraList[4].gameObject.SetActive(true);

        camView = CameraView.FrontPOV;
    }

    public void FirstPOVCamera()
    {
        for (int i = 0; i < cameraList.Length; i++)
        {
            cameraList[i].gameObject.SetActive(false);

            if (cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt == null)
            {
                if (drawManager.girl1 != null)
                    cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt = drawManager.girl1.transform.Find("Petra.002/hips").gameObject.transform;
            }
        }

        cameraList[15].gameObject.SetActive(true);

        camView = CameraView.FirstPOV;
    }

    public void BackToMenu()
    {
        gameManager.WriteToLogFile("Back Button Click: BackToMenu()");

        gameManager.InitAnimationInfo();
        drawManager.StopEditing();
        aniGraphManager.cntAvatar = 0;
        drawManager.Pause();
        drawManager.ResetFrame();
        aniGraphManager.isTutorial = 0;
        gameManager.numMission = 0;

        levelManager.GotoScreen("MainMenu");
    }

    public void ToProfile()
    {
        levelManager.GotoScreen("Profile");
    }

    public void ToTraining()
    {
        levelManager.GotoScreen("Training");
    }

    public void ToNextLevel()
    {
        levelManager.NextLevel();
    }

    public void ToQuit()
    {
        gameManager.WriteToLogFile("End!");

        if (Application.isEditor)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        else
        {
            Application.Quit();
        }
    }

    public void ToBaseLevel1()
    {
        levelManager.GotoScreen("BaseLevel1");
    }

    private void ShowTakeOff()
    {
        somersaultPosition.text = MainParameters.Instance.joints.takeOffParam.rotation.ToString();
        tiltPosition.text = MainParameters.Instance.joints.takeOffParam.tilt.ToString();


        ///////////////////////////
        twistPosition.text = drawManager.takeOffParamTwistPosition.ToString();
        horizontalPosition.text = drawManager.takeOffParamHorizontalPosition.ToString();
        verticalPosition.text = drawManager.takeOffParamVerticalPosition.ToString();
        tiltSpeed.text = drawManager.takeOffParamTiltSpeed.ToString();
        ///////////////////////////


        somersaultSpeed.text = MainParameters.Instance.joints.takeOffParam.somersaultSpeed.ToString();
        twistSpeed.text = MainParameters.Instance.joints.takeOffParam.twistSpeed.ToString();
        horizontalSpeed.text = MainParameters.Instance.joints.takeOffParam.anteroposteriorSpeed.ToString();
        verticalSpeed.text = MainParameters.Instance.joints.takeOffParam.verticalSpeed.ToString();

        simulationDuration.text = MainParameters.Instance.joints.duration.ToString();
    }

    public void MissionLoad()
    {
        gameManager.WriteToLogFile("Load Button Click");
        int ret = gameManager.MissionLoad();

        gameManager.WriteToLogFile("Load return value: " + ret.ToString());

        if (ret < 0)
        {
            if (ret == -1)
            {
                if (MainParameters.Instance.languages.Used.toolTipButtonQuit == "Quit")
                {
                    ErrorMessage("Please load files first");
                }
                else
                {
                    ErrorMessage("SVP charger d'abord les fichiers");
                }
            }
            else
            {
                if (MainParameters.Instance.languages.Used.toolTipButtonQuit == "Quit")
                {
                    ErrorMessage("Loaded incorrect Simulation files:  " + ret.ToString());
                }
                else
                {
                    ErrorMessage("Fichiers de simulation incorrects chargés:  " + ret.ToString());
                }
            }
            return;
        }

        gameManager.WriteToLogFile("Success to load a file");

        fileName.text = Path.GetFileName(MainParameters.Instance.joints.fileName);

        if (drawManager.setAvatar == DrawManager.AvatarMode.SingleFemale)
            drawManager.LoadAvatar(DrawManager.AvatarMode.SingleFemale);
        else
            drawManager.LoadAvatar(DrawManager.AvatarMode.SingleMale);

        TakeOffOn();
        InitDropdownDDLNames(0);

        dropDownCondition.value = MainParameters.Instance.joints.condition;

        drawManager.StopEditing();

        FrontCameraPOV(drawManager.CheckPositionAvatar());

        TutorialMessage();
        aniGraphManager.cntAvatar = 0;

        float t = (drawManager.numberFrames - 1) * 0.02f;
        endFrameText.text = t + " sec";

        if (t < MainParameters.Instance.joints.duration) MainParameters.Instance.joints.duration = t;

        gameManager.InterpolationDDL();
        gameManager.DisplayDDL(0, true);

        ShowTakeOff();
    }

    public void MissionLoad2()
    {
        gameManager.WriteToLogFile("Load Two avatar Button Click");

        int ret = gameManager.LoadSimulationSecond();

        if (ret < 0)
        {
            if (ret == -1)
            {
                if (MainParameters.Instance.languages.Used.toolTipButtonQuit == "Quit")
                {
                    ErrorMessage("Please load files first");
                }
                else
                {
                    ErrorMessage("SVP charger d'abord les fichiers");
                }
            }
            else
            {
                if (MainParameters.Instance.languages.Used.toolTipButtonQuit == "Quit")
                {
                    ErrorMessage("Loaded incorrect Simulation files:  " + ret.ToString());
                }
                else
                {
                    ErrorMessage("Fichiers de simulation incorrects chargés:  " + ret.ToString());
                }
            }
            return;
        }

        gameManager.WriteToLogFile("Success to load one");

        ShowTakeOff();

        if (drawManager.setAvatar == DrawManager.AvatarMode.SingleFemale)
        {
            if (!drawManager.LoadAvatar(DrawManager.AvatarMode.DoubleFemale))
            {
                if (MainParameters.Instance.languages.Used.toolTipButtonQuit == "Quit")
                {
                    ErrorMessage("Failed to load files for the second avatar");
                }
                else
                {
                    ErrorMessage("Impossible de charger les fichiers pour le deuxième avatar");
                }

                return;
            }
        }
        else
        {
            if (!drawManager.LoadAvatar(DrawManager.AvatarMode.DoubleMale))
            {
                if (MainParameters.Instance.languages.Used.toolTipButtonQuit == "Quit")
                {
                    ErrorMessage("Failed to load files for the second avatar");
                }
                else
                {
                    ErrorMessage("Impossible de charger les fichiers pour le deuxième avatar");
                }

                return;
            }
        }

        gameManager.WriteToLogFile("Success to load two");

        TakeOffOn();
        InitDropdownDDLNames(0);

        drawManager.StopEditing();

        TutorialMessage();

        aniGraphManager.cntAvatar = 1;

        gameManager.WriteToLogFile("Success to load one");
    }

    public void SaveFile()
    {
        gameManager.WriteToLogFile("SaveFile()");

        gameManager.SaveFile();
    }

    public void TakeOffOn()
    {
        gameManager.WriteToLogFile("GraphOn");

        aniGraphManager.GraphOn();
    }

    public void TakeOffOff()
    {
        aniGraphManager.TaskOffGraphOff();
    }

    public void InitDropdownDDLNames(int ddl)
    {
        if (uiManager.GetCurrentTab() != 2) return;

        if (drawManager.girl1 == null)
        {
            if (MainParameters.Instance.languages.Used.toolTipButtonQuit == "Quit")
            {
                ErrorMessage("Please load files first");
            }
            else
            {
                ErrorMessage("SVP charger d'abord les fichiers");
            }
            return;
        }

        gameManager.WriteToLogFile("InitDropdownDDLNames() ddl: " + ddl.ToString());

        List<string> dropDownOptions = new List<string>();
        for (int i = 0; i < 6; i++)
        {
            if (i == 0) dropDownOptions.Add("Hanche_Flexion");
            else if (i == 1) dropDownOptions.Add("Genou_Flexion");
            else if (i == 2) dropDownOptions.Add("Bras_Droit_Flexion");
            else if (i == 3) dropDownOptions.Add("Bras_Droit_Abduction");
            else if (i == 4) dropDownOptions.Add("Bras_Gauche_Flexion");
            else if (i == 5) dropDownOptions.Add("Bras_Gauche_Abduction");
        }
        dropDownDDLNames.ClearOptions();
        dropDownDDLNames.AddOptions(dropDownOptions);
        if (ddl >= 0)
        {
            dropDownDDLNames.value = ddl;
        }
    }

    public void DisplayDDL(int ddl, bool axisRange)
    {
        if (MainParameters.Instance.joints.nodes == null) return;

        if (ddl >= 0)
        {
            aniGraphManager.DisplayCurveAndNodes(0, ddl, axisRange);
            if (MainParameters.Instance.joints.nodes[ddl].ddlOppositeSide >= 0)
            {
                aniGraphManager.DisplayCurveAndNodes(1, MainParameters.Instance.joints.nodes[ddl].ddlOppositeSide, axisRange);
            }
        }
    }

    public void DropDownDDLNamesOnValueChanged(int value)
    {
        DisplayDDL(value, true);
    }


    public void ThirdPOVCamera()
    {
        if (drawManager.girl1 == null)
        {
            if (MainParameters.Instance.languages.Used.toolTipButtonQuit == "Quit")
            {
                ErrorMessage("Please load files first");
            }
            else
            {
                ErrorMessage("SVP charger d'abord les fichiers");
            }
            return;
        }

        FrontCameraPOV(drawManager.CheckPositionAvatar());
    }

    public void PlayAvatar()
    {
        gameManager.WriteToLogFile("PlayAvatar()");

        if (drawManager.girl1 == null)
        {
            if (MainParameters.Instance.languages.Used.toolTipButtonQuit == "Quit")
            {
                ErrorMessage("Please load files first");
            }
            else
            {
                ErrorMessage("SVP charger d'abord les fichiers");
            }
            return;
        }

        drawManager.ShowAvatar();
    }

    public void PlayAvatarButton()
    {
        if (drawManager.canResumeAnimation)
        {
            PauseAvatarButton();
            return;
        }

        drawManager.Resume();

        if (drawManager.girl1 == null || !drawManager.girl1.activeSelf)
        {
            return;
        }

        string playSpeed = dropDownPlaySpeed.captionText.text;
        if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedSlow3)
            drawManager.SetAnimationSpeed(10);
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedSlow2)
            drawManager.SetAnimationSpeed(3);
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedSlow1)
            drawManager.SetAnimationSpeed(1.5f);
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedNormal)
            drawManager.SetAnimationSpeed(1);
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedFast)
            drawManager.SetAnimationSpeed(0.8f);

        drawManager.StopEditing();
        drawManager.PlayAvatar();

        SwitchCameraView();

        TakeOffOn();

        sliderPlay.ShowPauseButton();
    }

    public void PauseAvatarButton()
    {
        if (!drawManager.PauseAvatar()) 
            return;
        if (drawManager.isPaused)
        {
            sliderPlay.ShowPlayButton();
        } else
        {
            sliderPlay.ShowPauseButton();
        }
    }

    public void SetTab(TabProperties _properties)
    {
        uiManager.SetCurrentTab(_properties.TabIndex);
        
        if (CurrentTabContent != null)
            CurrentTabContent.SetActive(false);
        CurrentTabContent = _properties.Content;
        CurrentTabContent.SetActive(true);
        
        _properties.BackgroundImage.sprite = _properties.BackgroundSprite;
        if (_properties.IsAGestureMode)
            SetGestureMode();
        else
            SetSimulationMode();

        drawManager.ForceFullUpdate();
        SwitchCameraView();

        if (_properties.HasTutorial)
            TutorialMessage();

    }

    public void SetFrench()
    {
        gameManager.WriteToLogFile("SetFrench()");

        MainParameters.Instance.languages.Used = MainParameters.Instance.languages.french;
    }

    public void SetEnglish()
    {
        gameManager.WriteToLogFile("SetEnglish()");

        MainParameters.Instance.languages.Used = MainParameters.Instance.languages.english;
    }

    public void SetTooltip(bool _flag)
    {
        gameManager.WriteToLogFile("SetTooltip() _flag: " + _flag.ToString());

        uiManager.SetTooltip(_flag);
    }

    public void SetMaleAvatar()
    {
        gameManager.WriteToLogFile("SetMaleAvatar()");

        drawManager.setAvatar = DrawManager.AvatarMode.SingleMale;
    }

    public void SetFemaleAvatar()
    {
        gameManager.WriteToLogFile("SetFemaleAvatar()");

        drawManager.setAvatar = DrawManager.AvatarMode.SingleFemale;
    }

    public void SetSimulationMode()
    {
        gameManager.WriteToLogFile("ActivateSimulationMode()");

        drawManager.ActivateSimulationMode();
    }

    public void SetGestureMode()
    {
        gameManager.WriteToLogFile("ActivateGestureMode()");

        drawManager.ActivateGestureMode();
    }

    public void ErrorMessage(string _msg)
    {
        ErrorObject.GetComponent<Animator>().Play("Panel In");
        ErrorObject.GetComponentInChildren<Text>().text = _msg;
        Invoke("CloseMsg", 0.5f);
    }

    private void CloseMsg()
    {
        ErrorObject.GetComponent<Animator>().Play("Panel Out");
    }

    public void NodeName(string _msg)
    {
        NodeNameObject.GetComponent<Animator>().Play("Panel In");
        NodeNameObject.GetComponentInChildren<Text>().text = _msg;
        Invoke("CloseNodeName", 0.5f);
    }

    private void CloseNodeName()
    {
        NodeNameObject.GetComponent<Animator>().Play("Panel Out");
    }

    public void TutorialMessage()
    {
        if (drawManager.girl1 == null) return;

        if(aniGraphManager.isTutorial == 0)
        {
            TakeOffOn();
            InitDropdownDDLNames(0);
            gameManager.InterpolationDDL();
            gameManager.DisplayDDL(0, true);

            aniGraphManager.isTutorial++;
            TutorialObject.GetComponent<Animator>().Play("Panel In");

            if (MainParameters.Instance.languages.Used.toolTipButtonQuit == "Quit")
            {
                TutorialObject.GetComponentInChildren<Text>().text = "1. Mouse Right Button: On/Off Rotation Modifier\n" +
                    "2. Each shoulder can be clicked twice(x and y)\n" +
                    "3. Shift + Mouse Left Button: Rotate 3D avatar\n" +
                    "4. Mouse Drag: Change Node value";
            }
            else
            {
                TutorialObject.GetComponentInChildren<Text>().text = "1. Bouton droit de la souris: modificateur de rotation On/Off\n" +
                    "2. Chaque épaule peut être cliquée deux fois (x et y)\n" +
                    "3. Shift + Bouton gauche de la souris: faire pivoter l'avatar 3D\n" +
                    "4. Glisser la souris: modifier la valeur du noeud";
            }
        }
    }

    void Start()
    {
        levelManager = ToolBox.GetInstance().GetManager<LevelManager>();
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        aniGraphManager = ToolBox.GetInstance().GetManager<AniGraphManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();

        // Give some handler to relevant scripts
        drawManager.SetGround(Floor);
    }

    private void Update()
    {
        if(aniGraphManager.isTutorial == 1)
        {
            if(Input.anyKeyDown)
            {
                aniGraphManager.isTutorial++;
                TutorialObject.GetComponent<Animator>().Play("Panel Out");
            }
        }
    }
}