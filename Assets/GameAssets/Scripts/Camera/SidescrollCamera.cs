using UnityEngine;
 
public class SidescrollCamera : MonoBehaviour
{
    public int LevelArea = 100;
 
    public int ScrollArea = 25;
    public int ScrollSpeed = 25;
    public int DragSpeed = 100;
 
    public int ZoomSpeed = 25;
    public int ZoomMin = 20;
    public int ZoomMax = 100;
 
    public int PanSpeed = 50;
    public int PanAngleMin = 25;
    public int PanAngleMax = 80;
	
	private int ZoomAdjust;
 
    // Update is called once per frame
    void Update()
    {
        // Init camera translation for this frame.
        var translation = Vector3.zero;
		var ZoomAdjust = camera.orthographicSize / 10;
 
        // Zoom in or out
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
			float amount = Input.GetAxis("Mouse ScrollWheel")*ZoomSpeed*10*Time.deltaTime;
			if (camera.orthographicSize - amount < ZoomMin ) {
				camera.orthographicSize = ZoomMin;
			} else if (camera.orthographicSize - amount > ZoomMax) {
				camera.orthographicSize = ZoomMax;
			} else {
				camera.orthographicSize -= amount;
			}
        }

 
        // Move camera with arrow keys
        translation += new Vector3(Input.GetAxis("Horizontal")*ZoomAdjust, 0, Input.GetAxis("Vertical")*ZoomAdjust);
 
		/* CONFLICTS WITH SELECTION TOOL!
        // Move camera with mouse
        if (Input.GetMouseButton(1)) // MMB
        {
            // Hold button and drag camera around
            translation -= new Vector3((Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y"))  * ZoomAdjust * DragSpeed * Time.deltaTime, 0, 
                               (Input.GetAxis("Mouse Y") - Input.GetAxis("Mouse X")) * ZoomAdjust * DragSpeed * Time.deltaTime);
        }
        else
        {
            // DISABLED FOR DEVELOPMENT ONLY!
			// Move camera if mouse pointer reaches screen borders
            if (Input.mousePosition.x < ScrollArea)
            {
                translation += new Vector3(-ScrollSpeed * ZoomAdjust * Time.deltaTime,0,ScrollSpeed * ZoomAdjust * Time.deltaTime);
            }
 
            if (Input.mousePosition.x >= Screen.width - ScrollArea)
            {
                translation += new Vector3(ScrollSpeed * ZoomAdjust * Time.deltaTime,0,-ScrollSpeed * ZoomAdjust * Time.deltaTime);
            }
 
            if (Input.mousePosition.y < ScrollArea)
            {
                translation += new Vector3(-ScrollSpeed * ZoomAdjust * Time.deltaTime,0, -ScrollSpeed * ZoomAdjust * Time.deltaTime);
            }
 
            if (Input.mousePosition.y > Screen.height - ScrollArea)
            {
                translation += new Vector3(ScrollSpeed * ZoomAdjust * Time.deltaTime,0,ScrollSpeed * ZoomAdjust * Time.deltaTime);
            }
        }
		*/
 
        // Keep camera within level and zoom area

 
        // Finally move camera parallel to world axis
        camera.transform.position += translation;
    }
}