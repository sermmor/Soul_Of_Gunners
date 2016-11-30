using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SaveCreditInCreditList : MonoBehaviour {
    
    public InputField nameInputField;

    private PlayerInformation info;
    private int totalPoints;
    
	void Start () {
        info = new PlayerInformation("", (int) Player.score, UserInterfaceGraphics.MODE_CHOOSE, (int) Player.timeGame);

        totalPoints = (int) PlayerInformation.punctuationPlayer(info);
	}

    public void SaveScorePlayer(string nameNick) {
        info.pNick = nameNick;
        // Comprobar si existen las variables pos1, pos2,..., pos20, y si no existen, crearlas.
        if (!PlayerPrefs.HasKey("pos1")) {
            // Si no existe la primera posición, no existe ninguna.
            PlayerInformation.createTablePositions();
            // Colocar al player en la primera posición.
            PlayerInformation.setPlayerInPositionInPodium(1, info);
        } else {
            /* Comprobar si está (por tiempo, score o modo) entre uno de los 10 (OJO CUIDAO borrar 
               antes el nick que estaba en la posición 10). Si no está, no se guarda. */
            int posPodium = PlayerInformation.isInPodium(info);
            if (posPodium != -1) {
                PlayerInformation.setPlayerInPositionInPodium(posPodium, info);
            }
        }
    }
    
	void Update () {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0)) {
            // Si ha pulsado la tecla enter.
            if (nameInputField.text != null && !("").Equals(nameInputField.text)) {
                // Confirmar el nick y guardar score en caso de que esté en el podium.
                SaveScorePlayer(nameInputField.text);
                // Pasar a la pantalla de scores.
                Application.LoadLevel(UserInterfaceGraphics.NUMBER_SCREEN_CREDITS_TABLE);
            }
        }
	}

    // -------------------------------------- Manejo de la información relativa a los puestos del score guardados.

    public class PlayerInformation {
        public string pNick;
        public int pScore, pModeGame, pTime;
        public PlayerInformation(string pNick, int pScore, int pModeGame, int pTime) {
            this.pNick = pNick;
            this.pScore = pScore;
            this.pModeGame = pModeGame;
            this.pTime = pTime;
        }
        
        // DECODIFICA.
        private static PlayerInformation getDecodifyPlayer(string pNickName, string playerInfo) {
            PlayerInformation ret = null;
            string[] str = playerInfo.Split(new string[] {UserInterfaceGraphics.MODES[0]}, System.StringSplitOptions.None);
            if (str == null || str.Length <2) {
                // Es modo normal o difícil.
                str = playerInfo.Split(new string[] {UserInterfaceGraphics.MODES[1]}, System.StringSplitOptions.None);
                if (str == null || str.Length <2) {
                    // Es modo difícil.
                    str = playerInfo.Split(new string[] {UserInterfaceGraphics.MODES[2]}, System.StringSplitOptions.None);
                    if (str == null || str.Length <2) {
                        return null; // CASO DE ERROR.
                    } else {
                        ret = new PlayerInformation(pNickName, System.Int32.Parse(str[0]), 2, System.Int32.Parse(str[1]));
                    }
                } else {
                    ret = new PlayerInformation(pNickName, System.Int32.Parse(str[0]), 1, System.Int32.Parse(str[1]));
                }
            } else {
                ret = new PlayerInformation(pNickName, System.Int32.Parse(str[0]), 0, System.Int32.Parse(str[1]));
            }
            return ret;
        }

        // CODIFICA.
        private static string codifyPlayer(PlayerInformation playerInfo) {
            return "" + playerInfo.pScore + UserInterfaceGraphics.MODES[playerInfo.pModeGame] + playerInfo.pTime;
        }

        // Útiles.
        // Devuelve el nick que se muestra por pantalla, no el que se guarda.
        public static string getScreenNickName(string nick) {
            string realNick = nick;
            while (realNick.EndsWith("~!")) {
                realNick = realNick.Substring(0, realNick.Length - 2);
            }
            return realNick;
        }
        
        public static PlayerInformation getPlayerByNickName(string nick) {
            string allCodifyInfo = PlayerPrefs.GetString(nick);
            return getDecodifyPlayer(nick, allCodifyInfo);
        }

        public static PlayerInformation getPlayerByPositionInPodium(int positionIn) {
            return getPlayerByNickName(PlayerPrefs.GetString("pos" + positionIn));
        }

        public static void setPlayerInPositionInPodium(int positionIn, PlayerInformation p) {
            string allCodifyInfo = codifyPlayer(p);
            string pNickName = p.pNick;
            // Compruebo si ya existe el nick (si existe, añadir ~! al final). 
            while (PlayerPrefs.HasKey(pNickName)) {
                pNickName = pNickName + "~!";
            }

            // Borro el player que está en la última posición (10) y desplazo a todos los players más abajo de la posición positionIn.
            if (PlayerPrefs.HasKey("pos10")) {
                PlayerPrefs.DeleteKey(PlayerPrefs.GetString("pos10"));
            }
            
            for (int i = 9; i >=positionIn; i--) {
                string nickToDownPosition = PlayerPrefs.GetString("pos"+i);
                PlayerPrefs.SetString("pos"+(i+1), nickToDownPosition);
            }

            // Colocamos el player en la posición ya libre.
            PlayerPrefs.SetString("pos" + positionIn, pNickName);
            PlayerPrefs.SetString(pNickName, allCodifyInfo);
        }

        // Devuelve la puntuación real de la formula.
        public static int punctuationPlayer(PlayerInformation p) {
            const int scoreInBeginningLevel2 = 174; // Puntos que se obtienen al llegar al nivel 2.
            int allPoints = p.pScore;
            if ((p.pModeGame == 1 || p.pModeGame == 2) && (p.pScore >= scoreInBeginningLevel2)) {
                /* En el caso del nivel medio o difícil, añadimos un plus por el nivel si ha llegado al nivel 2 
                   ya que es donde comienzan a diferenciarse los niveles. */
                allPoints = allPoints + (1000 * p.pModeGame);
            }
            // Si se pasó el juego (el boss son 5000 puntos), se da una bonificación por el tiempo sobrante.
            if (p.pScore > 5000) {
                allPoints = allPoints + (p.pTime * 2);
            }
            return allPoints;
        }

        // Dado un PlayerInformation devuelve si entraría (posición entre 1 y 10) o no en el podium (posición -1).
        public static int isInPodium(PlayerInformation p) {
            int pPoints = punctuationPlayer(p);
            int counter = 1;
            while (counter <= 10 && !(pPoints > punctuationPlayer(getPlayerByPositionInPodium(counter)))) {
                counter++;
            }
            return (counter <= 10) ? counter : -1;
        }

        // Con la propia información del juego, calculamos si está el player en el podium.
        public static int isInPodium() {
            return isInPodium(new PlayerInformation("", (int) Player.score, UserInterfaceGraphics.MODE_CHOOSE, (int) Player.timeGame));
        }

        // Devuelve un array de PlayerInformation que contiene todo el podium.
        public static PlayerInformation[] getPodium() {
            PlayerInformation[] pi = new PlayerInformation[10];
            for (int i = 0; i < 10; i++) {
                pi[i] = getPlayerByPositionInPodium(i+1);
            }
            return pi;
        }

        private static void setPlayerInPositionVoidInPodium(int positionIn, PlayerInformation p) {
            string allCodifyInfo = codifyPlayer(p);
            string pNickName = p.pNick;
            // Compruebo si ya existe el nick (si existe, añadir ~! al final).
            while (PlayerPrefs.HasKey(pNickName)) {
                pNickName = pNickName + "~!";
            }

            // Colocamos el player en la posición ya libre.
            PlayerPrefs.SetString("pos" + positionIn, pNickName);
            PlayerPrefs.SetString(pNickName, allCodifyInfo);
        }

        // En caso de que no exista la tabla, se guarda una tabla predeterminada en el sistema.
        public static void createTablePositions() {
            for (int i = 1; i <= 10; i++) {
                setPlayerInPositionVoidInPodium(i, new PlayerInformation("AAAAAA", 0, 0, 1000));
            }
        }
    }
}
