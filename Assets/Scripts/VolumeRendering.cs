using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeRendering : MonoBehaviour
{

    [SerializeField] protected Shader shader;
    protected Material material;
    public Texture volume;
    public Texture dataMap;
    private Vector3 rotateAxis;
    private float zPlane;
    public Vector3 pickRayPos;
    public Vector3 pickRayDir;

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(new Vector3(1, 0, 0), -30);
        transform.Rotate(new Vector3(0, 1, 0), -45);
        material = new Material(shader);
        GetComponent<MeshFilter>().sharedMesh = Build();
        GetComponent<MeshRenderer>().sharedMaterial = material;

        rotateAxis = new Vector3(0, 1, 0);

        material.SetTexture("_Volume", volume);
        material.SetTexture("_DataMap", dataMap);
        material.SetVector("_PickRayPos", pickRayPos);
        material.SetVector("_PickRayDir", pickRayDir.normalized);

        zPlane = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(rotateAxis, 30 * Time.deltaTime);

        zPlane += Input.GetAxisRaw("Vertical") * Time.deltaTime / 2;

        if (zPlane < 0) zPlane = 0;
        if (zPlane > 1) zPlane = 1;

        material.SetFloat("_Plane", zPlane);
    }

    Mesh Build()
    {
        var vertices = new Vector3[] {
            new Vector3 (-0.5f, -0.5f, -0.5f),
            new Vector3 ( 0.5f, -0.5f, -0.5f),
            new Vector3 ( 0.5f,  0.5f, -0.5f),
            new Vector3 (-0.5f,  0.5f, -0.5f),
            new Vector3 (-0.5f,  0.5f,  0.5f),
            new Vector3 ( 0.5f,  0.5f,  0.5f),
            new Vector3 ( 0.5f, -0.5f,  0.5f),
            new Vector3 (-0.5f, -0.5f,  0.5f),
        };
        var triangles = new int[] {
            0, 2, 1,
            0, 3, 2,
            2, 3, 4,
            2, 4, 5,
            1, 2, 5,
            1, 5, 6,
            0, 7, 4,
            0, 4, 3,
            5, 4, 7,
            5, 7, 6,
            0, 6, 7,
            0, 1, 6
        };

        var mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.hideFlags = HideFlags.HideAndDontSave;
        return mesh;
    }

    void OnDestroy()
    {
        Destroy(material);
    }
}
