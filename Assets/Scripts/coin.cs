using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coin : MonoBehaviour
{
    [SerializeField] private AchievementManager _am;
    private bool isTouched = false;
    AudioSource audioSource;
    private void Start()
    {
        _am = GameObject.FindGameObjectWithTag("AcMan").GetComponent<AchievementManager>();
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")&& isTouched == false)
        {
            Debug.Log("Coin Get!");
            GetComponent<Animator>().SetBool("Collected", true);
            audioSource.Play();
            gameObject.transform.GetChild(4).GetComponent<ParticleSystem>().Play();
            _am.AddAchievementProgress(3, 1);
            isTouched = true;
            Destroy(gameObject, 1f);
        }
    }
}
