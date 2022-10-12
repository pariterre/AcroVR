using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChartAndGraph;

public class DisplayResultGraphicS : MonoBehaviour
{
    protected DrawManager drawManager;
    protected AniGraphManager aniGraphManager;

	public Dropdown dropDownGraphicName;
	public GraphChart graph;
	public int panelGraphicNumber;
	public Text textLabelAxisX;
	public Text textLabelAxisY;
	
	public GameObject panelLegend;
	public Text textLegendCurveName1;
	public Text textLegendCurveName2;
    public Text textLegendCurveName3;

    public Text resultText;
    public Text resultText2;

	void Awake()
	{
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        aniGraphManager = ToolBox.GetInstance().GetManager<AniGraphManager>();
	}

	void OnEnable()
	{
		List<string> dropDownOptions = new List<string>();
		dropDownOptions.Add(MainParameters.Instance.languages.Used.resultsGraphicsSelectionRotationsVsTime);
		dropDownOptions.Add(MainParameters.Instance.languages.Used.resultsGraphicsSelectionTiltVsTime);
		dropDownOptions.Add(MainParameters.Instance.languages.Used.resultsGraphicsSelectionTiltVsSomersault);
		dropDownOptions.Add(MainParameters.Instance.languages.Used.resultsGraphicsSelectionTiltVsTwist);
		dropDownOptions.Add(MainParameters.Instance.languages.Used.resultsGraphicsSelectionTwistVsSomersault);
		dropDownOptions.Add(MainParameters.Instance.languages.Used.resultsGraphicsSelectionAngularSpeedVsTime);
		dropDownGraphicName.ClearOptions();
		dropDownGraphicName.AddOptions(dropDownOptions);
		dropDownGraphicName.value = MainParameters.Instance.resultsGraphicsUsed[panelGraphicNumber - 1];

        DropDownGraphicNameOnValueChanged(MainParameters.Instance.resultsGraphicsUsed[panelGraphicNumber - 1]);

        drawManager.RegisterResultShow(this);
    }

    public void CloseBox()
    {
        drawManager.UnregisterResultShow();
    }

    public void ShowResult()
    {
        if (drawManager.Joints(0).q0 == null) return;

        if (aniGraphManager.cntAvatar == 1)
        {
            resultText.text = drawManager.DisplayMessage();
            resultText2.text = drawManager.DisplayMessageSecond();
        }
        else
        {
            resultText.text = drawManager.DisplayMessage();
            resultText2.text = "";
        }
    }

    public void DropDownAvatarChanged(int value)
    {
        if (drawManager.Joints(0).q0 == null) return;

        aniGraphManager.cntAvatar = value;
        DropDownGraphicNameOnValueChanged(MainParameters.Instance.resultsGraphicsUsed[panelGraphicNumber - 1]);
        ShowResult();
    }
        
    public void DropDownGraphicNameOnValueChanged(int value)
    {
        if (drawManager.Joints(0).q0 == null) return;
        if (drawManager.Joints(0).rot == null) return;

        MainParameters.Instance.resultsGraphicsUsed[panelGraphicNumber - 1] = value;

        ShowResultGraph(value);

        switch (value)
        {
            case 0:
                textLabelAxisX.text = MainParameters.Instance.languages.Used.resultsGraphicsLabelAxisTime;
                textLabelAxisY.text = MainParameters.Instance.languages.Used.resultsGraphicsLabelAxisRotation;
                panelLegend.SetActive(true);
                textLegendCurveName1.text = MainParameters.Instance.languages.Used.resultsGraphicsLegendCurveNameSomersault;
                textLegendCurveName2.text = MainParameters.Instance.languages.Used.resultsGraphicsLegendCurveNameTilt;
                textLegendCurveName3.text = MainParameters.Instance.languages.Used.resultsGraphicsLegendCurveNameTwist;
                break;
            case 1:
                textLabelAxisX.text = MainParameters.Instance.languages.Used.resultsGraphicsLabelAxisTime;
                textLabelAxisY.text = MainParameters.Instance.languages.Used.resultsGraphicsLabelAxisTilt;
                panelLegend.SetActive(false);
                break;
            case 2:
                textLabelAxisX.text = MainParameters.Instance.languages.Used.resultsGraphicsLabelAxisSomersault;
                textLabelAxisY.text = MainParameters.Instance.languages.Used.resultsGraphicsLabelAxisTilt;
                panelLegend.SetActive(false);
                break;
            case 3:
                textLabelAxisX.text = MainParameters.Instance.languages.Used.resultsGraphicsLabelAxisTwist;
                textLabelAxisY.text = MainParameters.Instance.languages.Used.resultsGraphicsLabelAxisTilt;
                panelLegend.SetActive(false);
                break;
            case 4:
                textLabelAxisX.text = MainParameters.Instance.languages.Used.resultsGraphicsLabelAxisSomersault;
                textLabelAxisY.text = MainParameters.Instance.languages.Used.resultsGraphicsLabelAxisTwist;
                panelLegend.SetActive(false);
                break;
            case 5:
                textLabelAxisX.text = MainParameters.Instance.languages.Used.resultsGraphicsLabelAxisTime;
                textLabelAxisY.text = MainParameters.Instance.languages.Used.resultsGraphicsLabelAxisAngularSpeed;
                panelLegend.SetActive(true);
                textLegendCurveName1.text = MainParameters.Instance.languages.Used.resultsGraphicsLegendCurveNameSomersault;
                textLegendCurveName2.text = MainParameters.Instance.languages.Used.resultsGraphicsLegendCurveNameTilt;
                textLegendCurveName3.text = MainParameters.Instance.languages.Used.resultsGraphicsLegendCurveNameTwist;
                break;
        }

        ShowResult();
    }

