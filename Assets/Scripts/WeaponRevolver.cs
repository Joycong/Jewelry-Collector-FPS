using System.Collections;
using UnityEngine;

public class WeaponRevolver : WeaponBase
{
    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect;   // �ѱ� ����Ʈ (On/Off)

    [Header("Spawn Points")]
    [SerializeField]
    private Transform bulletSpawnPoint; // �Ѿ� ���� ��ġ

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipFire;        // ���� ����
    [SerializeField]
    private AudioClip audioClipReload;    // ���� ����

    private ImpactMemoryPool impactMemoryPool;  // ���� ȿ�� ���� �� Ȱ��/��Ȱ�� ����
    private Camera mainCamera;          // ���� �߻�

    private void OnEnable()
    {
        // �ѱ� ����Ʈ ������Ʈ ��Ȱ��ȭ
        muzzleFlashEffect.SetActive(false);

        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ źâ ������ �����Ѵ�
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ �����Ѵ�
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

        ResetVariables();
    }

    private void Awake()
    {
        base.Setup();

        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        // ó�� źâ ���� �ִ�� ����
        weaponSetting.currentMagazine = weaponSetting.maxMagazine;
        // ó�� ź ���� �ִ�� ����
        weaponSetting.currentAmmo = weaponSetting.maxAmmo;
    }

    public override void StartWeaponAction(int type = 0)
    {
        if (type == 0 && isAttack == false && isReload == false)
        {
            OnAttack();
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        isAttack = false;
    }

    public override void StartReload()
    {
        // ���� ������ ���̰ų� źâ ���� 0�̸� ������ �Ұ���
        if (isReload == true || weaponSetting.currentMagazine <= 0) return;

        // ���� �׼� ���߿� 'R'Ű�� ���� �������� �õ��ϸ� ���� �׼� ���� �� ������
        StopWeaponAction();

        StartCoroutine("OnReload");
    }

    public void OnAttack()
    {
        if (Time.time - lastAttackTime > weaponSetting.attackRate)
        {
            // �ٰ����� ���� ������ �� ����
            if (animator.MoveSpeed > 0.5f)
            {
                return;
            }

            // �����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
            lastAttackTime = Time.time;

            // ź ���� ������ ���� �Ұ���
            if (weaponSetting.currentAmmo <= 0)
            {
                return;
            }
            // ���ݽ� currentAmmo 1 ����, ź �� UI ������Ʈ
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            // ���� �ִϸ��̼� ���
            animator.Play("Fire", -1, 0);
            // �ѱ� ����Ʈ ���
            StartCoroutine("OnMuzzleFlashEffect");
            // ���� ���� ���
            PlaySound(audioClipFire);

            // ������ �߻��� ���ϴ� ��ġ ����
            TwoStepRaycast();
        }
    }

    private IEnumerator OnMuzzleFlashEffect()
    {
        muzzleFlashEffect.SetActive(true);

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f);

        muzzleFlashEffect.SetActive(false);
    }

    private IEnumerator OnReload()
    {
        isReload = true;

        // ������ �ִϸ��̼�, ���� ���
        animator.OnReload();
        PlaySound(audioClipReload);

        while (true)
        {
            // ���尡 ������� �ƴϰ�, ���� �ִϸ��̼��� Movement�̸�
            // ������ �ִϸ��̼�(, ����) ����� ����Ǿ��ٴ� ��
            if (audioSource.isPlaying == false && animator.CurrentAnimationIs("Movement"))
            {
                isReload = false;

                // ���� źâ ���� 1 ���ҽ�Ű��, �ٲ� źâ ������ Text UI�� ������Ʈ
                weaponSetting.currentMagazine--;
                onMagazineEvent.Invoke(weaponSetting.currentMagazine);

                // ���� ź ���� �ִ�� �����ϰ�, �ٲ� ź �� ������ Text UI�� ������Ʈ
                weaponSetting.currentAmmo = weaponSetting.maxAmmo;
                onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

                yield break;
            }

            yield return null;
        }
    }

    private void TwoStepRaycast()
    {
        Ray ray;
        RaycastHit hit;
        Vector3 targetPoint = Vector3.zero;

        // ȭ���� �߾� ��ǥ (Aim �������� Raycast ����)
        ray = mainCamera.ViewportPointToRay(Vector2.one * 0.5f);
        // ���� ��Ÿ�(attackDistance) �ȿ� �ε����� ������Ʈ�� ������ targetPoint�� ������ �ε��� ��ġ
        if (Physics.Raycast(ray, out hit, weaponSetting.attackDistance))
        {
            targetPoint = hit.point;
        }
        // ���� ��Ÿ� �ȿ� �ε����� ������Ʈ�� ������ targetPoint�� �ִ� ��Ÿ� ��ġ
        else
        {
            targetPoint = ray.origin + ray.direction * weaponSetting.attackDistance;
        }

        // ù��° Raycast�������� ����� targetPoint�� ��ǥ�������� �����ϰ�,
        // �ѱ��� ������������ �Ͽ� Raycast ����
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized;
        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance))
        {
            impactMemoryPool.SpawnImpact(hit);

            if (hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
            else if (hit.transform.CompareTag("InteractionObject"))
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage);
            }
        }
    }

    private void ResetVariables()
    {
        isReload = false;
        isAttack = false;
    }
}

