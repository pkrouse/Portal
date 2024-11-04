using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;
using DG.Tweening;
using Unity.XR.CoreUtils;

public class PortalGun : MonoBehaviour
{
    private PlayerInput playerInput;
    private enum GunSettings
    {
        Blue,
        Orange
    }
    private GunSettings currentGunSetting = GunSettings.Blue;

    [SerializeField] private Transform needle;
    [SerializeField] private GameObject gunBody;
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material orangeMaterial;

    [SerializeField] private GameObject OrangePortalPrefab;
    private GameObject OrangePortal = null;
    [SerializeField] private GameObject BluePortalPrefab;
    private GameObject BluePortal = null;
    private GameObject CurrentPortal = null;

    [SerializeField] private Transform you;
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private float launchForce = 20f;
    [SerializeField] private float offsetDistance = 0.75f;

    private float bluePosition = 25;
    private float orangePosition = -25;

    private LineRenderer laserLine;
    [SerializeField] private Material YellowLaserMaterial;
    [SerializeField] private Material GreenLaserMaterial;
    private float lineWidth = 0.01f;

    LayerMask hitMask;

    [SerializeField] private Transform aimer;

    private RaycastHit hit;
    private bool hitValid = false;

    private bool isHeld = false;
    void Start()
    {
        hitMask = LayerMask.GetMask("AimFriendly");
        laserLine = GetComponent<LineRenderer>();
        laserLine.startWidth = lineWidth;
        laserLine.endWidth = lineWidth;

        playerInput = GetComponent<PlayerInput>();
        playerInput.onActionTriggered += onActionTriggered;
        SetGun(GunSettings.Blue);
    }


    void Update()
    {
        if (isHeld && Physics.Raycast(aimer.position, aimer.forward, out hit, 500, hitMask))
        {
            hitValid = true;
            laserLine.enabled = true;
            laserLine.SetPosition(0, aimer.position);
            laserLine.SetPosition(1, hit.point);
            if (hit.collider.gameObject.tag == "Portal")
                laserLine.material = YellowLaserMaterial;
            else
                laserLine.material = GreenLaserMaterial;
        }
        else
        {
            laserLine.enabled = false;
            hitValid = false;
        }
    }

    public void PickedUp( )
    {
        isHeld = true;
    }

    public void Dropped( )
    {
        isHeld = false;
    }

    private void onActionTriggered(InputAction.CallbackContext context)
    {
        if (context.action.name == "PortalFire" && hitValid == true)
        {
            if (context.action.phase == InputActionPhase.Started)
            {
                PortalAction(context);
            }
        }
        else if (context.action.name == "PortalToggle")
        {
            if (context.action.phase == InputActionPhase.Started)
            {
                PortaGunlToggle(context);
            }
        }
    }

    private void PortaGunlToggle(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>().x < 0)
        {
            SetGun(GunSettings.Blue);
            if (BluePortal is not null)
                CurrentPortal = BluePortal;
        }
        else
        {
            SetGun(GunSettings.Orange);
            if (OrangePortal is not null)
                CurrentPortal = OrangePortal;
        }
    }

    private void PortalAction(InputAction.CallbackContext context)
    {
        if (hit.collider.gameObject.tag == "Portal")
        {
            if (BluePortal is null || OrangePortal is null)
                return;
            PortalTransport();
        }
        else
        {
            PortalPlace();
        }
    }

    private void PortalPlace()
    {
        if (currentGunSetting == GunSettings.Blue)
        {
            Debug.Log("Placing blue portal");
            if (BluePortal is null)
            {
                BluePortal = GameObject.Instantiate(BluePortalPrefab, hit.point, hit.collider.transform.rotation);
                CurrentPortal = BluePortal;
                HandleParticles();
            }
        }
        else
        {
            Debug.Log("Placing orange portal");
            if (OrangePortal is null)
            {
                OrangePortal = GameObject.Instantiate(OrangePortalPrefab, hit.point, hit.collider.transform.rotation);
                CurrentPortal = OrangePortal;
                HandleParticles();
            }
        }
        CurrentPortal.transform.position = hit.point;
        CurrentPortal.transform.rotation = hit.collider.transform.rotation;
    }
    private void PortalTransport()
    {
        if (hit.collider.gameObject.name == "BluePortalCollider")
        {
            Debug.Log("Going through the blue portal.");
            Transport(BluePortal.transform, OrangePortal.transform);
        }
        else
        {
            Debug.Log("Going through the orange portal.");
            Transport(OrangePortal.transform, BluePortal.transform);
        }
        
    }
    
    private void Transport(Transform portalFrom, Transform portalTo)
    {
        Debug.Log("transporting ");
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(you.DOMove(portalFrom.transform.position, 1));
        mySequence.Join(you.DORotate(portalFrom.transform.localRotation.eulerAngles.Inverse(), 1));
        mySequence.onComplete = () => FinishTransport(portalTo);
    }

    private void FinishTransport(Transform portalTo)
    {
        Debug.Log("Finishing the transport");
       
        you.transform.rotation = portalTo.rotation;
        you.transform.position = portalTo.position + you.transform.forward * offsetDistance;
        
        //Rigidbody r = you.gameObject.GetComponentInChildren<Rigidbody>();
        //if (r == null) Debug.LogError("No rigidbody found!!!");
        rigidBody.AddForce(you.forward * launchForce, ForceMode.Impulse);
    }

    private void SetGun(GunSettings gunSetting)
    {
        currentGunSetting = gunSetting;
        if (gunSetting == GunSettings.Blue)
        {
            needle.localEulerAngles = new Vector3(0, bluePosition, 0);
            gunBody.GetComponent<Renderer>().material = blueMaterial;
            CurrentPortal = BluePortal;
        }
        else
        {
            needle.localEulerAngles = new Vector3(0, orangePosition, 0);
            gunBody.GetComponent<Renderer>().material = orangeMaterial;
            CurrentPortal = OrangePortal;
        }
    }

    private void HandleParticles()
    {
        if (BluePortal is not null && OrangePortal is not null)
        {
            Debug.Log("Turning particles off.");
            BluePortal.GetComponent<PortalController>().ShutDownParticles();
            OrangePortal.GetComponent<PortalController>().ShutDownParticles();
        }
        else
        {
            Debug.Log("Skipping particle turnoff");
        }
    }
}
