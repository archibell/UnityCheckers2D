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
    public Player player;
    Vector2 position2dOnMouseDown;
    Board board;

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
        Debug.Log("OnMouseDown gameObject: " + this.gameObject.name);
        mouse = Mouse.current;  // Initialize mouse.
                                // Mouse.current >> The mouse that was added or updated last or null if there is no mouse connected to the system.                                
        position2dOnMouseDown = PixelToWorldandV3ToV2(mouse);
        rigidbody2d.MovePosition(position2dOnMouseDown);
        Debug.Log("OnMouseDown position2d: " + position2dOnMouseDown);

    }

    // OnMouseDrag is called when the user has clicked on a Collider and is still holding down the mouse.
    void OnMouseDrag()
    {
        Vector2 position2d = PixelToWorldandV3ToV2(mouse);
        rigidbody2d.MovePosition(position2d);
        
    }

    // OnMouseUpAsButton is only called when the mouse is released over the same Collider as it was pressed.
    void OnMouseUpAsButton()
    {
        Debug.Log("OnMouseUpAsButton: " + this.gameObject.name);
        Vector2 position2d = SnapPositionToGrid(PixelToWorldandV3ToV2(mouse));
        if (board.IsMoveValid(position2dOnMouseDown, position2d, player))
        {
            rigidbody2d.MovePosition(position2d);
            Debug.Log("OnMouseUpAsButton position2d: " + position2d);
        }
        else
        {
            // Snap to original position.
            rigidbody2d.MovePosition(SnapPositionToGrid(position2dOnMouseDown));
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
