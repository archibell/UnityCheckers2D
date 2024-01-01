using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Board : MonoBehaviour
{
    // Variable declarations.
    List<Piece> pieces = new List<Piece>();

    // Start is called before the first frame update
    private void Start()
    {
        // Variable initializations.
        pieces.AddRange(Object.FindObjectsOfType<Piece>()); // AddRange copies items from an Array to a list.
        Debug.Log("List<Piece> pieces: " + pieces);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsMoveValid(Vector2 fromPosition, Vector2 toPosition, Player player)
    {
        // Snap positions to make code simplier.
        fromPosition = SnapPositionToGrid(fromPosition);
        toPosition = SnapPositionToGrid(toPosition);
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
        foreach (Piece piece in pieces)
        {
            Vector2 piecePosition2d = piece.GetComponent<Rigidbody2D>().position;
            if (toPosition == piecePosition2d)
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
        if (!IsValidDiagStep(fromPosition, toPosition, player, 1) && !IsValidAttackMove(fromPosition, toPosition, player))
        {
            return false;
        }
        return true;
    }

    bool IsDarkCell(Vector2 position)
    {
        position = SnapPositionToGrid(position);
        if ((Mathf.Floor(position.x) + Mathf.Floor(position.y)) % 2 == 0)
        {
            return true;
        }
        return false;
    }

    Vector2 SnapPositionToGrid(Vector2 position)
    {
        position.x = Mathf.Floor(position.x) + .5f;
        position.y = Mathf.Floor(position.y) + .5f;
        return position;
    }

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

    bool IsValidAttackMove(Vector2 fromPosition, Vector2 toPosition, Player player)
    {
        if (!IsValidDiagStep(fromPosition, toPosition, player, 2))
        {
            return false;
        }
        // Get position of the cell we are flying over.
        Vector2 flyOverPosition = new Vector2(); // Create a new (0, 0) x,y vector.
        flyOverPosition.x = fromPosition.x + ((toPosition.x - fromPosition.x) / 2);
        flyOverPosition.y = fromPosition.y + ((toPosition.y - fromPosition.y) / 2);
        // Check if the flyOverPosition is flying over an enemy piece.
        foreach (Piece piece in pieces)
        {
            Vector2 piecePosition2d = piece.GetComponent<Rigidbody2D>().position;
            if (piecePosition2d == flyOverPosition)
            {
                // Check the piece you are flying over is not your own.
                if (player != piece.player)
                {
                    return true;
                }
            }
        }
        return false;
    }


}
