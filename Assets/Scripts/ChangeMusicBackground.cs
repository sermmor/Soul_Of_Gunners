using UnityEngine;
using System.Collections;

public class ChangeMusicBackground : MonoBehaviour {

    public AudioClip audioBucle;

    private BossGiantRobot boss;
    private AudioSource currentAudioSource;
    private BlockCamera blockCamera;
    private bool isPlaying;

	// Use this for initialization
	void Start () {
        GameObject gamecamera = GameObject.FindGameObjectWithTag("MainCamera");
        currentAudioSource = (AudioSource) gamecamera.GetComponent<AudioSource>();
	    blockCamera = (BlockCamera) gameObject.GetComponent("BlockCamera");
        boss = (BossGiantRobot) GameObject.FindGameObjectWithTag("Boss").GetComponent("BossGiantRobot");
        isPlaying = false;
	}
	
    void playLoop() {
        // Paramos anterior música
        currentAudioSource.loop = false;
        currentAudioSource.Stop();
        // Comenzamos la nueva.
        currentAudioSource.clip = audioBucle;
        currentAudioSource.loop = true;
        currentAudioSource.Play();
    }

	// Update is called once per frame
	void Update () {
	    if (!isPlaying && blockCamera.isBlockCameraInThisBlock()) {
            playLoop();
            isPlaying = true;
            // Activamos al boss.
            boss.activeBoss();
        }
	}
}
