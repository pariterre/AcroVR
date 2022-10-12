using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///		UI In-Game menu logic
/// </summary>

[System.Serializable]
public class UserUIInputs
{
    public InputField Duration;
    public void SetDuration(float _value, bool _activateField = true) {
        SetInput(Duration, _value, _activateField);
        MainParameters.Instance.joints.Duration = _value;
    }

    public InputField Somersault;
    public void SetSomersault(float _value, bool _activateField = true) {
        SetInput(Somersault, _value, _activateField);
        MainParameters.Instance.joints.takeOffParam.Somersault = _value;
    }

    public InputField Tilt;
    public void SetTilt(float _value, bool _activateField = true) {
        SetInput(Tilt, _value, _activateField);
        MainParameters.Instance.joints.takeOffParam.Tilt = _value;
    }

    public InputField Twist;
    public void SetTwist(float _value, bool _activateField = true) {
        SetInput(Twist, _value, _activateField);
        MainParameters.Instance.joints.takeOffParam.Twist = _value;
    }

    public InputField HorizontalPosition;
    public void SetHorizontalPosition(float _value, bool _activateField = true) {
        SetInput(HorizontalPosition, _value, _activateField);
        MainParameters.Instance.joints.takeOffParam.HorizontalPosition = _value;
    }

    public InputField VerticalPosition;
    public void SetVerticalPosition(float _value, bool _activateField = true) {
        SetInput(VerticalPosition, _value, _activateField);
        MainParameters.Instance.joints.takeOffParam.VerticalPosition = _value;
    }

    public InputField SomersaultSpeed;
    public void SetSomersaultSpeed(float _value, bool _activateField = true) {
        SetInput(SomersaultSpeed, _value, _activateField);
        MainParameters.Instance.joints.takeOffParam.SomersaultSpeed = _value;
    }

    public InputField TiltSpeed;
    public void SetTiltSpeed(float _value, bool _activateField = true) {
        SetInput(TiltSpeed, _value, _activateField);
        MainParameters.Instance.joints.takeOffParam.TiltSpeed = _value;
    }

    public InputField TwistSpeed;
    public void SetTwistSpeed(float _value, bool _activateField = true) {
        SetInput(TwistSpeed, _value, _activateField);
        MainParameters.Instance.joints.takeOffParam.TwistSpeed = _value;
    }

    public InputField HorizontalSpeed;
    public void SetHorizontalSpeed(float _value, bool _activateField = true) {
        SetInput(HorizontalSpeed, _value, _activateField);
        MainParameters.Instance.joints.takeOffParam.HorizontalSpeed = _value;
    }

    public InputField VerticalSpeed;
    public void SetVerticalSpeed(float _value, bool _activateField = true) {
        SetInput(VerticalSpeed, _value, _activateField);
        MainParameters.Instance.joints.takeOffParam.VerticalSpeed = _value;
    }

    public Dropdown PresetConditions;
    public void SetPresetConditions(int _value, bool _activateField = true) {
        SetInput(PresetConditions, _value, _activateField);
        MainParameters.Instance.joints.condition = _value;
        if (PresetConditions != null)
            PlayerPrefs.SetInt("PresetConditions", PresetConditions.value);
    }

    public Toggle UseGravity;
    public void SetUseGravity(bool _value, bool _activateField = true) {
        SetInput(UseGravity, _value, _activateField);
        MainParameters.Instance.joints.UseGravity = _value;  // TODO Change this to drawManager.Joints(0)
        if (UseGravity != null)
            ToolBox.GetInstance().GetManager<DrawManager>().ForceFullUpdate(0);
    }

    public Toggle StopOnGround;
    public void SetStopOnGround(bool _value, bool _activateField = true) {
        SetInput(StopOnGround, _value, _activateField);
        MainParameters.Instance.joints.StopOnGround = _value;
        if (StopOnGround != null)
            ToolBox.GetInstance().GetManager<DrawManager>().ForceFullUpdate(0);
    }

    public void SetParameters(UserUIInputsValues _values){
        var _isActives = new UserUIInputsIsActive();
        _isActives.Initialize();
        SetParameters(_values, _isActives);
    }

