using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sculptable : MonoBehaviour
{
    Mesh deformingMesh;
    Vector3[] originalVertices, displacedVertices;
    Vector3[] vertexVelocities;


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
        vertexVelocities = new Vector3[originalVertices.Length];
    }

    void Update()
    {
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
    }

    void UpdateVertex(int i)
    {
        Vector3 velocity = vertexVelocities[i];
        displacedVertices[i] += velocity * Time.deltaTime;
    }

    public int nearestVertex(Vector3 pos)
    {

        int nearestI = -1;
        float nearestDist = 0;
        for(int i = 0; i < deformingMesh.vertexCount; i++)
        {
            if (nearestI == -1 || nearestDist > Vector3.Distance(pos, deformingMesh.vertices[i]))
            {
                nearestDist = Vector3.Distance(pos, deformingMesh.vertices[i]);
                nearestI = i;
            }
        }
        return nearestI;
    }

    Vector3 getForce(Vector3 start, Vector3 current)
    {
        return current - start;
    }

    public void pinch(Vector3 current, Vector3 last)
    {
        //Vector3 f = getForce(last, current);
        //print(f);
        //f.Normalize();
        //
        //for (int i = 0; i < displacedVertices.Length; i++)
        //{
        //    Vector3 vr = displacedVertices[i] - current;
        //    Vector3 disp = current - last;
        //    Vector3 r_e = new Vector3(vr.x, vr.y, vr.z);
        //
        //    float epsilon = 2; //radial scale
        //
        //    float r = vr.magnitude;
        //
        //    float re = Mathf.Sqrt(r * r + radiusScale_1 * radiusScale_1); //regularized distance: sqrt(r^2 + epsilon^2)
        //
        //    Vector3 dis = Vector3.zero;
        //    if (brushMode == BrushModes.k_GRAB)
        //    {
        //        //float u = (a - b) / re + a * epsilon * epsilon / (2 * re * re * re) + b * r * r / (re * re * re);
        //        float u = (a - b) / re + a * radiusScale_1 * radiusScale_1 / (2 * re * re * re) + b * r * r / (re * re * re);
        //        //float u = ((a - b) / re + (b * r * r) / (re * re * re) + (a / 2) * (epsilon * epsilon) / (re * re * re));
        //        u *= c;
        //        dis = f * u;
        //    }
        //    displacedVertices[i] += dis * multiplier;
        //}
        //
        //deformingMesh.vertices = displacedVertices;
        //deformingMesh.RecalculateNormals();
        //
        //GetComponent<MeshCollider>().sharedMesh = null; //TODO: switch to box collider with bounds
        //GetComponent<MeshCollider>().sharedMesh = deformingMesh;

        Vector3 point = current;
        Vector3 delta = current - last;
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            AddForceToVertex(i, transform.InverseTransformPoint(point), transform.InverseTransformVector(delta));
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
        
        int vi = nearestVertex(point);
        print(vi);
        print(displacedVertices.Length);
        Vector3 n = Vector3.one;// deformingMesh.normals[vi];

        SculptManager.SetPointerVisual(transform.localToWorldMatrix * current, Quaternion.LookRotation(n, Vector3.one), 1f);
    }

    public void pinchEnd()
    {
        
    }

    public void sculpt(OVRSkeleton hand)
    {
        OVRBone[] points = new OVRBone[12] {
            hand.Bones[(int)OVRPlugin.BoneId.Hand_Index1],
            hand.Bones[(int)OVRPlugin.BoneId.Hand_Index2],
            hand.Bones[(int)OVRPlugin.BoneId.Hand_Index3],
            hand.Bones[(int)OVRPlugin.BoneId.Hand_Middle1],
            hand.Bones[(int)OVRPlugin.BoneId.Hand_Middle2],
            hand.Bones[(int)OVRPlugin.BoneId.Hand_Middle3],
            hand.Bones[(int)OVRPlugin.BoneId.Hand_Ring1],
            hand.Bones[(int)OVRPlugin.BoneId.Hand_Ring2],
            hand.Bones[(int)OVRPlugin.BoneId.Hand_Ring3],
            hand.Bones[(int)OVRPlugin.BoneId.Hand_Thumb1],
            hand.Bones[(int)OVRPlugin.BoneId.Hand_Thumb2],
            hand.Bones[(int)OVRPlugin.BoneId.Hand_Thumb3]
        };
        foreach (OVRBone bone in points)
        {
            Vector3 pos = transform.InverseTransformPoint(bone.Transform.position);

            for (int i = 0; i < displacedVertices.Length; i++)
            {
                if (Vector3.Distance(displacedVertices[i], pos) < 0.15f)
                {
                    Vector3 dir = (displacedVertices[i] - pos).normalized;
                    dir = -deformingMesh.normals[i];
                    float alignment = Vector3.Dot((displacedVertices[i] - pos).normalized, dir);
                    float dist = (displacedVertices[i] - pos).magnitude;
                    dist *= 2;
                    displacedVertices[i] += dir * Time.deltaTime * (1 / 1 + (dist / 0.1f));
                }
            }
        }
        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
    }

    void AddForceToVertex(int i, Vector3 current, Vector3 delta)
    {
        Vector3 pointToVertex = displacedVertices[i] - current;
        float force = 5f;
        float attenuatedForce = force / (1 + 5 * pointToVertex.sqrMagnitude);

        Vector3 moveAmt = attenuatedForce * delta;
        displacedVertices[i] += moveAmt;
    }
}
