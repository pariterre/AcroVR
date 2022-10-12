using UnityEngine;
using System;
using System.Linq;
using System.Text;
using Microsoft.Research.Oslo;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class DrawManager : MonoBehaviour
{
    const string dllpath = "biorbd_c.dll";
    [DllImport(dllpath)] static extern IntPtr c_biorbdModel(StringBuilder pathToModel);
    [DllImport(dllpath)] static extern int c_nQ(IntPtr model);
    [DllImport(dllpath)] static extern void c_massMatrix(IntPtr model, IntPtr q, IntPtr massMatrix);
    [DllImport(dllpath)] static extern void c_inverseDynamics(IntPtr model, IntPtr q, IntPtr qdot, IntPtr qddot, IntPtr tau);
    [DllImport(dllpath)] static extern void c_solveLinearSystem(IntPtr matA, int nbCol, int nbLigne, IntPtr matB, IntPtr solX);

    protected AvatarManager avatarManager;
    protected StatManager statManager;
    protected UIManager uiManager;

    protected SliderPlayAnimation sliderAnimation;
    protected DisplayResultGraphicS resultGraphics;

    public GameObject girl2;
    GameObject girl2Prefab;

    public float InitialFeetHeight { get; protected set; } = (float)Double.NaN;
    // Hip
    private GameObject girl2LeftThigh;
    private GameObject girl2RightThigh;
    // Knee
    private GameObject girl2LeftLeg;
    private GameObject girl2RightLeg;
    // Shoulder
    private GameObject girl2LeftArm;
    private GameObject girl2RightArm;
    // Root
    private GameObject girl2Hip;

    private GameObject firstView;
    public bool canResumeAnimation { get; protected set; } = false;
    public void SetCanResumeAnimation(bool value) { canResumeAnimation = value; }

    public float[,] AllQ { get; protected set; }
    float[,] q_girl2;
    double[] qf_girl2;
    public float frameRate { get; } = 0.02f;
    public int frameN { get; protected set; } = 0;
    public void SetFrameN(int value) { 
        frameN = value;
        if (sliderAnimation) sliderAnimation.SetSlider(frameN);
    }
    public float frameNtime { get { return frameN * frameRate; } }
    public int secondFrameN { get; protected set; } = 0;
    public void SetSecondFrameN(int value) { secondFrameN = value; }
    public float secondFrameNtime { get { return secondFrameN * frameRate; } }
    int firstFrame = 0;
    internal int numberFrames = 0;
    public float timeElapsed = 0;
    public float timeFrame = 0;
    float timeStarted = 0;
    float factorPlaySpeed = 1f;

    string playMode = MainParameters.Instance.languages.Used.animatorPlayModeSimulation;

    protected float[,] q1;
    float[,] q1_girl2;

    public bool isPaused { get; protected set; } = true;
    public void Pause() { isPaused = true; secondPaused = false; }
    public void Resume(){ isPaused = false; secondPaused = false; }
    public bool IsEditing { get; protected set; } = false;

    float pauseStart = 0;
    float pauseTime = 0;

    protected int CurrentVizualisationMode = 0;
    public bool IsSimulationMode { get { return CurrentVizualisationMode == 0; } }
    public void ActivateSimulationMode() { CurrentVizualisationMode = 0; }
    public bool IsGestureMode { get { return CurrentVizualisationMode == 1; } }
    public void ActivateGestureMode() { CurrentVizualisationMode = 1; }

    GameObject Ground;
    public void SetGround(GameObject _floor) { Ground = _floor; }

    public AvatarSimulation secondParameters = new AvatarSimulation();

    public int secondNumberFrames = 0;
    public bool secondPaused = false;
    public List<string> secondResultMessages = new List<string>();


    void Start()
    {
        avatarManager = ToolBox.GetInstance().GetManager<AvatarManager>();
        statManager = ToolBox.GetInstance().GetManager<StatManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();

        uiManager.UpdateAllPropertiesFromDropdown();
    }

    public void RegisterResultShow(DisplayResultGraphicS _newResultGraphics)
    {
        resultGraphics = _newResultGraphics;
    }

    public void UnregisterResultShow()
    {
        resultGraphics = null;
    }

    public void RegisterSliderAnimation(SliderPlayAnimation _newSliderAnimation)
    {
        sliderAnimation = _newSliderAnimation;
    }

    public void UnregisterSliderAnimation()
    {
        sliderAnimation = null;
    }

    void Update()
    {
        if (isPaused && pauseStart == 0) 
            pauseStart = Time.time;

        if (!isPaused && pauseStart != 0)
        {
            pauseTime = Time.time - pauseStart;
            pauseStart = 0;
        }

        if (frameN <= 0 && !isPaused) timeStarted = Time.time;
        if (Time.time - (timeStarted + pauseTime) >= (timeFrame * frameN) * factorPlaySpeed)
        {
            if(!isPaused)
                timeElapsed = Time.time - (timeStarted + pauseTime);

            if (ShouldContinuePlaying())
                PlayOneFrame();
            else
                PlayEnd();
        }

        if (Time.time - (timeStarted + pauseTime) >= (timeFrame * secondFrameN) * factorPlaySpeed)
        {
            if (secondFrameN < secondNumberFrames-1)
            {
                PlayOneFrameForSecond();
            }
            else
                secondPaused = true;
        }
    }

    public bool ShouldContinuePlaying()
    {
        if (frameN >= numberFrames - 1) return false;

        if (
            frameN != 0 
            && MainParameters.Instance.joints.StopOnGround && 
            !IsGestureMode && 
            avatarManager.FeetHeight() < InitialFeetHeight
        ) 
            return false;
        
        return true;
    }

    public void UpdateFullKinematics(bool restartToZero)
    {
        MakeSimulationFrame();
        Play_s(q1, 0, q1.GetUpperBound(1) + 1, restartToZero);   
    }

    public void MakeSimulationFrame()
    {
        if (MainParameters.Instance.joints.nodes == null) return;
        q1 = MakeSimulation();
        if(avatarManager.NumberOfLoadedAvatars > 1)
        {
            q1_girl2 = MakeSimulationSecond();
            q_girl2 = MathFunc.MatrixCopy(q1_girl2);
        }
    }

    public void ForceFullUpdate()
    {
        var _currentFrame = frameN;
        ShowAvatar();
        ShowGround();
        PlayOneFrame();
        SetFrameN(_currentFrame);
    }

    public void ShowAvatar()
    {
        MakeSimulationFrame();
        if (MainParameters.Instance.joints.nodes == null) return;
        CenterAvatar(0);

        Play_s(q1, 0, q1.GetUpperBound(1) + 1, true);

        if (avatarManager.NumberOfLoadedAvatars > 1)
        {
            girl2.transform.rotation = Quaternion.identity;
            girl2.transform.position = Vector3.zero;
            CenterAvatar(1);

            secondNumberFrames = q1_girl2.GetUpperBound(1) + 1;
        }
    }

    public void CenterAvatar(int _index)
    {
        var _model = avatarManager.LoadedModels[_index];
        Vector3 _scaling = _model.gameObject.transform.localScale;
        var _hipTranslations = Double.IsNaN(InitialFeetHeight) ? new Vector3(0f, 0f, 0f) : new Vector3(0f, -InitialFeetHeight * _scaling.y, 0f);
        var _hipRotations = new Vector3(0f, 0f, 0f);
        if (IsSimulationMode && avatarManager.Q != null)
        {
            var _q = avatarManager.Q;
            _hipTranslations += new Vector3((float)_q[6] * _scaling.x, (float)_q[8] * _scaling.y, (float)_q[7] * _scaling.z);
            _hipRotations += new Vector3((float)_q[9] * Mathf.Rad2Deg, (float)_q[10] * Mathf.Rad2Deg, (float)_q[11] * Mathf.Rad2Deg);
        }
        _model.Hip.transform.localPosition = _hipTranslations;
        _model.Hip.transform.localEulerAngles = _hipRotations;
    }

    public void ShowGround(){
        if (Ground != null)
            Ground.SetActive(MainParameters.Instance.joints.StopOnGround);
    }

    public void SetAnimationSpeed(float speed)
    {
        factorPlaySpeed = speed;
    }

    public void SetFirstView(GameObject _view)
    {
        firstView = _view;
    }
    public GameObject GetFirstViewTransform()
    {
        return firstView;
    }

    public void PlayAvatar()
    {
        if (MainParameters.Instance.joints.nodes == null) return;
        ShowAvatar();
        Resume();
        canResumeAnimation = true;
    }

    public void PlayEnd()
    {
        Pause();
    }

    private void DisplayNewMessage(bool clear, bool display, string message)
    {
        if (clear) MainParameters.Instance.scrollViewMessages.Clear();
        MainParameters.Instance.scrollViewMessages.Add(message);
    }

    private void AddsecondMessage(bool clear, bool display, string message)
    {
        if (clear) secondResultMessages.Clear();
        secondResultMessages.Add(message);
    }

    public string DisplayMessage()
    {
        return string.Join(Environment.NewLine, MainParameters.Instance.scrollViewMessages.ToArray());
    }

    public string DisplayMessageSecond()
    {
        return string.Join(Environment.NewLine, secondResultMessages.ToArray());
    }

    protected void Play_s(float[,] qq, int frFrame, int nFrames, bool restartToZero)
    {
        MainParameters.StrucJoints joints = MainParameters.Instance.joints;

        AllQ = MathFunc.MatrixCopy(qq);
        if (restartToZero)
            SetFrameN(0);
        firstFrame = frFrame;
        numberFrames = nFrames;

        timeElapsed = 0;

        pauseTime = 0;
        pauseStart = 0;

        if (nFrames > 1)
        {
            if (joints.tc > 0)                          // Il y a eu contact avec le sol, alors seulement une partie des données sont utilisé
                timeFrame = joints.tc / (numberFrames - 1);
            else                                        // Aucun contact avec le sol, alors toutes les données sont utilisé
                timeFrame = joints.Duration / (numberFrames - 1);
        }
        else
            timeFrame = 0;

        if (avatarManager.NumberOfLoadedAvatars > 1)
        {
            secondFrameN = 0;
        }
    }

    private void Quintic_s(float t, float ti, float tj, float qi, float qj, out float p, out float v, out float a)
    {
        if (t < ti)
            t = ti;
        else if (t > tj)
            t = tj;
        float tp0 = tj - ti;
        float tp1 = t - ti;
        float tp2 = tp1 / tp0;
        float tp3 = tp2 * tp2;
        float tp4 = tp3 * tp2 * (6 * tp3 - 15 * tp2 + 10);
        float tp5 = qj - qi;
        float tp6 = tj - t;
        float tp7 = Mathf.Pow(tp0, 5);
        p = qi + tp5 * tp4;
        v = 30 * tp5 * tp1 * tp1 * tp6 * tp6 / tp7;
        a = 60 * tp5 * tp1 * tp6 * (tj + ti - 2 * t) / tp7;
    }

    public void Trajectory_s(LagrangianModelManager.StrucLagrangianModel lagrangianModel, float t, int[] qi, out float[] qd, out float[] qdotd, out float[] qddotd)
    {
        qd = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
        qdotd = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
        qddotd = new float[MainParameters.Instance.joints.lagrangianModel.nDDL];
        for (int i = 0; i < qd.Length; i++)
        {
            qd[i] = 0;
            qdotd[i] = 0;
            qddotd[i] = 0;
        }

        int n = qi.Length;

        // n=6, 6Node (HipFlexion, KneeFlexion ...)
        for (int i = 0; i < MainParameters.Instance.joints.nodes.Length; i++)
        {
            int ii = qi[i] - lagrangianModel.q2[0];
            MainParameters.StrucNodes nodes = MainParameters.Instance.joints.nodes[ii];

            int j = 1;
            while (j < nodes.T.Length - 1 && t > nodes.T[j]) j++;
            Quintic_s(t, nodes.T[j - 1], nodes.T[j], nodes.Q[j - 1], nodes.Q[j], out qd[ii], out qdotd[ii], out qddotd[ii]);
        }
    }

    public void TrajectorySecond(LagrangianModelManager.StrucLagrangianModel lagrangianModel, float t, int[] qi, out float[] qd, out float[] qdotd, out float[] qddotd)
    {
        qd = new float[secondParameters.joints.lagrangianModel.nDDL];
        qdotd = new float[secondParameters.joints.lagrangianModel.nDDL];
        qddotd = new float[secondParameters.joints.lagrangianModel.nDDL];
        for (int i = 0; i < qd.Length; i++)
        {
            qd[i] = 0;
            qdotd[i] = 0;
            qddotd[i] = 0;
        }

        int n = qi.Length;

        // n=6, 6Node (HipFlexion, KneeFlexion ...)
        for (int i = 0; i < secondParameters.joints.nodes.Length; i++)
        {
            int ii = qi[i] - lagrangianModel.q2[0];
            AvatarSimulation.StrucNodes nodes = secondParameters.joints.nodes[ii];
            int j = 1;
            while (j < nodes.T.Length - 1 && t > nodes.T[j]) j++;
            Quintic_s(t, nodes.T[j - 1], nodes.T[j], nodes.Q[j - 1], nodes.Q[j], out qd[ii], out qdotd[ii], out qddotd[ii]);
        }
    }

    private float[,] MakeSimulation()
    {
        if (MainParameters.Instance.joints.nodes == null) return new float[0,0];

        MainParameters.StrucJoints joints = MainParameters.Instance.joints;
        float[] q0 = new float[joints.lagrangianModel.nDDL];
        float[] q0dot = new float[joints.lagrangianModel.nDDL];
        if (Double.IsNaN(InitialFeetHeight)){
            InitialFeetHeight = avatarManager.FeetHeight(q0);
        }

        for (int i = 0; i < MainParameters.Instance.joints.nodes.Length; i++)
        {
            MainParameters.StrucNodes nodes = MainParameters.Instance.joints.nodes[i];
            q0[i] = nodes.Q[0];
        }

        // Beginning Pose
        int[] rotation = new int[3] { joints.lagrangianModel.root_somersault, joints.lagrangianModel.root_tilt, joints.lagrangianModel.root_twist };
        int[] rotationSign = MathFunc.Sign(rotation);
        for (int i = 0; i < rotation.Length; i++) rotation[i] = Math.Abs(rotation[i]);

        int[] translation = new int[3] { joints.lagrangianModel.root_right, joints.lagrangianModel.root_foreward, joints.lagrangianModel.root_upward };
        int[] translationS = MathFunc.Sign(translation);
        for (int i = 0; i < translation.Length; i++) translation[i] = Math.Abs(translation[i]);

        float rotRadians = joints.takeOffParam.Somersault * (float)Math.PI / 180;

        float tilt = joints.takeOffParam.Tilt;
        if (tilt == 90)
            tilt = 90.001f;
        else if (tilt == -90)
            tilt = -90.01f;

        // q0[12]
        // q0[9] = somersault
        // q0[10] = tilt
        q0[Math.Abs(joints.lagrangianModel.root_tilt) - 1] = tilt * (float)Math.PI / 180; 
        q0[Math.Abs(joints.lagrangianModel.root_somersault) - 1] = rotRadians; 

        //q0dot[12]
        //q0dot[7] = AnteroposteriorSpeed
        //q0dot[8] = verticalSpeed
        //q0dot[9] = somersaultSpeed
        //q0dot[11] = twistSpeed
        q0dot[Math.Abs(joints.lagrangianModel.root_foreward) - 1] = joints.takeOffParam.HorizontalSpeed;                       // m/s
        q0dot[Math.Abs(joints.lagrangianModel.root_upward) - 1] = joints.takeOffParam.VerticalSpeed;                                // m/s
        q0dot[Math.Abs(joints.lagrangianModel.root_somersault) - 1] = joints.takeOffParam.SomersaultSpeed * 2 * (float)Math.PI;     // radians/s
        q0dot[Math.Abs(joints.lagrangianModel.root_twist) - 1] = joints.takeOffParam.TwistSpeed * 2 * (float)Math.PI;               // radians/s


        // q0[11] = twist
        // q0dot[10] = tiltSpeed
        q0[Math.Abs(joints.lagrangianModel.root_twist) - 1] = joints.takeOffParam.Twist * (float)Math.PI / 180;
        q0dot[Math.Abs(joints.lagrangianModel.root_tilt) - 1] = joints.takeOffParam.TiltSpeed * 2 * (float)Math.PI;


        double[] Q = new double[joints.lagrangianModel.nDDL];
        for (int i = 0; i < joints.lagrangianModel.nDDL; i++)
            Q[i] = q0[i];
        avatarManager.EvaluateTags(Q, out float[] tagX, out float[] tagY, out float[] tagZ);

        // Q[12]
        // tagX[26], tagY[26], tagZ[26]

        //the last one = Center of Mass
        float[] cg = new float[3];
        cg[0] = tagX[tagX.Length - 1];
        cg[1] = tagY[tagX.Length - 1];
        cg[2] = tagZ[tagX.Length - 1];

        float[] u1 = new float[3];
        float[,] rot = new float[3, 1];
        for (int i = 0; i < 3; i++)
        {
            u1[i] = cg[i] - q0[translation[i] - 1] * translationS[i];
            rot[i, 0] = q0dot[rotation[i] - 1] * rotationSign[i];
        }
        float[,] u = { { 0, -u1[2], u1[1] }, { u1[2], 0, -u1[0] }, { -u1[1], u1[0], 0 } };
        float[,] rotM = MathFunc.MatrixMultiply(u, rot);
        for (int i = 0; i < 3; i++)
        {
            q0dot[translation[i] - 1] = q0dot[translation[i] - 1] * translationS[i] + rotM[i, 0];
            q0dot[translation[i] - 1] = q0dot[translation[i] - 1] * translationS[i];
        }

        q0[Math.Abs(joints.lagrangianModel.root_foreward) - 1] += joints.takeOffParam.HorizontalPosition;
        q0[Math.Abs(joints.lagrangianModel.root_upward) - 1] += joints.takeOffParam.VerticalPosition;

        double[] x0 = new double[joints.lagrangianModel.nDDL * 2];
        for (int i = 0; i < joints.lagrangianModel.nDDL; i++)
        {
            x0[i] = q0[i];
            x0[joints.lagrangianModel.nDDL + i] = q0dot[i];
        }

        // x0[24]

        Options options = new Options();
        options.InitialStep = joints.lagrangianModel.dt;
        var sol = Ode.RK547M(0, joints.Duration + joints.lagrangianModel.dt, new Vector(x0), ShortDynamics_s, options);

        var points = sol.SolveFromToStep(0, joints.Duration + joints.lagrangianModel.dt, joints.lagrangianModel.dt).ToArray();

        // test0 = point[51]
        // test1 = point[251]
        double[] t = new double[points.GetUpperBound(0) + 1];
        double[,] q = new double[joints.lagrangianModel.nDDL, points.GetUpperBound(0) + 1];
        double[,] qdot = new double[joints.lagrangianModel.nDDL, points.GetUpperBound(0) + 1];
        for (int i = 0; i < joints.lagrangianModel.nDDL; i++)
        {
            for (int j = 0; j <= points.GetUpperBound(0); j++)
            {
                if (i <= 0)
                    t[j] = points[j].T;

                q[i, j] = points[j].X[i];
                qdot[i, j] = points[j].X[joints.lagrangianModel.nDDL + i];
            }
        }

        // test0 = t[51], q[12,51], qdot[12,51]
        // test1 = t[251], q[12,251], qdot[12,251]
        int tIndex = 0;
        MainParameters.Instance.joints.tc = 0;
        for (int i = 0; i <= q.GetUpperBound(1); i++)
        {
            tIndex++;
            double[] qq = new double[joints.lagrangianModel.nDDL];
            for (int j = 0; j < joints.lagrangianModel.nDDL; j++)
                qq[j] = q[j, i];
            avatarManager.EvaluateTags(qq, out tagX, out tagY, out tagZ);

            // Cut the trial when the feet crosses the ground (vertical axis = 0)
            if (
                  !IsGestureMode && i > 0 
                  && MainParameters.Instance.joints.StopOnGround 
                  && MainParameters.Instance.joints.UseGravity 
                  && tagZ.Min() < InitialFeetHeight
            )
            {
                MainParameters.Instance.joints.tc = (float)t[i];
                break;
            }
        }

        MainParameters.Instance.joints.t = new float[tIndex];
        float[,] qOut = new float[joints.lagrangianModel.nDDL, tIndex];
        float[,] qdot1 = new float[joints.lagrangianModel.nDDL, tIndex];
        for (int i = 0; i < tIndex; i++)
        {
            MainParameters.Instance.joints.t[i] = (float)t[i];
            for (int j = 0; j < joints.lagrangianModel.nDDL; j++)
            {
                qOut[j, i] = (float)q[j, i];
                qdot1[j, i] = (float)qdot[j, i];
            }
        }

        MainParameters.Instance.joints.rot = new float[tIndex, rotation.Length];
        MainParameters.Instance.joints.rotdot = new float[tIndex, rotation.Length];
        float[,] rotAbs = new float[tIndex, rotation.Length];
        for (int i = 0; i < rotation.Length; i++)
        {
            float[] rotCol = new float[tIndex];
            float[] rotdotCol = new float[tIndex];
            rotCol = MathFunc.unwrap(MathFunc.MatrixGetRow(qOut, rotation[i] - 1));
            rotdotCol = MathFunc.unwrap(MathFunc.MatrixGetRow(qdot1, rotation[i] - 1));
            for (int j = 0; j < tIndex; j++)
            {
                MainParameters.Instance.joints.rot[j, i] = rotCol[j] / (2 * (float)Math.PI);
                MainParameters.Instance.joints.rotdot[j, i] = rotdotCol[j] / (2 * (float)Math.PI);
                rotAbs[j, i] = Math.Abs(MainParameters.Instance.joints.rot[j, i]);
            }
        }

        float numSomersault = MathFunc.MatrixGetColumn(rotAbs, 0).Max() + MainParameters.Instance.joints.takeOffParam.Somersault / 360;
        DisplayNewMessage(true, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgNumberSomersaults, numSomersault));
        DisplayNewMessage(false, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgNumberTwists, MathFunc.MatrixGetColumn(rotAbs, 2).Max()));
        DisplayNewMessage(false, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgFinalTwist, MainParameters.Instance.joints.rot[tIndex - 1, 2]));
        DisplayNewMessage(false, true, string.Format(" {0} = {1:0}°", MainParameters.Instance.languages.Used.displayMsgMaxTilt, MathFunc.MatrixGetColumn(rotAbs, 1).Max() * 360));
        DisplayNewMessage(false, true, string.Format(" {0} = {1:0}°", MainParameters.Instance.languages.Used.displayMsgFinalTilt, MainParameters.Instance.joints.rot[tIndex - 1, 1] * 360));

        return qOut;
    }

    private float[,] MakeSimulationSecond()
    {
        if (secondParameters.joints.nodes == null) return new float[0, 0];

        AvatarSimulation.StrucJoints joints = secondParameters.joints;
        float[] q0 = new float[joints.lagrangianModel.nDDL];
        float[] q0dot = new float[joints.lagrangianModel.nDDL];

        for (int i = 0; i < secondParameters.joints.nodes.Length; i++)
        {
            AvatarSimulation.StrucNodes nodes = secondParameters.joints.nodes[i];
            q0[i] = nodes.Q[0];
        }

        int[] rotation = new int[3] { joints.lagrangianModel.root_somersault, joints.lagrangianModel.root_tilt, joints.lagrangianModel.root_twist };
        int[] rotationS = MathFunc.Sign(rotation);
        for (int i = 0; i < rotation.Length; i++) rotation[i] = Math.Abs(rotation[i]);

        int[] translation = new int[3] { joints.lagrangianModel.root_right, joints.lagrangianModel.root_foreward, joints.lagrangianModel.root_upward };
        int[] translationS = MathFunc.Sign(translation);
        for (int i = 0; i < translation.Length; i++) translation[i] = Math.Abs(translation[i]);

        float rotRadians = joints.takeOffParam.Somersault * (float)Math.PI / 180;

        float tilt = joints.takeOffParam.Tilt;
        if (tilt == 90)
            tilt = 90.001f;
        else if (tilt == -90)
            tilt = -90.01f;

        q0[Math.Abs(joints.lagrangianModel.root_tilt) - 1] = tilt * (float)Math.PI / 180;                                        // en radians
        q0[Math.Abs(joints.lagrangianModel.root_somersault) - 1] = rotRadians;                                         // en radians

        q0dot[Math.Abs(joints.lagrangianModel.root_foreward) - 1] = joints.takeOffParam.HorizontalSpeed;                       // en m/s
        q0dot[Math.Abs(joints.lagrangianModel.root_upward) - 1] = joints.takeOffParam.VerticalSpeed;                                // en m/s
        q0dot[Math.Abs(joints.lagrangianModel.root_somersault) - 1] = joints.takeOffParam.SomersaultSpeed * 2 * (float)Math.PI;     // en radians/s
        q0dot[Math.Abs(joints.lagrangianModel.root_twist) - 1] = joints.takeOffParam.TwistSpeed * 2 * (float)Math.PI;               // en radians/s

        double[] Q = new double[joints.lagrangianModel.nDDL];
        for (int i = 0; i < joints.lagrangianModel.nDDL; i++)
            Q[i] = q0[i];
        float[] tagX;
        float[] tagY;
        float[] tagZ;
        avatarManager.EvaluateTags(Q, out tagX, out tagY, out tagZ);

        float[] cg = new float[3];
        cg[0] = tagX[tagX.Length - 1];
        cg[1] = tagY[tagX.Length - 1];
        cg[2] = tagZ[tagX.Length - 1];

        float[] u1 = new float[3];
        float[,] rot = new float[3, 1];
        for (int i = 0; i < 3; i++)
        {
            u1[i] = cg[i] - q0[translation[i] - 1] * translationS[i];
            rot[i, 0] = q0dot[rotation[i] - 1] * rotationS[i];
        }
        float[,] u = { { 0, -u1[2], u1[1] }, { u1[2], 0, -u1[0] }, { -u1[1], u1[0], 0 } };
        float[,] rotM = MathFunc.MatrixMultiply(u, rot);
        for (int i = 0; i < 3; i++)
        {
            q0dot[translation[i] - 1] = q0dot[translation[i] - 1] * translationS[i] + rotM[i, 0];
            q0dot[translation[i] - 1] = q0dot[translation[i] - 1] * translationS[i];
        }

        float hFeet = Math.Min(tagZ[joints.lagrangianModel.feet[0] - 1], tagZ[joints.lagrangianModel.feet[1] - 1]);
        float hHand = Math.Min(tagZ[joints.lagrangianModel.hand[0] - 1], tagZ[joints.lagrangianModel.hand[1] - 1]);

        if (Math.Cos(rotRadians) > 0)
            q0[Math.Abs(joints.lagrangianModel.root_upward) - 1] += joints.lagrangianModel.hauteurs[joints.condition] - hFeet;
        else                                                            // bars, vault and tumbling from hands
            q0[Math.Abs(joints.lagrangianModel.root_upward) - 1] += joints.lagrangianModel.hauteurs[joints.condition] - hHand;

        double[] x0 = new double[joints.lagrangianModel.nDDL * 2];
        for (int i = 0; i < joints.lagrangianModel.nDDL; i++)
        {
            x0[i] = q0[i];
            x0[joints.lagrangianModel.nDDL + i] = q0dot[i];
        }

        Options options = new Options();
        options.InitialStep = joints.lagrangianModel.dt;

        var sol = Ode.RK547M(0, joints.Duration + joints.lagrangianModel.dt, new Vector(x0), ShortDynamicsSecond, options);
        var points = sol.SolveFromToStep(0, joints.Duration + joints.lagrangianModel.dt, joints.lagrangianModel.dt).ToArray();

        double[] t = new double[points.GetUpperBound(0) + 1];
        double[,] q = new double[joints.lagrangianModel.nDDL, points.GetUpperBound(0) + 1];
        double[,] qdot = new double[joints.lagrangianModel.nDDL, points.GetUpperBound(0) + 1];
        for (int i = 0; i < joints.lagrangianModel.nDDL; i++)
        {
            for (int j = 0; j <= points.GetUpperBound(0); j++)
            {
                if (i <= 0)
                    t[j] = points[j].T;

                q[i, j] = points[j].X[i];
                qdot[i, j] = points[j].X[joints.lagrangianModel.nDDL + i];
            }
        }

        int tIndex = 0;
        secondParameters.joints.tc = 0;
        for (int i = 0; i <= q.GetUpperBound(1); i++)
        {
            tIndex++;
            double[] qq = new double[joints.lagrangianModel.nDDL];
            for (int j = 0; j < joints.lagrangianModel.nDDL; j++)
                qq[j] = q[j, i];
            avatarManager.EvaluateTags(qq, out tagX, out tagY, out tagZ);
            if (MainParameters.Instance.joints.UseGravity && tagZ.Min() < -0.05f)
            {
                secondParameters.joints.tc = (float)t[i];
                break;
            }
        }

        secondParameters.joints.t = new float[tIndex];
        float[,] qOut = new float[joints.lagrangianModel.nDDL, tIndex];
        float[,] qdot1 = new float[joints.lagrangianModel.nDDL, tIndex];
        for (int i = 0; i < tIndex; i++)
        {
            secondParameters.joints.t[i] = (float)t[i];
            for (int j = 0; j < joints.lagrangianModel.nDDL; j++)
            {
                qOut[j, i] = (float)q[j, i];
                qdot1[j, i] = (float)qdot[j, i];
            }
        }

        secondParameters.joints.rot = new float[tIndex, rotation.Length];
        secondParameters.joints.rotdot = new float[tIndex, rotation.Length];
        float[,] rotAbs = new float[tIndex, rotation.Length];
        for (int i = 0; i < rotation.Length; i++)
        {
            float[] rotCol = new float[tIndex];
            float[] rotdotCol = new float[tIndex];
            rotCol = MathFunc.unwrap(MathFunc.MatrixGetRow(qOut, rotation[i] - 1));
            rotdotCol = MathFunc.unwrap(MathFunc.MatrixGetRow(qdot1, rotation[i] - 1));
            for (int j = 0; j < tIndex; j++)
            {
                secondParameters.joints.rot[j, i] = rotCol[j] / (2 * (float)Math.PI);
                secondParameters.joints.rotdot[j, i] = rotdotCol[j] / (2 * (float)Math.PI);
                rotAbs[j, i] = Math.Abs(secondParameters.joints.rot[j, i]);
            }
        }

        float numSomersault = MathFunc.MatrixGetColumn(rotAbs, 0).Max() + secondParameters.joints.takeOffParam.Somersault / 360;
        AddsecondMessage(true, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgNumberSomersaults, numSomersault));
        AddsecondMessage(false, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgNumberTwists, MathFunc.MatrixGetColumn(rotAbs, 2).Max()));
        AddsecondMessage(false, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgFinalTwist, secondParameters.joints.rot[tIndex - 1, 2]));
        AddsecondMessage(false, true, string.Format(" {0} = {1:0}°", MainParameters.Instance.languages.Used.displayMsgMaxTilt, MathFunc.MatrixGetColumn(rotAbs, 1).Max() * 360));
        AddsecondMessage(false, true, string.Format(" {0} = {1:0}°", MainParameters.Instance.languages.Used.displayMsgFinalTilt, secondParameters.joints.rot[tIndex - 1, 1] * 360));

        return qOut;
    }


    private Vector ShortDynamics_s(double t, Vector x)
    {
        int nDDL = MainParameters.Instance.joints.lagrangianModel.nDDL;

        double[] q = new double[nDDL];
        double[] qdot = new double[nDDL];
        for (int i = 0; i < nDDL; i++)
        {
            q[i] = x[i];
            qdot[i] = x[nDDL + i];
        }

        double[,] m12;
        double[] n1;
        Inertia11Simple inertia11Simple = new Inertia11Simple();
        double[,] m11 = inertia11Simple.Inertia11(q);

        Inertia12Simple inertia12Simple = new Inertia12Simple();
        m12 = inertia12Simple.Inertia12(q);
        NLEffects1Simple nlEffects1Simple = new NLEffects1Simple();
        n1 = nlEffects1Simple.NLEffects1(q, qdot);

        if (!MainParameters.Instance.joints.UseGravity)
        {
            double[] n1zero;
            n1zero = nlEffects1Simple.NLEffects1(q, new double[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            for (int i = 0; i < 6; i++)
                n1[i] = n1[i] - n1zero[i];
        }

        float kp = 10;
        float kv = 3;
        float[] qd = new float[nDDL];
        float[] qdotd = new float[nDDL];
        float[] qddotd = new float[nDDL];

        Trajectory_s(MainParameters.Instance.joints.lagrangianModel, (float)t, MainParameters.Instance.joints.lagrangianModel.q2, out qd, out qdotd, out qddotd);

        float[] qddot = new float[nDDL];
        for (int i = 0; i < nDDL; i++)
            qddot[i] = qddotd[i] + kp * (qd[i] - (float)q[i]) + kv * (qdotd[i] - (float)qdot[i]);

        double[,] mA = MatrixInverse.MtrxInverse(m11);

        double[] q2qddot = new double[MainParameters.Instance.joints.lagrangianModel.q2.Length];
        for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.q2.Length; i++)
            q2qddot[i] = qddot[MainParameters.Instance.joints.lagrangianModel.q2[i] - 1];
        double[,] mB = MatrixInverse.MtrxProduct(m12, q2qddot);

        double[,] n1mB = new double[mB.GetUpperBound(0) + 1, mB.GetUpperBound(1) + 1];
        for (int i = 0; i <= mB.GetUpperBound(0); i++)
            for (int j = 0; j <= mB.GetUpperBound(1); j++)
                n1mB[i, j] = -n1[i] - mB[i, j];

        double[,] mC = MatrixInverse.MtrxProduct(mA, n1mB);

        for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.q1.Length; i++)
            qddot[MainParameters.Instance.joints.lagrangianModel.q1[i] - 1] = (float)mC[i, 0];

        double[] xdot = new double[MainParameters.Instance.joints.lagrangianModel.nDDL * 2];
        for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.nDDL; i++)
        {
            xdot[i] = qdot[i];
            xdot[MainParameters.Instance.joints.lagrangianModel.nDDL + i] = qddot[i];
        }

        //xdot[24]
        return new Vector(xdot);
    }

    private Vector ShortDynamicsSecond(double t, Vector x)
    {
        int nDDL = secondParameters.joints.lagrangianModel.nDDL;

        double[] q = new double[nDDL];
        double[] qdot = new double[nDDL];
        for (int i = 0; i < nDDL; i++)
        {
            q[i] = x[i];
            qdot[i] = x[nDDL + i];
        }

        double[,] m12;
        double[] n1;
        Inertia11Simple inertia11Simple = new Inertia11Simple();
        double[,] m11 = inertia11Simple.Inertia11(q);

        Inertia12Simple inertia12Simple = new Inertia12Simple();
        m12 = inertia12Simple.Inertia12(q);
        NLEffects1Simple nlEffects1Simple = new NLEffects1Simple();
        n1 = nlEffects1Simple.NLEffects1(q, qdot);
        if (!MainParameters.Instance.joints.UseGravity)
        {
            double[] n1zero;
            n1zero = nlEffects1Simple.NLEffects1(q, new double[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            for (int i = 0; i < 6; i++)
                n1[i] = n1[i] - n1zero[i];
        }

        float kp = 10;
        float kv = 3;
        float[] qd = new float[nDDL];
        float[] qdotd = new float[nDDL];
        float[] qddotd = new float[nDDL];

        TrajectorySecond(secondParameters.joints.lagrangianModel, (float)t, secondParameters.joints.lagrangianModel.q2, out qd, out qdotd, out qddotd);

        float[] qddot = new float[nDDL];
        for (int i = 0; i < nDDL; i++)
            qddot[i] = qddotd[i] + kp * (qd[i] - (float)q[i]) + kv * (qdotd[i] - (float)qdot[i]);

        double[,] mA = MatrixInverse.MtrxInverse(m11);

        double[] q2qddot = new double[secondParameters.joints.lagrangianModel.q2.Length];
        for (int i = 0; i < secondParameters.joints.lagrangianModel.q2.Length; i++)
            q2qddot[i] = qddot[secondParameters.joints.lagrangianModel.q2[i] - 1];
        double[,] mB = MatrixInverse.MtrxProduct(m12, q2qddot);

        double[,] n1mB = new double[mB.GetUpperBound(0) + 1, mB.GetUpperBound(1) + 1];
        for (int i = 0; i <= mB.GetUpperBound(0); i++)
            for (int j = 0; j <= mB.GetUpperBound(1); j++)
                n1mB[i, j] = -n1[i] - mB[i, j];

        double[,] mC = MatrixInverse.MtrxProduct(mA, n1mB);

        for (int i = 0; i < secondParameters.joints.lagrangianModel.q1.Length; i++)
            qddot[secondParameters.joints.lagrangianModel.q1[i] - 1] = (float)mC[i, 0];

        double[] xdot = new double[secondParameters.joints.lagrangianModel.nDDL * 2];
        for (int i = 0; i < secondParameters.joints.lagrangianModel.nDDL; i++)
        {
            xdot[i] = qdot[i];
            xdot[secondParameters.joints.lagrangianModel.nDDL + i] = qddot[i];
        }

        //xdot[24]
        return new Vector(xdot);
    }

    public void PlayOneFrameForSecond()
    {
        MainParameters.StrucJoints joints = MainParameters.Instance.joints;

        if (!IsEditing)
            if (q_girl2.GetUpperBound(1) >= secondFrameN)
            {
                qf_girl2 = MathFunc.MatrixGetColumnD(q_girl2, firstFrame + secondFrameN);
                if (playMode == MainParameters.Instance.languages.Used.animatorPlayModeGesticulation)
                    for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.q1.Length; i++)
                        qf_girl2[MainParameters.Instance.joints.lagrangianModel.q1[i] - 1] = 0;
            }

        avatarManager.SetAllDof(qf_girl2);
        girl2Hip.transform.localRotation = Quaternion.AngleAxis((float)qf_girl2[9] * Mathf.Rad2Deg, Vector3.right) *
                                            Quaternion.AngleAxis((float)qf_girl2[10] * Mathf.Rad2Deg, Vector3.forward) *
                                            Quaternion.AngleAxis((float)qf_girl2[11] * Mathf.Rad2Deg, Vector3.up);

        girl2Hip.transform.position = new Vector3((float)qf_girl2[6], (float)qf_girl2[8], (float)qf_girl2[7]);

        if (!secondPaused) secondFrameN++;
    }

    public float TravelDistance { 
        get {
            var _startPoint = MathFunc.MatrixGetColumnD(AllQ, 1);
            var _endPoint = MathFunc.MatrixGetColumnD(AllQ, numberFrames - 1);
            return Vector3.Distance(
                new Vector3((float)_startPoint[6], (float)_startPoint[8], (float)_startPoint[7]),
                new Vector3((float)_endPoint[6], (float)_endPoint[8], (float)_endPoint[7])
            );
        }
    }

    public float HorizontalTravelDistance {
        get => Mathf.Max((float)MathFunc.MatrixGetColumnD(AllQ,1)[7], (float)MathFunc.MatrixGetColumnD(AllQ, numberFrames - 1)[7]);
    }
    
    public float VerticalTravelDistance {
        get => Mathf.Max((float)MathFunc.MatrixGetColumnD(AllQ, 1)[8], (float)MathFunc.MatrixGetColumnD(AllQ, numberFrames - 1)[8]);
    }

    public float CheckPositionAvatar()
    {
        if (IsGestureMode || AllQ == null || AllQ.GetUpperBound(1) == 0) return 0;

        float _max = Mathf.Max(VerticalTravelDistance, HorizontalTravelDistance);

        if (q_girl2 != null && avatarManager.NumberOfLoadedAvatars > 1)
        {
            float vertical2 = Mathf.Max((float)MathFunc.MatrixGetColumnD(q_girl2, 1)[8], (float)MathFunc.MatrixGetColumnD(q_girl2, secondNumberFrames - 1)[8]);
            float horizontal2 = Mathf.Max((float)MathFunc.MatrixGetColumnD(q_girl2, 1)[7], (float)MathFunc.MatrixGetColumnD(q_girl2, secondNumberFrames - 1)[7]);

            float max2 = Mathf.Max(vertical2, horizontal2);
            if (max2 > _max) return max2;
        }

        return _max;
    }

    public void PlayOneFrame()
    {
        if (!IsEditing && AllQ != null)
        {
            var _q = MathFunc.MatrixGetColumnD(AllQ, firstFrame + frameN);
            if (playMode == MainParameters.Instance.languages.Used.animatorPlayModeGesticulation)
                for (int i = 0; i < MainParameters.Instance.joints.lagrangianModel.q1.Length; i++)
                    _q[MainParameters.Instance.joints.lagrangianModel.q1[i] - 1] = 0;
            avatarManager.SetAllDof(_q);
            if (!isPaused) SetFrameN(frameN + 1);
        }
    }

    public void InitPoseAvatar()
    {
        avatarManager.SetAllDof(MathFunc.MatrixGetColumnD(AllQ, 1));
    }


    public void StartEditing()
    {
        IsEditing = true;
        canResumeAnimation = false;
        if (sliderAnimation) sliderAnimation.DisableSlider();
    }

    public void StopEditing()
    {
        statManager.ResetTemporaries();
        UpdateFullKinematics(false);
        IsEditing = false;
        Pause();
        if (sliderAnimation) 
            sliderAnimation.EnableSlider();
    }

    public GameObject Avatar { get => avatarManager.LoadedModels[0].gameObject; }

    public bool PauseAvatar()
    {
        if (MainParameters.Instance.joints.nodes == null || Avatar == null || !Avatar.activeSelf) 
            return false;

        // TODO move this to AvatarManager
        Avatar.transform.rotation = Quaternion.identity;
        isPaused = !isPaused;

        if (avatarManager.NumberOfLoadedAvatars > 1)
            secondPaused = !secondPaused;
        return true;
    }

    public void ResetFrame()
    {
        canResumeAnimation = false;
        SetFrameN(0);
        firstFrame = 0;
        numberFrames = 0;
        timeElapsed = 0;

        pauseTime = 0;
        pauseStart = 0;

        secondFrameN = 0;
    }
}
