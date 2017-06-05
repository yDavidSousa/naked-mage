using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour {
	
	void OnTriggerEnter2D (Collider2D collision2D) {
		if (collision2D.gameObject.CompareTag("Collider") || 
			collision2D.gameObject.CompareTag("Enemy"))
			Destroy(gameObject, 0.1f);
	}
}
