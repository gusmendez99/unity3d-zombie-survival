using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    public Canvas UI;
    // Start is called before the first frame update
    void Start()
    {
        UI.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (isPaused)
            {
                Continue();
            }
            else if (!isPaused)
            {
                Pause();
            }
        }
    }
    public void Pause()
    {
        Time.timeScale = 0.0f;
        UI.enabled = true;
    }
    public void Continue()
    {
        Time.timeScale = 1.0f;
        UI.enabled = false;
    }

    public void Return()
    {
        Application.Quit();
    }
}