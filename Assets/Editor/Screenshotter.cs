using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

/*
	AUTHOR: Stijn Raaijmakers @bugshake
	
	Custom editor window which will save a screenshot in the play windows resolution.
	For example if you have a 1080p screen, but want a 4k screenshot, just set that resolution in the Game window
	and the file will be a full 3840x2160 pixels
*/
public class Screenshotter : EditorWindow
{
    List<string> fileHistory = new List<string>();
    Vector2 scrollPos = Vector2.zero;

    [MenuItem("Tools/Screenshotter...")]
    public static void ShowWindow()
    {
        var win = EditorWindow.GetWindow<Screenshotter>(false, "Screenshotter");
    }

    void OnGUI()
    {
        string screenshotPath = string.Format("{0}/Screenshots", Application.persistentDataPath);
        if (GUILayout.Button("Capture"))
        {
            Directory.CreateDirectory(screenshotPath);
            string path = string.Format("{0}/s_{1:yyyy_MM_dd hh_mm_ss}.png", screenshotPath, DateTime.Now);
            ScreenCapture.CaptureScreenshot(path);
            fileHistory.Add(path);
        }
        if (GUILayout.Button("SCREENSHOTS:"))
        {
            Directory.CreateDirectory(screenshotPath);
            EditorUtility.RevealInFinder(screenshotPath + "/");
        }
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(100f));
        for( int i = fileHistory.Count; i-- > 0;)
        {
            GUILayout.Label(fileHistory[i]);
        }
        GUILayout.EndScrollView();
    }
}