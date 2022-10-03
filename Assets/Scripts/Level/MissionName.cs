using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionName : MonoBehaviour
{
    public Text[] nMission;
    public Text[] hMission;

    public GameObject[] MissionMenu;

    protected MissionManager missionManager;

    void Start()
    {
        missionManager = ToolBox.GetInstance().GetManager<MissionManager>();
    }

    public void SetMissionName(int _level)
    {
        int n = 0;
        for (int i = 0; i < 6; i++)
            MissionMenu[i].SetActive(true);

        for (int i =0; i < missionManager.AllMissions.count; i++)
        {
            if (missionManager.AllMissions.missions[i].Level == _level)
            {
                nMission[n].text = missionManager.AllMissions.missions[i].Name;
                hMission[n].text = missionManager.AllMissions.missions[i].Name;
                n++;
            }
        }

        for(int i = 0; i < 6-n; i++)
        {
            nMission[i + n].text = "";
            MissionMenu[i + n].SetActive(false);
        }

        missionManager.SetLevel(_level);
    }

    public void SetMission(int num)
    {
        missionManager.SetSubLevel(num);
    }
}