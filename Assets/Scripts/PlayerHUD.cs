using System.Collections;
using System.Collections.Generic; // �Ϲ�ȭ ����Ʈ ����� ����
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    private WeaponBase weapon;                  // ���� ������ ��µǴ� ����

    [Header("Components")]
    [SerializeField]
    private Status status;					// �÷��̾��� ���� (�̵��ӵ�, ü��)

    [Header("Weapon Base")]
    [SerializeField]
    private TextMeshProUGUI textWeaponName; // ���� �̸�
    [SerializeField]
    private Image imageWeaponIcon; // ���� ������
    [SerializeField]
    private Sprite[] spriteWeaponIcons; // ���� �����ܿ� ���Ǵ� sprite �迭
    [SerializeField]
    private Vector2[] sizeWeaponIcons;		// ���� �������� UI ũ�� �迭

    [Header("Ammo")]
    [SerializeField]
    private TextMeshProUGUI textAmmo; // ����/�ִ� ź �� ��� Text

    [Header("Magazine")]
    [SerializeField]
    private GameObject magazineUIPrefab; // źâ UI ������
    [SerializeField]
    private Transform magazineParent; // źâ UI�� ��ġ�Ǵ� Panel
    [SerializeField]
    private int maxMagazineCount;	// ó�� �����ϴ� �ִ� źâ ��

    private List<GameObject> magazineList; // źâ UI ����Ʈ

    [Header("HP & BloodScreen UI")]
    [SerializeField]
    private TextMeshProUGUI textHP;             // �÷��̾��� ü���� ����ϴ� Text
    [SerializeField]
    private Image imageBloodScreen; // �÷��̾ ���ݹ޾��� �� ȭ�鿡 ǥ�õǴ� Image
    [SerializeField]
    private AnimationCurve curveBloodScreen;

    private void Awake()
    {
        /*SetupWeapon(); // ���� ���� ���� ����
        SetupMagazine(); // ���� źâ ���� ����

        // �޼ҵ尡 ��ϵǾ� �ִ� �̺�Ʈ Ŭ����(weapon.xx)��
        // Invoke() �޼ҵ尡 ȣ��� �� ��ϵ� �޼ҵ�(�Ű�����)�� ����ȴ�
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD); // onAmmoEventŬ������ UpdateAmmoHUD �޼ҵ� ��� 
        weapon.onMagazineEvent.AddListener(UpdateMagazineHUD);*/ // onMagazinEventŬ������ UpdataMagazineHUD �޼ҵ� ���
        status.onHPEvent.AddListener(UpdateHPHUD);
    }

    public void SetupAllWeapons(WeaponBase[] weapons)
    {
        SetupMagazine(); // �ִ� źâ �� ��ŭ UI ����

        // ��� ������ ��� ������ �̺�Ʈ ���
        for (int i = 0; i < weapons.Length; ++i)
        {
            weapons[i].onAmmoEvent.AddListener(UpdateAmmoHUD);
            weapons[i].onMagazineEvent.AddListener(UpdateMagazineHUD);
        }
    }

    public void SwitchingWeapon(WeaponBase newWeapon) // ���ⱳü
    {
        weapon = newWeapon;

        SetupWeapon();
    }

    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString(); // ���� �̸� ���
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName]; // ���� �̹��� ���
        imageWeaponIcon.rectTransform.sizeDelta = sizeWeaponIcons[(int)weapon.WeaponName]; // ���� �̹��� �ٲ� �̹��� ũ�⵵ ����
    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}"; // textAmmo�� �Ű������� �޾ƿ� �� ���
    }

    private void SetupMagazine()
    {
        // weapon�� ��ϵǾ� �ִ� �ִ� źâ ������ŭ Image Icon�� ����
        // magazineParent ������Ʈ�� �ڽ����� ��� �� ��� ��Ȱ��ȭ/����Ʈ�� ����
        magazineList = new List<GameObject>();
        for (int i = 0; i < maxMagazineCount; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab); // ������ �ִ� źâ �� ��ŭ źâ UI ����
            clone.transform.SetParent(magazineParent); // ������ źâ UI�� �θ� ������Ʈ�� magazineParent�� ����
            clone.SetActive(false); // ��Ȱ��ȭ

            magazineList.Add(clone); // magazineList�� ����
        }
    }

    private void UpdateMagazineHUD(int currentMagazine)
    {
        // ���� ��Ȱ��ȭ�ϰ�, currentMagazine ������ŭ Ȱ��ȭ
        for (int i = 0; i < magazineList.Count; ++i)
        {
            magazineList[i].SetActive(false); // magazineList�� ����Ǿ� �ִ� ��� źâ UI�� ��Ȱ��ȭ
        }
        for (int i = 0; i < currentMagazine; ++i)
        {
            magazineList[i].SetActive(true); // currentMagazine ������ŭ źâ UI Ȱ��ȭ
        }
    }

    private void UpdateHPHUD(int previous, int current)
    {
        textHP.text = "HP " + current;

        // ü���� �������� ���� ȭ�鿡 ������ �̹����� ������� �ʵ��� return
        if (previous <= current) return;

        if (previous - current > 0)
        {
            StopCoroutine("OnBloodScreen");
            StartCoroutine("OnBloodScreen");
        }
    }

    private IEnumerator OnBloodScreen()
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime;

            Color color = imageBloodScreen.color;
            color.a = Mathf.Lerp(1, 0, curveBloodScreen.Evaluate(percent));
            imageBloodScreen.color = color;

            yield return null;
        }
    }
}
