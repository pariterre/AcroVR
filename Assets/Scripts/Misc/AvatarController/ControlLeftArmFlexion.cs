using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlLeftArmFlexion : MonoBehaviour
{
    [HideInInspector] public double dof;
    [HideInInspector] public int node = 0;

    private Vector3 mouseDistance;
    private Vector3 lastPosition;
    private GameObject girl;

    public bool bActive = false;

    private GameObject arrowPrefab;
    private GameObject arrow;

    private GameObject circlePrefab;
    private GameObject circle;

    //    private GameObject circlePrefab_shoulder;
    //    private List<GameObject> circles = new List<GameObject>();

    public void Init(GameObject _girl, double _dof)
    {
        girl = _girl;
        dof = _dof;

        if (!arrow)
        {
            arrowPrefab = (GameObject)Resources.Load("Arrow", typeof(GameObject));
            arrow = Instantiate(arrowPrefab, gameObject.transform.position + new Vector3(0, 0, 0.1f), Quaternion.identity);
            arrow.transform.rotation = Quaternion.Euler(0, 0, 90);
            arrow.transform.localScale = new Vector3(0.3f, 0.2f, 0.1f);
        }
        else
            arrow.SetActive(true);

        if (!circle)
        {
            circlePrefab = (GameObject)Resources.Load("NodeCircle", typeof(GameObject));
            circle = Instantiate(circlePrefab, gameObject.transform.position, Quaternion.identity);
            circle.transform.rotation = Quaternion.Euler(0, 90, 90);
        }
        else
            circle.SetActive(true);

        /*        circlePrefab_shoulder = (GameObject)Resources.Load("HandleCircle_shoulder", typeof(GameObject));

                for (int i = 0; i < 32; i++)
                {
                    float angle = i * Mathf.PI * 2f / 32;
                    Vector3 newPos = new Vector3(Mathf.Cos(angle) * 0.1f, 0, Mathf.Sin(angle) * 0.1f);
                    circles.Add(Instantiate(circlePrefab_shoulder, gameObject.transform.position + newPos, Quaternion.identity));
        //            circles[i].transform.parent = gameObject.transform;
                }*/
    }

    public void DestroyCircle()
    {
        circle.SetActive(false);
        arrow.SetActive(false);
        //        for (int i = 0; i < circles.Count; i++)
        //            Destroy(circles[i].gameObject);
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
        if (!bActive) return;

        lastPosition = Input.mousePosition;
        mouseDistance.y = (float)dof * 30f;
        ToolBox.GetInstance().GetManager<StatManager>().dofName = "LeftArmFlexion";
    }

    void OnMouseDrag()
    {
        if (!bActive) return;

        Vector3 newPosition = Input.mousePosition;
        mouseDistance += newPosition - lastPosition;

        HandleDof(mouseDistance.y);

        lastPosition = newPosition;
    }

    void OnMouseUp()
    {
        if (!bActive) return;

        mouseDistance = Vector3.zero;
    }

    void HandleDof(float _value)
    {
        transform.rotation = Quaternion.Euler(0,_value, 0);
        dof = _value / 30;

        MainParameters.Instance.joints.nodes[4].Q[node] = (float)dof;
        ToolBox.GetInstance().GetManager<GameManager>().InterpolationDDL();
        ToolBox.GetInstance().GetManager<GameManager>().DisplayDDL(4, true);
        ToolBox.GetInstance().GetManager<DrawManager>().ControlLeftArmFlexion((float)dof);
    }
}
