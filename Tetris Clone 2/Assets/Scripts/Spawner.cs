using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockColor{RANDOM, RED, BLUE, PURPLE, CYAN, GREEN, YELLOW }

public class Spawner : MonoBehaviour {

    [SerializeField]
    private GameObject[] Tetromino;

    // Use this for initialization
    void Start () {
        SpawnRandom(new Vector3 (0,0,0));
    }
	
	// Update is called once per frame
	void Update () {	
	}

    public void SpawnRandom(Vector3 pos) {
		int index = 0; 
		if(FindObjectOfType<Game>().currentScore >= 0 && FindObjectOfType<Game>().currentScore < 10 ){
			index = Random.Range(0, 2);
		}
		else if(FindObjectOfType<Game>().currentScore >= 10 && FindObjectOfType<Game>().currentScore < 30){
			index = Random.Range(0, 3);
		}
		else {
			index = Random.Range(0, System.Enum.GetValues(typeof(BlockColor)).Length - 1);
		}
        Instantiate(Tetromino[index], transform.position, Quaternion.identity);
    }

    // Spawn a block with a certain color
    public void Spawn(Vector3 pos, BlockColor color) {
    
        if (color != BlockColor.RED) {
            GameObject block = Instantiate(Tetromino[(int)color - 1], pos, Quaternion.identity);
        }
    }
}
