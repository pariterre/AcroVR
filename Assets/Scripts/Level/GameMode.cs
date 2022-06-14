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

    private int currMission = 0;
    private int num = 0;
    private int resultValue = 0;

    void Start()
    {
        num = ToolBox.GetInstance().GetManager<GameManager>().numMission;

        if (num > 0)
        {
            MissionName.GetComponent<Animator>().Play("Panel In");

            for (int i = 0; i < ToolBox.GetInstance().GetManager<GameManager>().listMission.count; i++)
            {
                if (ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[i].Level == ToolBox.GetInstance().GetManager<GameManager>().numLevel)
                {
                    currMission = i + num - 1;
                    MissionName.GetComponentInChildren<Text>().text = ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[currMission].Name;
                    CheckParameterOnOff(currMission);
                    break;
                }
            }
        }
    }

    void CheckParameterOnOff(int _n)
    {
        Buttons btn = ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[_n].disableButton;

        if(!btn.Salto)
        {
            PanelSomersaultPosition.GetComponent<InputField>().enabled = false;
            PanelSomersaultPosition.GetComponent<InputField>().image.color = Color.blue;
        }
        if (!btn.SaltoVelocity)
        {
            PanelSomersaultSpeed.GetComponent<InputField>().enabled = false;
            PanelSomersaultSpeed.GetComponent<InputField>().image.color = Color.blue;
        }
        if (!btn.Inclinaison)
        {
            PanelTiltPosition.GetComponent<InputField>().enabled = false;
            PanelTiltPosition.GetComponent<InputField>().image.color = Color.blue;
        }
        if (!btn.InclinaisonVelocity)
        {
            PanelTiltSpeed.GetComponent<InputField>().enabled = false;
            PanelTiltSpeed.GetComponent<InputField>().image.color = Color.blue;
        }
        if (!btn.Vrille)
        {
            PanelTwistPosition.GetComponent<InputField>().enabled = false;
            PanelTwistPosition.GetComponent<InputField>().image.color = Color.blue;
        }
        if (!btn.VrilleVelocity)
        {
            PanelTwistSpeed.GetComponent<InputField>().enabled = false;
            PanelTwistSpeed.GetComponent<InputField>().image.color = Color.blue;
        }
        if (!btn.HorizontalPosition)
        {
            PanelHorizontalPosition.GetComponent<InputField>().enabled = false;
            PanelHorizontalPosition.GetComponent<InputField>().image.color = Color.blue;
        }
        if (!btn.HorizontalVelocity)
        {
            PanelHorizontalSpeed.GetComponent<InputField>().enabled = false;
            PanelHorizontalSpeed.GetComponent<InputField>().image.color = Color.blue;
        }
        if (!btn.VerticalPosition)
        {
            PanelVerticalPosition.GetComponent<InputField>().enabled = false;
            PanelVerticalPosition.GetComponent<InputField>().image.color = Color.blue;
        }
        if (!btn.VerticalVelocity)
        {
            PanelVerticalSpeed.GetComponent<InputField>().enabled = false;
            PanelVerticalSpeed.GetComponent<InputField>().image.color = Color.blue;
        }
        if (!btn.Duration)
        {
            PanelDuration.GetComponent<InputField>().enabled = false;
            PanelDuration.GetComponent<InputField>().image.color = Color.blue;
        }
    }

    public void GetTakeOffParameters()
    {
        if (num > 0)
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
        MissionInfo mission = ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[currMission];

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
            if(slider.value >= ToolBox.GetInstance().GetManager<DrawManager>().numberFrames)
            {
                MissionName.GetComponent<Animator>().Play("Panel In");

                if (resultValue == 1)
                    MissionName.GetComponentInChildren<Text>().text = "Succès";
                else
                {
                    string txt = "Désolé, vous n’avez pas atteint l’objectif avec une précision suffisante.\n";
                    string hints = null;

                    if (ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[currMission].Hint != null)
                        hints = ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[currMission].Hint;

                    MissionName.GetComponentInChildren<Text>().text = txt + hints;
                }

                ToolBox.GetInstance().GetManager<GameManager>().numMission = num;
                resultValue = 0;
            }
        }
    }
}
