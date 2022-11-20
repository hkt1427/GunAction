using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent (typeof(GunController))]

public class Player : LivingEntity
{
    float x, z;

    float moveSpeed = 3f;
    public float speed = 0.05f;

    public float YrotSpeed = 3f;
    public float XrotSpeed = 1f;

    public FixedJoystick inputMove;
    public FloatingJoystick inputRot;
    public FixedJoystick inputRotGun;

    public GameObject camera;
    Quaternion cameraRot;
    Quaternion characterRot;

    float Xsensityvity = 0.5f;
    float Ysensityvity = 2.2f;
    public float XsensityvityStick = 1f;
    public float YsensityvityStick = 2f;

    //角度
    float minX = -60f;
    float maxX = 60f;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;
    GameManager gameManager;

    bool push = false;
    bool stop = false;

    protected override void Start()
    {
        base.Start();

        cameraRot = camera.transform.localRotation;
        characterRot = transform.localRotation;

        stop = false;
    }

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
    }

    void Update()
    {
        //ジョイコン操作でのプレイヤーの移動
        this.transform.position += this.transform.forward * inputMove.Vertical * moveSpeed * Time.deltaTime;
        this.transform.position += this.transform.right * inputMove.Horizontal * moveSpeed * Time.deltaTime;

        //pc用
        // float xRot = Input.GetAxis("Mouse X") * YrotSpeed;  //x方向の視点回転
        // float yRot = Input.GetAxis("Mouse Y") * XrotSpeed;  //y方向の視点回転

        //スマホ用
        float xRot = inputRot.Horizontal * Ysensityvity;
        float yRot = inputRot.Vertical * Xsensityvity;

        //射撃ボタンを押しながら視点回転
        float xRotGun = inputRotGun.Horizontal * YsensityvityStick;
        float yRotGun = inputRotGun.Vertical * XsensityvityStick;

        cameraRot *= Quaternion.Euler(-yRot - yRotGun, 0, 0);
        characterRot *= Quaternion.Euler(0, xRot + xRotGun, 0);

        //Updateで作成した関数を呼ぶ
        cameraRot = ClampRotation(cameraRot);

        camera.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;
        
        

        //Weapon input
        if (push)
        {
            gunController.OnTriggerHold();  //射撃ボタン
        }
        if (Input.GetMouseButton(0))
        {
            // gunController.OnTriggerHold();  //画面長押しで射撃
        }

        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }

        //ステージから落下
        if (transform.position.y < -10)
        {
            gameManager.GameOver();
        }
    }

    private void FixedUpdate()
    {
        x = 0;
        z = 0;

        //キーボード操作でのプレイヤー移動
        x = Input.GetAxis("Horizontal") * speed;
        z = Input.GetAxis("Vertical") * speed;

        transform.position += camera.transform.forward * z + camera.transform.right * x;

        //Look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);

            controller.LookAt(point);
            gunController.Aim(point);

            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                gunController.Aim(point);
            }
        }

        if (stop)
        {
            speed = 0;
            YrotSpeed = 0;
            XrotSpeed = 0;
            push = false;
        }
    }

    public void StopPlayer()
    {
        stop = true;
    }

    

    public Quaternion ClampRotation(Quaternion q)
    {
        //q = x,y,z,w

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX, minX, maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }

    public void OnButtonDown()
    {
        push = true;
    }

    public void OnButtonUp()
    {
        push = false;
    }

    public void OnReloadButton()
    {
        gunController.Reload();
    }
}
