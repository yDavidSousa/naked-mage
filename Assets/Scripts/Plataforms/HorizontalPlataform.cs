using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HorizontalPlataform : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.DOMoveX(-0.5485f, 5f).
						SetLoops(-1, LoopType.Yoyo).
						SetEase(Ease.Linear);
	}
	
}
