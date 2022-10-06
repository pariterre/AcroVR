using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using Crosstales.FB;
using System.Globalization;


[System.Serializable]
public struct Goal
{
    public float[] Distance;
    public float[] Duration;
    public float[] VerticalDistance;
    public float[] Vrille;
    public float[] Salto;
}

[System.Serializable]
public struct SolutionConstraints
{
    public float[] SomersaultPosition;
    public float[] SomersaultSpeed;
    public float[] TwistPosition;
    public float[] TwistSpeed;
    public float[] TiltPosition;
    public float[] TiltSpeed;

    public float[] HorizontalPosition;
    public float[] HorizontalSpeed;
    public float[] VerticalPosition;
    public float[] VerticalSpeed;

    public float[] Duration;

    public MissionNodes HipFlexion;
    public MissionNodes KneeFlexion;
    public MissionNodes RightArmFlexion;
    public MissionNodes RightArmAbduction;
    public MissionNodes LeftArmFlexion;
    public MissionNodes LeftArmAbduction;
}


[System.Serializable]
public class UserUIInputs
{
    public InputField Duration;

    public InputField Somersault;
    public InputField Tilt;
    public InputField Twist;
    public InputField HorizontalPosition;
    public InputField VerticalPosition;
    public InputField SomersaultSpeed;
    public InputField TiltSpeed;
    public InputField TwistSpeed;
    public InputField HorizontalSpeed;
    public InputField VerticalSpeed;

    public Dropdown PresetConditions;
    public Toggle Gravity;
    public Toggle StopOnGround;

    public void SetPositions(UserUIInputsValues _values)
    {
        SetInput(Somersault, true, _values.Somersault);
        SetInput(Tilt, true, _values.Tilt);
        SetInput(Twist, true, _values.Twist);
        SetInput(HorizontalPosition, true, _values.HorizontalPosition);
        SetInput(VerticalPosition, true, _values.VerticalPosition);
    }

    public void SetAll(UserUIInputsValues _values)
    {
        SetInput(Duration, true, _values.Somersault);

        SetInput(Somersault, true, _values.Somersault);
        SetInput(Tilt, true, _values.Tilt);
        SetInput(Twist, true, _values.Twist);
        SetInput(HorizontalPosition, true, _values.HorizontalPosition);
        SetInput(VerticalPosition, true, _values.VerticalPosition);
        SetInput(SomersaultSpeed, true, _values.SomersaultSpeed);
        SetInput(TiltSpeed, true, _values.TiltSpeed);
        SetInput(TwistSpeed, true, _values.TwistSpeed);
        SetInput(HorizontalSpeed, true, _values.HorizontalSpeed);
        SetInput(VerticalSpeed, true, _values.VerticalSpeed);
    }

    public void SetAll(UserUIInputsIsActive _statuses, UserUIInputsValues _values)
    {

        SetInput(Duration, _statuses.Duration, _values.Somersault);

        SetInput(Somersault, _statuses.Somersault, _values.Somersault);
        SetInput(Tilt, _statuses.Tilt, _values.Tilt);
        SetInput(Twist, _statuses.Twist, _values.Twist);
        SetInput(HorizontalPosition, _statuses.HorizontalPosition, _values.HorizontalPosition);
        SetInput(VerticalPosition, _statuses.VerticalPosition, _values.VerticalPosition);
        SetInput(SomersaultSpeed, _statuses.SomersaultSpeed, _values.SomersaultSpeed);
        SetInput(TiltSpeed, _statuses.TiltSpeed, _values.TiltSpeed);
        SetInput(TwistSpeed, _statuses.TwistSpeed, _values.TwistSpeed);
        SetInput(HorizontalSpeed, _statuses.HorizontalSpeed, _values.HorizontalSpeed);
        SetInput(VerticalSpeed, _statuses.VerticalSpeed, _values.VerticalSpeed);
    }

    public void SetInput(InputField _field, bool _activate, string _value = "0.0")
    {
        _field.enabled = _activate;
        _field.image.color = _activate ? Color.white : Color.blue;
        _field.text = _value;
    }

    public void SetInput(Toggle _field, bool _activate, bool _value = false)
    {
        _field.enabled = _activate;
        _field.image.color = _activate ? Color.white : Color.blue;
        _field.isOn = _value;
    }
    public void SetInput(Dropdown _field, bool _activate, int _value = 0)
    {
        _field.enabled = _activate;
        _field.image.color = _activate ? Color.white : Color.blue;
        _field.value = _value;
    }
}

public class UserUIInputsValues
{
    public string Duration;

    public string Somersault;
    public string Tilt;
    public string Twist;
    public string HorizontalPosition;
    public string VerticalPosition;
    public string SomersaultSpeed;
    public string TiltSpeed;
    public string TwistSpeed;
    public string HorizontalSpeed;
    public string VerticalSpeed;

    public int PresetConditions;
    public bool Gravity;
    public bool StopOnGround;

