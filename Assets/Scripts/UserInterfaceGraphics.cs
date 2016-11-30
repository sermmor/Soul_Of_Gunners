using UnityEngine;
using System.Collections;

public class UserInterfaceGraphics : MonoBehaviour {
    // Para identificar cada layer.
    public const int LAYER_POWER_UP = 8;
    public const int LAYER_SHOOT = 9;
    public const int LAYER_WALL = 10;
    public const int LAYER_PLAYER = 11;
    public const int LAYER_ENEMY = 12;
    public const int LAYER_ENEMY_SHOOT = 13;
    public const int LAYER_ENEMY_MINE = 14;
    public const int LAYER_BOSS = 15;
    public const int LAYER_BOSS_HANDS = 16;

    // Número pantalla de introducir credit.
    public const int NUMBER_SCREEN_INTRODUCE_CREDITS = 8;
    public const int NUMBER_SCREEN_CREDITS_TABLE = 9;

    // MODOS DE JUEGO.
    public static string[] MODES = new string[] {"Easy", "Normal", "Hard"};
    public static int MODE_CHOOSE = 1;

    //------------ Margenes elementos.
    private const float MARGIN_TOP_AVATAR = -0.5f;//-0.2; // Margen entre el elemento y la parte superior de la pantalla.
    private const float MARGIN_LEFT_AVATAR = 2f;//0.6f; // Margen entre el avatar y la parte izquierda de la pantalla.
    private const float MARGIN_TOP_LIFEBAR = -0.75f; // Margen entre el elemento y la parte superior de la pantalla.
    private const float MARGIN_LEFT_LIFEBAR = 0.72f; // Margen entre la barra de vida y el avatar.
    private const float MARGIN_TOP_SCORE = -0.4f; // Margen entre el elemento y la parte superior de la pantalla.
    private const float MARGIN_RIGHT_SCORE = 4f;//3f; // Margen entre el score y la parte derecha de la pantalla.
    private const float MARGIN_TOP_TIME = -0.1f; // Margen entre el elemento y la parte superior de la pantalla.
    private const float MARGIN_TOP_GO = -1f; // Margen entre el GO y la parte intermedia de la pantalla.
    private const float MARGIN_RIGHT_GO = 2.2f; // Margen entre el GO y la parte derecha de la pantalla.

    public GameObject avatar;
    public GameObject time;
    public GameObject score;
    public GameObject goAlert;
    public GameObject goLifeBarForeground;
    public GameObject goLifeBarBackground;
    public GameObject goLifeBoss;
    
    private Player player;
    private float totalScaleX;

    /** Devuelve el ancho de la cámara. */
    public static float getWidthCamera() {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        return height * cam.aspect;
    }

    /** Devuelve el alto de la cámara. */
    public static float getHeightCamera() {
        Camera cam = Camera.main;
        return cam.orthographicSize;;
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

    // Use this for initialization 
    void Start() {
        player = (Player) GameObject.FindGameObjectWithTag("Player").GetComponent("Player");
        totalScaleX = goLifeBarForeground.transform.localScale.x;
        setPositionElementsCamera();
    }
    
    /* Recoloca bien en la pantalla elementos que se muestran estáticos en la cámara, 
    teniendo en cuenta la resolución de la cámara. Así se hace que el juego sea apto
    para todo tipo de resolución. */
    void setPositionElementsCamera() {
        Camera cam = Camera.main;
        float heightCamera = cam.orthographicSize;
        float widthCamera = getWidthCamera()/2;
        // Cambio posición avatar.
        Vector3 aux = avatar.transform.position;
        avatar.transform.position = new Vector3(-widthCamera + MARGIN_LEFT_AVATAR, heightCamera + MARGIN_TOP_AVATAR, aux.z);

        // Cambio posición barra de vida.
        aux = goLifeBarBackground.transform.position;
        goLifeBarBackground.transform.position = new Vector3(avatar.transform.position.x + MARGIN_LEFT_LIFEBAR,
            heightCamera + MARGIN_TOP_LIFEBAR, aux.z);
        goLifeBarForeground.transform.position = new Vector3(goLifeBarBackground.transform.position.x, 
            goLifeBarBackground.transform.position.y, goLifeBarForeground.transform.position.z);

        // Cambio posición score.
        aux = score.transform.position;
        score.transform.position = new Vector3(widthCamera - MARGIN_RIGHT_SCORE, heightCamera + MARGIN_TOP_SCORE, aux.z);

        // Time debe estar siempre en la misma X que la cámara.
        aux = time.transform.position;
        time.transform.position = new Vector3(Camera.main.transform.position.x, heightCamera + MARGIN_TOP_TIME, aux.z);

        // La alerta del GO siempre estará en la misma Y que la cámara.
        aux = goAlert.transform.position;
        goAlert.transform.position = new Vector3(widthCamera - MARGIN_RIGHT_GO, Camera.main.transform.position.y + MARGIN_TOP_GO, aux.z);

        // La vida del GO estará con la misma X del score, pero abajo del todo.
        aux = score.transform.position;
        goLifeBoss.transform.position = new Vector3(aux.x, goLifeBoss.transform.position.y, goLifeBoss.transform.position.z);
        
    }

    void FixedUpdate() {
        // Calculo nuevo scaleX a la caja "verde" y se lo asigno.
        //float totalScaleX = goLifeBarBackground.transform.localScale.x;
        float lifePercent = (player.getCurrentLife()/player.maxLife);
        float newScaleX = totalScaleX * lifePercent;
        goLifeBarForeground.transform.localScale = new Vector3(newScaleX, 
            goLifeBarForeground.transform.localScale.y, goLifeBarForeground.transform.localScale.z);

        // Recoloco la caja "verde" (recordemos que newScaleX == nuevo ancho y totalScaleX == ancho barra completa).
        float diffX = totalScaleX * ((1f - lifePercent)/2f);
        goLifeBarForeground.transform.position = new Vector3(goLifeBarBackground.transform.position.x - diffX,
            goLifeBarBackground.transform.position.y, goLifeBarForeground.transform.position.z);

    }
}
