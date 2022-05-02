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

    public GameObject selected;

    public static float grabRange = 0.3f;

    LineRenderer line;

    private void Awake()
    {
        hand = GetComponent<OVRHand>();
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
    }

    public Vector3 getCenter()
    {
        if (left)
        {
            return transform.position + 0.1f * transform.right;
        }
        else
        {
            return transform.position - 0.1f * transform.right;
        }
    }

    GameObject nearestObj()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Object");
        GameObject nearest = null;
        float nearestDist = 9999;
        foreach(GameObject g in objs)
        {
            float dist = Vector3.Distance(g.transform.position, getCenter());
            if (dist < nearestDist && dist < grabRange)
            {
                nearest = g;
                nearestDist = dist;
            }
        }
        return nearest;
    }

    private void Update()
    {
        if(hand.IsSystemGestureInProgress)
        {
            return;
        }


        selected = nearestObj();
        if (selected != null)
        {
            line.enabled = true;
            line.SetPosition(0, getCenter());
            line.SetPosition(1, selected.transform.position);

        } else
        {
            line.enabled = false;
        }
        detectPinches();
    }

    void detectPinches()
    {
        if (hand.GetFingerIsPinching(OVRHand.HandFinger.Index))
        {
            OnIndexPinch.Invoke(this);

            if (left) {
                leftPinching = true;
            }
            else {
                rightPinching = true;
            }
        }
        else
        {
            if (left) {
                leftPinching = false;
            }
            else {
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
        }
        else
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
