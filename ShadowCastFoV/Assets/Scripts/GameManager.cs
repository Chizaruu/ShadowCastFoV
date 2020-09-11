using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	private PlayerController player;

	void Awake()
	{
		if(instance != null)
		{
			Debug.LogWarning("More than 1 instance of GameManager");
			return;
		}
		instance = this;
	}

	//use this Tilemap for all other scripts to access the tilemap

	public Grid grid;
	
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
	public Tilemap fogTilemap;

	public TileBase wall;

	public bool isMoving = false;

	public float time = 0.2f;

	// Use this for initialization
	void Start (){
		player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
	}

	public void TurnChange()
	{
			StartCoroutine(waiting());
			isMoving = true;
	}

	IEnumerator waiting()
	{
		yield return new WaitForSeconds(time);
		isMoving = false;
	}
}
