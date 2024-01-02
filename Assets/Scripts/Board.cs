using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Board : MonoBehaviour
{
    // Variable declarations.
    List<Piece> pieceList = new List<Piece>();
    public Player playerTurn; // `PlayerDark` or `PlayerLight` is assigned to Pieces in Unity UI.
    public Piece attackingPiece { get; set; } // A reference to the piece that must be used to do another attack. If not set, it means the player can move any piece.

    // Start is called before the first frame update
    private void Start()
    {
        // Variable initializations.
        pieceList.AddRange(Object.FindObjectsOfType<Piece>()); // AddRange copies items from an Array to a list.
        foreach (Piece piece in pieceList) // Initialize all piece's amIQueen to false.
        {
            piece.amIQueen = false;
            Debug.Log("piece amIQueen: " + piece.amIQueen);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsWithinBoardBounds(Vector2 fromPosition, Vector2 toPosition)
    {
        // Check if toPosition is within the board bounds.
        if (toPosition.x < -4 || toPosition.x > 4)
        {
            return false;
        }
        if (toPosition.y < -4 || toPosition.y > 4)
        {
            return false;
        }
        return true;
    }

    public bool IsMoveValid(Vector2 fromPosition, Vector2 toPosition, Player player)
    {
        // Check if toPosition is within the board bounds.
        if (toPosition.x < -4 || toPosition.x > 4) 
        {
            return false;
        }
        if (toPosition.y < -4 || toPosition.y > 4)
        {
            return false;
        }
        // Check we are not landing on a cell that is occupied.
        foreach (Piece piece in pieceList)
        {
            Vector2 piecePosition = piece.GetComponent<Rigidbody2D>().position;
            if (toPosition == piecePosition)
            {
                return false;
            }
        }
        // Check we are landing on a dark cell, the only cells you can move into in checkers.
        if (!IsDarkCell(toPosition))
        {
            return false;
        }
        // Check if this is a valid diagonal move or a valid attack move.
        if (!IsValidDiagStep(fromPosition, toPosition, player, 1) && GetAttackedPiece(fromPosition, toPosition, player) == null)
        {
            return false;
        }
        return true;
    }

    // Check we are landing on a dark cell, the only cells you can move into in checkers. 
    bool IsDarkCell(Vector2 position)
    {        
        if ((Mathf.Floor(position.x) + Mathf.Floor(position.y)) % 2 == 0)
        {
            return true;
        }
        return false;
    }

    // Check if this is a valid diagonal move.
    bool IsValidDiagStep (Vector2 fromPosition, Vector2 toPosition, Player player, float distance)
    {
        if (player.amIPlayerDark) // Check for PlayerDark.
        {
            // If toPosition.x is neither +distance nor -distance, then false.
            if ((toPosition.x != (fromPosition.x - distance)) && (toPosition.x != (fromPosition.x + distance)))
            {
                return false;
            }
            // If toPosition.y is not +distance, then false.
            if (toPosition.y != (fromPosition.y + distance))
            {
                return false;
            }
        }
        if (!player.amIPlayerDark) // Check for PlayerLight.
        {
            // If toPosition.x is neither +distance nor -distance, then false.
            if ((toPosition.x != (fromPosition.x - distance)) && (toPosition.x != (fromPosition.x + distance)))
            {
                return false;
            }
            // If toPosition.y is not -distance, then false.
            if (toPosition.y != (fromPosition.y - distance))
            {
                return false;
            }
        }
        return true;
    }

    // Return the enemyPiece in a valid attack move.
    public Piece GetAttackedPiece(Vector2 fromPosition, Vector2 toPosition, Player player)
    {
        if (!IsValidDiagStep(fromPosition, toPosition, player, 2))
        {
            return null;
        }
        // Get position of the cell we are flying over.
        Vector2 flyOverPosition = new Vector2(); // Create a new (0, 0) x,y vector.
        flyOverPosition.x = fromPosition.x + ((toPosition.x - fromPosition.x) / 2);
        flyOverPosition.y = fromPosition.y + ((toPosition.y - fromPosition.y) / 2);
        // Check if the flyOverPosition is flying over an enemy piece.
        foreach (Piece piece in pieceList)
        {
            Vector2 piecePosition2d = piece.GetComponent<Rigidbody2D>().position;
            if (piecePosition2d == flyOverPosition)
            {
                // Check the piece you are flying over is not your own.
                if (player != piece.player)
                {
                    return piece;
                }
            }
        }
        return null;
    }

    public bool IsThereAnotherAttack(Vector2 fromPosition, Player player)
    {
        if (player.amIPlayerDark)
        {
            Vector2 toPositionUpLeft = new Vector2(fromPosition.x - 2, fromPosition.y + 2);
            Vector2 toPositionUpRight = new Vector2(fromPosition.x + 2, fromPosition.y + 2);
            if (IsMoveValid(fromPosition, toPositionUpLeft, player))
            {
                return true;
            }
            if (IsMoveValid(fromPosition, toPositionUpRight, player))
            {
                return true;
            }
            return false;
        }
        else
        {
            Vector2 toPositionDownLeft = new Vector2(fromPosition.x - 2, fromPosition.y - 2);
            Vector2 toPositionDownRight = new Vector2(fromPosition.x + 2, fromPosition.y - 2);
            if (IsMoveValid(fromPosition, toPositionDownLeft, player))
            {
                return true;
            }
            if (IsMoveValid(fromPosition, toPositionDownRight, player))
            {
                return true;
            }
            return false;
        }
    }

    public void RemoveEnemyPiece(Piece enemyPiece)
    {
        Destroy(enemyPiece.gameObject); // Destroy the enemyPiece gameObject so that it disappears from the game.
        pieceList.Remove(enemyPiece); // Remove the piece from the list of pieces b/c it's no longer in play.
    }

    public void EndTurn(Vector2 piecePosition, Piece piece, Player player)
    {
        if (player.amIPlayerDark) // If current player is dark, then set playerTurn to PlayerLight.
        {
            playerTurn = GameObject.Find("PlayerLight").GetComponent<Player>();
        }
        else // Otherwise, current player is light, then set playerTurn to PlayerDark.
        {
            playerTurn = GameObject.Find("PlayerDark").GetComponent<Player>();
        }
        attackingPiece = null; // Reset the attackingPiece so the next player can move any of their pieces.
        // Check if player should become Queen.
        piece.amIQueen = BecomeQueen(piecePosition, piece, player);
        Debug.Log("I became Queen: " + piece.amIQueen);        
    }

    // Check if piece should become a Queen or not. If becomes a Queen, then change the piece sprite to reflect the queenSprite.
    bool BecomeQueen(Vector2 piecePosition, Piece piece, Player player)
    {
        SpriteRenderer pieceRenderer = piece.GetComponent<SpriteRenderer>();
        if (player.amIPlayerDark && !piece.amIQueen)
        {            
            if (piecePosition.y == 3.5f)
            {
                pieceRenderer.sprite = piece.queenSprite;
                return true;
            }
        }
        if (!player.amIPlayerDark && !piece.amIQueen)
        {
            if (piecePosition.y == -3.5f)
            {
                pieceRenderer.sprite = piece.queenSprite;
                return true;
            }
        }
        return false;
    }


}
