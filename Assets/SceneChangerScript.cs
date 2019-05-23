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

    // Start is called before the first frame update
    void Start()
    {
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
}
