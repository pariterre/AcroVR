using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class SelectStudent : MonoBehaviour
{
    public PlayableDirector mission1;
    Dropdown m_Dropdown;
    public Text m_Text;

    void Start()
    {
        ToolBox.GetInstance().GetManager<DrawManager>().SetAnimationSpeed(3);

        m_Dropdown = GetComponent<Dropdown>();
        m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(m_Dropdown);
        });

        mission1.playableGraph.GetRootPlayable(0).SetSpeed(0);
    }

    void DropdownValueChanged(Dropdown change)
    {
        switch(change.value)
        {
            case 0:
                ToolBox.GetInstance().GetManager<StatManager>().ProfileLoad("Student0.json");
                break;
            case 1:
                ToolBox.GetInstance().GetManager<StatManager>().ProfileLoad("Student1.json");
                break;
            case 2:
                ToolBox.GetInstance().GetManager<StatManager>().ProfileLoad("Student2.json");
                break;
        }

        m_Text.text = ToolBox.GetInstance().GetManager<StatManager>().info.Name;

        mission1.playableGraph.GetRootPlayable(0).SetSpeed(1d);
    }
}
