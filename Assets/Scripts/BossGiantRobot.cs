using UnityEngine;
using System.Collections;

public class BossGiantRobot : MonoBehaviour {
    const int NumberCallEnemiesModeEasy = 2;
    const int NumberCallEnemiesModeNormal = 3;
    const int NumberCallEnemiesModeHard = 5;
    const float timeToLaunchMineEasy = 2f;
    const float timeToLaunchMineNormal = 1f;
    const float timeToLaunchMineHard = 0.5f;
    private const float FACTOR_DECREASE_VELOCITY_X = 3f;
    private const float TIME_TO_ATTACK = 1.5f;
    private const float TIME_IN_ATTACK = 0.4f; // 0.7f;
    
    public float lenghtSight = 10f;
    public float lenghtAttack = 3f;
    public float maxSpeed = 1.5f;
    public float speed = 5000;
    public float assaultSpeed = 250000;
    public float power = 10;
    public float jumpPower = 160000;
    public float currentLife = 500;
    public float addToScore = 5000;
    public GameObject BossHands; // Objecto con la física de las manos del boss.
    public GameObject minePrefab;
    public GameObject shadowPrefabs;
    public GameObject[] EnemyPrefabsEasy;
    public GameObject[] EnemyPrefabsNormal;
    public GameObject[] EnemyPrefabsHard;
    public AudioClip audioShoot;
    public AudioClip audioHurt;
    public AudioClip audioDie;
    public TextMesh showLifeBoss;

    private Strategy[] strategies;
    private int indexCurrentStrategy;
    private bool isActive;
    
    private bool isInAssault = false;
    private float currentTimeLaunchMine;
    private float maxLife;
    private bool isTouchFloor;
    protected float widthCamera;
    private string stateEnemy;
    private GameObject shadow;
    private GameObject goPlayer;
    private Player player;
    private Animator anim;
    private Rigidbody2D rb2d;
    protected GameObject gamecamera;
    private CameraFollow cameraFollow;
    private PauseMenu pauseMenu;
    // Para el hurt (tiempo que dura el hurt, tiempo entre la transición normal/rojo), y si está en hurt.
    private const float timeInHurt = 0.25f;
    private const float timeBlink = 0.07f;
    private float currentTimeBlink;
    private bool isInRedColor;
    private bool isBlinkInRed = false;

	void Start () {
        // Lo típico.
        gamecamera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraFollow = (CameraFollow) gamecamera.GetComponent("CameraFollow");
        pauseMenu = (PauseMenu) gamecamera.GetComponent("PauseMenu");
        goPlayer = GameObject.FindGameObjectWithTag("Player");
        player = (Player) goPlayer.GetComponent("Player");
        anim = gameObject.GetComponent<Animator>();
	    rb2d = gameObject.GetComponent<Rigidbody2D>();
        isTouchFloor = true;
        currentTimeLaunchMine = 0f;
        //------------- Colisiones a ignorar.
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY, UserInterfaceGraphics.LAYER_ENEMY, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_ENEMY_SHOOT, UserInterfaceGraphics.LAYER_ENEMY, true);
        //------------- Calcular ancho de la cámara.
        widthCamera = UserInterfaceGraphics.getWidthCamera()/2;
        //------------- Creo la sombra del enemigo.
        shadow = GameObject.Instantiate<GameObject>(shadowPrefabs);
        ShadowCharacter sc = (ShadowCharacter) shadow.GetComponent("ShadowCharacter");
        sc.characterToFollow = gameObject;

