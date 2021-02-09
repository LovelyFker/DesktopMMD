using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoadAssetBundles
{
    /// <summary>
    /// 用于request.progress获取资源读取的进度
    /// </summary>
    public static AssetBundleCreateRequest request { get; private set; }
    private static AssetBundle assetBundle { get; set; }

    public enum LoadType
    {
        /// <summary>
        /// 异步加载
        /// </summary>
        Async = 0,
        /// <summary>
        /// 同步加载
        /// </summary>
        Sync = 1
    }

    /// <summary>
    /// 读取AssetsBundle资源
    /// </summary>
    /// <param name="path">资源名称路径</param>
    /// <param name="type">读取方式</param>
    /// <param name="postion">父级对象</param>
    /// <returns></returns>
    public static IEnumerator Load(string path, LoadType type, Transform postion)
    {
        path = path.Insert(0, "AssetBundles/");

        //先卸载之前加载的assetbundle
        if (assetBundle != null)
        {
            Unload();
        }

        switch (type)
        {
            case LoadType.Async:
                request = AssetBundle.LoadFromFileAsync(path);//异步加载
                yield return request;
                assetBundle = request.assetBundle;
                break;
            case LoadType.Sync:
                assetBundle = AssetBundle.LoadFromFile(path);//同步加载
                break;
            default:
                assetBundle = AssetBundle.LoadFromFile(path);//默认同步加载
                break;
        }

        //使用里面的资源
        Object[] obj = assetBundle.LoadAllAssets<GameObject>();//加载出来放入数组中

        foreach(GameObject go in obj)
        {
            GameObject.Instantiate(go, postion);
        }
    }

    /// <summary>
    /// 卸载AssetBundles资源
    /// </summary>
    public static void Unload()
    {
        if (assetBundle)
            assetBundle.Unload(true);
    }

    public static bool IsLoading
    {
        get {
            if (request != null)
            {
                if (request.progress > 0 && request.progress < 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
