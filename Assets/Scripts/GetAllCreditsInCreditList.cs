using UnityEngine;
using System.Collections;

public class GetAllCreditsInCreditList : MonoBehaviour {
    
    public TextMesh[] textsInfoPos1; //{nick, modeGame, time, punctuation}
    public TextMesh[] textsInfoPos2;
    public TextMesh[] textsInfoPos3;
    public TextMesh[] textsInfoPos4;
    public TextMesh[] textsInfoPos5;
    public TextMesh[] textsInfoPos6;
    public TextMesh[] textsInfoPos7;
    public TextMesh[] textsInfoPos8;
    public TextMesh[] textsInfoPos9;
    public TextMesh[] textsInfoPos10;

	void Start () {
	    SaveCreditInCreditList.PlayerInformation[] allPlayers = SaveCreditInCreditList.PlayerInformation.getPodium();
        // Escribimos ordenadamente toda la información del podium en la tabla.
        // Fila 1.
        if (allPlayers[0] != null) {
            textsInfoPos1[0].text = SaveCreditInCreditList.PlayerInformation.getScreenNickName(allPlayers[0].pNick);
            textsInfoPos1[1].text = UserInterfaceGraphics.MODES[allPlayers[0].pModeGame];
            textsInfoPos1[2].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[0]);
            textsInfoPos1[3].text = "" + allPlayers[0].pTime;
            textsInfoPos1[4].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[0]);
        }
        // Fila 2.
        if (allPlayers[1] != null) {
            textsInfoPos2[0].text = SaveCreditInCreditList.PlayerInformation.getScreenNickName(allPlayers[1].pNick);
            textsInfoPos2[1].text = UserInterfaceGraphics.MODES[allPlayers[1].pModeGame];
            textsInfoPos2[2].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[1]);
            textsInfoPos2[3].text = "" + allPlayers[1].pTime;
            textsInfoPos2[4].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[1]);
        }
        // Fila 3.
        if (allPlayers[2] != null) {
            textsInfoPos3[0].text = SaveCreditInCreditList.PlayerInformation.getScreenNickName(allPlayers[2].pNick);
            textsInfoPos3[1].text = UserInterfaceGraphics.MODES[allPlayers[2].pModeGame];
            textsInfoPos3[2].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[2]);
            textsInfoPos3[3].text = "" + allPlayers[2].pTime;
            textsInfoPos3[4].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[2]);
        }
        // Fila 4.
        if (allPlayers[3] != null) {
            textsInfoPos4[0].text = SaveCreditInCreditList.PlayerInformation.getScreenNickName(allPlayers[3].pNick);
            textsInfoPos4[1].text = UserInterfaceGraphics.MODES[allPlayers[3].pModeGame];
            textsInfoPos4[2].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[3]);
            textsInfoPos4[3].text = "" + allPlayers[3].pTime;
            textsInfoPos4[4].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[3]);
        }
        // Fila 5.
        if (allPlayers[4] != null) {
            textsInfoPos5[0].text = SaveCreditInCreditList.PlayerInformation.getScreenNickName(allPlayers[4].pNick);
            textsInfoPos5[1].text = UserInterfaceGraphics.MODES[allPlayers[4].pModeGame];
            textsInfoPos5[2].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[4]);
            textsInfoPos5[3].text = "" + allPlayers[4].pTime;
            textsInfoPos5[4].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[4]);
        }
        // Fila 6.
        if (allPlayers[5] != null) {
            textsInfoPos6[0].text = SaveCreditInCreditList.PlayerInformation.getScreenNickName(allPlayers[5].pNick);
            textsInfoPos6[1].text = UserInterfaceGraphics.MODES[allPlayers[5].pModeGame];
            textsInfoPos6[2].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[5]);
            textsInfoPos6[3].text = "" + allPlayers[5].pTime;
            textsInfoPos6[4].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[5]);
        }
        // Fila 7.
        if (allPlayers[6] != null) {
            textsInfoPos7[0].text = SaveCreditInCreditList.PlayerInformation.getScreenNickName(allPlayers[6].pNick);
            textsInfoPos7[1].text = UserInterfaceGraphics.MODES[allPlayers[6].pModeGame];
            textsInfoPos7[2].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[6]);
            textsInfoPos7[3].text = "" + allPlayers[6].pTime;
            textsInfoPos7[4].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[6]);
        }
        // Fila 8.
        if (allPlayers[7] != null) {
            textsInfoPos8[0].text =SaveCreditInCreditList.PlayerInformation.getScreenNickName( allPlayers[7].pNick);
            textsInfoPos8[1].text = UserInterfaceGraphics.MODES[allPlayers[7].pModeGame];
            textsInfoPos8[2].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[7]);
            textsInfoPos8[3].text = "" + allPlayers[7].pTime;
            textsInfoPos8[4].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[7]);
        }
        // Fila 9.
        if (allPlayers[8] != null) {
            textsInfoPos9[0].text = SaveCreditInCreditList.PlayerInformation.getScreenNickName(allPlayers[8].pNick);
            textsInfoPos9[1].text = UserInterfaceGraphics.MODES[allPlayers[8].pModeGame];
            textsInfoPos9[2].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[8]);
            textsInfoPos9[3].text = "" + allPlayers[8].pTime;
            textsInfoPos9[4].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[8]);
        }
        // Fila 10.
        if (allPlayers[9] != null) {
            textsInfoPos10[0].text = SaveCreditInCreditList.PlayerInformation.getScreenNickName(allPlayers[9].pNick);
            textsInfoPos10[1].text = UserInterfaceGraphics.MODES[allPlayers[9].pModeGame];
            textsInfoPos10[2].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[9]);
            textsInfoPos10[3].text = "" + allPlayers[9].pTime;
            textsInfoPos10[4].text = "" + SaveCreditInCreditList.PlayerInformation.punctuationPlayer(allPlayers[9]);
        }
	}
	
    
	void Update () {
        //PlayerPrefs.DeleteAll(); Debug.Log("SCORE LIST RESETEADA");
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K) 
                || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton0)) {
            // Si ha pulsado la F, J, K, o la tecla enter, pasar a la pantalla 1.
            Application.LoadLevel(1);
        }
	}
}
