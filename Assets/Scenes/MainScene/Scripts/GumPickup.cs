using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GumPickup : MonoBehaviour
{
	[SerializeField] private int gumCount;
	
	void OnCollisionEnter2D(Collision2D other)
	{
		if (!other.gameObject.CompareTag("Player"))
		{
			return;
		}
		other.gameObject.GetComponent<PlayerController>().playerState.gumCount += gumCount;
		Destroy(gameObject);
	}
}