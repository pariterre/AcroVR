using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class GameMode : MonoBehaviour
{
    public GameObject MissionName;

    public GameObject PanelSomersaultPosition;
    public GameObject PanelTiltPosition;
    public GameObject PanelTwistPosition;
    public GameObject PanelHorizontalPosition;
    public GameObject PanelVerticalPosition;
    public GameObject PanelSomersaultSpeed;
    public GameObject PanelTiltSpeed;
    public GameObject PanelTwistSpeed;
    public GameObject PanelHorizontalSpeed;
    public GameObject PanelVerticalSpeed;
    public GameObject PanelDuration;

    public Slider slider;

    float TwistSpeed = 0;
    float HorizontalSpeed = 0;
    float VerticalSpeed = 0;
    float Duration = 0;

    private int currentMission = 0;
    private int numberOfMissions = 0;
    private int resultValue = 0;

    void Start()
    {
        numberOfMissions = ToolBox.GetInstance().GetManager<GameManager>().numMission;
        ShowCurrentMission();
    }

    void ShowCurrentMission() {

        if (numberOfMissions > 0)
        {
            MissionName.GetComponent<Animator>().Play("Panel In");

            for (int i = 0; i < ToolBox.GetInstance().GetManager<GameManager>().listMission.count; i++)
            {
                if (ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[i].Level == ToolBox.GetInstance().GetManager<GameManager>().numLevel)
                {
                    currentMission = i + numberOfMissions - 1;
                    MissionName.GetComponentInChildren<Text>().text = ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[currentMission].Name;
                    CheckParameterOnOff(currentMission);
                    break;
                }
            }
        }
    }

    void CheckParameterOnOff(int _n)
    {
        void ManageInputField(bool keepEnabled, InputField field)
        {
            if (keepEnabled) return;

            field.enabled = false;
            field.image.color = Color.blue;
        }

        Buttons btn = ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[_n].disableButton;

        ManageInputField(btn.Salto, PanelSomersaultPosition.GetComponent<InputField>());
        ManageInputField(btn.SaltoVelocity, PanelSomersaultSpeed.GetComponent<InputField>());
        ManageInputField(btn.Inclinaison, PanelTiltPosition.GetComponent<InputField>());
        ManageInputField(btn.InclinaisonVelocity, PanelTiltSpeed.GetComponent<InputField>());
        ManageInputField(btn.Vrille, PanelTwistPosition.GetComponent<InputField>());
        ManageInputField(btn.VrilleVelocity, PanelTwistSpeed.GetComponent<InputField>());
        ManageInputField(btn.HorizontalPosition, PanelHorizontalPosition.GetComponent<InputField>());
        ManageInputField(btn.HorizontalVelocity, PanelHorizontalSpeed.GetComponent<InputField>());
        ManageInputField(btn.VerticalPosition, PanelVerticalPosition.GetComponent<InputField>());
        ManageInputField(btn.VerticalVelocity, PanelVerticalSpeed.GetComponent<InputField>());
        ManageInputField(btn.Duration, PanelDuration.GetComponent<InputField>());
    }

    public void GetTakeOffParameters()
    {
        if (numberOfMissions > 0)
        {
            TwistSpeed = float.Parse(PanelTwistSpeed.GetComponent<InputField>().text, NumberStyles.Number, CultureInfo.InvariantCulture);
            HorizontalSpeed = float.Parse(PanelHorizontalSpeed.GetComponent<InputField>().text, NumberStyles.Number, CultureInfo.InvariantCulture);
            VerticalSpeed = float.Parse(PanelVerticalSpeed.GetComponent<InputField>().text, NumberStyles.Number, CultureInfo.InvariantCulture);
            Duration = float.Parse(PanelDuration.GetComponent<InputField>().text, NumberStyles.Number, CultureInfo.InvariantCulture);

            CheckGameResult();
        }
    }

    void CheckGameResult()
    {
        MissionInfo mission = ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[currentMission];

        if(HorizontalSpeed != 0)
        {
            float min = 0;
            float max = 999;

            // Check Input value
            if (mission.solution.HorizontalVelocity.Length == 1) min = mission.solution.HorizontalVelocity[0];
            else
            {
                min = mission.solution.HorizontalVelocity[0];
                max = mission.solution.HorizontalVelocity[1];
            }

            if (CheckMinMax(HorizontalSpeed, min, max))
            {
                //Success
                resultValue = 1;
            }
            else
            {
                //Fail
                resultValue = 2;
            }

            // Check Result distance
            if(resultValue == 1)
            {
                float distance = ToolBox.GetInstance().GetManager<DrawManager>().resultDistance;

                min = mission.goal.Distance[0];
                max = mission.goal.Distance[1];
                if (!CheckMinMax(distance, min, max))
                {
                    //Fail
                    resultValue = 2;
                }
            }
        }
    }

    bool CheckMinMax(float input, float min, float max)
    {
        return (input >= min && input <= max);
    }

    IEnumerator WaitThenNextLevel()
    {
        yield return new WaitForSeconds(3);
        ShowCurrentMission();
    }

    void Update()
    {
        if (ToolBox.GetInstance().GetManager<GameManager>().numMission > 0)
        {
            if (Input.anyKeyDown)
            {
                ToolBox.GetInstance().GetManager<GameManager>().numMission = 0;
                MissionName.GetComponent<Animator>().Play("Panel Out");
            }
        }

        if (resultValue > 0)
        {
            if(slider.value >= ToolBox.GetInstance().GetManager<DrawManager>().numberFrames - 1)
            {
                MissionName.GetComponent<Animator>().Play("Panel In");

                if (resultValue == 1)
                {
                    MissionName.GetComponentInChildren<Text>().text = "Succès";
                    StartCoroutine(WaitThenNextLevel());
                    resultValue = 0;
                    ToolBox.GetInstance().GetManager<GameManager>().numMission = numberOfMissions + 1;
                }
                else
                {
                    string txt = "Désolé, vous n’avez pas atteint l’objectif avec une précision suffisante.\n";
                    string hints = null;

                    if (ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[currentMission].Hint != null)
                        hints = ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[currentMission].Hint;

                    MissionName.GetComponentInChildren<Text>().text = txt + hints;
                }

            }
        }
    }
}
