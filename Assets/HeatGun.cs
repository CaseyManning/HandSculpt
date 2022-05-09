using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatGun : MonoBehaviour
{
    LineRenderer lr;

    float darkenRange = 0.03f;

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

                if (m.colors.Length < vertices.Length)
                {
                    Color[] ncolors = new Color[vertices.Length];
                    for (int i = 0; i < vertices.Length; i++) {
                        ncolors[i] = Color.white; new Color(240/255, 218/255, 182/255);
                    }
                    m.colors = ncolors;
                }
                

                Color[] colors = new Color[vertices.Length];

                Vector3[] newVerts = new Vector3[vertices.Length];

                for (int i = 0; i < vertices.Length; i++)
                {
                    newVerts[i] = m.vertices[i];
                    colors[i] = m.colors[i];
                    if(Vector3.Distance(hit.point, ball.transform.TransformPoint(vertices[i])) < darkenRange)
                    {
                        colors[i] = m.colors[i] * 0.87f;
                        newVerts[i] += m.normals[i] * Time.deltaTime * 0.08f;

                    } else
                    {
                        colors[i] = m.colors[i];
                        
                    }

                     
                }
                m.colors = colors;
                m.vertices = newVerts;
            }

        } else
        {
            lr.SetPosition(1, transform.position);
        }
        
    }
}
