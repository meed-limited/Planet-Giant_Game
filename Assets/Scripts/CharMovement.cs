using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharMovement : MonoBehaviour
{
    private Animator _ani;
    [SerializeField] private GameObject target;
    //
    public float moveSpeed = 5;
    public float rotationSpeed = 5;
    bool _isMoving = false;
    [SerializeField] private GameManager _gm;
    private NavMeshAgent _navAgent;
    [SerializeField] private Transform _movePos;
    [SerializeField] private AchievementManager _am;
    [SerializeField] private AudioClip[] _sfxs;
    private AudioSource _audioSource;


    void Start()
    {
        _ani = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player");
        _gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _navAgent = GetComponent<NavMeshAgent>();
        _movePos = target.GetComponent<Transform>();
        _am = GameObject.FindGameObjectWithTag("AcMan").GetComponent<AchievementManager>();
        _audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        float _dis = Vector3.Distance(gameObject.transform.position, target.transform.position);
        if (_dis <= 5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), rotationSpeed * Time.deltaTime);
            if (_navAgent.enabled == true)
            {
                _navAgent.destination = _movePos.position;
            }

        }
       

        //move towards the player
        
        if (_dis >= 1.5f && _isMoving == true)
        {

            _navAgent.enabled = true;
            _ani.SetFloat("Speed", 1);
        }
        else
        {
            _navAgent.enabled = false;
            _ani.SetFloat("Speed", 0);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_isMoving == false)
            {
                PlaySfx();
                _gm.ReduceCount();
                _am.AddAchievementProgress(0, 1);

            }
            _ani.SetFloat("Speed", 1);
            _isMoving = true;
            _navAgent.enabled = true;
            ParticleSystem _vfx = gameObject.GetComponentInChildren<ParticleSystem>();
            _vfx.Play();
        }

        if (other.gameObject.CompareTag("building"))
        {
            Debug.Log("Spawn in building"); 
            int _spawnX = UnityEngine.Random.Range(-107, 24);

            int _spawnZ = UnityEngine.Random.Range(1, 128);

            transform.position = new Vector3(_spawnX, transform.position.y, _spawnZ);
        }
    }

    private void PlaySfx()
    {
        int index = UnityEngine.Random.Range(0, _sfxs.Length);
        _audioSource.clip = _sfxs[index];
        _audioSource.Play();
    }
}
