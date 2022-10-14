using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public struct InputData
{
    public float Velocity;
    public float Distance;
    public float Duration;
}

[System.Serializable]
public struct PlayerInfo
{
    public string Name;
    public string Id;
    public float Score;
    public InputData input;
}

[System.Serializable]
public struct PlayerReplayInfo
{
    public PlayerInfo player;
    public AnimationInfo replay;
}

public class StatManager : MonoBehaviour
{
    AvatarManager avatarManager;
    DrawManager drawManager;
    GameManager gameManager;
    LevelManager levelManager;
    UIManager uiManager;

    public PlayerInfo info;
    protected GameObject selectedJoint;
    protected ControlSegmentGeneric currentControlSegment;
    protected bool isRotating = false;
    protected Vector3 initPosition;
    public int currentJointSubIdx;
    RaycastHit hit;

    public string dofName;

    Camera _avatarCameraInternal;
    Ray mouseRay { get {
            if (_avatarCameraInternal == null)
            {
                var tp = GameObject.Find("AvatarCamera"); 
                if (tp == null) return new Ray();
                _avatarCameraInternal = tp.GetComponent<Camera>();
            }
            return _avatarCameraInternal.ScreenPointToRay(Input.mousePosition);
    }}
    Color colorBlue;
    Color colorWhite;

    GameObject arrow;

    GameObject error;
    BaseProfile _baseProfileInternal;
    BaseProfile baseProfile { get {
        if (error == null) error = GameObject.Find("Training");
        if (_baseProfileInternal == null) _baseProfileInternal = error.GetComponent<BaseProfile>();
        return _baseProfileInternal;}
    }


    void Start()
    {
        avatarManager = ToolBox.GetInstance().GetManager<AvatarManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        gameManager = transform.parent.GetComponentInChildren<GameManager>();
        levelManager = transform.parent.GetComponentInChildren<LevelManager>();
        uiManager = ToolBox.GetInstance().GetManager<UIManager>();

        error = GameObject.Find("Training");

        colorBlue = Color.blue;
        colorBlue.a = 0.5f;

        colorWhite = Color.white;
        colorWhite.a = 0.5f;

    }

    public void ProfileLoad(string fileName)
    {
        ReadDataFromJSON(fileName);
    }

    public void ProfileSave()
    {
        if (info.Name != null)
        {
            WriteDataToJSON(info.Name);
        }
    }


    public void ProfileReplaySave(string fileName)
    {
        PlayerReplayInfo replayInfo = new PlayerReplayInfo();

        replayInfo.player = info;

        replayInfo.replay.Objective = "default";
        replayInfo.replay.Duration = drawManager.Duration;
        replayInfo.replay.UseGravity = drawManager.UseGravity;
        replayInfo.replay.StopOnGround = drawManager.StopOnGround;

        replayInfo.replay.Somersault = drawManager.TakeOffParameters.Somersault;
        replayInfo.replay.Tilt = drawManager.TakeOffParameters.Tilt;
        replayInfo.replay.Twist = drawManager.TakeOffParameters.Twist;
        replayInfo.replay.HorizontalPosition = drawManager.TakeOffParameters.HorizontalPosition;
        replayInfo.replay.VerticalPosition = drawManager.TakeOffParameters.VerticalPosition;
        replayInfo.replay.SomersaultSpeed = drawManager.TakeOffParameters.SomersaultSpeed;
        replayInfo.replay.TiltSpeed = drawManager.TakeOffParameters.TiltSpeed;
        replayInfo.replay.TwistSpeed = drawManager.TakeOffParameters.TwistSpeed;
        replayInfo.replay.HorizontalSpeed = drawManager.TakeOffParameters.HorizontalSpeed;
        replayInfo.replay.VerticalSpeed = drawManager.TakeOffParameters.VerticalSpeed;

        for (int i = 0; i < avatarManager.LoadedModels[0].Joints.nodes.Length; i++)
        {
            Nodes n = new Nodes();
            n.Name = avatarManager.LoadedModels[0].Joints.nodes[i].name;
            n.T = avatarManager.LoadedModels[0].Joints.nodes[i].T;
            n.Q = avatarManager.LoadedModels[0].Joints.nodes[i].Q;

            replayInfo.replay.nodes.Add(n);
        }

        string jsonData = JsonUtility.ToJson(replayInfo, true);
        File.WriteAllText(fileName, jsonData);
    }

    private void WriteDataToJSON(string fileName)
    {
        string jsonData = JsonUtility.ToJson(info, true);
        File.WriteAllText(fileName, jsonData);
    }

    private void ReadDataFromJSON(string fileName)
    {
        string dataAsJson = File.ReadAllText(fileName);
        info = JsonUtility.FromJson<PlayerInfo>(dataAsJson);
    }

