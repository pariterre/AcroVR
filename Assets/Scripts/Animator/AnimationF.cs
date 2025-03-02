﻿using System;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Research.Oslo;

// =================================================================================================================================================================
/// <summary> Fonctions utilisées pour exécuter les animations. </summary>

public class AnimationF : MonoBehaviour
{
    public static AnimationF Instance;
    public Camera cameraAnimation;
    public GameObject panelAnimator;
    public GameObject panelLegend;
    public Text textCurveName1;
    public Text textCurveName2;
    public Text textChrono;
    public Text textMsg;
    public Dropdown dropDownPlayMode;
    public Dropdown dropDownPlayView;
    public Dropdown dropDownPlaySpeed;
    public Button buttonPlay;
    public Image buttonPlayImage;
    public GameObject buttonStop;
    public Button buttonGraph;
    public Image buttonGraphImage;
    public GameObject panelResultsGraphics;
    public Button buttonRealTime;
    public Image buttonRealTimeImage;
    public GameObject buttonOffline;
    public Button buttonFirstPerson;
    public Image buttonFirstPersonImage;
    public GameObject buttonThirdPerson;
    public Button buttonVRHeadSet;
    public Image buttonVRHeadSetImage;
    public GameObject buttonNoVRHeadSet;

    public Text textScrollViewMessages;

    public LineRenderer[] lineStickFigure;
    LineRenderer lineCenterOfMass;
    LineRenderer[] lineFilledFigure;
    LineRenderer[] lineFloor;

    public bool animateON = false;
	public string playMode = MainParameters.Instance.languages.Used.animatorPlayModeSimulation;

	/// <summary> Vecteur contenant l'état (q0 et q0dot) au temps t(frame - 1). </summary>
	public static double[] xTFrame0;
	/// <summary> Vecteur contenant l'état (q0 et q0dot) au temps t(frame). </summary>
	public static double[] xTFrame1;

	// Vecteurs contenant les positions, vitesses et accelerations des articulations à l'instant t(frame - 1), t(frame - 0.5) et t(frame)

	float[] qFrame0;                  // t(frame - 1)
	float[] qdotFrame0;
	float[] qddotFrame0;
	float[] qFrame1;                  // t(frame - 0.5)
	float[] qdotFrame1;
	float[] qddotFrame1;
	float[] qFrame2;                  // t(frame)
	float[] qdotFrame2;
	float[] qddotFrame2;

	public int frameN = 0;
    //int firstFrame = 0;
    int numberFrames = 0;
    int frameContactGround = 0;
    float tagXMin = 0;
    float tagXMax = 0;
    float tagYMin = 0;
    float tagYMax = 0;
    float tagZMin = 0;
    float tagZMax = 0;
    float factorTags2Screen = 1;
    float factorTags2ScreenX = 1;
    float factorTags2ScreenY = 1;
    float animationMaxDimOnScreen = 20;         // Dimension maximum de la silhouette à l'écran en unité de "Line renderer" (même dimension dans les 3 directions x, y, z)

    float timeElapsed = 0;
	public float[] debugTimeElapsed;
    float timeFrame = 0;
    float timeStarted = 0;
    float factorPlaySpeed = 1;

    double[] tQ;
    double[,] q;
    double[,] qdot;

    float[,] qd;                                // Positions des angles des articulations (ddl) (2 * NDDL = échantillonnage à dt / 2)
    float[,] qdotd;                             // Vitesses des angles des articulations (ddl)
    float[,] qddotd;                            // Accélérations des angles des articulations (ddl)

    // =================================================================================================================================================================
    /// <summary> Initialisation du script. </summary>

    void Start()
    {
        Instance = this;

        lineStickFigure = null;
        lineCenterOfMass = null;
        lineFilledFigure = null;
        lineFloor = null;

        dropDownPlayMode.interactable = false;
        dropDownPlayView.interactable = false;
		if (!MainParameters.Instance.testXSensUsed)
		{
			buttonPlay.interactable = false;
			buttonPlayImage.color = Color.gray;
		}
		dropDownPlaySpeed.interactable = false;
		buttonGraph.interactable = false;
		buttonGraphImage.color = Color.gray;
        buttonRealTime.interactable = false;
        buttonRealTimeImage.color = Color.gray;
        buttonFirstPerson.interactable = false;
        buttonFirstPersonImage.color = Color.gray;
        buttonVRHeadSet.interactable = false;
        buttonVRHeadSetImage.color = Color.gray;

        if (Screen.width / Screen.height >= 1.7)
            animationMaxDimOnScreen = 20;
        else
            animationMaxDimOnScreen = 18;

        // Initialisation du modèle BioRBD utilisé

#if UNITY_STANDALONE_OSX
		//string pathToM_Model = string.Format("{0}/Modele_HuManS_somersault.s2mMod", Application.streamingAssetsPath); //Recupération du modèle biorbd
		string pathToM_Model = @"AcroVR/Contents/Resources/Data/StreamingAssets/Modele_HuManS_somersault.s2mMod";
		//System.IO.File.AppendAllText(@"AcroVR_Debug.txt", string.Format("{0}{1}", pathToM_Model, System.Environment.NewLine));
		//System.IO.File.AppendAllText(@"AcroVR_Debug.txt", string.Format("{0}{1}", dllpath, System.Environment.NewLine));
#else
        string pathToM_Model = string.Format("{0}/Modele_HuManS_somersault.s2mMod", Application.streamingAssetsPath); //Recupération du modèle biorbd
#endif
        MainParameters.Instance.ptr_model = MainParameters.c_biorbdModel(new StringBuilder(pathToM_Model));
    }

