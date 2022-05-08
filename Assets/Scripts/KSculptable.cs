using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSculptable : MonoBehaviour
{
    bool dragging = false;

    Vector3 startPos;
    Vector3 lastPos;

    public float radiusScale_1 = 1f;
    public float elasticShearModulus = 1;
	public float poissonRatio = 0.4f;

    float a;
    float b;
    float c;

    public float multiplier = 0.01f;

    enum BrushModes
    {
        k_GRAB
    }

    BrushModes brushMode = BrushModes.k_GRAB;

    Mesh deformingMesh;
    Vector3[] originalVertices, displacedVertices;

    // Start is called before the first frame update
    void Start()
    {
        a = 1 / (4 * Mathf.PI * elasticShearModulus);
        b = a / (4 * (1 - poissonRatio));
        c = 2 / (3 * a - 2 * b);

        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
        GetComponent<MeshCollider>().sharedMesh = deformingMesh;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Vector3 pos;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                pos = hit.transform.position;
                if (!dragging)
                {
                    print("starting drag");
                    startPos = pos;
                    dragging = true;
                }
            }
            
        }
        if(Input.GetMouseButton(0) && dragging)
        {
            Vector3 mpos = Input.mousePosition;
            mpos.z = Vector3.Distance(Camera.main.transform.position, startPos);
            Vector3 curr = Camera.main.ScreenToWorldPoint(mpos);
            displace_grab(curr, lastPos);
            lastPos = curr;
        } else
        {
            dragging = false;
        }
    }

    Vector3 getForce(Vector3 start, Vector3 current)
    {
        return current - start;
    }

    void displace_grab(Vector3 current, Vector3 last)
    {
        Vector3 f = getForce(last, current);
        print(f);
        f.Normalize();

        for (int i = 0; i < displacedVertices.Length; i++)
        {
            Vector3 vr = displacedVertices[i] - current;
            Vector3 disp = current - last;
            Vector3 r_e = new Vector3(vr.x, vr.y, vr.z);

            float epsilon = 2; //radial scale

            float r = vr.magnitude;

            float re = Mathf.Sqrt(r * r + radiusScale_1 * radiusScale_1); //regularized distance: sqrt(r^2 + epsilon^2)

            Vector3 dis = Vector3.zero;
            if (brushMode == BrushModes.k_GRAB)
            {
                //float u = (a - b) / re + a * epsilon * epsilon / (2 * re * re * re) + b * r * r / (re * re * re);
                float u = (a - b) / re + a * radiusScale_1 * radiusScale_1 / (2 * re * re * re) + b * r * r / (re * re * re);
                //float u = ((a - b) / re + (b * r * r) / (re * re * re) + (a / 2) * (epsilon * epsilon) / (re * re * re));
                u *= c;
                dis = f * u;
            }
            displacedVertices[i] += dis*multiplier;
        }

        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = null; //TODO: switch to box collider with bounds
        GetComponent<MeshCollider>().sharedMesh = deformingMesh;
        
    }
}
