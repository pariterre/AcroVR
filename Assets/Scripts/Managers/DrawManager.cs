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

    public class AvatarProperties
    {
        public AvatarProperties(MainParameters.StrucJoints _joints, float _frameRate)
        {
            Joints = _joints;
            FrameRate = _frameRate;
        }

        public bool IsPaused = true;
        public int CurrentFrame = 0;
        public float FrameRate;
        public float CurrentTime { get => CurrentFrame * FrameRate; }

        public float InitialFeetHeight = (float)Double.NaN;
        public float[,] Q;  // q1
        public MainParameters.StrucJoints Joints;
    }

    protected AvatarManager avatarManager;
    public MainParameters.StrucJoints Joints(int _avatarIndex) => avatarProperties[_avatarIndex].Joints;
    public void SetJoints(int _avatarIndex, MainParameters.StrucJoints _joints)
    {
        avatarProperties[_avatarIndex].Joints = _joints;
        if (_avatarIndex == 0)
            MainParameters.Instance.joints = _joints;
    }
    public void SetJointsTandQ(int _avatarIndex, float[] t0, float[,] q0) {
        avatarProperties[_avatarIndex].Joints.t0 = MathFunc.MatrixCopy(t0);
        avatarProperties[_avatarIndex].Joints.q0 = MathFunc.MatrixCopy(q0);
    }
    public void SetJointsNodes(int _avatarIndex, MainParameters.StrucNodes[] _nodes) { avatarProperties[_avatarIndex].Joints.nodes = _nodes; }
    protected StatManager statManager;
    protected UIManager uiManager;

    protected SliderPlayAnimation sliderAnimation;
    protected DisplayResultGraphicS resultGraphics;

    protected List<AvatarProperties> avatarProperties = new List<AvatarProperties>();
    public float FrameRate { get; } = 0.02f;
    public GameObject girl2;

    private GameObject firstView;
    public bool canResumeAnimation { get; protected set; } = false;
    public void SetCanResumeAnimation(bool value) { canResumeAnimation = value; }

    public float[,] AllQ { get; protected set; } 
    public void SetCurrrentFrame(int _avatarIndex, int value) {
        avatarProperties[_avatarIndex].CurrentFrame = value;
        
        if (_avatarIndex == 0 && sliderAnimation) sliderAnimation.SetSlider(avatarProperties[_avatarIndex].CurrentFrame);
    }
    
    int firstFrame = 0;
    public int CurrentFrame { get => avatarProperties[0].CurrentFrame; }
    public float CurrentTime { get => avatarProperties[0].CurrentTime; }
    public int NumberFrames = 0;
    public float timeElapsed = 0;
    public float timeFrame = 0;
    float timeStarted = 0;
    float factorPlaySpeed = 1f;

    string playMode = MainParameters.Instance.languages.Used.animatorPlayModeSimulation;
    
    public void Pause() { avatarProperties[0].IsPaused = true; avatarProperties[1].IsPaused = true; }
    public bool IsPaused { get => avatarProperties[0].IsPaused || avatarProperties[1].IsPaused; }
    public void Resume(){ avatarProperties[0].IsPaused = false; avatarProperties[1].IsPaused = false; }
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

    public int secondNumberFrames = 0;
    public bool secondPaused = false;
    public List<string> secondResultMessages = new List<string>();


    void Start()
    {
        avatarManager = ToolBox.GetInstance().GetManager<AvatarManager>();
        statManager = ToolBox.GetInstance().GetManager<StatManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();

        avatarProperties.Add(new AvatarProperties(MainParameters.Instance.joints, FrameRate));
        avatarProperties.Add(new AvatarProperties(new MainParameters.StrucJoints(), FrameRate));

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
        if (avatarProperties[0].IsPaused && pauseStart == 0) 
            pauseStart = Time.time;

        if (!avatarProperties[0].IsPaused && pauseStart != 0)
        {
            pauseTime = Time.time - pauseStart;
            pauseStart = 0;
        }

        AdvanceTime(0);
        AdvanceTime(1);
    }

    void AdvanceTime(int _avatarIndex)
    {
        if (avatarProperties[_avatarIndex].CurrentFrame <= 0 && !avatarProperties[_avatarIndex].IsPaused) 
            timeStarted = Time.time;

        if (Time.time - (timeStarted + pauseTime) >= (timeFrame * avatarProperties[_avatarIndex].CurrentFrame) * factorPlaySpeed)
        {
            if (!avatarProperties[_avatarIndex].IsPaused)
                timeElapsed = Time.time - (timeStarted + pauseTime);

            if (ShouldContinuePlaying(_avatarIndex))
                PlayOneFrame(_avatarIndex);
            else
                PlayEnd();
        }
    }

    public bool ShouldContinuePlaying(int _avatarIndex)
    {
        if (avatarProperties[_avatarIndex].CurrentFrame >= NumberFrames - 1) return false;

        if (
            avatarProperties[_avatarIndex].CurrentFrame != 0 
            && Joints(_avatarIndex).StopOnGround && 
            !IsGestureMode && 
            avatarManager.FeetHeight() < avatarProperties[_avatarIndex].InitialFeetHeight
        ) 
            return false;
        
        return true;
    }

    public void UpdateFullKinematics(bool restartToZero)
    {
        for (int _avatarIndex = 0; _avatarIndex < avatarProperties.Count; ++_avatarIndex)
        {
            MakeSimulationFrame(_avatarIndex);
            Play(_avatarIndex, avatarProperties[0].Q, 0, avatarProperties[0].Q.GetUpperBound(1) + 1, restartToZero);
        }
    }

    public void MakeSimulationFrame(int _avatarIndex)
    {
        if (Joints(_avatarIndex).nodes == null) return;

        avatarProperties[_avatarIndex].Q = MakeSimulation(_avatarIndex);
    }

    public void ForceFullUpdate(int _avatarIndex)
    {
        ShowAvatar(_avatarIndex);
        ShowGround();
        PlayOneFrame(_avatarIndex);
        SetCurrrentFrame(_avatarIndex, avatarProperties[_avatarIndex].CurrentFrame);
    }

    public void ShowAvatar(int _avatarIndex)
    {
        MakeSimulationFrame(_avatarIndex);
        if (Joints(_avatarIndex).nodes == null) return;

        CenterAvatar(_avatarIndex);
        Play(_avatarIndex, avatarProperties[_avatarIndex].Q, 0, avatarProperties[_avatarIndex].Q.GetUpperBound(1) + 1, true);
    }

    public void CenterAvatar(int _avatarIndex)
    {
        var _model = avatarManager.LoadedModels[_avatarIndex];
        if (!_model.IsLoaded) return;

        Vector3 _scaling = _model.gameObject.transform.localScale;
        var _hipTranslations = Double.IsNaN(avatarProperties[_avatarIndex].InitialFeetHeight) 
            ? new Vector3(0f, 0f, 0f) 
            : new Vector3(0f, -avatarProperties[_avatarIndex].InitialFeetHeight * _scaling.y, 0f);
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
            Ground.SetActive(Joints(0).StopOnGround);
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

    public void PlayAvatar(int _avatarIndex)
    {
        if (Joints(_avatarIndex).nodes == null) return;

        ShowAvatar(_avatarIndex);
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

    protected void Play(int _avatarIndex, float[,] qq, int frFrame, int nFrames, bool restartToZero)
    {
        MainParameters.StrucJoints joints = Joints(_avatarIndex);

        AllQ = MathFunc.MatrixCopy(qq);
        if (restartToZero)
            SetCurrrentFrame(_avatarIndex, 0);
        firstFrame = frFrame;
        NumberFrames = nFrames;

        timeElapsed = 0;

        pauseTime = 0;
        pauseStart = 0;

        if (nFrames > 1)
        {
            if (joints.tc > 0)                          // Il y a eu contact avec le sol, alors seulement une partie des données sont utilisé
                timeFrame = joints.tc / (NumberFrames - 1);
            else                                        // Aucun contact avec le sol, alors toutes les données sont utilisé
                timeFrame = joints.Duration / (NumberFrames - 1);
        }
        else
            timeFrame = 0;
    }

    private void Quintic(float t, float ti, float tj, float qi, float qj, out float p, out float v, out float a)
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

    public void Trajectory(MainParameters.StrucJoints _joints, float t, int[] qi, out float[] qd, out float[] qdotd, out float[] qddotd)
    {
        qd = new float[_joints.lagrangianModel.nDDL];
        qdotd = new float[_joints.lagrangianModel.nDDL];
        qddotd = new float[_joints.lagrangianModel.nDDL];
        for (int i = 0; i < qd.Length; i++)
        {
            qd[i] = 0;
            qdotd[i] = 0;
            qddotd[i] = 0;
        }

        int n = qi.Length;

        // n=6, 6Node (HipFlexion, KneeFlexion ...)
        for (int i = 0; i < _joints.nodes.Length; i++)
        {
            int ii = qi[i] - _joints.lagrangianModel.q2[0];
            MainParameters.StrucNodes nodes = _joints.nodes[ii];

            int j = 1;
            while (j < nodes.T.Length - 1 && t > nodes.T[j]) j++;
            Quintic(t, nodes.T[j - 1], nodes.T[j], nodes.Q[j - 1], nodes.Q[j], out qd[ii], out qdotd[ii], out qddotd[ii]);
        }
    }

    private float[,] MakeSimulation(int _avatarIndex)
    {
        if (avatarProperties[_avatarIndex].Joints.nodes == null) return new float[0,0];

        MainParameters.StrucJoints _joints = avatarProperties[_avatarIndex].Joints;
        float[] q0 = new float[_joints.lagrangianModel.nDDL];
        float[] q0dot = new float[_joints.lagrangianModel.nDDL];
        if (Double.IsNaN(avatarProperties[_avatarIndex].InitialFeetHeight)){
            avatarProperties[_avatarIndex].InitialFeetHeight = avatarManager.FeetHeight(q0);
        }

        for (int i = 0; i < _joints.nodes.Length; i++)
        {
            MainParameters.StrucNodes nodes = _joints.nodes[i];
            q0[i] = nodes.Q[0];
        }

        // Beginning Pose
        int[] rotation = new int[3] { _joints.lagrangianModel.root_somersault, _joints.lagrangianModel.root_tilt, _joints.lagrangianModel.root_twist };
        int[] rotationSign = MathFunc.Sign(rotation);
        for (int i = 0; i < rotation.Length; i++) rotation[i] = Math.Abs(rotation[i]);

        int[] translation = new int[3] { _joints.lagrangianModel.root_right, _joints.lagrangianModel.root_foreward, _joints.lagrangianModel.root_upward };
        int[] translationS = MathFunc.Sign(translation);
        for (int i = 0; i < translation.Length; i++) translation[i] = Math.Abs(translation[i]);

        float rotRadians = _joints.takeOffParam.Somersault * (float)Math.PI / 180;

        float tilt = _joints.takeOffParam.Tilt;
        if (tilt == 90)
            tilt = 90.001f;
        else if (tilt == -90)
            tilt = -90.01f;

        // q0[12]
        // q0[9] = somersault
        // q0[10] = tilt
        q0[Math.Abs(_joints.lagrangianModel.root_tilt) - 1] = tilt * (float)Math.PI / 180; 
        q0[Math.Abs(_joints.lagrangianModel.root_somersault) - 1] = rotRadians; 

        //q0dot[12]
        //q0dot[7] = AnteroposteriorSpeed
        //q0dot[8] = verticalSpeed
        //q0dot[9] = somersaultSpeed
        //q0dot[11] = twistSpeed
        q0dot[Math.Abs(_joints.lagrangianModel.root_foreward) - 1] = _joints.takeOffParam.HorizontalSpeed;                       // m/s
        q0dot[Math.Abs(_joints.lagrangianModel.root_upward) - 1] = _joints.takeOffParam.VerticalSpeed;                                // m/s
        q0dot[Math.Abs(_joints.lagrangianModel.root_somersault) - 1] = _joints.takeOffParam.SomersaultSpeed * 2 * (float)Math.PI;     // radians/s
        q0dot[Math.Abs(_joints.lagrangianModel.root_twist) - 1] = _joints.takeOffParam.TwistSpeed * 2 * (float)Math.PI;               // radians/s


        // q0[11] = twist
        // q0dot[10] = tiltSpeed
        q0[Math.Abs(_joints.lagrangianModel.root_twist) - 1] = _joints.takeOffParam.Twist * (float)Math.PI / 180;
        q0dot[Math.Abs(_joints.lagrangianModel.root_tilt) - 1] = _joints.takeOffParam.TiltSpeed * 2 * (float)Math.PI;


        double[] Q = new double[_joints.lagrangianModel.nDDL];
        for (int i = 0; i < _joints.lagrangianModel.nDDL; i++)
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

        q0[Math.Abs(_joints.lagrangianModel.root_foreward) - 1] += _joints.takeOffParam.HorizontalPosition;
        q0[Math.Abs(_joints.lagrangianModel.root_upward) - 1] += _joints.takeOffParam.VerticalPosition;

        double[] x0 = new double[_joints.lagrangianModel.nDDL * 2];
        for (int i = 0; i < _joints.lagrangianModel.nDDL; i++)
        {
            x0[i] = q0[i];
            x0[_joints.lagrangianModel.nDDL + i] = q0dot[i];
        }

        // x0[24]

        Options options = new Options();
        options.InitialStep = _joints.lagrangianModel.dt;
        var sol = Ode.RK547M(_joints, 0, _joints.Duration + _joints.lagrangianModel.dt, new Vector(x0), ShortDynamics, options);

        var points = sol.SolveFromToStep(0, _joints.Duration + _joints.lagrangianModel.dt, _joints.lagrangianModel.dt).ToArray();

        // test0 = point[51]
        // test1 = point[251]
        double[] t = new double[points.GetUpperBound(0) + 1];
        double[,] q = new double[_joints.lagrangianModel.nDDL, points.GetUpperBound(0) + 1];
        double[,] qdot = new double[_joints.lagrangianModel.nDDL, points.GetUpperBound(0) + 1];
        for (int i = 0; i < _joints.lagrangianModel.nDDL; i++)
        {
            for (int j = 0; j <= points.GetUpperBound(0); j++)
            {
                if (i <= 0)
                    t[j] = points[j].T;

                q[i, j] = points[j].X[i];
                qdot[i, j] = points[j].X[_joints.lagrangianModel.nDDL + i];
            }
        }

        // test0 = t[51], q[12,51], qdot[12,51]
        // test1 = t[251], q[12,251], qdot[12,251]
        int tIndex = 0;
        _joints.tc = 0;
        for (int i = 0; i <= q.GetUpperBound(1); i++)
        {
            tIndex++;
            double[] qq = new double[_joints.lagrangianModel.nDDL];
            for (int j = 0; j < _joints.lagrangianModel.nDDL; j++)
                qq[j] = q[j, i];
            avatarManager.EvaluateTags(qq, out tagX, out tagY, out tagZ);

            // Cut the trial when the feet crosses the ground (vertical axis = 0)
            if (
                  !IsGestureMode && i > 0 
                  && _joints.StopOnGround 
                  && _joints.UseGravity 
                  && tagZ.Min() < avatarProperties[_avatarIndex].InitialFeetHeight
            )
            {
                _joints.tc = (float)t[i];
                break;
            }
        }

        _joints.t = new float[tIndex];
        float[,] qOut = new float[_joints.lagrangianModel.nDDL, tIndex];
        float[,] qdot1 = new float[_joints.lagrangianModel.nDDL, tIndex];
        for (int i = 0; i < tIndex; i++)
        {
            _joints.t[i] = (float)t[i];
            for (int j = 0; j < _joints.lagrangianModel.nDDL; j++)
            {
                qOut[j, i] = (float)q[j, i];
                qdot1[j, i] = (float)qdot[j, i];
            }
        }

        _joints.rot = new float[tIndex, rotation.Length];
        _joints.rotdot = new float[tIndex, rotation.Length];
        float[,] rotAbs = new float[tIndex, rotation.Length];
        for (int i = 0; i < rotation.Length; i++)
        {
            float[] rotCol = new float[tIndex];
            float[] rotdotCol = new float[tIndex];
            rotCol = MathFunc.unwrap(MathFunc.MatrixGetRow(qOut, rotation[i] - 1));
            rotdotCol = MathFunc.unwrap(MathFunc.MatrixGetRow(qdot1, rotation[i] - 1));
            for (int j = 0; j < tIndex; j++)
            {
                _joints.rot[j, i] = rotCol[j] / (2 * (float)Math.PI);
                _joints.rotdot[j, i] = rotdotCol[j] / (2 * (float)Math.PI);
                rotAbs[j, i] = Math.Abs(_joints.rot[j, i]);
            }
        }

        float numSomersault = MathFunc.MatrixGetColumn(rotAbs, 0).Max() + _joints.takeOffParam.Somersault / 360;
        DisplayNewMessage(true, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgNumberSomersaults, numSomersault));
        DisplayNewMessage(false, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgNumberTwists, MathFunc.MatrixGetColumn(rotAbs, 2).Max()));
        DisplayNewMessage(false, true, string.Format(" {0} = {1:0.00}", MainParameters.Instance.languages.Used.displayMsgFinalTwist, _joints.rot[tIndex - 1, 2]));
        DisplayNewMessage(false, true, string.Format(" {0} = {1:0}°", MainParameters.Instance.languages.Used.displayMsgMaxTilt, MathFunc.MatrixGetColumn(rotAbs, 1).Max() * 360));
        DisplayNewMessage(false, true, string.Format(" {0} = {1:0}°", MainParameters.Instance.languages.Used.displayMsgFinalTilt, _joints.rot[tIndex - 1, 1] * 360));

        return qOut;
    }

    protected Vector ShortDynamics(MainParameters.StrucJoints _joints, double t, Vector x)
    {
        int nDDL = _joints.lagrangianModel.nDDL;

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

        if (!_joints.UseGravity)
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

        Trajectory(_joints, (float)t, _joints.lagrangianModel.q2, out qd, out qdotd, out qddotd);

        float[] qddot = new float[nDDL];
        for (int i = 0; i < nDDL; i++)
            qddot[i] = qddotd[i] + kp * (qd[i] - (float)q[i]) + kv * (qdotd[i] - (float)qdot[i]);

        double[,] mA = MatrixInverse.MtrxInverse(m11);

        double[] q2qddot = new double[_joints.lagrangianModel.q2.Length];
        for (int i = 0; i < _joints.lagrangianModel.q2.Length; i++)
            q2qddot[i] = qddot[_joints.lagrangianModel.q2[i] - 1];
        double[,] mB = MatrixInverse.MtrxProduct(m12, q2qddot);

        double[,] n1mB = new double[mB.GetUpperBound(0) + 1, mB.GetUpperBound(1) + 1];
        for (int i = 0; i <= mB.GetUpperBound(0); i++)
            for (int j = 0; j <= mB.GetUpperBound(1); j++)
                n1mB[i, j] = -n1[i] - mB[i, j];

        double[,] mC = MatrixInverse.MtrxProduct(mA, n1mB);

        for (int i = 0; i < _joints.lagrangianModel.q1.Length; i++)
            qddot[_joints.lagrangianModel.q1[i] - 1] = (float)mC[i, 0];

        double[] xdot = new double[_joints.lagrangianModel.nDDL * 2];
        for (int i = 0; i < _joints.lagrangianModel.nDDL; i++)
        {
            xdot[i] = qdot[i];
            xdot[_joints.lagrangianModel.nDDL + i] = qddot[i];
        }

        //xdot[24]
        return new Vector(xdot);
    }

    public float TravelDistance { 
        get {
            var _startPoint = MathFunc.MatrixGetColumnD(AllQ, 1);
            var _endPoint = MathFunc.MatrixGetColumnD(AllQ, NumberFrames - 1);
            return Vector3.Distance(
                new Vector3((float)_startPoint[6], (float)_startPoint[8], (float)_startPoint[7]),
                new Vector3((float)_endPoint[6], (float)_endPoint[8], (float)_endPoint[7])
            );
        }
    }

    public float HorizontalTravelDistance {
        get => Mathf.Max((float)MathFunc.MatrixGetColumnD(AllQ,1)[7], (float)MathFunc.MatrixGetColumnD(AllQ, NumberFrames - 1)[7]);
    }
    
    public float VerticalTravelDistance {
        get => Mathf.Max((float)MathFunc.MatrixGetColumnD(AllQ, 1)[8], (float)MathFunc.MatrixGetColumnD(AllQ, NumberFrames - 1)[8]);
    }

    public float CheckPositionAvatar()
    {
        if (IsGestureMode || AllQ == null || AllQ.GetUpperBound(1) == 0) return 0;

        float _max = Mathf.Max(VerticalTravelDistance, HorizontalTravelDistance);

        return _max;
    }

    public void PlayOneFrame(int _avatarIndex)
    {
        if (!IsEditing && AllQ != null)
        {
            var _q = MathFunc.MatrixGetColumnD(AllQ, firstFrame + avatarProperties[_avatarIndex].CurrentFrame);
            if (playMode == MainParameters.Instance.languages.Used.animatorPlayModeGesticulation)
                for (int i = 0; i < Joints(_avatarIndex).lagrangianModel.q1.Length; i++)
                    _q[Joints(_avatarIndex).lagrangianModel.q1[i] - 1] = 0;
            avatarManager.SetAllDof(_avatarIndex, _q);
            
            if (!avatarProperties[_avatarIndex].IsPaused) SetCurrrentFrame(_avatarIndex, avatarProperties[_avatarIndex].CurrentFrame + 1);
        }
    }

    public void InitPoseAvatar()
    {
        avatarManager.SetAllDof(0, MathFunc.MatrixGetColumnD(AllQ, 1));
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

    public GameObject Avatar(int _avatarIndex) => avatarManager.LoadedModels[_avatarIndex].gameObject;

    public bool PauseAvatar(int _avatarIndex)
    {
        if (Joints(_avatarIndex).nodes == null || Avatar(_avatarIndex) == null || !Avatar(_avatarIndex).activeSelf) 
            return false;

        Avatar(_avatarIndex).transform.rotation = Quaternion.identity;
        avatarProperties[_avatarIndex].IsPaused = !avatarProperties[_avatarIndex].IsPaused;

        return true;
    }

    public void ResetFrame()
    {
        canResumeAnimation = false;
        SetCurrrentFrame(0, 0);
        SetCurrrentFrame(1, 0);
        firstFrame = 0;
        NumberFrames = 0;
        timeElapsed = 0;

        pauseTime = 0;
        pauseStart = 0;
    }
}
