using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{

    float fall = 0;
    private float fallSpeed; //move one unit per second

    // smooth movement when pressing keys
    private float continuousVerticalSpeed = 0.03f; // the speed at which the block will move when the down button is held down
    private float continuousHorizontalSpeed = 0.1f; // the speed which the block will move when the left or right button are held down
    private float buttonDownWaitMax = 0.2f; // how long to wait bbefore the block recognize that a button is being held down

    private float verticalTimer = 0;
    private float horizontalTimer = 0;
    private float buttonDownWaitTimerHorizontal = 0;
    private float buttonDownWaitTimerVertical = 0;
    private bool movedImmediateHorizontal = false;
    private bool movedImmediateVertical = false;

    //public bool StopSpawner = false;
	public BlockColor colorOfNextSpawn = BlockColor.RANDOM;

    // Use this for initialization
    void Start() {
        fallSpeed = GameObject.Find("GameScript").GetComponent<Game>().fallSpeed;
    }

    // Update is called once per frame
    void Update() {
        CheckUserInput();
    }

    void CheckUserInput() {  
        
        //makes the move of the blocks smoother
        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow)) {
            movedImmediateHorizontal = false;
            horizontalTimer = 0;
            buttonDownWaitTimerHorizontal = 0;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow)) {
            movedImmediateVertical = false;
            verticalTimer = 0;
            buttonDownWaitTimerVertical = 0;
        }
        // right key
        if (Input.GetKey(KeyCode.RightArrow)) {
            MoveRight();   
        }
        // left key
        if (Input.GetKey(KeyCode.LeftArrow)) {
            MoveLeft();
        }
        // down key
        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallSpeed) {
            MoveDown();
        }
    }

    void MoveLeft () {
        if (movedImmediateHorizontal) {
            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax) {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }
            if (horizontalTimer < continuousHorizontalSpeed) {
                horizontalTimer += Time.deltaTime; //moves the block in steps
                return;
            }
        }
        if (!movedImmediateHorizontal)
            movedImmediateHorizontal = true;

        horizontalTimer = 0; // reset
        transform.position += new Vector3(-1, 0, 0);
        if (CheckIfValidPosition()) {
            FindObjectOfType<Game>().UpdateGrid(this);
        }
        else {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    void MoveRight() {
        if (movedImmediateHorizontal) {
            if (buttonDownWaitTimerHorizontal < buttonDownWaitMax) {
                buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }
            if (horizontalTimer < continuousHorizontalSpeed) {
                horizontalTimer += Time.deltaTime; //moves the block in steps
                return;
            }
        }
        if (!movedImmediateHorizontal)
            movedImmediateHorizontal = true;

        horizontalTimer = 0; // reset

        transform.position += new Vector3(1, 0, 0); // move block to the right
        if (CheckIfValidPosition()) {
            FindObjectOfType<Game>().UpdateGrid(this);
        }
        else {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    void MoveDown() {
        if (movedImmediateVertical) {
            if (buttonDownWaitTimerVertical < buttonDownWaitMax) {
                buttonDownWaitTimerVertical += Time.deltaTime;
                return;
            }
            if (verticalTimer < continuousVerticalSpeed) {
                verticalTimer += Time.deltaTime; //moves the block in steps
                return;
            }
        }
        if (!movedImmediateVertical)
            movedImmediateVertical = true;
        verticalTimer = 0; // reset

        transform.position += new Vector3(0, -1, 0);
        if (CheckIfValidPosition()) {
            FindObjectOfType<Game>().UpdateGrid(this);
        }
        else {
            transform.position += new Vector3(0, 1, 0);
            enabled = false; // stops the block from moving

            Vector2 pos = FindObjectOfType<Game>().Round(transform.position);

            // Check if block is above grid -> GameOver
            if (FindObjectOfType<Game>().CheckIfAboveGrid(pos) == true) { 
                FindObjectOfType<Game>().GameOver(); 
            }

            FindObjectOfType<Game>().changeColor(this);
            FindObjectOfType<Game>().consecutiveBlocks.Push(this);
            FindObjectOfType<Game>().CheckForConsecutiveBlocks(this);

            if (colorOfNextSpawn == BlockColor.RANDOM) {
                FindObjectOfType<Spawner>().SpawnRandom(FindObjectOfType<Game>().position);
            }
            else if (colorOfNextSpawn == BlockColor.PURPLE) {
                FindObjectOfType<Spawner>().Spawn(transform.position, BlockColor.PURPLE);
            }
            else if (colorOfNextSpawn == BlockColor.YELLOW) { 
                FindObjectOfType<Spawner>().Spawn(transform.position, BlockColor.YELLOW);
            }
            else if (colorOfNextSpawn == BlockColor.CYAN) {
                FindObjectOfType<Spawner>().Spawn(transform.position, BlockColor.CYAN);
            }
            FindObjectOfType<Game>().UpdateGrid(this);
        }
        fall = Time.time;
    }

    bool CheckIfValidPosition() {
        foreach (Transform mino in transform) {
            Vector2 pos = FindObjectOfType<Game>().Round(mino.position);
            
            // Check if block is inside grid
            if (FindObjectOfType<Game>().CheckIfInsideGrid(pos) == false) { 
                return false;
            }
            if (FindObjectOfType<Game>().GetTransformGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformGridPosition(pos).parent != transform) {
                return false;
            }
        }
        return true;
    }


}
  