        // Creamos estrategias e inicializamos variables asociadas a las mismas.
        strategies = new Strategy[] { new StopStrategy(1f, this), new StopStrategy(0.5f, this),
            new JumpStrategy(-1f, this), new CallEnemiesStrategy(-1f, this),
            new FlyTypeZ(-1f , this), new AssaultStrategy(-1, this), new StopStrategy(2f, this)};
        indexCurrentStrategy = 0;
        isIn23 = true;
        isIn13 = false;
        currentJump = 0;
        maxLife = currentLife;
        // Como al inicio estamos en idle (y no en RUN o JUMP), activo las manos, desactivo el boss.
        enableHands(true);
        isActive = false;
        // No está parpadeando en rojo al inicio.
        currentTimeBlink = 0;
        isInRedColor = false;
        isBlinkInRed = false;
        // Desactivo colisiones que no necesito.
	    Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_BOSS, UserInterfaceGraphics.LAYER_ENEMY, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_BOSS, UserInterfaceGraphics.LAYER_ENEMY_SHOOT, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_BOSS, UserInterfaceGraphics.LAYER_POWER_UP, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_BOSS_HANDS, UserInterfaceGraphics.LAYER_ENEMY, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_BOSS_HANDS, UserInterfaceGraphics.LAYER_ENEMY_SHOOT, true);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_BOSS_HANDS, UserInterfaceGraphics.LAYER_POWER_UP, true);
	}
	
    /** Activa al boss. */
    public void activeBoss() {
        isActive = true;
    }

    /** Habilita las manos si enable == true, las desabilita si enable == false. */
    void enableHands(bool enable) {
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_BOSS_HANDS, UserInterfaceGraphics.LAYER_PLAYER, !enable);
        Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_BOSS_HANDS, UserInterfaceGraphics.LAYER_SHOOT, !enable);
    }

    /** Activa o desactiva gravedad. */
    void enableGravity(bool enable) {
        rb2d.gravityScale = enable? 1 : 0;
    }

    Vector2 getVelocity() {
        return new Vector2(rb2d.velocity.x, rb2d.velocity.y);
    }
    void setVelocity(float x, float y) {
        rb2d.velocity = new Vector2(x, y);
    }

    void setIsTouchFloor(bool newValue) {
        isTouchFloor = newValue;
    }

    bool getIsInFloor() {
        return isTouchFloor;
    }
    
    public void damage(float n) {
        currentLife -= n;
        if (currentLife < 0) {
            currentLife = 0;
        } else if (audioHurt != null) {
            // Insertamos sonido de hurt.
            AudioSource currentAS = (AudioSource) gameObject.GetComponent<AudioSource>();
            if (currentAS.clip != audioHurt) {
                currentAS.clip = audioHurt;
            }
            currentAS.Play();
        }

        // Parpadeamos en rojo.
        isBlinkInRed = true;
    }

    public string getState() {
        return stateEnemy;
    }

    void refleshStatus() {
        if (anim.GetBool("isAttack")) {
            stateEnemy = "isAttack";
        } else if (anim.GetBool("isIdle")) {
            stateEnemy = "isIdle";
        } else if (anim.GetBool("isWalk")) {
            stateEnemy = "isWalk";
        } else if (anim.GetBool("isRun")) {
            stateEnemy = "isRun";
        } else if (anim.GetBool("isJump")) {
            stateEnemy = "isJump";
        } else if (anim.GetBool("isDie")) {
            stateEnemy = "isDie";
        } else {
            // Si está todo en falso porque ha entrado en un estado extraño, poner en idle.
            anim.SetBool("isIdle", true);
            stateEnemy = "isIdle";
        }

    }

     void putLookingToPlayer() {
        if (goPlayer.transform.position.x < transform.position.x) {
            // Mirar hacia la izquierda.
            transform.localScale = new Vector3(-1, 1, 1);
        } else {
            // Mirar hacia la derecha.
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void idle() {
        anim.SetBool(stateEnemy, false);
        anim.SetBool("isIdle", true);
        stateEnemy = "isIdle";
        setVelocity(0, 0);
    }

    float timeToLaunchMineByGameMode() {
        float ret = timeToLaunchMineNormal; // Modo normal.
        if (UserInterfaceGraphics.MODE_CHOOSE == 0) {
            ret = timeToLaunchMineEasy; // Modo fácil.
        } else if (UserInterfaceGraphics.MODE_CHOOSE == 2) {
            ret = timeToLaunchMineHard; // Modo difícil.
        }
        return ret;
    }

    void launchMines() {
        currentTimeLaunchMine = currentTimeLaunchMine + Time.deltaTime;
        if (currentTimeLaunchMine >= timeToLaunchMineByGameMode()) {
            // Lanzamos la mina.
            GameObject goMine = GameObject.Instantiate<GameObject>(minePrefab);
            goMine.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 
                goMine.transform.position.z);

            currentTimeLaunchMine = 0;

            // Insertamos sonido de mina.
            if (audioShoot != null) {
                AudioSource currentAS = (AudioSource) gameObject.GetComponent<AudioSource>();
                if (currentAS.clip != audioShoot) {
                    currentAS.clip = audioShoot;
                }
                currentAS.Play();
            }
        }
    }

    void move() {
        if (rb2d != null) {
            // Incrementar velocidad de andar pero controlando que no se pase de maxSpeed ni se chorre.
            Vector3 easeVelocity = new Vector3();
            easeVelocity.y = rb2d.velocity.y;
            easeVelocity.z = 0.0f;
            easeVelocity.x = rb2d.velocity.x * FACTOR_DECREASE_VELOCITY_X;

            // Asignar velocidad con factor de decrecimiento de velocidad si no está en el aire.
            if (rb2d.velocity.y == 0) {
                rb2d.velocity = easeVelocity;
            }

            float direction = (player.transform.position.x < transform.position.x) ? -1 : 1;
            rb2d.AddForce(Vector2.right * speed * direction);

            // Controlamos que no se pase del rango [-maxSpeed, maxSpeed].
            if (rb2d.velocity.x > maxSpeed) {
                rb2d.velocity = new Vector2(maxSpeed, rb2d.velocity.y);
            }

            if (rb2d.velocity.x < -maxSpeed)
            {
                rb2d.velocity = new Vector2(-maxSpeed, rb2d.velocity.y);
            }
        } else if (!("isDie").Equals(stateEnemy)) {
            die();
        }
    }

    void moveAssault() {
        if (rb2d != null) {
            // Incrementar velocidad de andar pero controlando que no se pase de maxSpeed ni se chorre.
            Vector3 easeVelocity = new Vector3();
            easeVelocity.y = rb2d.velocity.y;
            easeVelocity.z = 0.0f;
            easeVelocity.x = rb2d.velocity.x * FACTOR_DECREASE_VELOCITY_X;

            // Asignar velocidad con factor de decrecimiento de velocidad si no está en el aire.
            if (rb2d.velocity.y == 0) {
                rb2d.velocity = easeVelocity;
            }

            float direction = transform.localScale.x;
            rb2d.AddForce(Vector2.right * assaultSpeed * direction);

            // Controlamos que no se pase del rango [-maxSpeed, maxSpeed].
            if (rb2d.velocity.x > maxSpeed) {
                rb2d.velocity = new Vector2(maxSpeed, rb2d.velocity.y);
            }

            if (rb2d.velocity.x < -maxSpeed)
            {
                rb2d.velocity = new Vector2(-maxSpeed, rb2d.velocity.y);
            }
        } else if (!("isDie").Equals(stateEnemy)) {
            die();
        }
    }

    void walk() {
        // Colocar en su dirección correcta y asignar estado walk.
        putLookingToPlayer();
        anim.SetBool(stateEnemy, false);
        anim.SetBool("isWalk", true);
        stateEnemy = "isWalk";
        move();
    }
    
    void onlyChangeToJumpState() {
        anim.SetBool(stateEnemy, false);
        anim.SetBool("isJump", true);
        stateEnemy = "isJump";
    }

    void jump() {
        if (isTouchFloor) {
            float direction = transform.localScale.x;
            rb2d.AddForce(new Vector2(speed * 10 * direction, jumpPower));
            onlyChangeToJumpState();
            isTouchFloor = false;
            enableHands(false);
        }
    }
    
    void preAssault() {
        // Antes de realizar el asalto.
        putLookingToPlayer();
        enableHands(false);
    }

    void launchAssault() {
        // Asalto.
        anim.SetBool(stateEnemy, false);
        anim.SetBool("isRun", true);
        stateEnemy = "isRun";
        isInAssault = true;
    }

    void postAssault() {
        // Fin del asalto.
        rb2d.velocity = new Vector2(0, 0);
        isInAssault = false;
        enableHands(true);
    }
	
    private const int numbOfJumps = 6;
    private int currentJump = 0;
    void enemyIALifeMoreThan23() {
        switch (indexCurrentStrategy) {
            case 0:
                // Ha acabado el Stop => Volver a saltar.
                currentJump = 0;
                indexCurrentStrategy = 2;
            break;
            case 1:
                // Tras pequeño parón, volver a salto.
                putLookingToPlayer();
                indexCurrentStrategy = 2;
            break;
            case 2:
                currentJump++;
                if (currentJump >= numbOfJumps) {
                    // Ha acabado de saltar => Invoca enemigos.
                    indexCurrentStrategy = 3;
                } else {
                    // Ir a pequeño idle antes de siguiente salto.
                    indexCurrentStrategy = 1;
                }
            break;
            case 3:
                // Ha acabado de invocar enemigos => Stop.
                indexCurrentStrategy = 0;
            break;
        }
    }

    void enemyIALifeMoreThan13() {
        switch (indexCurrentStrategy) {
            case 0:
                // Ha acabado el Stop => llamamos enemigos.
                indexCurrentStrategy = 3;
            break;
            case 1:
                // Ha acabado el stop pequeño => ataque Z.
                indexCurrentStrategy = 4;
            break;
            case 3:
                // Ha acabado el invocar enemigos => llamamos a un stop pequeño.
                indexCurrentStrategy = 1;
            break;
            case 4:
                // Ha acabado ataque Z => ir a stop largo.
                indexCurrentStrategy = 0;
            break;
        }
    }

    void enemyIANearDeath() {
        switch (indexCurrentStrategy) {
            case 1:
                // Tras pequeño parón, saltar.
                indexCurrentStrategy = 2;
            break;
            case 2:
                // Ha acabado de saltar => Invoca enemigos.
                indexCurrentStrategy = 3;
            break;
            case 3:
                // Ha acabado de invocar enemigos => Stop.
                indexCurrentStrategy = 6;
            break;
            case 5:
                // Ha acabado el asalto => pequeño parón.
                indexCurrentStrategy = 1;
            break;
            case 6:
                // Ha acabado el Stop => Asalto.
                currentJump = 0;
                indexCurrentStrategy = 5;
            break;
        }
    }
    
    private bool isIn23 = true;
    private bool isIn13 = false;
    void enemyIA() {
        // CAMBIOS DE ESTRATEGIAS.
        Strategy currentStrategy = strategies[indexCurrentStrategy];
        bool isEndStrategy = currentStrategy.doStrategy();
        if (isEndStrategy) {
            if (currentLife > (maxLife*2/3)) {
                enemyIALifeMoreThan23();
            } else if (currentLife > (maxLife*1/3)) {
                if (isIn23) {
                    // Comenzamos 2ª fase del combate.
                    Debug.Log("Vida del boss <= 2/3");
                    isIn23 = false;
                    isIn13 = true;
                    indexCurrentStrategy = 0;
                } else {
                    enemyIALifeMoreThan13();
                }
            } else {
                if (isIn13) {
                    // Comenzamos 3ª fase del combate.
                    isIn13 = false;
                    indexCurrentStrategy = 6;
                    power = power + 10; // Incrementamos su fuerza en cinco unidades más.
                    Debug.Log("Vida del boss <= 1/3");
                } else {
                    enemyIANearDeath();
                }
            }
        }
        
    }

    void destroySelf() {
        Player.score = Player.score + addToScore;
        // Destruir enemigo.
        Destroy(this.gameObject);
        // Pasamos a la pantalla de fin del juego.
        Application.LoadLevel(6);
    }

    void die() {
        // Asignar estado muerte.
        anim.SetBool(stateEnemy, false);
        anim.SetBool("isDie", true);
        stateEnemy = "isDie";
        // Destruyo BoxCollider2D y RigidBody2D, para desactivar física en este estado.
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        if (rb2d != null) {
            Destroy(rb2d);
        }
        // Destruyo la sombra.
        Destroy(shadow);
        // Marco soltar powerUp y a destruir enemigo.
        Invoke("destroySelf", 1f);

        // Activo sonido de muerte.
        if (audioDie != null) {
            // Insertamos sonido de muerte.
            AudioSource currentAS = (AudioSource) gameObject.GetComponent<AudioSource>();
            if (currentAS.clip != audioDie) {
                currentAS.clip = audioDie;
                currentAS.Play();
            }
        }
    }
    
    void blinkRed() {
        if (isBlinkInRed) {
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();

            currentTimeBlink = currentTimeBlink + Time.deltaTime;
            if (currentTimeBlink < timeInHurt) {
                if (isInRedColor) {
                    // Pasamos a color normal.
                    sr.color = new Color(1f, 1f, 1f);
                } else {
                    // Pasamos a rojo.
                    sr.color = new Color(1f, 0f, 0f);
                }
                isInRedColor = !isInRedColor;
                // Seguimos parpadeando.
                Invoke("blinkRed", timeBlink);
            } else {
                // Nos aseguramos de quitar el rojo y desactivar parpadeo antes de acabar.
                currentTimeBlink = 0;
                isInRedColor = false;
                isBlinkInRed = false;
                sr.color = new Color(1f, 1f, 1f);
            }
        }
    }

	void Update () {
        if (isActive) {
            if (!pauseMenu.isGameInPause()) {
                refleshStatus();
                if (currentLife <= 0) {
                    die();
                } else {
                    enemyIA();
                }
            }
            // Mostrar vida del boss.
            showLifeBoss.text = "Boss: " + currentLife;
            // Parpadeo en caso de hurt.
            blinkRed();
        }
	}

    bool canMoveInScreen() {
        bool ret = true;
        if (cameraFollow.isFreezeCamera()) {
            // Si la cámara está congelada comprobar si el enemigo está en los límites.
            float leftSideCameraX = gamecamera.transform.position.x - widthCamera;
            float rightSideCameraX = gamecamera.transform.position.x + widthCamera;
            float posX = transform.position.x;

            if ((leftSideCameraX > posX) && (Input.GetAxis("Horizontal") < 0)) {
                transform.position = new Vector3(leftSideCameraX, transform.position.y, transform.position.z);
                ret = false;
            } else if ((posX > rightSideCameraX) && (Input.GetAxis("Horizontal") > 0)) {
                transform.position = new Vector3(rightSideCameraX, transform.position.y, transform.position.z);
                ret = false;
            }
        }
        return ret;
    }

    void FixedUpdate() {
        if (isActive) {
            if (!pauseMenu.isGameInPause()) {
                // Si estamos en asalto, moverse.
                if (isInAssault) {
                    moveAssault();
                }

                // Evitar que el enemigo se salga de los límites.
                if (!canMoveInScreen() && (rb2d != null)) {
                    rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
                }
            }
        }
    }
    
    // Colisiones contra el enemigo.
    void OnCollisionEnter2D(Collision2D col) {
        switch (col.gameObject.tag)
        {
            case "Player":
                ((Player) (col.gameObject.GetComponent("Player"))).damage(power);
                if (!isTouchFloor) {
                    // Si ha saltado encima del player, para que caiga en el suelo.
                    Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_BOSS, UserInterfaceGraphics.LAYER_PLAYER, true);
                }
                break;
            case "Ground":
                isTouchFloor = true;
                if (anim.GetBool("isJump")) {
                    idle();
                    enableHands(true);
                }
                // Tras aterrizar en el suelo, debe tener velocidad (0,0) para que no se chorre.
                rb2d.velocity = new Vector2(0, 0);
                // Me aseguro que el contacto con el player está activo.
                Physics2D.IgnoreLayerCollision(UserInterfaceGraphics.LAYER_BOSS, UserInterfaceGraphics.LAYER_PLAYER, false);
                break;
        }
    }

