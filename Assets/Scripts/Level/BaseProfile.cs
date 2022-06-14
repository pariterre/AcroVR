using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class BaseProfile : LevelBase
{
    public Dropdown dropDownCondition;
    public Dropdown dropDownDDLNames;
//    public GameObject anchorSidePOV = null;
//    public GameObject anchorThirdPOV = null;
    public Camera AvatarCamera;

    public Dropdown dropDownPlaySpeed;

    public GameObject ErrorObject;
    public GameObject TutorialObject;
    public GameObject NodeNameObject;

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

    public Toggle pauseButton;
    public GameObject pauseBackground;

    public Text fileName;

    public enum CameraView
    {
        FirstPOV,
        FrontPOV,
        SidePOV,
        ThreeQuarterPOV
    }

    public CameraView camView;

    public override void SetPrefab(GameObject _prefab)
    {
    }

    public override void CreateLevel()
    {
    }

    public void SwtichCameraView()
    {
        if (ToolBox.GetInstance().GetManager<DrawManager>().girl1 == null)
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

        switch (camView)
        {
            case CameraView.FirstPOV:
                FirstPOVCamera();
                break;
            case CameraView.FrontPOV:
                FrontCameraPOV(ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar());
                break;
            case CameraView.SidePOV:
                SideCameraPOV(ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar());
                break;
            case CameraView.ThreeQuarterPOV:
                ThreeQuarterCameraPOV(ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar());
                break;
        }
    }

    public void FrontCameraPOV(float _v)
    {
        if (cameraList == null) return;

        if(cameraList[15] == null)
        {
            cameraList[15] = ToolBox.GetInstance().GetManager<DrawManager>().GetFirstViewTransform();
        }

        for (int i = 0; i < cameraList.Length; i++)
        {
            cameraList[i].gameObject.SetActive(false);

            if(cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt == null)
            {
                if(ToolBox.GetInstance().GetManager<DrawManager>().girl1 != null)
                    cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt = ToolBox.GetInstance().GetManager<DrawManager>().girl1.transform.Find("Petra.002/hips").gameObject.transform;
            }
        }


        if(_v >= 0 && _v < 2)
        {
            cameraList[0].gameObject.SetActive(true);
        }
        else if(_v >= 2 && _v < 4)
        {
            cameraList[1].gameObject.SetActive(true);
        }
        else if(_v >= 4 && _v < 6)
        {
            cameraList[2].gameObject.SetActive(true);
        }
        else if (_v >= 6 && _v < 8)
        {
            cameraList[3].gameObject.SetActive(true);
        }
        else
        {
            cameraList[4].gameObject.SetActive(true);
        }

        camView = CameraView.FrontPOV;

        /*        if(h != 0)
                {
                    cameraList[(int)cam].gameObject.transform.position = new Vector3(0, 1, 3 + h);
                }

                cameraList[(int)cam].gameObject.SetActive(true);*/
    }

    public void SideCameraPOV(float _v)
    {
        for (int i = 0; i < cameraList.Length; i++)
        {
            cameraList[i].gameObject.SetActive(false);

            if (cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt == null)
            {
                if (ToolBox.GetInstance().GetManager<DrawManager>().girl1 != null)
                    cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt = ToolBox.GetInstance().GetManager<DrawManager>().girl1.transform.Find("Petra.002/hips").gameObject.transform;
            }
        }

        if (_v >= 0 && _v < 2)
        {
            cameraList[5].gameObject.SetActive(true);
        }
        else if (_v >= 2 && _v < 4)
        {
            cameraList[6].gameObject.SetActive(true);
        }
        else if (_v >= 4 && _v < 6)
        {
            cameraList[7].gameObject.SetActive(true);
        }
        else if (_v >= 6 && _v < 8)
        {
            cameraList[8].gameObject.SetActive(true);
        }
        else
        {
            cameraList[9].gameObject.SetActive(true);
        }

        camView = CameraView.SidePOV;

        /*        if(h != 0)
                {
                    cameraList[(int)cam].gameObject.transform.position = new Vector3(0, 1, 3 + h);
                }

                cameraList[(int)cam].gameObject.SetActive(true);*/
    }

    public void ThreeQuarterCameraPOV(float _v)
    {
        for (int i = 0; i < cameraList.Length; i++)
        {
            cameraList[i].gameObject.SetActive(false);

            if (cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt == null)
            {
                if (ToolBox.GetInstance().GetManager<DrawManager>().girl1 != null)
                    cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt = ToolBox.GetInstance().GetManager<DrawManager>().girl1.transform.Find("Petra.002/hips").gameObject.transform;
            }
        }

        if (_v >= 0 && _v < 2)
        {
            cameraList[10].gameObject.SetActive(true);
        }
        else if (_v >= 2 && _v < 4)
        {
            cameraList[11].gameObject.SetActive(true);
        }
        else if (_v >= 4 && _v < 6)
        {
            cameraList[12].gameObject.SetActive(true);
        }
        else if (_v >= 6 && _v < 8)
        {
            cameraList[13].gameObject.SetActive(true);
        }
        else
        {
            cameraList[14].gameObject.SetActive(true);
        }

        camView = CameraView.ThreeQuarterPOV;
        /*        if(h != 0)
                {
                    cameraList[(int)cam].gameObject.transform.position = new Vector3(0, 1, 3 + h);
                }

                cameraList[(int)cam].gameObject.SetActive(true);*/
    }

    public void FirstPOVCamera()
    {
        for (int i = 0; i < cameraList.Length; i++)
        {
            cameraList[i].gameObject.SetActive(false);

            if (cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt == null)
            {
                if (ToolBox.GetInstance().GetManager<DrawManager>().girl1 != null)
                    cameraList[i].GetComponent<CinemachineVirtualCamera>().LookAt = ToolBox.GetInstance().GetManager<DrawManager>().girl1.transform.Find("Petra.002/hips").gameObject.transform;
            }
        }

        cameraList[15].gameObject.SetActive(true);

        camView = CameraView.FirstPOV;
    }

    public void BackToMenu()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("Back Button Click: BackToMenu()");

        ToolBox.GetInstance().GetManager<GameManager>().InitAnimationInfo();
        ToolBox.GetInstance().GetManager<DrawManager>().isEditing = false;
        ToolBox.GetInstance().GetManager<DrawManager>().animateON = false;
        ToolBox.GetInstance().GetManager<AniGraphManager>().cntAvatar = 0;
        ToolBox.GetInstance().GetManager<DrawManager>().ResetPause();
        ToolBox.GetInstance().GetManager<DrawManager>().ResetFrame();
        ToolBox.GetInstance().GetManager<AniGraphManager>().isTutorial = 0;
        ToolBox.GetInstance().GetManager<GameManager>().numMission = 0;

        ToolBox.GetInstance().GetManager<LevelManager>().GotoScreen("MainMenu");
    }

    public void ToProfile()
    {
        ToolBox.GetInstance().GetManager<LevelManager>().GotoScreen("Profile");
    }

    public void ToTraining()
    {
        ToolBox.GetInstance().GetManager<LevelManager>().GotoScreen("Training");
    }

    public void ToNextLevel()
    {
        ToolBox.GetInstance().GetManager<LevelManager>().NextLevel();
    }

    public void ToQuit()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("End!");

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

