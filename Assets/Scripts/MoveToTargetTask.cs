using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides functionality to move the attached object to a given position
/// </summary>
public class MoveToTargetTask : MonoBehaviour
{
    public UnityEvent OnCompleted = new UnityEvent();

    public bool useTransform = true;
    public bool useRotation = false;
    public bool useScale = false;

    /// <summary>
    /// The material to use for the target visual
    /// </summary>
    public Material targetMaterial;
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

    private GameObject targetGameObject;
    public GameObject arrowObjectPrefab;
    private GameObject arrowObject;

    private ObjectManipulator objectManipulator;

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

    void Start()
    {
        objectManipulator = GetComponent<ObjectManipulator>();
        if (objectManipulator)
        {
            objectManipulator.OnManipulationStarted.AddListener(arg0 =>
            {
                CreateDragPreview();
            });
            objectManipulator.OnManipulationEnded.AddListener(arg0 =>
            {
                DestroyDragPreview();
            });
        }
    }

    void CreateDragPreview()
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

        targetGameObject.transform.localScale = transform.localScale;
        targetGameObject.transform.position = targetTransform;
        targetGameObject.transform.rotation = targetRotation;

        if (targetMaterial)
        {
            targetGameObject.GetComponent<MeshRenderer>().material = targetMaterial;
        }

        arrowObject = Instantiate(arrowObjectPrefab);
    }

    void DestroyDragPreview()
    {
        if (targetGameObject)
        {
            Destroy(targetGameObject.gameObject);
        }

        if (arrowObject)
        {
            Destroy(arrowObject.gameObject);
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

        OnCompleted.Invoke();
        return true;
    }

    void AnimateArrow()
    {
        if (targetGameObject != null && arrowObject != null)
        {
            // Get the bounds of the target object in its local space
            Bounds bounds = targetGameObject.GetComponent<Renderer>().bounds;

            // Set the arrow's position to be on top of the target object using its bounds
            Vector3 arrowPosition = new Vector3(targetGameObject.transform.position.x,
                bounds.max.y + 0.03f, // Place the arrow on top (Y-axis) of the object's bounds
                targetGameObject.transform.position.z);

            // Apply the arrow's position considering the rotation of the target object
            arrowObject.transform.position = arrowPosition;
        }
    }

    void Update()
    {
        CheckCompletion();
        AnimateArrow();
    }
}
