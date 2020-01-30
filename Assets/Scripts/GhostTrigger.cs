using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTrigger : MonoBehaviour
{
    private CaptureScript capture;

    private void Start()
    {
        capture = FindObjectOfType<CaptureScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ghost") && !capture.capturing)
            if (capture.currentGhost != other.GetComponent<GhostScript>())
                capture.currentGhost = other.GetComponent<GhostScript>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ghost") && !capture.capturing)
            capture.currentGhost = null;
    }
}
