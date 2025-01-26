using System;
using System.Collections;
using UnityEngine;

public class BubbleGum : MonoBehaviour
{
    [SerializeField] private float autoDestroyTime = 10f;

    private float _lifeTimer = 0f;

    private void Update()
    {
        _lifeTimer += Time.deltaTime;

        if (_lifeTimer >= autoDestroyTime)
        {
            Destroy(gameObject);
        }
    }
}
