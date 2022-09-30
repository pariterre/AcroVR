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

    private GameManager gameManager;

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
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();

        numberOfMissions = gameManager.numMission;
        ShowCurrentMission();
    }

    void ShowCurrentMission() {

        if (numberOfMissions > 0)
        {
            MissionName.GetComponent<Animator>().Play("Panel In");

            for (int i = 0; i < gameManager.listMission.count; i++)
            {
                if (gameManager.listMission.missions[i].Level == gameManager.numLevel)
                {
                    currentMission = i + numberOfMissions - 1;
                    MissionName.GetComponentInChildren<Text>().text = gameManager.listMission.missions[currentMission].Name;
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

        Buttons btn = gameManager.listMission.missions[_n].disableButton;

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

    public void CheckMissionResult()
    {
        if (numberOfMissions == 0) return;
        TwistSpeed = float.Parse(PanelTwistSpeed.GetComponent<InputField>().text, NumberStyles.Number, CultureInfo.InvariantCulture);
        HorizontalSpeed = float.Parse(PanelHorizontalSpeed.GetComponent<InputField>().text, NumberStyles.Number, CultureInfo.InvariantCulture);
        VerticalSpeed = float.Parse(PanelVerticalSpeed.GetComponent<InputField>().text, NumberStyles.Number, CultureInfo.InvariantCulture);
        Duration = float.Parse(PanelDuration.GetComponent<InputField>().text, NumberStyles.Number, CultureInfo.InvariantCulture);

        MissionInfo mission = gameManager.listMission.missions[currentMission];

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

    IEnumerator WaitThenShowCurrentMission(int time)
    {
        yield return new WaitForSeconds(time);
        ShowCurrentMission();
    }

    void Update()
    {
        if (gameManager.numMission > 0)
        {
            if (Input.anyKeyDown)
            {
                gameManager.SetNumberOfMissions(0);
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
                    resultValue = 0;
                    gameManager.SetNumberOfMissions(numberOfMissions + 1);
                    StartCoroutine(WaitThenShowCurrentMission(3));
                }
                else
                {
                    string txt = "Désolé, vous n’avez pas atteint l’objectif avec une précision suffisante.\n";
                    string hints = null;

                    if (gameManager.listMission.missions[currentMission].Hint != null)
                        hints = gameManager.listMission.missions[currentMission].Hint;

                    MissionName.GetComponentInChildren<Text>().text = txt + hints;
                }

            }
        }
    }
}
