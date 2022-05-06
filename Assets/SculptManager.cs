using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SculptManager : MonoBehaviour
{
    public static GameObject editing;

    public static float sculptScale = 0.1f;

    public GameObject sculptVisual;
    static SculptManager instance;

    void Start()
    {
        instance = this;
    }

    void Update()
    {

    }

    public static void SetPointerVisual(Vector3 position, Quaternion rot, float scale)
    {
        instance.sculptVisual.SetActive(true);
        instance.sculptVisual.transform.position = position;
        instance.sculptVisual.transform.rotation = rot;
        instance.sculptVisual.transform.localScale = Vector3.one * scale * sculptScale;
    }

    public static void deactivateVisual()
    {
        instance.sculptVisual.SetActive(false);
    }
}
