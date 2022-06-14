using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMovement : MonoBehaviour
{

    //    GameObject player;
    public GameObject tPlayer;
    private CinemachineVirtualCamera vcam;

    void Start()
    {
        //        player = GameObject.FindGameObjectWithTag("Player");
        //        pos = player.transform.Find("FirstViewPoint").transform.position;

        //        player = ToolBox.GetInstance().GetManager<DrawManager>().girl1Hip;

        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    void Update () {

        if (tPlayer == null)
        {
            tPlayer = GameObject.FindWithTag("Player").transform.Find("Petra.002/hips").gameObject;
        }
        else
            vcam.LookAt = tPlayer.transform;

        /*        if(ToolBox.GetInstance().GetManager<DrawManager>().girl1Hip != null)
                {
                    if(ToolBox.GetInstance().GetManager<DrawManager>().girl1Hip.transform.position.z > 1.0f)
                    {
                        transform.position = new Vector3(0, 3, 17f);
                    }
        //            transform.LookAt(ToolBox.GetInstance().GetManager<DrawManager>().girl1Hip.transform);
        //            transform.Translate(new Vector3(0,0,player.transform.position.y));
                }*/

        /*        if (Input.GetMouseButtonDown(2))
                {
                    dragOrigin = Input.mousePosition;
                    return;
                }

                if (!Input.GetMouseButton(2)) return;

                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
                Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);

                transform.Translate(move, Space.World);*/
    }
}
