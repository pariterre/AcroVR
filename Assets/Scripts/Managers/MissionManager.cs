using System.Collections;
using System.Collections.Generic;
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
    public bool IsBannerShown { get; protected set; } = false;
    protected Animator InformationBannerAnimator;
    protected Text InformationBannerText;
    public void SetInformationBanner(GameObject _banner)
    {
        if (_banner == null) return;

        InformationBannerAnimator = _banner.GetComponent<Animator>();
        InformationBannerText = _banner.GetComponentInChildren<Text>();
    }

    public MissionList AllMissions { get; protected set; }
    public void SetMissions(MissionList _missions) { AllMissions = _missions; }

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
    protected Fireworks fireworks;
    public void SetupFireworks(Fireworks _fireworks){ fireworks = _fireworks; }


    public int Level { get; protected set; } = -1;
    public void SetLevel(int _level) { Level = _level; }
    public int SubLevel { get; protected set; } = -1;
    public void SetSubLevel(int _subLevel) { SubLevel = _subLevel; }
    protected int CurrentMissionIndex = -1;
    public bool HasActiveMission { get { return CurrentMissionIndex >= 0; } }
    public Result MissionResult { get; protected set; } = Result.NOT_APPLICABLE;


    void Start()
    {
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
    }

    public void UnloadMission()
    {
        Level = -1;
        SubLevel = -1;
        CurrentMissionIndex = -1;
    }

    public void SetAndShowCurrentMission()
    {
        if (InformationBannerText == null || InformationBannerAnimator == null) return;

        SetCurrentMission();
        ShowCurrentMission();
    }

    public void SetCurrentMission()
    {
        if (Level < 0 || SubLevel < 0) return;

        for (int i = 0; i < AllMissions.count; i++)
        {
            if (AllMissions.missions[i].Level == Level)
            {
                CurrentMissionIndex = i + SubLevel - 1;  // 1-indexed!
                InformationBannerText.text = AllMissions.missions[CurrentMissionIndex].Name;
                CheckParameterOnOff();
                break;
            }
        }
    }

    public void ShowCurrentMission() {
        if (HasActiveMission)
        {
            InformationBannerAnimator.Play("Panel In");
            IsBannerShown = true;
            StartCoroutine(WaitClickToCloseBanner());
        }
    }

    void CheckParameterOnOff()
    {
        void ManageInputField(bool keepEnabled, InputField field)
        {
            if (keepEnabled) return;

            field.enabled = false;
            field.image.color = Color.blue;
        }

        Buttons inputs = AllMissions.missions[CurrentMissionIndex].enabledInputs;

        ManageInputField(inputs.Somersault, InputFieldSomersaultPosition);
        ManageInputField(inputs.SomersaultSpeed, InputFieldSomersaultSpeed);
        ManageInputField(inputs.Tilt, InputFieldTiltPosition);
        ManageInputField(inputs.TiltSpeed, InputFieldTiltSpeed);
        ManageInputField(inputs.Twist, InputFieldTwistPosition);
        ManageInputField(inputs.TwistSpeed, InputFieldTwistSpeed);
        ManageInputField(inputs.HorizontalPosition, InputFieldHorizontalPosition);
        ManageInputField(inputs.HorizontalSpeed, InputFieldHorizontalSpeed);
        ManageInputField(inputs.VerticalPosition, InputFieldVerticalPosition);
        ManageInputField(inputs.VerticalSpeed, InputFieldVerticalSpeed);
        ManageInputField(inputs.Duration, InputFieldDuration);
    }

    public void CheckMissionResult()
    {
        if (CurrentMissionIndex < 0) return;

        var HorizontalSpeed = float.Parse(InputFieldHorizontalSpeed.text, NumberStyles.Number, CultureInfo.InvariantCulture);

        MissionInfo mission = AllMissions.missions[CurrentMissionIndex];

        var _minAcceptedDistance = mission.goal.Distance[0];
        var _maxAcceptedDistance = mission.goal.Distance[1];
        var _resultHorizontalDistance = CheckMinMax(HorizontalSpeed, _minAcceptedDistance, _maxAcceptedDistance) ? Result.SUCCESS : Result.FAIL;

        var _minAcceptedSpeed = mission.constraints.HorizontalSpeed[0];
        var _maxAcceptedSpeed = mission.constraints.HorizontalSpeed.Length > 1 ? mission.constraints.HorizontalSpeed[1] : 999;
        var _resultHorizontalSpeed = CheckMinMax(HorizontalSpeed, _minAcceptedSpeed, _maxAcceptedSpeed) ? Result.SUCCESS : Result.FAIL;

        MissionResult = 
            _resultHorizontalDistance == Result.SUCCESS 
                && _resultHorizontalSpeed == Result.SUCCESS 
            ? Result.SUCCESS 
            : Result.FAIL;
        
        ShowBannerResult();
    }

    bool CheckMinMax(float input, float min, float max)
    {
        return (input >= min && input <= max);
    }

    void ShowBannerResult(){
        InformationBannerAnimator.Play("Panel In");
        IsBannerShown = true;

        if (MissionResult == Result.SUCCESS)
        {
            InformationBannerText.text = "Succès";
            MissionResult = Result.NOT_APPLICABLE;
            if (fireworks != null)
                fireworks.StartFireworks();
            // TODO: Launch next mission if requested
        }
        else
        {
            string txt = "Désolé, vous n’avez pas atteint l’objectif avec une précision suffisante.\n";
            string hints = null;

            if (AllMissions.missions[CurrentMissionIndex].Hint != null)
                hints = AllMissions.missions[CurrentMissionIndex].Hint;

            InformationBannerText.text = txt + hints + "" + "Veuillez réessayer";
        }
        StartCoroutine(WaitClickToCloseBanner());
    }

    IEnumerator WaitClickToCloseBanner(){
        while (true){
            if (Input.anyKeyDown)
            {
                InformationBannerAnimator.Play("Panel Out");
                IsBannerShown = false;
                MissionResult = Result.NOT_APPLICABLE;
                if (fireworks != null)
                    fireworks.EndFireworks();
                break;
            }
            yield return null;
        }
    }

}
