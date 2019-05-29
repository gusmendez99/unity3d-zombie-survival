using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class videoManagerScript : MonoBehaviour
{
    public GameObject videoCam;
    public int sceneToJumpIndex;
    public int videoDuration;
    // Start is called before the first frame update
    void Start()
    {
        if (videoCam)
        {
            StartCoroutine(ShowIntro());
        }
    }

    // Update is called once per frame
    void Update()
    {

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
}
