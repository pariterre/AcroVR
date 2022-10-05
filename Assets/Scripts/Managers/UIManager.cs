using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///		UI In-Game menu logic
/// </summary>

public class UIManager : MonoBehaviour
{
    DrawManager drawManager;

    public GameObject panelToolTip;
    GameObject panelToolTipPrefab;
    RectTransform rectTransformBackground;
    Text textToolTip;
    int displayToolTipNum = 0;

    public bool tooltipOn;

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