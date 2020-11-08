namespace OpenCvSharp
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class WebCamController : MonoBehaviour
    {

        int width = 1920;
        int height = 1080;
        int fps = 30;
        Texture2D cap_tex;
        Texture2D out_tex;
        WebCamTexture webcamTex;
        Color32[] colors = null;

        IEnumerator Init()
        {
            while (true)
            {
                if (webcamTex.width > 16 && webcamTex.height > 16)
                {
                    colors = new Color32[webcamTex.width * webcamTex.height];
                    cap_tex = new Texture2D(webcamTex.width, webcamTex.height, TextureFormat.RGBA32, false);
                    //GetComponent<Renderer>().material.mainTexture = texture;
                    break;
                }
                yield return null;
            }
        }
        void Start()
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            webcamTex = new WebCamTexture(devices[0].name, this.width, this.height, this.fps);
            webcamTex.Play();

            StartCoroutine(Init());
        }
        void Update()
        {
            if (colors != null)
            {
                webcamTex.GetPixels32(colors);

                int width = webcamTex.width;
                int height = webcamTex.height;
                Color32 rc = new Color32(0, 0, 0, byte.MaxValue);

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Color32 c = colors[x + y * width];
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

        Texture2D To_Mono(Texture2D tex)
        {
            //matの定義
            Mat mat;

            //textureをmatに変換
            mat = Unity.TextureToMat(tex);

            //画像をグレスケに変換
            Mat matGray = mat.CvtColor(ColorConversionCodes.BGR2GRAY);

            //画像を2値化
            Mat matMono = matGray.Threshold(100, 255, ThresholdTypes.Otsu);

            //2値化画像を白黒反転
            Cv2.BitwiseNot(matMono, matMono);

            //matMonoをtexture2Dに変換
            tex = Unity.MatToTexture(matMono);

            return tex;
        }
    }
}
