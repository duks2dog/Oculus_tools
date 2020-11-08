namespace OpenCvSharp
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class WebCamController : MonoBehaviour {

        Texture2D cap_tex;
        Texture2D out_tex;
        WebCamTexture webcamTex;
        int webcamTexwh = webcamTex.width * webcamTex.height;
        int width = webcamTex.width;
        int height = webcamTex.height;
        int width = 1920;
        int height = 1080;
        int fps = 30;
        Color32[] colors = null;
        // WebカメラをInputとして扱う準備
        // 画像の分のメモリを確保
        IEnumerator Init() {
            while (true) {
                if (webcamTex.width > 16 && webcamTex.height > 16) {
                    colors = new Color32[webcamTexwh];
                    cap_tex = new Texture2D(webcamTex.width, webcamTex.height, TextureFormat.RGBA32, false);
                    break;
                }
                yield return null;
            }
        }
        // テクスチャとして利用できるようにする
        void Start() {
            // WebcamをInputとして受け取る
            WebCamDevice[] devices = WebCamTexture.devices;
            webcamTex = new WebCamTexture(devices[0].name, this.width, this.height, this.fps);
            webcamTex.Play();

            StartCoroutine(Init());
        }
        
        void Update() {
            // 全ピクセルを走査しする2重ループ
            if (colors != null) {
                webcamTex.GetPixels32(colors);
                Color32 rc = new Color32(0, 0, 0, byte.MaxValue);
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        Color32 c = colors[x + y * width];
                        // 1ピクセルごとにグレースケール化
                        byte gray = (byte)(0.1f * c.r + 0.7f * c.g + 0.2f * c.b);
                        rc.r = rc.g = rc.b = gray;
                        colors[x + y * width] = rc;
                    }
                }
                cap_tex.SetPixels32(this.colors);
                cap_tex.Apply();
            }
            // 2値化して出力
            out_tex = To_Mono(cap_tex);
            GetComponent<RawImage>().texture = out_tex;
        }

        Texture2D To_Mono(Texture2D tex) {
            Mat mat;
            //textureをmatに変換
            mat = Unity.TextureToMat(tex);
            //画像をグレスケに変換
            Mat gray = mat.CvtColor(ColorConversionCodes.BGR2GRAY);
            //閾値を指定して、超えたら白となるように二値化
            //INPUTはグレスケ画像である必要がある
            Mat mono = gray.Threshold(127, 255, ThresholdTypes.Otsu);
            //二値化を白黒反転
            Cv2.BitwiseNot(mono, mono);
            tex = Unity.MatToTexture(mono);
            return tex;
        }
    }
}
