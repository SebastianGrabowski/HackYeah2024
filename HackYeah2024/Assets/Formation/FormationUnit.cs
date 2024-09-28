using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormationUnit : MonoBehaviour
{
    public Vector3 Target;

    void Update()
    {
        var dir = Target-transform.position;
        transform.position += dir.normalized * Time.deltaTime * 4.0f;
    }
}
