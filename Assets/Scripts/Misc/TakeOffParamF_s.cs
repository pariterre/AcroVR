using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class TakeOffParamF_s : MonoBehaviour
{
    public BaseProfile bp;

/*    void OnEnable()
    {
        float value = MainParameters.Instance.joints.takeOffParam.rotation;
        transform.Find("PanelParameters/PanelParametersColumn1/PanelSomersaultPosition").gameObject.GetComponentInChildren<InputField>().text = string.Format("{0:0.0}", value);
        value = MainParameters.Instance.joints.takeOffParam.tilt;
        transform.Find("PanelParameters/PanelParametersColumn1/PanelTilt").gameObject.GetComponentInChildren<InputField>().text = string.Format("{0:0.0}", value);
        value = MainParameters.Instance.joints.takeOffParam.anteroposteriorSpeed;
        transform.Find("PanelParameters/PanelParametersColumn2/PanelHorizontalSpeed").gameObject.GetComponentInChildren<InputField>().text = string.Format("{0:0.0}", value);
        value = MainParameters.Instance.joints.takeOffParam.verticalSpeed;
        transform.Find("PanelParameters/PanelParametersColumn2/PanelVerticalSpeed").gameObject.GetComponentInChildren<InputField>().text = string.Format("{0:0.0}", value);
        value = MainParameters.Instance.joints.takeOffParam.somersaultSpeed;
        transform.Find("PanelParameters/PanelParametersColumn2/PanelSomersaultSpeed").gameObject.GetComponentInChildren<InputField>().text = string.Format("{0:0.000}", value);
        value = MainParameters.Instance.joints.takeOffParam.twistSpeed;
        transform.Find("PanelParameters/PanelParametersColumn2/PanelTwistSpeed").gameObject.GetComponentInChildren<InputField>().text = string.Format("{0:0.000}", value);
    }*/

    public void DropDownDDLNamesOnValueChanged(int value)
    {
//        ToolBox.GetInstance().GetManager<GameManager>().InterpolationDDL();
//        ToolBox.GetInstance().GetManager<GameManager>().DisplayDDL(value, true);

        MainParameters.Instance.joints.condition = value;

        UpdatePositions(value);
    }

    public void UpdatePositions(int value)
    {
        if (!ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[value].Gravity)
            MainParameters.Instance.joints.condition = 0;

        MainParameters.Instance.joints.takeOffParam.rotation = ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[value].SomersaultPosition;
        ToolBox.GetInstance().GetManager<DrawManager>().takeOffParamTwistPosition = ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[value].TwistPosition;
        MainParameters.Instance.joints.takeOffParam.tilt = ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[value].TiltPosition;
        ToolBox.GetInstance().GetManager<DrawManager>().takeOffParamHorizontalPosition = ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[value].HorizontalPosition;
        ToolBox.GetInstance().GetManager<DrawManager>().takeOffParamVerticalPosition = ToolBox.GetInstance().GetManager<GameManager>().listCondition.conditions[value].VerticalPosition;

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
            ToolBox.GetInstance().GetManager<DrawManager>().takeOffParamTwistPosition = value;
            ApplyAvatar();
        }
        else if (panel.name == "PanelHorizontalPosition")
        {
            ToolBox.GetInstance().GetManager<DrawManager>().takeOffParamHorizontalPosition = value;
            ApplyAvatar();
        }
        else if (panel.name == "PanelVerticalPosition")
        {
            ToolBox.GetInstance().GetManager<DrawManager>().takeOffParamVerticalPosition = value;
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
            ToolBox.GetInstance().GetManager<DrawManager>().takeOffParamTiltSpeed = value;
        }
        else if(panel.name == "PanelTimeDuration")
        {
            MainParameters.Instance.joints.duration = value;
        }
    }

    private void ApplyAvatar()
    {
//        ToolBox.GetInstance().GetManager<DrawManager>().GestureMode();
        ToolBox.GetInstance().GetManager<DrawManager>().MakeSimulationFrame();
        ToolBox.GetInstance().GetManager<DrawManager>().ShowAvatar();
        ToolBox.GetInstance().GetManager<DrawManager>().InitPoseAvatar();

        //        ToolBox.GetInstance().GetManager<DrawManager>().PlayAvatar();

        /*        if (ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar())
                    bp.CameraPOV(BaseProfile.POV.LongFrontView);
                else
                    bp.CameraPOV(BaseProfile.POV.FrontView);*/

        bp.FrontCameraPOV(ToolBox.GetInstance().GetManager<DrawManager>().CheckPositionAvatar());
    }
}
