using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coin : MonoBehaviour
{
    [SerializeField] private AchievementManager _am;
    AudioSource audioSource;
    private void Start()
    {
        _am = GameObject.FindGameObjectWithTag("AcMan").GetComponent<AchievementManager>();
        audioSource = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Coin Get!");
            GetComponent<Animator>().SetBool("Collected", true);
            audioSource.Play();
            gameObject.transform.GetChild(4).GetComponent<ParticleSystem>().Play();
            _am.AddAchievementProgress(3, 1);
            Destroy(gameObject, 1.5f);
        }
    }
}
