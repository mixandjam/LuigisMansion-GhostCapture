using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CaptureScript : MonoBehaviour
{
    public CinemachineVirtualCamera gameCamera;
    CinemachineImpulseSource impulse;

    public float speed;
    private CharacterController c;
    [Space]

    [Header("Ghost")]
    public GhostScript currentGhost;

    [Space]
    [Header("Booleans")]
    public bool capturing;

    [Space]
    [Header("Rigs")]
    public Rig flashRig;
    public Rig captureRig;

    private Vector3 axis;

    [Space]
    [Header("Effects")]
    public ParticleSystem captureParticle;
    public ParticleSystem finishParticle;
    public GameObject light;
    public GameObject tornado;
    public Transform controller;

    Animator anim;
    RandomRotation randomRot;
    Renderer tornadoRenderer;
    Renderer lightRenderer;
    float amount;

    void Start()
    {
        c = GetComponent<CharacterController>();
        impulse = gameCamera.GetComponent<CinemachineImpulseSource>();
        tornadoRenderer = tornado.GetComponent<Renderer>();
        lightRenderer = light.GetComponent<Renderer>();
        tornadoRenderer.material.SetFloat("Opacity", 0);
        lightRenderer.material.SetFloat("Opacity", .3f);
        randomRot = GetComponent<RandomRotation>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

        if (Input.GetKeyDown(KeyCode.Space))
            Time.timeScale = (Time.timeScale == 1) ? .2f : 1;

        if (currentGhost != null)
        {
            if (Input.GetKeyDown(KeyCode.Z) && !capturing)
            {
                currentGhost.Stun();
                light.GetComponent<Renderer>().material.DOFloat(3, "Opacity", .06f).OnComplete(() =>
                light.GetComponent<Renderer>().material.DOFloat(.15f, "Opacity", .2f));

                light.GetComponentInChildren<Light>().DOIntensity(105, .06f).OnComplete(() =>
                light.GetComponentInChildren<Light>().DOIntensity(7, .25f));
            }

            if (Input.GetKeyDown(KeyCode.X) && currentGhost.stunned)
            {
                currentGhost.ActivateEscapeRig();
                GetComponent<MovementInput>().enabled = false;
                CaptureState(true);
            }
        }

        if (!capturing)
            return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        axis = new Vector3(-x, 0, -z);
        float angle = Vector3.Angle(transform.forward, axis);

        c.Move(transform.forward* speed * Time.deltaTime);

        if(angle > 130)
        {
            c.Move(axis * (speed/2) * Time.deltaTime);

            if (currentGhost.energy > 0)
                currentGhost.Damage(angle, axis);
            else
                Capture();
        }

        amount = (angle > 130) ? (-10 * Mathf.Abs(axis.magnitude)) : 10;
        controller.localEulerAngles = new Vector3(Mathf.LerpAngle(controller.localEulerAngles.x, amount, .35f),
            0, Mathf.LerpAngle(controller.localEulerAngles.z, amount, .3f));
    }

    private void Capture()
    {
        capturing = false;
        currentGhost.Capture();
        CaptureState(false);
    }

    public void CaptureState(bool state)
    {
        capturing = state;
        light.transform.GetChild(0).gameObject.SetActive(!state);
        randomRot.enabled = state;
        anim.SetBool("capturing", state);

        tornadoRenderer.material.SetFloat("Opacity", state ? 0 : 1);
        tornadoRenderer.material.DOFloat(state ? 1 : 0, "Opacity", .3f);
        lightRenderer.material.SetFloat("Opacity", state ? .3f : 0);
        lightRenderer.material.DOFloat(state ? 0 : .3f, "Opacity", .3f);

        DOVirtual.Float(state ? 1 : 0, state ? 0 : 1,.2f, FlashRig);
        DOVirtual.Float(state ? 0 : 1, state ? 1 : 0,.2f, CaptureRig);

        if (state)
        {
            captureParticle.Play();
        }
        else
        {
            captureParticle.Stop();
        }
    }

    public void ShakeScreen()
    {
        impulse.GenerateImpulse();
    }

    public void FlashRig(float x)
    {
        flashRig.weight = x;
    }
    public void CaptureRig(float x)
    {
        captureRig.weight = x;
    }
}
