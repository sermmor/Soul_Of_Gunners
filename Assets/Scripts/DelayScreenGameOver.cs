using UnityEngine;
using System.Collections;

public class DelayScreenGameOver : MonoBehaviour {
    /*public float delayTime = 3;
    public int goToScreen = 1;

    IEnumerator Start() {
        yield return new WaitForSeconds(delayTime);
        Application.LoadLevel(goToScreen);
    } */
    

    public float delayTime = 3;
    public int goToScreen = 1;
    
	void Start () {
	    Invoke("changeScreen", delayTime);
	}
	
	void changeScreen() {
        if (SaveCreditInCreditList.PlayerInformation.isInPodium() != -1) {
            // Pasamos a la pantalla de introducir score.
            Application.LoadLevel(UserInterfaceGraphics.NUMBER_SCREEN_INTRODUCE_CREDITS);
        } else {
            // Pasamos a la pantalla del menú principal.
            Application.LoadLevel(goToScreen);
        }
    }
}
