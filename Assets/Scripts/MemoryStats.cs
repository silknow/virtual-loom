using System.Text;
using Unity.Profiling;
using UnityEngine;

public class MemoryStats : MonoBehaviour
{
    public static long weaveDuration=0;
    string statsText;
    ProfilerRecorder meshMemoryRecorder;
    ProfilerRecorder textureMemoryRecorder;
    ProfilerRecorder systemUsedMemoryRecorder;
    ProfilerRecorder mainThreadTimeRecorder;
    private ProfilerRecorder VerticesRecorder;

    void OnEnable()
    {
        meshMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Mesh Memory");
        textureMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Texture Memory");
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 30);
        VerticesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render,"Vertices Count");
    }

    void OnDisable()
    {
        meshMemoryRecorder.Dispose();
        textureMemoryRecorder.Dispose();
        systemUsedMemoryRecorder.Dispose();
        VerticesRecorder.Dispose();
    }

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;
        unsafe
        {
            var samples = stackalloc ProfilerRecorderSample[samplesCount];
            recorder.CopyTo(samples, samplesCount);
            for (var i = 0; i < samplesCount; ++i)
                r += samples[i].Value;
            r /= samplesCount;
        }

        r *= (1e-6f);
        return r;
    }
    void Update()
    {
        var sb = new StringBuilder(500);
        sb.AppendLine($"Weave duration: {weaveDuration} ms");
        sb.AppendLine($"Frames per Second: {1000.0f / (GetRecorderFrameAverage(mainThreadTimeRecorder) ):F0} fps");
        sb.AppendLine($"Mesh Memory: {meshMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"Texture Memory: {textureMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"Vertices: {VerticesRecorder.LastValue}");
        statsText = sb.ToString();
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(Screen.width-260, Screen.height-120, 250, 90), statsText);
    }


}