using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    private static AudioManager mInstance;
    /// <summary>
    /// BGM播放源
    /// </summary>
    private AudioSource mAudioSource;

    public static AudioManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                GameObject root = new GameObject("_AudioManager");
                mInstance = root.AddComponent<AudioManager>();
                mInstance.mAudioSource = root.AddComponent<AudioSource>();
            }

            return mInstance;
        }
    }

    /// <summary>
    /// 构造函数私有化，防止其他类通过new操作产生新的实例
    /// </summary>
    private AudioManager()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 一次性播放声音
    /// </summary>
    /// <param name="source">声音源</param>
    /// <param name="audioClip">声音文件</param>
    public void PlayAudioClipOneShot(AudioSource source, AudioClip audioClip)
    {
        source.PlayOneShot(audioClip);
    }
}
