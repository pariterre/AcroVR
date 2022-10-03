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
    protected Animator InformationBandAnimator;
    protected Text InformationBandText;
    public void SetInformationBand(GameObject _tutorial)
    {
        if (_tutorial == null) return;

        InformationBandAnimator = _tutorial.GetComponent<Animator>();
        InformationBandText = _tutorial.GetComponentInChildren<Text>();
    }

    protected InputField InputFieldSomersaultPosition;
    protected InputField InputFieldTiltPosition;
    protected InputField InputFieldTwistPosition;
    protected InputField InputFieldHorizontalPosition;
    protected InputField InputFieldVerticalPosition;
    protected InputField InputFieldSomersaultSpeed;
    protected InputField InputFieldTiltSpeed;
    protected InputField InputFieldTwistSpeed;
    protected InputField InputFieldHorizontalSpeed;
    protected InputField InputFieldVerticalSpeed;
    protected InputField InputFieldDuration;
    public void SetAllInputField(
        InputField _somersaultPosition, InputField _tiltPosition, InputField _twistPosition, InputField _horizontalPosition, InputField _verticalPosition,
        InputField _somersaultSpeed, InputField _tiltSpeed, InputField _twistSpeed, InputField _horizontalSpeed, InputField _verticalSpeed, InputField _duration
    )
    {
        InputFieldSomersaultPosition = _somersaultPosition;
        InputFieldTiltPosition = _tiltPosition;
        InputFieldTwistPosition = _twistPosition;
        InputFieldHorizontalPosition = _horizontalPosition;
        InputFieldVerticalPosition = _verticalPosition;
        InputFieldSomersaultSpeed = _somersaultSpeed;
        InputFieldTiltSpeed = _tiltSpeed;
        InputFieldTwistSpeed = _twistSpeed;
        InputFieldHorizontalSpeed = _horizontalSpeed;
        InputFieldVerticalSpeed = _verticalSpeed;
        InputFieldDuration = _duration;
    }

    private GameManager gameManager;

    float TwistSpeed = 0;
    float HorizontalSpeed = 0;
    float VerticalSpeed = 0;
    float Duration = 0;

    private int currentMission = 0;
    public Result MissionResult { get; protected set; } = Result.NOT_APPLICABLE;

    void Start()
    {
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
    }


    public void ShowCurrentMission() {

        if (gameManager.numMission > 0)
        {
            InformationBandAnimator.Play("Panel In");

            for (int i = 0; i < gameManager.listMission.count; i++)
            {
                if (gameManager.listMission.missions[i].Level == gameManager.numLevel)
                {
                    currentMission = i + gameManager.numMission - 1;
                    InformationBandText.text = gameManager.listMission.missions[currentMission].Name;
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

        ManageInputField(btn.Salto, InputFieldSomersaultPosition);
        ManageInputField(btn.SaltoVelocity, InputFieldSomersaultSpeed);
        ManageInputField(btn.Inclinaison, InputFieldTiltPosition);
        ManageInputField(btn.InclinaisonVelocity, InputFieldTiltSpeed);
        ManageInputField(btn.Vrille, InputFieldTwistPosition);
        ManageInputField(btn.VrilleVelocity, InputFieldTwistSpeed);
        ManageInputField(btn.HorizontalPosition, InputFieldHorizontalPosition);
        ManageInputField(btn.HorizontalVelocity, InputFieldHorizontalSpeed);
        ManageInputField(btn.VerticalPosition, InputFieldVerticalPosition);
        ManageInputField(btn.VerticalVelocity, InputFieldVerticalSpeed);
        ManageInputField(btn.Duration, InputFieldDuration);
    }

    public void CheckMissionResult()
    {
        TwistSpeed = float.Parse(InputFieldTwistSpeed.text, NumberStyles.Number, CultureInfo.InvariantCulture);
        HorizontalSpeed = float.Parse(InputFieldHorizontalSpeed.text, NumberStyles.Number, CultureInfo.InvariantCulture);
        VerticalSpeed = float.Parse(InputFieldVerticalSpeed.text, NumberStyles.Number, CultureInfo.InvariantCulture);
        Duration = float.Parse(InputFieldDuration.text, NumberStyles.Number, CultureInfo.InvariantCulture);

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

    void Update()
    {
        if (gameManager.numMission > 0)
        {
            if (Input.anyKeyDown)
            {
                gameManager.SetNumberOfMissions(0);
                InformationBandAnimator.Play("Panel Out");
            }
        }

        if (MissionResult != Result.NOT_APPLICABLE)
        {
                InformationBandAnimator.Play("Panel In");

                if (MissionResult == Result.SUCCESS)
                {
                    InformationBandText.text = "Succès";
                    MissionResult = Result.NOT_APPLICABLE;
                    gameManager.SetNumberOfMissions(gameManager.numMission + 1);
                    // TODO: Launch next mission if requested
                }
                else
                {
                    string txt = "Désolé, vous n’avez pas atteint l’objectif avec une précision suffisante.\n";
                    string hints = null;

                    if (gameManager.listMission.missions[currentMission].Hint != null)
                        hints = gameManager.listMission.missions[currentMission].Hint;

                    InformationBandText.text = txt + hints;
                }

        }
    }
}
