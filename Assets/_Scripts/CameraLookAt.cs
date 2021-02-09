using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    public Transform CharacterPostion;
    private GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            target = CharacterPostion.GetChild(0).gameObject;
        }
        catch
        {

        }

        if (target)
        {
            //相机x轴平滑跟随角色
            this.transform.position = Vector3.Lerp(this.transform.position,
                                            new Vector3(target.transform.position.x, this.transform.position.y, this.transform.position.z),
                                            0.01f);
        }
    }
}
