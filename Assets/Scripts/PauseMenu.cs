using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {
    private float[] RGBA_COLOR_SELECTED = new float[] {1f, 1f, 1f};
    

    public GameObject pauseUI;
    public GameObject[] textsMenuPause;
    public GameObject[] arrowImageTextsMenuPause;
    public GameObject objectWithAudioSource;

    private bool paused;
    private int currentOption;
    private TextMesh[] tmMenuPause;
    private SpriteRenderer[] srArrowMenuPause;
    private float[] rgbaColorUnselected;
    private MusicScene musicScene;
    public AudioClip clicInOption;

	// Use this for initialization
	void Start () {
	    // Al inicio la pausa debe estar desactivada.
        paused = false;
        pauseUI.SetActive(false);
        currentOption = 0;
        GameObject gamecamera = GameObject.FindGameObjectWithTag("MainCamera");
        musicScene = (MusicScene) gamecamera.GetComponent("MusicScene");
        tmMenuPause = new TextMesh[textsMenuPause.Length];
        for (int i = 0; i < textsMenuPause.Length; i++) {
            tmMenuPause[i] = textsMenuPause[i].GetComponent<TextMesh>();
        }
        Color c = tmMenuPause[tmMenuPause.Length - 1].color;
        rgbaColorUnselected = new float[] {c.r, c.g, c.b, c.a};
        if ((arrowImageTextsMenuPause != null) && (arrowImageTextsMenuPause.Length == textsMenuPause.Length)) {
            // Si hay flechas extraer sus SpriteRenderer.
            srArrowMenuPause = new SpriteRenderer[arrowImageTextsMenuPause.Length];
            for (int i = 0; i < arrowImageTextsMenuPause.Length; i++) {
                srArrowMenuPause[i] = arrowImageTextsMenuPause[i].GetComponent<SpriteRenderer>();
            }
        }
	}

    public bool isGameInPause() {
        return paused;
    }
	
    /** Sonido al seleccionar de una opción a la siguiente. */
    void clicSound() {
        if ((objectWithAudioSource != null) && (clicInOption != null)) {
            AudioSource currentAS = (AudioSource) objectWithAudioSource.GetComponent<AudioSource>();
            if (currentAS.clip != clicInOption) {
                currentAS.clip = clicInOption;
            }
                currentAS.Play();
        }
    }

    void incrementCurrentOption() {
        if (currentOption < (textsMenuPause.Length - 1)) {
            currentOption++;
        } else {
            currentOption = 0;
        }
        clicSound();
    }

    void decrementCurrentOption() {
        if (currentOption > 0) {
            currentOption--;
        } else {
            currentOption = textsMenuPause.Length - 1;
        }
        clicSound();
    }

    public void Resume() {
        // Quitar la pausa.
        paused = false;
        musicScene.outPause(); // Activamos opción de música fuera de pausa.
    }

    public void Restart() {
        // Volvemos a empezar desde el último nivel cargado.
        Application.LoadLevel(Application.loadedLevel);
    }

    public void MainMenu() {
        Time.timeScale = 1; // Prosigo el juego.
        Application.LoadLevel(1); // Ir a menú principal sin pasar por la splashscreen.
    }

    public void Quit() {
        Application.Quit();
    }

    void launchCurrentOption() {
        if (currentOption == 0) {
            Resume();
        } else if (currentOption == 1) {
            Restart();
        } else if (currentOption == 2) {
            MainMenu();
        } else if (currentOption == 3) {
            Quit();
        }
    }

    /** Devuelve el estado del eje vertical del mando para los menus. */
    private static bool isEnable;
    public static float getAxisVerticalForMenus() {
        float ret = 0;
        float temp = Input.GetAxis("Vertical");
        isEnable = isEnable || (!isEnable && (temp == 0));
        
        if (isEnable && ((temp > 0) || (temp < 0))) {
            ret = temp;
            isEnable = false;
        }
        return ret;
    }

    void checkActions() {
        // Comprobar si se ha pulsado en un "botón" de la pantalla mediante el teclado o el pad.
        bool isKeyboard = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) 
            || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.X);
        float axisV = (isKeyboard)? 0 : getAxisVerticalForMenus();

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || (axisV == 1f)) {
            // Si hemos pulsado arriba, seleccionamos opción de arriba.
            decrementCurrentOption();
        } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.X) || (axisV == -1f)) {
            // Si hemos pulsado abajo, seleccionamos opción de abajo.
            incrementCurrentOption();
        } else if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K) 
            || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0)) {
            // Si ha pulsado la F, J, K, el espacio o la tecla enter, confirmar la opción seleccionada.
            launchCurrentOption();
        }
        
        // Cambiar de color las opciones no seleccionadas y esconder las flecha no activas (si hay flechas en el menú).
        bool areArrows = (arrowImageTextsMenuPause != null) && (arrowImageTextsMenuPause.Length == textsMenuPause.Length);
        for (int i = 0; i < textsMenuPause.Length; i++) {
            tmMenuPause[i].color = new Color(rgbaColorUnselected[0], rgbaColorUnselected[1], rgbaColorUnselected[2]);
            if (areArrows) {
                SpriteRenderer sr = srArrowMenuPause[i];
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
            }
        }
        // Cambiar de color la opción seleccionada y mostrar la flecha indicada (si hay flechas en el menú).
        tmMenuPause[currentOption].color = new Color(RGBA_COLOR_SELECTED[0], RGBA_COLOR_SELECTED[1], RGBA_COLOR_SELECTED[2]);
        if (areArrows) {
            SpriteRenderer sr = srArrowMenuPause[currentOption];
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
        }
    }

	// Update is called once per frame
	void Update () {
        bool isChanged = false;
	     // Activamos o desactivamos la pausa con botón de escape.
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)) {
            paused = !paused;
            isChanged = true;
        }

        if (paused) {
            // Pauso el juego.
            pauseUI.SetActive(true); // Activo panel de pausa.
            Time.timeScale = 0; // Detengo el juego.
            if (isChanged) {
                musicScene.inPause(); // Activamos opción de música en pausa.
            }
            checkActions();
        }

        if (!paused) {
            pauseUI.SetActive(false); // Desactivo panel de pausa.
            Time.timeScale = 1; // Prosigo el juego
            if (isChanged) {
                musicScene.outPause(); // Activamos opción de música fuera de pausa.
            }
        }
	}
}
