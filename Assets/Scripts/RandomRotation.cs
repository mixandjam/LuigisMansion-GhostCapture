using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    private bool right;

    public float rotateSpeed = 10;
    public AnimationCurve lerpEase = default;
    public float yRot;

    [Space]
    [Header("Ghost Aim")]
    public Transform player;

    private void OnEnable()
    {
        StartCoroutine(RotateTo());
        StartCoroutine(ChooseDir());
    }

    IEnumerator RotateTo()
    {
        yRot += Random.Range(15, 45) * (right ? 1 : -1);
        float distance = Mathf.Abs(Mathf.DeltaAngle(transform.localEulerAngles.y, yRot));

        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(0, yRot, 0);

        float animateTime = 0;
        float animationLength = distance / rotateSpeed;

        while (animateTime < animationLength)
        {
            animateTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRot, endRot, lerpEase.Evaluate(animateTime / (animationLength)));
            yield return null;
        }

        StartCoroutine(RotateTo());
    }

    IEnumerator ChooseDir()
    {
        yield return new WaitForSeconds(Random.Range(1, 3));
        right = (Random.value > 0.5f);
        StartCoroutine(ChooseDir());
    }

    private void OnDisable()
    {
        StopCoroutine("RotateTo");
        StopCoroutine("ChooseDir");
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        if(FindObjectOfType<CaptureScript>() != null)
            transform.rotation = FindObjectOfType<CaptureScript>().transform.rotation;
    }
}
