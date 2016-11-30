using UnityEngine;
using System.Collections;

public class ShowScoreAtGameEnding : MonoBehaviour {
    
    public TextMesh score;
    public TextMesh modeGame;

	// Use this for initialization
	void Start () {
        if (modeGame != null) {
            modeGame.text = "Difficulty Level: " + UserInterfaceGraphics.MODES[UserInterfaceGraphics.MODE_CHOOSE];
        }
	    score.text = "Score: " + Player.score;
	}
	
}