    public void SetAll(UserUIInputs _inputs)
    {
        Duration = _inputs.Duration != null ? _inputs.Duration.text : "1.0";

        Somersault = _inputs.Somersault != null ? _inputs.Somersault.text : "0.0";
        Tilt = _inputs.Tilt != null ? _inputs.Tilt.text : "0.0";
        Twist = _inputs.Twist != null ? _inputs.Twist.text : "0.0";
        HorizontalPosition = _inputs.HorizontalPosition != null ? _inputs.HorizontalPosition.text : "0.0";
        VerticalPosition = _inputs.VerticalPosition != null ? _inputs.VerticalPosition.text : "0.0";
        SomersaultSpeed = _inputs.SomersaultSpeed != null ? _inputs.SomersaultSpeed.text : "0.0";
        TiltSpeed = _inputs.TiltSpeed != null ? _inputs.TiltSpeed.text : "0.0";
        TwistSpeed = _inputs.TwistSpeed != null ? _inputs.TwistSpeed.text : "0.0";
        HorizontalSpeed = _inputs.HorizontalSpeed != null ? _inputs.HorizontalSpeed.text : "0.0";
        VerticalSpeed = _inputs.VerticalSpeed != null ? _inputs.VerticalSpeed.text : "0.0";

        PresetConditions = _inputs.PresetConditions != null ? _inputs.PresetConditions.value : 0;
        Gravity = _inputs.Gravity != null ? _inputs.Gravity.isOn : true;
        StopOnGround = _inputs.StopOnGround != null ? _inputs.StopOnGround.isOn : false;
    }
}

[System.Serializable]
public struct UserUIInputsIsActive
{
    public bool Somersault;
    public bool SomersaultSpeed;
    public bool Tilt;
    public bool TiltSpeed;
    public bool Twist;
    public bool TwistSpeed;
    public bool HorizontalPosition;
    public bool HorizontalSpeed;
    public bool VerticalPosition;
    public bool VerticalSpeed;
    public bool Duration;
}

[System.Serializable]
public struct MissionInfo
{
    public int Level;
    public string Name;
    public Goal goal;
    public SolutionConstraints constraints;
    public int maxActions;
    public UserUIInputsIsActive enabledInputs;
    public int Condition;
    public string Hint;

    public MissionNodes HancheFlexion;
    public MissionNodes BrasGaucheAbduction;
    public MissionNodes BrasDroitAbduction;

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
    public string Name;
    public float[] T;
    public float[,] Q;
}

[System.Serializable]
public struct Nodes
{
    public string Name;
    public float[] T;
    public float[] Q;
}

[System.Serializable]
public class AnimationInfo
{
    public string Objective;
    public float Duration;
    public int Condition;
    public float VerticalSpeed;
    public float AnteroposteriorSpeed;
    public float SomersaultSpeed;
    public float TwistSpeed;
    public float Tilt;
    public float Rotation;

    /// New parameters
    public float TwistPosition;
    public float HorizontalPosition;
    public float VerticalPosition;
    public float TiltSpeed;
    ///////////////

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
    protected DrawManager drawManager;
    protected MissionManager missionManager;
    protected UIManager uiManager;
    public MissionInfo mission;

	public string pathDataFiles;
	public string pathUserDocumentsFiles;
	public string pathUserSystemFiles;

    public ConditionList listCondition;

	string conditionJsonFileName;				// Répertoire et nom du fichier des conditions

	private void Start()
    {
        missionManager = ToolBox.GetInstance().GetManager<MissionManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
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
            if(!ReadDataFiles_s(fileName))
                return -2;
        }
        else
        {
            if (!ReadAniFromJson(fileName))
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
            if (!ReadDataFileSecond(fileName))
                return -2;
        }
        else
        {
            if (!ReadAniFromJsonSecond(fileName))
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
        MainParameters.StrucJoints jointsTemp = new MainParameters.StrucJoints();
        jointsTemp.fileName = null;
        jointsTemp.nodes = null;
        jointsTemp.duration = 1;
        jointsTemp.condition = 0;
        jointsTemp.takeOffParam.verticalSpeed = 0;
        jointsTemp.takeOffParam.anteroposteriorSpeed = 0;
        jointsTemp.takeOffParam.somersaultSpeed = 0;
        jointsTemp.takeOffParam.twistSpeed = 0;
        jointsTemp.takeOffParam.tilt = 0;
        jointsTemp.takeOffParam.rotation = 0;

        jointsTemp.nodes = new MainParameters.StrucNodes[6];

        for (int i = 0; i < 6; i++)
        {
            jointsTemp.nodes[i].ddl = i + 1;

            //            jointsTemp.nodes[i].name = null;

            if (i == 0) jointsTemp.nodes[i].name = "Hip_Flexion";
            else if (i == 1) jointsTemp.nodes[i].name = "Knee_Flexion";
            else if (i == 2) jointsTemp.nodes[i].name = "Right_Arm_Flexion";
            else if (i == 3) jointsTemp.nodes[i].name = "Right_Arm_Abduction";
            else if (i == 4) jointsTemp.nodes[i].name = "Left_Arm_Flexion";
            else if (i == 5) jointsTemp.nodes[i].name = "Left_Arm_Abduction";


            jointsTemp.nodes[i].interpolation = MainParameters.Instance.interpolationDefault;
//            jointsTemp.nodes[i].T = new float[] { 0, 0.0001f};
            jointsTemp.nodes[i].T = new float[] { 0, 1.000000f };
            jointsTemp.nodes[i].Q = new float[] { 0, 0.0f};
            jointsTemp.nodes[i].ddlOppositeSide = -1;
        }

        MainParameters.Instance.joints = jointsTemp;

        LagrangianModelSimple lagrangianModelSimple = new LagrangianModelSimple();
        MainParameters.Instance.joints.lagrangianModel = lagrangianModelSimple.GetParameters;
    }

