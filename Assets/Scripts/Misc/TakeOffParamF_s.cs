using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class TakeOffParamF_s : MonoBehaviour
{
    protected GameManager gameManager;
    protected DrawManager drawManager;

    public BaseProfile bp;

    void OnEnable()
    {
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
    }

    public void DropDownDDLNamesOnValueChanged(int value)
    {
        MainParameters.Instance.joints.condition = value;

        UpdatePositions(value);
    }

    public void UpdatePositions(int value)
    {
        drawManager.SetGravity(gameManager.listCondition.conditions[value].userInputsValues.Gravity);
        MainParameters.Instance.joints.takeOffParam.rotation = Utils.ToFloat(gameManager.listCondition.conditions[value].userInputsValues.Somersault);
        MainParameters.Instance.joints.takeOffParam.tilt = Utils.ToFloat(gameManager.listCondition.conditions[value].userInputsValues.Tilt);
        drawManager.takeOffParamTwistPosition = Utils.ToFloat(gameManager.listCondition.conditions[value].userInputsValues.Twist);
        drawManager.takeOffParamHorizontalPosition = Utils.ToFloat(gameManager.listCondition.conditions[value].userInputsValues.HorizontalPosition);
        drawManager.takeOffParamVerticalPosition = Utils.ToFloat(gameManager.listCondition.conditions[value].userInputsValues.VerticalPosition);

        ApplyAvatar();
    }

    public void CheckTakeOffParam(GameObject panel)
    {
        float value = Utils.ToFloat(panel.GetComponentInChildren<InputField>().text);

        if (panel.name == "PanelSomersaultPosition")
            MainParameters.Instance.joints.takeOffParam.rotation = value;
        else if (panel.name == "PanelTiltPosition")
            MainParameters.Instance.joints.takeOffParam.tilt = value;
        else if (panel.name == "PanelTwistPosition")
            drawManager.takeOffParamTwistPosition = value;
        else if (panel.name == "PanelHorizontalPosition")
            drawManager.takeOffParamHorizontalPosition = value;
        else if (panel.name == "PanelVerticalPosition")
            drawManager.takeOffParamVerticalPosition = value;
        else if (panel.name == "PanelHorizontalSpeed")
            MainParameters.Instance.joints.takeOffParam.anteroposteriorSpeed = value;
        else if (panel.name == "PanelVerticalSpeed")
            MainParameters.Instance.joints.takeOffParam.verticalSpeed = value;
        else if (panel.name == "PanelSomersaultSpeed")
            MainParameters.Instance.joints.takeOffParam.somersaultSpeed = value;
        else if (panel.name == "PanelTwistSpeed")
            MainParameters.Instance.joints.takeOffParam.twistSpeed = value;
        else if (panel.name == "PanelTiltSpeed")
            drawManager.takeOffParamTiltSpeed = value;
        else if(panel.name == "PanelTimeDuration")
            MainParameters.Instance.joints.duration = value;

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
