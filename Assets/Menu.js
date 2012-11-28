var play : int;

var help : int;

var exit : int;

 

function OnGUI () {

	GUILayout.BeginArea (Rect (Screen.width/2 -75, Screen.height/2 -75, 150, 150));
	
    if (GUILayout.Button ("Play")) {

        Application.LoadLevel (play);

    }

    if (GUILayout.Button ("Exit")) {

        Application.Quit();

    }
    GUILayout.EndArea ();

}