    public void SetParameters(UserUIInputsValues _values, UserUIInputsIsActive _isActives)
    {
        SetDuration(_values.Duration, _isActives.Duration);
        SetUseGravity(_values.UseGravity, _isActives.UseGravity);
        SetStopOnGround(_values.StopOnGround, _isActives.StopOnGround);
    }

    public void SetPositions(UserUIInputsValues _values){
        var _isActives = new UserUIInputsIsActive();
        _isActives.Initialize();
        SetPositions(_values, _isActives);
    }

    public void SetPositions(UserUIInputsValues _values, UserUIInputsIsActive _isActives)
    {
        SetSomersault(_values.Somersault, _isActives.Somersault);
        SetTilt(_values.Tilt, _isActives.Tilt);
        SetTwist(_values.Twist, _isActives.Twist);
        SetHorizontalPosition(_values.HorizontalPosition, _isActives.HorizontalPosition);
        SetVerticalPosition(_values.VerticalPosition, _isActives.VerticalPosition);        
    }

    public void SetSpeeds(UserUIInputsValues _values){
        var _isActives = new UserUIInputsIsActive();
        _isActives.Initialize();
        SetSpeeds(_values, _isActives);
    }
    public void SetSpeeds(UserUIInputsValues _values, UserUIInputsIsActive _isActives)
    {
        SetSomersaultSpeed(_values.SomersaultSpeed, _isActives.SomersaultSpeed);
        SetTiltSpeed(_values.TiltSpeed, _isActives.TiltSpeed);
        SetTwistSpeed(_values.TwistSpeed, _isActives.TwistSpeed);
        SetHorizontalSpeed(_values.HorizontalSpeed, _isActives.HorizontalSpeed);
        SetVerticalSpeed(_values.VerticalSpeed, _isActives.VerticalSpeed);    
    }

    public void SetAll(
        UserUIInputsValues _values,
        bool _setParameters = true, 
        bool _setPositions = true, 
        bool _setSpeeds = true
    ){
        var _isActives = new UserUIInputsIsActive();
        _isActives.Initialize();
        SetAll(_values, _isActives, _setParameters, _setPositions, _setSpeeds);
    }
    public void SetAll(
        UserUIInputsValues _values,
        UserUIInputsIsActive _isActives,
        bool _setParameters = true, 
        bool _setPositions = true, 
        bool _setSpeeds = true)
    {
        if (_setParameters) SetParameters(_values, _isActives);
        if (_setPositions) SetPositions(_values, _isActives);
        if (_setSpeeds) SetSpeeds(_values, _isActives);
    }
    public void SetAllFromUI(
        UserUIInputsIsActive _isActives,
        bool _setParameters = true, 
        bool _setPositions = true, 
        bool _setSpeeds = true
    ){
        UserUIInputsValues _values = new UserUIInputsValues();
        _values.SetAll(ToolBox.GetInstance().GetManager<UIManager>().userInputs);
        
        SetAll(_values, _isActives, _setParameters, _setPositions, _setSpeeds);
    }
    public void SetAllFromUI(
        bool _setParameters = true, 
        bool _setPositions = true, 
        bool _setSpeeds = true
    ){
        UserUIInputsIsActive _isActives = new UserUIInputsIsActive();
        _isActives.Initialize();
        
        SetAllFromUI(_isActives, _setParameters, _setPositions, _setSpeeds);
    }

    public void SetInput(InputField _field, float _value = 0, bool _activate = true)
    {
        if (_field == null) return;
        _field.enabled = _activate;
        _field.image.color = _activate ? Color.white : Color.blue;
        _field.text = Utils.ToString(_value);
    }

    public void SetInput(Toggle _field, bool _value = false, bool _activate = true)
    {
        if (_field == null) return;
        _field.enabled = _activate;
        _field.image.color = _activate ? Color.white : Color.blue;
        _field.isOn = _value;
    }
    public void SetInput(Dropdown _field, int _value = 0, bool _activate = true)
    {
        if (_field == null) return;
        _field.enabled = _activate;
        _field.image.color = _activate ? Color.white : Color.blue;
        _field.value = _value;
    }
}

