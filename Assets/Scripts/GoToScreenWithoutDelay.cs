using UnityEngine;
using System.Collections;

public class GoToScreenWithoutDelay : MonoBehaviour {
    
    public int goToScreen = 1;
	
	void Start () {
	    Application.LoadLevel(goToScreen);
	}
}