/**************************************************** ESTRATEGIAS ******************************************************/
    class Strategy {
        protected float times; // Tiempos para cada acción (-1 si la acción no necesita tiempo).
        protected float currentTime;
        protected BossGiantRobot boss;

        public Strategy(float times, BossGiantRobot boss) {
            this.boss = boss;
            this.times = times;
        }

        public void setTimes(float newTime) {
            times = newTime;
        }

        /*** Clase a implementar por descendientes. Indica qué hace en una acción determinada de la estrategia. 
             Devuelve si la acción ha acabado o no.*/
        protected virtual bool launchAction() {
            return false;
        }

        /*** Devuelve true si ha acabado la acción actual. */
        bool executeAction() {
            bool isEnding = false;
            if (times > 0) {
                currentTime = currentTime + Time.deltaTime;
                if (currentTime < times) {
                    if (launchAction()) {
                        isEnding = true;
                        currentTime = 0;
                    }
                } else {
                    isEnding = true;
                    currentTime = 0;
                }
            } else if (launchAction()) {
                isEnding = true;
                currentTime = 0;
            }
            return isEnding;
        }
        
        private void reset() {
            currentTime = 0;
        }

        /*** Ejecuta la estrategia en curso (si ha acabado la estrategia - es decir han acabado 
        todas las acciones -, devolverá true). */
        public bool doStrategy() {
            bool isEnding = executeAction();
            if (isEnding) {
                reset();
            }
            return isEnding;
        }
    }
    
    // TERMINADO.
    class StopStrategy : Strategy {
        public StopStrategy(float times, BossGiantRobot boss) : base(times, boss) { }

        protected override bool launchAction() {
            bool isEnding = false;
            boss.idle();
            return isEnding;
        }
    }
    
    // TERMINADO.
    class JumpStrategy : Strategy {
        private bool isInJump;

        public JumpStrategy(float times, BossGiantRobot boss) : base(times, boss) {
            isInJump = false;
        }

        protected override bool launchAction() {
            bool isEnding = false;
            // Salta (atemporal).
            if (!isInJump && boss.isTouchFloor) {
                //boss.putLookingToPlayer();
                boss.jump();
                isInJump = true;
            } else if (boss.isTouchFloor) {
                // Fin de la estrategia (pongo las variables con su valor inicial).
                isInJump = false;

                isEnding = true;
            }
            return isEnding;
        }
    }
    
    class CallEnemiesStrategy : Strategy {
        private float currentTimeToGenerate;
        private int currentNumberOfEnemies;

        public CallEnemiesStrategy(float times, BossGiantRobot boss) : base(times, boss) {
            currentTimeToGenerate = 0;
            currentNumberOfEnemies = numberOfEnemiesByGameMode();
        }

        void markAsFromBoss(GameObject enemy) {
            FloorEnemy floor = (FloorEnemy) enemy.GetComponent("FloorEnemy");
            if (floor != null) {
                floor.createdFromBoss(boss);
            } else {
                AirEnemyType2 airType2 = (AirEnemyType2) enemy.GetComponent("AirEnemyType2");
                if (airType2 != null) {
                    airType2.createdFromBoss(boss);
                } else {
                    AirEnemy air = (AirEnemy) enemy.GetComponent("AirEnemy");
                    air.createdFromBoss(boss);
                }
            }
        }

        void createTypeOfEnemiesByGameMode() {
            GameObject[] toChoose = boss.EnemyPrefabsNormal; // Modo normal.
            if (UserInterfaceGraphics.MODE_CHOOSE == 0) {
                toChoose = boss.EnemyPrefabsEasy; // Modo fácil.
            } else if (UserInterfaceGraphics.MODE_CHOOSE == 2) {
                toChoose = boss.EnemyPrefabsHard; // Modo difícil.
            }

            if (!("isDie").Equals(boss.getState())) {
                // Escoger aleatoriamente lado por el que saldrá el enemigo.
                int side = Random.Range(0, 2);
                float posX = (side==0) ? 
                    boss.gamecamera.transform.position.x + boss.widthCamera 
                    : boss.gamecamera.transform.position.x - boss.widthCamera;
                // Escoger aleatoriamente enemigo.
                int n = Random.Range(0, toChoose.Length);
            
                GameObject ePrefab = toChoose[n];
                GameObject enemy = GameObject.Instantiate<GameObject>(ePrefab);
                markAsFromBoss(enemy);
                Transform tEnemy = enemy.transform;
                bool isAirType2 = (enemy.GetComponent("AirEnemyType2") != null);
                if ((enemy.GetComponent("AirEnemy") != null) || isAirType2) {
                    // Si es un enemigo de aire colocar en la posición de la pantalla.
                    if (isAirType2 && (side==1)) {
                        // Como aparece del otro lado, cambiar la dirección de este enemigo.
                        tEnemy.localScale = new Vector3(-tEnemy.localScale.x, tEnemy.localScale.y, tEnemy.localScale.z);
                    }
                    tEnemy.position = new Vector3(posX, boss.transform.position.y, boss.transform.position.z);
                } else {
                    // Si es un enemigo de tierra, colocar en con y = Y_POS_FLOOR_ENEMY.
                    float offsetX = (side==0)? EnemyGenerator.X_OFFSET_FLOOR_ENEMY : -EnemyGenerator.X_OFFSET_FLOOR_ENEMY;
                    tEnemy.position = new Vector3(posX + offsetX, EnemyGenerator.Y_POS_FLOOR_ENEMY, boss.transform.position.z);
                }
            }

        }

        int numberOfEnemiesByGameMode() {
            int ret = NumberCallEnemiesModeNormal; // Modo normal.
            if (UserInterfaceGraphics.MODE_CHOOSE == 0) {
                ret = NumberCallEnemiesModeEasy; // Modo fácil.
            } else if (UserInterfaceGraphics.MODE_CHOOSE == 2) {
                ret = NumberCallEnemiesModeHard; // Modo difícil.
            }
            return ret;
        }
        
        protected override bool launchAction() {
            bool isEnding = false;
            currentTimeToGenerate = currentTimeToGenerate + Time.deltaTime;
            if (currentTimeToGenerate > EnemyGenerator.TIME_INTERVAL_CALL_ENEMIES) {
                createTypeOfEnemiesByGameMode();
                currentTimeToGenerate = 0;
                currentNumberOfEnemies--;
                if (currentNumberOfEnemies <= 0) {
                    isEnding = true;
                    currentNumberOfEnemies = numberOfEnemiesByGameMode();
                }
            }
            return isEnding;
        }
    }

    class FlyTypeZ : Strategy {
        private const float factorImpulseX = 20;
	    private const float factorImpulseY = 10;
	    private const float ceilingMargin = 2;

        private bool isInUp;
        private bool isInCorner;
        private bool isStepGravityEnable;
        private bool initGoToCorner;

        public FlyTypeZ(float times, BossGiantRobot boss) : base(times, boss) {
            isInUp = false;
            isInCorner = false;
            isStepGravityEnable = false;
            initGoToCorner = true;
        }
        
        bool flyUp() {
            bool isUp = false;
            if (!isStepGravityEnable) {
                // Asegurarse que el boss mira hacia el player.
                boss.putLookingToPlayer();
                // Cambiamos el estado al mismo que el de salto.
                boss.onlyChangeToJumpState();
                boss.setIsTouchFloor(false);
                // Desactivamos la gravedad.
                boss.enableGravity(false);
                isStepGravityEnable = true;
            }
            
            isUp = (boss.transform.position.y >= ceilingMargin + UserInterfaceGraphics.getHeightCamera());
            if (isUp) {
                // Paramos y reseteamos las variables utilizadas.
                boss.setVelocity(0, 0);
                isStepGravityEnable = false;
            } else {
                // Lanzamos impulso ascendente hacia el aliado.
                boss.setVelocity(boss.transform.localScale.x * factorImpulseX, factorImpulseY);
            }

            return isUp;
        }

        bool goToCorner() {
            bool isCorner = false;
            if (initGoToCorner) {
                // Asegurarse que el boss mira hacia el player.
                boss.putLookingToPlayer();
                initGoToCorner = false;
            }
            // Desplazarse hacia una esquina de la pantalla.
            boss.setVelocity(boss.transform.localScale.x * factorImpulseX/10f, 0);

            // Comprobar si ya hemos llegado a la esquina.
            float posBossX = boss.transform.position.x;
            float posCamX = boss.gamecamera.transform.position.x;
            isCorner = (posBossX >= (posCamX + boss.widthCamera)) || (posBossX <= (posCamX - boss.widthCamera + 0.3f));
            if (isCorner) {
                // Si hemos llegado reseteamos variables utilizadas.
                initGoToCorner = true;
            }
            return isCorner;
        }
        
        protected override bool launchAction() {
            bool isEnding = false;
            if (!isInUp) {
                isInUp = flyUp();
            } else if (!isInCorner) {
                isInCorner = goToCorner();
                if (isInCorner) {
                    // Activamos la gravedad.
                    boss.enableGravity(true);
                }
            } else if (boss.getIsInFloor()) {
                // Hemos acabado.
                isEnding = true;
                // Reseteamos variables.
                isInUp = false;
                isInCorner = false;
            }
            // Lanzar minas.
            if (!isInUp || !isInCorner) {
                boss.launchMines();
            }
            return isEnding;
        }
    }

    class AssaultStrategy : Strategy {
        private const float assaultOffset = 3f;
        private bool isInInitAssault;
        private bool isInAssaultSpace;

        public AssaultStrategy(float times, BossGiantRobot boss) : base(times, boss) {
            isInInitAssault = true;
            isInAssaultSpace = false;
        }

        protected override bool launchAction() {
            bool isEnding = false;
            // Realizo asalto.
            if (isInInitAssault) {
                boss.preAssault();
                isInInitAssault = false;
            }
            boss.launchAssault();
            // Comprobaciones.
            if (isInAssaultSpace) {
                // Comprobamos si hemos llegado al lugar donde detener el asalto.
                float posBossX = boss.transform.position.x;
                float posCamX = boss.gamecamera.transform.position.x;
                isEnding = (posBossX >= (posCamX + boss.widthCamera - assaultOffset)) 
                    || (posBossX <= (posCamX - boss.widthCamera + 0.3f + assaultOffset));
                if (isEnding) {
                    // Hacemos el postAssault y reiniciamos variables.
                    boss.postAssault();
                    isInAssaultSpace = false;
                    isInInitAssault = true;
                }
            } else {
                // Comprobamos si ya estamos dentro del espacio intermedio.
                float posBossX = boss.transform.position.x;
                float posCamX = boss.gamecamera.transform.position.x;
                isInAssaultSpace = (posBossX < (posCamX + boss.widthCamera - assaultOffset)) 
                    && (posBossX > (posCamX - boss.widthCamera + 0.3f + assaultOffset));
            }
            return isEnding;
        }
    }

}
