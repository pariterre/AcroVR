using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using Crosstales.FB;
using System.Globalization;


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
    public int Condition;


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
    protected BaseProfile profile;

    protected AvatarManager avatarManager;
    protected DrawManager drawManager;
    protected GameManager gameManager;
    protected MissionManager missionManager;
    protected UIManager uiManager;
    public MissionInfo mission;

	public string pathDataFiles;
	public string pathUserDocumentsFiles;
	public string pathUserSystemFiles;

    public int SelectedPresetCondition { get; protected set; } = 0;
    public void SetSelectedPresetCondition(int _value) { SelectedPresetCondition = _value; }
    public ConditionList PresetConditions { get; protected set; }

	string conditionJsonFileName;				// Répertoire et nom du fichier des conditions

	private void Start()
    {
        avatarManager = ToolBox.GetInstance().GetManager<AvatarManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
        missionManager = ToolBox.GetInstance().GetManager<MissionManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();

        System.Globalization.NumberFormatInfo nfi = new System.Globalization.NumberFormatInfo();
        nfi.NumberDecimalSeparator = ".";
        System.Globalization.CultureInfo ci = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        ci.NumberFormat = nfi;

		GetPathForDataFiles();

		// Copier le fichier des conditions dans un répertoire que l'utilisateur peut accéder en mode "Write".
		// Si utilisez via un fichier d'installation, alors le logiciel va s'installer dans un répertoire où l'utilisateur n'aura probablement pas d'accès "Write" (C:\Program Files).

		conditionJsonFileName = string.Format(@"{0}/Conditions.json", pathUserSystemFiles);
		try
		{
			File.Copy(string.Format(@"{0}/ConditionJson/Conditions.json", pathDataFiles), conditionJsonFileName, false);
		}
		catch (IOException e)
		{
			if (!e.Message.Contains("already exists"))
				Debug.Log("Erreur GameManager(Start): " + e.Message);
		}
		LoadConditions(conditionJsonFileName);
        LoadMissions(string.Format(@"{0}/MissionJson/Missions.json", pathDataFiles));
	}

    public int MissionLoad()
    {
        ExtensionFilter[] extensions = new[]
        {
            new ExtensionFilter(MainParameters.Instance.languages.Used.movementLoadDataFileAllFiles, "*" ),
        };

		string dirSimulationFiles = string.Format(@"{0}/SimulationJson", pathDataFiles);

		string fileName = FileBrowser.OpenSingleFile(MainParameters.Instance.languages.Used.movementLoadDataFileTitle, dirSimulationFiles, extensions);
        if (fileName.Length <= 0)
        {
            WriteToLogFile("fileName.Length false: " + fileName.Length.ToString());
            return -1;
        }

        WriteToLogFile(fileName);
        WriteToLogFile("CultureInfo.CurrentCulture.Name: " + CultureInfo.CurrentCulture.Name);

        string extension = GetSimpleExtension(fileName);
        if (extension == "txt")
        {
            if(!ReadDataFiles(0, fileName))
                return -2;
        }
        else
        {
            if (!ReadAniFromJson(0, fileName))
                return -3;
        }

        return 1;
    }

    public int LoadSimulationSecond()
    {
        ExtensionFilter[] extensions = new[]
        {
            new ExtensionFilter(MainParameters.Instance.languages.Used.movementLoadDataFileTxtFile, "*"),
            new ExtensionFilter(MainParameters.Instance.languages.Used.movementLoadDataFileAllFiles, "*" ),
        };

		string dirSimulationFiles = string.Format(@"{0}\SimulationJson", pathDataFiles);

		string fileName = FileBrowser.OpenSingleFile(MainParameters.Instance.languages.Used.movementLoadDataFileTitle, dirSimulationFiles, extensions);
        if (fileName.Length <= 0)
        {
            WriteToLogFile("fileName.Length false: " + fileName.Length.ToString());
            return -1;
        }

        WriteToLogFile(fileName);

        WriteToLogFile("CultureInfo.CurrentCulture.Name: " + CultureInfo.CurrentCulture.Name);

        string extension = GetSimpleExtension(fileName);
        if (extension == "txt")
        {
            if (!ReadDataFiles(1, fileName))
                return -2;
        }
        else
        {
            if (!ReadAniFromJson(1, fileName))
                return -3;
        }

        return 1;
    }

    private string GetSimpleExtension(string fileName)
    {
        return Path.GetExtension(fileName).Replace(".", "");
    }

    public void ReadDataFromJSON(string fileName)
    {
        WriteToLogFile("ReadDataFromJSON()");

        string dataAsJson = File.ReadAllText(fileName);
        mission = JsonUtility.FromJson<MissionInfo>(dataAsJson);
    }

    public void InitAnimationInfo()
    {
        avatarManager.LoadedModels[0].ResetJoints();
    }

    protected bool ReadAniFromJson(int _avatarIndex, string fileName)
    {
        WriteToLogFile("ReadAniFromJSON()");

        string dataAsJson = File.ReadAllText(fileName);

        if (dataAsJson[0] != '{')
        {
            WriteToLogFile("Parse Error [0]: " + dataAsJson[0]);
            return false;
        }

        AnimationInfo info = JsonUtility.FromJson<AnimationInfo>(dataAsJson);
        // TODO: Deal with these extra information
        Debug.Log("Deal with the info");
        //jointsTemp.Duration = info.Duration;
        //jointsTemp.UseGravity = info.UseGravity;
        //jointsTemp.StopOnGround = info.StopOnGround;

        MainParameters.StrucJoints jointsTemp = new MainParameters.StrucJoints();
        jointsTemp.fileName = fileName;
        jointsTemp.nodes = null;
        //jointsTemp.condition = info.Condition;
        //jointsTemp.takeOffParam.Somersault = info.Somersault;
        //jointsTemp.takeOffParam.Tilt = info.Tilt;
        //jointsTemp.takeOffParam.Twist = info.Twist;
        //jointsTemp.takeOffParam.HorizontalPosition = info.HorizontalPosition;
        //jointsTemp.takeOffParam.VerticalPosition = info.VerticalPosition;
        //jointsTemp.takeOffParam.SomersaultSpeed = info.SomersaultSpeed;
        //jointsTemp.takeOffParam.TiltSpeed = info.TiltSpeed;
        //jointsTemp.takeOffParam.TwistSpeed = info.TwistSpeed;
        //jointsTemp.takeOffParam.HorizontalSpeed = info.HorizontalSpeed;
        //jointsTemp.takeOffParam.VerticalSpeed = info.VerticalSpeed;

        jointsTemp.nodes = new MainParameters.StrucNodes[info.nodes.Count];

        WriteToLogFile("For() Start info.nodes.Count: " + info.nodes.Count.ToString());

        for (int i = 0; i < info.nodes.Count; i++)
        {
            jointsTemp.nodes[i].ddl = i + 1;
            jointsTemp.nodes[i].interpolation = MainParameters.Instance.interpolationDefault;
            jointsTemp.nodes[i].T = info.nodes[i].T;
            jointsTemp.nodes[i].Q = info.nodes[i].Q;
            jointsTemp.nodes[i].ddlOppositeSide = -1;
        }

        jointsTemp.lagrangianModel = new LagrangianModelSimple().GetParameters;

        avatarManager.LoadedModels[_avatarIndex].SetJoints(jointsTemp);

        return true;
    }

    public void SaveCondition(string name)
    {
        ConditionInfo n = new ConditionInfo();
        n.name = name;
        n.userInputsValues.SetAll(uiManager.userInputs);

        PresetConditions.conditions.Add(n);
        PresetConditions.count++;

        string jsonData = JsonUtility.ToJson(PresetConditions, true);
        File.WriteAllText(conditionJsonFileName, jsonData);
    }

    public void RemoveCondition(int index)
    {
        PresetConditions.conditions.RemoveAt(index);
        PresetConditions.count--;

        string jsonData = JsonUtility.ToJson(PresetConditions, true);
        File.WriteAllText(conditionJsonFileName, jsonData);
    }

    public bool LoadConditions(string fileName)
    {
		string dataAsJson = File.ReadAllText(fileName);

        if (dataAsJson[0] != '{')
        {
            WriteToLogFile("Parse Error [0]: " + dataAsJson[0]);
            return false;
        }

        PresetConditions = JsonUtility.FromJson<ConditionList>(dataAsJson);
        return true;
    }

    public bool LoadMissions(string fileName)
    {
        string dataAsJson = File.ReadAllText(fileName);

        if (dataAsJson[0] != '{')
        {
            WriteToLogFile("Parse Error [0]: " + dataAsJson[0]);
            return false;
        }

        missionManager.SetMissions(dataAsJson);

        return true;
    }

    private void WriteDataToJSON(string fileName)
    {
        AnimationInfo info = new AnimationInfo();

        info.Objective = "default";
        info.Duration = drawManager.TakeOffParameters.Duration;
        info.UseGravity = drawManager.TakeOffParameters.UseGravity;
        info.StopOnGround = drawManager.TakeOffParameters.StopOnGround;
        info.Condition = gameManager.SelectedPresetCondition;
        info.Somersault = drawManager.TakeOffParameters.Somersault;
        info.Tilt = drawManager.TakeOffParameters.Tilt;
        info.Twist = drawManager.TakeOffParameters.Twist;
        info.HorizontalPosition = drawManager.TakeOffParameters.HorizontalPosition;
        info.VerticalPosition = drawManager.TakeOffParameters.VerticalPosition;
        info.SomersaultSpeed = drawManager.TakeOffParameters.SomersaultSpeed;
        info.TiltSpeed = drawManager.TakeOffParameters.TiltSpeed;
        info.TwistSpeed = drawManager.TakeOffParameters.TwistSpeed;
        info.HorizontalSpeed = drawManager.TakeOffParameters.HorizontalSpeed;
        info.VerticalSpeed = drawManager.TakeOffParameters.VerticalSpeed;

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
        string fileLines = string.Format(
            "Duration: {0}{1}Condition: {2}{3}VerticalSpeed: {4:0.000}{5}AnteroposteriorSpeed: {6:0.000}{7}SomersaultSpeed: {8:0.000}{9}TwistSpeed: {10:0.000}{11}Tilt: {12:0.000}{13}Rotation: {14:0.000}{15}{16}",
            drawManager.TakeOffParameters.Duration, System.Environment.NewLine,
            gameManager.SelectedPresetCondition, System.Environment.NewLine,
            drawManager.TakeOffParameters.Somersault, System.Environment.NewLine,
            drawManager.TakeOffParameters.Tilt, System.Environment.NewLine,
            drawManager.TakeOffParameters.Twist, System.Environment.NewLine,
            drawManager.TakeOffParameters.HorizontalPosition, System.Environment.NewLine,
            drawManager.TakeOffParameters.VerticalPosition, System.Environment.NewLine,
            drawManager.TakeOffParameters.SomersaultSpeed, System.Environment.NewLine,
            drawManager.TakeOffParameters.TiltSpeed, System.Environment.NewLine,
            drawManager.TakeOffParameters.TwistSpeed, System.Environment.NewLine,
            drawManager.TakeOffParameters.HorizontalSpeed, System.Environment.NewLine,
            drawManager.TakeOffParameters.VerticalSpeed, System.Environment.NewLine
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

    protected bool ReadDataFiles(int _avatarIndex, string fileName)
    {
        WriteToLogFile("ReadDataFilesTxT()");

        // TODO Fix the file to read
        Debug.Log("Fix the file to read");
        string[] fileLines = System.IO.File.ReadAllLines(fileName);

        if(fileLines[0][0] == '{')
        {
            WriteToLogFile("Parse Error [0]: " + fileLines[0][0]);
            return false;
        }

        MainParameters.StrucTakeOffParam _takeOff = MainParameters.StrucTakeOffParam.Default;
        string[] values;
        int ddlNum = -1;

        WriteToLogFile("For() Start fileLines.Length: " + fileLines.Length.ToString());

        for (int i = 0; i < fileLines.Length; i++)
        {
            values = Regex.Split(fileLines[i], ":");

            WriteToLogFile("Regex.Split values: " + values[0]);

            if (values[0].Contains("Duration"))
            {
                WriteToLogFile("In Duration");
                var _duration = Utils.ToFloat(values[1]);
                if (_duration == -999)
                    _duration = MainParameters.StrucTakeOffParam.Default.Duration;
                drawManager.TakeOffParameters.Duration = _duration;
                WriteToLogFile("jointsTemp.Duration: " + drawManager.TakeOffParameters.Duration.ToString());
            }
            else if (values[0].Contains("UseGravity"))
            {
                WriteToLogFile("In UseGravity");
                drawManager.TakeOffParameters.UseGravity = Utils.ToBool(values[1]);
                WriteToLogFile("jointsTemp.UseGravity " + drawManager.TakeOffParameters.UseGravity.ToString());
            }
            else if (values[0].Contains("StopOnGround"))
            {
                WriteToLogFile("In StopOnGround");
                drawManager.TakeOffParameters.StopOnGround = Utils.ToBool(values[1]);
                WriteToLogFile("jointsTemp.StopOnGround " + drawManager.TakeOffParameters.StopOnGround.ToString());
            }
            else if (values[0].Contains("Condition"))
            {
                WriteToLogFile("In Condition");

                var _presetCondition = int.Parse(values[1], CultureInfo.InvariantCulture);
                if (_presetCondition == -999)
                    _presetCondition = MainParameters.StrucTakeOffParam.Default.PresetCondition;
                SetSelectedPresetCondition(_presetCondition);

                WriteToLogFile("jointsTemp.condition: " + PresetConditions.ToString());
            }
            else if (values[0].Contains("Somersault"))
            {
                WriteToLogFile("In Somerssault");

                var _somersault = Utils.ToFloat(values[1]);
                if (_somersault == -999)
                    _somersault = MainParameters.StrucTakeOffParam.Default.Somersault;
                _takeOff.Somersault = _somersault;

                WriteToLogFile("_takeOff.Somersault: " + _takeOff.Somersault.ToString());
            }
            else if (values[0].Contains("Tilt"))
            {
                WriteToLogFile("In Tilt");

                var _tilt = Utils.ToFloat(values[1]);
                if (_tilt == -999)
                    _tilt = MainParameters.StrucTakeOffParam.Default.Tilt;
                _takeOff.Tilt = _tilt;

                WriteToLogFile("_takeOff.Tilt: " + _takeOff.Tilt.ToString());
            }
            else if (values[0].Contains("Twist"))
            {
                WriteToLogFile("In Twist");

                var _twist = Utils.ToFloat(values[1]);
                if (_twist == -999)
                    _twist = MainParameters.StrucTakeOffParam.Default.Twist;
                _takeOff.Twist = _twist;

                WriteToLogFile("_takeOff.Twist: " + _takeOff.Twist.ToString());
            }
            else if (values[0].Contains("HorizontalPosition"))
            {
                WriteToLogFile("In HorizontalPosition");

                var _hPosition = Utils.ToFloat(values[1]);
                if (_hPosition == -999)
                    _hPosition = MainParameters.StrucTakeOffParam.Default.HorizontalPosition;
                _takeOff.HorizontalPosition = _hPosition;

                WriteToLogFile("_takeOff.HorizontalPosition: " + _takeOff.HorizontalPosition.ToString());
            }
            else if (values[0].Contains("VerticalPosition"))
            {
                WriteToLogFile("In VerticalPosition");
                var _vPosition = Utils.ToFloat(values[1]);
                if (_vPosition == -999)
                    _vPosition = MainParameters.StrucTakeOffParam.Default.VerticalPosition;
                _takeOff.VerticalPosition = _vPosition;

                WriteToLogFile("_takeOff.VerticalPosition: " + _takeOff.VerticalPosition.ToString());
            }
            else if (values[0].Contains("SomersaultSpeed"))
            {
                WriteToLogFile("In SomersaultSpeed");

                var _somersaultSpeed = Utils.ToFloat(values[1]);
                if (_somersaultSpeed == -999)
                    _somersaultSpeed = MainParameters.StrucTakeOffParam.Default.SomersaultSpeed;
                _takeOff.SomersaultSpeed = _somersaultSpeed;

                WriteToLogFile("_takeOff.SomersaultSpeed: " + _takeOff.SomersaultSpeed.ToString());
            }
            else if (values[0].Contains("TiltSpeed"))
            {
                WriteToLogFile("In TiltSpeed");

                var _tiltSpeed = Utils.ToFloat(values[1]);
                if (_tiltSpeed == -999)
                    _tiltSpeed = MainParameters.StrucTakeOffParam.Default.TiltSpeed;
                _takeOff.TiltSpeed = _tiltSpeed;

                WriteToLogFile("_takeOff.TiltSpeed: " + _takeOff.TiltSpeed.ToString());
            }
            else if (values[0].Contains("TwistSpeed"))
            {
                WriteToLogFile("In TwistSpeed");

                var _twistSpeed = Utils.ToFloat(values[1]);
                if (_twistSpeed == -999)
                    _twistSpeed = MainParameters.StrucTakeOffParam.Default.TwistSpeed;
                _takeOff.TwistSpeed = _twistSpeed;

                WriteToLogFile("_takeOff.TwistSpeed: " + _takeOff.TwistSpeed.ToString());
            }
            else if (values[0].Contains("HorizontalSpeed"))
            {
                WriteToLogFile("In HorizontalSpeed");

                var _hSpeed = Utils.ToFloat(values[1]);
                if (_hSpeed == -999)
                    _hSpeed = MainParameters.StrucTakeOffParam.Default.HorizontalSpeed;
                _takeOff.HorizontalSpeed = _hSpeed;

                WriteToLogFile("_takeOff.HorizontalSpeed: " + _takeOff.HorizontalSpeed.ToString());
            }
            else if (values[0].Contains("VerticalSpeed"))
            {
                WriteToLogFile("In VerticalSpeed");
                var _vSpeed = Utils.ToFloat(values[1]);
                if (_vSpeed == -999)
                    _vSpeed = MainParameters.StrucTakeOffParam.Default.VerticalSpeed;
                _takeOff.VerticalSpeed = _vSpeed;

                WriteToLogFile("_takeOff.VerticalSpeed: " + _takeOff.VerticalSpeed.ToString());
            }
            else if (values[0].Contains("DDL"))
            {
                WriteToLogFile("In DDL");

                avatarManager.LoadedModels[_avatarIndex].SetJointsNodes(new MainParameters.StrucNodes[fileLines.Length - i - 1]);
                ddlNum = 0;

                int temp = fileLines.Length - i - 1;

                WriteToLogFile("jointsTemp.nodes: " + temp.ToString());
            }
            else if (ddlNum >= 0)
            {
                WriteToLogFile("In ddlNum: " + ddlNum.ToString());

                avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].ddl = int.Parse(values[0], CultureInfo.InvariantCulture);

                WriteToLogFile("avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].ddl: " + avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].ddl.ToString());

                avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].name = values[1];

                WriteToLogFile("avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].name: " + avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].name);

                avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].interpolation = MainParameters.Instance.interpolationDefault;

                WriteToLogFile("avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].interpolation: " + avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].interpolation.type.ToString());

                int indexTQ = 2;

                WriteToLogFile("values.Length: " + values.Length.ToString());

                if (values.Length > 5)
                {
                    WriteToLogFile("In values.Length > 5");

                    string[] subValues;
                    subValues = Regex.Split(values[2], ",");
                    if (subValues[0].Contains(MainParameters.InterpolationType.CubicSpline.ToString()))
                        avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].interpolation.type = MainParameters.InterpolationType.CubicSpline;
                    else
                        avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].interpolation.type = MainParameters.InterpolationType.Quintic;
                    avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].interpolation.numIntervals = int.Parse(subValues[1], CultureInfo.InvariantCulture);
                    avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].interpolation.slope[0] = Utils.ToFloat(subValues[2]);
                    avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].interpolation.slope[1] = Utils.ToFloat(subValues[3]);
                    indexTQ++;
                }
                avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].T = ExtractDataTQ(values[indexTQ]);

                foreach(float a in avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].T)
                    WriteToLogFile("avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].T: " + a.ToString());

                avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].Q = ExtractDataTQ(values[indexTQ + 1]);

                foreach (float b in avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].Q)
                    WriteToLogFile("avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].Q: " + b.ToString());

                avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].ddlOppositeSide = -1;

                WriteToLogFile("avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].ddlOppositeSide: " + avatarManager.LoadedModels[_avatarIndex].Joints.nodes[ddlNum].ddlOppositeSide.ToString());

                ddlNum++;
            }
        }


        WriteToLogFile("Assigned drawManager.Joints(0)");
        
        if (avatarManager.LoadedModels[_avatarIndex].Joints.lagrangianModelName == MainParameters.LagrangianModelNames.Sasha23ddl)
        {
            WriteToLogFile("LagrangianModelSasha23ddl()");
            avatarManager.LoadedModels[_avatarIndex].SetJointsLagrangianModel(new LagrangianModelSasha23ddl().GetParameters);
        }
        else
        {
            WriteToLogFile("LagrangianModelSimple()");
            avatarManager.LoadedModels[_avatarIndex].SetJointsLagrangianModel(new LagrangianModelSimple().GetParameters);
        }

        int nDDL = 0;
        MainParameters.StrucNodes[] nodes = new MainParameters.StrucNodes[avatarManager.LoadedModels[_avatarIndex].Joints.lagrangianModel.q2.Length];
        int nNodes = avatarManager.LoadedModels[_avatarIndex].Joints.nodes.Length;
        MainParameters.StrucInterpolation interpolation = avatarManager.LoadedModels[_avatarIndex].Joints.nodes[0].interpolation;

        WriteToLogFile("For() Start avatarManager.LoadedModels[_avatarIndex].Joints.lagrangianModel.q2.Length: " + avatarManager.LoadedModels[_avatarIndex].Joints.lagrangianModel.q2.Length.ToString());

        foreach (int i in avatarManager.LoadedModels[_avatarIndex].Joints.lagrangianModel.q2)
        {
            int j = 0;
            string ddlname = avatarManager.LoadedModels[_avatarIndex].Joints.lagrangianModel.ddlName[i - 1].ToLower();
            while (j < nNodes && !ddlname.Contains(avatarManager.LoadedModels[_avatarIndex].Joints.nodes[j].name.ToLower()))
                j++;
            if (j < nNodes)                                 // Articulations défini dans le fichier de données, le conserver
            {
                nodes[nDDL] = avatarManager.LoadedModels[_avatarIndex].Joints.nodes[j];
                nodes[nDDL].ddl = i;
            }
            else                                            // Articulations non défini dans le fichier de données, alors utilisé la définition de défaut selon le modèle Lagrangien
            {
                nodes[nDDL].ddl = i;
                nodes[nDDL].name = avatarManager.LoadedModels[_avatarIndex].Joints.lagrangianModel.ddlName[i - 1];
                nodes[nDDL].T = new float[3] { 
                    drawManager.TakeOffParameters.Duration * 0.25f, 
                    drawManager.TakeOffParameters.Duration * 0.5f,
                    drawManager.TakeOffParameters.Duration * 0.75f 
                };
                nodes[nDDL].Q = new float[3] { 0, 0, 0 };
                nodes[nDDL].interpolation = interpolation;
                nodes[nDDL].ddlOppositeSide = -1;
            }
            nDDL++;
        }

        WriteToLogFile("For() Start nodes.Length: " + nodes.Length.ToString());

        for (int i = 0; i < nodes.Length; i++)
        {
            string nameOppSide = "";
            string name = nodes[i].name.ToLower();
            if (name.Contains("left") || name.Contains("right"))
            {
                if (name.Contains("left"))
                    nameOppSide = "right" + name.Substring(name.IndexOf("left") + 4);
                else
                    nameOppSide = "left" + name.Substring(name.IndexOf("right") + 5);
                for (int j = 0; j < nodes.Length; j++)
                {
                    name = nodes[j].name.ToLower();
                    if (name.Contains(nameOppSide))
                        nodes[i].ddlOppositeSide = j;
                }
            }
        }

        avatarManager.LoadedModels[_avatarIndex].SetJointsNodes(nodes);

        return true;
    }

    public void SaveFile()
    {
		string fileName = FileBrowser.SaveFile(MainParameters.Instance.languages.Used.movementSaveDataFileTitle, pathUserDocumentsFiles, "DefaultFile", "json");
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

    public void InterpolationDDL()
    {
        float[] t0;
        float[,] q0;
        GenerateQ0_s(avatarManager.LoadedModels[0].Joints, drawManager.TakeOffParameters.Duration, 0, out t0, out q0);

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

    public void WriteToLogFile(string msg)
    {
#if UNITY_EDITOR 
        using (System.IO.StreamWriter logFile = new System.IO.StreamWriter(@".\LogFile.txt", true))
        {
            System.DateTime dt = System.DateTime.Now;
            logFile.WriteLine(dt.ToString("yyyy-MM-dd HH:mm:ss") +": " + msg);
        }
#endif
    }

	// =================================================================================================================================================================
	/// <summary> Configuration des répertoires utilisés pour accéder aux différents fichiers de données, selon que la plateforme d'Unity utilisée (OSX, Windows, Editor). </summary>

	void GetPathForDataFiles()
	{
#if UNITY_STANDALONE_OSX                       // À modifier quand la plateforme OSX sera configuré, pas fait pour le moment
		//string dirSimulationFiles;
		//int n = Application.dataPath.IndexOf("/AcroVR.app");
		//if (n > 0)
		//	dirSimulationFiles = string.Format("{0}/SimulationFiles", Application.dataPath.Substring(0, n));
		//else
		//	dirSimulationFiles = string.Format("{0}/Documents", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
		//Debug.Log(string.Format("Mac: dirSimulationFiles = {0}", dirSimulationFiles));

		//string dirCheckFileName = string.Format("{0}/Documents/AcroVR/Lib", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
		//string checkFileName = string.Format("{0}/AcroVR.dll", dirCheckFileName);

		//string dirSimulationFiles = string.Format("{0}/Documents/AcroVR", Environment.GetFolderPath(Environment.SpecialFolder.Personal));

#elif UNITY_EDITOR

		pathDataFiles = string.Format(@"{0}/DataFiles", Application.dataPath);
		int i = pathDataFiles.IndexOf("/Assets");
		if (i >= 0)
			pathDataFiles = pathDataFiles.Remove(i, 7);
		if (!System.IO.Directory.Exists(pathDataFiles))
		{
			pathDataFiles = string.Format(@"{0}\S2M\AcroVR\DataFiles", System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles));
			if (!System.IO.Directory.Exists(pathDataFiles))
			{
				pathDataFiles = string.Format(@"{0}\S2M\AcroVR\DataFiles", System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86));
				if (!System.IO.Directory.Exists(pathDataFiles))
					pathDataFiles = "";
			}
		}

		pathUserSystemFiles = Application.persistentDataPath;
		pathUserDocumentsFiles = System.Environment.ExpandEnvironmentVariables(@"%UserProfile%\Documents\AcroVR");
#else
		pathDataFiles = string.Format(@"{0}\S2M\AcroVR\DataFiles", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
		if (!System.IO.Directory.Exists(pathDataFiles))
		{
			pathDataFiles = string.Format(@"{0}\S2M\AcroVR\DataFiles", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
			if (!System.IO.Directory.Exists(pathDataFiles))
			{
				int i = Application.dataPath.IndexOf("/Build");
				if (i >= 0)
				{
					pathDataFiles = string.Format(@"{0}/DataFiles", Application.dataPath.Remove(i));
					if (!System.IO.Directory.Exists(pathDataFiles))
						pathDataFiles = "";
				}
				else
					pathDataFiles = "";
			}
		}
		pathUserSystemFiles = Application.persistentDataPath;
		pathUserDocumentsFiles = System.Environment.ExpandEnvironmentVariables(@"%UserProfile%\Documents\AcroVR");
#endif
	}
}
