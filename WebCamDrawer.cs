using UnityEngine;
using System.Collections;

public class WebCamDrawer : MonoBehaviour {
    public string deviceNameKeyword = "THETA";

    void Start() {
        StartStreaming();
    }

    void StartStreaming() {
        WebCamDevice device = new WebCamDevice();
        // 参照渡しでdeviceを書き換え
        if (!FindDevice(ref device)) {
            Debug.LogError("<" + deviceNameKeyword + ">を含むWebカメラが検出できませんでした。");
            return;
        }
        WebCamTexture webcamTexture = new WebCamTexture(device.name);
        Material mat = GetTargetMaterial();
        if (mat == null) {
            return;
        }
        mat.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    bool FindDevice(ref WebCamDevice target) {
        bool deviceExists = false;
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (WebCamDevice device in devices) {
            Debug.Log("device.name => " + device.name);
            if (device.name.Contains(deviceNameKeyword)) {
                target = device;
                deviceExists = true;
            }
        }
        return deviceExists;
    }

    Material GetTargetMaterial() {
        Skybox skybox = GetComponent<Skybox>();
        if (skybox != null) {
            return skybox.material;
        }
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null) {
            return renderer.material;
        }
        Debug.LogError("Renderer/Skyboxコンポーネントがありません。");
        return null;
    }
}
