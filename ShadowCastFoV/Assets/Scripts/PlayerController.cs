using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
// From https://github.com/ChizaruuGCO/InputSystem-8DirectionMovement



public class PlayerController : MonoBehaviour
{
    //Tilemaps
    private Grid grid;
    private Tilemap floorTilemap;
    private Tilemap wallTilemap;
    
    //Input System
    private PlayerMovement controls;

    public Animator animator;

    private PlayerFov playerFov;
    private GameManager gameManager;

    private void Awake(){
        controls = new PlayerMovement();
    }
    
    private void OnEnable(){
        controls.Enable();
    }

    private void OnDisable(){
        controls.Disable();
    }

    void Start()
    {
        gameManager = GameManager.instance;
        grid = gameManager.grid;
        floorTilemap = gameManager.floorMap;
        wallTilemap = gameManager.wallMap;
        playerFov = PlayerFov.instance;
        StartCoroutine(Position());

        //Used for single turn by turn movement
        //controls.Main.Movement.performed += _ => Move();
    }

    void FixedUpdate(){
            //Used for repeated 8-direction movement
            //PC, Linux, *Gasps* Mac
            #if UNITY_STANDALONE 
            //Arrowkeys
            var up = Keyboard.current.upArrowKey.IsPressed();
            var down = Keyboard.current.downArrowKey.IsPressed();
            var left = Keyboard.current.leftArrowKey.IsPressed();
            var right = Keyboard.current.rightArrowKey.IsPressed();
            //Letterkeys
            var w = Keyboard.current.wKey.IsPressed();
            var s = Keyboard.current.sKey.IsPressed();
            var a = Keyboard.current.aKey.IsPressed();
            var d = Keyboard.current.dKey.IsPressed();

            //Used to stop all movement if 3 arrowkeys are pressed at once.
            if(up && down && left) return;
            if(up && down && right) return;
            if(up && left && right) return;
            if(down && left && right) return;

            //Used to stop all movement if 3 letterkeys are pressed at once.
            if(w && s && a) return;
            if(w && s && d) return;
            if(w && a && d) return;
            if(s && a && d) return;

            //Player moves when an arrow key is pressed
            if(up || down || right || left){
                //Invoke because we want to give some time to read values if we want diagonal movement.
                Invoke("Move", 0.1f);
            }

            //Player moves when an letter key is pressed
            if(w || s || d || a){
                //Invoke because we want to give some time to read values if we want diagonal movement.
                Invoke("Move", 0.1f);
            }
            #endif
        }

    IEnumerator Position(){
            yield return new WaitForSeconds(0.1f);
            Vector3Int lPos = grid.LocalToCell(this.transform.position);
            playerFov.PlayerVisibility(lPos);
        }

    private void Move(){
        Vector3 vec = (Vector3)controls.Main.Movement.ReadValue<Vector2>();
        vec = new Vector3(Mathf.Round(vec.x), Mathf.Round(vec.y), Mathf.Round(vec.z));

        Vector3 futurevec = transform.position + vec;
        if(gameManager.isMoving) return;
        //Checks if you can move.
        if (!CanMove(vec)) return;
        //Checks if next movement will be same position.
        if(futurevec == transform.position) return;
        animator.SetFloat("x", vec.x);
        animator.SetFloat("y", vec.y);
        transform.position += vec;
        gameManager.isMoving = true;
        StartCoroutine(Position());
        gameManager.TurnChange();
    }

    private bool CanMove(Vector3 vec){
        Vector3Int gridPosition = floorTilemap.WorldToCell(transform.position + vec);
        if (!floorTilemap.HasTile(gridPosition) || wallTilemap.HasTile(gridPosition))
            return false;
        return true;
    }
}
