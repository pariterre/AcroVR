using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionName : MonoBehaviour
{
    public Text[] nMission;
    public Text[] hMission;

    public GameObject[] MissionMenu;

    public void SetMissionName(int _level)
    {
        int n = 0;
        for (int i = 0; i < 6; i++)
            MissionMenu[i].SetActive(true);

        for (int i =0; i < ToolBox.GetInstance().GetManager<GameManager>().listMission.count; i++)
        {
            if (ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[i].Level == _level)
            {
                nMission[n].text = ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[i].Name;
                hMission[n].text = ToolBox.GetInstance().GetManager<GameManager>().listMission.missions[i].Name;
                n++;
            }
        }

        for(int i = 0; i < 6-n; i++)
        {
            nMission[i + n].text = "";
            MissionMenu[i + n].SetActive(false);
        }

        ToolBox.GetInstance().GetManager<GameManager>().numLevel = _level;
    }

    public void SetMission(int num)
    {
        ToolBox.GetInstance().GetManager<GameManager>().SetNumberOfMissions(num);
    }
}