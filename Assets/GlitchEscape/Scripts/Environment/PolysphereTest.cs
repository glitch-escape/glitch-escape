using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class PolysphereTest : MonoBehaviour {
    public Mesh initialMesh;

    void ExplodeMesh() {
        var mf = GetComponent<MeshFilter>();
        var mesh = initialMesh;
        var vertices = mesh.vertices;
        var n = mesh.vertexCount;
        for (int i = 0; i < n; ++i) {
            vertices[i].z = Random.Range(-1f, 1f);
        }
        var newMesh = new Mesh();
        newMesh.vertices = vertices;
        newMesh.uv = mesh.uv;
        newMesh.normals = mesh.normals;
        newMesh.triangles = mesh.triangles;
        mf.mesh = newMesh;
    }

    public bool reset = false;
    public bool explodedMesh = false;

    void Update() {
        if (!explodedMesh) {
            explodedMesh = true;
            ExplodeMesh();
        } else if (reset) {
            reset = false;
            if (initialMesh != null)
                GetComponent<MeshFilter>().mesh = initialMesh;
        }
    }
}
