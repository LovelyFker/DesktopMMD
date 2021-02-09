using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectButton : MonoBehaviour
{
    public Transform CharacterPostion;
    [Header("初音")]
    public GameObject SignBoardMiku;
    [Header("莫娜")]
    public GameObject Mona;
    private IEnumerator load;

    // Start is called before the first frame update
    void Start()
    {
        //LoadCharacterFromAssetBundles(SignBoardMiku, LoadAssetBundles.LoadType.Async);
        //LoadCharacterFromAssetBundles(Mona, LoadAssetBundles.LoadType.Async);
        GameObject.Instantiate(SignBoardMiku, CharacterPostion);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowCharacter(GameObject character)
    {
        string name = CharacterPostion.GetChild(0).gameObject.name;
        //-7是因为克隆体带有(clone)字符串
        if (!name.Substring(0, name.Length - 7).Equals(character.name))
        {
            GameObject.DestroyImmediate(CharacterPostion.GetChild(0).gameObject);
            GameObject.Instantiate(character, CharacterPostion);
        }
    }

    /// <summary>
    /// 从打包的AssetsBundle资源中读取角色模型
    /// </summary>
    private void LoadCharacterFromAssetBundles(GameObject character, LoadAssetBundles.LoadType loadType)
    {
        string path = "model." + character.name;
        load = LoadAssetBundles.Load(path, loadType, CharacterPostion);
        StartCoroutine(load);
    }

    private void UnloadAssetBundles()
    {
        LoadAssetBundles.Unload();
    }

    private void OnDestroy()
    {
        UnloadAssetBundles();
    }
}
