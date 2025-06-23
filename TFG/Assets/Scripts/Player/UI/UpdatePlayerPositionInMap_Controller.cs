using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; // Assuming you are using Unity's UI system for Image
using UnityEngine;

public class UpdatePlayerPositionInMap_Controller : MonoBehaviour
{

    private Grid2D<Image> mapGrid; // Assuming you have a Grid2D<Image> to represent the map grid
    private bool isGridSet = false; // Flag to check if the player is in the map
    private Vector2Int lastVisitedPosition = new Vector2Int(0, 0); // To store the last visited position of the player

    [SerializeField] private Sprite playerImage; // The image that represents the player in the map
    [SerializeField] private Sprite mapImage; // The image that represents the map

    private Sprite mapImg;
    private Sprite auxMapImg;

    void Start()
    {
        mapImg = mapImage; // Store the map image for later use
    }

    // Update is called once per frame
    void Update()
    {

        if (isGridSet)
        {
            Vector2Int playerPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z) * -1);

            float offsetX = -6f;
            float offsetY = 6f;

            int i = Mathf.FloorToInt((playerPosition.x - offsetX) / 12f);
            int j = Mathf.FloorToInt((offsetY - playerPosition.y) / 12f); // nota: el eje Y va invertido
            j = j * -1;

            if (i != lastVisitedPosition.x || j != lastVisitedPosition.y)
            {
                // Update the last visited position
                auxMapImg = mapGrid[i, j].sprite; // Store the current sprite of the grid cell
                mapGrid[i, j].sprite = playerImage; // Set the sprite of the grid cell to the player image
                mapGrid[lastVisitedPosition].sprite = mapImg; // Reset the previous position to the map image
                mapImg = auxMapImg; // Update the map image to the previous position's sprite

                lastVisitedPosition = new Vector2Int(i, j);
            }

        }

    }

    public void SetGrid(Grid2D<Image> grid)
    {
        mapGrid = grid;

        mapGrid[0, 0].sprite = playerImage; // Set the initial position to the map image

        isGridSet = true;
    }
    
    public void ResetGrid()
    {
        isGridSet = false;
        lastVisitedPosition = new Vector2Int(0, 0);
        mapGrid = null; // Clear the grid reference
    }
}
