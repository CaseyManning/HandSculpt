using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class HandGesture : UnityEvent<CustomHand> { };

public static class StructExts
{
    public static T Clone<T>(this T val) where T : struct => val;
}

[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerData;

    public HandGesture onRecognized;
    public HandGesture onEnd;

}

[System.Serializable]
public struct GHand
{
    public OVRSkeleton skeleton;
    public  List<OVRBone> fingerBones;

    public Gesture prevGesture;

    public List<Gesture> gestures;

    public CustomHand customHand;


}

public class GestureDetector : MonoBehaviour
{

    public List<Gesture> gestures;

    public float threshold = 0.07f;

    public GHand left;
    public GHand right;

    GHand[] hands;

    // Start is called before the first frame update
    void Start()
    {
        left.fingerBones = new List<OVRBone>(left.skeleton.Bones);
        left.prevGesture = new Gesture();

        right.fingerBones = new List<OVRBone>(right.skeleton.Bones);
        right.prevGesture = new Gesture();

        gestures[0].onRecognized.AddListener(ObjectManager.OnGrab);
        gestures[0].onEnd.AddListener(ObjectManager.OnGrabStop);

        gestures[1].onRecognized.AddListener(ObjectManager.OnPoke);
        gestures[1].onEnd.AddListener(ObjectManager.OnPokeStop);

        right.gestures = new List<Gesture>();
        foreach(Gesture g in gestures)
        {
            Gesture n = g.Clone();
            n.fingerData = new List<Vector3>(g.fingerData);
            right.gestures.Add(n);

        }

        for(int i = 0; i <gestures.Count; i++)
        {
            
            for(int j = 0; j < gestures[i].fingerData.Count; j++)
            {
                gestures[i].fingerData[j] *= -1;
            }
        }
        left.gestures = new List<Gesture>(gestures.ToArray());


        hands = new GHand[] { left, right };

    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < hands.Length; i++) {
            Gesture currentGesture = Recognize(i);
            bool hasRecognized = !currentGesture.Equals(new Gesture());
            if (hasRecognized && !currentGesture.Equals(hands[i].prevGesture))
            {
                hands[i].prevGesture = currentGesture;
                currentGesture.onRecognized.Invoke(hands[i].customHand);
                print("New Gesture Found: " + currentGesture.name);
            }
            else if (!hasRecognized)
            {
                if (hands[i].prevGesture.onEnd != null)
                {
                    hands[i].prevGesture.onEnd.Invoke(hands[i].customHand);
                }
            }
            if (!hasRecognized)
            {
                hands[i].prevGesture = new Gesture();
            }
        }
    }

    public void Save()
    {
        Gesture g = new Gesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        print("—————————————");
        foreach(var bone in right.fingerBones)
        {
            data.Add(right.skeleton.transform.InverseTransformPoint(bone.Transform.position));
            print(right.skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }
        g.fingerData = data;
        print("—————————————");
        gestures.Add(g);
    }

    Gesture Recognize(int hand)
    {
        Gesture current = new Gesture();
        float currentMin = Mathf.Infinity;

        foreach(var gesture in hands[hand].gestures)
        {
            float sumDist = 0;
            bool isDiscarded = false;
            for(int i = 0; i < hands[hand].fingerBones.Count; i++)
            {
                Vector3 currentData = hands[hand].skeleton.transform.InverseTransformPoint(hands[hand].fingerBones[i].Transform.position);
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
