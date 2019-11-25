using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickCameraManager : MonoBehaviour
{
    public GameObject volume;
    public bool isPickMode;

    private void OnPreRender()
    {
        volume.GetComponent<MeshRenderer>().sharedMaterial.SetInt("_PickMode", isPickMode ? 1 : 0);
    }
}
