using System;
using UnityEngine;

public class HorizontalSpit : MonoBehaviour
{
    [SerializeField] private HorizontalSpitDescender descenderPrefab;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (other.transform.position.y > transform.position.y)
        {
            return;
        }

        var descenderTopContactPoint = other.GetContact(0).point;
        var descender = Instantiate(descenderPrefab, descenderTopContactPoint + Vector2.down * 0.2f, Quaternion.identity);
        descender.player = other.gameObject.GetComponent<PlayerController>();
        descender.topContactPoint = descenderTopContactPoint;
    }
}
