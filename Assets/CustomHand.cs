using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class CustomHand : MonoBehaviour
{
    public FingerPinch OnIndexPinch = new FingerPinch();
    public FingerPinch OnMiddlePinch = new FingerPinch();
    public FingerPinch OnMiddlePinchStop = new FingerPinch();

    public OVRHand hand { get; private set; } = null;

    public bool left = false;

    public static bool leftPinching = false;
    public static bool rightPinching = false;

    bool wasMiddlePinching = false;

    private void Awake()
    {
        hand = GetComponent<OVRHand>();
    }

    private void Update()
    {
        if(hand.IsSystemGestureInProgress)
        {
            return;
        }

        if(hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            OnIndexPinch.Invoke(this);

            if(left)
            {
                leftPinching = true;
            } else
            {
                rightPinching = true;
            }
        } else
        {
            if (left)
            {
                leftPinching = false;
            }
            else
            {
                rightPinching = false;
            }
        }

        if (hand.GetFingerIsPinching(OVRHand.HandFinger.Middle))
        {
            if (!wasMiddlePinching)
            {
                OnMiddlePinch.Invoke(this);
                wasMiddlePinching = true;
            }
        } else
        {
            if (wasMiddlePinching)
            {
                wasMiddlePinching = false;
                OnMiddlePinchStop.Invoke(this);
            }
        }
        
    }

    [Serializable]
    public class FingerPinch : UnityEvent<CustomHand> {};
    
}
