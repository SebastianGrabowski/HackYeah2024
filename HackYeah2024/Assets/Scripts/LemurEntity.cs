using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LemurEntity : MonoBehaviour
{
    public Collider Collider;
    public PlayerController PlayerController;
    public Vector3 Target;
    public bool IsInTeam;

    public void SetInTeam()
    {
        IsInTeam = true;
        Collider.isTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsInTeam)
            return;

        var dir = (PlayerController.TargetPos + Target)-transform.position;
        transform.position += dir.normalized * Time.deltaTime * 4.0f;
    }
}
