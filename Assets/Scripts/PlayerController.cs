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
    private KeyCode keyCodeRun = KeyCode.LeftShift; // �޸��� Ű
    [SerializeField]
    private KeyCode keyCodeJump = KeyCode.Space; // ���� Ű
    [SerializeField]
    private KeyCode keyCodeReload = KeyCode.R; // ź ������ Ű

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioCilpWalk; // �ȱ� ����
    [SerializeField]
    private AudioClip audioClipRun; // �ٱ� ����

    private RotateToMouse rotateToMouse; // ���콺 �̵����� ī�޶� ȸ��
    private MovementCharacterController movement; // Ű���� �Է����� �÷��̾� �̵�, ����
    private Status status; // �̵��ӵ� ���� �÷��̾� ����
    private AudioSource audioSource; // ���� ��� ����
    private WeaponBase weapon;			// ��� ���Ⱑ ��ӹ޴� ��� Ŭ����

    private bool isDie;

    private void Awake()
    {
        // ���콺 ī���� ������ �ʰ� �����ϰ�, ���� ��ġ�� ������Ų��
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

        // �̵��� �� �� (�ȱ� or �ٱ�)
        if (isDie == false && (x != 0 || z != 0))
        {
            bool isRun = false;

            // ���̳� �ڷ� �̵��� ���� �޸� �� ������ ����
            if (z > 0) isRun = Input.GetKey(keyCodeRun); // ���� ����ƮŰ�� ������ isRun�� true ���� ��

            // isRun�� true�̸� �̵��ӵ��� RunSpeed����, �ƴϸ� WalkSpeed����
            movement.MoveSpeed = (isRun == true) ? status.RunSpeed : status.WalkSpeed;
            weapon.Animator.MoveSpeed = isRun == true ? 1 : 0.5f;
            audioSource.clip = (isRun == true) ? audioClipRun : audioCilpWalk; // �۶��� �޸��� �Ҹ�, �������� �ȴ� �Ҹ�

            // ����Ű �Է� ���δ� �� ������ Ȯ���ϱ� ������
            // ���� ������� ���� �ٽ� ������� �ʵ��� isPlaying���� Ȯ���ϰ� ���� ���
            if (audioSource.isPlaying == false) // ������� �ƴϸ�!
            {
                audioSource.loop = true; // �ݺ� ����ϵ��� ����
                audioSource.Play();
            }
        }
        // ���ڸ��� �������� ��
        else
        {
            movement.MoveSpeed = 0;
            weapon.Animator.MoveSpeed = 0;

            // ������ �� ���尡 ������̸� ����
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
        if (isDie == false && Input.GetMouseButtonDown(0)) // ���콺 ���� ��ư�� ������ ��
        {
            weapon.StartWeaponAction();
        }
        else if (Input.GetMouseButtonUp(0)) // ���콺 ���� ��ư�� ���� ��
        {
            weapon.StopWeaponAction();
        }

        if (isDie == false && Input.GetMouseButtonDown(1)) // ���콺 ������ ��ư�� ������ ��
        {
            weapon.StartWeaponAction(1);
        }
        else if (Input.GetMouseButtonUp(1)) // ���콺 ������ ��ư�� ���� ��
        {
            weapon.StopWeaponAction(1);
        }


        if (isDie == false && Input.GetKeyDown(keyCodeReload)) // RŰ�� ������ ��
        {

            weapon.StartReload(); // ������
        }
    }

    public void TakeDamage(int damage) // �÷��̾ ���ݹ޾��� �� ȣ���ϴ� �޼ҵ�
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
