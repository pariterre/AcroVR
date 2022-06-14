using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class SelectMission : MonoBehaviour
{
    public PlayableDirector mission1;
    Dropdown m_Dropdown;
    public Text m_Text;

    void Start()
    {
        m_Dropdown = GetComponent<Dropdown>();
        m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(m_Dropdown);
        });

        mission1.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }

    void DropdownValueChanged(Dropdown change)
    {
        switch (change.value)
        {
            case 0:
                ToolBox.GetInstance().GetManager<GameManager>().ReadDataFromJSON("Mission0.json");
                break;
            case 1:
                ToolBox.GetInstance().GetManager<GameManager>().ReadDataFromJSON("Mission1.json");
                break;
            case 2:
                ToolBox.GetInstance().GetManager<GameManager>().ReadDataFromJSON("Mission2.json");
                break;
        }

        m_Text.text = ToolBox.GetInstance().GetManager<GameManager>().mission.Name;

        mission1.playableGraph.GetRootPlayable(0).SetSpeed(1d);
    }
}