    private bool ReadAniFromJson(string fileName)
    {
        WriteToLogFile("ReadAniFromJSON()");

        string dataAsJson = File.ReadAllText(fileName);

        if (dataAsJson[0] != '{')
        {
            WriteToLogFile("Parse Error [0]: " + dataAsJson[0]);
            return false;
        }

        AnimationInfo info = JsonUtility.FromJson<AnimationInfo>(dataAsJson);

        MainParameters.StrucJoints jointsTemp = new MainParameters.StrucJoints();
        jointsTemp.fileName = fileName;
        jointsTemp.nodes = null;
        jointsTemp.duration = info.Duration;
        jointsTemp.condition = info.Condition;
        jointsTemp.takeOffParam.verticalSpeed = info.VerticalSpeed;
        jointsTemp.takeOffParam.anteroposteriorSpeed = info.AnteroposteriorSpeed;
        jointsTemp.takeOffParam.somersaultSpeed = info.SomersaultSpeed;
        jointsTemp.takeOffParam.twistSpeed = info.TwistSpeed;
        jointsTemp.takeOffParam.tilt = info.Tilt;
        jointsTemp.takeOffParam.rotation = info.Rotation;


        ////////////////
        drawManager.takeOffParamTwistPosition = info.TwistPosition;
        drawManager.takeOffParamHorizontalPosition = info.HorizontalPosition;
        drawManager.takeOffParamVerticalPosition = info.VerticalPosition;
        drawManager.takeOffParamTiltSpeed = info.TiltSpeed;
        ////////////////////


        jointsTemp.nodes = new MainParameters.StrucNodes[info.nodes.Count];

        WriteToLogFile("For() Start info.nodes.Count: " + info.nodes.Count.ToString());

        for (int i = 0; i < info.nodes.Count; i++)
        {
            jointsTemp.nodes[i].ddl = i + 1;
            jointsTemp.nodes[i].name = info.nodes[i].Name;
            jointsTemp.nodes[i].interpolation = MainParameters.Instance.interpolationDefault;
            jointsTemp.nodes[i].T = info.nodes[i].T;
            jointsTemp.nodes[i].Q = info.nodes[i].Q;
            jointsTemp.nodes[i].ddlOppositeSide = -1;
        }

        MainParameters.Instance.joints = jointsTemp;

        LagrangianModelSimple lagrangianModelSimple = new LagrangianModelSimple();
        MainParameters.Instance.joints.lagrangianModel = lagrangianModelSimple.GetParameters;

        return true;
    }

    private bool ReadAniFromJsonSecond(string fileName)
    {
        WriteToLogFile("ReadAniFromJsonSecond()");

        string dataAsJson = File.ReadAllText(fileName);

        if (dataAsJson[0] != '{')
        {
            WriteToLogFile("Parse Error [0]: " + dataAsJson[0]);
            return false;
        }

        AnimationInfo info = JsonUtility.FromJson<AnimationInfo>(dataAsJson);

        AvatarSimulation.StrucJoints jointsTemp = new AvatarSimulation.StrucJoints();
        jointsTemp.fileName = fileName;
        jointsTemp.nodes = null;
        jointsTemp.duration = info.Duration;
        jointsTemp.condition = info.Condition;
        jointsTemp.takeOffParam.verticalSpeed = info.VerticalSpeed;
        jointsTemp.takeOffParam.anteroposteriorSpeed = info.AnteroposteriorSpeed;
        jointsTemp.takeOffParam.somersaultSpeed = info.SomersaultSpeed;
        jointsTemp.takeOffParam.twistSpeed = info.TwistSpeed;
        jointsTemp.takeOffParam.tilt = info.Tilt;
        jointsTemp.takeOffParam.rotation = info.Rotation;


        ////////////////
        drawManager.takeOffParamTwistPosition = info.TwistPosition;
        drawManager.takeOffParamHorizontalPosition = info.HorizontalPosition;
        drawManager.takeOffParamVerticalPosition = info.VerticalPosition;
        drawManager.takeOffParamTiltSpeed = info.TiltSpeed;
        ////////////////////


        jointsTemp.nodes = new AvatarSimulation.StrucNodes[info.nodes.Count];

        WriteToLogFile("For() Start info.nodes.Count: " + info.nodes.Count.ToString());

        for (int i = 0; i < info.nodes.Count; i++)
        {
            jointsTemp.nodes[i].ddl = i + 1;
            jointsTemp.nodes[i].name = info.nodes[i].Name;
            jointsTemp.nodes[i].interpolation = drawManager.secondParameters.interpolationDefault;
            jointsTemp.nodes[i].T = info.nodes[i].T;
            jointsTemp.nodes[i].Q = info.nodes[i].Q;
            jointsTemp.nodes[i].ddlOppositeSide = -1;
        }

        drawManager.secondParameters.joints = jointsTemp;

        LagrangianModelSimple lagrangianModelSimple = new LagrangianModelSimple();
        drawManager.secondParameters.joints.lagrangianModel = lagrangianModelSimple.GetParameters;

        return true;
    }

