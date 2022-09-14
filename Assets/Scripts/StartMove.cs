using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StartMove : MonoBehaviour
{
    private NavMeshAgent _nav;

    private void Start()
    {
        _nav = gameObject.GetComponent<NavMeshAgent>();
    }

    public void StartMoving()
    {
        _nav.enabled = true;
    }
}
