using UnityEngine;
using System.Collections;

public class DelayScreen : MonoBehaviour {
    public float delayTime = 5;
    public int goToScreen = 1;

    IEnumerator Start() {
        yield return new WaitForSeconds(delayTime);
        Application.LoadLevel(goToScreen);
    }
}
