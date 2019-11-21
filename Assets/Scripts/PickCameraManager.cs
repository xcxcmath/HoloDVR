using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickCameraManager : MonoBehaviour
{
    public GameObject pickObject;
    private Camera pickCamera;

    private void OnEnable()
    {
        Camera.onPreRender += MyPreRender;
    }
    private void OnDisable()
    {
        Camera.onPreRender -= MyPreRender;
    }

    private void Start()
    {
        pickCamera = pickObject.GetComponent<Camera>();
    }

    void MyPreRender(Camera cam)
    {
        GetComponent<MeshRenderer>().sharedMaterial.SetInt("_PickMode", cam == pickCamera ? 1 : 0);
        if(cam == pickCamera)
        {
            Debug.Log("yes");
        } else
        {
            Debug.Log("no");
        }
    }
}
