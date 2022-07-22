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
    public PlayerInfo info;
//    private GameObject circlePrefab;
//    private GameObject circlePrefab_shoulder;
    GameObject selectedJoint;
    string previousSelectedJointName;
    RaycastHit hit;

//    private List<GameObject> circles = new List<GameObject>();

    private int previousFrameN = 0;
    public int previousFrameN2 = 0;

    public string dofName;

    Camera _avatarCameraInternal;
    Ray mouseRay { get {
        if(_avatarCameraInternal == null)
            _avatarCameraInternal = GameObject.Find("AvatarCamera").GetComponent<Camera>();
        return _avatarCameraInternal.ScreenPointToRay(Input.mousePosition);
    }}
    Color colorBlue;
    Color colorWhite;

    GameObject arrow;

    //    GameObject errorPrefab;
    //    Text errorMessage;

    // Caching
    GameManager gameManager;
    DrawManager drawManager;
    LevelManager levelManager;
    GameObject error;
    BaseProfile _baseProfileInternal;
    BaseProfile baseProfile { get {
        if (error == null) error = GameObject.Find("Training");
        if (_baseProfileInternal == null) _baseProfileInternal = error.GetComponent<BaseProfile>();
        return _baseProfileInternal;}
    }


    void Start()
    {
        gameManager = transform.parent.GetComponentInChildren<GameManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        levelManager = transform.parent.GetComponentInChildren<LevelManager>();
        error = GameObject.Find("Training");

        //        circlePrefab = (GameObject)Resources.Load("HandleCircle", typeof(GameObject));
//        circlePrefab_shoulder = (GameObject)Resources.Load("HandleCircle_shoulder", typeof(GameObject));

        colorBlue = Color.blue;
        colorBlue.a = 0.5f;

        colorWhite = Color.white;
        colorWhite.a = 0.5f;

/*        errorPrefab = (GameObject)Resources.Load("ErrorMessage", typeof(GameObject));
        errorPrefab = Instantiate(errorPrefab);
        errorMessage = errorPrefab.GetComponentInChildren<Text>();
        errorMessage.text = "";
        errorPrefab.SetActive(false);*/
    }

