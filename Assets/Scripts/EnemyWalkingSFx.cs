using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalkingSFx : MonoBehaviour
{
    AudioSource _sfx;
    private void Start()
    {
        _sfx = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _sfx.Play();
            Debug.Log("Step");
        }
    }
}
