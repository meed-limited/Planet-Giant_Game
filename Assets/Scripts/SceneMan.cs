using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMan : MonoBehaviour
{
    public void SceneReload()
    {
        Debug.Log("Next Level");
        SceneManager.LoadScene(3);
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
    }


}
