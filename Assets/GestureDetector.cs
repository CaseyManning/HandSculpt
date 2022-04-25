using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerData;

    public UnityEvent onRecognized;
    public UnityEvent onEnd;
}

//[System.Serializable]
//public struct GHand
//{
//    public OVRSkeleton skeleton;
//    public  List<OVRBone> fingerBones;
//
//    public Gesture prevGesture;
//}

public class GestureDetector : MonoBehaviour
{

    public List<Gesture> gestures;

    public float threshold = 0.07f;

    public OVRSkeleton skeleton;
    private List<OVRBone> fingerBones;

    Gesture prevGesture;

    //public GHand left;
    //public GHand right;


    // Start is called before the first frame update
    void Start()
    {
        fingerBones = new List<OVRBone>(skeleton.Bones);
        prevGesture = new Gesture();

        if(skeleton.gameObject.GetComponent<CustomHand>().left)
        {
            for(int i = 0; i <gestures.Count; i++)
            {
                for(int j = 0; j < gestures[i].fingerData.Count; j++)
                {
                    gestures[i].fingerData[j] *= -1;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        Gesture currentGesture = Recognize();
        bool hasRecognized = !currentGesture.Equals(new Gesture());
        if(hasRecognized && !currentGesture.Equals(prevGesture))
        {
            prevGesture = currentGesture;
            currentGesture.onRecognized.Invoke();
            print("New Gesture Found");
        } else if(!hasRecognized)
        {
            if (prevGesture.onEnd != null)
            {
                prevGesture.onEnd.Invoke();
            }
        }
        if(!hasRecognized)
        {
            prevGesture = new Gesture();
        }
    }

    public void Save()
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        print("—————————————");
        foreach(var bone in fingerBones)
        {
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
            print(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }
        g.fingerData = data;
        print("—————————————");
        gestures.Add(g);
    }

    Gesture Recognize()
    {
        Gesture current = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach(var gesture in gestures)
        {
            float sumDist = 0;
            bool isDiscarded = false;
            for(int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerData[i]);
                if(distance > threshold)
                {
                    isDiscarded = true;
                    break;
                }
                sumDist += distance;
            }
            if(!isDiscarded && sumDist < currentMin)
            {
                currentMin = sumDist;
                current = gesture;
            }
        }
        return current;
    }
}
