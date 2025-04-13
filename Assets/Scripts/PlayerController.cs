using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject DieTextObject;

    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keyCodeRun = KeyCode.LeftShift; // 달리기 키
    [SerializeField]
    private KeyCode keyCodeJump = KeyCode.Space; // 점프 키
    [SerializeField]
    private KeyCode keyCodeReload = KeyCode.R; // 탄 재장전 키

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioCilpWalk; // 걷기 사운드
    [SerializeField]
    private AudioClip audioClipRun; // 뛰기 사운드

    private RotateToMouse rotateToMouse; // 마우스 이동으로 카메라 회전
    private MovementCharacterController movement; // 키보드 입력으로 플레이어 이동, 점프
    private Status status; // 이동속도 등의 플레이어 정보
    private AudioSource audioSource; // 사운드 재생 제어
    private WeaponBase weapon;			// 모든 무기가 상속받는 기반 클래스

    private bool isDie;

    private void Awake()
    {
        // 마우스 카서를 보이지 않게 설정하고, 현재 위치에 고정시킨다
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    
        rotateToMouse = GetComponent<RotateToMouse>();
        movement = GetComponent<MovementCharacterController>();
        status = GetComponent<Status>();
        audioSource = GetComponent<AudioSource>();

        DieTextObject.SetActive(false);
        isDie = false;
    }

    private void Update()
    {
        UpdateRotate();
        UpdateMove();
        UpdateJump();
        UpdateWeaponAction();

        if(isDie && Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene("MYFPSGAME sin#1");
        }
    }

    private void UpdateRotate() 
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }

    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // 이동중 일 때 (걷기 or 뛰기)
        if (isDie == false && (x != 0 || z != 0))
        {
            bool isRun = false;

            // 옆이나 뒤로 이동할 때는 달릴 수 없도록 구현
            if (z > 0) isRun = Input.GetKey(keyCodeRun); // 왼쪽 쉬프트키를 누르면 isRun에 true 저장 됨

            // isRun이 true이면 이동속도를 RunSpeed적용, 아니면 WalkSpeed적용
            movement.MoveSpeed = (isRun == true) ? status.RunSpeed : status.WalkSpeed;
            weapon.Animator.MoveSpeed = isRun == true ? 1 : 0.5f;
            audioSource.clip = (isRun == true) ? audioClipRun : audioCilpWalk; // 뛸때는 달리는 소리, 걸을때는 걷는 소리

            // 방향키 입력 여부는 매 프레임 확인하기 때문에
            // 사운드 재생중일 때는 다시 재생하지 않도록 isPlaying으로 확인하고 사운드 재생
            if (audioSource.isPlaying == false) // 재생중이 아니면!
            {
                audioSource.loop = true; // 반복 재생하도록 설정
                audioSource.Play();
            }
        }
        // 제자리에 멈춰있을 때
        else
        {
            movement.MoveSpeed = 0;
            weapon.Animator.MoveSpeed = 0;

            // 멈췄을 때 사운드가 재생중이면 정지
            if (audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }
    
        movement.MoveTo(new Vector3(x, 0, z));
    }

    private void UpdateJump()
    {
        if (isDie == false&&Input.GetKeyDown(keyCodeJump))
        {
            movement.Jump();
        }
    }

    private void UpdateWeaponAction()
    {
        if (isDie == false && Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼을 눌렀을 때
        {
            weapon.StartWeaponAction();
        }
        else if (Input.GetMouseButtonUp(0)) // 마우스 왼쪽 버튼을 땟을 때
        {
            weapon.StopWeaponAction();
        }

        if (isDie == false && Input.GetMouseButtonDown(1)) // 마우스 오른쪽 버튼을 눌렀을 때
        {
            weapon.StartWeaponAction(1);
        }
        else if (Input.GetMouseButtonUp(1)) // 마우스 오른쪽 버튼을 땟을 때
        {
            weapon.StopWeaponAction(1);
        }


        if (isDie == false && Input.GetKeyDown(keyCodeReload)) // R키를 눌렀을 때
        {

            weapon.StartReload(); // 재장전
        }
    }

    public void TakeDamage(int damage) // 플레이어가 공격받았을 때 호출하는 메소드
    {
        isDie = status.DecreaseHP(damage);

        if (isDie == true)
        {
            DieTextObject.SetActive(true);
        }
    }

    public void SwitchingWeapon(WeaponBase newWeapon)
    {
        weapon = newWeapon;
    }
}
