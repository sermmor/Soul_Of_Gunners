using UnityEngine;
using System.Collections;

public class PressStart : MonoBehaviour {
	private const float TimeBlink = 0.7f;
    
    public TextMesh pressStart;
    public int screenGoTo = 2;

    private string oldText;
    private float currentTimeBlink;
    private bool showPressStart;

	void Start () {
        currentTimeBlink = 0;
        showPressStart = true;
	    oldText = pressStart.text;
	}

	void Update () {
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
            // Pasamos a la pantalla del menú principal.
            Application.LoadLevel(screenGoTo);
        }
	}
}
