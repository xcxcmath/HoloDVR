//Copyright 2017-2019 Looking Glass Factory Inc.
//All rights reserved.
//Unauthorized copying or distribution of this file, and the source code contained herein, is strictly prohibited.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

namespace LookingGlass.Demos {
    public class DemoBlurUnblur : MonoBehaviour {
        public PostProcessVolume postProcessingVolume;
        public UnityEngine.UI.Text label;
        public bool enablePostProcessing;

        void Update() {
            if (enablePostProcessing) {
                label.text = "Post Processing";
                postProcessingVolume.enabled = true;
            } else {
                label.text = "No Post Processing";
                postProcessingVolume.enabled = false;
            }
        }
    }
}
