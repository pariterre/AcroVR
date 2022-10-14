using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChartAndGraph;
#if UNITY_EDITOR 
using UnityEditor;
#endif
//using System.Drawing;
//using System.Drawing.Imaging;

public class AniGraphManager : MonoBehaviour
{
    protected AvatarManager avatarManager;
    protected DrawManager drawManager;
    protected GameManager gameManager;
    protected UIManager uiManager;

    public GraphChart graph;
    GameObject takeoffPrefab;
    public GameObject takeoffCanvas;
    public bool bDraw = false;

    string[] dataCategories;
    string[] nodesCategories;
    string nodesTemp1Category;
    string nodesTemp2Category;
    public string[] dataCurvesCategories;
    public string[] dataCurvesCategories2;

    private Material data1GraphLine;
    private Material data2GraphLine;
    private Material data3GraphLine;
    private Material nodes1GraphPoint;

    public bool mouseTracking = false;

    public int ddlUsed = 0;
    int nodeUsed = 0;
    int numNodes = 0;
    public float axisXmin = 0;
    public float axisXmax = 0;
    public float axisYmin = 0;
    public float axisYmax = 0;
    public float axisXmaxDefault = 0;
    public float axisYminDefault = 0;
    public float axisYmaxDefault = 0;

    float factorGraphRatioX = 0;
    float factorGraphRatioY = 0;
    float q0MinCurve0;
    float q0MaxCurve0;

    double mousePosX;
    double mousePosY;
    public float mousePosSaveX;
    public float mousePosSaveY;
    public bool mouseLeftButtonON = false;
    public bool mouseRightButtonON = false;

    public int isTutorial = 0;
    public float speed = 0.3f;

    public int cntAvatar = 0;

    void Start()
    {
        avatarManager = ToolBox.GetInstance().GetManager<AvatarManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();

        data1GraphLine = (Material)Resources.Load("Data1GraphLine", typeof(Material));
        data2GraphLine = (Material)Resources.Load("Data2GraphLine", typeof(Material));
        data3GraphLine = (Material)Resources.Load("Data3GraphLine", typeof(Material));
        nodes1GraphPoint = (Material)Resources.Load("Nodes1GraphPoint", typeof(Material));
        takeoffPrefab = (GameObject)Resources.Load("TakeOffParamPrefab", typeof(GameObject));

        dataCategories = new string[2] { "Data1", "Data2" };
        nodesCategories = new string[2] { "Nodes1", "Nodes2" };
        nodesTemp1Category = "NodesTemp1";
        nodesTemp2Category = "NodesTemp2";
        dataCurvesCategories = new string[3] { "Data1", "Data2", "Data3"};
        dataCurvesCategories2 = new string[3] { "Data4", "Data5", "Data6" };

    }

    void Update()
    {
        if (avatarManager.LoadedModels[0].Joints.nodes == null) return;

        if (graph && uiManager.GetCurrentTab() == 2)
        {
            graph.MouseToClient(out mousePosX, out mousePosY);
            if (mousePosX < graph.DataSource.HorizontalViewOrigin || mousePosX > graph.DataSource.HorizontalViewOrigin + graph.DataSource.HorizontalViewSize ||
                mousePosY < graph.DataSource.VerticalViewOrigin || mousePosY > graph.DataSource.VerticalViewOrigin + graph.DataSource.VerticalViewSize)
            {
                return;
            }

            if (isTutorial < 2) return;

            if (mouseRightButtonON) return;

            if (Input.GetMouseButtonDown(0))
            {
                DisplayNodesTemp2();
                mouseLeftButtonON = true;
            }

            if (mouseLeftButtonON)
            {
                graph.DataSource.StartBatch();
                graph.DataSource.ClearCategory(nodesTemp1Category);
                if (avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].interpolation.type == MainParameters.InterpolationType.Quintic)
                    graph.DataSource.AddPointToCategory(nodesTemp1Category, mousePosX, mousePosY);
                else
                    graph.DataSource.AddPointToCategory(nodesTemp1Category, avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T[nodeUsed], mousePosY);
                graph.DataSource.EndBatch();
            }

            if (Input.GetMouseButtonUp(0) && mouseLeftButtonON)
            {
                RemoveNodesTemp();

                if (avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].interpolation.type == MainParameters.InterpolationType.Quintic)
                {
                    if ((nodeUsed <= 0 && mousePosX >= avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T[1]) || (nodeUsed >= numNodes - 1 && mousePosX <= avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T[nodeUsed - 1]) ||
                    (nodeUsed > 0 && nodeUsed < numNodes - 1 && (mousePosX <= avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T[nodeUsed - 1] || mousePosX >= avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T[nodeUsed + 1])))
                    {
                        return;
                    }

                    avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T[nodeUsed] = (float)mousePosX;
                }

                avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].Q[nodeUsed] = (float)mousePosY / Mathf.Rad2Deg;
                gameManager.InterpolationDDL();
                gameManager.DisplayDDL(ddlUsed, true);

