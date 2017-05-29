using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;
	public AudioSource musicSource, efxSource;

	void Awake () {
		if (instance == null) 
			instance = this;
		else
			Destroy(this);
	}
	public void PlaySingle (AudioClip clip) {
		efxSource.volume = 1;
		efxSource.clip = clip;
		efxSource.Play();
	}

	public void PlaySingle (AudioClip clip, float volume) {
		efxSource.volume = volume;
		efxSource.clip = clip;
		efxSource.Play();
	}
}
