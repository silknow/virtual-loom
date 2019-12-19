

using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

[RequireComponent(typeof(Button))]
public class SaveScreenshotDialog : MonoBehaviour, IPointerDownHandler {

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // Browser plugin should be called in OnPointerDown.
    public void OnPointerDown(PointerEventData eventData) {
       
        StartCoroutine(nameof(RecordFrameWebGL));
           

       
    }
    IEnumerator RecordFrameWebGL()
    {
        yield return new WaitForEndOfFrame();
       
        var w = Screen.width;
        var h = Screen.height;
        
        Rect rect = new Rect(0, 0, w, h);
        RenderTexture renderTexture = new RenderTexture(w, h, 24);
        Texture2D texture = new Texture2D(w, h, TextureFormat.RGBA32, false);
 
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();
 
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
 
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // do something with texture
        var byteArray = texture.EncodeToJPG();
        DownloadFile(gameObject.name, "OnFileDownload", "screenshot.jpg", byteArray, byteArray.Length);
        Object.Destroy(texture);
    }

    // Called from browser
    public void OnFileDownload() {
        //output.text = "File Successfully Downloaded";
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    // Listen OnClick event in standlone builds
    void Start() {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {

        StartCoroutine(nameof(RecordFrame));

    }
    IEnumerator RecordFrame()
    {
        yield return new WaitForEndOfFrame();

        var w = Screen.width;
        var h = Screen.height;
        
        Rect rect = new Rect(0, 0, w, h);
        RenderTexture renderTexture = new RenderTexture(w, h, 24);
        Texture2D texture = new Texture2D(w, h, TextureFormat.RGBA32, false);
 
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();
 
        RenderTexture.active = renderTexture;
        texture.ReadPixels(rect, 0, 0);
 
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        
        // do something with texture
        var path = StandaloneFileBrowser.SaveFilePanel("Save Image as", "", "screenshot", "png");
        if (!string.IsNullOrEmpty(path)) {
            File.WriteAllBytes(path,texture.EncodeToPNG());
        }
        Object.Destroy(texture);
    }

#endif
}