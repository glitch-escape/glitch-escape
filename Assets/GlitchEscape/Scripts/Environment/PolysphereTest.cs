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
        var v0 = mesh.vertices;
        var t0 = mesh.triangles;
        var u0 = mesh.uv;
        var n = mesh.triangles.Length;

        var tris = new int[n];
        var verts = new Vector3[n];
        var uvs = new Vector2[n];

        var yoffset = Random.Range(-1f, 1f);
        var camera = Camera.main;
        for (int i = 0; i < n; ++i) {
            if (i % 3 == 0) {
                yoffset = Random.Range(-0.5f, 0.5f);
            }
            tris[i] = i;
            verts[i] = v0[t0[i]];
            var worldPoint = v0[t0[i]];
            var screenPoint = camera.WorldToScreenPoint(worldPoint);
            var ray = camera.ScreenPointToRay(screenPoint);
            var distance = Vector3.Distance(worldPoint, camera.transform.position);
            verts[i] = ray.GetPoint(distance + yoffset + Random.Range(-0.05f, 0.05f));
            uvs[i] = u0[t0[i]];
        }
        var newMesh = new Mesh();
        newMesh.vertices = verts;
        newMesh.uv = uvs;
        newMesh.triangles = tris;
        newMesh.RecalculateNormals();
        newMesh.RecalculateTangents();
        newMesh.RecalculateBounds();
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
        // transform.Rotate(Vector3.up, 360f * 0.7f * Time.deltaTime);
    }
}
