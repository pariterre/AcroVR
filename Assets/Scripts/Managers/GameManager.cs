using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using SFB;


[System.Serializable]
public struct AllMissionNodes
{
    public MissionNodes HipFlexion;
    public MissionNodes KneeFlexion;
    public MissionNodes RightArmFlexion;
    public MissionNodes RightArmAbduction;
    public MissionNodes LeftArmFlexion;
    public MissionNodes LeftArmAbduction;
}

[System.Serializable]
public struct MissionSolution
{
    // For each of the item, if it is null, then this item is ignored.
    // If the array is one element, then answer must equal the solution's value. 
    // If the array is two elements, then answer must be comprised between solution[0] and solution[1].

    // Solution on general parameters
    public float[] Duration;
    public bool[] UseGravity;
    public bool[] StopOnGround;

    // Solution on take off parameters
    public float[] Somersault;
    public float[] Tilt;
    public float[] Twist;
    public float[] HorizontalPosition;
    public float[] VerticalPosition;
    public float[] SomersaultSpeed;
    public float[] TiltSpeed;
    public float[] TwistSpeed;
    public float[] HorizontalSpeed;
    public float[] VerticalSpeed;
    
    // Solution on resulting computation
    public float[] TravelDistance; 
    public float[] HorizontalTravelDistance; 
    public float[] VerticalTravelDistance; 

    // Solution on angle nodes
    public AllMissionNodes Nodes;
}

[System.Serializable]
public struct MissionInfo
{
    public string Name;
    public int Level;

    public int PresetCondition;
    public UserUIInputsIsActive EnabledInputs;
    public AllMissionNodes StartingPositions;

    public MissionSolution Solution;
    public string Hint;

    public string ToHash(){
        return Hash128.Compute(Name).ToString();
    }
}

[System.Serializable]
public class MissionList
{
    public int count;
    public List<MissionInfo> missions = new List<MissionInfo>();
}

[System.Serializable]
public struct MissionNodes
{
    public Nodes Min;
    public Nodes Max;
}

[System.Serializable]
public struct Nodes
{
    public float[] T;
    public float[] Q;
}

[System.Serializable]
public class AnimationInfo
{
    public string Objective;
    public float Duration;
    public bool UseGravity;
    public bool StopOnGround;
    public float Somersault;
    public float Tilt;
    public float Twist;
    public float HorizontalPosition;
    public float VerticalPosition;
    public float SomersaultSpeed;
    public float TiltSpeed;
    public float TwistSpeed;
    public float HorizontalSpeed;
    public float VerticalSpeed;

    public List<Nodes> nodes = new List<Nodes>();
}

[System.Serializable]
public class ConditionList
{
    public int count;
    public List<ConditionInfo> conditions = new List<ConditionInfo>();
}

[System.Serializable]
public class ConditionInfo
{
    public string name;
    public UserUIInputsValues userInputsValues = new UserUIInputsValues();
}

public class GameManager : MonoBehaviour
{
    protected AvatarManager avatarManager;
    protected DrawManager drawManager;
    protected MissionManager missionManager;
    protected UIManager uiManager;
    public MissionInfo mission;

	public string TargetConfigFolder;
	public string SourceConfigFolder;

    public int SelectedPresetCondition { get; protected set; } = 0;
    public void SetSelectedPresetCondition(int _value) { SelectedPresetCondition = _value; }
    public ConditionList PresetConditions { get; protected set; }

	private void Start()
    {
        avatarManager = ToolBox.GetInstance().GetManager<AvatarManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        missionManager = ToolBox.GetInstance().GetManager<MissionManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();

        System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        System.Globalization.CultureInfo ci = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        ci.NumberFormat = nfi;

		GetPathForDataFiles();

        // Make a copy of the config folder so the user can safely modify them
        CopyJsonToTarget("Animations");
        CopyJsonToTarget("PresetConditions");
        CopyJsonToTarget("Missions");

		LoadConditions($"{TargetConfigFolder}/PresetConditions/PresetConditions.json");
        LoadMissions($"{SourceConfigFolder}/Missions/Missions.json");
	}

    public int AnimationLoad(int _avatarIndex)
    {
        ExtensionFilter[] extensions = new[] 
        { 
            new ExtensionFilter(MainParameters.Instance.languages.Used.movementLoadDataFileTxtFile, "json"), 
        }; 

		string dirAnimationFiles = $"{TargetConfigFolder}/Animations/";
        string[] fileNames = StandaloneFileBrowser.OpenFilePanel(
            MainParameters.Instance.languages.Used.movementLoadDataFileTitle, 
            dirAnimationFiles, 
            extensions, 
            false
        );
        if (fileNames.Length == 0) return -1;

        if (!ReadAniFromJson(_avatarIndex, fileNames[0]))
            return -3;

        return 1;
    }