//    public void ShowResultGraph()
//    {
//        ToolBox.GetInstance().GetManager<AniGraphManager>().ResultGraphOn();
//    }

    public void ToBaseLevel1()
    {
        ToolBox.GetInstance().GetManager<LevelManager>().GotoScreen("BaseLevel1");
    }

    private void ShowTakeOff()
    {
        somersaultPosition.text = MainParameters.Instance.joints.takeOffParam.rotation.ToString();
        tiltPosition.text = MainParameters.Instance.joints.takeOffParam.tilt.ToString();


        ///////////////////////////
        twistPosition.text = ToolBox.GetInstance().GetManager<DrawManager>().takeOffParamTwistPosition.ToString();
        horizontalPosition.text = ToolBox.GetInstance().GetManager<DrawManager>().takeOffParamHorizontalPosition.ToString();
        verticalPosition.text = ToolBox.GetInstance().GetManager<DrawManager>().takeOffParamVerticalPosition.ToString();
        tiltSpeed.text = ToolBox.GetInstance().GetManager<DrawManager>().takeOffParamTiltSpeed.ToString();
        ///////////////////////////


        somersaultSpeed.text = MainParameters.Instance.joints.takeOffParam.somersaultSpeed.ToString();
        twistSpeed.text = MainParameters.Instance.joints.takeOffParam.twistSpeed.ToString();
        horizontalSpeed.text = MainParameters.Instance.joints.takeOffParam.anteroposteriorSpeed.ToString();
        verticalSpeed.text = MainParameters.Instance.joints.takeOffParam.verticalSpeed.ToString();

        simulationDuration.text = MainParameters.Instance.joints.duration.ToString();
    }

    public void MissionLoad()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("Load Button Click");

        //        AvatarCamera.transform.position = anchorThirdPOV.transform.position;
        //        AvatarCamera.transform.rotation = anchorThirdPOV.transform.rotation;

        int ret = ToolBox.GetInstance().GetManager<GameManager>().MissionLoad();

        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("Load return value: " + ret.ToString());

        if (ret < 0)
        {
            if(ret == -1)
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

        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("Success to load a file");

        fileName.text = Path.GetFileName(MainParameters.Instance.joints.fileName);

        if (ToolBox.GetInstance().GetManager<DrawManager>().setAvatar == DrawManager.AvatarMode.SingleFemale)
            ToolBox.GetInstance().GetManager<DrawManager>().LoadAvatar(DrawManager.AvatarMode.SingleFemale);
        else
            ToolBox.GetInstance().GetManager<DrawManager>().LoadAvatar(DrawManager.AvatarMode.SingleMale);

        TakeOffOn();
        InitDropdownDDLNames(0);

        dropDownCondition.value = MainParameters.Instance.joints.condition;

        ToolBox.GetInstance().GetManager<DrawManager>().isEditing = false;
        ToolBox.GetInstance().GetManager<DrawManager>().MakeSimulationFrame();
        ToolBox.GetInstance().GetManager<DrawManager>().ShowAvatar();

        ToolBox.GetInstance().GetManager<DrawManager>().animateON = false;

        FrontCameraPOV(ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar());

        if (!pauseBackground.activeSelf) pauseBackground.SetActive(true);

        /*        if (ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar())
                {
                    CameraPOV(POV.LongFrontView);
        //            LongDistanceCamera();
                }
                else
                {
                    CameraPOV(POV.FrontView);
        //            AvatarCamera.transform.position = anchorThirdPOV.transform.position;
        //            AvatarCamera.transform.rotation = anchorThirdPOV.transform.rotation;
                }*/

        TutorialMessage();
        ToolBox.GetInstance().GetManager<AniGraphManager>().cntAvatar = 0;

        float t = (ToolBox.GetInstance().GetManager<DrawManager>().numberFrames-1) * 0.02f;
        endFrameText.text = t + " sec";

        if (t < MainParameters.Instance.joints.duration) MainParameters.Instance.joints.duration = t;

        ToolBox.GetInstance().GetManager<GameManager>().InterpolationDDL();
        ToolBox.GetInstance().GetManager<GameManager>().DisplayDDL(0, true);

        ShowTakeOff();
    }

    public void MissionLoad2()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("Load Two avatar Button Click");

        int ret = ToolBox.GetInstance().GetManager<GameManager>().LoadSimulationSecond();

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

        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("Success to load one");

        ShowTakeOff();

        if (ToolBox.GetInstance().GetManager<DrawManager>().setAvatar == DrawManager.AvatarMode.SingleFemale)
        {
            if (!ToolBox.GetInstance().GetManager<DrawManager>().LoadAvatar(DrawManager.AvatarMode.DoubleFemale))
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
            if (!ToolBox.GetInstance().GetManager<DrawManager>().LoadAvatar(DrawManager.AvatarMode.DoubleMale))
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

        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("Success to load two");

        TakeOffOn();
        InitDropdownDDLNames(0);

        ToolBox.GetInstance().GetManager<DrawManager>().isEditing = false;
        ToolBox.GetInstance().GetManager<DrawManager>().MakeSimulationFrame();
        ToolBox.GetInstance().GetManager<DrawManager>().ShowAvatar();

        ToolBox.GetInstance().GetManager<DrawManager>().animateON = false;

        FrontCameraPOV(ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar());

        if (!pauseBackground.activeSelf) pauseBackground.SetActive(true);

        /*        if (ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar())
                {
                    CameraPOV(POV.LongFrontView);
        //            LongDistanceCamera();
                }
                else
                {
                    CameraPOV(POV.FrontView);
        //            AvatarCamera.transform.position = anchorThirdPOV.transform.position;
        //            AvatarCamera.transform.rotation = anchorThirdPOV.transform.rotation;
                }*/

        TutorialMessage();

        ToolBox.GetInstance().GetManager<AniGraphManager>().cntAvatar = 1;

        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("Success to load one");
    }

    public void SaveFile()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("SaveFile()");

        ToolBox.GetInstance().GetManager<GameManager>().SaveFile();
    }

    public void TakeOffOn()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("GraphOn");

        ToolBox.GetInstance().GetManager<AniGraphManager>().GraphOn();
    }

    public void TakeOffOff()
    {
        ToolBox.GetInstance().GetManager<AniGraphManager>().TaskOffGraphOff();
    }

    public void InitDropdownDDLNames(int ddl)
    {
        if (ToolBox.GetInstance().GetManager<UIManager>().GetCurrentTab() != 2) return;

        if (ToolBox.GetInstance().GetManager<DrawManager>().girl1 == null)
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

        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("InitDropdownDDLNames() ddl: " + ddl.ToString());

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
            ToolBox.GetInstance().GetManager<AniGraphManager>().DisplayCurveAndNodes(0, ddl, axisRange);
            if (MainParameters.Instance.joints.nodes[ddl].ddlOppositeSide >= 0)
            {
                ToolBox.GetInstance().GetManager<AniGraphManager>().DisplayCurveAndNodes(1, MainParameters.Instance.joints.nodes[ddl].ddlOppositeSide, axisRange);
            }
        }
    }

    public void DropDownDDLNamesOnValueChanged(int value)
    {
        DisplayDDL(value, true);
    }

