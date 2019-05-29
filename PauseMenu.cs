using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    private bool isPaused = false;
    public Canvas ui;
    // Start is called before the first frame update
    void Start()
    {
        ui.enabled = false;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Continue();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Pause()
    {
        Cursor.visible = true;
        ui.enabled=true;
        Time.timeScale = 0.0f;
        isPaused = true;
    }
    public void Continue()
    {
        Cursor.visible = false;
        ui.enabled=false;
        Time.timeScale = 1.0f;
        isPaused = false;
    }
    public void Return()
    {
        Application.Quit();
    }
}