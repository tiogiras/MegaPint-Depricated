using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Object = UnityEngine.Object;

namespace MegaPint.Editor.Utility {
    public class MegaPintScreenshot {

        public static Texture2D PreviewTexture;
        public static Texture2D WindowPreviewTexture;
        public static Camera RenderCamera;
        public static string FileName;
        public static string WindowName;

        public static MegaPintScreenshotResolutions CurrentResolution;
        public enum MegaPintScreenshotResolutions {
            HD, FullHD, WQHD, UHD1, UHD2
        }

        public static MegaPintTargetWindows CurrentWindow;
        public enum MegaPintTargetWindows {
            SceneView, WindowByName
        }

        private static readonly Resolution[] Resolutions = {
            new(){width = 1280, height = 720},
            new(){width = 1920, height = 1080},
            new(){width = 2560, height = 1440},
            new(){width = 3840, height = 2160},
            new(){width = 7680, height = 4320},
        };

        #region RenderTool Methods

        public static void RenderPreview() {
            var normal = Render(GraphicsFormat.R8G8B8A8_SRGB, GraphicsFormat.D32_SFloat_S8_UInt);
            var glow = Render(GraphicsFormat.R32G32B32A32_SFloat, GraphicsFormat.D32_SFloat_S8_UInt);
        
            var normalTexture = ConvertToTexture(normal);
            var glowTexture = ConvertToTexture(glow);
                    
            var bytes = MixImages(normalTexture, glowTexture).EncodeToPNG();

            if (PreviewTexture == null) PreviewTexture = new Texture2D(normal.width, normal.height);
            PreviewTexture.LoadImage(bytes);
            PreviewTexture.Apply();
            
            Object.DestroyImmediate(normalTexture);
            Object.DestroyImmediate(glowTexture);
            
            Object.DestroyImmediate(normal);
            Object.DestroyImmediate(glow);
        }
        
        public static void RenderImage( ) {
            if ( MegaPint.Settings.screenshotSavePath.Equals( "" ) || FileName.Equals( "" ) || RenderCamera == null ) return;
            if ( MegaPint.Settings.screenshotStrengthNormal == 0 ) return;
        
            var normal = Render(GraphicsFormat.R8G8B8A8_SRGB, GraphicsFormat.D32_SFloat_S8_UInt);
            var glow = Render(GraphicsFormat.R32G32B32A32_SFloat, GraphicsFormat.D32_SFloat_S8_UInt);

            var normalTexture = ConvertToTexture(normal);
            var glowTexture = ConvertToTexture(glow);

            var bytes = MixImages(normalTexture, glowTexture).EncodeToPNG();
            File.WriteAllBytes(MegaPint.GetApplicationPath() + MegaPint.Settings.screenshotSavePath + "/" + FileName + ".png", bytes);
            AssetDatabase.Refresh();
            
            Object.DestroyImmediate(normalTexture);
            Object.DestroyImmediate(glowTexture);
            
            Object.DestroyImmediate(normal);
            Object.DestroyImmediate(glow);
        }

        public static void RenderWindowPreview() {
            while (true) {
                EditorWindow activeWindow = null;
                switch (CurrentWindow) {
                    case MegaPintTargetWindows.SceneView:
                        activeWindow = EditorWindow.GetWindow<SceneView>();
                        break;
                    case MegaPintTargetWindows.WindowByName:
                        var allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
                        foreach (var window in allWindows) {
                            if (window.titleContent.text.Equals(WindowName)) activeWindow = window;
                        }
                        break;
                    default: return;
                }

                if (activeWindow == null) return;
                if (!activeWindow.hasFocus) {
                    activeWindow.Focus();
                    continue;
                }

                var position = activeWindow.position.position;
                var sizeX = activeWindow.position.width;
                var sizeY = activeWindow.position.height;

                var colors = InternalEditorUtility.ReadScreenPixel(position, (int)sizeX, (int)sizeY);

                var result = new Texture2D((int)sizeX, (int)sizeY);
                result.SetPixels(colors);

                var bytes = result.EncodeToPNG();

                if (WindowPreviewTexture == null) WindowPreviewTexture = new Texture2D(result.width, result.height);
                WindowPreviewTexture.LoadImage(bytes);
                WindowPreviewTexture.Apply();
            
                Object.DestroyImmediate(result);
                break;
            }
        }

        public static void RenderWindowImage() {
            while (true) {
                EditorWindow activeWindow = null;
                switch (CurrentWindow) {
                    case MegaPintTargetWindows.SceneView:
                        activeWindow = EditorWindow.GetWindow<SceneView>();
                        break;
                    case MegaPintTargetWindows.WindowByName:
                        var allWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
                        foreach (var window in allWindows) {
                            if (window.titleContent.text.Equals(WindowName)) activeWindow = window;
                        }
                        break;
                    default: return;
                }

                if (activeWindow == null) return;
                if (!activeWindow.hasFocus) {
                    activeWindow.Focus();
                    continue;
                }

                var position = activeWindow.position.position;
                var sizeX = activeWindow.position.width;
                var sizeY = activeWindow.position.height;

                var colors = InternalEditorUtility.ReadScreenPixel(position, (int)sizeX, (int)sizeY);

                var result = new Texture2D((int)sizeX, (int)sizeY);
                result.SetPixels(colors);

                var bytes = result.EncodeToPNG();

                File.WriteAllBytes(MegaPint.GetApplicationPath() + MegaPint.Settings.screenshotSavePath + "/" + FileName + ".png", bytes);
                AssetDatabase.Refresh();

                Object.DestroyImmediate(result);
                break;
            }
        }

        private static RenderTexture Render(GraphicsFormat format, GraphicsFormat depth) {
            var resIndex = (int)CurrentResolution;
            var result = new RenderTexture(Resolutions[resIndex].width, Resolutions[resIndex].height, format, depth);
            RenderCamera.targetTexture = result;
            RenderCamera.Render();
            RenderCamera.targetTexture = null;
            return result;
        }
                
        private static Texture2D ConvertToTexture(RenderTexture target) {
            var image = new Texture2D(target.width, target.height);
            RenderTexture.active = target;
            image.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
            image.Apply( );
            RenderTexture.active = null;
            return image;
        }
        
        private static Texture2D MixImages(Texture2D basic, Texture2D glow) {
            var result = new Texture2D(basic.width, basic.height);
            for ( var i = 0; i < basic.width; i++ ) {
                for ( var j = 0; j < basic.height; j++ ) {
                    var basicColor = basic.GetPixel( i, j );
                    var glowColor = glow.GetPixel( i, j );
                    result.SetPixel(i, j, basicColor * MegaPint.Settings.screenshotStrengthNormal + glowColor * MegaPint.Settings.screenshotStrengthGlow);
                }
            }
            return result;
        }

        #endregion
    }
}