/*    public void FirstPOVCamera()
    {
        if (ToolBox.GetInstance().GetManager<DrawManager>().girl1 == null)
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

        AvatarCamera.transform.position = ToolBox.GetInstance().GetManager<DrawManager>().GetFirstViewTransform().transform.position;
        AvatarCamera.transform.rotation = ToolBox.GetInstance().GetManager<DrawManager>().GetFirstViewTransform().transform.rotation;
    }*/

    public void ThirdPOVCamera()
    {
        if (ToolBox.GetInstance().GetManager<DrawManager>().girl1 == null)
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

        FrontCameraPOV(ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar());

        /*        if (ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar())
                {
                    CameraPOV(POV.LongFrontView);
        //            LongDistanceCamera();
                }
                else
                {
                    CameraPOV(POV.FrontView);
        //            AvatarCamera.transform.position = anchorThirdPOV.transform.position;
        //            AvatarCamera.transform.rotation = anchorThirdPOV.transform.rotation;
                }*/
    }

    public void SidePOVCamera()
    {
        if (ToolBox.GetInstance().GetManager<DrawManager>().girl1 == null)
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


        SideCameraPOV(ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar());

/*        if(ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar())
        {
            ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("CheckPositionAvatar()");

            CameraPOV(POV.LongSideView);
//            AvatarCamera.transform.position = new Vector3(-15, 3, 7f);
//            AvatarCamera.transform.rotation = anchorSidePOV.transform.rotation;
        }
        else
        {
            CameraPOV(POV.SideView);

            //            AvatarCamera.transform.position = anchorSidePOV.transform.position;
            //            AvatarCamera.transform.rotation = anchorSidePOV.transform.rotation;
        }*/
    }

    public void ThreeQuarterPOVCamera()
    {
        if (ToolBox.GetInstance().GetManager<DrawManager>().girl1 == null)
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

        ThreeQuarterCameraPOV(ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar());
    }

    public void LongDistanceCamera()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("LongDistanceCamera()");

