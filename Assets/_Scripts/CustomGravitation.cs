using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravitation : MonoBehaviour
{
    private float offset = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = this.transform.position;
        Vector3 _gravity = new Vector3(correctGravity(75, this.transform.position.x, offset),
                                       correctGravity(75, this.transform.position.y, offset),
                                       correctGravity(75, this.transform.position.z, offset));
        this.GetComponent<Rigidbody>().AddForce(_gravity, ForceMode.Force);

        Ray ray = new Ray(this.transform.position, _gravity);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);
        }
    }

    private float correctGravity(float r, float f, float offset)
    {
        if (f < 0)
        {
            offset = -offset;
            r = -r;
        }

        if (r - f < 0)
        {
            return 0 + offset;
        }
        else
        {
            return (r - f) / 100 + offset;
        }
    }
}