                drawManager.StartEditing();
                drawManager.SetCurrrentFrame(0, (int)(avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T[nodeUsed] / drawManager.FrameRate));
                drawManager.StopEditing();
            }
        }
    }

    void RemoveNodesTemp()
    {
        graph.DataSource.StartBatch();
        graph.DataSource.ClearCategory(nodesTemp1Category);
        graph.DataSource.ClearCategory(nodesTemp2Category);
        graph.DataSource.ClearCategory(nodesCategories[0]);
        for (int i = 0; i < avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T.Length; i++)
        {
            float value = avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].Q[i] * Mathf.Rad2Deg;
            if (avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T[i] <= axisXmax && value >= axisYmin && value <= axisYmax)
                graph.DataSource.AddPointToCategory(nodesCategories[0], avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T[i], value);
        }
        graph.DataSource.EndBatch();
        mouseLeftButtonON = false;
    }

    public void DisplayCurveAndNodes(int curve, int ddl, bool axisRange)
    {
        if (avatarManager.LoadedModels[0].Joints.nodes == null) return;
        if (avatarManager.LoadedModels[0].Joints.t0 == null) return;

        if (graph == null) return;

        graph.DataSource.StartBatch();

        graph.DataSource.ClearCategory(dataCategories[curve]);
        graph.DataSource.ClearCategory(nodesCategories[curve]);
        if (curve <= 0)
        {
            ddlUsed = ddl;
            numNodes = avatarManager.LoadedModels[0].Joints.nodes[ddl].T.Length;
            graph.DataSource.ClearCategory(dataCategories[1]);
            graph.DataSource.ClearCategory(nodesCategories[1]);
        }

        float q0Min = 360;
        float q0Max = -360;
        float value;

        int t0Length = avatarManager.LoadedModels[0].Joints.t0.Length;
        for (int i = 0; i < t0Length; i++)
        {
            value = avatarManager.LoadedModels[0].Joints.q0[ddl, i] * Mathf.Rad2Deg;
            if (!axisRange && value < axisYmin)
                value = axisYmin;
            if (!axisRange && value > axisYmax)
                value = axisYmax;
            if (axisRange || avatarManager.LoadedModels[0].Joints.t0[i] <= axisXmax)
                graph.DataSource.AddPointToCategory(dataCategories[curve], avatarManager.LoadedModels[0].Joints.t0[i], value);
            if (value < q0Min) q0Min = value;
            if (value > q0Max) q0Max = value;
        }

        for (int i = 0; i < avatarManager.LoadedModels[0].Joints.nodes[ddl].T.Length; i++)
        {
            value = avatarManager.LoadedModels[0].Joints.nodes[ddl].Q[i] * Mathf.Rad2Deg;
            if (axisRange || (avatarManager.LoadedModels[0].Joints.nodes[ddl].T[i] <= axisXmax && value >= axisYmin && value <= axisYmax))
                graph.DataSource.AddPointToCategory(nodesCategories[curve], avatarManager.LoadedModels[0].Joints.nodes[ddl].T[i], value);
        }

        MaterialTiling tiling = new MaterialTiling(false, 45.5f);

        if (ddl == 2 || ddl == 3)
        {
            graph.DataSource.SetCategoryLine(dataCategories[curve], data3GraphLine, 2.58f, tiling);
        }
        else if (ddl == 4 || ddl == 5)
        {
            graph.DataSource.SetCategoryLine(dataCategories[curve], data2GraphLine, 2.58f, tiling);
        }
        else
        {
            graph.DataSource.SetCategoryLine(dataCategories[curve], data1GraphLine, 2.58f, tiling);
        }

        graph.DataSource.SetCategoryPoint(nodesCategories[curve], nodes1GraphPoint, 8);

        if (axisRange)
        {
            axisXmin = Mathf.Round(avatarManager.LoadedModels[0].Joints.t0[0] - 0.5f);
            axisXmax = Mathf.Round(avatarManager.LoadedModels[0].Joints.t0[t0Length - 1] + 0.5f);
            axisXmaxDefault = axisXmax;
            graph.DataSource.HorizontalViewOrigin = axisXmin;
            graph.DataSource.HorizontalViewSize = axisXmax - axisXmin;
            factorGraphRatioX = graph.WidthRatio / (float)graph.DataSource.HorizontalViewSize;
            if (curve <= 0)
            {
                q0MinCurve0 = q0Min;
                q0MaxCurve0 = q0Max;
            }
            else
            {
                q0Min = Mathf.Min(q0Min, q0MinCurve0);
                q0Max = Mathf.Max(q0Max, q0MaxCurve0);
            }
            axisYmin = Mathf.Round((q0Min - 30) / 10) * 10;
            axisYmax = Mathf.Round((q0Max + 30) / 10) * 10;
            axisYminDefault = axisYmin;
            axisYmaxDefault = axisYmax;
            graph.DataSource.VerticalViewOrigin = axisYmin;
            graph.DataSource.VerticalViewSize = axisYmax - axisYmin;
            factorGraphRatioY = graph.HeightRatio / (float)graph.DataSource.VerticalViewSize;
        }
        else
        {
            graph.DataSource.HorizontalViewOrigin = axisXmin;
            graph.DataSource.HorizontalViewSize = axisXmax - axisXmin;
            factorGraphRatioX = graph.WidthRatio / (float)graph.DataSource.HorizontalViewSize;
            graph.DataSource.VerticalViewOrigin = axisYmin;
            graph.DataSource.VerticalViewSize = axisYmax - axisYmin;
            factorGraphRatioY = graph.HeightRatio / (float)graph.DataSource.VerticalViewSize;
        }
        graph.DataSource.EndBatch();
    }


    public void DisplayNodesTemp2()
    {
        graph.DataSource.StartBatch();

        // Effacer les noeuds précédents
        graph.DataSource.ClearCategory(nodesCategories[0]);

        // Trouver le noeud le plus près de la position de la souris (en tenant compte du ratio X vs Y du graphique), ça sera ce noeud qui sera modifié

        mousePosSaveX = (float)mousePosX;
        mousePosSaveY = (float)mousePosY;
        nodeUsed = FindNearestNode();
        // Ajouter les noeuds dans la nouvelle courbe Nodes et NodesTemp2

        for (int i = 0; i < numNodes; i++)
        {
            float value = avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].Q[i] * Mathf.Rad2Deg;
            if (avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T[i] <= axisXmax && value >= axisYmin && value <= axisYmax)
            {
                if (i != nodeUsed)
                    graph.DataSource.AddPointToCategory(nodesCategories[0], avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T[i], value);
                else
                    graph.DataSource.AddPointToCategory(nodesTemp2Category, avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T[i], value);
            }
        }

        graph.DataSource.EndBatch();
    }

    public int FindNearestNode()
    {
        int node = 0;
        float minDistanceToNode = 99999;

        for (int i = 0; i < avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T.Length; i++)
        {
            float distanceToNode = Mathf.Pow((mousePosSaveX - avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].T[i]) * factorGraphRatioX, 2) +
                Mathf.Pow((mousePosSaveY - avatarManager.LoadedModels[0].Joints.nodes[ddlUsed].Q[i] * Mathf.Rad2Deg) * factorGraphRatioY, 2);
            if (distanceToNode < minDistanceToNode)
            {
                node = i;
                minDistanceToNode = distanceToNode;
            }
        }
        return node;
    }

    public void DisplayCurves(GraphChart graphCurves, float[] t, float[] data, float[] t2 = null, float[] data2 = null)
    {
        float[,] data1 = new float[data.GetUpperBound(0) + 1, 1];
        for (int i = 0; i <= data.GetUpperBound(0); i++)
            data1[i, 0] = data[i];

        if (t2 != null)
        {
            float[,] data3 = new float[data2.GetUpperBound(0) + 1, 1];
            for (int i = 0; i <= data2.GetUpperBound(0); i++)
                data3[i, 0] = data2[i];

            DisplayCurves(graphCurves, t, data1, t2, data3);
        }
        else
            DisplayCurves(graphCurves, t, data1);
    }

    public void DisplayCurves(GraphChart graphCurves, float[] t, float[,] data, float[] t2 = null, float[,] data2 = null)
    {
        if (data == null) return;

        if (graphCurves == null) return;
        graphCurves.DataSource.StartBatch();

        for (int i = 0; i < dataCurvesCategories.Length; i++)
            graphCurves.DataSource.ClearCategory(dataCurvesCategories[i]);


        float tMin = 999999;
        float tMax = -999999;
        float dataMin = 999999;
        float dataMax = -999999;
        for (int i = 0; i <= data.GetUpperBound(1); i++)
        {
            for (int j = 0; j < t.Length; j++)
            {
                graphCurves.DataSource.AddPointToCategory(dataCurvesCategories[i], t[j], data[j, i]);
                if (i <= 0 && t[j] < tMin) tMin = t[j];
                if (i <= 0 && t[j] > tMax) tMax = t[j];
                if (data[j, i] < dataMin) dataMin = data[j, i];
                if (data[j, i] > dataMax) dataMax = data[j, i];
            }
        }

        if(t2 != null)
        {
            for (int i = 0; i < dataCurvesCategories2.Length; i++)
                graphCurves.DataSource.ClearCategory(dataCurvesCategories2[i]);

            for (int i = 0; i <= data2.GetUpperBound(1); i++)
            {
                for (int j = 0; j < t2.Length; j++)
                {
                    graphCurves.DataSource.AddPointToCategory(dataCurvesCategories2[i], t2[j], data2[j, i]);
                    if (i <= 0 && t2[j] < tMin) tMin = t2[j];
                    if (i <= 0 && t2[j] > tMax) tMax = t2[j];
                    if (data2[j, i] < dataMin) dataMin = data2[j, i];
                    if (data2[j, i] > dataMax) dataMax = data2[j, i];
                }
            }
        }
        if (Mathf.Abs(dataMin) < 1e-10 && Mathf.Abs(dataMax) < 1e-10)
        {
            dataMin = -0.5f;
            dataMax = 0.5f;
        }

        graphCurves.DataSource.HorizontalViewOrigin = tMin;
        graphCurves.DataSource.HorizontalViewSize = tMax - tMin;

        if (tMax - tMin <= 2)
            graphCurves.GetComponent<HorizontalAxis>().MainDivisions.FractionDigits = 2;
        else
            graphCurves.GetComponent<HorizontalAxis>().MainDivisions.FractionDigits = 1;
        graphCurves.DataSource.VerticalViewOrigin = dataMin;
        graphCurves.DataSource.VerticalViewSize = dataMax - dataMin;
        if (dataMax - dataMin <= 0.2)
            graphCurves.GetComponent<VerticalAxis>().MainDivisions.FractionDigits = 3;
        else if (dataMax - dataMin <= 2)
            graphCurves.GetComponent<VerticalAxis>().MainDivisions.FractionDigits = 2;
        else
            graphCurves.GetComponent<VerticalAxis>().MainDivisions.FractionDigits = 1;

        graphCurves.DataSource.EndBatch();

        bDraw = true;
    }

    public void TaskOffGraphOn()
    {
        takeoffCanvas = Instantiate(takeoffPrefab);
        graph = takeoffCanvas.GetComponentInChildren<GraphChart>();

        drawManager.PlayEnd();
    }

    public void TaskOffGraphOff()
    {
        Destroy(takeoffCanvas);
    }

    public void GraphOn()
    {
        if(graph == null)
            graph = GameObject.Find("TrainingMenu").transform.Find("Canvas/TabPanel/TabContainer/TabTwo/Content2/PanelGraph/GraphMultiple/").gameObject.GetComponent<GraphChart>();
    }

}
