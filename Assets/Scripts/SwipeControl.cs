using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SwipeControl : MonoBehaviour
{
    //variables for swipe input detection
    private Vector3 fp; //First finger position
    private Vector3 lp; //Last finger position
    private float dragDistance;  //Distance needed for a swipe to register

    //variables for determining the shot power and position
    public float power;  //power at which the ball is shot
    private Vector3 footballPos; //initial football position for replacing the ball at the same posiiton
    private float factor = 34f; // keep this factor constant, also used to determine force of shot

    public bool canShoot = true;  //flag to check if shot can be taken
    public int scorePlayer = 0;  //score of player
    public int scoreOpponent = 0; //score of oponent
    public int turn = 0;   //0 for striker, 1 for goalie
    public bool isGameOver = false; //flag for game over detection
    Vector3 oppKickDir;   //direction at which the ball is kicked by opponent
    public int shotsTaken = 0;  //number of rounds of penalties taken
    private bool returned = true;  //flag to check if the ball is returned to its initial position
    public bool isKickedPlayer = false; //flag to check if the player has kicked the ball
    public bool isKickedOpponent = false; //flag to check if the opponent has kicked the ball
    public bool triggered = false;
    public float fontSize = 27; //preferred fontsize for this screen size
    public int value = 20;  //factor value for changing fontsize if needed
    public GUISkin skin;

    void Start()
    {
        Time.timeScale = 1;    //set it to 1 on start so as to overcome the effects of restarting the game by script
        dragDistance = Screen.height * 20 / 100; //20% of the screen should be swiped to shoot
        Physics.gravity = new Vector3(0, -20, 0); //reset the gravity of the ball to 20
        footballPos = transform.position;  //store the initial position of the football
        fontSize = Mathf.Min(Screen.width, Screen.height) / value;
    }

    // Update is called once per frame
    void Update()
    {
        if (returned)
        {     //check if the football is in its initial position
            if (turn == 0 && !isGameOver)
            { //if its users turn to shoot and if the game is not over
                playerLogic();   //call the playerLogic fucntion
            }
            else if (turn == 1 && !isGameOver)
            { //if its opponent's turn to shoot
                opponentLogic(); //call the respective function
            }
        }



        //Logic to determine if the game is over
        if (shotsTaken >= 3)
        {
            //check if shots taken is greater than 5 and the scores are not equal
            if (shotsTaken >= 5 && scorePlayer != scoreOpponent)
            {
                isGameOver = true;    //set the game over flag to true
            }
            //check if the scores differ by a value of 3
            if ((scorePlayer == scoreOpponent + 3) || (scoreOpponent == scorePlayer + 3))
            {
                isGameOver = true;
            }


        }
    }
    void playerLogic()
    {
        //Examine the touch inputs
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fp = touch.position;
                lp = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                lp = touch.position;
                //First check if it's actually a drag

                if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
                {   //It's a drag

                    //x and y repesent force to be added in the x, y axes.
                    float x = (lp.x - fp.x) / Screen.height * factor;
                    float y = (lp.y - fp.y) / Screen.height * factor;
                    //Now check what direction the drag was
                    //First check which axis
                    if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
                    {   //If the horizontal movement is greater than the vertical movement...

                        if ((lp.x > fp.x) && canShoot)  //If the movement was to the right)
                        {   //Right move
                            GetComponent<Rigidbody>().AddForce((new Vector3(x, 10, 15)) * power);
                        }
                        else
                        {   //Left move
                            GetComponent<Rigidbody>().AddForce((new Vector3(x, 10, 15)) * power);
                        }
                    }
                    else
                    {   //the vertical movement is greater than the horizontal movement
                        if (lp.y > fp.y)  //If the movement was up
                        {   //Up move
                            GetComponent<Rigidbody>().AddForce((new Vector3(x, y, 15)) * power);
                        }
                        else
                        {   //Down move

                        }
                    }
                }
                canShoot = false;
                returned = false;
                isKickedPlayer = true;
                StartCoroutine(ReturnBall());
            }
            else
            {   //It's a tap

            }
        }
    }

    IEnumerator ReturnBall()
    {
        yield return new WaitForSeconds(5.0f);  //set a delay of 5 seconds before the ball is returned
        GetComponent<Rigidbody>().velocity = Vector3.zero;   //set the velocity of the ball to zero
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;  //set its angular vel to zero
        transform.position = footballPos;   //re positon it to initial position
                                            //take turns in shooting
        if (turn == 1)
            turn = 0;
        else if (turn == 0)
            turn = 1;
        canShoot = true;     //set the canshoot flag to true
        returned = true;     //set football returned flag to true as well
        triggered = false;
    }


    void opponentLogic()
    {
        //check for screen tap
        int fingerCount = 0;
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                fingerCount++;
        }
        //if tapped, the opponent will shoot the football after some time delay as mentioned below
        if (fingerCount > 0)
        {
            StartCoroutine(DelayAdd());  //add delay before the ball is shot
            isKickedOpponent = true;  //set opponent kicked to true
            shotsTaken++;    //increase set of penalty taken
            returned = false;
            StartCoroutine(ReturnBall()); //return the ball back to its initial position
        }
    }

    IEnumerator DelayAdd()
    {
        yield return new WaitForSeconds(0.2f);  //I have added a delay of 0.2 seconds
        oppKickDir = new Vector3(Random.Range(-4f, 4f), Random.Range(5f, 10f), Random.Range(6f, 12f));     //generate a random x and y value in the range mentioned
        GetComponent<Rigidbody>().AddForce(oppKickDir, ForceMode.Impulse); //add the force
    }

    //function to check if its a goal
    void OnTriggerEnter(Collider other)
    {
        //check if the football has triggered an object named GoalLine and triggered is not true
        if (other.gameObject.name == "GoalLine" && !triggered)
        {
            //if the scorer is the player
            if (turn == 0)
                scorePlayer++; //increment the goals tally of player
                               //if the scorer is the opponent
            else
                scoreOpponent++; //increment the goals tally of opponent

            triggered = true;       //check triggered to true

        }
    }

    void OnGUI()
    {
        GUI.skin = skin;   //use the custom GUISkin
        skin.button.fontSize = (int)fontSize; //set the fontsize of the button 
        skin.box.fontSize = (int)fontSize; //set the font size of box
        skin.label.fontSize = (int)fontSize;
        //check if game is not over, if so, display the score
        if (!isGameOver)
        {
            GUI.Label(new Rect(10, 10, Screen.width / 5, Screen.height / 6), "PLAYER: " + (scorePlayer).ToString()); //display player's score
            GUI.Label(new Rect(Screen.width - (Screen.width / 6), 10, Screen.width / 6, Screen.height / 6), "OPPONENT: " + (scoreOpponent.ToString()));   //display opponent's score
        }
        else
        {
            Time.timeScale = 0; //set the timescale to zero so as to stop the game world
                                //display the final score
            if (scorePlayer > scoreOpponent)
                GUI.Box(new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), "YOU WIN!\nFINAL SCORE: " + scorePlayer + " : " + scoreOpponent);
            else
                GUI.Box(new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), "YOU LOSE\nFINAL SCORE: " + scorePlayer + " : " + scoreOpponent);

            //restart the game on click
            if (GUI.Button(new Rect(Screen.width / 4 + 10, Screen.height / 4 + Screen.height / 10 + 10, Screen.width / 2 - 20, Screen.height / 10), "RESTART"))
            {
                Scene scene = SceneManager.GetActiveScene();
                SceneManager.LoadScene(scene.name);
            }

            //load the main menu, which as of now has not been created
            if (GUI.Button(new Rect(Screen.width / 4 + 10, Screen.height / 4 + 2 * Screen.height / 10 + 10, Screen.width / 2 - 20, Screen.height / 10), "MAIN MENU"))
            {
                SceneManager.LoadScene("MainMenu");
            }

            //exit the game
            if (GUI.Button(new Rect(Screen.width / 4 + 10, Screen.height / 4 + 3 * Screen.height / 10 + 10, Screen.width / 2 - 20, Screen.height / 10), "EXIT GAME"))
            {
                Application.Quit();
            }
        }
    }
}