using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TestMiniStick : MonoBehaviour
{
    public MRTKRayInteractor interactor;
    public ObjectStick currentInterableJoystickVisual;
    private ObjectManipulator currentInteractable;
    private bool active = false;
    
    public Orbital orbital;
    
    void Start()
    {
        Disable();
    }

    public void Activate(XRBaseControllerInteractor interactor, ObjectManipulator interactable)
    {
        active = true;
        currentInteractable = interactable;
        currentInterableJoystickVisual.currentInteractable = interactable;
        currentInterableJoystickVisual.SetActive(true);
    }

    public void Disable()
    {
        active = false;
        currentInterableJoystickVisual.currentInteractable = null;
        currentInterableJoystickVisual.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Interactable has been destroyed
        if (active && currentInteractable == null)
        {
            Disable();
        }
    }
}
