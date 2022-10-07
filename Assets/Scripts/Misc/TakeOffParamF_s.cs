using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class TakeOffParamF_s : MonoBehaviour
{
    protected GameManager gameManager;
    protected DrawManager drawManager;
    protected UIManager uiManager;

    public BaseProfile bp;

    void OnEnable()
    {
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();
    }

    public void DropDownDDLNamesOnValueChanged(int value)
    {
        MainParameters.Instance.joints.condition = value;

        UpdatePositions(value);
    }

    public void UpdatePositions(int value)
    {
        uiManager.userInputs.SetPositions(gameManager.listCondition.conditions[value].userInputsValues);

        ApplyAvatar();
    }

    public void CheckTakeOffParam(GameObject panel)
    {
        float value = Utils.ToFloat(panel.GetComponentInChildren<InputField>().text);
        // TODO: Just update everything from the InputFields...
        if(panel.name == "PanelTimeDuration")
            MainParameters.Instance.joints.Duration = value;
        else if (panel.name == "PanelSomersaultPosition")
            MainParameters.Instance.joints.takeOffParam.Somersault = value;
        else if (panel.name == "PanelTiltPosition")
            MainParameters.Instance.joints.takeOffParam.Tilt = value;
        else if (panel.name == "PanelTwistPosition")
            MainParameters.Instance.joints.takeOffParam.Twist = value;
        else if (panel.name == "PanelHorizontalPosition")
            MainParameters.Instance.joints.takeOffParam.HorizontalPosition = value;
        else if (panel.name == "PanelVerticalPosition")
            MainParameters.Instance.joints.takeOffParam.VerticalPosition = value;
        else if (panel.name == "PanelSomersaultSpeed")
            MainParameters.Instance.joints.takeOffParam.SomersaultSpeed = value;
        else if (panel.name == "PanelTiltSpeed")
            MainParameters.Instance.joints.takeOffParam.TiltSpeed = value;
        else if (panel.name == "PanelTwistSpeed")
            MainParameters.Instance.joints.takeOffParam.TwistSpeed = value;
        else if (panel.name == "PanelHorizontalSpeed")
            MainParameters.Instance.joints.takeOffParam.HorizontalSpeed = value;
        else if (panel.name == "PanelVerticalSpeed")
            MainParameters.Instance.joints.takeOffParam.VerticalSpeed = value;

        ApplyAvatar();
    }

    private void ApplyAvatar()
    {
        drawManager.ShowAvatar();
        drawManager.InitPoseAvatar();
        gameManager.InterpolationDDL();
        bp.FrontCameraPOV(drawManager.CheckPositionAvatar());
    }
}
