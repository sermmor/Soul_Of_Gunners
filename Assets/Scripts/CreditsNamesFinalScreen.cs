using UnityEngine;
using System.Collections;

public class CreditsNamesFinalScreen : MonoBehaviour {
    private const float TimeBlink = 0.7f;
    private const float timeToShowText = 3f;
    private const float posXToAddCamera = 0.005f;
    
    public TextMesh title;
    public TextMesh score;
    public TextMesh gameMode;
    public TextMesh pressStart;
    public TextMesh[] credits;
    public float posXInitMove = -1f;
    public float posXEndMove = 1f;
    public int screenGoTo = 1;
    
    private Camera cam;
    private bool isMoveCamera;
    private string[] copiesText;
    private string[] copiesTextsCredits;
    private string oldText;
    private float currentTimeBlink;
    private float currenTimeToShowText;
    private bool showPressStart;
    private bool showStart;
    private int currentCredit;

	void Start () {
        showStart = false;
        currentTimeBlink = 0;
        currenTimeToShowText = 0;
        currentCredit = 0;
        showPressStart = true;
	    oldText = pressStart.text;
        // Deshabilitar todo texto para primero mostrar los créditos.
        copiesText = new string[] {title.text, score.text, gameMode.text,  pressStart.text};
        title.text = "";
        score.text = "";
        gameMode.text = "";
        pressStart.text = "";
        copiesTextsCredits = new string[credits.Length];
        for (int i = 0; i<credits.Length; i++) {
            TextMesh tm = credits[i];
            copiesTextsCredits[i] = tm.text;
            tm.text = "";
        }
        // Cambiamos posición de la cámara.
        cam = Camera.main;
        cam.transform.position = new Vector3(posXInitMove, cam.transform.position.y, cam.transform.position.z);
        isMoveCamera = true;
	}

    void showTheEnd() {
        // Que salta parpadeando el texto en pressStart.
        currentTimeBlink = currentTimeBlink + Time.deltaTime;
        if (currentTimeBlink > TimeBlink) {
            currentTimeBlink = 0;
            // Enseñamos u ocultamos el texto.
            showPressStart = !showPressStart;
            if (showPressStart) {
                pressStart.text = oldText;
            } else {
                pressStart.text = "";
            }
        }

	    if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton7)) {
            if (SaveCreditInCreditList.PlayerInformation.isInPodium() != -1) {
                // Pasamos a la pantalla de introducir score.
                Application.LoadLevel(UserInterfaceGraphics.NUMBER_SCREEN_INTRODUCE_CREDITS);
            } else {
                // Pasamos a la pantalla del menú principal.
                Application.LoadLevel(screenGoTo);
            }
        }
    }

    void moveCamera() {
        if (isMoveCamera) {
            float camX = cam.transform.position.x + posXToAddCamera;
            if (camX <= posXEndMove) {
                cam.transform.position = new Vector3(camX, cam.transform.position.y, cam.transform.position.z);
            } else {
                isMoveCamera = false;
            }
        }
    }

	void Update () {
        // Movemos la cámara.
        moveCamera();
        // Tratamos todo el tema de los créditos y demás textos.
        if (showStart) {
            showTheEnd();
        } else {
            if (!("").Equals(score.text)) {
                copiesText[1] = score.text;
                score.text = "";
            }

            if (!("").Equals(gameMode.text)) {
                copiesText[2] = gameMode.text;
                gameMode.text = "";
            }

            // Mostrar créditos.
            currenTimeToShowText = currenTimeToShowText + Time.deltaTime;
            credits[currentCredit].text = copiesTextsCredits[currentCredit];
            if (currenTimeToShowText >= timeToShowText) {
                // Borro crédito actualmente mostrado.
                credits[currentCredit].text = "";
                currenTimeToShowText = 0;
                // Muestro lo siguiente.
                currentCredit++;
                if (currentCredit >= credits.Length) {
                    // Muestro final.
                    showStart = true;
                    title.text = copiesText[0];
                    score.text = copiesText[1];
                    gameMode.text = copiesText[2];
                    pressStart.text = copiesText[3];
                }
            }
        }
	}


}
