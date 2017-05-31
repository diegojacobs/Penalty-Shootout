using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    public GUISkin myskin;   //custom GUIskin reference  
    float virtualWidth = 960.0f; //width of the device you're using
    float virtualHeight = 540.0f; //height of the device
    public float fontSize = 27; //preferred fontsize for this screen size
    public int value = 20;  //factor value for changing fontsize if needed

    private void Start()
    {
        //check if the size on which game is being played is different
        if (virtualWidth != Screen.width || virtualHeight != Screen.height)
        {
            //set the new screen sizes if different
            virtualWidth = Screen.width;
            virtualHeight = Screen.height;
            //screen size dependent font size calculation
            fontSize = Mathf.Min(Screen.width, Screen.height) / value;
        }
    }


      private void OnGUI()
    {
        GUI.skin = myskin;   //use the custom GUISkin
        myskin.button.fontSize = (int)fontSize; //set the fontsize of the button 
        myskin.box.fontSize = (int)fontSize; //set the font size of box

        //create a menu
        GUI.Box(new Rect(Screen.width / 8, 10, 3 * Screen.width / 4, 3 * Screen.height / 4), "MAIN MENU"); //a box to hold all the buttons

        if (GUI.Button(new Rect(Screen.width / 8, Screen.height / 8 + 10, 3 * Screen.width / 4, Screen.height / 8), "PLAY 3D"))
        {
            SceneManager.LoadScene("Penalty Kick Game"); //open the game scene
        }

        if (GUI.Button(new Rect(Screen.width / 8, 2 * Screen.height / 8 + 10, 3 * Screen.width / 4, Screen.height / 8), "PLAY AR"))
        {
            SceneManager.LoadScene("Penalty Kick Game AR");  // open the credits scene
        }

        if (GUI.Button(new Rect(Screen.width / 8, 3 * Screen.height / 8 + 10, 3 * Screen.width / 4, Screen.height / 8), "EXIT"))
        {
            Application.Quit(); // exit the game
        }
    }

}