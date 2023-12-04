using System;
using System.Collections;
using System.Collections.Generic;
using MixedReality.Toolkit.Input;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;
using UnityEngine.Events;

public class MiniStickLock : MonoBehaviour
{
    
    public Collider activatorCollider;
    public TestMiniStick miniStick;
    public MRTKRayInteractor interactor;
    public UnityEvent onFingerEnter = new UnityEvent();
    private bool canLock = false;
    private bool canUnlock = true;
    private bool locked = false;

    private void Start()
    {
        interactor.selectEntered.AddListener(arg0 =>
        {
            canLock = true;
        });
        
        interactor.selectExited.AddListener(arg0 =>
        {
            canLock = false;
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        // Finger has interacted with it. NearInteractionModeDetector is on the fingertip of hands
        if (other == activatorCollider && canLock && !locked)
        {
            locked = true;
            onFingerEnter.Invoke();
            miniStick.Activate(interactor, interactor.firstInteractableSelected.transform.GetComponent<ObjectManipulator>());
        } else if (other == activatorCollider && locked)
        {
            locked = false;
            miniStick.Disable();
        }
    }
}
