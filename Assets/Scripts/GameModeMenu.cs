using UnityEngine;
using System.Collections;

public class GameModeMenu : MonoBehaviour {

	private float[] RGBA_COLOR_SELECTED = new float[] {1f, 1f, 1f};
    
    public GameObject[] textsMenuPause;
    public GameObject[] arrowImageTextsMenuPause;
    public GameObject menuModeGameIU;
    public GameObject objectWithAudioSource;
    public AudioClip audioChooseSelection;
    public AudioClip clicInOption;

    private int currentOption;
    private TextMesh[] tmMenuPause;
    private SpriteRenderer[] srArrowMenuPause;
    private float[] rgbaColorUnselected;

    // Use this for initialization
	void Start () {
        menuModeGameIU.SetActive(false); 
        if ((textsMenuPause != null) && (textsMenuPause.Length > 0)) {
            currentOption = 0;
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

	public void PlayGame() {
        Application.LoadLevel(3);
    }

    private bool ignoreFirstPush = true; // Para que no pille la pulsación del menú anterior porque los dedos en el teclado no son taaan rápidos.
    void launchCurrentOption() {
        if (!ignoreFirstPush) {
            if (currentOption == 0) {
                UserInterfaceGraphics.MODE_CHOOSE = 2; // Modo difícil.
                PlayGame();
            } else if (currentOption == 1) {
                UserInterfaceGraphics.MODE_CHOOSE = 1; // Modo normal.
                PlayGame();
            } else if (currentOption == 2) {
                UserInterfaceGraphics.MODE_CHOOSE = 0; // Modo fácil.
                PlayGame();
            }
            
            // Sonido de opción seleccionada.
            if (audioChooseSelection != null) {
                // Insertamos sonido de seleccionado.
                AudioSource currentAS = (AudioSource) menuModeGameIU.GetComponent<AudioSource>();
                if (currentAS.clip != audioChooseSelection) {
                    currentAS.clip = audioChooseSelection;
                }
                currentAS.Play();
            }
        } else {
            ignoreFirstPush = false;
        }
    }

    void checkActions() {
        // Comprobar si se ha pulsado en un "botón" de la pantalla mediante el teclado.
        bool isKeyboard = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) 
            || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.X);
        float axisV = (isKeyboard)? 0 : UserInterfaceGraphics.getAxisVerticalForMenus();

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

    void Update () {
        if (menuModeGameIU.activeSelf) {
            checkActions();
        }
    }
}
