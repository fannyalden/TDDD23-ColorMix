using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // get access to text type object in Unity

public class Game : MonoBehaviour
{

    public static int gridWidth = 5;
    public static int gridHeight = 16;
    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    [SerializeField]
    public GameObject[] Tetromino;

    public Vector2 position;
    static int consecBlock = 5;
    public float fallSpeed = 1.0f;

    // Minos can be at position 0,0
    public Stack<Tetromino> consecutiveBlocks = new Stack<Tetromino>(); //connected blocks
    public Queue<Tetromino> blocksToCheck = new Queue<Tetromino>(); //blocks to check neighbours

    // scoring variables
    public int score = 0;
    public Text hud_score;
    // level variables
    public int currentScore = 0;
    public int currentLevel = 0;
    public int counter = 100;
    // high score variables
    private int startingHighScore1;
    private int startingHighScore2;
    private int startingHighScore3;

    // sound
    public AudioClip scoreSound; // used when scoring
    public AudioClip plopSound; //used when blocks are destroyed
    public AudioClip LevelUpSound;
    private AudioSource audioSource;

    // Use this for initialization
    void Start() {
        audioSource = GetComponent<AudioSource>();
        startingHighScore1 = PlayerPrefs.GetInt("highscore1");
        startingHighScore2 = PlayerPrefs.GetInt("highscore2");
        startingHighScore3 = PlayerPrefs.GetInt("highscore3");
    }

    void Update() {
        //UpdateLevel();
        UpdateSpeed();
    }

