using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowClose : MonoBehaviour
{
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
