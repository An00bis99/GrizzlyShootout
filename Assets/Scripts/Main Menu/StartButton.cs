using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class StartButton : MonoBehaviour
{
    public Canvas myCanvas;
    public MainMenu canvasScript;
    private Button myButton;


    void Start()
    {
        canvasScript = myCanvas.GetComponent<MainMenu>();
        myButton = myCanvas.GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnClick()
    {

    }
}
