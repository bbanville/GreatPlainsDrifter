using Assets.Scripts.Managers;
using Assets.Scripts.Weapons;
using BrendonBanville.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;

public class TargetPointer : Singleton<TargetPointer>
{
    public GameObject target;
    public Transform guntip;

    [Header("Crosshair")]
    public RectTransform crosshair;
    public Texture2D crosshairTexture;
    public Image crosshairSprite;
    [Tooltip("The path to the crosshair sprite")]
    public string crosshairPath = "Sprites/TestCrosshairRotatOnly.png";
    public Vector2 shotOffset;
    private Animator _crosshairAnimator;
    [HideInInspector] public int crosshairFocus;
    //public LayerMask targetsToIgnore;

    private BaseGun _playerGun;
    private Vector3 targetPoint;
    private Vector3 cursorPos;

    // Start is called before the first frame update
    void Start()
    {
        _crosshairAnimator = crosshair.GetComponent<Animator>();
        //_crosshairCamera = Camera.main;

        _playerGun = DuelingManager.Instance._playerGun.GetComponent<BaseGun>();
        guntip = _playerGun.Tip;
    }

    // Update is called once per frame
    void Update()
    {
        crosshairFocus = Mathf.RoundToInt(DuelingManager.Instance.playerFocus * 100);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        
        MoveCrosshair();
        SetCrosshair();


        if (_crosshairAnimator.GetCurrentAnimatorStateInfo(0).IsName("FadeCrosshair"))
        {
            SetAnimFrame("ResizeCrosshair", 0, crosshairFocus);
            //SetAnimFrame("RotateCrosshair", 2, crosshairFocus);
        }

        if (DuelingManager.Instance.gunDrawn)
        {
            targetPoint = ShotPoint();
            //targetPoint.x += shotOffset.x;
            //targetPoint.y += shotOffset.y;
        }

        if (targetPoint != null)
        {
            guntip.transform.LookAt(targetPoint);
        }
    }

    /// <summary>
    /// Sets the crosshair active or inactive
    /// </summary>
    /// <param name="state">If set to <c>true</c> turns the crosshair active, turns it off otherwise.</param>
    public virtual void SetCrosshairActive(bool state)
    {
        if (crosshair != null)
        {
            crosshair.gameObject.SetActive(state);
        }
    }

    public void MoveCrosshair()
    {
        if (!(crosshair.anchorMin == Vector2.zero) || !(crosshair.anchorMax == Vector2.zero))
        {
            if (!(crosshair.anchorMin == Vector2.zero))
            {
                crosshair.anchorMin.Set(0.0f, 0.0f);
            }

            if (!(crosshair.anchorMax == Vector2.zero))
            {
                crosshair.anchorMax.Set(0.0f, 0.0f);
            }
        }

        CanvasScaler scaler = crosshair.GetComponentInParent<CanvasScaler>();
        crosshair.anchoredPosition = new Vector2((Input.mousePosition.x * scaler.referenceResolution.x / Screen.width) + shotOffset.x,
                                                 (Input.mousePosition.y * scaler.referenceResolution.y / Screen.height) + shotOffset.y);
    }

    public Vector3 ShotPoint()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        Debug.DrawRay(camRay.origin, camRay.direction, Color.red);
        if (Physics.Raycast(camRay, out hitInfo))
        {
            guntip.transform.forward = camRay.direction;
            return hitInfo.point;
        }

        return Vector3.zero;
    }

    void SetAnimFrame(string animName, int animLayer, float frameTime)
    {
        var desired_play_time = frameTime;

        _crosshairAnimator.speed = 0f;
        _crosshairAnimator.Play(animName, animLayer, desired_play_time);
    }

    void SetCrosshair()
    {
        Sprite[] crosshairSprites = Resources.LoadAll<Sprite>(crosshairTexture.name);

        if (crosshairFocus == 0)
        {
            crosshairSprite.sprite = crosshairSprites[0];
        }
        else
        {
            crosshairSprite.sprite = crosshairSprites[crosshairFocus - 1];
        }
    }

    void SetCrosshairRotation()
    {
        float crosshairRot = DuelingManager.Instance.playerFocus * 360;
        crosshair.rotation = Quaternion.Euler(0, 0, crosshairRot);
    }

    void SetCrosshairResize()
    {

    }
}
