using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopController : MonoBehaviour
{
    [SerializeField] private Vector3 _endPos;
    [SerializeField] private Vector3 _endRotation;
    [SerializeField] private float _moveTime;
    [SerializeField] private PlayerController _player;
    [SerializeField] private GameManager _gameManager;

    private bool _isMoving;

    PlayerController _playerController;

    Animator animator;

    private void Start()
    {
        _isMoving = true;
        _playerController = _player.GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!_isMoving)
        {
            transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, transform.position.z);
        }
    }

    private void PlayIdle()
    {
        animator.CrossFadeInFixedTime("Idle", 0.3f);
    }


    public void StartMoving()
    {
        animator.Play("StartRunning");
        StartCoroutine(MoveTowardsPlayer(_moveTime));
    }

    public IEnumerator MoveTowardsPlayerDeath(float duration)
    {
        _isMoving = true;
        float timeElapsed = 0;
        Vector3 startPos = transform.position;
        float endPosZ = _player.transform.position.z - 2f;
        while (timeElapsed < duration)
        {
            if (endPosZ - transform.position.z < 0.1f  && !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                PlayIdle();
            }
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(new Vector3(_player.transform.position.x, _player.transform.position.y, startPos.z), new Vector3(_player.transform.position.x, _player.transform.position.y, endPosZ), t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _isMoving = false;
    }

    public IEnumerator MoveTowardsPlayerStumble(float duration)
    {
        _isMoving = true;
        float timeElapsed = 0;
        Vector3 startPos = transform.position;
        float endPosZ = _player.transform.position.z - 1.8f;
        while (timeElapsed < duration && _gameManager.IsRunning)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(new Vector3(_player.transform.position.x, _player.transform.position.y, startPos.z), new Vector3(_player.transform.position.x, _player.transform.position.y, endPosZ), t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(BackUp(4f));
        _isMoving = false;
    }

    private IEnumerator MoveTowardsPlayer(float duration)
    {
        _isMoving = true;
        float timeElapsed = 0;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        while (timeElapsed < duration && _gameManager.IsRunning)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(startPos, _endPos, t);
            transform.rotation = Quaternion.Euler(Vector3.Lerp(new Vector3(startRot.x, startRot.y, startRot.z), _endRotation, t));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        _isMoving = false;
        _playerController._gameStarted = true;
        StartCoroutine(BackUp(2f));
    }

    IEnumerator BackUp(float duration)
    {
        float timeElapsed = 0;
        Vector3 startPos = transform.position;
        Vector3 backUpPos = new Vector3(startPos.x, startPos.y, startPos.z - 3f);
        while (timeElapsed < duration && _gameManager.IsRunning && !_isMoving)
        {
            float t = timeElapsed / duration;
            t = t * t * (3f - 2f * t);
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y, startPos.z), new Vector3(transform.position.x, transform.position.y, backUpPos.z), t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
     }
}
