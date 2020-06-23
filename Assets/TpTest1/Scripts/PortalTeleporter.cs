using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class PortalTeleporter : MonoBehaviour {

	public Transform player;
	public Transform otherPortal;

	private bool playerIsOverlapping = false;

    public CinemachineFreeLook playerCamera;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;

        otherPortal = FindObjectsOfType<PortalTeleporter>().FirstOrDefault(x => x != this).transform;
    }

	void FixedUpdate () {
		if (playerIsOverlapping)
		{
			Vector3 portalToPlayer = player.position - transform.position;
			float dotProduct = Vector3.Dot(transform.forward, portalToPlayer);


            //Debug.Log(dotProduct);
			//Crossed teleport plane. Teleport
			if (dotProduct < 0f)
			{
                float rotationDiff = Quaternion.Angle(transform.rotation, otherPortal.rotation);

			    float angleFromEnteringPortal = Quaternion.Angle(transform.rotation, player.rotation);
                //Debug.Log($"enterAngle:{angleFromEnteringPortal} playerY:{player.rotation.eulerAngles.y}");
				rotationDiff += 180;
			    player.rotation = Quaternion.Euler(0, player.rotation.eulerAngles.y + rotationDiff, 0);
                
			    Vector3 exitVector = Quaternion.AngleAxis(angleFromEnteringPortal, Vector3.up) * otherPortal.forward;
			    player.rotation = Quaternion.LookRotation(-exitVector, Vector3.up);
                
                Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
				player.position = otherPortal.position + positionOffset;

			    //Debug.Log($"exit playerY:{player.rotation.eulerAngles.y}./. portaltoplayer {portalToPlayer}");

                playerIsOverlapping = false;
                
                if (pauseEditorOnTp)
			        EditorApplication.isPaused = true;
			}
		}
	}
    
    public bool pauseEditorOnTp = false;
    
	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Player")
		{
			playerIsOverlapping = true;
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.tag == "Player")
		{
			playerIsOverlapping = false;
		}
	}
}
