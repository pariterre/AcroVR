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
        if (!gameManager.listCondition.conditions[value].Gravity)
            MainParameters.Instance.joints.condition = 0;

        MainParameters.Instance.joints.takeOffParam.rotation = gameManager.listCondition.conditions[value].SomersaultPosition;
        drawManager.takeOffParamTwistPosition = gameManager.listCondition.conditions[value].TwistPosition;
        MainParameters.Instance.joints.takeOffParam.tilt = gameManager.listCondition.conditions[value].TiltPosition;
        drawManager.takeOffParamHorizontalPosition = gameManager.listCondition.conditions[value].HorizontalPosition;
        drawManager.takeOffParamVerticalPosition = gameManager.listCondition.conditions[value].VerticalPosition;

        ApplyAvatar();
    }

    public void CheckTakeOffParamCondition(Dropdown dropDown)
    {
        MainParameters.Instance.joints.condition = dropDown.value;
    }   

    public void CheckTakeOffParam(GameObject panel)
    {
        float value = float.Parse(panel.GetComponentInChildren<InputField>().text, NumberStyles.Number, CultureInfo.InvariantCulture);

        if (panel.name == "PanelSomersaultPosition")
        {
            MainParameters.Instance.joints.takeOffParam.rotation = value;
            ApplyAvatar();
        }
        else if (panel.name == "PanelTiltPosition")
        {
            MainParameters.Instance.joints.takeOffParam.tilt = value;
            ApplyAvatar();
        }
        else if (panel.name == "PanelTwistPosition")
        {
            drawManager.takeOffParamTwistPosition = value;
            ApplyAvatar();
        }
        else if (panel.name == "PanelHorizontalPosition")
        {
            drawManager.takeOffParamHorizontalPosition = value;
            ApplyAvatar();
        }
        else if (panel.name == "PanelVerticalPosition")
        {
            drawManager.takeOffParamVerticalPosition = value;
            ApplyAvatar();
        }
        else if (panel.name == "PanelHorizontalSpeed")
        {
            MainParameters.Instance.joints.takeOffParam.anteroposteriorSpeed = value;
        }
        else if (panel.name == "PanelVerticalSpeed")
        {
            MainParameters.Instance.joints.takeOffParam.verticalSpeed = value;
        }
        else if (panel.name == "PanelSomersaultSpeed")
        {
            MainParameters.Instance.joints.takeOffParam.somersaultSpeed = value;
        }
        else if (panel.name == "PanelTwistSpeed")
        {
            MainParameters.Instance.joints.takeOffParam.twistSpeed = value;
        }
        else if (panel.name == "PanelTiltSpeed")
        {
            drawManager.takeOffParamTiltSpeed = value;
        }
        else if(panel.name == "PanelTimeDuration")
        {
            MainParameters.Instance.joints.duration = value;
        }
    }

    private void ApplyAvatar()
    {
//        drawManager.GestureMode();
        drawManager.ShowAvatar(true);
        drawManager.InitPoseAvatar();

        //        drawManager.PlayAvatar();

        /*        if (drawManager.CheckPositionAvatar())
                    bp.CameraPOV(BaseProfile.POV.LongFrontView);
                else
                    bp.CameraPOV(BaseProfile.POV.FrontView);*/

        bp.FrontCameraPOV(drawManager.CheckPositionAvatar());
    }
}
