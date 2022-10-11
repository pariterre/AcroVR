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
    protected DrawManager drawManager;
    protected GameManager gameManager;
    protected UIManager uiManager;
    
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

    protected MissionBanner missionBanner;
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
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();
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
        if (HasActiveMission){
            ManageConditions();
            ManageInputFields();
            missionBanner.SetText(AllMissions.missions[CurrentMissionIndex].Name);
            missionBanner.Show(false, false);
        }
    }

    public void SetCurrentMission()
    {
        if (Level < 0 || SubLevel < 0) return;

        for (int i = 0; i < AllMissions.count; i++)
        {
            if (AllMissions.missions[i].Level == Level)
            {
                CurrentMissionIndex = i + SubLevel - 1;  // 1-indexed!
                return;
            }
        }
    }

    void ManageConditions(){
        uiManager.SetDropDownPresetCondition(AllMissions.missions[CurrentMissionIndex].Condition);
    }

    void ManageInputFields()
    {
        UserUIInputsIsActive _status = AllMissions.missions[CurrentMissionIndex].enabledInputs;
        uiManager.userInputs.SetAllFromUI(_status);
    }

    public void CheckMissionResult()
    {
        Result IsSuccess(float _value, float[] _contraint){
            bool CheckMinMax(float input, float min, float max) => input >= min && input <= max;
            
            if (_contraint == null) return Result.SUCCESS;  // If no constraint was applied
            
            var _minAccepted = _contraint[0];
            var _maxAccepted = _contraint.Length > 1 ? _contraint[1] : _contraint[0];
            return CheckMinMax(_value, _minAccepted, _maxAccepted) ? Result.SUCCESS : Result.FAIL;
        }

        if (CurrentMissionIndex < 0) return;
        MissionInfo mission = AllMissions.missions[CurrentMissionIndex];

        // Get the input results from the UI
        var _somersault = Utils.ToFloat(uiManager.userInputs.Somersault.text);
        var _tilt = Utils.ToFloat(uiManager.userInputs.Tilt.text);
        var _twist = Utils.ToFloat(uiManager.userInputs.Twist.text);
        var _horizontalPosition = Utils.ToFloat(uiManager.userInputs.HorizontalPosition.text);
        var _verticalPosition = Utils.ToFloat(uiManager.userInputs.VerticalPosition.text);
        var _somersaultSpeed = Utils.ToFloat(uiManager.userInputs.SomersaultSpeed.text);
        var _tiltSpeed = Utils.ToFloat(uiManager.userInputs.TiltSpeed.text);
        var _twistSpeed = Utils.ToFloat(uiManager.userInputs.TwistSpeed.text);
        var _horizontalSpeed = Utils.ToFloat(uiManager.userInputs.HorizontalSpeed.text);
        var _verticalSpeed = Utils.ToFloat(uiManager.userInputs.VerticalSpeed.text);

        // Get computed results
        var _horizontalDistance = drawManager.HorizontalTravelDistance;
        var _verticalDistance = drawManager.VerticalTravelDistance;
        var _travelDistance = drawManager.TravelDistance;

        // All condition must be SUCCESS to declare the result to be valid
        MissionResult = 
            (
                IsSuccess(_horizontalSpeed, mission.constraints.HorizontalSpeed) == Result.SUCCESS 
                && IsSuccess(_verticalSpeed, mission.constraints.VerticalSpeed) == Result.SUCCESS 
                && IsSuccess(_travelDistance, mission.goal.Distance) == Result.SUCCESS 

            )
            ? Result.SUCCESS 
            : Result.FAIL;

        ProcessResult();
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