//        AvatarCamera.transform.position = new Vector3(0, 3, 17f);
//        AvatarCamera.transform.rotation = anchorThirdPOV.transform.rotation;
    }

    public void PlayAvatar()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("PlayAvatar()");

        if (ToolBox.GetInstance().GetManager<DrawManager>().girl1 == null)
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

        ToolBox.GetInstance().GetManager<DrawManager>().MakeSimulationFrame();
        ToolBox.GetInstance().GetManager<DrawManager>().ShowAvatar();
    }

    public void PlayAvatarButton()
    {
        pauseButton.isOn = true;
        if (!pauseBackground.activeSelf) pauseBackground.SetActive(true);

        ToolBox.GetInstance().GetManager<DrawManager>().ResetPause();

        if (ToolBox.GetInstance().GetManager<DrawManager>().girl1 == null)
        {
            return;
        }

        if (!ToolBox.GetInstance().GetManager<DrawManager>().girl1.activeSelf)
        {
            return;
        }

        string playSpeed = dropDownPlaySpeed.captionText.text;
        if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedSlow3)
            ToolBox.GetInstance().GetManager<DrawManager>().SetAnimationSpeed(10);
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedSlow2)
            ToolBox.GetInstance().GetManager<DrawManager>().SetAnimationSpeed(3);
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedSlow1)
            ToolBox.GetInstance().GetManager<DrawManager>().SetAnimationSpeed(1.5f);
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedNormal)
            ToolBox.GetInstance().GetManager<DrawManager>().SetAnimationSpeed(1);
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedFast)
            ToolBox.GetInstance().GetManager<DrawManager>().SetAnimationSpeed(0.8f);

        ToolBox.GetInstance().GetManager<DrawManager>().isEditing = false;
        ToolBox.GetInstance().GetManager<DrawManager>().MakeSimulationFrame();
        ToolBox.GetInstance().GetManager<DrawManager>().ShowAvatar();
        ToolBox.GetInstance().GetManager<DrawManager>().PlayAvatar();

        SwtichCameraView();

        TakeOffOn();
        InitDropdownDDLNames(0);
        ToolBox.GetInstance().GetManager<GameManager>().InterpolationDDL();
        ToolBox.GetInstance().GetManager<GameManager>().DisplayDDL(0, false);
    }

    public void PauseAvatarButton()
    {
//        isPaused = !isPaused;
        ToolBox.GetInstance().GetManager<DrawManager>().PauseAvatar();

        if (ToolBox.GetInstance().GetManager<DrawManager>().isPaused)
            pauseBackground.SetActive(false);
        else
            pauseBackground.SetActive(true);
    }

    public void SetTab(int _num)
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("SetTab() _num: " + _num.ToString());

        if (ToolBox.GetInstance().GetManager<DrawManager>().girl1 == null)
        {
            if (MainParameters.Instance.languages.Used.toolTipButtonQuit == "Quit")
            {
                ErrorMessage("Please load files first");
            }
            else
            {
                ErrorMessage("SVP charger d'abord les fichiers");
            }
//            return;
        }

        ToolBox.GetInstance().GetManager<UIManager>().SetCurrentTab(_num);
    }

    public void SetFrench()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("SetFrench()");

        MainParameters.Instance.languages.Used = MainParameters.Instance.languages.french;
    }

    public void SetEnglish()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("SetEnglish()");

        MainParameters.Instance.languages.Used = MainParameters.Instance.languages.english;
    }

    public void SetTooltip(bool _flag)
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("SetTooltip() _flag: " + _flag.ToString());

        ToolBox.GetInstance().GetManager<UIManager>().SetTooltip(_flag);
    }

    public void SetMaleAvatar()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("SetMaleAvatar()");

        ToolBox.GetInstance().GetManager<DrawManager>().setAvatar = DrawManager.AvatarMode.SingleMale;
    }

    public void SetFemaleAvatar()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("SetFemaleAvatar()");

        ToolBox.GetInstance().GetManager<DrawManager>().setAvatar = DrawManager.AvatarMode.SingleFemale;
    }

    public void SetSimulationMode()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("SetSimulationMode()");

        ToolBox.GetInstance().GetManager<DrawManager>().SimulationMode();
    }

    public void SetGestureMode()
    {
        ToolBox.GetInstance().GetManager<GameManager>().WriteToLogFile("SetGestureMode()");

        ToolBox.GetInstance().GetManager<DrawManager>().GestureMode();
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
        if (ToolBox.GetInstance().GetManager<DrawManager>().girl1 == null) return;

        if(ToolBox.GetInstance().GetManager<AniGraphManager>().isTutorial == 0)
        {
            TakeOffOn();
            InitDropdownDDLNames(0);
            ToolBox.GetInstance().GetManager<GameManager>().InterpolationDDL();
            ToolBox.GetInstance().GetManager<GameManager>().DisplayDDL(0, true);

            ToolBox.GetInstance().GetManager<AniGraphManager>().isTutorial++;
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

    private void Update()
    {
        if(ToolBox.GetInstance().GetManager<AniGraphManager>().isTutorial == 1)
        {
            if(Input.anyKeyDown)
            {
                ToolBox.GetInstance().GetManager<AniGraphManager>().isTutorial++;
                TutorialObject.GetComponent<Animator>().Play("Panel Out");
            }
        }
    }
}