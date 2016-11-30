using UnityEngine;
using System.Collections;

public class LevelDisplay : MonoBehaviour {
    private const float SHOW_LEVEL_TIME = 1f;
    
    public TextMesh textLevel;
    public GameObject[] pointLevel;

    private int currentPointLevel;
    private bool isInit;
    private GameObject gamecamera;
    private float widthCamera;
    private static int numberLevel;
    private static bool newLevel;
    private static float currentTimeShowNewLevel;

	void Start () {
        isInit = true;
	    numberLevel = 0;
        currentPointLevel = 0;
        notificateNewLevel();

        gamecamera = GameObject.FindGameObjectWithTag("MainCamera");
        //------------- Calcular ancho de la cámara.
        widthCamera = UserInterfaceGraphics.getWidthCamera()/2;
	}
	
    public static void notificateNewLevel() {
        newLevel = true;
        currentTimeShowNewLevel = 0f;
        numberLevel++;
    }

    void showLevel() {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)) {
            textLevel.text = "";
            newLevel = false;
            isInit = false;
        }
        if (newLevel) {
            currentTimeShowNewLevel = currentTimeShowNewLevel + Time.deltaTime;
            if (currentTimeShowNewLevel < SHOW_LEVEL_TIME) {
                // Mostrar título nivel.
                if (numberLevel < 4) {
                    textLevel.text = "Level " + numberLevel;
                } else {
                    textLevel.text = "Boss";
                }
            } else {
                // Dejar de mostrar título del nivel.
                textLevel.text = "";
                newLevel = false;
                isInit = false;
            }
        }
    }
    
	void Update () {
        if (isInit) {
	        showLevel();
        } else if (!newLevel) {
            // Comprobar si hemos llegado a algún punto de nivel (array pointLevel).
            if (currentPointLevel < pointLevel.Length) {
                GameObject go = pointLevel[currentPointLevel];
                float rightSideCameraX = gamecamera.transform.position.x + widthCamera;
                if (rightSideCameraX >= go.transform.position.x) {
                    notificateNewLevel();
                    currentPointLevel++;
                }
            }
        } else {
            showLevel();
        }
	}
}
