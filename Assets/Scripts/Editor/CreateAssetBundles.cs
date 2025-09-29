using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/*
    간단한 에셋번들 생성코드 
    https://github.com/ozlael/SpriteAtlasSample/blob/master/Assets/Editor/CreateAssetBundles.cs
*/

public class CreateAssetBundles
{
    public static string assetBundleDirectory = "Assets/StreamingAssets/AssetBundles";

    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        //If your OS is Windows, use BuildTarget.StandaloneWindows;
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.Android);
    }
}
