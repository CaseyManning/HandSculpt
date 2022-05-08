using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatGun : MonoBehaviour
{
    LineRenderer lr;

    float darkenRange = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, transform.position);
        RaycastHit hit;
        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        if (Physics.Raycast(transform.position, transform.right, out hit, 100, layerMask))
        {
            lr.SetPosition(1, hit.point);

            GameObject ball = hit.transform.gameObject;
            if (ball.CompareTag("Object"))
            {
                Mesh m = ball.GetComponent<MeshFilter>().mesh;
                Vector3[] vertices = m.vertices;

                Color[] colors = new Color[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    if(Vector3.Distance(hit.point, ball.transform.TransformPoint(vertices[i])) < darkenRange)
                    {
                        colors[i] = Color.red;
                    } else
                    {
                        if (m.colors.Length > i)
                        {
                            colors[i] = m.colors[i];
                        } else
                        {
                            colors[i] = new Color(255, 250, 250);
                        }
                    }
                }
                m.colors = colors;
            }

        } else
        {
            lr.SetPosition(1, transform.position);
        }
        
    }
}