    // =================================================================================================================================================================
    /// <summary> Exécution de la fonction à chaque frame. </summary>

    void Update()
    {
        // Exécuter seulement si l'animation a été démarré

        if (!animateON) return;

        // Synchroniser l'affichage pour que l'exécution de l'animation prenne le temps de simulation spécifié.
        // Synchroniser aussi l'affichage selon la vitesse d'exécution de l'animation spécifié par l'utilisateur.

        if (frameN <= 0) timeStarted = Time.time;
        if (Time.time - timeStarted >= (timeFrame * frameN) * factorPlaySpeed)
        {
            timeElapsed = Time.time - timeStarted;

            // Affichage du chronomètre

            if (numberFrames > 1)
				textChrono.text = string.Format("{0:0.0}", timeElapsed);

			// Traiter le prochain frame

			if (frameN < numberFrames)
			{
				debugTimeElapsed[frameN] = timeElapsed;
				PlayOneFrame();                     // Affichage de la silhouette
			}
			else
				PlayEnd();                          // Fin de l'animation
        }
    }

    // =================================================================================================================================================================
    /// <summary> La liste déroulante PlayView a été modifier. </summary>

    public void DropDownPlayView()
    {
        if (dropDownPlayView.captionText.text == MainParameters.Instance.languages.Used.animatorPlayViewFrontal)
        {
            cameraAnimation.transform.position = new Vector3(0, 18, 0);
            cameraAnimation.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
        else
        {
            cameraAnimation.transform.position = new Vector3(-18, 0, 0);
            cameraAnimation.transform.rotation = Quaternion.Euler(0, 90, 90);
        }
    }

    // =================================================================================================================================================================
    /// <summary> Bouton Play a été appuyer. </summary>

    public void ButtonPlay()
    {
        // Afficher le bouton Stop et désactiver les autres contrôles du logiciel, durant l'animation

        buttonStop.SetActive(true);
        Main.Instance.EnableDisableControls(false, true);

        // Lecture des paramètres de décolage

        MainParameters.Instance.joints.condition = MovementF.Instance.dropDownCondition.value;
        MainParameters.Instance.joints.takeOffParam.rotation = float.Parse(MovementF.Instance.inputFieldSomersaultPosition.text);
        MainParameters.Instance.joints.takeOffParam.tilt = float.Parse(MovementF.Instance.inputFieldTilt.text);
        MainParameters.Instance.joints.takeOffParam.anteroposteriorSpeed = float.Parse(MovementF.Instance.inputFieldHorizontalSpeed.text);
        MainParameters.Instance.joints.takeOffParam.verticalSpeed = float.Parse(MovementF.Instance.inputFieldVerticalSpeed.text);
        MainParameters.Instance.joints.takeOffParam.somersaultSpeed = float.Parse(MovementF.Instance.inputFieldSomersaultSpeed.text);
        MainParameters.Instance.joints.takeOffParam.twistSpeed = float.Parse(MovementF.Instance.inputFieldTwistSpeed.text);

        // Lecture du mode d'exécution de l'animation

        playMode = dropDownPlayMode.captionText.text;

        // Lecture de la vitesse d'exécution de l'animation

        string playSpeed = dropDownPlaySpeed.captionText.text;
        if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedSlow3)
            factorPlaySpeed = 10;
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedSlow2)
            factorPlaySpeed = 3;
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedSlow1)
            factorPlaySpeed = 1.5f;
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedNormal)
            factorPlaySpeed = 1;
        else if (playSpeed == MainParameters.Instance.languages.Used.animatorPlaySpeedFast)
            factorPlaySpeed = 0.8f;

        // Affichage d'un message dans la boîte des messages

        DisplayNewMessage(true, false, string.Format(" {0}", MainParameters.Instance.languages.Used.displayMsgStartSimulation));
        DisplayNewMessage(false, false, string.Format(" {0} = {1:0.00000} s", MainParameters.Instance.languages.Used.displayMsgDtValue, MainParameters.Instance.joints.lagrangianModel.dt));

        // Exécution des calculs de simulation

        float[,] q1;
        int numFramesTot = DoSimulation.GetSimulation(out q1);

        // Calculer un facteur de correspondance entre le volume utilisé par la silhouette et la dimension du volume disponible pour l'affichage
        // Pour cela, il nous faut calculer les valeurs minimum et maximum des DDLs de la silhouette, dans les 3 dimensions
        // Même si on modifie la dimension de la silhouette, on conserve quand même les proportions de la sihouette dans les 3 dimensions, donc le facteur est unique pour les 3 dimensions
        // On ajoute une marge de 10% sur les dimensions mimimum et maximum, pour être certain que les mouvements ne dépasseront pas la dimension du panneau utilisé

        if (playMode == MainParameters.Instance.languages.Used.animatorPlayModeSimulation || playMode == MainParameters.Instance.languages.Used.animatorPlayModeGesticulation)
			InitScreenVolumeDimension(q1);
        else
            PlayReset();

		// Afficher la silhouette pour toute l'animation

		Play(0, numFramesTot);
	}

	// =================================================================================================================================================================
	/// <summary> Bouton Stop a été appuyer. </summary>

	public void ButtonStop()
    {
        PlayEnd();
    }

    // =================================================================================================================================================================
    /// <summary> Bouton Graphiques des résultats a été appuyer. </summary>

    public void ButtonGraph()
    {
        Main.Instance.EnableDisableControls(false, true);
        EnableDisableAnimationOutline(false);
        GraphManager.Instance.mouseTracking = false;
        panelResultsGraphics.SetActive(true);
    }

    // =================================================================================================================================================================
    /// <summary> Bouton OK du panneau Graphiques des résultats a été appuyer. </summary>

    public void ButtonGraphOK()
    {
        Main.Instance.EnableDisableControls(true, true);
        EnableDisableAnimationOutline(true);
        GraphManager.Instance.mouseTracking = true;
        panelResultsGraphics.SetActive(false);
    }

    // =================================================================================================================================================================
    /// <summary> Bouton RealTime a été appuyer. </summary>

    public void ButtonRealTime()
    {
        buttonOffline.SetActive(true);
    }

    // =================================================================================================================================================================
    /// <summary> Bouton Offline a été appuyer. </summary>

    public void ButtonOffline()
    {
        buttonOffline.SetActive(false);
    }

    // =================================================================================================================================================================
    /// <summary> Bouton FirstPerson a été appuyer. </summary>

    public void ButtonFirstPerson()
    {
        buttonThirdPerson.SetActive(true);
    }

    // =================================================================================================================================================================
    /// <summary> Bouton ThirdPerson a été appuyer. </summary>

    public void ButtonThirdPerson()
    {
        buttonThirdPerson.SetActive(false);
    }

    // =================================================================================================================================================================
    /// <summary> Bouton VR HeadSet a été appuyer. </summary>

    public void ButtonVRHeadSet()
    {
        buttonNoVRHeadSet.SetActive(true);
    }

    // =================================================================================================================================================================
    /// <summary> Bouton ThirdPerson a été appuyer. </summary>

    public void ButtonNoVRHeadSet()
    {
        buttonNoVRHeadSet.SetActive(false);
    }

    // =================================================================================================================================================================
    /// <summary> Démarrer l'exécution de l'animation. </summary>

    public void Play(int frFrame, int nFrames)
    {
        MainParameters.StrucJoints joints = MainParameters.Instance.joints;

        // Initialisation de certains paramètres

        tQ = new double[nFrames];
        q = new double[joints.lagrangianModel.nDDL, nFrames];
        qdot = new double[joints.lagrangianModel.nDDL, nFrames];

        qFrame0 = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
        qdotFrame0 = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
        qddotFrame0 = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
		qFrame1 = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
		qdotFrame1 = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
		qddotFrame1 = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
		qFrame2 = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
		qdotFrame2 = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
		qddotFrame2 = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];

		xTFrame1 = new double[MainParameters.Instance.joints.lagrangianModel.nDDL * 2];

		frameN = 0;
        // firstFrame = frFrame;
        numberFrames = nFrames;
		debugTimeElapsed = new float[numberFrames];
        frameContactGround = numberFrames + 1;
        if (nFrames > 1)
            timeFrame = MainParameters.Instance.joints.lagrangianModel.dt;
        else
            timeFrame = 0;
		animateON = !Main.Instance.testXSensUsed;
        GraphManager.Instance.mouseTracking = false;

        // Effacer le message affiché au bas du panneau d'animation

        textMsg.text = "";

        // Création et initialisation des "GameObject Line Renderer"

        if (lineStickFigure == null && lineCenterOfMass == null && lineFilledFigure == null)
        {
            GameObject lineObject = new GameObject();
            LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
			lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
			lineRenderer.startWidth = 0.04f;
            lineRenderer.endWidth = 0.04f;
            lineRenderer.gameObject.layer = 8;

            lineFloor = new LineRenderer[4];
            for (int i = 0; i < 4; i++)
            {
                lineFloor[i] = Instantiate(lineRenderer);
                lineFloor[i].startColor = Color.green;
                lineFloor[i].endColor = Color.green;
                lineFloor[i].startWidth = 0.1f;
                lineFloor[i].endWidth = 0.1f;
                lineFloor[i].name = string.Format("LineFloor{0}", i + 1);
                lineFloor[i].transform.parent = panelAnimator.transform;
            }

            lineStickFigure = new LineRenderer[joints.lagrangianModel.stickFigure.Length / 2];
            for (int i = 0; i < joints.lagrangianModel.stickFigure.Length / 2; i++)
            {
                lineStickFigure[i] = Instantiate(lineRenderer);
                lineStickFigure[i].name = string.Format("LineStickFigure{0}", i + 1);
                lineStickFigure[i].transform.parent = panelAnimator.transform;
                if (i <= 2 || (i >= 17 && i <= 19))                             // Côté gauche (jambe, pied, bras et main)
                {
                    lineStickFigure[i].startColor = new Color(0, 0.5882f, 0, 1);
                    lineStickFigure[i].endColor = new Color(0, 0.5882f, 0, 1);
                }
                else if ((i >= 3 && i <= 5) || (i >= 20 && i <= 22))             // Côté droit
                {
                    lineStickFigure[i].startColor = new Color(0.9412f, 0, 0.9412f, 1);
                    lineStickFigure[i].endColor = new Color(0.9412f, 0, 0.9412f, 1);
                }
            }

            lineCenterOfMass = Instantiate(lineRenderer);
            lineCenterOfMass.startColor = Color.red;
            lineCenterOfMass.endColor = Color.red;
            lineCenterOfMass.name = "LineCenterOfMass";
            lineCenterOfMass.transform.parent = panelAnimator.transform;

            lineFilledFigure = new LineRenderer[joints.lagrangianModel.filledFigure.Length / 4];
            for (int i = 0; i < joints.lagrangianModel.filledFigure.Length / 4; i++)
            {
                lineFilledFigure[i] = Instantiate(lineRenderer);
                lineFilledFigure[i].startColor = Color.yellow;
                lineFilledFigure[i].endColor = Color.yellow;
                lineFilledFigure[i].name = string.Format("LineFilledFigure{0}", i + 1);
                lineFilledFigure[i].transform.parent = panelAnimator.transform;
            }

            Destroy(lineObject);

            textCurveName1.text = MainParameters.Instance.languages.Used.leftSide;
            textCurveName2.text = MainParameters.Instance.languages.Used.rightSide;
            panelLegend.SetActive(true);
        }
    }

	// =================================================================================================================================================================
	/// <summary> Exécution d'un frame de l'animation. </summary>

	public void PlayOneFrame()
	{
		MainParameters.StrucJoints joints = MainParameters.Instance.joints;

		// Effacer la silhouette précédente en premier

		for (int i = 0; i < joints.lagrangianModel.stickFigure.Length / 2; i++)
			DrawObjects.Instance.Delete(lineStickFigure[i]);
		DrawObjects.Instance.Delete(lineCenterOfMass);
		for (int i = 0; i < joints.lagrangianModel.filledFigure.Length / 4; i++)
			DrawObjects.Instance.Delete(lineFilledFigure[i]);
		for (int i = 0; i < 4; i++)
			DrawObjects.Instance.Delete(lineFloor[i]);

		// Cas spécial à t = 0: on ne fait pas de calcul d'intégration et on utilise les données q0 et q0dot tel quel

		Trajectory trajectory;
		if (frameN <= 0)
		{
			for (int i = 0; i < xTFrame0.Length; i++)
				xTFrame1[i] = xTFrame0[i];

			// Trajectory pour le t = 0

			trajectory = new Trajectory(MainParameters.Instance.joints.lagrangianModel, 0, MainParameters.Instance.joints.lagrangianModel.q2, out qFrame0, out qdotFrame0, out qddotFrame0);
		}

		// Calcul d'intégration

		else
		{
			float tFrame = (frameN - 1) * timeFrame;

			// Trajectory pour le t = timeFrame / 2

			trajectory = new Trajectory(MainParameters.Instance.joints.lagrangianModel, tFrame + MainParameters.Instance.joints.lagrangianModel.dt / 2,
				MainParameters.Instance.joints.lagrangianModel.q2, out qFrame1, out qdotFrame1, out qddotFrame1);

			// Trajectory pour le t = timeFrame

			trajectory = new Trajectory(MainParameters.Instance.joints.lagrangianModel, tFrame + MainParameters.Instance.joints.lagrangianModel.dt, MainParameters.Instance.joints.lagrangianModel.q2,
				 out qFrame2, out qdotFrame2, out qddotFrame2);

			float[,] qint = new float[MainParameters.Instance.joints.lagrangianModel.nDDL, 3];
			float[,] qdint = new float[MainParameters.Instance.joints.lagrangianModel.nDDL, 3];
			float[,] qddint = new float[MainParameters.Instance.joints.lagrangianModel.nDDL, 3];
			for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.nDDL; i++)
			{
				qint[i, 0] = qFrame0[i];
				qint[i, 1] = qFrame1[i];
				qint[i, 2] = qFrame2[i];
				qdint[i, 0] = qdotFrame0[i];
				qdint[i, 1] = qdotFrame1[i];
				qdint[i, 2] = qdotFrame2[i];
				qddint[i, 0] = qddotFrame0[i];
				qddint[i, 1] = qddotFrame1[i];
				qddint[i, 2] = qddotFrame2[i];
			}

			for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.nDDL; i++)
			{
				qFrame0[i] = qFrame2[i];
				qdotFrame0[i] = qdotFrame2[i];
				qddotFrame0[i] = qddotFrame2[i];
			}

			float[] t = new float[3] { 0, timeFrame / 2, timeFrame };
			double[] k_1 = DoSimulation.ShortDynamics_int(0, xTFrame0, t, qint, qdint, qddint);

			double[] xT = new double[MainParameters.Instance.joints.lagrangianModel.nDDL * 2];
			for (int i = 0; i < xTFrame0.Length; i++)
				xT[i] = xTFrame0[i] + (timeFrame * 4.0f / 27.0f) * k_1[i];
			double[] k_2 = DoSimulation.ShortDynamics_int(timeFrame * 4.0f / 27.0f, xT, t, qint, qdint, qddint);

			for (int i = 0; i < xTFrame0.Length; i++)
				xT[i] = xTFrame0[i] + (timeFrame / 18.0f) * (k_1[i] + 3.0f * k_2[i]);
			double[] k_3 = DoSimulation.ShortDynamics_int(timeFrame * 2.0f / 9.0f, xT, t, qint, qdint, qddint);

			for (int i = 0; i < xTFrame0.Length; i++)
				xT[i] = xTFrame0[i] + (timeFrame / 12.0f) * (k_1[i] + 3.0f * k_3[i]);
			double[] k_4 = DoSimulation.ShortDynamics_int(timeFrame * 1.0f / 3.0f, xT, t, qint, qdint, qddint);

			for (int i = 0; i < xTFrame0.Length; i++)
				xT[i] = xTFrame0[i] + (timeFrame / 8.0f) * (k_1[i] + 3.0f * k_4[i]);
			double[] k_5 = DoSimulation.ShortDynamics_int(timeFrame * 1.0f / 2.0f, xT, t, qint, qdint, qddint);

			for (int i = 0; i < xTFrame0.Length; i++)
				xT[i] = xTFrame0[i] + (timeFrame / 54.0f) * (13.0f * k_1[i] - 27.0f * k_3[i] + 42.0f * k_4[i] + 8.0f * k_5[i]);
			double[] k_6 = DoSimulation.ShortDynamics_int(timeFrame * 2.0f / 3.0f, xT, t, qint, qdint, qddint);

			for (int i = 0; i < xTFrame0.Length; i++)
				xT[i] = xTFrame0[i] + (timeFrame / 4320.0f) * (389.0f * k_1[i] - 54.0f * k_3[i] + 966.0f * k_4[i] - 824.0f * k_5[i] + 243.0f * k_6[i]);
			double[] k_7 = DoSimulation.ShortDynamics_int(timeFrame * 1.0f / 6.0f, xT, t, qint, qdint, qddint);

			for (int i = 0; i < xTFrame0.Length; i++)
				xT[i] = xTFrame0[i] + (timeFrame / 20.0f) * (-234.0f * k_1[i] + 81.0f * k_3[i] - 1164.0f * k_4[i] + 656.0f * k_5[i] - 122.0f * k_6[i] + 800.0f * k_7[i]);
			double[] k_8 = DoSimulation.ShortDynamics_int(timeFrame, xT, t, qint, qdint, qddint);

			for (int i = 0; i < xTFrame0.Length; i++)
				xT[i] = xTFrame0[i] + (timeFrame / 288.0f) * (-127.0f * k_1[i] + 18.0f * k_3[i] - 678.0f * k_4[i] + 456.0f * k_5[i] - 9.0f * k_6[i] + 576.0f * k_7[i] + 4.0f * k_8[i]);
			double[] k_9 = DoSimulation.ShortDynamics_int(timeFrame * 5.0f / 6.0f, xT, t, qint, qdint, qddint);

			for (int i = 0; i < xTFrame0.Length; i++)
				xT[i] = xTFrame0[i] + (timeFrame / 820.0f) * (1481.0f * k_1[i] - 81.0f * k_3[i] + 7104.0f * k_4[i] - 3376.0f * k_5[i] + 72.0f * k_6[i] - 5040.0f * k_7[i] -
														60.0f * k_8[i] + 720.0f * k_9[i]);
			double[] k_10 = DoSimulation.ShortDynamics_int(timeFrame, xT, t, qint, qdint, qddint);

			for (int i = 0; i < xTFrame0.Length; i++)
				xTFrame1[i] = xTFrame0[i] + timeFrame / 840.0f * (41.0f * k_1[i] + 27.0f * k_4[i] + 272.0f * k_5[i] + 27.0f * k_6[i] + 216.0f * k_7[i] + 216.0f * k_9[i] + 41.0f * k_10[i]);
		}

		// Initialisation du vecteur qT et copier le vecteur t(frame) dans le vecteur t(frame - 1)

		double[] qT = new double[MainParameters.Instance.joints.lagrangianModel.nDDL];
		for (int i = 0; i < qT.Length; i++)
			qT[i] = (float)xTFrame1[i];
		for (int i = 0; i < xTFrame0.Length; i++)
			xTFrame0[i] = xTFrame1[i];

		// Conserver les temps, positions et vitesses dans les matrices globales tQ, q et qdot, pour chacun des frames

		tQ[frameN] = timeFrame * frameN;
		for (int i = 0; i < joints.lagrangianModel.nDDL; i++)
		{
			q[i, frameN] = xTFrame1[i];
			qdot[i, frameN] = xTFrame1[joints.lagrangianModel.nDDL + i];
		}

		// Mode Gesticulation: Les DDL racine doivent être à zéro

		if (playMode == MainParameters.Instance.languages.Used.animatorPlayModeGesticulation)
			for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.q1.Length; i++)
				qT[MainParameters.Instance.joints.lagrangianModel.q1[i] - 1] = 0;

		// Calcul des "tags", selon le modèle lagrangien utilisé

		float[] tagX;
		float[] tagY;
		float[] tagZ;
		EvaluateTags(qT, out tagX, out tagY, out tagZ);

		// Vérifier s'il y a un contact avec le sol, si c'est le cas, alors il faut arrêter l'animation

		if (joints.condition > 0 && tagZ.Min() < -0.05f && playMode != MainParameters.Instance.languages.Used.animatorPlayModeGesticulation)
		{
			frameContactGround = frameN;
			numberFrames = frameN + 1;
		}

		// Si le facteur de correspondance n'a pas été calculer précédemment, alors il nous faut le calculer
		// Calculer un facteur de correspondance entre le volume utilisé par la silhouette et la dimension du volume disponible pour l'affichage
		// Pour cela, il nous faut calculer les valeurs minimum et maximum des DDLs de la silhouette, dans les 3 dimensions
		// Même si on modifie la dimension de la silhouette, on conserve quand même les proportions de la sihouette dans les 3 dimensions, donc le facteur est unique pour les 3 dimensions
		// Pour le mode simulation, on ajoute une marge de 10% sur les dimensions mimimum et maximum, pour être certain que les mouvements ne dépasseront pas la dimension du panneau utilisé
		// Pour le mode gesticulation, on ajouter une marge de 25% sur les dimensions mimimum et maximum, au cas où les bras ne sont pas levé et qui le seront pas la suite.

		int newTagLength = tagX.Length;
		if (tagXMin == 0 && tagXMax == 0 && tagYMin == 0 && tagYMax == 0 && tagZMin == 0 && tagZMax == 0)
		{
			tagXMin = Mathf.Min(tagX);
			tagXMax = Mathf.Max(tagX);
			tagYMin = Mathf.Min(tagY);
			tagYMax = Mathf.Max(tagY);
			tagZMin = Mathf.Min(tagZ);
			tagZMax = Mathf.Max(tagZ);
			if (playMode == MainParameters.Instance.languages.Used.animatorPlayModeSimulation)
				AddMarginOnMinMax(0.1f);
			else
				AddMarginOnMinMax(0.25f);
			EvaluateFactorTags2Screen();
		}

		// On applique le facteur de correspondance pour optimiser l'affichage de la silhouette
		// On centre la silhouette au milieu de l'espace disponible à l'écran (PanelAnimator)

		float tagHalfMaxMinZ = (tagZMax - tagZMin) * factorTags2Screen / 2;
		for (int i = 0; i < newTagLength; i++)
		{
			tagX[i] = (tagX[i] - tagXMin) * factorTags2Screen - (tagXMax - tagXMin) * factorTags2Screen / 2;
			tagY[i] = (tagY[i] - tagYMin) * factorTags2Screen - (tagYMax - tagYMin) * factorTags2Screen / 2;
			tagZ[i] = (tagZ[i] - tagZMin) * factorTags2Screen - tagHalfMaxMinZ;
		}

		// On conserve la nouvelle matrice de "tags" sous une forme Vector3

		Vector3[] tag = new Vector3[newTagLength];
		for (int i = 0; i < newTagLength; i++)
			tag[i] = new Vector3(tagX[i], tagY[i], tagZ[i]);

		// Afficher la silhouette et le plancher si nécessaire

		for (int i = 0; i < joints.lagrangianModel.stickFigure.Length / 2; i++)
			DrawObjects.Instance.Line(lineStickFigure[i], tag[joints.lagrangianModel.stickFigure[i, 0] - 1], tag[joints.lagrangianModel.stickFigure[i, 1] - 1]);
		DrawObjects.Instance.Circle(lineCenterOfMass, 0.08f, tag[newTagLength - 1]);
		for (int i = 0; i < joints.lagrangianModel.filledFigure.Length / 4; i++)
			DrawObjects.Instance.Triangle(lineFilledFigure[i], tag[joints.lagrangianModel.filledFigure[i, 0] - 1], tag[joints.lagrangianModel.filledFigure[i, 1] - 1], tag[joints.lagrangianModel.filledFigure[i, 2] - 1]);

		if (numberFrames > 1 && playMode == MainParameters.Instance.languages.Used.animatorPlayModeSimulation)
		{
			float tagHalfMaxMinX = (tagXMax - tagXMin) * factorTags2ScreenX / 2;
			float tagHalfMaxMinY = (tagYMax - tagYMin) * factorTags2ScreenY / 2;
			float originZ = -tagZMin * factorTags2Screen - tagHalfMaxMinZ;
			DrawObjects.Instance.Line(lineFloor[0], new Vector3(-tagHalfMaxMinX, -tagHalfMaxMinY, originZ), new Vector3(tagHalfMaxMinX, -tagHalfMaxMinY, originZ));
			DrawObjects.Instance.Line(lineFloor[1], new Vector3(tagHalfMaxMinX, -tagHalfMaxMinY, originZ), new Vector3(tagHalfMaxMinX, tagHalfMaxMinY, originZ));
			DrawObjects.Instance.Line(lineFloor[2], new Vector3(tagHalfMaxMinX, tagHalfMaxMinY, originZ), new Vector3(-tagHalfMaxMinX, tagHalfMaxMinY, originZ));
			DrawObjects.Instance.Line(lineFloor[3], new Vector3(-tagHalfMaxMinX, tagHalfMaxMinY, originZ), new Vector3(-tagHalfMaxMinX, -tagHalfMaxMinY, originZ));
		}

		// Afficher le déplacement d'un curseur selon l'échelle des temps, dans le graphique qui affiche les positions des angles pour l'articulation sélectionné

		//if (numberFrames > 1)														// Ça ralenti trop l'animation, on désactive pour le moment
		//	GraphManager.Instance.DisplayCursor((firstFrame + frameN) * joints.lagrangianModel.dt);

		// Inclémenter le compteur de frames

		frameN++;
	}

    // =================================================================================================================================================================
    /// <summary> Réinitialiser certains paramètres utilisés pour l'exécution de l'animation. </summary>

    public void PlayReset()
    {
        tagXMin = 0;
        tagXMax = 0;
        tagYMin = 0;
        tagYMax = 0;
        tagZMin = 0;
        tagZMax = 0;
    }

    // =================================================================================================================================================================
    /// <summary> Actions à faire quand l'exécution de l'animation est terminé. </summary>

    public void PlayEnd()
    {
        animateON = false;
		GraphManager.Instance.mouseTracking = !MainParameters.Instance.testXSensUsed;

        // Calculer les données de rotation, et autres données nécessaire, utilisé pour les graphiques des résultats

        EvaluateRotationData();

        // Afficher un message de fin d'exécution de l'animation dans la boîte des messages

        if (numberFrames > 1)
        {
            DisplayNewMessage(false, false, string.Format(" {0} = {1:0.00} s", MainParameters.Instance.languages.Used.displayMsgSimulationDuration, timeElapsed));
            DisplayNewMessage(false, true, string.Format(" {0}", MainParameters.Instance.languages.Used.displayMsgEndSimulation));
        }

        // Afficher un message, au bas du panneau de l'animation, pour indiquer que la contact avec le sol est très rapide, si c'est le cas

        if (frameN <= 5)
            textMsg.text = MainParameters.Instance.languages.Used.animatorMsgGroundContact;

        // Enlever le bouton Stop et activer les autres contrôles du logiciel

        buttonStop.SetActive(false);
		if (!Main.Instance.testXSensUsed) Main.Instance.EnableDisableControls(true, true);
    }

    // =================================================================================================================================================================
    /// <summary> Afficher un nouveau message dans la boîte des messages. </summary>

    public void DisplayNewMessage(bool clear, bool display, string message)
    {
        if (clear) MainParameters.Instance.scrollViewMessages.Clear();
        MainParameters.Instance.scrollViewMessages.Add(message);
        if (display) textScrollViewMessages.text = string.Join(Environment.NewLine, MainParameters.Instance.scrollViewMessages.ToArray());
    }

    // =================================================================================================================================================================
    /// <summary> Calcul des "tags", selon le modèle lagrangien utilisé. </summary>

    public void EvaluateTags(double[] q, out float[] tagX, out float[] tagY, out float[] tagZ)
    {
        double[] tag1;
        if (MainParameters.Instance.joints.lagrangianModelName == MainParameters.LagrangianModelNames.Sasha23ddl)
        {
            TagsSasha23ddl tagsSasha23ddl = new TagsSasha23ddl();
            tag1 = tagsSasha23ddl.Tags(q);
        }
        else
        {
            TagsSimple tagsSimple = new TagsSimple();
            tag1 = tagsSimple.Tags(q);
        }

        int newTagLength = tag1.Length / 3;
        tagX = new float[newTagLength];
        tagY = new float[newTagLength];
        tagZ = new float[newTagLength];
        for (int i = 0; i < newTagLength; i++)
        {
            tagX[i] = (float)tag1[i];
            tagY[i] = (float)tag1[i + newTagLength];
            tagZ[i] = (float)tag1[i + newTagLength * 2];
        }
    }

	// =================================================================================================================================================================
    /// <summary> Activer ou désactiver la silhouette. </summary>

    void EnableDisableAnimationOutline(bool status)
    {
        if (lineStickFigure != null)
        {
            for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.stickFigure.Length / 2; i++)
                lineStickFigure[i].enabled = status;
        }
        if (lineCenterOfMass != null)
            lineCenterOfMass.enabled = status;
        if (lineFilledFigure != null)
        {
            for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.filledFigure.Length / 4; i++)
                lineFilledFigure[i].enabled = status;
        }
        if (lineFloor != null)
        {
            for (int i = 0; i < 4; i++)
                lineFloor[i].enabled = status;
        }
    }

    // =================================================================================================================================================================
    /// <summary> Ajouter une marge de sécurité sur les dimensions mimimum et maximum, pour être certain que les mouvements ne dépasseront pas la dimension du panneau utilisé. </summary>

    public void AddMarginOnMinMax(float factor)
    {
        float margin;

        margin = (tagXMax - tagXMin) * factor;
        tagXMin -= margin;
        tagXMax += margin;

        margin = (tagYMax - tagYMin) * factor;
        tagYMin -= margin;
        tagYMax += margin;

        margin = (tagZMax - tagZMin) * factor;
        tagZMin -= margin;
        tagZMax += margin;
    }

    // =================================================================================================================================================================
    /// <summary> Calcul du facteur de correspondance entre la dimension du volume des Tags et la dimension du volume disponible à l'écran. </summary>

    public void EvaluateFactorTags2Screen()
    {
        // Calcul du facteur de correspondance.

        factorTags2ScreenX = animationMaxDimOnScreen / (tagXMax - tagXMin);
        factorTags2ScreenY = animationMaxDimOnScreen / (tagYMax - tagYMin);
        if (tagXMax - tagXMin > tagYMax - tagYMin && tagXMax - tagXMin > tagZMax - tagZMin)
            factorTags2Screen = factorTags2ScreenX;
        else if (tagYMax - tagYMin > tagZMax - tagZMin)
            factorTags2Screen = factorTags2ScreenY;
        else
            factorTags2Screen = animationMaxDimOnScreen / (tagZMax - tagZMin);
    }

    // =================================================================================================================================================================
    /// <summary> Calculer les données de rotation, et autres données nécessaire, utilisé pour les graphiques des résultats. </summary>

    void EvaluateRotationData()
    {
        int numF = numberFrames;
        if (frameContactGround < numberFrames)
        {
            MainParameters.Instance.joints.tc = (float)tQ[frameContactGround];
            AnimationF.Instance.DisplayNewMessage(false, true, string.Format(" {0} {1:0.00} s", MainParameters.Instance.languages.Used.displayMsgContactGround, MainParameters.Instance.joints.tc));
            numF = frameContactGround + 1;
        }

        MainParameters.Instance.joints.t = new float[numF];
        float[,] q1 = new float[MainParameters.Instance.joints.lagrangianModel.nDDL, numF];
        float[,] qdot1 = new float[MainParameters.Instance.joints.lagrangianModel.nDDL, numF];
        for (int i = 0; i < numF; i++)
        {
            MainParameters.Instance.joints.t[i] = (float)tQ[i];
            for (int j = 0; j < MainParameters.Instance.joints.lagrangianModel.nDDL; j++)
            {
                q1[j, i] = (float)q[j, i];
                qdot1[j, i] = (float)qdot[j, i];
            }
        }

        int[] rotation = new int[3] { MainParameters.Instance.joints.lagrangianModel.root_somersault, MainParameters.Instance.joints.lagrangianModel.root_tilt,
            MainParameters.Instance.joints.lagrangianModel.root_twist };
        for (int i = 0; i < rotation.Length; i++) rotation[i] = Math.Abs(rotation[i]);
        MainParameters.Instance.joints.rot = new float[numF, rotation.Length];
        MainParameters.Instance.joints.rotdot = new float[numF, rotation.Length];
        float[,] rotAbs = new float[numF, rotation.Length];
        for (int i = 0; i < rotation.Length; i++)
        {
            float[] rotCol = new float[numF];
            float[] rotdotCol = new float[numF];
            rotCol = MathFunc.unwrap(MathFunc.MatrixGetRow(q1, rotation[i] - 1));
            rotdotCol = MathFunc.unwrap(MathFunc.MatrixGetRow(qdot1, rotation[i] - 1));
            for (int j = 0; j < numF; j++)
            {
                MainParameters.Instance.joints.rot[j, i] = rotCol[j] / (2 * (float)Math.PI);
                MainParameters.Instance.joints.rotdot[j, i] = rotdotCol[j] / (2 * (float)Math.PI);
                rotAbs[j, i] = Math.Abs(MainParameters.Instance.joints.rot[j, i]);
            }
        }

        float numSomersault = MathFunc.MatrixGetColumn(rotAbs, 0).Max() + MainParameters.Instance.joints.takeOffParam.rotation / 360;
        AnimationF.Instance.DisplayNewMessage(false, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgNumberSomersaults, numSomersault));
        AnimationF.Instance.DisplayNewMessage(false, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgNumberTwists, MathFunc.MatrixGetColumn(rotAbs, 2).Max()));
        AnimationF.Instance.DisplayNewMessage(false, true, string.Format(" {0} = {1:0.00}",
            MainParameters.Instance.languages.Used.displayMsgFinalTwist, MainParameters.Instance.joints.rot[numF - 1, 2]));
        AnimationF.Instance.DisplayNewMessage(false, true, string.Format(" {0} = {1:0}°", MainParameters.Instance.languages.Used.displayMsgMaxTilt, MathFunc.MatrixGetColumn(rotAbs, 1).Max() * 360));
        AnimationF.Instance.DisplayNewMessage(false, true, string.Format(" {0} = {1:0}°",
            MainParameters.Instance.languages.Used.displayMsgFinalTilt, MainParameters.Instance.joints.rot[numF - 1, 1] * 360));
    }

	// =================================================================================================================================================================
	/// <summary> Initialisation de la dimension du volume utilisé à l'écran. </summary>
	// Calculer un facteur de correspondance entre le volume utilisé par la silhouette et la dimension du volume disponible pour l'affichage
	// Pour cela, il nous faut calculer les valeurs minimum et maximum des DDLs de la silhouette, dans les 3 dimensions
	// Même si on modifie la dimension de la silhouette, on conserve quand même les proportions de la sihouette dans les 3 dimensions, donc le facteur est unique pour les 3 dimensions
	// On ajoute une marge de 10% sur les dimensions mimimum et maximum, pour être certain que les mouvements ne dépasseront pas la dimension du panneau utilisé

	public void InitScreenVolumeDimension(float[,] q1)
	{
		float[] tagX, tagY, tagZ;
		tagXMin = tagYMin = tagZMin = 9999;
		tagXMax = tagYMax = tagZMax = -9999;
		double[] qf = new double[q1.GetUpperBound(0) + 1];
		for (int i = 0; i <= q1.GetUpperBound(1); i++)
		{
			qf = MathFunc.MatrixGetColumnD(q1, i);
			if (playMode == MainParameters.Instance.languages.Used.animatorPlayModeGesticulation)       // Mode Gesticulation: Les DDL racine doivent être à zéro
				for (int j = 0; j < MainParameters.Instance.joints.lagrangianModel.q1.Length; j++)
					qf[MainParameters.Instance.joints.lagrangianModel.q1[j] - 1] = 0;

			EvaluateTags(qf, out tagX, out tagY, out tagZ);
			tagXMin = Math.Min(tagXMin, Mathf.Min(tagX));
			tagXMax = Math.Max(tagXMax, Mathf.Max(tagX));
			tagYMin = Math.Min(tagYMin, Mathf.Min(tagY));
			tagYMax = Math.Max(tagYMax, Mathf.Max(tagY));
			tagZMin = Math.Min(tagZMin, Mathf.Min(tagZ));
			tagZMax = Math.Max(tagZMax, Mathf.Max(tagZ));
		}
		AddMarginOnMinMax(0.1f);
		EvaluateFactorTags2Screen();
	}
}
