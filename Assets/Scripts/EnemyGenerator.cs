using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour {
    public const float TIME_INTERVAL_CALL_ENEMIES = 1f;
    public const float X_OFFSET_FLOOR_ENEMY = 0.75f;
    public const float Y_POS_FLOOR_ENEMY = -0.27f;

    public GameObject[] EnemyPrefabs;
    public int numberEnemies;
    
    private int numEnemiesCreated;
    private bool isCalledEnemies;
    private bool unlockCamera;
    private bool isEnd;
    private GameObject gamecamera;
    private BlockCamera blockCamera;
    private System.Collections.Generic.List<GameObject> enemiesCreated;
    private float widthCamera;

	// Use this for initialization
	void Start () {
        isCalledEnemies = false;
        unlockCamera = false;
        isEnd = false;
        numEnemiesCreated = 0;
        gamecamera = GameObject.FindGameObjectWithTag("MainCamera");
        blockCamera = (BlockCamera) gameObject.GetComponent("BlockCamera");
        enemiesCreated = new System.Collections.Generic.List<GameObject>();

        //------------- Calcular ancho de la cámara.
        widthCamera = UserInterfaceGraphics.getWidthCamera()/2;
	}
	
	// Update is called once per frame
	void Update () {
        if (!isEnd) {
            if (!isCalledEnemies && blockCamera.isBlockCameraInThisBlock()) {
                // LLAMAR ENEMIGOS.
                InvokeRepeating("callEnemies", 0, TIME_INTERVAL_CALL_ENEMIES);
                isCalledEnemies = true;
            }
	        if (unlockCamera) {
                // Si se ha acabado con todos los enemigos, desbloquear cámara en este punto.
                blockCamera.unblockCamera();
                isEnd = true;
            } else {
                // COMPROBAR SI TODOS LOS ENEMIGOS CREADOS ESTÁN MUERTOS.
                bool allEnemiesAreDead = true;
                if ((enemiesCreated != null) && (enemiesCreated.Count > 0)) {
                    // Comprobar si hay elementos que no son nulos (no están muertos) en la lista de enemigos.
                    int count = 0;
                    int size = enemiesCreated.Count;
                    while ((count < size) && allEnemiesAreDead) {
                        GameObject go = enemiesCreated[count];
                        allEnemiesAreDead = (go == null);
                        count++;
                    }
                } else {
                    allEnemiesAreDead = false;
                }
                // Lo siguiente no tengo ni idea de si funcionará.
                if ((isCalledEnemies) && (numEnemiesCreated >= numberEnemies) && (allEnemiesAreDead)) {
                    unlockCamera = true;
                }
            }
        }
	}

    void callEnemies() {
        // Crear un enemigo.
        if (!isEnd && (numEnemiesCreated < numberEnemies)) {
            // Escoger aleatoriamente lado por el que saldrá el enemigo.
            int side = Random.Range(0, 2);
            float posX = (side==0)? 
                gamecamera.transform.position.x + widthCamera 
                : gamecamera.transform.position.x - widthCamera;
            // Escoger aleatoriamente enemigo.
            int n = Random.Range(1, EnemyPrefabs.Length + 1);
            
            GameObject ePrefab = EnemyPrefabs[n-1];
            GameObject enemy = GameObject.Instantiate<GameObject>(ePrefab);
            Transform tEnemy = enemy.transform;
            bool isAirType2 = (enemy.GetComponent("AirEnemyType2") != null);
            if ((enemy.GetComponent("AirEnemy") != null) || isAirType2) {
                // Si es un enemigo de aire colocar en la posición del generador.
                if (isAirType2 && (side==1)) {
                    // Como aparece del otro lado, cambiar la dirección de este enemigo.
                    tEnemy.localScale = new Vector3(-tEnemy.localScale.x, tEnemy.localScale.y, tEnemy.localScale.z);
                }
                tEnemy.position = new Vector3(posX, transform.position.y, transform.position.z);
            } else {
                // Si es un enemigo de tierra, colocar en con y = Y_POS_FLOOR_ENEMY.
                float offsetX = (side==0)? X_OFFSET_FLOOR_ENEMY : -X_OFFSET_FLOOR_ENEMY;
                tEnemy.position = new Vector3(posX + offsetX, Y_POS_FLOOR_ENEMY, transform.position.z);
            }

            enemiesCreated.Add(enemy);
            numEnemiesCreated++;
        }
    }
}
