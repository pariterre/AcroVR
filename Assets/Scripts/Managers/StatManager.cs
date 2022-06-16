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
    private GameObject circle;
    RaycastHit hit;

//    private List<GameObject> circles = new List<GameObject>();

    private int previousFrameN = 0;
    public int previousFrameN2 = 0;

    public string dofName;

    Camera avatarCamera;
    Color colorBlue;
    Color colorWhite;

    GameObject error;
    GameObject arrow;

    //    GameObject errorPrefab;
    //    Text errorMessage;

    void Start()
    {
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

    public void SetAvatarCamera()
    {
        if(avatarCamera == null)
            avatarCamera = GameObject.Find("AvatarCamera").GetComponent<Camera>();
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

    /*    private void ResetError()
        {
            errorPrefab.SetActive(false);
            errorMessage.text = "";
        }*/

    void Update()
    {
        if (transform.parent.GetComponentInChildren<LevelManager>().currentState != SceneState.Training) return;

        if (Input.GetMouseButtonDown(1) && ToolBox.GetInstance().GetManager<DrawManager>().frameN == 0)
        {
            SetAvatarCamera();
            Ray ray = avatarCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (error == null) error = GameObject.Find("Training");

                if (MainParameters.Instance.languages.Used.toolTipButtonQuit == "Quit")
                {
                    error.GetComponent<BaseProfile>().ErrorMessage("Please pause first while animating 3D avatar");
                }
                else
                {
                    error.GetComponent<BaseProfile>().ErrorMessage("Veuillez d'abord faire une pause pendant l'animation de l'avatar 3D");
                }
            }
            return;
        }

        if (Input.GetMouseButtonDown(1) && ToolBox.GetInstance().GetManager<DrawManager>().frameN != 0
            && ToolBox.GetInstance().GetManager<DrawManager>().frameN < ToolBox.GetInstance().GetManager<DrawManager>().numberFrames
            )
        {
            //            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray ray = avatarCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (error == null) error = GameObject.Find("Training");
//                if (arrow == null) arrow = GameObject.Find("Arrow");

                if (!circle)
                {
                    transform.parent.GetComponentInChildren<DrawManager>().isEditing = true;
                    circle = hit.collider.gameObject;
                    //                    circle.GetComponent<MeshRenderer>().enabled = true;
                    //                    circle.GetComponent<MeshRenderer>().material.SetColor("_Color", colorWhite);

                    //                    arrow.SetActive(true);
                    //                    arrow.transform.position = circle.transform.position;

                    switch (circle.name)
                    {
                        case "ControlShin":
                            circle.GetComponent<ControlShin>().Init(transform.parent.GetComponentInChildren<DrawManager>().girl1, transform.parent.GetComponentInChildren<DrawManager>().qf[1]);
                            circle.GetComponent<ControlShin>().node = AddNode(1);
                            error.GetComponent<BaseProfile>().InitDropdownDDLNames(1);
                            error.GetComponent<BaseProfile>().NodeName("KneeFlexion");
                            break;
                        case "ControlThigh":
                            circle.GetComponent<ControlThigh>().Init(transform.parent.GetComponentInChildren<DrawManager>().girl1, transform.parent.GetComponentInChildren<DrawManager>().qf[0]);
                            circle.GetComponent<ControlThigh>().node = AddNode(0);
                            error.GetComponent<BaseProfile>().InitDropdownDDLNames(0);
                            error.GetComponent<BaseProfile>().NodeName("HipFlexion");
                            break;
                        case "ControlLeftArm":
                            circle.GetComponent<ControlLeftArmAbduction>().bActive = false;
                            circle.GetComponent<ControlLeftArmFlexion>().bActive = true;
                            circle.GetComponent<ControlLeftArmFlexion>().Init(transform.parent.GetComponentInChildren<DrawManager>().girl1, transform.parent.GetComponentInChildren<DrawManager>().qf[2]);
                            circle.GetComponent<ControlLeftArmFlexion>().node = AddNode(4);
                            error.GetComponent<BaseProfile>().InitDropdownDDLNames(4);
                            error.GetComponent<BaseProfile>().NodeName("LeftArmFlexion");
                            break;
                        case "ControlRightArm":
                            circle.GetComponent<ControlRightArmAbduction>().bActive = false;
                            circle.GetComponent<ControlRightArmFlexion>().bActive = true;
                            circle.GetComponent<ControlRightArmFlexion>().Init(transform.parent.GetComponentInChildren<DrawManager>().girl1, transform.parent.GetComponentInChildren<DrawManager>().qf[4]);
                            circle.GetComponent<ControlRightArmFlexion>().node = AddNode(2);
                            error.GetComponent<BaseProfile>().InitDropdownDDLNames(2);
                            error.GetComponent<BaseProfile>().NodeName("RightArmFlexion");
                            break;
                    }

                    //                    circle = Instantiate(circlePrefab, hit.collider.transform.position, Quaternion.identity);
                    //                    circle.GetComponent<HandleCircle>().Init(transform.parent.GetComponentInChildren<DrawManager>().girl1, hit.collider.gameObject, transform.parent.GetComponentInChildren<DrawManager>().qf);
//                    CameraRotate(hit.collider.gameObject.name);
                    //                    AddNodeInDof();
                    ToolBox.GetInstance().GetManager<DrawManager>().isEditing = true;
                }
                else
                {
                    if (circle.name == "ControlLeftArm")
                    {
                        if (!circle.GetComponent<ControlLeftArmAbduction>().bActive)
                        {
                            circle.GetComponent<ControlLeftArmFlexion>().DestroyCircle();

                            previousFrameN = 0;
//                            circle.GetComponent<MeshRenderer>().material.SetColor("_Color", colorBlue);
                            circle.GetComponent<ControlLeftArmFlexion>().bActive = false;
                            circle.GetComponent<ControlLeftArmAbduction>().bActive = true;
                            circle.GetComponent<ControlLeftArmAbduction>().Init(transform.parent.GetComponentInChildren<DrawManager>().girl1, transform.parent.GetComponentInChildren<DrawManager>().qf[3]);
                            circle.GetComponent<ControlLeftArmAbduction>().node = AddNode(5);
                            error.GetComponent<BaseProfile>().InitDropdownDDLNames(5);
                            error.GetComponent<BaseProfile>().NodeName("LeftArmAbduction");
                        }
                        else
                        {
                            circle.GetComponent<ControlLeftArmAbduction>().DestroyCircle();

                            circle.GetComponent<ControlLeftArmFlexion>().bActive = false;
                            circle.GetComponent<ControlLeftArmAbduction>().bActive = false;
//                            circle.GetComponent<MeshRenderer>().enabled = false;
                            ToolBox.GetInstance().GetManager<DrawManager>().isEditing = false;
                            circle = null;
                            // ToolBox.GetInstance().GetManager<DrawManager>().MakeSimulationFrame();
                        }
                    }
                    else if (circle.name == "ControlRightArm")
                    {
                        if (!circle.GetComponent<ControlRightArmAbduction>().bActive)
                        {
                            circle.GetComponent<ControlRightArmFlexion>().DestroyCircle();

                            previousFrameN = 0;
//                            circle.GetComponent<MeshRenderer>().material.SetColor("_Color", colorBlue);
                            circle.GetComponent<ControlRightArmFlexion>().bActive = false;
                            circle.GetComponent<ControlRightArmAbduction>().bActive = true;
                            circle.GetComponent<ControlRightArmAbduction>().Init(transform.parent.GetComponentInChildren<DrawManager>().girl1, transform.parent.GetComponentInChildren<DrawManager>().qf[5]);
                            circle.GetComponent<ControlRightArmAbduction>().node = AddNode(3);
                            error.GetComponent<BaseProfile>().InitDropdownDDLNames(3);
                            error.GetComponent<BaseProfile>().NodeName("RightArmAbduction");
                        }
                        else
                        {
                            circle.GetComponent<ControlRightArmAbduction>().DestroyCircle();

                            circle.GetComponent<ControlRightArmFlexion>().bActive = false;
                            circle.GetComponent<ControlRightArmAbduction>().bActive = false;
//                            circle.GetComponent<MeshRenderer>().enabled = false;
                            ToolBox.GetInstance().GetManager<DrawManager>().isEditing = false;
                            circle = null;
                            //ToolBox.GetInstance().GetManager<DrawManager>().MakeSimulationFrame();
                        }
                    }
                    else
                    {
                        if(circle.name == "ControlThigh") circle.GetComponent<ControlThigh>().DestroyCircle();
                        else if (circle.name == "ControlShin") circle.GetComponent<ControlShin>().DestroyCircle();

//                        circle.GetComponent<MeshRenderer>().enabled = false;
                        ToolBox.GetInstance().GetManager<DrawManager>().isEditing = false;
                        circle = null;
                        //ToolBox.GetInstance().GetManager<DrawManager>().MakeSimulationFrame();
                        //                        ToolBox.GetInstance().GetManager<DrawManager>().ShowAvatar();
                    }
                    /*                    if (circle.transform.position == hit.collider.transform.position)
                                        {
                                            if (circle.GetComponent<HandleCircle>().target.name == "upper_arm.L")
                                            {
                                                if (transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation.eulerAngles.y != 90)
                                                {
                                                    transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation = Quaternion.Euler(Vector3.up * 90);
                                                    circle.transform.position = transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.Find("Petra.002/hips/spine/chest/chest1/shoulder.L/upper_arm.L").gameObject.transform.position;
                    //                                circle.GetComponent<HandleCircle>().rotateTarget = true;
                                                }
                                                else
                                                {
                                                    DestroyHandleCircle();
                                                    transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation = Quaternion.identity;
                                                }
                                            }
                                            else if (circle.GetComponent<HandleCircle>().target.name == "upper_arm.R")
                                            {
                                                if (transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation.eulerAngles.y != 270)
                                                {
                                                    transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation = Quaternion.Euler(Vector3.up * -90);
                                                    circle.transform.position = transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.Find("Petra.002/hips/spine/chest/chest1/shoulder.R/upper_arm.R").gameObject.transform.position;
                    //                                circle.GetComponent<HandleCircle>().rotateTarget = true;
                                                }
                                                else
                                                {
                                                    DestroyHandleCircle();
                                                    transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation = Quaternion.identity;
                                                }
                                            }
                                            else
                                            {
                                                DestroyHandleCircle();
                                                transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation = Quaternion.identity;
                                            }
                                        }
                                        else
                                        {
                                            DestroyHandleCircle();
                                            transform.parent.GetComponentInChildren<DrawManager>().isEditing = true;

                                            circle = Instantiate(circlePrefab, hit.collider.transform.position, Quaternion.identity);
                                            circle.GetComponent<HandleCircle>().Init(transform.parent.GetComponentInChildren<DrawManager>().girl1, hit.collider.gameObject, transform.parent.GetComponentInChildren<DrawManager>().qf);
                                            AddNodeInDof();
                                        }*/
                }
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0))
        {
            if (transform.parent.GetComponentInChildren<DrawManager>().girl1 != null)
            {
                transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.Rotate(Vector3.up * 100f * Time.deltaTime);
                if (circle)
                    circle.transform.position = hit.collider.gameObject.transform.position;
            }
        }
    }

    private IEnumerator RotateLerp(float _goal, float _speed)
    {
        float curr = 0;
        while (_goal > curr)
        {
            transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(Vector3.up * _goal), _speed * Time.time);
            curr = transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation.eulerAngles.y;
            yield return new WaitForEndOfFrame();
        }
    }

    void CameraRotate(string _n)
    {
        if (_n == "shin.L" || _n == "thigh.L")
        {
 //            StartCoroutine(RotateLerp(90, 0.2f));
            //            circle.transform.position = transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.Find("Petra.002/hips/thigh.L/shin.L").gameObject.transform.position;
            //            transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(Vector3.up * 90), 0.2f * Time.time);                                                                                                                                                                                                              //            transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation = Quaternion.Euler(Vector3.up * 90);
//            transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation = Quaternion.Euler(Vector3.up * 90);
//            circle.transform.position = transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.Find("Petra.002/hips/thigh.L/shin.L").gameObject.transform.position;
/*            for (int i = 0; i < 16; i++)
            {
                float angle = i * Mathf.PI * 2f / 16;
                Vector3 newPos = new Vector3(Mathf.Cos(angle) * 0.2f, 0, Mathf.Sin(angle) * 0.2f);
                GameObject go = Instantiate(circlePrefab_shoulder, hit.collider.transform.position + newPos, Quaternion.identity);
                go.transform.parent = circle.transform;
            }*/
        }
        else if (_n == "shin.R" || _n == "thigh.R")
        {
            //            transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(Vector3.up * -90), 0.2f * Time.time);
//            transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation = Quaternion.Euler(Vector3.up * -90);
//            circle.transform.position = transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.Find("Petra.002/hips/thigh.R/shin.R").gameObject.transform.position;
/*            for (int i = 0; i < 16; i++)
            {
                float angle = i * Mathf.PI * 2f / 16;
                Vector3 newPos = new Vector3(Mathf.Cos(angle) * 0.2f, 0, Mathf.Sin(angle) * 0.2f);
                GameObject go = Instantiate(circlePrefab_shoulder, hit.collider.transform.position + newPos, Quaternion.identity);
                go.transform.parent = circle.transform;
            }*/
        }
        else if (_n == "upper_arm.L")
        {
/*            for (int i = 0; i < 16; i++)
            {
                float angle = i * Mathf.PI * 2f / 16;
                Vector3 newPos = new Vector3(Mathf.Cos(angle) * 0.2f, 0, Mathf.Sin(angle) * 0.2f);
                GameObject go = Instantiate(circlePrefab_shoulder, hit.collider.transform.position + newPos, Quaternion.identity);
                go.transform.parent = circle.transform;
            }
            for (int i = 0; i < 16; i++)
            {
                float angle = i * Mathf.PI * 2f / 16;
                Vector3 newPos = new Vector3(0, Mathf.Cos(angle) * 0.2f, Mathf.Sin(angle) * 0.2f);
                GameObject go = Instantiate(circlePrefab_shoulder, hit.collider.transform.position + newPos, Quaternion.identity);
                go.transform.parent = circle.transform;
            }*/

            //            transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation = Quaternion.Euler(Vector3.up * 90);
        }
        else if (_n == "upper_arm.R")
        {
/*            for (int i = 0; i < 16; i++)
            {
                float angle = i * Mathf.PI * 2f / 16;
                Vector3 newPos = new Vector3(Mathf.Cos(angle) * 0.2f, 0, Mathf.Sin(angle) * 0.2f);
                GameObject go = Instantiate(circlePrefab_shoulder, hit.collider.transform.position + newPos, Quaternion.identity);
                go.transform.parent = circle.transform;
            }
            for (int i = 0; i < 16; i++)
            {
                float angle = i * Mathf.PI * 2f / 16;
                Vector3 newPos = new Vector3(0, Mathf.Cos(angle) * 0.2f, Mathf.Sin(angle) * 0.2f);
                GameObject go = Instantiate(circlePrefab_shoulder, hit.collider.transform.position + newPos, Quaternion.identity);
                go.transform.parent = circle.transform;
            }*/

            //            transform.parent.GetComponentInChildren<DrawManager>().girl1.transform.rotation = Quaternion.Euler(Vector3.up * -90);
        }
    }

    public void DestroyHandleCircle()
    {
        transform.parent.GetComponentInChildren<DrawManager>().isEditing = false;

        if (circle != null)
        {
//            Destroy(circle.gameObject);

            if (circle.name == "ControlThigh") circle.GetComponent<ControlThigh>().DestroyCircle();
            else if (circle.name == "ControlShin") circle.GetComponent<ControlShin>().DestroyCircle();
            else if (circle.name == "ControlLeftArm")
            {
                if (!circle.GetComponent<ControlLeftArmAbduction>().bActive)
                    circle.GetComponent<ControlLeftArmFlexion>().DestroyCircle();
                else
                    circle.GetComponent<ControlLeftArmAbduction>().DestroyCircle();
            }
            else if (circle.name == "ControlRightArm")
            {
                if (!circle.GetComponent<ControlRightArmAbduction>().bActive)
                    circle.GetComponent<ControlRightArmFlexion>().DestroyCircle();
                else
                    circle.GetComponent<ControlRightArmAbduction>().DestroyCircle();
            }

            circle = null;
        }
    }

    public int FindPreviousNode(int _dof)
    {
        int i = 0;
        while (i < MainParameters.Instance.joints.nodes[_dof].T.Length && transform.parent.GetComponentInChildren<DrawManager>().frameN*0.02 > MainParameters.Instance.joints.nodes[_dof].T[i])
            i++;
        return i - 1;
    }

    private void ModifyNode(int _dof)
    {
        int node = FindPreviousNode(_dof);
        MainParameters.Instance.joints.nodes[_dof].Q[node] = (float)circle.GetComponent<HandleCircle>().dof[_dof];

        transform.parent.GetComponentInChildren<DrawManager>().isEditing = false;
        transform.parent.GetComponentInChildren<DrawManager>().frameN = (int)(MainParameters.Instance.joints.nodes[_dof].T[node] / 0.02f);
        transform.parent.GetComponentInChildren<DrawManager>().PlayOneFrame();

        transform.parent.GetComponentInChildren<GameManager>().InterpolationDDL();
        transform.parent.GetComponentInChildren<GameManager>().DisplayDDL(_dof, true);
        transform.parent.GetComponentInChildren<DrawManager>().isEditing = true;
    }

    private void AddNodeInDof()
    {
        if (circle.GetComponent<HandleCircle>().target.name == "shin.L" || circle.GetComponent<HandleCircle>().target.name == "shin.R")
        {
            int node = AddNode(1);
            circle.GetComponent<HandleCircle>().node = node;
//                           ModifyNode(1);
            //                MainParameters.Instance.joints.nodes[1].Q[AddNode(1)] = (float)circle.GetComponent<HandleCircle>().dof[1];
            //                print(MainParameters.Instance.joints.nodes[1].Q[AddNode(1)]);
            //                print((-(float)circle.GetComponent<HandleCircle>().dof[1]* Mathf.Rad2Deg + 180)* Mathf.PI / 180);
            //                transform.parent.GetComponentInChildren<GameManager>().InterpolationDDL();
            //                transform.parent.GetComponentInChildren<GameManager>().DisplayDDL(1, true);
        }
        else if (circle.GetComponent<HandleCircle>().target.name == "thigh.L" || circle.GetComponent<HandleCircle>().target.name == "thigh.R")
        {
            int node = AddNode(0);
            circle.GetComponent<HandleCircle>().node = node;
//            ModifyNode(0);
            //                MainParameters.Instance.joints.nodes[0].Q[AddNode(0)] = (float)circle.GetComponent<HandleCircle>().dof[0];
            //                transform.parent.GetComponentInChildren<GameManager>().InterpolationDDL();
            //                transform.parent.GetComponentInChildren<GameManager>().DisplayDDL(0, true);
        }
        else if (circle.GetComponent<HandleCircle>().target.name == "upper_arm.L")
        {
//            circle.GetComponent<HandleCircle>().node = AddNode(2);
            //                mouseDistance.x = (float)dof[3] * 30f;
            //                mouseDistance.y = (float)dof[2] * 30f;
            //            transform.parent.GetComponentInChildren<GameManager>().InterpolationDDL();
            //            transform.parent.GetComponentInChildren<GameManager>().DisplayDDL(2, true);
        }
        else if (circle.GetComponent<HandleCircle>().target.name == "upper_arm.R")
        {
//            circle.GetComponent<HandleCircle>().node = AddNode(4);
            //                mouseDistance.x = (float)dof[5] * 30f;
            //                mouseDistance.y = (float)dof[4] * 30f;
            //            transform.parent.GetComponentInChildren<GameManager>().InterpolationDDL();
            //            transform.parent.GetComponentInChildren<GameManager>().DisplayDDL(4, true);
        }
    }

    public int AddNode(int _dof)
    {
        transform.parent.GetComponentInChildren<GameManager>().InterpolationDDL();
        transform.parent.GetComponentInChildren<GameManager>().DisplayDDL(_dof, true);

        int node = FindPreviousNode(_dof);

        if (previousFrameN <= transform.parent.GetComponentInChildren<DrawManager>().frameN + 1 &&
            previousFrameN >= transform.parent.GetComponentInChildren<DrawManager>().frameN - 1
            )
        {
            return node;
        }

        float marginT = transform.parent.GetComponentInChildren<DrawManager>().frameN * 0.02f;

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

        previousFrameN = transform.parent.GetComponentInChildren<DrawManager>().frameN;

        T[node + 1] = transform.parent.GetComponentInChildren<DrawManager>().frameN * 0.02f;

        switch(_dof)
        {
            case 0:
                Q[node + 1] = (float)circle.GetComponent<ControlThigh>().dof;
                break;
            case 1:
                Q[node + 1] = (float)circle.GetComponent<ControlShin>().dof;
                break;
            case 2:
                //                Q[node + 1] = (float)circle.GetComponent<ControlLeftArmFlexion>().dof;
                Q[node + 1] = (float)circle.GetComponent<ControlRightArmFlexion>().dof;
                break;
            case 3:
                //                Q[node + 1] = (float)circle.GetComponent<ControlLeftArmAbduction>().dof;
                Q[node + 1] = (float)circle.GetComponent<ControlRightArmAbduction>().dof;
                break;
            case 4:
                Q[node + 1] = (float)circle.GetComponent<ControlLeftArmFlexion>().dof;
                //                Q[node + 1] = (float)circle.GetComponent<ControlRightArmFlexion>().dof;
                break;
            case 5:
                Q[node + 1] = (float)circle.GetComponent<ControlLeftArmAbduction>().dof;
                //                Q[node + 1] = (float)circle.GetComponent<ControlRightArmAbduction>().dof;
                break;
        }

        for (int i = node + 1; i < MainParameters.Instance.joints.nodes[_dof].T.Length; i++)
        {
            T[i + 1] = MainParameters.Instance.joints.nodes[_dof].T[i];
            Q[i + 1] = MainParameters.Instance.joints.nodes[_dof].Q[i];
        }
        MainParameters.Instance.joints.nodes[_dof].T = MathFunc.MatrixCopy(T);
        MainParameters.Instance.joints.nodes[_dof].Q = MathFunc.MatrixCopy(Q);

        transform.parent.GetComponentInChildren<GameManager>().InterpolationDDL();
        transform.parent.GetComponentInChildren<GameManager>().DisplayDDL(_dof, true);

        return node + 1;
    }

    public int AddNode2(int _dof)
    {
        transform.parent.GetComponentInChildren<GameManager>().InterpolationDDL();
        transform.parent.GetComponentInChildren<GameManager>().DisplayDDL(_dof, true);

        int node = FindPreviousNode(_dof);

        if (previousFrameN2 == transform.parent.GetComponentInChildren<DrawManager>().frameN)
        {
            return node;
        }
        previousFrameN2 = transform.parent.GetComponentInChildren<DrawManager>().frameN;

        float[] T = new float[MainParameters.Instance.joints.nodes[_dof].T.Length + 1];
        float[] Q = new float[MainParameters.Instance.joints.nodes[_dof].Q.Length + 1];
        for (int i = 0; i <= node; i++)
        {
            T[i] = MainParameters.Instance.joints.nodes[_dof].T[i];
            Q[i] = MainParameters.Instance.joints.nodes[_dof].Q[i];
        }

        T[node + 1] = transform.parent.GetComponentInChildren<DrawManager>().frameN * 0.02f;
        Q[node + 1] = (float)circle.GetComponent<HandleCircle>().dof[_dof];
        for (int i = node + 1; i < MainParameters.Instance.joints.nodes[_dof].T.Length; i++)
        {
            T[i + 1] = MainParameters.Instance.joints.nodes[_dof].T[i];
            Q[i + 1] = MainParameters.Instance.joints.nodes[_dof].Q[i];
        }
        MainParameters.Instance.joints.nodes[_dof].T = MathFunc.MatrixCopy(T);
        MainParameters.Instance.joints.nodes[_dof].Q = MathFunc.MatrixCopy(Q);

        transform.parent.GetComponentInChildren<GameManager>().InterpolationDDL();
        transform.parent.GetComponentInChildren<GameManager>().DisplayDDL(_dof, true);

        return node + 1;
    }
}
