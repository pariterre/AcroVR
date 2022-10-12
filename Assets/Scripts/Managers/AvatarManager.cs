using System;
using System.Collections.Generic;
using UnityEngine;

public class AvatarManager : MonoBehaviour
{
    public enum Model
    {
        SingleFemale = 0,
        SingleMale = 1,
        DoubleFemale = 2,
        DoubleMale = 3,
    }

    public struct Avatar {
        public GameObject gameObject;

        public bool IsLoaded { get => gameObject != null; }

        // Root
        public GameObject Hip;

        // Hip
        public GameObject LeftThigh;
        public GameObject RightThigh;
        public ControlThigh ThighControl;
        // Knee
        public GameObject LeftLeg;
        public GameObject RightLeg;
        public ControlShin LegControl;
        // Shoulder
        public GameObject LeftArm;
        public ControlLeftArmAbduction LeftArmControlAbd;
        public ControlLeftArmFlexion LeftArmControlFlex;
        public GameObject RightArm;
        public ControlRightArmAbduction RightArmControlAbd;
        public ControlRightArmFlexion RightArmControlFlex;
    }

    protected DrawManager drawManager;
    public double[] Q { get; protected set; }

    void Start(){
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();

        for (int i=0; i<2; ++i)
        {
            NamePrefab.Add("");
            LoadedModels.Add(new Avatar());
        }

        SelectAvatar((AvatarManager.Model)PlayerPrefs.GetInt("AvatarModel", (int)AvatarManager.Model.SingleFemale));
    }

    public AvatarManager.Model SelectedAvatarModel { get; protected set; }
    public void SelectAvatar(AvatarManager.Model _avatar){
        SelectedAvatarModel = _avatar;
        PlayerPrefs.SetInt("AvatarModel", (int)SelectedAvatarModel);

        if (SelectedAvatarModel == AvatarManager.Model.SingleFemale || SelectedAvatarModel == AvatarManager.Model.DoubleFemale)
        {
            NamePrefab[0] = "girl1_control";
            NamePrefab[1] = "girl1";
        }
        else if (SelectedAvatarModel == AvatarManager.Model.SingleMale || SelectedAvatarModel == AvatarManager.Model.DoubleMale)
        {
            NamePrefab[0] = "man1_control";
            NamePrefab[1] = "man1";
        }
    }

    public List<string> NamePrefab { get; protected set; } = new List<string>(2);
    public List<Avatar> LoadedModels { get; protected set; } = new List<Avatar>(2);
    public int NumberOfLoadedAvatars { get => Convert.ToInt32(LoadedModels[0].IsLoaded) + Convert.ToInt32(LoadedModels[1].IsLoaded); }
    
    public void LoadAvatar(int _index)
    {
        LoadPrefab(_index);
        LoadAvatarControls(_index);

        LoadedModels[_index].gameObject.SetActive(true);
        if (_index > 0)
            LoadedModels[_index].gameObject.SetActive((int)SelectedAvatarModel > 1);
    }
    
    protected void LoadAvatarControls(int _index)
    {
        var _rootDirectory = PrefabRootDirectory(SelectedAvatarModel);

        var _model = LoadedModels[_index];
        _model.ThighControl = _model.gameObject.transform.Find(_rootDirectory + "/hips/zero_thigh.L/thigh.L/ControlThigh").GetComponent<ControlThigh>();
        _model.LegControl = _model.gameObject.transform.Find(_rootDirectory + "/hips/zero_thigh.L/thigh.L/zero_shin.L/shin.L/ControlShin").GetComponent<ControlShin>();
        _model.LeftArmControlAbd = _model.gameObject.transform.Find(_rootDirectory + "/hips/spine/chest/chest1/shoulder.L/zero_upper_arm.L/upper_arm.L/ControlLeftArm").GetComponent<ControlLeftArmAbduction>();
        _model.LeftArmControlFlex = _model.gameObject.transform.Find(_rootDirectory + "/hips/spine/chest/chest1/shoulder.L/zero_upper_arm.L/upper_arm.L/ControlLeftArm").GetComponent<ControlLeftArmFlexion>();
        _model.RightArmControlAbd = _model.gameObject.transform.Find(_rootDirectory + "/hips/spine/chest/chest1/shoulder.R/zero_upper_arm.R/upper_arm.R/ControlRightArm").GetComponent<ControlRightArmAbduction>();
        _model.RightArmControlFlex = _model.gameObject.transform.Find(_rootDirectory + "/hips/spine/chest/chest1/shoulder.R/zero_upper_arm.R/upper_arm.R/ControlRightArm").GetComponent<ControlRightArmFlexion>();
        LoadedModels[_index] = _model;
    }

