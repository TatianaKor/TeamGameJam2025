using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GumPickup : MonoBehaviour
{
	[SerializeField] private int gumCount;
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.gameObject.CompareTag("Player"))
		{
			return;
		}
		other.gameObject.GetComponent<PlayerController>().playerState.gumCount += gumCount;
		gameObject.SetActive(false);
	}
}