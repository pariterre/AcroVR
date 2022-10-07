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

    public InputField Somersault;
    public InputField Tilt;
    public InputField Twist;
    public InputField HorizontalPosition;
    public InputField VerticalPosition;
    public InputField SomersaultSpeed;
    public InputField TiltSpeed;
    public InputField TwistSpeed;
    public InputField HorizontalSpeed;
    public InputField VerticalSpeed;

    public Dropdown PresetConditions;
    public Toggle Gravity;
    public Toggle StopOnGround;

    public void SetPositions(UserUIInputsValues _values)
    {
        SetInput(Somersault, true, _values.Somersault);
        SetInput(Tilt, true, _values.Tilt);
        SetInput(Twist, true, _values.Twist);
        SetInput(HorizontalPosition, true, _values.HorizontalPosition);
        SetInput(VerticalPosition, true, _values.VerticalPosition);

        // drawManager.SetGravity(gameManager.listCondition.conditions[value].userInputsValues.Gravity);
        // MainParameters.Instance.joints.takeOffParam.rotation = Utils.ToFloat(gameManager.listCondition.conditions[value].userInputsValues.Somersault);
        // MainParameters.Instance.joints.takeOffParam.tilt = Utils.ToFloat(gameManager.listCondition.conditions[value].userInputsValues.Tilt);
        // drawManager.takeOffParamTwistPosition = Utils.ToFloat(gameManager.listCondition.conditions[value].userInputsValues.Twist);
        // drawManager.takeOffParamHorizontalPosition = Utils.ToFloat(gameManager.listCondition.conditions[value].userInputsValues.HorizontalPosition);
        // drawManager.takeOffParamVerticalPosition = Utils.ToFloat(gameManager.listCondition.conditions[value].userInputsValues.VerticalPosition);
    }

    public void SetAll(UserUIInputsValues _values)
    {
        SetInput(Duration, true, _values.Duration);

        SetInput(Somersault, true, _values.Somersault);
        SetInput(Tilt, true, _values.Tilt);
        SetInput(Twist, true, _values.Twist);
        SetInput(HorizontalPosition, true, _values.HorizontalPosition);
        SetInput(VerticalPosition, true, _values.VerticalPosition);
        SetInput(SomersaultSpeed, true, _values.SomersaultSpeed);
        SetInput(TiltSpeed, true, _values.TiltSpeed);
        SetInput(TwistSpeed, true, _values.TwistSpeed);
        SetInput(HorizontalSpeed, true, _values.HorizontalSpeed);
        SetInput(VerticalSpeed, true, _values.VerticalSpeed);
    }

    public void SetAll(UserUIInputsIsActive _statuses, UserUIInputsValues _values)
    {

        SetInput(Duration, _statuses.Duration, _values.Duration);

        SetInput(Somersault, _statuses.Somersault, _values.Somersault);
        SetInput(Tilt, _statuses.Tilt, _values.Tilt);
        SetInput(Twist, _statuses.Twist, _values.Twist);
        SetInput(HorizontalPosition, _statuses.HorizontalPosition, _values.HorizontalPosition);
        SetInput(VerticalPosition, _statuses.VerticalPosition, _values.VerticalPosition);
        SetInput(SomersaultSpeed, _statuses.SomersaultSpeed, _values.SomersaultSpeed);
        SetInput(TiltSpeed, _statuses.TiltSpeed, _values.TiltSpeed);
        SetInput(TwistSpeed, _statuses.TwistSpeed, _values.TwistSpeed);
        SetInput(HorizontalSpeed, _statuses.HorizontalSpeed, _values.HorizontalSpeed);
        SetInput(VerticalSpeed, _statuses.VerticalSpeed, _values.VerticalSpeed);
    }

    public void SetInput(InputField _field, bool _activate, string _value = "0.0")
    {
        _field.enabled = _activate;
        _field.image.color = _activate ? Color.white : Color.blue;
        _field.text = _value;
    }

    public void SetInput(Toggle _field, bool _activate, bool _value = false)
    {
        _field.enabled = _activate;
        _field.image.color = _activate ? Color.white : Color.blue;
        _field.isOn = _value;
    }
    public void SetInput(Dropdown _field, bool _activate, int _value = 0)
    {
        _field.enabled = _activate;
        _field.image.color = _activate ? Color.white : Color.blue;
        _field.value = _value;
    }
}

