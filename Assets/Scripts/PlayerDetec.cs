using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetec : MonoBehaviour
{
    public bool _isHere= false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isHere = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isHere = false;
        }
    }
}
