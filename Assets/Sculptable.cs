using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sculptable : MonoBehaviour
{
    Mesh deformingMesh;
    Vector3[] originalVertices, displacedVertices;
    Vector3[] vertexVelocities;

    void Start()
    {
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

    public void pinch(Vector3 point, Vector3 delta)
    {
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
        
        SculptManager.SetPointerVisual(transform.localToWorldMatrix * point, Quaternion.LookRotation(n, Vector3.one), 1f);
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
