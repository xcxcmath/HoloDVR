using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DataMapAssetBuilder : EditorWindow
{
    [MenuItem("Window/DataMapAssetBuilder")]
    static void Init()
    {
        var window = EditorWindow.GetWindow(typeof(DataMapAssetBuilder));
        window.Show();
    }

    private string inputPath, outputPath;
    private int width = 256, height = 256;
    private Object asset;

    void OnEnable()
    {
        inputPath = "Assets/DataMap.raw";
        outputPath = "Assets/DataMap.asset";
    }

    void OnGUI()
    {
        const float headerSize = 120F;
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Input pvm raw file path", GUILayout.Width(headerSize));
            asset = EditorGUILayout.ObjectField(asset, typeof(Object), true);
            inputPath = AssetDatabase.GetAssetPath(asset);
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Width", GUILayout.Width(headerSize));
            width = EditorGUILayout.IntField(width);
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Height", GUILayout.Width(headerSize));
            height = EditorGUILayout.IntField(height);
        }
        
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Label("Output path", GUILayout.Width(headerSize));
            outputPath = EditorGUILayout.TextField(outputPath);
        }

        if (GUILayout.Button("Build"))
        {
            Build(inputPath, outputPath, width, height);
        }
    }

    void Build(string inputPath, string outputPath, int width, int height)
    {
        if (!File.Exists(inputPath))
        {
            Debug.LogWarning(inputPath + " is not exist.");
            return;
        }

        var datamap = Build(inputPath, width, height);
        AssetDatabase.CreateAsset(datamap, outputPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static Texture2D Build(string path, int width, int height)
    {
        var max = width * height * 4;
        var tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        tex.anisoLevel = 0;

        using (var stream = new FileStream(path, FileMode.Open))
        {
            var len = stream.Length;
            if (len != max)
            {
                Debug.LogWarning(path + " doesn't have required resolution");
            }

            int i = 0, j = 0;
            Color[] colors = new Color[max];
            float[] bytes = new float[4];
            float inv = 1f / 255.0f;
            for (i = 0; i < max/4; ++i)
            {
                for (j = 0; j < 4; ++j)
                {
                    int v = stream.ReadByte();
                    bytes[j] = v * inv;
                }
                colors[i] = new Color(bytes[0], bytes[1], bytes[2], bytes[3]);
            }
            tex.SetPixels(colors);
            tex.Apply();
        }

        return tex;
    }
}
