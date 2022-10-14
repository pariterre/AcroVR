using UnityEngine;

public class TakeOffParamF_s : MonoBehaviour
{
    protected DrawManager drawManager;
    protected GameManager gameManager;
    protected UIManager uiManager;

    public BaseProfile bp;

    void Start()
    {
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();

        CheckTakeOffParam();
    }

    public void DropDownDDLNamesOnValueChanged(int value)
    {
        drawManager.SetPresetCondition(value);

        UpdatePositions(value);
    }

    public void UpdatePositions(int value)
    {
        uiManager.userInputs.SetPositions(gameManager.listCondition.conditions[value].userInputsValues);

        ApplyAvatar();
    }

    public void CheckTakeOffParam()
    {
        uiManager.userInputs.SetAllFromUI();    
        ApplyAvatar();
    }

    private void ApplyAvatar()
    {
        drawManager.ShowAvatar(0);
        drawManager.InitPoseAvatar();
        gameManager.InterpolationDDL();
        bp.FrontCameraPOV(drawManager.CheckPositionAvatar());
    }
}
