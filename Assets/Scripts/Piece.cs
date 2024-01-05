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
                                       // For a bool, the default value is always false.
    

    // Start is called before the first frame update.
    void Start()
    {
        // Variable initializations.
        rigidbody2d = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        mouse = Mouse.current;  // Initialize mouse.
                                // Mouse.current >> it's the mouse device connected to the computer.               
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
        position2dOnMouseDown = SnapPositionToGrid(PixelToWorld(mouse));
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
        Vector2 fromPosition = position2dOnMouseDown;
        Vector2 toPosition = PixelToWorld(mouse); // Can't SnapPositionToGrid() here b/c we need smooth mouse drag.
        if (!board.IsWithinBoardBounds(toPosition))
        {
            rigidbody2d.MovePosition(fromPosition); // If mouse is out of bounds, then move piece to original position & return early.
            return;
        }
        rigidbody2d.MovePosition(toPosition); // If reach here, the mouse is inside bounds, then move piece to new position.
        
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
        Vector2 toPosition = SnapPositionToGrid(PixelToWorld(mouse));
        if (PerformMove(fromPosition, toPosition)) // Check if we should perform a move.
        {
            rigidbody2d.MovePosition(toPosition); // If yes, then move the piece to the new position.
        }
        else // Otherwise, we should not move.
        {
            // Then snap to original position.
            rigidbody2d.MovePosition(SnapPositionToGrid(fromPosition));
        }                       
    }

    bool PerformMove(Vector2 fromPosition, Vector2 toPosition)
    {
        if (board.IsMoveValid(fromPosition, toPosition, this)) // Check if the player has a valid move.
        {
            Piece enemyPiece = board.GetAttackedPiece(fromPosition, toPosition, this); //GetAttackedPiece will return a Piece if a piece was attacked,
                                                                                       //or null if it was a plain move.
            if (enemyPiece != null) // Check if we are going over an enemyPiece.
            {
                board.RemoveEnemyPiece(enemyPiece); // If there is an enemyPiece, then remove it from the board.
                if (board.IsThereAnotherAttack(toPosition, this)) // Check if there is another attack.
                {
                    board.attackingPiece = this; // If so, then set the attackingPiece to the current piece to force the player to do another attack w/ this piece,
                                                 // and don't end the turn.
                }
                else
                {
                    board.EndTurn(toPosition, this); // End turn after a single attack.
                }
            }
            else // If enemyPiece is null, then it was a plain move.
            {
                board.EndTurn(toPosition, this); // Then end the turn.
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    // Convert Pixel/Screen to World position.
    Vector2 PixelToWorld(Mouse mouse)
    {
        Vector2 pixelPosition = mouse.position.ReadValue(); // Get pixel position.
        Vector2 worldPosition = cam.ScreenToWorldPoint(pixelPosition); // Convert pixel to world position.
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
