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
    protected MissionBanner missionBanner;
    public void SetInformationBanner(MissionBanner _banner){ missionBanner = _banner; }

    public MissionList AllMissions { get; protected set; }
    protected List<bool> MissionsCompleted = new List<bool>();
    public bool IsMissionCompleted(int _index) { return MissionsCompleted[_index]; }
    public bool IsLevelCompleted(int _index) { 
        bool _hasAtLeastOne = false; 
        for (int i=0; i<AllMissions.count; ++i){
            var _trueIndex = _index + 1;  // One-based
            if (AllMissions.missions[i].Level == _trueIndex){
                _hasAtLeastOne = true;
                if (!MissionsCompleted[i]) return false;
            }
        }
        return _hasAtLeastOne;
    }
    public void SetMissions(string _dataAsJson) { 
        AllMissions = JsonUtility.FromJson<MissionList>(_dataAsJson);

        MissionsCompleted.Clear();
        foreach (var _mission in AllMissions.missions){
            MissionsCompleted.Add(PlayerPrefs.GetInt(_mission.ToHash(), 0) != 0);
        }
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
        if (missionBanner == null) return;

        SetCurrentMission();
        if (HasActiveMission)
            missionBanner.Show(false, false);
    }

    public void SetCurrentMission()
    {
        if (Level < 0 || SubLevel < 0) return;

        for (int i = 0; i < AllMissions.count; i++)
        {
            if (AllMissions.missions[i].Level == Level)
            {
                CurrentMissionIndex = i + SubLevel - 1;  // 1-indexed!
                missionBanner.SetText(AllMissions.missions[CurrentMissionIndex].Name);
                CheckParameterOnOff();
                break;
            }
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
        
        ProcessResult();
    }

    bool CheckMinMax(float input, float min, float max)
    {
        return (input >= min && input <= max);
    }

    void ProcessResult(){
        var _currentMission = AllMissions.missions[CurrentMissionIndex];
        if (MissionResult == Result.SUCCESS)
        {
            missionBanner.SetText(MainParameters.Instance.languages.Used.missionSuccess);
            PlayerPrefs.SetInt(_currentMission.ToHash(), 1);
            MissionsCompleted[CurrentMissionIndex] = true;
            
            if (fireworks != null)
                fireworks.StartFireworks();
                missionBanner.Show(false, false, ProcessEndOfMission);
        }
        else
        {
            string txt = MainParameters.Instance.languages.Used.missionFailed;
            string hints = _currentMission.Hint != null ? _currentMission.Hint : null;
            missionBanner.SetText(txt + "\n" + hints + "\n" + MainParameters.Instance.languages.Used.missionTryAgain);
            missionBanner.Show(true, true, ProcessEndOfMission);
        }
    }

    void ProcessEndOfMission(bool _clickedContinue){
        if (fireworks != null)
            fireworks.EndFireworks();
        
        if (MissionResult == Result.SUCCESS || _clickedContinue)
            SubLevel += 1;
        MissionResult = Result.NOT_APPLICABLE;
 
        SetAndShowCurrentMission();
    }
}
