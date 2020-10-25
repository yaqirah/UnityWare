﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

// Required for sprites
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class objectProperties : MonoBehaviour
{
    //Variables for menu and properties
    private bool isDragging;
    KeyCode kbInput = KeyCode.Space;
    public bool isClickable = false;
    bool menuOpen = false;
    public bool isDraggable = false;
    public bool kbInputOn = false;
    public bool controllable = false;
    public bool mouseOver;
    Vector2 mousePos;

    // Variables related to audio
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    private int audioClipIndex = 0;
    public bool hasSound = false;

    // Create an array to hold the sprites
    public SpriteRenderer spriteRenderer;
    public Sprite[] objectSprites;
    private int objectSpriteIndex = 0;

    // Create an array with preset colors
    private Color[] colors = { /* Black: */ new Color(0, 0, 0, 1),  /* White: */ new Color(1, 1, 1, 1), /* Magenta: */ new Color(1, 0, 1, 1), /* Red: */ new Color(1, 0, 0, 1), /* Yellow: */ new Color(1, 1, 0, 1), /* Green: */ new Color(0, 1, 0, 1), /* Cyan: */ new Color(0, 1, 1, 1), /* Blue: */ new Color(0, 0, 1, 1) };
    private int colorIndex = 0;

    // GUIscript contains the variables for playing
    GUIscript playGUI;

    // Contains game variables
    public gameMechanics mechanics;
    private playerMoveScript movement;

    // Start called on the first frame update
    private void Start()
    {
        // retrieve scripts
        movement = (transform.root.gameObject).GetComponent<playerMoveScript>();

        // Load all sprites from the sprite sheet
        AsyncOperationHandle<Sprite[]> spriteHandle = Addressables.LoadAssetAsync<Sprite[]>("Assets/Graphics/Objects/sheet.png");
        spriteHandle.Completed += LoadSpritesWhenReady;

        // Add audio
        audioSource = gameObject.GetComponent<AudioSource>();

        // Load in the main camera that oversees variables
        GameObject mainCamera = GameObject.Find("Main Camera");
        playGUI = mainCamera.GetComponent<GUIscript>();
        mechanics = mainCamera.GetComponent<gameMechanics>();
    }

    // Helper method to load sprites
    void LoadSpritesWhenReady(AsyncOperationHandle<Sprite[]> handleToCheck)
    {
        if (handleToCheck.Status == AsyncOperationStatus.Succeeded)
        {
            objectSprites = handleToCheck.Result;
        }
    }

    private void OnMouseOver()
    {
        mouseOver = true;
    }

    private void OnMouseExit()
    {
        mouseOver = false;
    }

    private void OnGUI()
    {
        
        if (menuOpen && !playGUI.controller.playingGame)
       {
            mousePos = Input.mousePosition; 
            GUILayout.BeginArea(new Rect(0, 4, 200, 300), GUI.skin.box);
            GUILayout.Label("Object Properties for " + transform.root.name);
            isDraggable = GUILayout.Toggle(isDraggable, "Draggable");
            isClickable = GUILayout.Toggle(isClickable, "Clickable");
            kbInputOn = GUILayout.Toggle(kbInputOn, "Space Does Action");

            // toggles movement properties
            controllable = GUILayout.Toggle(controllable, "Controllable");
            if (controllable)
            {
                movement.moveLeft = true;
                movement.moveRight = true;
                movement.moveUp = true;
                movement.moveDown = true;
            }
            else
            {
                movement.moveLeft = false;
                movement.moveRight = false;
                movement.moveUp = false;
                movement.moveDown = false;
            }

            // Change Sprites
            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Previous Sprite"))
            {
                objectSpriteIndex--;
                // If the index has gone below the bounds of the array
                if (objectSpriteIndex <= -1)
                {
                    // Go to the last element
                    objectSpriteIndex = objectSprites.Length - 1;
                }
                // Render the previous sprite
                spriteRenderer.sprite = objectSprites[objectSpriteIndex];
            }
            if (GUILayout.Button("Next Sprite"))
            {
                objectSpriteIndex++;
                // If the index has gone over the array, reset to zero
                if (objectSpriteIndex >= objectSprites.Length)
                {
                    objectSpriteIndex = 0;
                }
                // Render the next sprite
                spriteRenderer.sprite = objectSprites[objectSpriteIndex];
            }
            GUILayout.EndHorizontal();

            // Change colors
            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Previous Color"))
            {
                colorIndex--;
                if (colorIndex <= -1)
                {
                    // Go to the last element
                    colorIndex = colors.Length - 1;
                }
                spriteRenderer.color = colors[colorIndex];
            }
            if (GUILayout.Button("Next Color"))
            {
                colorIndex++;
                if (colorIndex >= colors.Length)
                {
                    // Go to the first element
                    colorIndex = 0;
                }
                spriteRenderer.color = colors[colorIndex];
            }
            GUILayout.EndHorizontal();

            
            // Change sound effect
            hasSound = GUILayout.Toggle(hasSound, "Sound Effects");
            if (hasSound)
            {
                // Add sound to objects
                GUILayout.BeginHorizontal("box");
                if (GUILayout.Button("Previous Sound"))
                {
                    audioClipIndex--;
                    if (audioClipIndex <= -1)
                    {
                        audioClipIndex = audioClips.Length - 1;
                    }
                    audioSource.clip = audioClips[audioClipIndex];
                    audioSource.Play();
                }
                if (GUILayout.Button("Next Sound"))
                {
                    audioClipIndex++;
                    if (audioClipIndex >= audioClips.Length)
                    {
                        audioClipIndex = 0;
                    }
                    audioSource.clip = audioClips[audioClipIndex];
                    audioSource.Play();
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                audioSource.Stop();
                audioClipIndex = 0; // reset index
            }
            if (GUILayout.Button("Delete Object"))
            {
                deleteObject();
            }

            if (GUILayout.Button("Close"))
            {
                menuOpen = false;
            }

            GUILayout.EndArea();


        }
        
    }
    
    /* 
     * Helper method to delete the current object.
     */
    void deleteObject()
    {
        menuOpen = false; // close menu as the object no longer exists
        mechanics.objectList.Remove(transform.root.gameObject);
        Destroy(transform.root.gameObject);
    }

    public void OnMouseDown()
    {
        if (mouseOver)
        {
            isDragging = true;
        }
    }

    public void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        //right click on object opens editor menu
        if (mouseOver && Input.GetMouseButton(1))
        {
            objectProperties otherProperties;
            // make sure no other property menus are open, if they are then close them
            foreach (GameObject obj in mechanics.objectList)
            {
                otherProperties = (obj.transform.GetChild(0).gameObject).GetComponent<objectProperties>();

                // close any other open menus
                if (otherProperties.menuOpen)
                {
                    otherProperties.menuOpen = false;
                }
            }
            menuOpen = true;
        }
        //what happens with isDraggable
        /* Allows objects to be dragged in two cases
         * Case 1: The object is draggable and the game is playing
         * Case 2: The object is selected during editor mode
         */
        if (((isDragging && isDraggable) && mechanics.playingGame) || (isDragging && !mechanics.playingGame))
        {
            bool overlapping = false; // tracks if the objects are overlapping in editor mode

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            // if in editor mode ensure that it stays along the grid
            if (!mechanics.playingGame)
            {
                // round down to snap to the grid
                mousePos.x = Mathf.Round(mousePos.x);
                mousePos.y = Mathf.Round(mousePos.y);

                float originalx = transform.root.transform.position.x;
                float originaly = transform.root.transform.position.y;

                // make sure objects don't overlap in editor mode
                foreach (GameObject obj in mechanics.objectList)
                {    
                    // if not the same object
                    if (!string.Equals(transform.root.name, obj.name))
                    {
                        // if the current object is overlapping with the object that is moving
                        if ((originalx + mousePos.x) == obj.transform.position.x && (originaly + mousePos.y) == obj.transform.position.y)
                        {
                            overlapping = true;
                        }
                    }
                }
            }
            if (!overlapping)
            {
                transform.root.transform.Translate(mousePos);
            }
        
        }
        //What happens if clickable and left click
        if (isClickable && Input.GetMouseButton(0))
        {
            menuOpen = true;
        }

        if (Input.GetKey(kbInput) && kbInputOn)
        {
            //What will happen with the KB input
        }
    }
}