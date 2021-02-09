using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProgress : MonoBehaviour
{
    public GameObject mUIProgress;
    public Scrollbar mScrollbar;
    public Text mText;

    private void OnEnable()
    {
        mUIProgress.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (LoadAssetBundles.IsLoading)
        {
            mUIProgress.SetActive(true);
            mScrollbar.size = LoadAssetBundles.request.progress;
            mText.text = LoadAssetBundles.request.progress.ToString("0%");
        }
        else
        {
            mUIProgress.SetActive(false);
        }
    }
}