    public void SaveCondition(string name)
    {
        ConditionInfo n = new ConditionInfo();
        n.name = name;
        n.userInputsValues.SetAll(uiManager.userInputs);

        listCondition.conditions.Add(n);
        listCondition.count++;

        string jsonData = JsonUtility.ToJson(listCondition, true);
        File.WriteAllText(conditionJsonFileName, jsonData);
    }

    public void RemoveCondition(int index)
    {
        listCondition.conditions.RemoveAt(index);
        listCondition.count--;

        string jsonData = JsonUtility.ToJson(listCondition, true);
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

        listCondition = JsonUtility.FromJson<ConditionList>(dataAsJson);
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

        info.Objective = "defalut";
        info.Duration = MainParameters.Instance.joints.duration;
        info.Condition = MainParameters.Instance.joints.condition;
        info.VerticalSpeed = MainParameters.Instance.joints.takeOffParam.verticalSpeed;
        info.AnteroposteriorSpeed = MainParameters.Instance.joints.takeOffParam.anteroposteriorSpeed;
        info.SomersaultSpeed = MainParameters.Instance.joints.takeOffParam.somersaultSpeed;
        info.TwistSpeed = MainParameters.Instance.joints.takeOffParam.twistSpeed;
        info.Tilt = MainParameters.Instance.joints.takeOffParam.tilt;
        info.Rotation = MainParameters.Instance.joints.takeOffParam.rotation;


        ////////////////
        info.TwistPosition = drawManager.takeOffParamTwistPosition;
        info.HorizontalPosition = drawManager.takeOffParamHorizontalPosition;
        info.VerticalPosition = drawManager.takeOffParamVerticalPosition;
        info.TiltSpeed = drawManager.takeOffParamTiltSpeed;
        ////////////////////


        for (int i = 0; i < MainParameters.Instance.joints.nodes.Length; i++)
        {
            Nodes n = new Nodes();
            n.Name = MainParameters.Instance.joints.nodes[i].name;
            n.T = MainParameters.Instance.joints.nodes[i].T;
            n.Q = MainParameters.Instance.joints.nodes[i].Q;

            info.nodes.Add(n);
        }

        string jsonData = JsonUtility.ToJson(info, true);
        File.WriteAllText(fileName, jsonData);
    }

    public void WriteDataFiles_s(string fileName)
    {
        string fileLines = string.Format(
            "Duration: {0}{1}Condition: {2}{3}VerticalSpeed: {4:0.000}{5}AnteroposteriorSpeed: {6:0.000}{7}SomersaultSpeed: {8:0.000}{9}TwistSpeed: {10:0.000}{11}Tilt: {12:0.000}{13}Rotation: {14:0.000}{15}{16}",
            MainParameters.Instance.joints.duration, System.Environment.NewLine,
            MainParameters.Instance.joints.condition, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.verticalSpeed, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.anteroposteriorSpeed, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.somersaultSpeed, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.twistSpeed, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.tilt, System.Environment.NewLine,
            MainParameters.Instance.joints.takeOffParam.rotation, System.Environment.NewLine, System.Environment.NewLine);

        fileLines = string.Format("{0}Nodes{1}DDL, name, interpolation (type, numIntervals, slopes), T, Q{2}", fileLines, System.Environment.NewLine, System.Environment.NewLine);

        for (int i = 0; i < MainParameters.Instance.joints.nodes.Length; i++)
        {
            fileLines = string.Format("{0}{1}:{2}:{3},{4},{5:0.000000},{6:0.000000}:", fileLines, i + 1, MainParameters.Instance.joints.nodes[i].name, MainParameters.Instance.joints.nodes[i].interpolation.type,
                MainParameters.Instance.joints.nodes[i].interpolation.numIntervals, MainParameters.Instance.joints.nodes[i].interpolation.slope[0], MainParameters.Instance.joints.nodes[i].interpolation.slope[1]);
            for (int j = 0; j < MainParameters.Instance.joints.nodes[i].T.Length; j++)
            {
                if (j < MainParameters.Instance.joints.nodes[i].T.Length - 1)
                    fileLines = string.Format("{0}{1:0.000000},", fileLines, MainParameters.Instance.joints.nodes[i].T[j]);
                else
                    fileLines = string.Format("{0}{1:0.000000}:", fileLines, MainParameters.Instance.joints.nodes[i].T[j]);
            }
            for (int j = 0; j < MainParameters.Instance.joints.nodes[i].Q.Length; j++)
            {
                if (j < MainParameters.Instance.joints.nodes[i].Q.Length - 1)
                    fileLines = string.Format("{0}{1:0.000000},", fileLines, MainParameters.Instance.joints.nodes[i].Q[j]);
                else
                    fileLines = string.Format("{0}{1:0.000000}:{2}", fileLines, MainParameters.Instance.joints.nodes[i].Q[j], System.Environment.NewLine);
            }
        }

        System.IO.File.WriteAllText(fileName, fileLines);
    }