    void Update()
    {
        if (levelManager.currentState != SceneState.Training) return;

        bool hasHit = Physics.Raycast(mouseRay, out hit);

        if (Input.GetMouseButton(0))
        {
            if (isRotating)
            {
                Vector3 newPosition = Input.mousePosition;
                Vector3 mouseDistance = newPosition - initPosition;
                avatarManager.LoadedModels[0].gameObject.transform.Rotate(Vector3.up * -mouseDistance.x / 5f);
                initPosition = newPosition;
            }
            if (avatarManager.LoadedModels[0].IsLoaded && !drawManager.IsEditing && !isRotating && hasHit)
            {
                initPosition = Input.mousePosition;
                isRotating = true;
            }

        }
        else
        {
            isRotating = false;
        }

        if (Input.GetMouseButtonDown(1) && hasHit)
        {
            HandleJointClick();
        }
    }

    public void ResetTemporaries(){
        if (!drawManager.IsEditing) return;

        if (currentControlSegment)
            currentControlSegment.DestroyCircle();
        currentControlSegment = null;
        selectedJoint = null;
        currentJointSubIdx = -1;
    }

    void HandleJointClick() {
        if (!uiManager.IsInEditingTab){
            // Prevent from changing avantar position if not in modification tab
            return;
        }
        
        var _previousTp = selectedJoint;
        var _nextJointSubIdx = currentJointSubIdx + 1;  // Assume for now same joint
        ResetTemporaries();

        selectedJoint = hit.collider.gameObject;
        ControlSegmentGeneric[] _controlSegment = selectedJoint.GetComponents<ControlSegmentGeneric>();
        if (selectedJoint != _previousTp){
            // If not the same joint, reset to 0
            _nextJointSubIdx = 0;
        } else {
            // If we reached the end, joint is unselected
            if (_nextJointSubIdx >= _controlSegment.Length){
                drawManager.StopEditing();
                return;
            }
        }
        drawManager.StartEditing();

        currentJointSubIdx = _nextJointSubIdx;
        currentControlSegment = _controlSegment[currentJointSubIdx];
        currentControlSegment.Init(AddNode);

        baseProfile.InitDropdownDDLNames(currentControlSegment.avatarIndex);
        baseProfile.NodeName(currentControlSegment.dofName);
    }

    public int FindPreviousNode(int _dof)
    {
        int last = avatarManager.LoadedModels[0].Joints.nodes[_dof].T.Length - 1;
        if (drawManager.CurrentFrame == 0) 
            return 0;
        else if (drawManager.CurrentTime == avatarManager.LoadedModels[0].Joints.nodes[_dof].T[last]) 
            return last;

        int i = 0;
        while (
                i < avatarManager.LoadedModels[0].Joints.nodes[_dof].T.Length
                && drawManager.CurrentTime >= avatarManager.LoadedModels[0].Joints.nodes[_dof].T[i]
            )
        {
            i++;
        }
        return i - 1;
    }

    public int AddNode(int _dof)
    {
        gameManager.InterpolationDDL();
        gameManager.DisplayDDL(_dof, true);

        int node = FindPreviousNode(_dof);
        if (avatarManager.LoadedModels[0].Joints.nodes[_dof].T[node] == drawManager.CurrentTime)
            return node;


        float[] T = new float[avatarManager.LoadedModels[0].Joints.nodes[_dof].T.Length + 1];
        float[] Q = new float[avatarManager.LoadedModels[0].Joints.nodes[_dof].Q.Length + 1];

        for (int i = 0; i <= node; i++)
        {
            T[i] = avatarManager.LoadedModels[0].Joints.nodes[_dof].T[i];
            Q[i] = avatarManager.LoadedModels[0].Joints.nodes[_dof].Q[i];
        }

        T[node + 1] = drawManager.CurrentTime;
        Q[node + 1] = currentControlSegment.angle;

        for (int i = node + 1; i < avatarManager.LoadedModels[0].Joints.nodes[_dof].T.Length; i++)
        {
            T[i + 1] = avatarManager.LoadedModels[0].Joints.nodes[_dof].T[i];
            Q[i + 1] = avatarManager.LoadedModels[0].Joints.nodes[_dof].Q[i];
        }
        avatarManager.LoadedModels[0].Joints.nodes[_dof].T = MathFunc.MatrixCopy(T);
        avatarManager.LoadedModels[0].Joints.nodes[_dof].Q = MathFunc.MatrixCopy(Q);

        gameManager.InterpolationDDL();
        gameManager.DisplayDDL(_dof, true);

        return node + 1;
    }
}
