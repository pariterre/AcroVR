using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingTooltip : MonoBehaviour
{
    public struct StrucLanguages
    {
        public StrucMessageLists french;
        public StrucMessageLists english;
    }

    public struct StrucMessageLists
    {
        public string Tab1Title;
        public string Tab2Title;
        public string Tab3Title;
        public string Tab4Title;
        public string LoadSimulationFiles;
        public string loadButton;
        public string saveButton;
        public string compareButton;
        public string Back;
        public string PerspectiveView;
        public string PointOfView;
        public string AnimationSpeed;
        public string StartButton;
        public string PauseButton;
        public string ReplayButton;
        public string Condition;
        public string SimulationTime;
        public string Duration;

        public string TakeOffSomersault;
        public string TakeOffTilt;
        public string TakeOffTwist;
        public string TakeOffHorizontal;
        public string TakeOffVertical;
    }

    public StrucLanguages languages;

    public Text Tab1Title;
    public Text Tab2Title;
    public Text Tab3Title;
    public Text Tab4Title;

    public Text LoadSimulationFiles;

    public Text loadButton;
    public Text saveButton;
    public Text compareButton;

    public Text textBack;
    public Text PerspectiveView;
    public Text PointOfView;
    public Text AnimationSpeed;
    public Text SimulationTime;
    public Text Duration;

    public Text textTakeOffTitle;
    public Text textTakeOffSomersault;
    public Text textTakeOffTilt;
    public Text textTakeOffTwist;
    public Text textTakeOffHorizontal;
    public Text textTakeOffVertical;

    public Text textTakeOffSetupTitle;
//    public Text textTakeOffInitialPosture;

    public Text StartButton;
    public Text PauseButton;
    public Text ReplayButton;

    public Dropdown dropDownTakeOffCondition;
    public Dropdown dropDownAnimationSpeed;
    public Toggle checkGravity;

    public Text ConditionName;

    public BaseProfile bp;

    private double mousePosX;
    private double mousePosY;
    public GameObject panelAddRemoveNode;

    private void Start()
    {
        if (ToolBox.GetInstance().GetManager<DrawManager>().setAvatar == DrawManager.AvatarMode.SingleFemale)
            ToolBox.GetInstance().GetManager<DrawManager>().InitAvatar(DrawManager.AvatarMode.SingleFemale);
        else
            ToolBox.GetInstance().GetManager<DrawManager>().InitAvatar(DrawManager.AvatarMode.SingleMale);

        ToolBox.GetInstance().GetManager<GameManager>().InitAnimationInfo();

        languages.french.Tab1Title = "1. Décollage";
        languages.english.Tab1Title = "1. Take Off";
        languages.french.Tab2Title = "2. Mouvement";
        languages.english.Tab2Title = "2. Movement";
        languages.french.Tab3Title = "3. Simulation";
        languages.english.Tab3Title = "3. Simulation";
        languages.french.Tab4Title = "4. Résultat";
        languages.english.Tab4Title = "4. Result";

        languages.french.LoadSimulationFiles = "Charger des fichiers de simulation";
        languages.english.LoadSimulationFiles = "Load Simulation files";
        languages.french.loadButton = "Charger";
        languages.english.loadButton = "Load";
        languages.french.saveButton = "Enregistrer";
        languages.english.saveButton = "Save";
        languages.french.compareButton = "Comparer";
        languages.english.compareButton = "Compare";

        languages.french.Back = "Retour au menu";
        languages.english.Back = "Back to menu";
        languages.french.PerspectiveView = "Vue de perspective";
        languages.english.PerspectiveView = "Perspective View";
        languages.french.PointOfView = "Type de visualisation";
        languages.english.PointOfView = "Point of View";
        languages.french.AnimationSpeed = "La vitesse d'animation";
        languages.english.AnimationSpeed = "Animation Speed";

        languages.english.StartButton = "Start";
        languages.french.StartButton = "Démarrer";
        languages.english.PauseButton = "Pause";
        languages.french.PauseButton = "Pause";
        languages.english.ReplayButton = "Replay";
        languages.french.ReplayButton = "Rejouer";

        languages.english.Condition = "Condition of Take-off";
        languages.french.Condition = "Condition de décollage";

        languages.english.SimulationTime = "Simulation Time";
        languages.french.SimulationTime = "Temps de Simulation";
        languages.english.Duration = "Duration:";
        languages.french.Duration = "Durée:";

        languages.english.TakeOffSomersault = "Somersault";
        languages.french.TakeOffSomersault = "Salto";
        languages.english.TakeOffTilt = "Tilt";
        languages.french.TakeOffTilt = "Inclinaison";
        languages.english.TakeOffTwist = "Twist";
        languages.french.TakeOffTwist = "Vrille";
        languages.english.TakeOffHorizontal= "Horizontal";
        languages.french.TakeOffHorizontal = "Horizontale";
        languages.english.TakeOffVertical = "Vertical";
        languages.french.TakeOffVertical = "Verticale";

        ChangedLanguage();
        ToolBox.GetInstance().GetManager<UIManager>().SetTooltip();

        bp.FrontCameraPOV(0);

        UpdateDropDown();
    }

    public void AddCondition(Text name)
    {
        if (name.text != "")
        {
            ToolBox.GetInstance().GetManager<GameManager>().SaveCondition(dropDownTakeOffCondition.value, name.text);
            UpdateDropDown();
        }
    }

    public void DeleteCondition()
    {
        ToolBox.GetInstance().GetManager<GameManager>().RemoveCondition(dropDownTakeOffCondition.value);
        UpdateDropDown();
    }

    public void NameCondition()
    {
        ConditionName.text = ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[dropDownTakeOffCondition.value].name;
    }

    public void UpdateDropDown()
    {
        dropDownTakeOffCondition.options.Clear();

        for (int i = 0; i < ToolBox.GetInstance().GetManager<GameManager>().listCondition.count; i++)
        {
            dropDownTakeOffCondition.options.Add(new Dropdown.OptionData()
            {
                text = ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[i].name
            });
        }

        dropDownTakeOffCondition.value = 1;
        dropDownTakeOffCondition.value = 0;

        UpdateGravity();
        UpdatePositions();
    }

    public void UpdateGravity()
    {
        checkGravity.isOn = ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[dropDownTakeOffCondition.value].Gravity;
        ChangedGravity();
    }

    public void ChangedGravity()
    {
        if (checkGravity.isOn)
        {
            MainParameters.Instance.joints.condition = dropDownTakeOffCondition.value;
            if (MainParameters.Instance.joints.condition == 0) MainParameters.Instance.joints.condition = 1;
            ToolBox.GetInstance().GetManager<DrawManager>().takeOffParamGravity = true;
        }
        else
        {
            MainParameters.Instance.joints.condition = 0;
            ToolBox.GetInstance().GetManager<DrawManager>().takeOffParamGravity = false;
        }
    }

    public void UpdatePositions()
    {
        bp.somersaultPosition.text = ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[dropDownTakeOffCondition.value].SomersaultPosition.ToString();
        bp.twistPosition.text = ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[dropDownTakeOffCondition.value].TwistPosition.ToString();
        bp.tiltPosition.text = ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[dropDownTakeOffCondition.value].TiltPosition.ToString();
        bp.horizontalPosition.text = ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[dropDownTakeOffCondition.value].HorizontalPosition.ToString();
        bp.verticalPosition.text = ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[dropDownTakeOffCondition.value].VerticalPosition.ToString();
    }

    private void ChangedLanguage()
    {
        MainParameters.StrucMessageLists languagesUsed = MainParameters.Instance.languages.Used;

        /*        if (MainParameters.Instance.joints.nodes != null)
                {
                    MovementF.Instance.DisplayDDL(-1, false);
                    MovementF.Instance.InitDropdownDDLNames(-1);
                    MovementF.Instance.InitDropdownInterpolation(-1);
                    if (MainParameters.Instance.joints.nodes[GraphManager.Instance.ddlUsed].ddlOppositeSide >= 0)
                    {
                        MovementF.Instance.textCurveName1.text = languagesUsed.leftSide;
                        MovementF.Instance.textCurveName2.text = languagesUsed.rightSide;
                    }
                }*/

        /*        GraphSettings.Instance.textVerticalAxisTitle.text = languagesUsed.movementGraphSettingsVerticalTitle;
                GraphSettings.Instance.textVerticalAxisLowerBound.text = languagesUsed.movementGraphSettingsLowerBound;
                GraphSettings.Instance.textVerticalAxisUpperBound.text = languagesUsed.movementGraphSettingsUpperBound;
                GraphSettings.Instance.textHorizontalAxisTitle.text = languagesUsed.movementGraphSettingsHorizontalTitle;
                GraphSettings.Instance.textHorizontalAxisLowerBound.text = languagesUsed.movementGraphSettingsLowerBound;
                GraphSettings.Instance.textHorizontalAxisUpperBound.text = languagesUsed.movementGraphSettingsUpperBound;
                GraphSettings.Instance.toggleGraphSettingsUpdateSimulation.GetComponentInChildren<Text>().text = languagesUsed.movementGraphSettingsUpdateSimulation;
                GraphSettings.Instance.buttonGraphSettingsDefaultValues.GetComponentInChildren<Text>().text = languagesUsed.movementGraphSettingsDefaultValuesButton;
                GraphSettings.Instance.buttonGraphSettingsCancel.GetComponentInChildren<Text>().text = languagesUsed.movementGraphSettingsCancelButton;*/

        //        textTakeOffTitle.text = languagesUsed.takeOffTitle;
        //		textTakeOffSpeed.text = languagesUsed.takeOffTitleSpeed;


        /*        List<string> dropDownOptions = new List<string>();
                dropDownOptions.Add(languagesUsed.takeOffConditionNoGravity);
                dropDownOptions.Add(languagesUsed.takeOffConditionTrampolining);
                dropDownOptions.Add(languagesUsed.takeOffConditionTumbling);
                dropDownOptions.Add(languagesUsed.takeOffConditionDiving1m);
                dropDownOptions.Add(languagesUsed.takeOffConditionDiving3m);
                dropDownOptions.Add(languagesUsed.takeOffConditionDiving5m);
                dropDownOptions.Add(languagesUsed.takeOffConditionDiving10m);
                dropDownOptions.Add(languagesUsed.takeOffConditionHighBar);
                dropDownOptions.Add(languagesUsed.takeOffConditionUnevenBars);
                dropDownOptions.Add(languagesUsed.takeOffConditionVault);
                dropDownTakeOffCondition.ClearOptions();
                dropDownTakeOffCondition.AddOptions(dropDownOptions);
                textTakeOffInitialPosture.text = languagesUsed.takeOffInitialPosture*/


        //        textTakeOffSomersaultPosition.text = languagesUsed.takeOffSomersaultPosition;
        //        textTakeOffTilt.text = languagesUsed.takeOffTilt;
        //        textTakeOffHorizontalSpeed.text = languagesUsed.takeOffHorizontal;
        //        textTakeOffVerticalSpeed.text = languagesUsed.takeOffVertical;
        //        textTakeOffSomersaultSpeed.text = languagesUsed.takeOffSomersaultSpeed;
        //        textTakeOffTwistSpeed.text = languagesUsed.takeOffTwist;

        /*        if (AnimationF.Instance.lineStickFigure != null)
                {
                    AnimationF.Instance.textCurveName1.text = languagesUsed.leftSide;
                    AnimationF.Instance.textCurveName2.text = languagesUsed.rightSide;
                }*/
        

        List<string> dropDownOptions = new List<string>();
        dropDownOptions.Add(languagesUsed.animatorPlaySpeedFast);
        dropDownOptions.Add(languagesUsed.animatorPlaySpeedNormal);
        dropDownOptions.Add(languagesUsed.animatorPlaySpeedSlow1);
        dropDownOptions.Add(languagesUsed.animatorPlaySpeedSlow2);
        dropDownOptions.Add(languagesUsed.animatorPlaySpeedSlow3);
        dropDownAnimationSpeed.ClearOptions();
        dropDownAnimationSpeed.AddOptions(dropDownOptions);


        //        AnimationF.Instance.dropDownPlaySpeed.ClearOptions();
        //        AnimationF.Instance.dropDownPlaySpeed.AddOptions(dropDownOptions);


        if (languagesUsed.toolTipButtonQuit == "Quit")
        {
            textTakeOffTitle.text = languages.english.Condition;

            textTakeOffSomersault.text = languages.english.TakeOffSomersault;
            textTakeOffTilt.text = languages.english.TakeOffTilt;
            textTakeOffTwist.text = languages.english.TakeOffTwist;
            textTakeOffHorizontal.text = languages.english.TakeOffHorizontal;
            textTakeOffVertical.text = languages.english.TakeOffVertical;

            Tab1Title.text = languages.english.Tab1Title;
            Tab2Title.text = languages.english.Tab2Title;
            Tab3Title.text = languages.english.Tab3Title;
            Tab4Title.text = languages.english.Tab4Title;
            LoadSimulationFiles.text = languages.english.LoadSimulationFiles;
            loadButton.text = languages.english.loadButton;
            saveButton.text = languages.english.saveButton;
            compareButton.text = languages.english.compareButton;
            textBack.text = languages.english.Back;
            PerspectiveView.text = languages.english.PerspectiveView;
            PointOfView.text = languages.english.PointOfView;
            AnimationSpeed.text = languages.english.AnimationSpeed;
            StartButton.text = languages.english.StartButton;
            PauseButton.text = languages.english.PauseButton;
            ReplayButton.text = languages.english.ReplayButton;

            SimulationTime.text = languages.english.SimulationTime;
            Duration.text = languages.english.Duration;
        }
        else
        {
            textTakeOffTitle.text = languages.french.Condition;

            textTakeOffSomersault.text = languages.french.TakeOffSomersault;
            textTakeOffTilt.text = languages.french.TakeOffTilt;
            textTakeOffTwist.text = languages.french.TakeOffTwist;
            textTakeOffHorizontal.text = languages.french.TakeOffHorizontal;
            textTakeOffVertical.text = languages.french.TakeOffVertical;

            Tab1Title.text = languages.french.Tab1Title;
            Tab2Title.text = languages.french.Tab2Title;
            Tab3Title.text = languages.french.Tab3Title;
            Tab4Title.text = languages.french.Tab4Title;
            LoadSimulationFiles.text = languages.french.LoadSimulationFiles;
//            loadButton.text = languages.french.loadButton;
//            saveButton.text = languages.french.saveButton;
//            compareButton.text = languages.french.compareButton;
            textBack.text = languages.french.Back;
            PerspectiveView.text = languages.french.PerspectiveView;
            PointOfView.text = languages.french.PointOfView;
            AnimationSpeed.text = languages.french.AnimationSpeed;
            StartButton.text = languages.french.StartButton;
            PauseButton.text = languages.french.PauseButton;
            ReplayButton.text = languages.french.ReplayButton;

            SimulationTime.text = languages.french.SimulationTime;
            Duration.text = languages.french.Duration;
        }
    }

    private void Update()
    {
        if(ToolBox.GetInstance().GetManager<UIManager>().GetCurrentTab() == 1 && ToolBox.GetInstance().GetManager<UIManager>().tooltipOn)
        {
            ToolBox.GetInstance().GetManager<UIManager>().ShowToolTip(1, textTakeOffHorizontal.gameObject, MainParameters.Instance.languages.Used.toolTipTakeOffHorizontal);
            ToolBox.GetInstance().GetManager<UIManager>().ShowToolTip(2, textTakeOffVertical.gameObject, MainParameters.Instance.languages.Used.toolTipTakeOffVertical);
            ToolBox.GetInstance().GetManager<UIManager>().ShowToolTip(3, textTakeOffSomersault.gameObject, MainParameters.Instance.languages.Used.toolTipTakeOffSomersaultPosition);
//            ToolBox.GetInstance().GetManager<UIManager>().ShowToolTip(4, textTakeOffInitialPosture.gameObject, MainParameters.Instance.languages.Used.toolTipTakeOffInitialPosture);
            ToolBox.GetInstance().GetManager<UIManager>().ShowToolTip(5, textTakeOffSomersault.gameObject, MainParameters.Instance.languages.Used.toolTipTakeOffSomersaultSpeed);
            ToolBox.GetInstance().GetManager<UIManager>().ShowToolTip(6, textTakeOffTilt.gameObject, MainParameters.Instance.languages.Used.toolTipTakeOffTilt);
            ToolBox.GetInstance().GetManager<UIManager>().ShowToolTip(7, textTakeOffTwist.gameObject, MainParameters.Instance.languages.Used.toolTipTakeOffTwist);
        }

        if (ToolBox.GetInstance().GetManager<AniGraphManager>().graph != null)
            ToolBox.GetInstance().GetManager<AniGraphManager>().graph.MouseToClient(out mousePosX, out mousePosY);
        else
            return;

        if (Input.GetMouseButtonDown(1) && ToolBox.GetInstance().GetManager<UIManager>().IsOnGameObject(ToolBox.GetInstance().GetManager<AniGraphManager>().graph.gameObject))
        {
            if (ToolBox.GetInstance().GetManager<AniGraphManager>().mouseRightButtonON)
            {
                ToolBox.GetInstance().GetManager<AniGraphManager>().mouseRightButtonON = false;
                ToolBox.GetInstance().GetManager<AniGraphManager>().mouseLeftButtonON = false;
                panelAddRemoveNode.SetActive(false);
            }
            else
            {
                ToolBox.GetInstance().GetManager<AniGraphManager>().mouseRightButtonON = true;
                ToolBox.GetInstance().GetManager<AniGraphManager>().mousePosSaveX = (float)mousePosX;
                ToolBox.GetInstance().GetManager<AniGraphManager>().mousePosSaveY = (float)mousePosY;
                //            buttonAddNode.GetComponentInChildren<Text>().text = MainParameters.Instance.languages.Used.movementButtonAddNode;
                //            buttonRemoveNode.GetComponentInChildren<Text>().text = MainParameters.Instance.languages.Used.movementButtonRemoveNode;
                //            buttonCancelChanges1.GetComponentInChildren<Text>().text = MainParameters.Instance.languages.Used.movementButtonCancelChanges;
                DisplayContextMenu(panelAddRemoveNode);
            }
        }
    }

    void DisplayContextMenu(GameObject panel)
    {
        Vector3 mousePosWorldSpace;
        Vector3[] menuPos = new Vector3[4];
        Vector3[] graphPos = new Vector3[4];
        ToolBox.GetInstance().GetManager<AniGraphManager>().graph.PointToWorldSpace(out mousePosWorldSpace, mousePosX, mousePosY);
        panel.GetComponent<RectTransform>().GetWorldCorners(menuPos);
        ToolBox.GetInstance().GetManager<AniGraphManager>().graph.GetComponent<RectTransform>().GetWorldCorners(graphPos);
        float width = menuPos[2].x - menuPos[1].x;
//        if (mousePosWorldSpace.x < graphPos[2].x - width)
            panel.transform.position = mousePosWorldSpace + new Vector3(width / 2, 0, 0);
//        else
//            panel.transform.position = mousePosWorldSpace - new Vector3(width / 2, 0, 0);
        panel.SetActive(true);
    }

    public int FindPreviousNode(int _ddlUsed, float _mousePosSaveX)
    {
        int i = 0;
        while (i < MainParameters.Instance.joints.nodes[_ddlUsed].T.Length && _mousePosSaveX > MainParameters.Instance.joints.nodes[_ddlUsed].T[i])
            i++;
        return i - 1;
    }

    public void AddNode()
    {
        int ddl = ToolBox.GetInstance().GetManager<AniGraphManager>().ddlUsed;
        int node = FindPreviousNode(ddl, ToolBox.GetInstance().GetManager<AniGraphManager>().mousePosSaveX);

        //                int node = GraphManager.Instance.FindPreviousNode();
        //        int ddl = GraphManager.Instance.ddlUsed;

        ToolBox.GetInstance().GetManager<GameManager>().InterpolationDDL();
        ToolBox.GetInstance().GetManager<GameManager>().DisplayDDL(ddl, true);

        float[] T = new float[MainParameters.Instance.joints.nodes[ddl].T.Length + 1];
        float[] Q = new float[MainParameters.Instance.joints.nodes[ddl].Q.Length + 1];
        for (int i = 0; i <= node; i++)
        {
            T[i] = MainParameters.Instance.joints.nodes[ddl].T[i];
            Q[i] = MainParameters.Instance.joints.nodes[ddl].Q[i];
        }
        T[node + 1] = ToolBox.GetInstance().GetManager<AniGraphManager>().mousePosSaveX;
        Q[node + 1] = ToolBox.GetInstance().GetManager<AniGraphManager>().mousePosSaveY * Mathf.PI / 180;

        for (int i = node + 1; i < MainParameters.Instance.joints.nodes[ddl].T.Length; i++)
        {
            T[i + 1] = MainParameters.Instance.joints.nodes[ddl].T[i];
            Q[i + 1] = MainParameters.Instance.joints.nodes[ddl].Q[i];
        }
        MainParameters.Instance.joints.nodes[ddl].T = MathFunc.MatrixCopy(T);
        MainParameters.Instance.joints.nodes[ddl].Q = MathFunc.MatrixCopy(Q);

//        int frame = (int)Mathf.Round(mousePosSaveX / MainParameters.Instance.joints.lagrangianModel.dt);
        //        ToolBox.GetInstance().GetManager<AniGraphManager>().InterpolationAndDisplayDDL(ddl, ddl, frame, false);

        ToolBox.GetInstance().GetManager<GameManager>().InterpolationDDL();
        ToolBox.GetInstance().GetManager<GameManager>().DisplayDDL(ddl, false);

        ToolBox.GetInstance().GetManager<AniGraphManager>().mouseRightButtonON = false;
    }

    public void RemoveNode()
    {
        int ddl = ToolBox.GetInstance().GetManager<AniGraphManager>().ddlUsed;

        if (MainParameters.Instance.joints.nodes[ddl].T.Length < 3 || MainParameters.Instance.joints.nodes[ddl].Q.Length < 3)
        {
            //            GraphManager.Instance.panelMoveErrMsg.GetComponentInChildren<Text>().text = MainParameters.Instance.languages.Used.errorMsgNotEnoughNodes;
            //            GraphManager.Instance.mouseTracking = false;
            //            GraphManager.Instance.panelMoveErrMsg.SetActive(true);
            ToolBox.GetInstance().GetManager<AniGraphManager>().mouseLeftButtonON = false;
            ToolBox.GetInstance().GetManager<AniGraphManager>().mouseRightButtonON = false;
            return;
        }


        int node = ToolBox.GetInstance().GetManager<AniGraphManager>().FindNearestNode();

        float[] T = new float[MainParameters.Instance.joints.nodes[ddl].T.Length - 1];
        float[] Q = new float[MainParameters.Instance.joints.nodes[ddl].Q.Length - 1];
        for (int i = 0; i < node; i++)
        {
            T[i] = MainParameters.Instance.joints.nodes[ddl].T[i];
            Q[i] = MainParameters.Instance.joints.nodes[ddl].Q[i];
        }
        for (int i = node + 1; i < MainParameters.Instance.joints.nodes[ddl].T.Length; i++)
        {
            T[i - 1] = MainParameters.Instance.joints.nodes[ddl].T[i];
            Q[i - 1] = MainParameters.Instance.joints.nodes[ddl].Q[i];
        }
        MainParameters.Instance.joints.nodes[ddl].T = MathFunc.MatrixCopy(T);
        MainParameters.Instance.joints.nodes[ddl].Q = MathFunc.MatrixCopy(Q);

        ToolBox.GetInstance().GetManager<GameManager>().InterpolationDDL();
        ToolBox.GetInstance().GetManager<GameManager>().DisplayDDL(ddl, false);

        ToolBox.GetInstance().GetManager<AniGraphManager>().mouseRightButtonON = false;
    }
}