    private bool ReadDataFiles_s(string fileName)
    {
        WriteToLogFile("ReadDataFilesTxT()");

        string[] fileLines = System.IO.File.ReadAllLines(fileName);

        if(fileLines[0][0] == '{')
        {
            WriteToLogFile("Parse Error [0]: " + fileLines[0][0]);
            return false;
        }

        MainParameters.StrucJoints jointsTemp = new MainParameters.StrucJoints();
        jointsTemp.fileName = fileName;
        jointsTemp.nodes = null;
        jointsTemp.duration = 0;
        jointsTemp.condition = 0;
        jointsTemp.takeOffParam.verticalSpeed = 0;
        jointsTemp.takeOffParam.anteroposteriorSpeed = 0;
        jointsTemp.takeOffParam.somersaultSpeed = 0;
        jointsTemp.takeOffParam.twistSpeed = 0;
        jointsTemp.takeOffParam.tilt = 0;
        jointsTemp.takeOffParam.rotation = 0;

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

                jointsTemp.duration = Utils.ToFloat(values[1]);
                if (jointsTemp.duration == -999)
                    jointsTemp.duration = MainParameters.Instance.durationDefault;

                WriteToLogFile("jointsTemp.duration: " + jointsTemp.duration.ToString());
            }
            else if (values[0].Contains("Condition"))
            {
                WriteToLogFile("In Condition");

                jointsTemp.condition = int.Parse(values[1], CultureInfo.InvariantCulture);
                if (jointsTemp.condition == -999)
                    jointsTemp.condition = MainParameters.Instance.conditionDefault;

                WriteToLogFile("jointsTemp.condition: " + jointsTemp.condition.ToString());
            }
            else if (values[0].Contains("VerticalSpeed"))
            {
                WriteToLogFile("In VerticalSpeed");

                jointsTemp.takeOffParam.verticalSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.verticalSpeed == -999)
                    jointsTemp.takeOffParam.verticalSpeed = MainParameters.Instance.takeOffParamDefault.verticalSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.verticalSpeed: " + jointsTemp.takeOffParam.verticalSpeed.ToString());
            }
            else if (values[0].Contains("AnteroposteriorSpeed"))
            {
                WriteToLogFile("In AnteroposteriorSpeed");

                jointsTemp.takeOffParam.anteroposteriorSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.anteroposteriorSpeed == -999)
                    jointsTemp.takeOffParam.anteroposteriorSpeed = MainParameters.Instance.takeOffParamDefault.anteroposteriorSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.anteroposteriorSpeed: " + jointsTemp.takeOffParam.anteroposteriorSpeed.ToString());
            }
            else if (values[0].Contains("SomersaultSpeed"))
            {
                WriteToLogFile("In SomersaultSpeed");

                jointsTemp.takeOffParam.somersaultSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.somersaultSpeed == -999)
                    jointsTemp.takeOffParam.somersaultSpeed = MainParameters.Instance.takeOffParamDefault.somersaultSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.somersaultSpeed: " + jointsTemp.takeOffParam.somersaultSpeed.ToString());
            }
            else if (values[0].Contains("TwistSpeed"))
            {
                WriteToLogFile("In TwistSpeed");

                jointsTemp.takeOffParam.twistSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.twistSpeed == -999)
                    jointsTemp.takeOffParam.twistSpeed = MainParameters.Instance.takeOffParamDefault.twistSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.twistSpeed: " + jointsTemp.takeOffParam.twistSpeed.ToString());
            }
            else if (values[0].Contains("Tilt"))
            {
                WriteToLogFile("In Tilt");

                jointsTemp.takeOffParam.tilt = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.tilt == -999)
                    jointsTemp.takeOffParam.tilt = MainParameters.Instance.takeOffParamDefault.tilt;

                WriteToLogFile("jointsTemp.takeOffParam.tilt: " + jointsTemp.takeOffParam.tilt.ToString());
            }
            else if (values[0].Contains("Rotation"))
            {
                WriteToLogFile("In Rotation");

                jointsTemp.takeOffParam.rotation = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.rotation == -999)
                    jointsTemp.takeOffParam.rotation = MainParameters.Instance.takeOffParamDefault.rotation;

                WriteToLogFile("jointsTemp.takeOffParam.rotation: " + jointsTemp.takeOffParam.rotation.ToString());
            }
            else if (values[0].Contains("DDL"))
            {
                WriteToLogFile("In DDL");

                jointsTemp.nodes = new MainParameters.StrucNodes[fileLines.Length - i - 1];
                ddlNum = 0;

                int temp = fileLines.Length - i - 1;

                WriteToLogFile("jointsTemp.nodes: " + temp.ToString());
            }
            else if (ddlNum >= 0)
            {
                WriteToLogFile("In ddlNum: " + ddlNum.ToString());

                jointsTemp.nodes[ddlNum].ddl = int.Parse(values[0], CultureInfo.InvariantCulture);

                WriteToLogFile("jointsTemp.nodes[ddlNum].ddl: " + jointsTemp.nodes[ddlNum].ddl.ToString());

                jointsTemp.nodes[ddlNum].name = values[1];

                WriteToLogFile("jointsTemp.nodes[ddlNum].name: " + jointsTemp.nodes[ddlNum].name);

                jointsTemp.nodes[ddlNum].interpolation = MainParameters.Instance.interpolationDefault;

                WriteToLogFile("jointsTemp.nodes[ddlNum].interpolation: " + jointsTemp.nodes[ddlNum].interpolation.type.ToString());

                int indexTQ = 2;

                WriteToLogFile("values.Length: " + values.Length.ToString());

                if (values.Length > 5)
                {
                    WriteToLogFile("In values.Length > 5");

                    string[] subValues;
                    subValues = Regex.Split(values[2], ",");
                    if (subValues[0].Contains(MainParameters.InterpolationType.CubicSpline.ToString()))
                        jointsTemp.nodes[ddlNum].interpolation.type = MainParameters.InterpolationType.CubicSpline;
                    else
                        jointsTemp.nodes[ddlNum].interpolation.type = MainParameters.InterpolationType.Quintic;
                    jointsTemp.nodes[ddlNum].interpolation.numIntervals = int.Parse(subValues[1], CultureInfo.InvariantCulture);
                    jointsTemp.nodes[ddlNum].interpolation.slope[0] = Utils.ToFloat(subValues[2]);
                    jointsTemp.nodes[ddlNum].interpolation.slope[1] = Utils.ToFloat(subValues[3]);
                    indexTQ++;
                }
                jointsTemp.nodes[ddlNum].T = ExtractDataTQ(values[indexTQ]);

                foreach(float a in jointsTemp.nodes[ddlNum].T)
                    WriteToLogFile("jointsTemp.nodes[ddlNum].T: " + a.ToString());

                jointsTemp.nodes[ddlNum].Q = ExtractDataTQ(values[indexTQ + 1]);

                foreach (float b in jointsTemp.nodes[ddlNum].Q)
                    WriteToLogFile("jointsTemp.nodes[ddlNum].Q: " + b.ToString());

                jointsTemp.nodes[ddlNum].ddlOppositeSide = -1;

                WriteToLogFile("jointsTemp.nodes[ddlNum].ddlOppositeSide: " + jointsTemp.nodes[ddlNum].ddlOppositeSide.ToString());

                ddlNum++;
            }
        }

        MainParameters.Instance.joints = jointsTemp;

        WriteToLogFile("Assigned MainParameters.Instance.joints");

        //        LagrangianModelSimple lagrangianModelSimple = new LagrangianModelSimple();
        //        MainParameters.Instance.joints.lagrangianModel = lagrangianModelSimple.GetParameters;

        if (MainParameters.Instance.joints.lagrangianModelName == MainParameters.LagrangianModelNames.Sasha23ddl)
        {
            WriteToLogFile("LagrangianModelSasha23ddl()");

            LagrangianModelSasha23ddl lagrangianModelSasha23ddl = new LagrangianModelSasha23ddl();
            MainParameters.Instance.joints.lagrangianModel = lagrangianModelSasha23ddl.GetParameters;
        }
        else
        {
            WriteToLogFile("LagrangianModelSimple()");

            LagrangianModelSimple lagrangianModelSimple = new LagrangianModelSimple();
            MainParameters.Instance.joints.lagrangianModel = lagrangianModelSimple.GetParameters;
        }

        MainParameters.StrucJoints joints = MainParameters.Instance.joints;

        int nDDL = 0;
        MainParameters.StrucNodes[] nodes = new MainParameters.StrucNodes[joints.lagrangianModel.q2.Length];
        int nNodes = joints.nodes.Length;
        MainParameters.StrucInterpolation interpolation = joints.nodes[0].interpolation;

        WriteToLogFile("For() Start joints.lagrangianModel.q2.Length: " + joints.lagrangianModel.q2.Length.ToString());

        foreach (int i in joints.lagrangianModel.q2)
        {
            int j = 0;
            string ddlname = joints.lagrangianModel.ddlName[i - 1].ToLower();
            while (j < nNodes && !ddlname.Contains(joints.nodes[j].name.ToLower()))
                j++;
            if (j < nNodes)                                 // Articulations défini dans le fichier de données, le conserver
            {
                nodes[nDDL] = joints.nodes[j];
                nodes[nDDL].ddl = i;
            }
            else                                            // Articulations non défini dans le fichier de données, alors utilisé la définition de défaut selon le modèle Lagrangien
            {
                nodes[nDDL].ddl = i;
                nodes[nDDL].name = joints.lagrangianModel.ddlName[i - 1];
                nodes[nDDL].T = new float[3] { joints.duration * 0.25f, joints.duration * 0.5f, joints.duration * 0.75f };
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

        MainParameters.Instance.joints.nodes = nodes;

        return true;
    }

    private bool ReadDataFileSecond(string fileName)
    {
        WriteToLogFile("ReadDataFileSecondTxt()");

        string[] fileLines = System.IO.File.ReadAllLines(fileName);

        if (fileLines[0][0] == '{')
        {
            WriteToLogFile("Parse Error [0]: " + fileLines[0][0]);
            return false;
        }

        AvatarSimulation.StrucJoints jointsTemp = new AvatarSimulation.StrucJoints();
        jointsTemp.fileName = fileName;
        jointsTemp.nodes = null;
        jointsTemp.duration = 0;
        jointsTemp.condition = 0;
        jointsTemp.takeOffParam.verticalSpeed = 0;
        jointsTemp.takeOffParam.anteroposteriorSpeed = 0;
        jointsTemp.takeOffParam.somersaultSpeed = 0;
        jointsTemp.takeOffParam.twistSpeed = 0;
        jointsTemp.takeOffParam.tilt = 0;
        jointsTemp.takeOffParam.rotation = 0;

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

                jointsTemp.duration = Utils.ToFloat(values[1]);
                if (jointsTemp.duration == -999)
                    jointsTemp.duration = MainParameters.Instance.durationDefault;

                WriteToLogFile("jointsTemp.duration: " + jointsTemp.duration.ToString());
            }
            else if (values[0].Contains("Condition"))
            {
                WriteToLogFile("In Condition");

                jointsTemp.condition = int.Parse(values[1], CultureInfo.InvariantCulture);
                if (jointsTemp.condition == -999)
                    jointsTemp.condition = MainParameters.Instance.conditionDefault;

                WriteToLogFile("jointsTemp.condition: " + jointsTemp.condition.ToString());
            }
            else if (values[0].Contains("VerticalSpeed"))
            {
                WriteToLogFile("In VerticalSpeed");

                jointsTemp.takeOffParam.verticalSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.verticalSpeed == -999)
                    jointsTemp.takeOffParam.verticalSpeed = MainParameters.Instance.takeOffParamDefault.verticalSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.verticalSpeed: " + jointsTemp.takeOffParam.verticalSpeed.ToString());
            }
            else if (values[0].Contains("AnteroposteriorSpeed"))
            {
                WriteToLogFile("In AnteroposteriorSpeed");

                jointsTemp.takeOffParam.anteroposteriorSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.anteroposteriorSpeed == -999)
                    jointsTemp.takeOffParam.anteroposteriorSpeed = MainParameters.Instance.takeOffParamDefault.anteroposteriorSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.anteroposteriorSpeed: " + jointsTemp.takeOffParam.anteroposteriorSpeed.ToString());
            }
            else if (values[0].Contains("SomersaultSpeed"))
            {
                WriteToLogFile("In SomersaultSpeed");

                jointsTemp.takeOffParam.somersaultSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.somersaultSpeed == -999)
                    jointsTemp.takeOffParam.somersaultSpeed = MainParameters.Instance.takeOffParamDefault.somersaultSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.somersaultSpeed: " + jointsTemp.takeOffParam.somersaultSpeed.ToString());
            }
            else if (values[0].Contains("TwistSpeed"))
            {
                WriteToLogFile("In TwistSpeed");

                jointsTemp.takeOffParam.twistSpeed = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.twistSpeed == -999)
                    jointsTemp.takeOffParam.twistSpeed = MainParameters.Instance.takeOffParamDefault.twistSpeed;

                WriteToLogFile("jointsTemp.takeOffParam.twistSpeed: " + jointsTemp.takeOffParam.twistSpeed.ToString());
            }
            else if (values[0].Contains("Tilt"))
            {
                WriteToLogFile("In Tilt");

                jointsTemp.takeOffParam.tilt = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.tilt == -999)
                    jointsTemp.takeOffParam.tilt = MainParameters.Instance.takeOffParamDefault.tilt;

                WriteToLogFile("jointsTemp.takeOffParam.tilt: " + jointsTemp.takeOffParam.tilt.ToString());
            }
            else if (values[0].Contains("Rotation"))
            {
                WriteToLogFile("In Rotation");

                jointsTemp.takeOffParam.rotation = Utils.ToFloat(values[1]);
                if (jointsTemp.takeOffParam.rotation == -999)
                    jointsTemp.takeOffParam.rotation = MainParameters.Instance.takeOffParamDefault.rotation;

                WriteToLogFile("jointsTemp.takeOffParam.rotation: " + jointsTemp.takeOffParam.rotation.ToString());
            }
            else if (values[0].Contains("DDL"))
            {
                WriteToLogFile("In DDL");

                jointsTemp.nodes = new AvatarSimulation.StrucNodes[fileLines.Length - i - 1];
                ddlNum = 0;

                int temp = fileLines.Length - i - 1;

                WriteToLogFile("jointsTemp.nodes: " + temp.ToString());
            }
            else if (ddlNum >= 0)
            {
                WriteToLogFile("In ddlNum: " + ddlNum.ToString());

                jointsTemp.nodes[ddlNum].ddl = int.Parse(values[0], CultureInfo.InvariantCulture);

                WriteToLogFile("jointsTemp.nodes[ddlNum].ddl: " + jointsTemp.nodes[ddlNum].ddl.ToString());

                jointsTemp.nodes[ddlNum].name = values[1];

                WriteToLogFile("jointsTemp.nodes[ddlNum].name: " + jointsTemp.nodes[ddlNum].name);

                jointsTemp.nodes[ddlNum].interpolation = drawManager.secondParameters.interpolationDefault;

                WriteToLogFile("jointsTemp.nodes[ddlNum].interpolation: " + jointsTemp.nodes[ddlNum].interpolation.type.ToString());

                int indexTQ = 2;

                WriteToLogFile("values.Length: " + values.Length.ToString());

                jointsTemp.nodes[ddlNum].T = ExtractDataTQ(values[indexTQ]);
                jointsTemp.nodes[ddlNum].Q = ExtractDataTQ(values[indexTQ + 1]);
                jointsTemp.nodes[ddlNum].ddlOppositeSide = -1;
                ddlNum++;
            }
        }

        drawManager.secondParameters.joints = jointsTemp;

        WriteToLogFile("Assigned MainParameters.Instance.joints");

        LagrangianModelSimple lagrangianModelSimple = new LagrangianModelSimple();
        drawManager.secondParameters.joints.lagrangianModel = lagrangianModelSimple.GetParameters;

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
        int n = (int)(MainParameters.Instance.joints.duration / MainParameters.Instance.joints.lagrangianModel.dt)+1;
        float[] t0 = new float[n];
        float[,] q0 = new float[MainParameters.Instance.joints.lagrangianModel.nDDL, n];

        GenerateQ0_s(MainParameters.Instance.joints.lagrangianModel, MainParameters.Instance.joints.duration, 0, out t0, out q0);

        MainParameters.Instance.joints.t0 = MathFunc.MatrixCopy(t0);
        MainParameters.Instance.joints.q0 = MathFunc.MatrixCopy(q0);
    }

    private void GenerateQ0_s(LagrangianModelManager.StrucLagrangianModel lagrangianModel, float tf, int qi, out float[] t0, out float[,] q0)
    {
        int[] ni;
        if (qi > 0)
            ni = new int[1] { qi };
        else
            ni = lagrangianModel.q2;

        float[] qd;
        int n = (int)(tf / lagrangianModel.dt)+1;
        t0 = new float[n];
        q0 = new float[lagrangianModel.nDDL, n];

        int i = 0;
        for (float interval = 0; interval < tf; interval += lagrangianModel.dt)
        {
            t0[i] = interval;
            Trajectory_ss(lagrangianModel, interval, ni, out qd);
            //            Trajectory trajectory = new Trajectory(lagrangianModel, interval, ni, out qd);
            //            trajectory.ToString();                  // Pour enlever un warning lors de la compilation
            for (int ddl = 0; ddl < qd.Length; ddl++)
                q0[ddl, i] = qd[ddl];
            i++;

            if (i >= n) break;
        }
    }

    private void Trajectory_ss(LagrangianModelManager.StrucLagrangianModel lagrangianModel, float t, int[] qi, out float[] qd)
    {
        float[] qdotd;
        float[] qddotd;
        drawManager.Trajectory_s(lagrangianModel, t, qi, out qd, out qdotd, out qddotd);
    }

    public void DisplayDDL(int ddl, bool axisRange)
    {
        if (ddl >= 0)
        {
            transform.parent.GetComponentInChildren<AniGraphManager>().DisplayCurveAndNodes(0, ddl, axisRange);
            if (MainParameters.Instance.joints.nodes[ddl].ddlOppositeSide >= 0)
            {
                transform.parent.GetComponentInChildren<AniGraphManager>().DisplayCurveAndNodes(1, MainParameters.Instance.joints.nodes[ddl].ddlOppositeSide, true);
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
