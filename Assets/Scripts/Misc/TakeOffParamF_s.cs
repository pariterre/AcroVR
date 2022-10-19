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
        gameManager.SetSelectedPresetCondition(value);

        int _avatarIndex = 0;
        UpdatePositions(_avatarIndex, value);
    }

    public void UpdatePositions(int _avatarIndex, int value)
    {
        uiManager.userInputs.SetPositions(gameManager.PresetConditions.conditions[value].userInputsValues);

        ApplyAvatar(_avatarIndex);
    }

    public void CheckTakeOffParam()
    {
        int _avatarIndex = 0;
        uiManager.userInputs.SetAllFromUI();    
        ApplyAvatar(_avatarIndex);
    }

    private void ApplyAvatar(int _avatarIndex)
    {
        drawManager.ShowAvatar(_avatarIndex);
        drawManager.InitPoseAvatar(_avatarIndex);
        gameManager.InterpolationDDL(_avatarIndex);
        bp.FrontCameraPOV(drawManager.CheckPositionAvatar(_avatarIndex));
    }
}
