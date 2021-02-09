using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerControl : MonoBehaviour
{
    public float m_Speed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.GetComponent<Rigidbody>().AddForce(Vector3.forward * m_Speed, ForceMode.VelocityChange);
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.transform.GetComponent<Rigidbody>().AddForce(Vector3.back * m_Speed, ForceMode.VelocityChange);
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.transform.GetComponent<Rigidbody>().AddForce(Vector3.left * m_Speed / 2, ForceMode.VelocityChange);
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.transform.GetComponent<Rigidbody>().AddForce(Vector3.right * m_Speed / 2, ForceMode.VelocityChange);
        }
    }
}
