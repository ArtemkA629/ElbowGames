using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Utils
{
    #if UNITY_EDITOR
    public class ScreenShot : MonoBehaviour
    {
        private const int MaxScreenshotAmount = 15;
        private const string ScreenshotsDirectory = "StoreInfo/Screenshots";

        [MenuItem("Tools/Take Screenshot")]
        public static void TakeScreenshot()
        {
            var width = Screen.width;
            var height = Screen.height;

            for (var idx = 1; idx <= MaxScreenshotAmount; idx++)
            {
                var filename = $"Screenshot_{width}x{height}_{idx}.png";
                var path = Path.Combine(ScreenshotsDirectory, filename);

                if (File.Exists(path)) continue;
                
                ScreenCapture.CaptureScreenshot(path);
                break;
            }
        }
    }
    #endif
}