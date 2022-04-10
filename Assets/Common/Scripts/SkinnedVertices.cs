using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SkinnedMeshBone
{
    public Transform bone;
    public float weight;
    public Vector3 delta;
}

[Serializable]
public class SkinnedVertices : MonoBehaviour
{
    Mesh mesh;

    List<List<SkinnedMeshBone>> allBones = new List<List<SkinnedMeshBone>>();

    void Start()
    {
        SkinnedMeshRenderer skin = GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;

        var mesh = new Mesh();
        skin.BakeMesh(mesh);

        Debug.LogFormat("{0} vertices, {1} weights, {2} bones",
            mesh.vertexCount, mesh.boneWeights.Length, skin.bones.Length);

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            allBones.Add(VertexBones(skin, i));

            //if (bones.Count > 1)
            //{
            //    string msg = string.Format("vertex {0}, {1} bones", i, bones.Count);

            //    foreach (Bone bone in bones)
            //        msg += string.Format("\n\t{0} => {1} => {2}", bone.bone.name, bone.weight, bone.delta);

            //    Debug.Log(msg);
            //}
        }
    }

    public static List<SkinnedMeshBone> VertexBones(SkinnedMeshRenderer skin, int index)
    {
        var mesh = new Mesh();
        skin.BakeMesh(mesh);
        Vector3 position = mesh.vertices[index];
        position = skin.transform.TransformPoint(position);

        BoneWeight weights = skin.sharedMesh.boneWeights[index];
        int[] boneIndices = new int[] { weights.boneIndex0, weights.boneIndex1, weights.boneIndex2, weights.boneIndex3 };
        float[] boneWeights = new float[] { weights.weight0, weights.weight1, weights.weight2, weights.weight3 };

        List<SkinnedMeshBone> bones = new List<SkinnedMeshBone>();

        for (int j = 0; j < 4; j++)
        {
            if (boneWeights[j] > 0)
            {
                SkinnedMeshBone bone = new SkinnedMeshBone();
                bones.Add(bone);

                bone.bone = skin.bones[boneIndices[j]];
                bone.weight = boneWeights[j];
                bone.delta = bone.bone.InverseTransformPoint(position);
            }
        }

        return bones;
    }

    public static Vector3 VertexPosition(List<SkinnedMeshBone> bones, int index)
    {
        Vector3 position = Vector3.zero;
        foreach (SkinnedMeshBone bone in bones)
            position += bone.bone.TransformPoint(bone.delta) * bone.weight;
        return position;
    }

    public static Vector3 VertexNormal(Mesh mesh, List<SkinnedMeshBone> bones, int index)
    {
        Vector3 normal = Vector3.zero;
        foreach (SkinnedMeshBone bone in bones)
            normal += bone.bone.TransformDirection(mesh.normals[index]) * bone.weight;

        return normal;
    }

    public static Vector3 VertexTangent(Mesh mesh, List<SkinnedMeshBone> bones, int index)
    {
        Vector3 tangent = Vector3.zero;
        foreach (SkinnedMeshBone bone in bones)
            tangent += bone.bone.TransformDirection(mesh.tangents[index]) * bone.weight;

        return tangent;
    }

    public static void SetTransform(Transform t, Mesh mesh, List<SkinnedMeshBone> bones, int index)
    {
        t.position = VertexPosition(bones, index);
        t.LookAt(VertexTangent(mesh, bones, index), VertexNormal(mesh, bones, index));
    }

    void DrawVertex(int index)
    {
        int boneCount = allBones[index].Count;
        Gizmos.color = (boneCount == 4) ? Color.red :
            (boneCount == 3) ? Color.blue :
            (boneCount == 2) ? Color.green : Color.black;

        Gizmos.DrawWireCube(VertexPosition(allBones[index], index), allBones[index].Count * 0.005f * Vector3.one);
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying && enabled)
        {
            //for (int i = 0; i < mesh.vertexCount/100; i += 1)
            //{
            //    DrawVertex(i + 60 * mesh.vertexCount / 100);
            //}
            DrawVertex(5451);
        }
    }
}