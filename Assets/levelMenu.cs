﻿using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class levelMenu : MonoBehaviour
{
    public GameObject mainCamera;
    public returnToMenu editor;

    public changeBackground editorBackground;
    public GameObject displayBackground;

    private gameMechanics controller;
    private AudioSource editorAudio;
    private string lastLevelName = "";

    public GameObject selectedLevel;
    public string selectedLevelPath;
    public bool selected = false;
    private bool menuOpen = false;

    public List<GameObject> panelList; // list to hold all panels
    private int panelCount = 30; // number of panels

    Vector3 cameraNewPosition = new Vector3(30.25f, 4.0f, -10.0f); // camera position for this menu
    Vector3 cameraOldPosition = new Vector3(9.0f, 4.0f, -10.0f); // normal camera position

    // Start is called before the first frame update
    void Start()
    {
        // get editor objects
        editorBackground = (GameObject.Find("background")).GetComponent<changeBackground>();
        editorAudio = (GameObject.Find("background")).GetComponent<AudioSource>();
        editor = (GameObject.Find("background")).GetComponent<returnToMenu>();

        // get camera object
        mainCamera = GameObject.Find("Main Camera");

        // get background
        displayBackground = GameObject.Find("displaybackground");

        // get play gui
        controller = mainCamera.GetComponent<gameMechanics>();

        // add all panels to the panel list
        for (int i = 0; i < panelCount; i++)
        {
            panelList.Add(GameObject.Find("panel" + (i + 1)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        if (menuOpen)
        {
            // button to close menu
            if (GUI.Button(new Rect(13, 5, 23, 23), "X"))
            {
                exitMenu();
            }

            if (selected)
            {
                // button to load level
                if (GUI.Button(new Rect(Screen.width - 223, Screen.height - 35, 90, 20), "Load Level"))
                {
                    loadLevel();
                }

                // button to delete level
                if (GUI.Button(new Rect(Screen.width - 123, Screen.height - 35, 90, 20), "Delete Level"))
                {
                    deleteLevel();
                }
            }
        }
    }
    public void loadLevel()
    {
        // TODO: set lastLevelName to the path of the selected level if a level is selected
        editor.loadLevel(selectedLevelPath);

        // exit the menu
        exitMenu();
    }
    public void deleteLevel()
    {
        // TODO: Add a way to select levels
        (selectedLevel.GetComponent<levelThumbnails>()).unsetObject();

        selected = false;
    }
    public void enterMenu()
    {
        // load the level files
        string[] levelPaths = Directory.GetFiles("Assets/Resources/Saves/", "*.txt", SearchOption.AllDirectories);

        for (int i = 0; i < panelCount; i++)
        {
            // if i < levelPaths.length
            if (i < levelPaths.Length)
            {
                (panelList[i].GetComponent<levelThumbnails>()).setObject(levelPaths[i]);
            }
            else
            {
                // set to blank image
                (panelList[i].GetComponent<levelThumbnails>()).unsetObject();
            }
        }

        // move the camera to the level select menu
        mainCamera.transform.position = cameraNewPosition;

        // hide editor gui
        controller.showGUI = false;

        // if there is music, stop it
        if (editorBackground.hasSound)
        {
            editorAudio.Pause();
        }

        // open the display menu
        menuOpen = true;
    }
    public string exitMenu()
    {
        // move the camera back
        mainCamera.transform.position = cameraOldPosition;

        // show editor gui
        controller.showGUI = true;

        // if there is music, start it
        if (editorBackground.hasSound)
        {
            editorAudio.Play();
        }

        // close the display menu
        menuOpen = false;

        // clear any selected menus
        selected = false;

        return lastLevelName;
    }
}