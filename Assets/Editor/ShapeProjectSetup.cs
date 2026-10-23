using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ShapeProjectSetup
{
    public static void Create()
    {
        try
        {
            Directory.CreateDirectory("Assets/Shapes");
            AssetDatabase.Refresh();
            Mesh prism = MakeMesh(false);
            Mesh cone = MakeMesh(true);
            AssetDatabase.CreateAsset(prism, "Assets/Shapes/TriangularPrism.asset");
            AssetDatabase.CreateAsset(cone, "Assets/Shapes/Cone.asset");
            Material original = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Cube.mat");
            GameObject prismPrefab = MakePrefab("TriangularPrism", prism, original, new Color(1f, 0.75f, 0.15f));
            GameObject conePrefab = MakePrefab("Cone", cone, original, new Color(1f, 0.35f, 0.2f));
            var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
            Transform items = GameObject.Find("Items").transform;
            var children = new List<Transform>();
            foreach (Transform child in items) children.Add(child);
            for (int i = 0; i < children.Count; i++)
            {
                var item = (GameObject)PrefabUtility.InstantiatePrefab(i % 2 == 0 ? prismPrefab : conePrefab, items);
                item.transform.localPosition = children[i].localPosition;
                item.transform.localRotation = Quaternion.identity;
                item.transform.localScale = children[i].localScale;
                item.name = (i % 2 == 0 ? "TriangularPrism" : "Cone") + "_" + (i + 1).ToString("00");
                UnityEngine.Object.DestroyImmediate(children[i].gameObject);
            }
            if (items.childCount != 12) throw new Exception("Expected 12 items");
            foreach (Transform child in items)
            {
                var collider = child.GetComponent<MeshCollider>();
                if (!child.CompareTag("Item") || !collider.isTrigger || !collider.convex || !child.GetComponent<PickUp>())
                    throw new Exception("Invalid pickup: " + child.name);
            }
            var score = GameObject.Find("Score").GetComponent<ScoreManager>();
            if (!score.gameClearText) throw new Exception("Clear text is missing");
            EditorSceneManager.SaveScene(scene);
            PlayerSettings.productName = "Roll-a-Ball Shapes";
            AssetDatabase.SaveAssets();
            File.WriteAllText("ShapeSetupResult.txt", "PASS: 6 triangular prisms, 6 cones. Meshes, convex trigger colliders, PickUp scripts, Item tags and clear text reference verified. Scene saved.\n");
            EditorApplication.Exit(0);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            File.WriteAllText("ShapeSetupResult.txt", "FAIL: " + ex);
            EditorApplication.Exit(1);
        }
    }

    private static GameObject MakePrefab(string name, Mesh mesh, Material source, Color color)
    {
        var material = new Material(source) { name = name };
        material.SetColor("_BaseColor", color);
        AssetDatabase.CreateAsset(material, "Assets/Shapes/" + name + ".mat");
        var item = new GameObject(name);
        item.tag = "Item";
        item.AddComponent<MeshFilter>().sharedMesh = mesh;
        item.AddComponent<MeshRenderer>().sharedMaterial = material;
        var collider = item.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
        collider.convex = true;
        collider.isTrigger = true;
        item.AddComponent<PickUp>();
        var prefab = PrefabUtility.SaveAsPrefabAsset(item, "Assets/PreFabs/" + name + ".prefab");
        UnityEngine.Object.DestroyImmediate(item);
        return prefab;
    }

    private static Mesh MakeMesh(bool cone)
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        Action<Vector3, Vector3, Vector3> face = (a, b, c) =>
        {
            int start = vertices.Count;
            vertices.Add(a); vertices.Add(b); vertices.Add(c);
            triangles.Add(start); triangles.Add(start + 1); triangles.Add(start + 2);
        };
        if (cone)
        {
            for (int i = 0; i < 32; i++)
            {
                float a = 2 * Mathf.PI * i / 32;
                float b = 2 * Mathf.PI * (i + 1) / 32;
                var p = new Vector3(0.5f * Mathf.Cos(a), -0.5f, 0.5f * Mathf.Sin(a));
                var q = new Vector3(0.5f * Mathf.Cos(b), -0.5f, 0.5f * Mathf.Sin(b));
                face(p, Vector3.up * 0.5f, q);
                face(p, q, Vector3.down * 0.5f);
            }
        }
        else
        {
            var a = new Vector3(-0.5f, -0.5f, -0.5f);
            var b = new Vector3(0.5f, -0.5f, -0.5f);
            var c = new Vector3(0, 0.5f, -0.5f);
            var d = a + Vector3.forward;
            var e = b + Vector3.forward;
            var f = c + Vector3.forward;
            face(a, c, b); face(d, e, f);
            face(a, b, e); face(a, e, d);
            face(b, c, f); face(b, f, e);
            face(c, a, d); face(c, d, f);
        }
        var mesh = new Mesh { name = cone ? "Cone" : "TriangularPrism" };
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
