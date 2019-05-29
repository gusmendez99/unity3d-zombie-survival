using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChangerScript : MonoBehaviour
{
    public int index;
    public Light pointLight;

    public float minTime = 1f;
    public float maxTime = 3.2f;

    private float timer;

    private bool showText = false, someRandomCondition = true;
    private float currentTime = 0.0f, executedTime = 0.0f, timeToWait = 5.0f;


    private void OnEnable()
    {
        executedTime = Time.time;
    }

    // Start is called before the first frame update
    void Start()
    {
        executedTime = Time.time;
        timer = Random.Range(minTime, maxTime);
        if (pointLight != null)
        {
            pointLight.enabled = true;
        } //Enables light
    }

    // Update is called once per frame
    void Update()
    {
        // makes a variation on the intensity of the light
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            pointLight.enabled = !pointLight.enabled;

        }
        //Showing message
        currentTime = Time.time;
        if (someRandomCondition)
            showText = true;
        else
            showText = false;

        if (executedTime != 0.0f)
        {
            if (currentTime - executedTime > timeToWait)
            {
                executedTime = 0.0f;
                someRandomCondition = false;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            LoadScene(index);

        }
    }

    private void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    void OnGUI()
    {
        string text = "";
        if(index == 1)
        {
            text = "Go to the door! You have to escape";
        } else if(index == 2)
        {
            text = "Look for the helicopter now, you must go!";
        }
        GUI.contentColor = Color.red;
        GUIStyle myStyle = new GUIStyle(GUI.skin.GetStyle("label"));
        myStyle.fontSize = 25;

        if (showText)
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height - 100, 200, 100), text, myStyle);
    }
}
