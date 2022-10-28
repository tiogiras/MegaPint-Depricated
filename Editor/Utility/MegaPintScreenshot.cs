using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace MegaPint.Editor.Utility {
    public class MegaPintScreenshot {

        public static Camera RenderCamera;
        public static string FileName;

        public static MegaPintScreenshotResolutions CurrentResolution;
        public enum MegaPintScreenshotResolutions {
            HD, FullHD, WQHD, UHD1, UHD2
        }

        private static readonly Resolution[] Resolutions = {
            new(){width = 1280, height = 720},
            new(){width = 1920, height = 1080},
            new(){width = 2560, height = 1440},
            new(){width = 3840, height = 2160},
            new(){width = 7680, height = 4320},
        };

        #region RenderTool Methods

        private void RenderImage( ) {
            if ( MegaPint.Settings.screenshotSavePath.Equals( "" ) || FileName.Equals( "" ) || RenderCamera == null ) return;
            if ( MegaPint.Settings.screenshotStrengthNormal == 0 ) return;
        
            var normal = Render(GraphicsFormat.R8G8B8A8_SRGB, GraphicsFormat.D32_SFloat_S8_UInt);
            var glow = Render(GraphicsFormat.R32G32B32A32_SFloat, GraphicsFormat.D32_SFloat_S8_UInt);
        
            var normalTexture = ConvertToTexture(normal);
            var glowTexture = ConvertToTexture(glow);
                    
            var bytes = MixImages(normalTexture, glowTexture).EncodeToPNG();
            File.WriteAllBytes(MegaPint.Settings.screenshotSavePath + "/" + MegaPint.Settings.screenshotStrengthNormal + ".png", bytes);
            AssetDatabase.Refresh();
            
            //DestroyImmediate(normal);
            //DestroyImmediate(glow);
        }
        
        private RenderTexture Render(GraphicsFormat format, GraphicsFormat depth) {
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
        
        private Texture2D MixImages(Texture2D basic, Texture2D glow) {
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