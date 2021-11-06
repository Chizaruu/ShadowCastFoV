using System.Collections;
using UnityEngine;

/// <summary> The GameManager is the main controller of the game. It handles the game state and the game flow. </summary>
public class GameManager : MonoBehaviour
{
	public static GameManager instance; //Singleton

	[SerializeField]private float time = 0.075f;	//Time between each frame

	[SerializeField]private bool isPlaying = false; //Is the game currently playing?
	[SerializeField]private bool diagonalMovement = true; //true = diagonal movement, false = no diagonal movement

    public bool IsPlaying { get => isPlaying; set => isPlaying = value; } //Is the game currently playing?
	public bool DiagonalMovement { get => diagonalMovement; set => diagonalMovement = value; } //true = diagonal movement, false = no diagonal movement

	/// <summary> Awake is called when the script instance is being loaded. </summary>
	private void Awake()
	{
		if (instance == null) //If instance is not assigned
		{
			instance = this; //Assign instance to this
		}
		else //else no need for this gameobject!
		{
			Destroy(gameObject); //Destroy this gameobject
		}
	} 
    
	/// <summary> This function is called when the player ends its turn. </summary>
	public void TurnChange()
	{
		isPlaying = true; //Set isPlaying to true
		StartCoroutine(waiting()); //Start waiting coroutine
	}

	/// <summary> Coroutine to wait for a certain amount of time. </summary>
	private IEnumerator waiting() 
	{
		yield return new WaitForSeconds(time); //Wait for time
		isPlaying = false; //Set isPlaying to false
	}
}