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
        uiManager.userInputs.SetAllFromUI();    
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
