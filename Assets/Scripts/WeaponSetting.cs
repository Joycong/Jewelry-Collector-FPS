// ������ ������ ���� ������ �� �������� ����ϴ� �������� ��� �����ϸ�
// ������ �߰�/������ �� ����ü�� �����ϱ� ������ �߰�/������ ���� ������ ������

public enum WeaponName { AssaultRifle = 0, Revolver, CombatKnife, HandGrenade } // ���� �̸��� ��Ÿ���� WeaponName{} ����

[System.Serializable] //�̷��� ����ȭ���� ������ �ٸ� Ŭ������ ������ �����Ǿ��� �� Inspector View�� ��� �������� ����� ���� �ʴ´�
public class WeaponSetting
{
    public WeaponName weaponName; // ���� �̸�
    public int damage; // ���� ���ݷ�
    public int currentMagazine; // ���� źâ ��
    public int maxMagazine; // �ִ� źâ ��
    public int currentAmmo; // ���� ź�� ��
    public int maxAmmo; // �ִ� ź�� ��
    public float attackRate; //���� �ӵ�
    public float attackDistance; // ���� ��Ÿ�
    public bool isAutomaticAttack; // ���� ���� ����
}
