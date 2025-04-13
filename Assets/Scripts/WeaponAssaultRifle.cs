using System.Collections; // �ڷ�ƾ ����� ����
using UnityEngine;
using UnityEngine.UI;

/*[System.Serializable]
public class AmmoEvent : UnityEngine.Events.UnityEvent<int, int> { } // ������ ź �� ������ �ٲ� �� ���� �ܺο� �ִ� �޼ҵ带 �ڵ� ȣ���� �� �ֵ��� �̺�Ʈ Ŭ���� ����

[System.Serializable]
public class MagazineEvent : UnityEngine.Events.UnityEvent<int> { }*/ // ������ źâ �� ������ �ٲ� ���� �ܺο� �ִ� �޼ҵ带 �ڵ� ȣ���� �� �ֵ��� �̺�Ʈ Ŭ���� ����
public class WeaponAssaultRifle : WeaponBase //MonoBehaviour
{
    /*[HideInInspector]
    public AmmoEvent onAmmoEvent = new AmmoEvent();
    [HideInInspector]
    public MagazineEvent onMagazineEvent = new MagazineEvent();*/

    [Header("Fire Effects")]
    [SerializeField]
    private GameObject muzzleFlashEffect; // �ѱ� ����Ʈ (On/Off)

    [Header("Spawn Points")]
    [SerializeField]
    private Transform casingSpawnPoint; // ź�� ���� ��ġ
    [SerializeField]
    private Transform bulletSpawnPoint; // �Ѿ� ���� ��ġ

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipTakeOutWeapon; // ���� ���� ����
    [SerializeField]
    private AudioClip audioClipFire; // ���� ����
    [SerializeField]
    private AudioClip audioClipReload; // ������ ����

    /*[Header("Weapon Setting")]
    [SerializeField]
    private WeaponSetting weaponSetting; // ���� ����*/

    [Header("Aim UI")]
    [SerializeField]
    private Image imageAim; // default/aim ��忡 ���� Aim �̹��� Ȱ��/��Ȱ��

   /* private float lastAttackTime = 0; // ������ �߻�ð� üũ��
    private bool isReload = false; // ������ ������ üũ
    private bool isAttack = false;*/ // ���� ���� üũ��
    private bool isModeChange = false; // ��� ��ȯ ���� üũ��
    private float defaultModeFOV = 60; // �⺻��忡���� ī�޶� FOV
    private float aimModeFOV = 30; // AIM��忡���� ī�޶� FOV

    /*private AudioSource audioSource; // ���� ��� ������Ʈ
    private PlayerAnimatorController animator;*/ // �ִϸ��̼� ��� ����
    private CasingMemoryPool casingMemoryPool; // ź�� ���� �� Ȱ��/��Ȱ�� ����
    private ImpactMemoryPool impactMemoryPool; // ���� ȿ�� ���� �� Ȱ��/��Ȱ�� ����
    private Camera mainCamera; // ���� �߻�

   /* // �ܺο��� �ʿ��� ������ �����ϱ� ���� ������ Get Property's
    public WeaponName WeaponName => weaponSetting.weaponName;
    public int CurrentMagazine => weaponSetting.currentMagazine;
    public int MaxMagazine => weaponSetting.maxMagazine;*/

    private void Awake()
    {
        // ��� Ŭ������ �ʱ�ȭ�� ���� Setup() �޼ҵ� ȣ��
        base.Setup();

       /* audioSource = GetComponent<AudioSource>();
        animator = GetComponentInParent<PlayerAnimatorController>();*/
        casingMemoryPool = GetComponent<CasingMemoryPool>();
        impactMemoryPool = GetComponent<ImpactMemoryPool>();
        mainCamera = Camera.main;

        //ó�� źâ ���� �ִ�� ����
        weaponSetting.currentMagazine = weaponSetting.maxMagazine; // ���� źâ ���� �ִ� źâ ���� �������Ͽ� ó�� źâ ���� �ִ��
        // ó�� ź ���� �ִ�� ����
        weaponSetting.currentAmmo = weaponSetting.maxAmmo; // ���� ź ���� �ִ� ź���� �������Ͽ� ó�� ź ���� �ִ��
    }

