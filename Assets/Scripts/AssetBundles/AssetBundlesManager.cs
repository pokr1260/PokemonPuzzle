using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AssetBundlesManager : Singleton<AssetBundlesManager>
{
    [SerializeField]
    public string assetBundlePath;

    private string beforeAssetBundlesName = "";


    private static WWW loader;


    private Dictionary<string, AssetBundle> assetBundleList = new Dictionary<string, AssetBundle>();

    
    public object LoadAssetBundles(string assetBundlesName, string fileName)
    {
        string path = "file://" + Application.streamingAssetsPath + "/" + assetBundlePath + "/" + assetBundlesName;

        if (null == loader || 0 != beforeAssetBundlesName.CompareTo(assetBundlesName))
        {
            beforeAssetBundlesName = assetBundlesName;
            
            loader = new WWW(path);
        }

        if (null == loader.assetBundle)
        {
            Debug.LogError("loader.assetBundle is null / assetBundlesName : " + assetBundlesName + " , fileName : " + fileName);
            return null;
        }

        var loadAsset = loader.assetBundle.LoadAsset(fileName);

        if (null == loadAsset)
        {
            Debug.LogError("LoadAsset is Fail / assetBundlesName : " + assetBundlesName + " , fileName : " + fileName);
            return null;
        }

        return loadAsset;
    }

    public object LoadAssetBundlesAtLocal(string assetBundlesName, string fileName)
    {
        if (false == assetBundleList.ContainsKey(assetBundlesName))
        {
            string path = Path.Combine(Path.Combine(Application.streamingAssetsPath, assetBundlePath), assetBundlesName);

            var loadedAssetBundle = AssetBundle.LoadFromFile(path);

            if (null == loadedAssetBundle)
            {
                Debug.LogError("loader.assetBundle is null / fullPath : "+ path + ", assetBundlesName : " + assetBundlesName + " , fileName : " + fileName);
                return null;
            }

            assetBundleList.Add(assetBundlesName, loadedAssetBundle);
        }

        AssetBundle assetBundle = null;
        assetBundleList.TryGetValue(assetBundlesName, out assetBundle);

        if (null == assetBundle)
        {
            Debug.LogError("assetBundleList에 " + assetBundlesName + " 키에 해당되는것이 없다.");
            return null;
        }

        var loadAsset = assetBundle.LoadAsset(fileName);
        if (null == loadAsset)
        {
            Debug.LogError("LoadAsset is Fail / assetBundlesName : " + assetBundlesName + " , fileName : " + fileName);
            return null;
        }

        return loadAsset;
    }

    


}