[System.Serializable]
public class UserUIInputsValues
{
    public float Duration;

    public float Somersault;
    public float Tilt;
    public float Twist;
    public float HorizontalPosition;
    public float VerticalPosition;
    public float SomersaultSpeed;
    public float TiltSpeed;
    public float TwistSpeed;
    public float HorizontalSpeed;
    public float VerticalSpeed;

    public int PresetConditions;
    public bool UseGravity;
    public bool StopOnGround;

    public void SetAll(UserUIInputs _inputs)
    {
        Duration = _inputs.Duration != null ? Utils.ToFloat(_inputs.Duration.text) : 1;

        Somersault = _inputs.Somersault != null ? Utils.ToFloat(_inputs.Somersault.text) : 0;
        Tilt = _inputs.Tilt != null ? Utils.ToFloat(_inputs.Tilt.text) : 0;
        Twist = _inputs.Twist != null ? Utils.ToFloat(_inputs.Twist.text) : 0;
        HorizontalPosition = _inputs.HorizontalPosition != null ? Utils.ToFloat(_inputs.HorizontalPosition.text) : 0;
        VerticalPosition = _inputs.VerticalPosition != null ? Utils.ToFloat(_inputs.VerticalPosition.text) : 0;
        SomersaultSpeed = _inputs.SomersaultSpeed != null ? Utils.ToFloat(_inputs.SomersaultSpeed.text) : 0;
        TiltSpeed = _inputs.TiltSpeed != null ? Utils.ToFloat(_inputs.TiltSpeed.text) : 0;
        TwistSpeed = _inputs.TwistSpeed != null ? Utils.ToFloat(_inputs.TwistSpeed.text) : 0;
        HorizontalSpeed = _inputs.HorizontalSpeed != null ? Utils.ToFloat(_inputs.HorizontalSpeed.text) : 0;
        VerticalSpeed = _inputs.VerticalSpeed != null ? Utils.ToFloat(_inputs.VerticalSpeed.text) : 0;

        PresetConditions = _inputs.PresetConditions != null ? _inputs.PresetConditions.value : 0;
        UseGravity = _inputs.UseGravity != null ? _inputs.UseGravity.isOn : true;
        StopOnGround = _inputs.StopOnGround != null ? _inputs.StopOnGround.isOn : false;
    }
}

[System.Serializable]
public struct UserUIInputsIsActive
{
    public bool Duration;

    public bool Somersault;
    public bool Tilt;
    public bool Twist;
    public bool HorizontalPosition;
    public bool VerticalPosition;
    public bool SomersaultSpeed;
    public bool TiltSpeed;
    public bool TwistSpeed;
    public bool HorizontalSpeed;
    public bool VerticalSpeed;

    public bool UseGravity;
    public bool StopOnGround;

    public void Initialize(){
        // Initialise to current UI value
        var userInputs = ToolBox.GetInstance().GetManager<UIManager>().userInputs;

        Duration = userInputs.Duration.enabled;
        Somersault = userInputs.Somersault.enabled;
        Tilt = userInputs.Tilt.enabled;
        Twist = userInputs.Twist.enabled;
        HorizontalPosition = userInputs.HorizontalPosition.enabled;
        VerticalPosition = userInputs.VerticalPosition.enabled;
        SomersaultSpeed = userInputs.SomersaultSpeed.enabled;
        TiltSpeed = userInputs.TiltSpeed.enabled;
        TwistSpeed = userInputs.TwistSpeed.enabled;
        HorizontalSpeed = userInputs.HorizontalSpeed.enabled;
        VerticalSpeed = userInputs.VerticalSpeed.enabled;
        UseGravity = userInputs.UseGravity.enabled;
        StopOnGround = userInputs.StopOnGround.enabled;
    }
}

public class UIManager : MonoBehaviour
{
    protected DrawManager drawManager;
    protected GameManager gameManager;

