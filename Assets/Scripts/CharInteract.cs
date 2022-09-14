using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharInteract : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && Input.GetKeyDown("E"))
        {

        }
    }
}