    public void ReadDataFromJSON(string fileName)
    {
        string dataAsJson = File.ReadAllText(fileName);
        mission = JsonUtility.FromJson<MissionInfo>(dataAsJson);
    }

    public void InitAnimationInfo()
    {
        avatarManager.LoadedModels[0].ResetJoints();
    }

    protected bool ReadAniFromJson(int _avatarIndex, string fileName)
    {

        string dataAsJson = File.ReadAllText(fileName);

        if (dataAsJson[0] != '{') return false;

        AnimationInfo info = JsonUtility.FromJson<AnimationInfo>(dataAsJson);

        var _jointsTemp = avatarManager.LoadedModels[_avatarIndex].Joints;

        _jointsTemp.fileName = fileName;

        if (_avatarIndex == 0)
        {
            UserUIInputsValues _inputs = new UserUIInputsValues();
            _inputs.Duration = info.Duration;
            _inputs.UseGravity = info.UseGravity;
            _inputs.StopOnGround = info.StopOnGround;
            _inputs.Somersault = info.Somersault;
            _inputs.Tilt = info.Tilt;
            _inputs.Twist = info.Twist;
            _inputs.HorizontalPosition = info.HorizontalPosition;
            _inputs.VerticalPosition = info.VerticalPosition;
            _inputs.SomersaultSpeed = info.SomersaultSpeed;
            _inputs.TiltSpeed = info.TiltSpeed;
            _inputs.TwistSpeed = info.TwistSpeed;
            _inputs.HorizontalSpeed = info.HorizontalSpeed;
            _inputs.VerticalSpeed = info.VerticalSpeed;
            uiManager.userInputs.SetAll(_inputs);

        }
        else
        {
            drawManager.SetDuration(_avatarIndex, info.Duration);
            drawManager.avatarProperties[_avatarIndex].TakeOffParameters.UseGravity = info.UseGravity;
            drawManager.avatarProperties[_avatarIndex].TakeOffParameters.StopOnGround = info.StopOnGround;
            drawManager.avatarProperties[_avatarIndex].TakeOffParameters.Somersault = info.Somersault;
            drawManager.avatarProperties[_avatarIndex].TakeOffParameters.Tilt = info.Tilt;
            drawManager.avatarProperties[_avatarIndex].TakeOffParameters.Twist = info.Twist;
            drawManager.avatarProperties[_avatarIndex].TakeOffParameters.HorizontalPosition = info.HorizontalPosition;
            drawManager.avatarProperties[_avatarIndex].TakeOffParameters.VerticalPosition = info.VerticalPosition;
            drawManager.avatarProperties[_avatarIndex].TakeOffParameters.SomersaultSpeed = info.SomersaultSpeed;
            drawManager.avatarProperties[_avatarIndex].TakeOffParameters.TiltSpeed = info.TiltSpeed;
            drawManager.avatarProperties[_avatarIndex].TakeOffParameters.TwistSpeed = info.TwistSpeed;
            drawManager.avatarProperties[_avatarIndex].TakeOffParameters.HorizontalSpeed = info.HorizontalSpeed;
            drawManager.avatarProperties[_avatarIndex].TakeOffParameters.VerticalSpeed = info.VerticalSpeed;
        }


        _jointsTemp.nodes = new MainParameters.StrucNodes[info.nodes.Count];
        for (int i = 0; i < info.nodes.Count; i++)
        {
            _jointsTemp.nodes[i].ddl = i + 1;
            _jointsTemp.nodes[i].interpolation = MainParameters.Instance.interpolationDefault;
            _jointsTemp.nodes[i].T = info.nodes[i].T;
            _jointsTemp.nodes[i].Q = info.nodes[i].Q;
            _jointsTemp.nodes[i].ddlOppositeSide = -1;
        }

        _jointsTemp.lagrangianModel = new LagrangianModelSimple().GetParameters;

        avatarManager.LoadedModels[_avatarIndex].SetJoints(_jointsTemp);
        
        InterpolationDDL(_avatarIndex);
        for (int ddl=0; ddl==_jointsTemp.nodes.Length; ++ddl)
            DisplayDDL(ddl, true);  // In the event it was loaded while we were in the graph panel

        drawManager.PlayOneFrame(1);  // Force the avatar 1 to conform to its first frame

        return true;
    }

    public void UpdateDropDownNames()
    {
        if (uiManager.userInputs == null) return;

        uiManager.userInputs.PresetConditions.options.Clear();
        for (int i = 0; i < PresetConditions.count; i++)
        {
            uiManager.userInputs.PresetConditions.options.Add(new Dropdown.OptionData()
            {
                text = PresetConditions.conditions[i].name
            });
        }

        uiManager.UpdateAllPropertiesFromDropdown();
    }

