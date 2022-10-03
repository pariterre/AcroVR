using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public enum Result
{
    NOT_APPLICABLE,
    SUCCESS,
    FAIL,
}

public class MissionManager : MonoBehaviour
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
    public Result MissionResult { get; protected set; } = Result.NOT_APPLICABLE;

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

        var _minAcceptedDistance = mission.goal.Distance[0];
        var _maxAcceptedDistance = mission.goal.Distance[1];
        var _resultHorizontalDistance = CheckMinMax(HorizontalSpeed, _minAcceptedDistance, _maxAcceptedDistance) ? Result.SUCCESS : Result.FAIL;

        var _minAcceptedSpeed = mission.solution.HorizontalVelocity[0];
        var _maxAcceptedSpeed = mission.solution.HorizontalVelocity.Length > 1 ? mission.solution.HorizontalVelocity[1] : 999;
        var _resultHorizontalSpeed = CheckMinMax(HorizontalSpeed, _minAcceptedSpeed, _maxAcceptedSpeed) ? Result.SUCCESS : Result.FAIL;

        MissionResult = 
            _resultHorizontalDistance == Result.SUCCESS 
                && _resultHorizontalSpeed == Result.SUCCESS 
            ? Result.SUCCESS 
            : Result.FAIL;
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

        if (MissionResult != Result.NOT_APPLICABLE)
        {
            if(slider.value >= ToolBox.GetInstance().GetManager<DrawManager>().numberFrames - 1)
            {
                MissionName.GetComponent<Animator>().Play("Panel In");

                if (MissionResult == Result.SUCCESS)
                {
                    MissionName.GetComponentInChildren<Text>().text = "Succès";
                    MissionResult = Result.NOT_APPLICABLE;
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