    public void UpdateResults()
    {
        ShowResultGraph(MainParameters.Instance.resultsGraphicsUsed[panelGraphicNumber - 1]);
        ShowResult();
    }

    private void ShowResultGraph(int _v)
    {
        if(aniGraphManager.cntAvatar == 1)
        {
            if (drawManager.girl2 == null)
            {
                graph.DataSource.StartBatch();

                graph.DataSource.ClearCategory("Data1");
                graph.DataSource.ClearCategory("Data2");
                graph.DataSource.ClearCategory("Data3");

                graph.DataSource.ClearCategory("Data4");
                graph.DataSource.ClearCategory("Data5");
                graph.DataSource.ClearCategory("Data6");

                graph.DataSource.EndBatch();

                return;
            }

            switch (_v)
            {
                case 0:
                    aniGraphManager.DisplayCurves(
                        graph, 
                        drawManager.Joints(0).t,
                        drawManager.Joints(0).rot,
                        drawManager.Joints(1).t,
                        drawManager.Joints(1).rot
                        );
                    break;
                case 1:
                    aniGraphManager.DisplayCurves(
                        graph,
                        drawManager.Joints(0).t,
                        MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 1), 
                        drawManager.Joints(1).t, 
                        MathFunc.MatrixGetColumn(drawManager.Joints(1).rot, 1)
                    );
                    break;
                case 2:
                    aniGraphManager.DisplayCurves(
                        graph, 
                        MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 0), 
                        MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 1), 
                        MathFunc.MatrixGetColumn(drawManager.Joints(1).rot, 0), 
                        MathFunc.MatrixGetColumn(drawManager.Joints(1).rot, 1)
                    );
                    break;
                case 3:
                    aniGraphManager.DisplayCurves(
                        graph, 
                        MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 2), 
                        MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 1), 
                        MathFunc.MatrixGetColumn(drawManager.Joints(1).rot, 2), 
                        MathFunc.MatrixGetColumn(drawManager.Joints(1).rot, 1)
                    );
                    break;
                case 4:
                    aniGraphManager.DisplayCurves(
                        graph, 
                        MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 0), 
                        MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 2), 
                        MathFunc.MatrixGetColumn(drawManager.Joints(1).rot, 0), 
                        MathFunc.MatrixGetColumn(drawManager.Joints(1).rot, 2)
                    );
                    break;
                case 5:
                    aniGraphManager.DisplayCurves(
                        graph, 
                        drawManager.Joints(0).t, 
                        drawManager.Joints(0).rotdot, 
                        drawManager.Joints(1).t, 
                        drawManager.Joints(1).rotdot
                    );
                    break;
            }
        }
        else
        {
            if (_v == 0)
                aniGraphManager.DisplayCurves(graph, drawManager.Joints(0).t, drawManager.Joints(0).rot);
            else if (_v == 1)
                aniGraphManager.DisplayCurves(graph, drawManager.Joints(0).t, MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 1));
            else if (_v == 2)
                aniGraphManager.DisplayCurves(graph, MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 0), MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 1));
            else if (_v == 3)
                aniGraphManager.DisplayCurves(graph, MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 2), MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 1));
            else if (_v == 4)
                aniGraphManager.DisplayCurves(graph, MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 0), MathFunc.MatrixGetColumn(drawManager.Joints(0).rot, 2));
            else if (_v == 5)
                aniGraphManager.DisplayCurves(graph, drawManager.Joints(0).t, drawManager.Joints(0).rotdot);
            else
                throw new ArgumentException("Wrong _v");
        }
    }
}