/*    public void SetErrorMessage(string _msg)
    {
        errorPrefab.SetActive(true);
        errorMessage.text = _msg;
        Invoke("ResetError", 0.5f);
    }*/

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

    private void ProfileReplayLoad(string fileName)
    {
        string dataAsJson = File.ReadAllText(fileName);
        PlayerReplayInfo replayInfo = JsonUtility.FromJson<PlayerReplayInfo>(dataAsJson);

        info = replayInfo.player;

        MainParameters.StrucJoints jointsTemp = new MainParameters.StrucJoints();
        jointsTemp.fileName = fileName;
        jointsTemp.nodes = null;
        jointsTemp.duration = replayInfo.replay.Duration;
//        jointsTemp.condition = replayInfo.replay.Condition;
        jointsTemp.takeOffParam.verticalSpeed = replayInfo.replay.VerticalSpeed;
        jointsTemp.takeOffParam.anteroposteriorSpeed = replayInfo.replay.AnteroposteriorSpeed;
        jointsTemp.takeOffParam.somersaultSpeed = replayInfo.replay.SomersaultSpeed;
        jointsTemp.takeOffParam.twistSpeed = replayInfo.replay.TwistSpeed;
        jointsTemp.takeOffParam.tilt = replayInfo.replay.Tilt;
        jointsTemp.takeOffParam.rotation = replayInfo.replay.Rotation;

        jointsTemp.nodes = new MainParameters.StrucNodes[replayInfo.replay.nodes.Count];

        for (int i = 0; i < replayInfo.replay.nodes.Count; i++)
        {
            jointsTemp.nodes[i].ddl = i + 1;
            jointsTemp.nodes[i].name = replayInfo.replay.nodes[i].Name;
            jointsTemp.nodes[i].interpolation = MainParameters.Instance.interpolationDefault;
            jointsTemp.nodes[i].T = replayInfo.replay.nodes[i].T;
            jointsTemp.nodes[i].Q = replayInfo.replay.nodes[i].Q;
            jointsTemp.nodes[i].ddlOppositeSide = -1;
        }

        MainParameters.Instance.joints = jointsTemp;

        LagrangianModelSimple lagrangianModelSimple = new LagrangianModelSimple();
        MainParameters.Instance.joints.lagrangianModel = lagrangianModelSimple.GetParameters;
    }

    public void ProfileReplaySave(string fileName)
    {
        PlayerReplayInfo replayInfo = new PlayerReplayInfo();

        replayInfo.player = info;

        replayInfo.replay.Objective = "defalut";
        replayInfo.replay.Duration = MainParameters.Instance.joints.duration;
//        replayInfo.replay.Condition = MainParameters.Instance.joints.condition;
        replayInfo.replay.VerticalSpeed = MainParameters.Instance.joints.takeOffParam.verticalSpeed;
        replayInfo.replay.AnteroposteriorSpeed = MainParameters.Instance.joints.takeOffParam.anteroposteriorSpeed;
        replayInfo.replay.SomersaultSpeed = MainParameters.Instance.joints.takeOffParam.somersaultSpeed;
        replayInfo.replay.TwistSpeed = MainParameters.Instance.joints.takeOffParam.twistSpeed;
        replayInfo.replay.Tilt = MainParameters.Instance.joints.takeOffParam.tilt;
        replayInfo.replay.Rotation = MainParameters.Instance.joints.takeOffParam.rotation;

        for (int i = 0; i < MainParameters.Instance.joints.nodes.Length; i++)
        {
            Nodes n = new Nodes();
            n.Name = MainParameters.Instance.joints.nodes[i].name;
            n.T = MainParameters.Instance.joints.nodes[i].T;
            n.Q = MainParameters.Instance.joints.nodes[i].Q;

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

        if (Input.GetMouseButtonDown(1) && Physics.Raycast(mouseRay, out hit))
        {
            HandleJointClick();
        }

        if (Input.GetMouseButton(0) && !drawManager.isEditing)
        {
            if (drawManager.girl1 != null)
            {
                drawManager.girl1.transform.Rotate(Vector3.up * 100f * Time.deltaTime);
                if (selectedJoint)
                    selectedJoint.transform.position = hit.collider.gameObject.transform.position;
            }
        }
    }

    void ResetTemporaries(){
        if (!drawManager.isEditing) return;

        drawManager.isEditing = false;

        selectedJoint.GetComponent<ControlSegmentGeneric>().DestroyCircle();
        selectedJoint = null;
        previousSelectedJointName = null;

        previousFrameN = 0;
    }

    void HandleJointClick() {
        // For some reason it is impossible to modify the first node
        // TODO: Fix that
        if (drawManager.frameN == 0) return;

        var previousNameTp = previousSelectedJointName;
        ResetTemporaries();

        selectedJoint = hit.collider.gameObject;
        if (selectedJoint.name == previousNameTp) return;
        
        drawManager.isEditing = true;

        previousSelectedJointName = selectedJoint.name;
        ControlSegmentGeneric controlSegment = selectedJoint.GetComponent<ControlSegmentGeneric>();
        controlSegment.Init(AddNode);

        baseProfile.InitDropdownDDLNames(controlSegment.avatarIndex);
        baseProfile.NodeName(controlSegment.dofName);
    }

    IEnumerator RotateLerp(float _goal, float _speed)
    {
        float curr = 0;
        while (_goal > curr)
        {
            drawManager.girl1.transform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(Vector3.up * _goal), _speed * Time.time);
            curr = drawManager.girl1.transform.rotation.eulerAngles.y;
            yield return new WaitForEndOfFrame();
        }
    }

    void CameraRotate(string _n)
    {
        if (_n == "shin.L" || _n == "thigh.L")
        {
 //            StartCoroutine(RotateLerp(90, 0.2f));
            //            selectedJoint.transform.position = drawManager.girl1.transform.Find("Petra.002/hips/thigh.L/shin.L").gameObject.transform.position;
            //            drawManager.girl1.transform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(Vector3.up * 90), 0.2f * Time.time);                                                                                                                                                                                                              //            drawManager.girl1.transform.rotation = Quaternion.Euler(Vector3.up * 90);
//            drawManager.girl1.transform.rotation = Quaternion.Euler(Vector3.up * 90);
//            selectedJoint.transform.position = drawManager.girl1.transform.Find("Petra.002/hips/thigh.L/shin.L").gameObject.transform.position;
/*            for (int i = 0; i < 16; i++)
            {
                float angle = i * Mathf.PI * 2f / 16;
                Vector3 newPos = new Vector3(Mathf.Cos(angle) * 0.2f, 0, Mathf.Sin(angle) * 0.2f);
                GameObject go = Instantiate(circlePrefab_shoulder, hit.collider.transform.position + newPos, Quaternion.identity);
                go.transform.parent = selectedJoint.transform;
            }*/
        }
        else if (_n == "shin.R" || _n == "thigh.R")
        {
            //            drawManager.girl1.transform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(Vector3.up * -90), 0.2f * Time.time);
//            drawManager.girl1.transform.rotation = Quaternion.Euler(Vector3.up * -90);
//            selectedJoint.transform.position = drawManager.girl1.transform.Find("Petra.002/hips/thigh.R/shin.R").gameObject.transform.position;
/*            for (int i = 0; i < 16; i++)
            {
                float angle = i * Mathf.PI * 2f / 16;
                Vector3 newPos = new Vector3(Mathf.Cos(angle) * 0.2f, 0, Mathf.Sin(angle) * 0.2f);
                GameObject go = Instantiate(circlePrefab_shoulder, hit.collider.transform.position + newPos, Quaternion.identity);
                go.transform.parent = selectedJoint.transform;
            }*/
        }
        else if (_n == "upper_arm.L")
        {
/*            for (int i = 0; i < 16; i++)
            {
                float angle = i * Mathf.PI * 2f / 16;
                Vector3 newPos = new Vector3(Mathf.Cos(angle) * 0.2f, 0, Mathf.Sin(angle) * 0.2f);
                GameObject go = Instantiate(circlePrefab_shoulder, hit.collider.transform.position + newPos, Quaternion.identity);
                go.transform.parent = selectedJoint.transform;
            }
            for (int i = 0; i < 16; i++)
            {
                float angle = i * Mathf.PI * 2f / 16;
                Vector3 newPos = new Vector3(0, Mathf.Cos(angle) * 0.2f, Mathf.Sin(angle) * 0.2f);
                GameObject go = Instantiate(circlePrefab_shoulder, hit.collider.transform.position + newPos, Quaternion.identity);
                go.transform.parent = selectedJoint.transform;
            }*/

            //            drawManager.girl1.transform.rotation = Quaternion.Euler(Vector3.up * 90);
        }
        else if (_n == "upper_arm.R")
        {
/*            for (int i = 0; i < 16; i++)
            {
                float angle = i * Mathf.PI * 2f / 16;
                Vector3 newPos = new Vector3(Mathf.Cos(angle) * 0.2f, 0, Mathf.Sin(angle) * 0.2f);
                GameObject go = Instantiate(circlePrefab_shoulder, hit.collider.transform.position + newPos, Quaternion.identity);
                go.transform.parent = selectedJoint.transform;
            }
            for (int i = 0; i < 16; i++)
            {
                float angle = i * Mathf.PI * 2f / 16;
                Vector3 newPos = new Vector3(0, Mathf.Cos(angle) * 0.2f, Mathf.Sin(angle) * 0.2f);
                GameObject go = Instantiate(circlePrefab_shoulder, hit.collider.transform.position + newPos, Quaternion.identity);
                go.transform.parent = selectedJoint.transform;
            }*/

            //            drawManager.girl1.transform.rotation = Quaternion.Euler(Vector3.up * -90);
        }
    }

    public int FindPreviousNode(int _dof)
    {
        int i = 0;
        while (
                i < MainParameters.Instance.joints.nodes[_dof].T.Length
                && drawManager.frameN * 0.02 > MainParameters.Instance.joints.nodes[_dof].T[i]
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

        if (previousFrameN >= drawManager.frameN - 1 && previousFrameN <= drawManager.frameN + 1)
        {
            return node;
        }

        float marginT = drawManager.frameN * 0.02f;

        float[] T = new float[MainParameters.Instance.joints.nodes[_dof].T.Length + 1];
        float[] Q = new float[MainParameters.Instance.joints.nodes[_dof].Q.Length + 1];

        for (int i = 0; i <= node; i++)
        {
            T[i] = MainParameters.Instance.joints.nodes[_dof].T[i];
            Q[i] = MainParameters.Instance.joints.nodes[_dof].Q[i];
        }

        if (T[node] >= marginT - 0.02f && T[node] <= marginT + 0.02f)
        {
            return node;
        }

        previousFrameN = drawManager.frameN;

        T[node + 1] = drawManager.frameN * 0.02f;

        switch(_dof)
        {
            case 0:
                Q[node + 1] = (float)selectedJoint.GetComponent<ControlThigh>().angle;
                break;
            case 1:
                Q[node + 1] = (float)selectedJoint.GetComponent<ControlShin>().angle;
                break;
            case 2:
                //                Q[node + 1] = (float)selectedJoint.GetComponent<ControlLeftArmFlexion>().angle;
                Q[node + 1] = (float)selectedJoint.GetComponent<ControlRightArmFlexion>().dof;
                break;
            case 3:
                Q[node + 1] = (float)selectedJoint.GetComponent<ControlRightArmAbduction>().angle;
                break;
            case 4:
                Q[node + 1] = (float)selectedJoint.GetComponent<ControlLeftArmFlexion>().angle;
                break;
            case 5:
                Q[node + 1] = (float)selectedJoint.GetComponent<ControlLeftArmAbduction>().angle;
                break;
        }

        for (int i = node + 1; i < MainParameters.Instance.joints.nodes[_dof].T.Length; i++)
        {
            T[i + 1] = MainParameters.Instance.joints.nodes[_dof].T[i];
            Q[i + 1] = MainParameters.Instance.joints.nodes[_dof].Q[i];
        }
        MainParameters.Instance.joints.nodes[_dof].T = MathFunc.MatrixCopy(T);
        MainParameters.Instance.joints.nodes[_dof].Q = MathFunc.MatrixCopy(Q);

        gameManager.InterpolationDDL();
        gameManager.DisplayDDL(_dof, true);

        return node + 1;
    }
}
