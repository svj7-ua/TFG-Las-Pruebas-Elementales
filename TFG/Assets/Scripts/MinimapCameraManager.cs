using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LEGACY SCRIPT
/// This script manages the minimap camera position and rotation based on the player's position.
/// Replaced by the new minimap grid system.
/// </summary>

public class MinimapCameraManager : MonoBehaviour
{
	public Transform playerReference;
	public float playerOffset = 40f;


	private void Update()
	{
		if (playerReference != null)
		{
			transform.position = new Vector3(playerReference.position.x, playerReference.position.y + playerOffset, playerReference.position.z);
			transform.rotation = Quaternion.Euler(90f, playerReference.eulerAngles.y, 0f);
		}
	}
}