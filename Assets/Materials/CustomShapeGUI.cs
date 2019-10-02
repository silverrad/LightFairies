using UnityEngine;  
using System.Collections;  
  
public class CustomShapeGUI : MonoBehaviour  
{  
    //a variable to store the GUISkin  
    public GUISkin guiskin;  
    //a variable to store the GUI camera  
    public Camera cShapeGUIcamera;  
  
    //a variable that is used to check if the mouse is over the button  
    private bool isHovering = false;  
  
    //a variable that is used to check if the mouse has been clicked  
    private bool isDown = false;  
  
    //a ray to be cast  
    private Ray ray;  
  
    //create a RaycastHit to query information about the collisions  
    private RaycastHit hit;  
  
    void Update()  
    {  
        //cast a ray based on the mouse position on the screen  
        ray = cShapeGUIcamera.ScreenPointToRay(Input.mousePosition);  
  
        //Check for raycast collisions with anything  
        if (Physics.Raycast(ray, out hit, 10))  
        {  
            //if the name of what we have collided is "irregular_shape"  
            if(hit.transform.name=="BTN_circleGUI")  
            {  
                //set collided variable to true  
                isHovering = true;  
  
                //if the mouse buton have been pressed while the cursor was over the button  
                if(Input.GetButton("Fire1"))  
                {  
                    //if clicked, mouse button is down  
                    isDown = true;  
                }  
                else  
                {  
                    //the mouse button have been released  
                    isDown = false;  
                }  
            }  
        }  
        else //ray is not colliding,  
        {  
            //set collided to false  
            isHovering = false;  
        }  
    }  
  
    void OnGUI()  
    {  
        //if mouse cursor is not inside the button area  
        if(!isHovering)  
        {  
            //draws the normal state  
            GUI.Label(new Rect(10,10,161,145),"",guiskin.customStyles[0]);  
  
            //set mouse click down to false  
            isDown = false;  
        }  
        else //mouse is inside the button area  
        {  
            //draws the hover state  
            GUI.Label(new Rect(10,10,161,145),"",guiskin.customStyles[1]);  
  
            //if the mouse has been clicked while the cursor was over the button  
            if(isDown)  
            {  
                //draws the 'Pressed' state  
                GUI.Label(new Rect(10,10,161,145),"",guiskin.customStyles[2]);  
            }  
        }  
    }  
}  