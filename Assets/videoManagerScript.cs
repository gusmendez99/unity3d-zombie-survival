using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class videoManagerScript : MonoBehaviour
{
    public GameObject videoCam;
    public int sceneToJumpIndex;
    public int videoDuration;

    string message = "Press 'Enter' to skip";
    float displayTime = 3f;
    bool displayMessage = false;

    // Start is called before the first frame update
    void Start()
    {
        if (videoCam)
        {
            StartCoroutine(ShowIntro());
            StartCoroutine(ShowPrompt());
        }

        Cursor.lockState = CursorLockMode.None;
        // Hide cursor when locking
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        displayTime -= Time.deltaTime;
        if (displayTime <= 0.0)
        {
            displayMessage = false;
        }

        if (Input.GetKeyDown(KeyCode.Return)) //Jump this video
        {
            Debug.Log("Enter");
            LoadScene(sceneToJumpIndex);
        }
    }

    IEnumerator ShowPrompt()
    {
        print(Time.time);
        yield return new WaitForSeconds(5.0f);
        displayMessage = true;
        displayTime = 5.0f;
    }

    IEnumerator ShowIntro()
    {
        print(Time.time);
        yield return new WaitForSeconds(videoDuration);
        print(Time.time);
        LoadScene(sceneToJumpIndex);


    }

    private void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    private void OnGUI()
    {
        if (displayMessage)
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 200f, 200f), message);
        }
    }
}