    protected void LoadPrefab(int _index)
    {
        var _model = new Avatar();
        _model.gameObject = Instantiate((GameObject)Resources.Load(NamePrefab[_index], typeof(GameObject)));
        var _rootDirectory = PrefabRootDirectory(SelectedAvatarModel);

        // Root
        _model.Hip = _model.gameObject.transform.Find(_rootDirectory + "/hips").gameObject;
        _model.LeftThigh = _model.gameObject.transform.Find(_rootDirectory + "/hips/zero_thigh.L/thigh.L").gameObject;
        _model.RightThigh = _model.gameObject.transform.Find(_rootDirectory + "/hips/zero_thigh.R/thigh.R").gameObject;
        // Knee
        _model.LeftLeg = _model.gameObject.transform.Find(_rootDirectory + "/hips/zero_thigh.L/thigh.L/zero_shin.L/shin.L").gameObject;
        _model.RightLeg = _model.gameObject.transform.Find(_rootDirectory + "/hips/zero_thigh.R/thigh.R/zero_shin.R/shin.R").gameObject;
        // Shoulder
        _model.LeftArm = _model.gameObject.transform.Find(_rootDirectory + "/hips/spine/chest/chest1/shoulder.L/zero_upper_arm.L/upper_arm.L").gameObject;
        _model.RightArm = _model.gameObject.transform.Find(_rootDirectory + "/hips/spine/chest/chest1/shoulder.R/zero_upper_arm.R/upper_arm.R").gameObject;

        drawManager.SetFirstView(_model.gameObject.transform.Find(_rootDirectory + "/hips/FirstViewPoint").gameObject);

        // set to zero position
        _model.gameObject.transform.position = Vector3.zero;
        _model.Hip.transform.position = Vector3.zero;
        _model.LeftArm.transform.localRotation = Quaternion.identity;
        _model.RightArm.transform.localRotation = Quaternion.identity;

        LoadedModels[_index] = _model;
        drawManager.CenterAvatar(_index);
    }

    protected string PrefabRootDirectory(AvatarManager.Model _model){
        if (_model == AvatarManager.Model.SingleFemale || _model == AvatarManager.Model.DoubleFemale)
            return "Petra.002";
        else if (_model == AvatarManager.Model.SingleMale || _model == AvatarManager.Model.DoubleMale)
            return "Petra.002";
        else
            throw new NotImplementedException("This avatar is no implemented") ;
    }

    public void SetAllDof(int _avatarIndex, double[] _q)
    {
        Q = _q;
        drawManager.CenterAvatar(_avatarIndex);
        SetThigh(_avatarIndex);
        SetShin(_avatarIndex);
        SetRightArm(_avatarIndex);
        SetLeftArm(_avatarIndex);
    }

    public void SetThigh(int _avatarIndex, float _value)
    {
        Q[LoadedModels[0].ThighControl.avatarIndex] = _value;
        SetThigh(_avatarIndex);
    }
    protected void SetThigh(int _avatarIndex)
    {
        int _ddl = LoadedModels[_avatarIndex].ThighControl.avatarIndex;
        LoadedModels[_avatarIndex].LeftThigh.transform.localEulerAngles = new Vector3(-(float)Q[_ddl], 0f, 0f) * Mathf.Rad2Deg;
        LoadedModels[_avatarIndex].RightThigh.transform.localEulerAngles = new Vector3(-(float)Q[_ddl], 0f, 0f) * Mathf.Rad2Deg;
    }

