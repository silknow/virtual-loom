

using System.Collections;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using GLTF;
using UnityGLTF;


[RequireComponent(typeof(Button))]
public class SaveGLTFDialog : MonoBehaviour, IPointerDownHandler {
    // Sample text data
    private string _data = "Example text created by StandaloneFileBrowser";

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // Browser plugin should be called in OnPointerDown.
    public void OnPointerDown(PointerEventData eventData) {
        WizardController.instance.ToggleProcessingWindow();
        
        StartCoroutine(SaveWebGLTF(0.5f));
       
    }
    IEnumerator SaveWebGLTF(float delayTime)
    {
        var path = Application.persistentDataPath + "/textile.glb";
        if (!string.IsNullOrEmpty(path))
        {
            GameObject go = Instantiate(WizardController.instance.instantiatedPatch.gameObject);
            CambiarMaterialesaShader(go,"Standard");
            var exporter = new GLTFSceneExporter(new[] {go.transform}, new ExportOptions());
            exporter.SaveGLB(Path.GetDirectoryName(path),Path.GetFileName(path));
            Destroy(go);
            if (File.Exists(path))
            {
                byte[] byteArray = File.ReadAllBytes(path);
                DownloadFile(gameObject.name, "OnFileDownload", Path.GetFileName(path), byteArray, byteArray.Length);
            }
        }

        WizardController.instance.ToggleProcessingWindow();
        yield return null;
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

    public void OnClick() {
        
        var path = StandaloneFileBrowser.SaveFilePanel("Save GLTF as", "", "GLTFSample", "glb");
        if (!string.IsNullOrEmpty(path)) {
            WizardController.instance.ToggleProcessingWindow();
            StartCoroutine(SaveLocalGLTF(path,0.5f));

        }
    }

    protected void ExportScene (string fileName)
    {
        GameObject go = Instantiate(WizardController.instance.instantiatedPatch.gameObject);
        //GLTFExporter.ExportGameObjToGLTF(WizardController.instance.instantiatedPatch.gameObject, fileName, true, false);
        CambiarMaterialesaShader(go,"Standard");
        var exporter = new GLTFSceneExporter(new[] {go.transform}, new ExportOptions());
        exporter.SaveGLB(Path.GetDirectoryName(fileName),Path.GetFileName(fileName));
        Destroy(go);
    }

    

    IEnumerator SaveLocalGLTF(string path, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        ExportScene(path);
        WizardController.instance.ToggleProcessingWindow();
    }
#endif
    private void CambiarMaterialesaShader(GameObject go, string shaderName)
    {
        Shader s = Shader.Find(shaderName);

        if (!s)
        {
            Debug.Log("No Shader Found");
            return;
        }
        
        MeshRenderer[] renderers = go.GetComponentsInChildren<MeshRenderer>();
        foreach (var r in renderers)
        {
            r.material.shader = s;
        }
    }
}