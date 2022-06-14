using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlShin : MonoBehaviour
{
    [HideInInspector] public double dof;
    [HideInInspector] public int node = 0;

    private Vector3 mouseDistance;
    private Vector3 lastPosition;
    private GameObject girl;

//    private GameObject circlePrefab_shoulder;
//    private List<GameObject> circles = new List<GameObject>();

    private GameObject arrowPrefab;
    private GameObject arrow;

    private GameObject circlePrefab;
    private GameObject circle;

    public void Init(GameObject _girl, double _dof)
    {
        girl = _girl;
        dof = _dof;

        if (!arrow)
        {
            arrowPrefab = (GameObject)Resources.Load("Arrow", typeof(GameObject));
            arrow = Instantiate(arrowPrefab, gameObject.transform.position + new Vector3(0, 0, 0.1f), Quaternion.identity);
        }
        else
            arrow.SetActive(true);

        if (!circle)
        {
            circlePrefab = (GameObject)Resources.Load("NodeCircle", typeof(GameObject));
            circle = Instantiate(circlePrefab, gameObject.transform.position, Quaternion.identity);
            circle.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
        else
            circle.SetActive(true);

        /*        if (!circlePrefab_shoulder)
                {
                    circlePrefab_shoulder = (GameObject)Resources.Load("HandleCircle_shoulder", typeof(GameObject));

                    for (int i = 0; i < 32; i++)
                    {
                        float angle = i * Mathf.PI * 2f / 32;
                        Vector3 newPos = new Vector3(Mathf.Cos(angle) * 0.2f, 0, Mathf.Sin(angle) * 0.2f);
                        circles.Add(Instantiate(circlePrefab_shoulder, gameObject.transform.position + newPos, Quaternion.identity));
                        circles[i].transform.parent = gameObject.transform;
                    }
                }
                else
                {
                    for (int i = 0; i < circles.Count; i++)
                        circles[i].SetActive(true);
                }*/
    }

    public void DestroyCircle()
    {
        circle.SetActive(false);
        arrow.SetActive(false);
//        for (int i = 0; i < circles.Count; i++)
//            circles[i].SetActive(false);
    }

    private void Update()
    {
        if (arrow)
            arrow.transform.position = gameObject.transform.position + new Vector3(0, 0, 0.1f);

        if (circle)
            circle.transform.position = gameObject.transform.position;

    }

    void OnMouseDown()
    {
        if(ToolBox.GetInstance().GetManager<DrawManager>().isEditing)
        {
            lastPosition = Input.mousePosition;
            mouseDistance.x = (float)dof * 30f;
            ToolBox.GetInstance().GetManager<StatManager>().dofName = "KneeFlexion";
        }
    }

    void OnMouseDrag()
    {
        if (ToolBox.GetInstance().GetManager<DrawManager>().isEditing)
        {
            Vector3 newPosition = Input.mousePosition;
            mouseDistance += newPosition - lastPosition;

            HandleDof(mouseDistance.x);
            lastPosition = newPosition;
        }
    }

    void OnMouseUp()
    {
        if (ToolBox.GetInstance().GetManager<DrawManager>().isEditing)
        {
            mouseDistance = Vector3.zero;
            lastPosition = Vector3.zero;
        }
    }

    void HandleDof(float _value)
    {
        transform.rotation = Quaternion.Euler(0, -_value, 0);

//        if (girl.transform.forward.x >= 0)
//            dof = _value / 30;
//        else
            dof = -_value / 30;

        MainParameters.Instance.joints.nodes[1].Q[node] = (float)dof;
        ToolBox.GetInstance().GetManager<GameManager>().InterpolationDDL();
        ToolBox.GetInstance().GetManager<GameManager>().DisplayDDL(1, true);

        ToolBox.GetInstance().GetManager<DrawManager>().ControlShin((float)dof);
    }
}
