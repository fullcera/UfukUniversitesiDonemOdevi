using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    void Update()
    {
        RotateTrap();
    }
    public void RotateTrap()
    {
        transform.Rotate(new Vector3(0, 175f, 0) * Time.deltaTime);
    }
}
