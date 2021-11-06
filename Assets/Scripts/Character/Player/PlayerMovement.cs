using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using SCF.Map;

namespace SCF.Character
{
    /// <summary> Player Movement. </summary>
    public class PlayerMovement : MonoBehaviour
    {
        private Vector3 futurevec; //future position

        private Controls controls; //input controls

        [SerializeField]private FOV fov;

        /// <summary> Initialize controls. </summary>
        private void Awake() => controls = new Controls();

        /// <summary> Start listening to input. </summary>
        private void OnEnable() => controls.Enable();

        /// <summary> Stop listening to input. </summary>
        private void OnDisable() => controls.Disable();

        #if UNITY_STANDALONE 
        /// <summary> Update player movement. </summary>
        private void FixedUpdate()
        {
            //Used for repeated 8-direction movement
            //PC, Linux, *Gasps* Mac
            
            //Arrowkeys
            var up = Keyboard.current.upArrowKey.IsPressed(); 
            var down = Keyboard.current.downArrowKey.IsPressed(); 
            var left = Keyboard.current.leftArrowKey.IsPressed(); 
            var right = Keyboard.current.rightArrowKey.IsPressed(); 

            //Used to stop all movement if 3 arrowkeys are pressed at once.
            if(up && down && left) return;
            if(up && down && right) return;
            if(up && left && right) return;
            if(down && left && right) return;

            //Player moves when an arrow key is pressed
            if(up || down || right || left)
            {
                //Invoke because we want to give some time to read values if we want diagonal movement.
                Invoke("Move", 0.1f);
            }
        }
        #endif

        /// <summary> Move player using normal input. </summary>
        private void Move()
        {
            Vector3 vec = (Vector3)controls.Player.Movement.ReadValue<Vector2>(); //read movement vector
            vec = new Vector3(Mathf.Round(vec.x), Mathf.Round(vec.y), Mathf.Round(vec.z)); //round vector to prevent bad movement
            
            futurevec = transform.position + vec;//future position
            if(GameManager.instance.IsPlaying) return; //don't move if game is paused

            if (!CanMove(vec)) return; //don't move if player can't move

            if(futurevec == transform.position) return; //don't move if next position is same as current position

            StartCoroutine(MoveTo(futurevec));
        }

        /// <summary> Check if player can move. </summary>
        private bool CanMove(Vector3 vec)
        {
            Vector3Int gridPosition = MapManager.instance.floorMap.WorldToCell(transform.position + vec); //grid position of player
            if (!MapManager.instance.floorMap.HasTile(gridPosition) || MapManager.instance.obstacleMap.HasTile(gridPosition))
                return false; //if player can't move, return false
            return true;//if player can move, return true
        }

        /// <summary> Move player to future position. </summary>
        private IEnumerator MoveTo(Vector3 end)
        {
            yield return null;
            transform.position = end;
            fov.RefreshFieldOfView();
            GameManager.instance.TurnChange();
        }
        
    } 
}


