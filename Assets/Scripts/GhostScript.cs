using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GhostScript : MonoBehaviour
{
    CaptureScript capture;
    Vector3 playerPos;
    RandomRotation randomRot;

    [Header("Renderers")]
    public GameObject chestGhostMesh;
    public GameObject tailGhostMesh;

    [Space]
    [Header("Public")]
    public bool escaping;
    public bool stunned;
    public Animator chestAnimator;
    public Animator tailAnimator;
    public float energy = 100;
    public Transform head;

    [Space]
    [Header("Colors")]

    [ColorUsage(true,true)]
    public Color normalColor, flashColor;

    private void Start()
    {
        randomRot = GetComponent<RandomRotation>();
        capture = FindObjectOfType<CaptureScript>();
    }

    public void Stun()
    {
        Destroy(randomRot);
        chestGhostMesh.SetActive(false);
        tailGhostMesh.SetActive(true);
        transform.rotation = capture.transform.rotation;
        tailAnimator.SetTrigger("stunned");
        stunned = true;
        capture.ShakeScreen();


        StunShine();
    }

    public void StunShine()
    {
        tailGhostMesh.GetComponent<Renderer>().material.DOColor(flashColor, "GColor", .08f).OnComplete(() =>
        tailGhostMesh.GetComponent<Renderer>().material.DOColor(normalColor, "GColor", .25f));
    }

    public void ActivateEscapeRig()
    {
        transform.parent = capture.transform;
        transform.DOLocalRotate(Vector3.zero, .3f);
        transform.DOLocalMove(capture.transform.GetChild(0).localPosition, .2f);
        tailAnimator.SetTrigger("escape");

        escaping = true;
        CanvasManager.instance.ShowText(true);
    }

    public void Capture()
    {
        tailAnimator.SetTrigger("capture");
        StartCoroutine(DestroyGhost());

        CanvasManager.instance.ShowText(false);
    }

    IEnumerator DestroyGhost()
    {
        yield return new WaitForSeconds(.52f);
        FindObjectOfType<MovementInput>().enabled = true;
        capture.finishParticle.Play();
        capture.ShakeScreen();
        Destroy(gameObject);
    }
            

    public void Damage(float angle, Vector3 axis)
    {
        float damage = Remap(angle, 130, 180, .5f, .1f);
        energy = Mathf.Max(0,energy - (.6f - damage));
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void Update()
    {
        if (escaping)
            CanvasManager.instance.UpdateText(head.position, ((int)energy).ToString());
    }
}
