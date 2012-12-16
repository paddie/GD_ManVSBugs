var play : int;
var level1 : int;
var level2 : int;
var level3 : int;
var level4 : int;
var level5 : int;

var help : int;

var exit : int;

var menu = "MAIN";

function OnGUI () {
	if (menu == "MAIN") {
		MainMenu();
	} else if (menu == "LEVELPICKER") {
		LevelPicker();
	} else if (menu == "SCORESCREEN") {
		ScoreScreen();
	}
}

function MainMenu() {
	GUILayout.BeginArea (Rect (Screen.width/2 -75, Screen.height/2 -75, 150, 150));
    if (GUILayout.Button ("Play")) {
        menu = "LEVELPICKER";
    }
    if (GUILayout.Button ("Exit")) {
        Application.Quit();
    }
    GUILayout.EndArea ();
}

function LevelPicker() {
	GUILayout.BeginArea (Rect (Screen.width/2 -75, Screen.height/2 -75, 150, 150));
    if (GUILayout.Button ("Planet 1")) {
        Application.LoadLevel (level1);
    }
    if (GUILayout.Button ("Planet 2")) {
        Application.LoadLevel (level2);
    }
    if (GUILayout.Button ("Planet 3")) {
        Application.LoadLevel (level3);
    }
    if (GUILayout.Button ("Planet 4")) {
        Application.LoadLevel (level4);
    }
    if (GUILayout.Button ("Planet 5")) {
        Application.LoadLevel (level5);
    }
    if (GUILayout.Button ("Back")) {
        menu = "MAIN";
    }
    GUILayout.EndArea ();
}

function ScoreScreen() {
	GUILayout.BeginArea (Rect (Screen.width/2 -75, Screen.height/2 -75, 150, 150));
    if (PlayerPrefs.GetInt("Victory") == 1) {
        GUILayout.Label("VICTORY!");
    } else if (PlayerPrefs.GetInt("Victory") == 0) {
    	GUILayout.Label("DEFEAT!");
    }
    if (GUILayout.Button ("Sector overview")) {
        menu = "LEVELPICKER";
    }
    
    GUILayout.EndArea ();
}