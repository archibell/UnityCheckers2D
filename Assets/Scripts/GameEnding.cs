using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnding : MonoBehaviour
{
    public float fadeDuration = 1f; // Set default fadeDuration and make it public so it can adjusted from the Inspector.
    public float displayImageDuration = 1f; // To help extend the time after the Image has been faded in, before the application quits.
    public CanvasGroup darkWonImageBkgrdCanvasGroup; // A public variable for the Canvas Group component, which can be assigned in the Inspector.
    public CanvasGroup lightWonImageBkgrdCanvasGroup;
    float timer; // A timer, to ensure that the game doesn't end before the fade has finished.
    bool darkWon { get; set; } // A bool variable, if not assigned, is always initialized to false.
    Player playerLight;
    Player playerDark;

    // Start is called before the first frame update.
    void Start()
    {
        playerLight = GameObject.Find("PlayerLight").GetComponent<Player>();
        playerDark = GameObject.Find("PlayerDark").GetComponent<Player>();
    }

    // Update() is called every frame. Check whether a winner has been found every frame.
    void Update()
    {
        if (WinnerFound()) // If a winner has been found, then it drops into the if statement's code block.
        {
            if (darkWon)
            {
                EndGame(darkWonImageBkgrdCanvasGroup);
            }
            else // Otherwise, lightWon.
            {
                EndGame(lightWonImageBkgrdCanvasGroup);
            }
        }
    }

    // Check if there is a winner.
    bool WinnerFound()
    {
        int playerLightChildCount = playerLight.transform.childCount;
        Debug.Log("PlayerLightCount: " + playerLightChildCount);
        int playerDarkChildCount = playerDark.transform.childCount;
        Debug.Log("PlayerDarkCount: " + playerDarkChildCount);
        if (playerLightChildCount == 0) // If PlayerLight has no remaining pieces, then PlayerDark won.
        {
            darkWon = true;
            return true;
        }
        else if(playerDarkChildCount == 0) // If PlayerDark has no remaining pieces, then PlayerLight won.
        {
            darkWon = false;
            return true;
        }
        return false;
    }

    // Add CanvasGroup parameter into the EndLevel method, so that it can change the alpha of the new parameter.
    // The script will change the alpha of whatever is passed in as a parameter.
    void EndGame(CanvasGroup imageCanvasGroup)
    {
        timer += Time.deltaTime; // Start counting up the timer.
                                 // The Alpha value should be 0 when the timer is 0, and 1 when the timer is up to the fadeDuration.
                                 // In order to get this value, you can divide the timer by the duration.
                                 // FYI, the lower the Alpha value of a color, the more transparent the GameObject is.
                                 // Adjusting the Alpha will be the key to making the image fade in and out.
        imageCanvasGroup.alpha = timer / fadeDuration; // The image will now fade in when a winner has been determined.
        // Finally, the game needs to quit when the fade is finished. The fade will finish when the timer is greater than the duration.
        if (timer > fadeDuration + displayImageDuration)
        {
            Application.Quit(); // Quit the game.
        }
    }
}
