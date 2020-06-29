using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ThresholdImage : MonoBehaviour
{
    public RectTransform[] ThresholdPoints;
    public RectTransform ThresholdSpace;
    public Material ThresholdMaterial;

    private Color[] thresholdColors;
    public Texture2D result = null;
    public Texture2D originalImage = null;

    public float Scale = 1f;

    private class ThresholdSample {
        public int id;
        public Vector3 point;
        public Color color;
        public Color userColor;
        public bool active;

        public ThresholdSample() {
            id = -1;
            point = Vector3.zero;
            color = userColor = Color.white;
            active = false;
        }
    }
    private List<ThresholdSample> samples;

    private void Start() {
        samples = new List<ThresholdSample>();
    }

    public void setImage(Texture2D texture)
    {
        originalImage = texture;
        GetComponent<RawImage>().texture = originalImage;
    }

    Color getColor(Vector3 pos) {
        if (originalImage == null)
            originalImage = GetComponent<UnityEngine.UI.RawImage>().texture as Texture2D;

        return originalImage.GetPixel(
            (int)(pos.x * originalImage.width),
            (int)(pos.y * originalImage.height)
            );
    }

    Color getColor(RectTransform point, RectTransform space, Texture2D image) {

        Vector3 spritePoint = new Vector3(
                point.localPosition.x * image.width / space.rect.width,
                point.localPosition.y * image.height / space.rect.height,
                0);

        return image.GetPixel((int)spritePoint.x, (int)spritePoint.y);
    }

    void InitTexture() {
        if (originalImage == null)
            originalImage = GetComponent<UnityEngine.UI.RawImage>().texture as Texture2D;

        int width = (int)(originalImage.width * Scale);
        int height = (int)(originalImage.height * Scale);

        if ((result == null) || (result.width != width) || (result.height != height)) {
            result = new Texture2D(width, height, TextureFormat.RGB24, false, false);
            
        }
    }

    public void setPoint(int id, float x, float y) {
        foreach (ThresholdSample ts in samples) {
            if (ts.id == id) {
                ts.point = new Vector3(x, y, 0);
                ts.color = getColor(ts.point);
                Debug.Log("Point " + " " + id + " "+ x + " " + y + " " + ts.color);
                return;
            }
        }

        ThresholdSample ts2 = new ThresholdSample();
        ts2.id = id;
        ts2.point = new Vector3(x, y, 0);
        ts2.color = getColor(ts2.point);
        Debug.Log("Add point " + " " + id + " "+ x + " " + y + " " + ts2.color);
        samples.Add(ts2);
    }

    public void setUserColor(int id, float r, float g, float b) {
        foreach (ThresholdSample ts in samples) {
            if (ts.id == id) {
                ts.userColor = new Color(r, g, b);
                return;
            }
        }

        ThresholdSample ts2 = new ThresholdSample();
        ts2.id = id;
        ts2.userColor = new Color(r, g, b);
        samples.Add(ts2);
    }

    public void removeSample(int id) {
        foreach (ThresholdSample ts in samples) {
            if (ts.id == id) {
                samples.Remove(ts);
                return;
            }
        }
    }

    public void UpdateThreshold()
    {
        UpdateThreshold(0.1f);
    }

    public void UpdateThreshold(float sc)
    {
        Scale = sc;
        // Update samples with the point in the vector
        if (ThresholdPoints.Length > 0) {
            for (int i = 0; i < ThresholdPoints.Length; i++) {
                samples[i].point = new Vector3(
                    ThresholdPoints[i].localPosition.x / ThresholdSpace.rect.width,
                    ThresholdPoints[i].localPosition.y / ThresholdSpace.rect.height,
                    0);

                samples[i].active = true;
                samples[i].userColor = getColor(samples[i].point); 
            }
            for (int i = ThresholdPoints.Length; i < samples.Count; i++)
                samples[i].active = false;
        }

        InitTexture();

        int width = (int)(originalImage.width * Scale);
        int height = (int)(originalImage.height * Scale);
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++) {
                Color c = originalImage.GetPixel((int)(x / Scale),(int)(y / Scale));

                // Compute distance to each color
                float d = float.MaxValue;
                ThresholdSample tsmin = null;
                foreach (ThresholdSample ts in samples) {
                    float dref = (new Vector3(c.r,c.g,c.b) - new Vector3(ts.color.r, ts.color.g, ts.color.b)).sqrMagnitude;
                    if (dref < d) {
                        tsmin = ts;
                        d = dref;
                    }
                }

                if (tsmin != null)
                    result.SetPixel(x, y, tsmin.userColor);
                else
                    result.SetPixel(x, y, c);
            }

        result.Apply();

        GetComponent<UnityEngine.UI.RawImage>().texture = result;
    }

    public void RenderThreshold()
    {
        UpdateThreshold(1.0f);
//        byte[] resultBytes = thresholdImage.EncodeToPNG();
//        File.WriteAllBytes(Application.streamingAssetsPath + "/thresholdImage.png", resultBytes);
    }
}
