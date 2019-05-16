using UnityEngine;
using UnityEngine.UI;

public class ControlsHelper : MonoBehaviour
{
    private Text helpText = null;

    void Awake()
    {
        helpText = this.GetComponent<Text>();
        helpText.enabled = false;
    }    
    void Update()
    {
        if( Input.GetKeyDown( KeyCode.F1 ) )        
            helpText.enabled = !helpText.enabled;             
    }
}
