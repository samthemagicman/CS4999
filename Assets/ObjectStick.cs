using System;
using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectStick : MonoBehaviour
{
    public float ratio = 1;
    
    public MeshRenderer originalObjectPositionIdentifier;
    private Material originalObjectMaterial;
    
    public ObjectManipulator joystick;
    [HideInInspector]
    public ObjectManipulator currentInteractable;

    private Vector3 joystickStartPos;
    private Vector3 joystickStartScale;
    private Vector3 originalInteractablePos;
    private Vector3 originalInteractableScale;
    private Quaternion originalInteractableRot;
    private float originalScaleRatio;
    private bool beingDragged = false;

    private void Start()
    {
        joystickStartScale = joystick.transform.localScale;
        originalObjectMaterial = originalObjectPositionIdentifier.material;
        
        // joystick.AllowedManipulations = TransformFlags.None;
        joystick.selectEntered.AddListener(JoystickSelected);
        
        joystick.selectExited.AddListener(arg0 =>
        {
            //joystick.transform.localPosition = Vector3.zero;
            currentInteractable.interactionManager.SelectExit(arg0.interactorObject, currentInteractable);
        });
    }

    private void JoystickSelected(SelectEnterEventArgs selectEnterEventArgs)
    {
        joystickStartPos = joystick.transform.localPosition;
        originalInteractablePos = currentInteractable.transform.position;
        originalInteractableRot = currentInteractable.transform.rotation;
        originalScaleRatio = currentInteractable.transform.localScale.x / joystick.transform.localScale.x;
        originalInteractableScale = currentInteractable.transform.localScale;
    }

    private void JoystickUnselected()
    {
        
    }

    void UpdateJoystickSelected()
    {
        float joystickDistanceFromOrigin = Vector3.Distance(joystick.transform.position, originalObjectPositionIdentifier.transform.position);
        currentInteractable.transform.position = originalInteractablePos + joystick.transform.localPosition * (joystickDistanceFromOrigin * ratio);
        currentInteractable.transform.rotation = joystick.transform.rotation;
        //currentInteractable.transform.localScale = joystick.transform.localScale * originalScaleRatio;
        currentInteractable.transform.localScale = originalInteractableScale * (joystick.transform.localScale.x / joystickStartScale.x);
    }

    void UpdateJoystickNotSelected()
    {
        // Smoothly move back to zero, zero, zero
        joystick.transform.localPosition = Vector3.Lerp(joystick.transform.localPosition, Vector3.zero, Time.deltaTime * 10);
        joystick.transform.rotation = currentInteractable.transform.rotation;
        
        // Smoothly move scale back to original
        joystick.transform.localScale = Vector3.Lerp(joystick.transform.localScale, joystickStartScale, Time.deltaTime * 10);
    }
    

    // Update is called once per frame
    void Update()
    {
        if (joystick.isSelected)
        {
            UpdateJoystickSelected();
        }

        if (!joystick.isSelected)
        {
            UpdateJoystickNotSelected();
        }
        
        // While Joystick is away from originalObjectPositionIdentifier, make originalObjectMaterial more transparent smoothly base on distance
        float distance = Vector3.Distance(joystick.transform.position, originalObjectPositionIdentifier.transform.position);
        originalObjectMaterial.color = new Color(0.5f, 1, 1, Mathf.Lerp(0, 0.5f, distance*2f));//Color.Lerp(originalObjectMaterial.color, new Color(1, 1, 1, Mathf.Lerp(0, 0.5f, distance)), Time.deltaTime * 10);
        if (currentInteractable)
        {
            // joystick.transform.rotation = currentInteractable.transform.rotation;
        }
    }
    
    public void SetActive(bool active)
    {
        joystick.gameObject.SetActive(active);
        originalObjectPositionIdentifier.transform.gameObject.SetActive(active);
    }

    private void DisableObjectManipulator(ObjectManipulator manipulator)
    {
        manipulator.enabled = false;
    }
}
