using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionLevel : MonoBehaviour
{
    public Image[] SuccessStar;

    public void ActivateSuccessStar(){
        MissionManager _missionManager = ToolBox.GetInstance().GetManager<MissionManager>();
        for (int i=0; i<7; ++i){
            SuccessStar[i].gameObject.SetActive(_missionManager.IsLevelCompleted(i));
        }
    }

}
