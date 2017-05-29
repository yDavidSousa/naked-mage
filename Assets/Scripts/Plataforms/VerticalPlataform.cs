using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class VerticalPlataform : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.DOMoveY(0.06f, 5f).
						SetLoops(-1, LoopType.Yoyo).
						SetEase(Ease.Linear);
	}

}
