using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarManager : MonoBehaviour
{
    public enum Model
    {
        SingleFemale = 0,
        DoubleFemale = 1,
        SingleMale = 2,
        DoubleMale = 3,
    }

    public struct Avatar {
        public GameObject gameObject;
        bool IsLoaded { get => gameObject != null; }

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

    void Start(){
        SetAvatar((AvatarManager.Model)PlayerPrefs.GetInt("AvatarModel", (int)AvatarManager.Model.SingleFemale));

        // There are exactly two models that must be loaded
        LoadedModels.Add(new Avatar());
        LoadedModels.Add(new Avatar());
    }

    public AvatarManager.Model CurrentAvatarModel { get; protected set; }
    public void SetAvatar(AvatarManager.Model _avatar){
        CurrentAvatarModel = _avatar;
        PlayerPrefs.SetInt("AvatarModel", (int)CurrentAvatarModel);
    }

    string Name;
    protected GameObject Prefab;
    protected List<Avatar> LoadedModels = new List<Avatar>();
    public int NumberOfLoadedAvatars { get => (int)(LoadedModels[0].IsLoaded) + (int)(LoadedModels[1].IsLoaded); }
    
    public bool LoadAvatar(int _index)
    {
        string namePrefab = "";
        if 
        switch (_model)
        {
            case Avatar.Model.SingleFemale:
                namePrefab = "girl1_control";
                if (girl1) girl1.SetActive(true);
                if (girl2) girl2.SetActive(false);
                break;
            case Avatar.Model.DoubleFemale:
                namePrefab1 = "girl1_control";
                namePrefab2 = "girl2";
                if (girl1) girl1.SetActive(true);
                if (girl2) girl2.SetActive(true);
                break;
            case Avatar.Model.SingleMale:
                namePrefab1 = "man1_control";
                if (girl1) girl1.SetActive(true);
                if (girl2) girl2.SetActive(false);
                break;
            case Avatar.Model.DoubleMale:
                namePrefab1 = "man1_control";
                namePrefab2 = "man2";
                if (girl1) girl1.SetActive(true);
                if (girl2) girl2.SetActive(true);
                break;
        }

        if (NumberOfLoadedAvatars == 0)
        {
            LoadPrefab(
                namePrefab1, ref girl1Prefab, ref girl1, 
                ref girl1LeftThigh, ref girl1RightThigh, ref girl1LeftLeg, ref girl1RightLeg, 
                ref girl1RightArm, ref girl1LeftArm, ref girl1Hip, ref firstView
            );

            LoadAvatarControls(girl1, 
                ref girl1ThighControl, ref girl1LegControl, 
                ref girl1LeftArmControlAbd, ref girl1RightArmControlAbd, ref girl1LeftArmControlFlex, ref girl1RightArmControlFlex);
        }


        if (NumberOfLoadedAvatars > 1)
        {
            if (girl2 == null)
            {
                LoadGirlPrefab(
                    namePrefab2, ref girl2Prefab, ref girl2,
                    ref girl2LeftThigh, ref girl2RightThigh, ref girl2LeftLeg, ref girl2RightLeg,
                    ref girl2RightArm, ref girl2LeftArm, ref girl2Hip, ref firstView
                );
            }

            q1_girl2 = MakeSimulationSecond();
            q_girl2 = MathFunc.MatrixCopy(q1_girl2);
        }

        return true;
    }

    protected string PrefabName(AvatarManager.Model _model){
        if (_model == AvatarModel.SingleFemale)
            return "girl1_control";
        else if (_model == AvatarModel.SingleMale)
            return "man1_control";
        else
            throw new NotImplementedException("This avatar is no implemented") ;
    }

    protected string PrefabRootDirectory(AvatarManager.Model _model){
        if (_model == AvatarModel.SingleFemale)
            return _rootDirectory + "";
        else if (_model == AvatarModel.SingleMale)
            return _rootDirectory + "";
        else
            throw new NotImplementedException("This avatar is no implemented") ;
    }
    
    protected void LoadPrefab(int _index, AvatarManager.Model _model, ref GameObject _view)
    {
        _namePrefab = PrefabName(_model);
        Prefab = (GameObject)Resources.Load(_namePrefab, typeof(GameObject));

        LoadedModel[_index] = new Avatar();
        _model = LoadedModel[_index];
        _model.gameObject = Instantiate(Prefab);
        var _rootDirectory = PrefabRootDirectory(_model);

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

        _view = _model.gameObject.transform.Find(_rootDirectory + "/hips/FirstViewPoint").gameObject;

        // set to zero position
        _model.gameObject.transform.position = Vector3.zero;
        _model.Hip.transform.position = Vector3.zero;
        CenterAvatar(_index);
        _model.LeftArm.transform.localRotation = Quaternion.identity;
        _model.RightArm.transform.localRotation = Quaternion.identity;
    }
    
    protected void LoadAvatarControls(int _index )
    {
        var _rootDirectory = PrefabRootDirectory(_model);

        var _model = LoadedModel[_index];
        _model.ThighControl = _model.gameObject.transform.Find(_rootDirectory + "/hips/zero_thigh.L/thigh.L/ControlThigh").GetComponent<ControlThigh>();
        _model.LegControl = _model.gameObject.transform.Find(_rootDirectory + "/hips/zero_thigh.L/thigh.L/zero_shin.L/shin.L/ControlShin").GetComponent<ControlShin>();
        _model.LeftArmControlAbd = _model.gameObject.transform.Find(_rootDirectory + "/hips/spine/chest/chest1/shoulder.L/zero_upper_arm.L/upper_arm.L/ControlLeftArm").GetComponent<ControlLeftArmAbduction>();
        _model.LeftArmControlFlex = _model.gameObject.transform.Find(_rootDirectory + "/hips/spine/chest/chest1/shoulder.L/zero_upper_arm.L/upper_arm.L/ControlLeftArm").GetComponent<ControlLeftArmFlexion>();
        _model.RightArmControlAbd = _model.gameObject.transform.Find(_rootDirectory + "/hips/spine/chest/chest1/shoulder.R/zero_upper_arm.R/upper_arm.R/ControlRightArm").GetComponent<ControlRightArmAbduction>();
        _model.RightArmControlFlex = _model.gameObject.transform.Find(_rootDirectory + "/hips/spine/chest/chest1/shoulder.R/zero_upper_arm.R/upper_arm.R/ControlRightArm").GetComponent<ControlRightArmFlexion>();
    }

    
    public void CenterAvatar(int _index)
    {
        Vector3 _scaling = LoadedModel[_index].gameObject.transform.localScale;
        var _hipTranslations = Double.IsNaN(InitialFeetHeight) ? new Vector3(0f, 0f, 0f) : new Vector3(0f, -InitialFeetHeight * _scaling.y, 0f);
        var _hipRotations = new Vector3(0f, 0f, 0f);
        if (IsSimulationMode && qf != null)
        {
            _hipTranslations += new Vector3((float)qf[6] * _scaling.x, (float)qf[8] * _scaling.y, (float)qf[7] * _scaling.z);
            _hipRotations += new Vector3((float)qf[9] * Mathf.Rad2Deg, (float)qf[10] * Mathf.Rad2Deg, (float)qf[11] * Mathf.Rad2Deg);
        }
        LoadedModel[_index].Hip.transform.localPosition = _hipTranslations;
        LoadedModel[_index].Hip.transform.localEulerAngles = _hipRotations;
    }
}