    public void SetShin(int _avatarIndex, float _value)
    {
        Q[LoadedModels[_avatarIndex].LegControl.avatarIndex] = _value;
        SetShin(_avatarIndex);
    }
    protected void SetShin(int _avatarIndex)
    {
        int ddl = LoadedModels[_avatarIndex].LegControl.avatarIndex;
        LoadedModels[_avatarIndex].LeftLeg.transform.localEulerAngles = new Vector3((float)Q[ddl], 0f, 0f) * Mathf.Rad2Deg;
        LoadedModels[_avatarIndex].RightLeg.transform.localEulerAngles = new Vector3((float)Q[ddl], 0f, 0f) * Mathf.Rad2Deg;
    }


    public void SetLeftArmAbduction(int _avatarIndex, float _value)
    {
        Q[LoadedModels[_avatarIndex].LeftArmControlAbd.avatarIndex] = _value;
        SetLeftArm(_avatarIndex);
    }
    public void SetLeftArmFlexion(int _avatarIndex, float _value)
    {
        Q[LoadedModels[_avatarIndex].LeftArmControlFlex.avatarIndex] = _value;
        SetLeftArm(_avatarIndex);
    }
    protected void SetLeftArm(int _avatarIndex)
    {
        int ddlAbduction = LoadedModels[_avatarIndex].LeftArmControlAbd.avatarIndex;
        int ddlFlexion = LoadedModels[_avatarIndex].LeftArmControlFlex.avatarIndex;
        LoadedModels[_avatarIndex].LeftArm.transform.localEulerAngles = new Vector3((float)Q[ddlFlexion], 0, (float)Q[ddlAbduction]) * Mathf.Rad2Deg;
    }

    public void SetRightArmAbduction(int _avatarIndex, float _value)
    {
        Q[LoadedModels[_avatarIndex].RightArmControlAbd.avatarIndex] = _value;
        SetRightArm(_avatarIndex);
    }
    public void SetRightArmFlexion(int _avatarIndex, float _value)
    {
        Q[LoadedModels[_avatarIndex].RightArmControlFlex.avatarIndex] = _value;
        SetRightArm(_avatarIndex);
    }
    protected void SetRightArm(int _avatarIndex)
    {
        int ddlAbduction = LoadedModels[_avatarIndex].RightArmControlAbd.avatarIndex;
        int ddlFlexion = LoadedModels[_avatarIndex].RightArmControlFlex.avatarIndex;
        LoadedModels[_avatarIndex].RightArm.transform.localEulerAngles = new Vector3((float)Q[ddlFlexion], 0, (float)Q[ddlAbduction]) * Mathf.Rad2Deg;
    }

    public float FeetHeight()
    {
        return (float)FeetHeight(Q);
    }
    public float FeetHeight(float[] q)
    {
        double[] qDouble = new double[q.Length];
        for (int i = 0; i < q.Length; ++i)
            qDouble[i] = q[i];

        return (float)FeetHeight(qDouble);
    }
    public double FeetHeight(double[] q)
    {
        float[] tagX;
        float[] tagY;
        float[] tagZ;
        EvaluateTags(q, out tagX, out tagY, out tagZ);
        return Math.Min(
            tagZ[MainParameters.Instance.joints.lagrangianModel.feet[0] - 1],
            tagZ[MainParameters.Instance.joints.lagrangianModel.feet[1] - 1]
        );
    }

    public void EvaluateTags(double[] q, out float[] tagX, out float[] tagY, out float[] tagZ)
    {
        // q[12]

        double[] tag1;
        TagsSimple tagsSimple = new TagsSimple();
        tag1 = tagsSimple.Tags(q);

        // tag1[78]

        int newTagLength = tag1.Length / 3;

        // newTagLength = 26;

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
}
