using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Daftar Senjata (isi dengan kartu)")]
    public WeaponData[] weapons;
    private int currentIndex = 0;

    [Header("Muzzle Flash")]
    public GameObject muzzleFlash;
    public float flashDuration = 0.05f;

    [Header("Camera Shake")]
    public float shakeDuration = 0.1f;

    private float nextFireTime;
    public SpriteRenderer sr;

    private WeaponData Current => weapons[currentIndex];

    private void Awake()
    {
    if (sr == null)
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        Equip(0);
    }

    private void Update()
    {
        RotateToMouse();
        SwitchWeapon();
        Shoot();
    }

    // Tekan angka 1, 2, 3, 4 untuk ganti senjata
    private void SwitchWeapon()
    {
        for (int i = 0; i < weapons.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                Equip(i);
        }
    }

    private void Equip(int index)
    {
        if (weapons.Length == 0) return;

        currentIndex = index;

        // ganti gambar senjata kalau kartunya punya gambar
        if (sr != null && Current.weaponSprite != null)
            sr.sprite = Current.weaponSprite;
    }

    private void RotateToMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePosition - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Vector3 scale = transform.localScale;

        if (angle > 90f || angle < -90f)
            scale.y = -Mathf.Abs(scale.y);
        else
            scale.y = Mathf.Abs(scale.y);

        transform.localScale = scale;
    }

    private void Shoot()
    {
        if (weapons.Length == 0) return;

        // automatic = tahan klik, kalau tidak = harus klik satu-satu
        bool wantsToFire = Current.automatic
            ? Input.GetMouseButton(0)
            : Input.GetMouseButtonDown(0);

        if (!wantsToFire || Time.time < nextFireTime) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // keluarkan peluru sebanyak "bulletsPerShot"
        for (int i = 0; i < Current.bulletsPerShot; i++)
        {
            float offset = Random.Range(-Current.spreadAngle / 2f, Current.spreadAngle / 2f);

            GameObject bullet = Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.Euler(0f, 0f, baseAngle - 90f + offset)
            );

            // titip angka dari kartu ke peluru
            Projectile p = bullet.GetComponent<Projectile>();
            p.damage = Current.damage;
            p.speed = Current.bulletSpeed;
            p.lifeTime = Current.bulletLifetime;
            p.pierce = Current.pierce;
        }

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(shakeDuration, Current.shakeMagnitude);

        if (muzzleFlash != null)
            StartCoroutine(ShowMuzzleFlash());

        nextFireTime = Time.time + Current.fireRate;
    }

    private IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(flashDuration);
        muzzleFlash.SetActive(false);
    }
}