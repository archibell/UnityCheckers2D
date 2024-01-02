using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Piece : MonoBehaviour
{
    // Variable declarations.
    Rigidbody2D rigidbody2d;
    Mouse mouse;    // Class Mouse >> An input device representing a mouse. 
                    // Putting mouse here allows other method calls to call on it once the mouse has been established.
    Camera cam;
    public Player player; // `PlayerDark` or `PlayerLight` is assigned to Pieces in Unity UI.
    Vector2 position2dOnMouseDown;
    Board board;
    public bool amIQueen { get; set; } // A reference to a piece that becomes a Queen, which is set by the Board class.
    public Sprite queenSprite; // `PlayerDarkQueen` or `PlayerLightQueen` is assigned to Piece component in Unity UI.

    // Start is called before the first frame update.
    void Start()
    {
        // Variable initializations.
        rigidbody2d = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        board = GameObject.Find("Board").GetComponent<Board>();
    }

    // Update is called once per frame.
    void Update()
    {
        
    }

    // OnMouseDown is called when the user has pressed the mouse button while over the Collider.
    // This function is called on Colliders and 2D Colliders marked as trigger.
    void OnMouseDown()
    {
        if (board.attackingPiece != null) // If there is an attackingPiece
        {
            if (board.attackingPiece != this) // and this piece is not that one, then return early.
            {
                return;
            }
        }
        if (board.playerTurn != player) // If it's not my turn, then return early.
        {
            return;
        }
        mouse = Mouse.current;  // Initialize mouse.
                                // Mouse.current >> The mouse that was added or updated last or null if there is no mouse connected to the system.                                
        position2dOnMouseDown = SnapPositionToGrid(PixelToWorldandV3ToV2(mouse));
        rigidbody2d.MovePosition(position2dOnMouseDown);
    }

    // OnMouseDrag is called when the user has clicked on a Collider and is still holding down the mouse.
    void OnMouseDrag()
    {
        if (board.attackingPiece != null) // If there is an attackingPiece
        {
            if (board.attackingPiece != this) // and this piece is not that one, then return early.
            {
                return;
            }
        }
        if (board.playerTurn != player) // If it's not my turn then return early.
        {
            return;
        }
        Vector2 position2d = PixelToWorldandV3ToV2(mouse);
        rigidbody2d.MovePosition(position2d);
        
    }

    // OnMouseUpAsButton is only called when the mouse is released over the same Collider as it was pressed.
    void OnMouseUpAsButton()
    {
        if (board.attackingPiece != null) // If there is an attackingPiece
        {
            if (board.attackingPiece != this) // and this piece is not that one, then return early.
            {
                return;
            }
        }
        if (board.playerTurn != player) // If it's not my turn then return early.
        {
            return;
        }
        Vector2 fromPosition = position2dOnMouseDown;

        Vector2 toPosition = SnapPositionToGrid(PixelToWorldandV3ToV2(mouse));
        if (board.IsMoveValid(fromPosition, toPosition, player)) // Check if the player has a valid move.
        {
            Piece enemyPiece = board.GetAttackedPiece(fromPosition, toPosition, player); //GetAttackedPiece will return a Piece if a piece was attacked, or null if it was a plain move.
            if (enemyPiece != null)
            {
                board.RemoveEnemyPiece(enemyPiece); // If there is an enemyPiece, then remove it from the board.
                if (board.IsThereAnotherAttack(toPosition, player)) // Check if there is another attack.
                {
                    board.attackingPiece = this; // If so, then set the attackingPiece to the current piece to force the player to do another attack w/ this piece,
                                                 // and don't end the turn.
                }
                else
                {
                    board.EndTurn(toPosition, this, player); // End turn after a single attack.
                }
            }
            else // If enemyPiece is null, then it was a plain move.
            {
                board.EndTurn(toPosition, this, player); // Then end the turn.
            }
            rigidbody2d.MovePosition(toPosition); // In all cases, you move the piece to the new position b/c the position is valid.
        }
        else
        {
            // If it was not a valid move, then snap to original position.
            rigidbody2d.MovePosition(SnapPositionToGrid(fromPosition));
        }                       
    }

    // Convert Pixel/Screen to World position.
    Vector2 PixelToWorldandV3ToV2(Mouse mouse)
    {
        Vector3 pixelPosition = mouse.position.ReadValue(); // Get pixel position.
        Vector3 worldPosition = cam.ScreenToWorldPoint(pixelPosition); // Convert pixel to world position.
        Vector2 position2d = worldPosition;
        return position2d;
    }

    Vector2 SnapPositionToGrid(Vector2 position2d)
    {
        position2d.x = Mathf.Floor(position2d.x) + .5f;
        position2d.y = Mathf.Floor(position2d.y) + .5f;
        return position2d;
    }

}
