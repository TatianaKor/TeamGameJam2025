using System;
using System.Collections;
using UnityEngine;

public class BubbleGum : MonoBehaviour
{
    [SerializeField] private float autoDestroyTime = 10f;
    [SerializeField] private float inflateAnimationTime = 1f;

    public PlayerController player;

    private float _lifeTimer = 0f;

    private void Start()
    {
        StartCoroutine(DetachFromPlayer());
    }

    private IEnumerator DetachFromPlayer()
    {
        yield return new WaitForSeconds(inflateAnimationTime);
        player.DetachBubble(this);
    }

    private void Update()
    {
        _lifeTimer += Time.deltaTime;

        if (_lifeTimer >= autoDestroyTime)
        {
            Destroy(gameObject);
        }
    }
}