    public void SaveCondition(string name)
    {
        ConditionInfo n = new ConditionInfo();
        n.name = name;
        n.userInputsValues.SetAll(uiManager.userInputs);

        PresetConditions.conditions.Add(n);
        PresetConditions.count++;

        string jsonData = JsonUtility.ToJson(PresetConditions, true);
        File.WriteAllText($"{TargetConfigFolder}/PresetConditions/PresetConditions.json", jsonData);
        UpdateDropDownNames();
    }

    public void RemoveCondition(int index)
    {
        PresetConditions.conditions.RemoveAt(index);
        PresetConditions.count--;

        string jsonData = JsonUtility.ToJson(PresetConditions, true);
        File.WriteAllText($"{TargetConfigFolder}/PresetConditions/PresetConditions.json", jsonData);
        UpdateDropDownNames();
    }

    public bool LoadConditions(string fileName)
    {
		string dataAsJson = File.ReadAllText(fileName);

        if (dataAsJson[0] != '{') return false;
        
        PresetConditions = JsonUtility.FromJson<ConditionList>(dataAsJson);
        UpdateDropDownNames();
        return true;
    }

    public bool LoadMissions(string fileName)
    {
        string dataAsJson = File.ReadAllText(fileName);

        if (dataAsJson[0] != '{') return false;

        missionManager.SetMissions(dataAsJson);

        return true;
    }

    private void WriteDataToJSON(string fileName)
    {
        AnimationInfo info = new AnimationInfo();
        var _takeOffParameters = drawManager.avatarProperties[0].TakeOffParameters;

        info.Objective = "default";
        info.Duration = _takeOffParameters.Duration;
        info.UseGravity = _takeOffParameters.UseGravity;
        info.StopOnGround = _takeOffParameters.StopOnGround;
        info.Somersault = _takeOffParameters.Somersault;
        info.Tilt = _takeOffParameters.Tilt;
        info.Twist = _takeOffParameters.Twist;
        info.HorizontalPosition = _takeOffParameters.HorizontalPosition;
        info.VerticalPosition = _takeOffParameters.VerticalPosition;
        info.SomersaultSpeed = _takeOffParameters.SomersaultSpeed;
        info.TiltSpeed = _takeOffParameters.TiltSpeed;
        info.TwistSpeed = _takeOffParameters.TwistSpeed;
        info.HorizontalSpeed = _takeOffParameters.HorizontalSpeed;
        info.VerticalSpeed = _takeOffParameters.VerticalSpeed;

        MainParameters.StrucJoints _joints = avatarManager.LoadedModels[0].Joints;
        for (int i = 0; i < _joints.nodes.Length; i++)
        {
            Nodes n = new Nodes();
            n.T = _joints.nodes[i].T;
            n.Q = _joints.nodes[i].Q;

            info.nodes.Add(n);
        }

        string jsonData = JsonUtility.ToJson(info, true);
        File.WriteAllText(fileName, jsonData);
    }

    public void WriteDataFiles_s(string fileName)
    {
        var _takeOffParameters = drawManager.avatarProperties[0].TakeOffParameters;

        string fileLines = string.Format(
            "Duration: {0}{1}Condition: {2}{3}VerticalSpeed: {4:0.000}{5}AnteroposteriorSpeed: {6:0.000}{7}SomersaultSpeed: {8:0.000}{9}TwistSpeed: {10:0.000}{11}Tilt: {12:0.000}{13}Rotation: {14:0.000}{15}{16}",
            _takeOffParameters.Duration, System.Environment.NewLine,
            _takeOffParameters.Somersault, System.Environment.NewLine,
            _takeOffParameters.Tilt, System.Environment.NewLine,
            _takeOffParameters.Twist, System.Environment.NewLine,
            _takeOffParameters.HorizontalPosition, System.Environment.NewLine,
            _takeOffParameters.VerticalPosition, System.Environment.NewLine,
            _takeOffParameters.SomersaultSpeed, System.Environment.NewLine,
            _takeOffParameters.TiltSpeed, System.Environment.NewLine,
            _takeOffParameters.TwistSpeed, System.Environment.NewLine,
            _takeOffParameters.HorizontalSpeed, System.Environment.NewLine,
            _takeOffParameters.VerticalSpeed, System.Environment.NewLine
        );

        fileLines = string.Format("{0}Nodes{1}DDL, name, interpolation (type, numIntervals, slopes), T, Q{2}", fileLines, System.Environment.NewLine, System.Environment.NewLine);

        var _nodes = avatarManager.LoadedModels[0].Joints.nodes;
        for (int i = 0; i < _nodes.Length; i++)
        {
            fileLines = string.Format("{0}{1}:{2}:{3},{4},{5:0.000000},{6:0.000000}:", fileLines, i + 1, _nodes[i].name, _nodes[i].interpolation.type,
                _nodes[i].interpolation.numIntervals, _nodes[i].interpolation.slope[0], _nodes[i].interpolation.slope[1]);
            for (int j = 0; j < _nodes[i].T.Length; j++)
            {
                if (j < _nodes[i].T.Length - 1)
                    fileLines = string.Format("{0}{1:0.000000},", fileLines, _nodes[i].T[j]);
                else
                    fileLines = string.Format("{0}{1:0.000000}:", fileLines, _nodes[i].T[j]);
            }
            for (int j = 0; j < _nodes[i].Q.Length; j++)
            {
                if (j < _nodes[i].Q.Length - 1)
                    fileLines = string.Format("{0}{1:0.000000},", fileLines, _nodes[i].Q[j]);
                else
                    fileLines = string.Format("{0}{1:0.000000}:{2}", fileLines, _nodes[i].Q[j], System.Environment.NewLine);
            }
        }

        System.IO.File.WriteAllText(fileName, fileLines);
    }

