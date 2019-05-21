using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class videoManagerScript : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject lobbyCam;
    public GameObject userInt;
    public GameObject videoCam;
    
    // Start is called before the first frame update
    void Start()
    {
        videoCam.SetActive(true);
        lobbyCam.SetActive(false);
        userInt.SetActive(false);
        gameManager.SetActive(false);
        StartCoroutine(ShowIntro());
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ShowIntro()
    {
        print(Time.time);
        yield return new WaitForSeconds(48f);
        print(Time.time);
        lobbyCam.SetActive(true);
        videoCam.SetActive(false);
        userInt.SetActive(true);
        gameManager.SetActive(true);
    }
}
