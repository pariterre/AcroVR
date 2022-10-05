using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionName : MonoBehaviour
{
    public Text[] nMission;
    public Text[] hMission;

    public GameObject[] MissionMenu;
    public Image[] SuccessStar;

    protected MissionManager missionManager;
    

    void Start()
    {
        missionManager = ToolBox.GetInstance().GetManager<MissionManager>();
    }

    public void SetMissionName(int _level)
    {
        int n = 0;
        for (int i = 0; i < 6; i++){
            nMission[i].text = "";
            MissionMenu[i].SetActive(false);
        }

        for (int i=0; i < missionManager.AllMissions.count; i++)
        {
            var _mission = missionManager.AllMissions.missions[i];
            var _isCompleted = missionManager.MissionCompleted(i);
            if (_mission.Level == _level)
            {
                nMission[n].text = _mission.Name;
                hMission[n].text = _mission.Name;
                MissionMenu[n].SetActive(true);
                SuccessStar[n].gameObject.SetActive(_isCompleted);
                n++;
            }
        }


        missionManager.SetLevel(_level);
    }

    public void SetMission(int num)
    {
        missionManager.SetSubLevel(num);
    }
}