    private void OnEnable()
    {
        // ���� ���� ���� ���
        PlaySound(audioClipTakeOutWeapon);
        // �ѱ� ����Ʈ ������Ʈ ��Ȱ��ȭ
        muzzleFlashEffect.SetActive(false);

        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ źâ ������ �����Ѵ�.
        onMagazineEvent.Invoke(weaponSetting.currentMagazine);
        // ���Ⱑ Ȱ��ȭ�� �� �ش� ������ ź �� ������ �����Ѵ�
        onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo); // �̺�Ʈ Ŭ������ ��ϵ� �ܺ� �޼ҵ尡 ȣ��Ǵ� ������ �̺�Ʈ Ŭ������ Invoke�޼ҵ尡 ȣ��� ����

        ResetVariables();
    }

    public override void StartWeaponAction(int type = 0)
    {
        // ������ ���� ���� ���� �׼��� �� �� ����
        if(isReload == true) return;

        // ��� ��ȯ���̸� ���� �׼��� �� �� ����
        if (isModeChange == true) return;

        // ���콺 ���� Ŭ�� (���� ����)
        if (type == 0)
        {
            // ���� ����
            if (weaponSetting.isAutomaticAttack == true)
            {
                isAttack = true; // ���������� �˸��� �뵵
                StartCoroutine("OnAttackLoop"); // �ڷ�ƾ �޼ҵ� ȣ��
            }
            // �ܹ� ����
            else
            {
                OnAttack(); // ���� ������ ���� �� �޼ҵ�
            }
        }
        // ���콺 ������ Ŭ�� (��� ��ȯ)
        else
        {
            // ���� ���� ���� ��� ��ȯ�� �� �� ����
            if (isAttack == true) return;

            StartCoroutine("OnModeChange"); // ��� ��ȯ
        }
    }

    public override void StopWeaponAction(int type = 0)
    {
        // ���콺 ���� Ŭ�� (���� ����)
        if (type == 0)
        {
            isAttack = false; // �������� �ƴ��� �˸��� �뵵
            StopCoroutine("OnAttackLoop"); // �ܹ��϶� ��� ������ ���Ӱ����� �ϴ� �ڷ�ƾ �޼ҵ常 ����
        }
    }

    public override void StartReload()
    { 
        // ���� ������ ���̰ų� źâ ���� 0�̸� ������ �Ұ���
        if (isReload == true || weaponSetting.currentMagazine <= 0) return;

        // ���� �׼� ���߿� 'R'Ű�� ���� �������� �õ��ϸ� ���� �׼� ���� �� ������
        StopWeaponAction(); // ���� �׼� ����
        
        StartCoroutine("OnReload"); // ������
    }
    private IEnumerator OnAttackLoop() // ���� ���� ������ OnAttack()�� �� ������ ����
    {
        while (true)
        {
            OnAttack();

            yield return null;
        }
    }

    public void OnAttack() // ���� ���� ����
    {
        if (Time.time - lastAttackTime > weaponSetting.attackRate) // ����ð� - lastAttackTime > weaponSetting.attackRate�� �� ���� �� weaponSetting.attackRate�ð����� ������
        {
            // �ٰ����� ���� ������ �� ������
            if (animator.MoveSpeed > 0.5f)
            {
                return;
            }

            // �����ֱⰡ �Ǿ�� ������ �� �ֵ��� �ϱ� ���� ���� �ð� ����
            lastAttackTime = Time.time;

            //ź ���� ������ ���� �Ұ���
            if (weaponSetting.currentAmmo <= 0)
            {
                return;
            }
            // ���ݽ� currentAmmo 1 ����, ź �� UI ������Ʈ
            weaponSetting.currentAmmo--;
            onAmmoEvent.Invoke(weaponSetting.currentAmmo, weaponSetting.maxAmmo);

            // ���� �ִϸ��̼� ��� (��忡 ���� AimFire or Fire �ִϸ��̼� ���)
            //animator.Play("Fire", -1, 0); // ���� �ִϸ��̼��� �ݺ��� �� �ִϸ��̼��� ���� ó������ �ٽ� ���
            string animation = (animator.AimModeIs == true) ? "AimFire" : "Fire";
            animator.Play(animation, -1, 0);
            // �ѱ� ����Ʈ ��� (default mode �� ���� ���)
            if(animator.AimModeIs == false) StartCoroutine("OnMuzzleFlashEffect"); // OnMuzzleFlashEffect() �ڷ�ƾ �޼ҵ� ȣ��
            // ���� ���� ���
            PlaySound(audioClipFire);
            // ź�� ����
            casingMemoryPool.SpawnCasing(casingSpawnPoint.position, transform.right);

            // ������ �߻��� ���ϴ� ��ġ ����(+Impact Effect)
            TwoStepRaycast();
        }
    }

    private IEnumerator OnMuzzleFlashEffect() // ���� ���ݼӵ����� ������ ���� ��� ����Ʈ Ȱ��ȭ
    {
        muzzleFlashEffect.SetActive(true); // �ѱ� ����Ʈ Ȱ��ȭ

        yield return new WaitForSeconds(weaponSetting.attackRate * 0.3f); // ���� ��� ����� ��

        muzzleFlashEffect.SetActive(false); // �ѱ� ����Ʈ ��Ȱ��ȭ
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
            if (audioSource.isPlaying == false && (animator.CurrentAnimationIs("Movement")))
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
        Debug.DrawRay(ray.origin, ray.direction * weaponSetting.attackDistance, Color.red); // �׽�Ʈ�� ����� (�� �信�� ���� ������ �������� Ȯ��)

        // ù��° Raycast�������� ����� targetPoint�� ��ǥ�������� �����ϰ�,
        // �ѱ��� ������������ �Ͽ� Raycast ����
        Vector3 attackDirection = (targetPoint - bulletSpawnPoint.position).normalized; // ���� ���� �������� ���
        if (Physics.Raycast(bulletSpawnPoint.position, attackDirection, out hit, weaponSetting.attackDistance)) // ������ ��ġ���� ���� �𷢼� �������� ���� ���Ͻ� ���̸�ŭ ������ �߻��Ͽ� �浹�ϴ� ������Ʈ�� ������
        {
            impactMemoryPool.SpawnImpact(hit); // Ÿ�� ����Ʈ ����

            if (hit.transform.CompareTag("ImpactEnemy"))
            {
                hit.transform.GetComponent<EnemyFSM>().TakeDamage(weaponSetting.damage);
            }
            else if (hit.transform.CompareTag("InteractionObject")) // �÷��̾��� ���ݿ� �ε��� ������Ʈ �±װ� InteractionObject�̸�
            {
                hit.transform.GetComponent<InteractionObject>().TakeDamage(weaponSetting.damage); // weaponSetting.damage��ŭ ü�� �����ϵ���
            }
        }
        Debug.DrawRay(bulletSpawnPoint.position, attackDirection*weaponSetting.attackDistance, Color.blue); // �׽�Ʈ�� ����� (�� �信�� ���� ������ �������� Ȯ��)

    }

    private IEnumerator OnModeChange()
    {
        float current = 0;
        float percent = 0;
        float time = 0.35f;

        animator.AimModeIs = !animator.AimModeIs; // ���� ��� �Ķ���͸� �ݴ�� ����
        imageAim.enabled = !imageAim.enabled; // ���� �̹����� ���̸� ������ �ʵ���, ������ ������ ���̵���

        float start = mainCamera.fieldOfView; // ���� ī�޶��� FOV��
        float end = (animator.AimModeIs == true) ? aimModeFOV : defaultModeFOV; // ���� ��� �Ķ���Ϳ� ���� FOV�� ����

        isModeChange = true; // ��� �����ϴ� ���� ������ �� ������

        while (percent < 1) // time�ð����� FOV�� start���� end���� ����
        { 
            current += Time.deltaTime;
            percent = current / time;

            // mode�� ���� ī�޶��� �þ߰��� ����
            mainCamera.fieldOfView = Mathf.Lerp(start, end, percent);

            yield return null;
        }

        isModeChange = false;
    }

    private void ResetVariables()
    { 
        isReload = false;
        isAttack = false;
        isModeChange = false;
    }
    
}
