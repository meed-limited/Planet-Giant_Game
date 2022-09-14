using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DetectFollow : MonoBehaviour
{
    private void Start()
    {
        int index = PlayerPrefs.GetInt("CharSelected", 0);
        gameObject.GetComponent<CinemachineVirtualCamera>().Follow = GameObject.Find("PlayerCameraRoot"+index).transform;
    }
}