    void UpdateSpeed() {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    public void DeleteBlockAt(int x, int y) {
        Destroy(grid[x, y].gameObject);
        grid[x, y] = null;
    }

    public void MoveBlockOneStepDown(int x, int y) {
        grid[x, y - 1] = grid[x, y];
        grid[x, y] = null;
        grid[x, y - 1].parent.position += new Vector3(0, -1, 0); // update actual position
    }

    public void MoveBlocksDown() {
        for (int y = 0; y < gridHeight; ++y) {
            for (int x = 0; x < gridWidth; ++x) {
                if (grid[x, y] != null) {
                    int tempy = y;
                    if (CheckIfInsideGrid(new Vector2(x, tempy - 1))) {
                        while (grid[x, tempy - 1] == null) {
                            MoveBlockOneStepDown(x, tempy);
                            if (tempy == 1) {
                                break;
                            }
                            --tempy;
                        }
                    }
                }
            }
        }
    }

    // called from Tetromino.cs when a key is pressed, updates the position for the mino/block
    public void UpdateGrid(Tetromino tetromino) {
        for (int y = 0; y < gridHeight; ++y) {
            for (int x = 0; x < gridWidth; ++x) {
                if (grid[x, y] != null) {
                    if (grid[x, y].parent == tetromino.transform) {
                        grid[x, y] = null; //update grid
                    }
                }
            }
        }
        foreach (Transform mino in tetromino.transform) {
            Vector2 pos = Round(mino.position);
            if (pos.y < gridHeight) {
                grid[(int)pos.x, (int)pos.y] = mino; //update grid
            }
        }
    }

    // called from Tetromino.cs to check if the block has a valid position
    public Transform GetTransformGridPosition(Vector2 pos) {
        if (pos.y > gridHeight - 1) {
            return null;
        }
        else {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    public bool CheckIfAboveGrid(Vector2 pos) {
        return (pos.y > gridHeight - 3);
    }

    public bool CheckIfInsideGrid(Vector2 pos) {
		return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0 && !(pos.y > gridHeight-1)); // return true if everything in the parenteses are true
    }

    // round a float to an integer
    public Vector2 Round(Vector2 pos) {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

	// Change color on block
	public void changeColor(Tetromino block){
		Vector2 blockPos = new Vector2((int)block.transform.position.x, (int)block.transform.position.y-1);
		if (CheckIfInsideGrid (blockPos)) {
			if (grid [(int)blockPos.x, (int)blockPos.y] != null) {
				if (!(block.CompareTag (grid [(int)block.transform.position.x, (int)block.transform.position.y - 1].parent.tag))) {
					if (block.tag == "BlueBlock" && grid [(int)block.transform.position.x, (int)block.transform.position.y - 1].parent.tag == "RedBlock"
						|| block.tag == "RedBlock" && grid [(int)block.transform.position.x, (int)block.transform.position.y - 1].parent.tag == "BlueBlock") {
						block.colorOfNextSpawn = BlockColor.PURPLE;
						DeleteBlockAt ((int)block.transform.position.x, (int)block.transform.position.y);
					}
                    else if (block.tag == "RedBlock" && grid[(int)block.transform.position.x, (int)block.transform.position.y - 1].parent.tag == "GreenBlock"
                        || block.tag == "GreenBlock" && grid[(int)block.transform.position.x, (int)block.transform.position.y - 1].parent.tag == "RedBlock") {
                        block.colorOfNextSpawn = BlockColor.YELLOW;
                        DeleteBlockAt((int)block.transform.position.x, (int)block.transform.position.y);
                    }
                    else if (block.tag == "GreenBlock" && grid[(int)block.transform.position.x, (int)block.transform.position.y - 1].parent.tag == "BlueBlock"
                        || block.tag == "BlueBlock" && grid[(int)block.transform.position.x, (int)block.transform.position.y - 1].parent.tag == "GreenBlock") {
                        block.colorOfNextSpawn = BlockColor.CYAN;
                        DeleteBlockAt((int)block.transform.position.x, (int)block.transform.position.y);
                    }
                }
			}
		}
	}

	// Check neighbours if same color put in consecutiveBlocks and blocksToCheck
	void CheckNeighbours (Tetromino block, int x, int y){
		Tetromino neighbour;

		Vector2 blockPos = new Vector2((int)x, (int)y);
		if (CheckIfInsideGrid (blockPos)) {
			if (grid [(int)blockPos.x, (int)blockPos.y] != null) { // check if position is not empty
				neighbour = grid [(int)blockPos.x, (int)blockPos.y].parent.gameObject.GetComponent<Tetromino> ();
				if (block.CompareTag (grid [(int)blockPos.x, (int)blockPos.y].parent.tag)) { // check if the type (color) is the same
					if (!consecutiveBlocks.Contains (neighbour)) { // check if already in the list
						blocksToCheck.Enqueue (neighbour);
						consecutiveBlocks.Push (neighbour);
					}
				} 
			}
		}
	}

    // Check if the neighbours of the block is the same type (color)
    public void CheckForConsecutiveBlocks(Tetromino block) {

		CheckNeighbours(block, (int)block.transform.position.x, (int)block.transform.position.y + 1);		// Up-position
		CheckNeighbours(block, (int)block.transform.position.x, (int)block.transform.position.y - 1);		// Down-position
		CheckNeighbours(block, (int)block.transform.position.x + 1, (int)block.transform.position.y);		// Right-position  
		CheckNeighbours(block, (int)block.transform.position.x - 1, (int)block.transform.position.y);		// Left-position

		if (blocksToCheck.Count > 0) {
			Tetromino nextBlock = blocksToCheck.Dequeue();
			CheckForConsecutiveBlocks(nextBlock);
		}
        else {	
			if (consecutiveBlocks.Count > consecBlock) {
                currentScore += (consecutiveBlocks.Count * consecutiveBlocks.Count) - 30;
                RemoveConsecutiveBlocks ();
            }
			consecutiveBlocks.Clear ();
			blocksToCheck.Clear ();    
        }
    }

	void RemoveConsecutiveBlocks(){
		//PlayPlopAudio();

        while (consecutiveBlocks.Count > 0) {
			Tetromino block = consecutiveBlocks.Pop ();
            
			DeleteBlockAt ((int)block.transform.position.x, (int)block.transform.position.y);
		}

        MoveBlocksDown ();
		RemoveBlocksSecondary ();
        PlayScoreAudio();
		currentScore += score;
 
        // level up
        if (currentScore > counter) {
            PlayLevelUpAudio();
            currentLevel = currentLevel + 1;
            counter = counter + 100;
        }
        hud_score.text = currentScore.ToString(); //a text object must be a String
	}

    public void UpdateHighScore() {
        if (currentScore > startingHighScore1) {
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", startingHighScore1);
            PlayerPrefs.SetInt("highscore1", currentScore);
        }
        else if (currentScore > startingHighScore2){
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", currentScore);
        }
        else if (currentScore > startingHighScore3){
            PlayerPrefs.SetInt("highscore3", currentScore);
        }
    }

	// Remove blocks that has been moved after underlying blocks have been removed
	void RemoveBlocksSecondary(){
		for (int y = 0; y < gridHeight; ++y) {
			for (int x = 0; x < gridWidth; ++x) {
				Vector2 blockPos = new Vector2((int)x, (int)y);
				Tetromino block;

				if (CheckIfInsideGrid (blockPos)) {
					if (grid [x, y] != null) { // check if position is not empty
						block = grid [x, y].parent.gameObject.GetComponent<Tetromino> ();
						CheckForConsecutiveBlocks (block);
					}
				}
			}
		}
	}

    void PlayPlopAudio() {
        audioSource.PlayOneShot(plopSound);
    }

    void PlayLevelUpAudio() {
        audioSource.PlayOneShot(LevelUpSound);
    }

    void PlayScoreAudio() {
        audioSource.PlayOneShot(scoreSound);
    }

    public void GameOver() {
        if(currentScore > startingHighScore3) { // new highscore
            FindObjectOfType<Game>().UpdateHighScore();
            FindObjectOfType<LevelController>().NewHighScore();
        } else
            SceneManager.LoadScene("GameOver");
    }
}