    public GameObject panelToolTip { get; protected set; }
    RectTransform rectTransformBackgroundToolTip;
    Text textToolTip;
    public void SetPanelToolTip(GameObject _panel){
        panelToolTip = _panel.transform.Find("PanelToolTip").gameObject;
        rectTransformBackgroundToolTip = panelToolTip.transform.Find("background").GetComponent<RectTransform>();
        textToolTip = panelToolTip.transform.Find("text").GetComponent<Text>();

        panelToolTip.SetActive(false);
    }

    int displayToolTipNum = 0;

    public bool tooltipOn;

    public UserUIInputs userInputs { get; protected set; }
    protected UserUIInputsValues userInputsDefaultValues;
    public void SetUserInputs(UserUIInputs _userUIInputs, UserUIInputsValues _default)
    {
        userInputs = _userUIInputs;
        userInputsDefaultValues = _default;

        SetTooltip(PlayerPrefs.GetInt("WithToolTip", 0) == 1);        
        SetDropDownPresetCondition(PlayerPrefs.GetInt("PresetConditions", 0));
        UpdateAllPropertiesFromDropdown();
    }

    void Start()
    {
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
    }

    public void SetDropDownPresetCondition(int value){

        userInputs.SetPresetConditions(value);
    }

    public int currentTab = 1;

    public void SetCurrentTab(int _num)
    {
        // Do not allow changing tab if the model is currently being changed
        if (drawManager.IsEditing) return;  
        currentTab = _num;
    }

    public bool IsInParameterTab => currentTab == 1;
    public bool IsInEditingTab => currentTab == 2;
    public bool IsInResultTab => currentTab == 4;

    public int GetCurrentTab()
    {
        return currentTab;
    }

    public void ShowToolTip(int num, GameObject gameObject, string stringToolTip)
    {
        if (tooltipOn)
        {
            if (IsOnGameObject(gameObject))
            {
                panelToolTip.SetActive(true);
                panelToolTip.transform.SetAsLastSibling();

                textToolTip.text = stringToolTip;
                float paddingSize = 4;
                Vector2 backgroundSize = new Vector2(textToolTip.preferredWidth + paddingSize * 2, textToolTip.preferredHeight + paddingSize * 2);
                rectTransformBackgroundToolTip.sizeDelta = backgroundSize;

                Vector2 localPoint = Input.mousePosition;
                panelToolTip.transform.position = localPoint;

                displayToolTipNum = num;
            }
            else if (displayToolTipNum == num)
            {
                HideToolTip();
            }
        }
    }

    public void HideToolTip()
    {
        panelToolTip.SetActive(false);
        displayToolTipNum = 0;
    }

    public bool IsOnGameObject(GameObject gameObject)
    {
        if (gameObject != null)
        {
            Vector2 inputMousePos = Input.mousePosition;
            Vector3[] menuPos = new Vector3[4];
            gameObject.GetComponent<RectTransform>().GetWorldCorners(menuPos);
            Vector3[] gameObjectPos = new Vector3[2];
            gameObjectPos[0] = RectTransformUtility.WorldToScreenPoint(Camera.main, menuPos[0]);
            gameObjectPos[1] = RectTransformUtility.WorldToScreenPoint(Camera.main, menuPos[2]);

            return (gameObject.activeSelf && inputMousePos.x >= gameObjectPos[0].x && inputMousePos.x <= gameObjectPos[1].x && inputMousePos.y >= gameObjectPos[0].y && inputMousePos.y <= gameObjectPos[1].y);
        }
        else
            return false;
    }

    public void SetTooltip(bool _flag)
    {
        tooltipOn = _flag;
        PlayerPrefs.SetInt("WithToolTip", tooltipOn ? 1 : 0);
    }

    public void UpdateAllPropertiesFromDropdown(bool _skipSpeedColumn = false){
        if (userInputs == null || userInputs.PresetConditions == null) return;
        var values = gameManager.listCondition.conditions[userInputs.PresetConditions.value].userInputsValues;
        userInputs.SetAll(values, true, true, !_skipSpeedColumn);
    }

    public void ToggleGravity(){
        userInputs.SetUseGravity(userInputs.UseGravity.isOn);
    }

    public void ToggleStopOnGround()
    {
        userInputs.SetStopOnGround(userInputs.StopOnGround.isOn);
    }
}