using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlSegmentGeneric : MonoBehaviour
{
    public float angle { get; protected set;}
    protected float initAngle;
    int node;

    protected Vector3 mouseDistance;
    protected Vector3 lastPosition;
    protected float avatarRotationDragSpeed = 5f;

    protected GameObject arrowPrefab;
    protected GameObject arrow;
    protected abstract Vector3 arrowOrientation { get; }
    protected abstract Quaternion circleOrientation { get; }

    protected GameObject circlePrefab;
    protected GameObject circle;

    // Abstract getter
    public abstract string dofName { get; }
    public abstract int avatarIndexDDL { get; }
    public abstract int jointSubIndex { get; }
    public abstract int qIndex { get; }
    protected abstract DrawingCallback drawingCallback { get; }
    public abstract int direction { get; }

    // Cache variables
    protected bool isInitialized = false;
    protected AvatarManager avatarManager;
    protected DrawManager drawManager;
    protected GameManager gameManager;
    protected StatManager statManager;

    // Delegate
    protected delegate void DrawingCallback(int _avatarIndex, float value);
    public delegate int GetNodeCallback(int _avatarIndex, int index);

    public virtual void Init(int _avatarIndex, GetNodeCallback getNodeCallback)
    {
        node = getNodeCallback(_avatarIndex, avatarIndexDDL);

        avatarManager = ToolBox.GetInstance().GetManager<AvatarManager>();
        drawManager = ToolBox.GetInstance().GetManager<DrawManager>();
        gameManager = ToolBox.GetInstance().GetManager<GameManager>();
        statManager = ToolBox.GetInstance().GetManager<StatManager>();

        angle = (float)avatarManager.LoadedModels[0].Q[qIndex];

        if (!arrow)
        {
            arrowPrefab = (GameObject)Resources.Load("Arrow", typeof(GameObject));
            arrow = Instantiate(arrowPrefab, gameObject.transform.position + new Vector3(0, 0, 0.1f), Quaternion.identity);
        }
        else
            arrow.SetActive(true);

        if (!circle)
        {
            circlePrefab = (GameObject)Resources.Load("NodeCircle", typeof(GameObject));
            circle = Instantiate(circlePrefab, gameObject.transform.position, Quaternion.identity);
            circle.transform.rotation = circleOrientation;
        }
        else
            circle.SetActive(true);

        isInitialized = true;
    }

    public void DestroyCircle()
    {
        if (!isInitialized) return;

        if (circle)
            Destroy(circle);
        if (arrow)
            Destroy(arrow);
        circle = null;
        arrow = null;
    }

    void Update()
    {
        if (!isInitialized) return;
        if (statManager.currentJointSubIdx != jointSubIndex) return;

        if (arrow)
            arrow.transform.position = gameObject.transform.position + new Vector3(0, 0, 0.1f);

        if (circle)
            circle.transform.position = gameObject.transform.position;
    }

    protected float CurrentAngle {
        get { return avatarManager.LoadedModels[0].Joints.nodes[avatarIndexDDL].Q[node]; } 
        set { avatarManager.LoadedModels[0].Joints.nodes[avatarIndexDDL].Q[node] = value; }
    }

    void OnMouseDown()
    {
        if (!isInitialized) return;
        if (statManager.currentJointSubIdx != jointSubIndex) return;

        if(drawManager.IsEditing)
        {
            initAngle = CurrentAngle;
            lastPosition = Input.mousePosition;
            mouseDistance.x = angle * 30f;
            statManager.dofName = dofName;
        }
    }

    void OnMouseDrag()
    {
        if (!isInitialized) return;
        if (statManager.currentJointSubIdx != jointSubIndex) return;

        if (drawManager.IsEditing)
        {
            Vector3 newPosition = Input.mousePosition;
            mouseDistance += newPosition - lastPosition;

            int _avatarIndex = 0;
            HandleDof(_avatarIndex, mouseDistance.x / 30);
            lastPosition = newPosition;
        }
    }

    void OnMouseUp()
    {
        if (!isInitialized) return;
        if (statManager.currentJointSubIdx != jointSubIndex) return;

        if (drawManager.IsEditing)
        {
            mouseDistance = Vector3.zero;
            lastPosition = Vector3.zero;
        }
    }

    protected virtual void HandleDof(int _avatarIndex, float _nextAngle)
    {
        if (!isInitialized) return;
        if (statManager.currentJointSubIdx != jointSubIndex) return;
        if (angle == _nextAngle) return;

        CurrentAngle = _nextAngle / avatarRotationDragSpeed + initAngle;
        gameManager.InterpolationDDL(_avatarIndex);
        gameManager.DisplayDDL(avatarIndexDDL, true);

        drawingCallback(0, CurrentAngle);
    }
}