[System.Serializable]
public class UserUIInputsValues
{
    public string Duration;

    public string Somersault;
    public string Tilt;
    public string Twist;
    public string HorizontalPosition;
    public string VerticalPosition;
    public string SomersaultSpeed;
    public string TiltSpeed;
    public string TwistSpeed;
    public string HorizontalSpeed;
    public string VerticalSpeed;

    public int PresetConditions;
    public bool Gravity;
    public bool StopOnGround;

    public void SetAll(UserUIInputs _inputs)
    {
        Duration = _inputs.Duration != null ? _inputs.Duration.text : "1.0";

        Somersault = _inputs.Somersault != null ? _inputs.Somersault.text : "0.0";
        Tilt = _inputs.Tilt != null ? _inputs.Tilt.text : "0.0";
        Twist = _inputs.Twist != null ? _inputs.Twist.text : "0.0";
        HorizontalPosition = _inputs.HorizontalPosition != null ? _inputs.HorizontalPosition.text : "0.0";
        VerticalPosition = _inputs.VerticalPosition != null ? _inputs.VerticalPosition.text : "0.0";
        SomersaultSpeed = _inputs.SomersaultSpeed != null ? _inputs.SomersaultSpeed.text : "0.0";
        TiltSpeed = _inputs.TiltSpeed != null ? _inputs.TiltSpeed.text : "0.0";
        TwistSpeed = _inputs.TwistSpeed != null ? _inputs.TwistSpeed.text : "0.0";
        HorizontalSpeed = _inputs.HorizontalSpeed != null ? _inputs.HorizontalSpeed.text : "0.0";
        VerticalSpeed = _inputs.VerticalSpeed != null ? _inputs.VerticalSpeed.text : "0.0";

        PresetConditions = _inputs.PresetConditions != null ? _inputs.PresetConditions.value : 0;
        Gravity = _inputs.Gravity != null ? _inputs.Gravity.isOn : true;
        StopOnGround = _inputs.StopOnGround != null ? _inputs.StopOnGround.isOn : false;
    }
}

[System.Serializable]
public struct UserUIInputsIsActive
{
    public bool Somersault;
    public bool SomersaultSpeed;
    public bool Tilt;
    public bool TiltSpeed;
    public bool Twist;
    public bool TwistSpeed;
    public bool HorizontalPosition;
    public bool HorizontalSpeed;
    public bool VerticalPosition;
    public bool VerticalSpeed;
    public bool Duration;
}

public class UIManager : MonoBehaviour
{
    DrawManager drawManager;

    public GameObject panelToolTip;
    GameObject panelToolTipPrefab;
    RectTransform rectTransformBackground;
    Text textToolTip;
    int displayToolTipNum = 0;

    public bool tooltipOn;


    public UserUIInputs userInputs { get; protected set; }
    protected UserUIInputsValues userInputsDefaultValues;
    public void SetUserInputs(UserUIInputs _userUIInputs, UserUIInputsValues _default)
    {
        userInputs = _userUIInputs;
        userInputsDefaultValues = _default;
    }

    void Start()
    {
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        SetTooltip(PlayerPrefs.GetInt("WithToolTip", 0) == 1);
    }

    public void SetTooltip()
    {
        panelToolTipPrefab = (GameObject)Resources.Load("ToolTipCanvas", typeof(GameObject));
        panelToolTipPrefab = Instantiate(panelToolTipPrefab);
        panelToolTip = panelToolTipPrefab.transform.Find("PanelToolTip").gameObject;
        rectTransformBackground = panelToolTip.transform.Find("background").GetComponent<RectTransform>();
        textToolTip = panelToolTip.transform.Find("text").GetComponent<Text>();

        panelToolTip.SetActive(false);
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
                rectTransformBackground.sizeDelta = backgroundSize;

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
}