using MixedReality.Toolkit.SpatialManipulation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides functionality to move the attached object to a given position
/// </summary>
public class MoveToTargetTask : MonoBehaviour
{
    #region public variables
    public UnityEvent OnCompleted = new UnityEvent();
    [HideInInspector]
    public bool TaskIsComplete = false;

    [HideInInspector] public bool beingDragged = false;
    public bool alwaysShowTarget = true;

    [Tooltip("Will reset the object to the original position if the user fails")]
    public bool resetPositionOnDragFailed = false;

    private Vector3 originalPosition;

    public bool useTransform = true;
    public bool useRotation = false;
    public bool useScale = false;

    /// <summary>
    /// The material to use for the target visual
    /// </summary>
    public Material targetMaterial;
    public Material targetSuccessMaterial;
    public Vector3 targetTransform;
    public Quaternion targetRotation;
    public Vector3 targetScale;

    /// <summary>
    /// The tolerance (in degrees) before the target rotation is accepted
    /// </summary>
    public float rotationTargetTolerance;
    /// <summary>
    /// The margin before the target position is accepted
    /// </summary>
    public float transformTargetTolerance;
    public float scaleTargetTolerance;
    #endregion
    #region private variables
    private GameObject targetGameObject;

    private ObjectManipulator objectManipulator;
    #endregion

    #region getters/setters
    public bool isInTargetScale
    {
        get
        {
            // Check if the position is within the margin of the targetTransform
            if (Vector3.Distance(transform.localScale, targetScale) <= scaleTargetTolerance)
            {
                return true;
            }

            return false;
        }
    }
    public bool isInTargetTransform
    {
        get
        {
            // Check if the position is within the margin of the targetTransform
            if (Vector3.Distance(transform.position, targetTransform) <= transformTargetTolerance)
            {
                return true;
            }
            return false;
        }
    }
    public bool isInTargetRotation
    {
        get
        {
            // Check if the rotation is within the tolerance of the targetRotation
            if (Quaternion.Angle(transform.rotation, targetRotation) <= rotationTargetTolerance)
            {
                return true;
            }
            return false;
        }
    }
    #endregion

    void Start()
    {
        if (alwaysShowTarget)
        {
            CreateDragPreview();
        }
        originalPosition = transform.position;
        objectManipulator = GetComponent<ObjectManipulator>();
        if (objectManipulator)
        {
            objectManipulator.firstSelectEntered.AddListener(arg0 =>
            {
                CreateDragPreview();
                beingDragged = true;
            });
            objectManipulator.lastSelectExited.AddListener(arg0 =>
            {
                beingDragged = false;
                DestroyDragPreview();
                if (CheckCompletion()) {
                    OnCompleted.Invoke();
                    TaskIsComplete = true;
                }
                else // failed
                {
                    if (resetPositionOnDragFailed) transform.position = originalPosition;
                    TaskIsComplete = false;
                }
            });
        }
    }

    void CreateDragPreview()
    {
        if (targetGameObject == null)
        {

            // Create a new empty GameObject and copy the mesh
            MeshFilter sourceMeshFilter = gameObject.GetComponent<MeshFilter>();
            if (sourceMeshFilter != null && sourceMeshFilter.sharedMesh != null)
            {
                MeshRenderer sourceMeshRenderer = gameObject.GetComponent<MeshRenderer>();

                // Create a new empty GameObject
                GameObject newObject = new GameObject("CopiedMeshObject");

                // Copy MeshFilter component
                MeshFilter newMeshFilter = newObject.AddComponent<MeshFilter>();
                newMeshFilter.sharedMesh = Instantiate(sourceMeshFilter.sharedMesh); // Clone the mesh

                // Copy MeshRenderer component
                if (sourceMeshRenderer != null)
                {
                    MeshRenderer newMeshRenderer = newObject.AddComponent<MeshRenderer>();
                    newMeshRenderer.sharedMaterials = sourceMeshRenderer.sharedMaterials;
                }

                // Set the new object as a child of the empty GameObject this script is attached to
                targetGameObject = newObject;
            }
            else
            {
                Debug.LogException(new Exception("Couldn't create preview object"));
            }

            targetGameObject.transform.localScale = targetScale;
            targetGameObject.transform.position = targetTransform;
            targetGameObject.transform.rotation = targetRotation;

            if (targetMaterial)
            {
                setTargetGameObjectMaterial(targetMaterial);
            }
        }
    }

    void setTargetGameObjectMaterial(Material material) {
        if (targetGameObject != null && material != null) {
            MeshRenderer meshRenderer = targetGameObject.GetComponent<MeshRenderer>();
            if (meshRenderer.material != material) {
                meshRenderer.material = material; 
            }
        }
    }

    void DestroyDragPreview()
    {
        if (!alwaysShowTarget && targetGameObject)
        {
            Destroy(targetGameObject.gameObject);
        }
    }

    bool CheckCompletion()
    {
        if (useRotation)
        {
            if (!isInTargetRotation)
            {
                return false;
            }
        }
        if (useTransform)
        {
            if (!isInTargetTransform)
            {
                return false;
            }
        }

        if (useScale)
        {
            if (!isInTargetScale)
            {
                return false;
            }
        }
        return true;
    }

    void Update()
    {
        if (targetGameObject && beingDragged)
        {
            if (!useRotation)
            {
                targetGameObject.transform.rotation = transform.rotation;
            }
            if (!useScale)
            {
                targetGameObject.transform.localScale = transform.localScale;
            }
        }

        if (CheckCompletion()) {
            setTargetGameObjectMaterial(targetSuccessMaterial);
        } else {
            setTargetGameObjectMaterial(targetMaterial);
        };
    }

    private void OnDestroy()
    {
        if (targetGameObject != null)
        {
            Destroy(targetGameObject);
        }
    }
}
