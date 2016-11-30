using UnityEngine;
using System.Collections;

public class MusicScene : MonoBehaviour {

    public AudioClip audioIntro;
    public AudioClip audioBucle;

    private AudioSource currentAudioSource;
    private bool isIntro;
    private float volumeMusic;

	// Use this for initialization
	void Start () {
        GameObject gamecamera = GameObject.FindGameObjectWithTag("MainCamera");
        currentAudioSource = (AudioSource) gamecamera.GetComponent<AudioSource>();
        
        volumeMusic = currentAudioSource.volume;
        if (audioIntro != null) {
            currentAudioSource.clip = audioIntro;
            currentAudioSource.Play();
            isIntro = true;
            Invoke("playLoop", audioIntro.length - 0.1f); // Invocamos a reproducir el bucle casi al acabar la canción para que no queden parones.
        } else {
            isIntro = false;
            playLoop();
        }
	}

    void playLoop() {
        isIntro = false;
        currentAudioSource.clip = audioBucle;
        currentAudioSource.loop = true;
        currentAudioSource.Play();
    }

    public void inPause() {
        currentAudioSource.volume = volumeMusic/4f;
    }

    public void outPause() {
        currentAudioSource.volume = volumeMusic;
    }

    void Update () {
        if (isIntro && !currentAudioSource.isPlaying) {
            // Para evitar errores en casos como que se hace pausa en la intro.
            playLoop();
        }
    }
}