    public void SaveFile()
    {
        string dirSimulationFiles = $"{TargetConfigFolder}/Animations";
        string fileName = StandaloneFileBrowser.SaveFilePanel(
            MainParameters.Instance.languages.Used.movementSaveDataFileTitle, 
            dirSimulationFiles, 
            "CustomSimulation",
            "json"
        );

        if (fileName.Length <= 0)
            return;

        WriteDataToJSON(fileName);
    }

    private float[] ExtractDataTQ(string values)
    {
        string[] subValues = Regex.Split(values, ",");
        float[] data = new float[subValues.Length];
        for (int i = 0; i < subValues.Length; i++)
            data[i] = Utils.ToFloat(subValues[i]);
        return data;
    }

    public void InterpolationDDL(int _avatarIndex)
    {
        float[] t0;
        float[,] q0;
        GenerateQ0_s(avatarManager.LoadedModels[_avatarIndex].Joints, drawManager.avatarProperties[_avatarIndex].TakeOffParameters.Duration, 0, out t0, out q0);

        avatarManager.LoadedModels[0].SetJointsTandQ(t0, q0);
    }

    private void GenerateQ0_s(MainParameters.StrucJoints _joints, float tf, int qi, out float[] t0, out float[,] q0)
    {
        int[] ni;
        if (qi > 0)
            ni = new int[1] { qi };
        else
            ni = _joints.lagrangianModel.q2;

        float[] qd;
        int n = (int)(tf / _joints.lagrangianModel.dt)+1;
        t0 = new float[n];
        q0 = new float[_joints.lagrangianModel.nDDL, n];

        int i = 0;
        for (float interval = 0; interval < tf; interval += _joints.lagrangianModel.dt)
        {
            t0[i] = interval;
            Trajectory_ss(_joints, interval, ni, out qd);
            for (int ddl = 0; ddl < qd.Length; ddl++)
                q0[ddl, i] = qd[ddl];
            i++;

            if (i >= n) break;
        }
    }

    private void Trajectory_ss(MainParameters.StrucJoints _joints, float t, int[] qi, out float[] qd)
    {
        float[] qdotd;
        float[] qddotd;
        drawManager.Trajectory(_joints, t, qi, out qd, out qdotd, out qddotd);
    }

    public void DisplayDDL(int ddl, bool axisRange)
    {
        if (ddl >= 0)
        {
            transform.parent.GetComponentInChildren<AniGraphManager>().DisplayCurveAndNodes(0, ddl, axisRange);
            if (avatarManager.LoadedModels[0].Joints.nodes[ddl].ddlOppositeSide >= 0)
            {
                transform.parent.GetComponentInChildren<AniGraphManager>().DisplayCurveAndNodes(1, avatarManager.LoadedModels[0].Joints.nodes[ddl].ddlOppositeSide, true);
            }
        }
    }

	// =================================================================================================================================================================
	/// <summary> Configuration des répertoires utilisés pour accéder aux différents fichiers de données, selon que la plateforme d'Unity utilisée (OSX, Windows, Editor). </summary>

	void GetPathForDataFiles()
	{
		SourceConfigFolder = Application.streamingAssetsPath;
		TargetConfigFolder = Application.persistentDataPath;
	}

    protected void CopyJsonToTarget(string _subfolder)
    {
        var _sourcePath = $"{SourceConfigFolder}/{_subfolder}/";
        var _targetPath = $"{TargetConfigFolder}/{_subfolder}/";

        // Create all of the directories
        Directory.CreateDirectory(_targetPath);

        // Copy all the files if they don't already exist
        foreach (string newPath in Directory.GetFiles(_sourcePath, "*.json"))
        {
            try {
                File.Copy(newPath, newPath.Replace(_sourcePath, _targetPath), false);
            } catch (IOException){
                // If the file already exists, do nothing
            }
        }
    }
}
