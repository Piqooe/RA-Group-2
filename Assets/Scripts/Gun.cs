using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public SpriteRenderer sr;          // tarik objek WeaponSprite ke sini

    [Header("Daftar Senjata (isi dengan kartu)")]
    public WeaponData[] weapons;
    private int currentIndex = 0;

    [Header("Muzzle Flash")]
    public GameObject muzzleFlash;
    public float flashDuration = 0.05f;

    [Header("Camera Shake")]
    public float shakeDuration = 0.1f;

    private float nextFireTime;
    private Coroutine animRoutine;
    [Header("Preview di Edit Mode")]
    [Range(0, 3)] public int previewIndex = 0;
    [HideInInspector] public int lastPreviewIndex = -1;

    private WeaponData Current => weapons[currentIndex];

    private void Awake()
    {
        if (sr == null)
        {
            Transform t = transform.Find("WeaponSprite");
            if (t != null) sr = t.GetComponent<SpriteRenderer>();
        }
    }

    // Dipanggil saat nilai di Inspector berubah (tanpa Play)
    private void OnValidate()
    {
    #if UNITY_EDITOR
        if (Application.isPlaying) return;
        if (previewIndex == lastPreviewIndex) return;
        lastPreviewIndex = previewIndex;

        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            if (weapons == null || previewIndex >= weapons.Length || weapons[previewIndex] == null) return;

            WeaponData w = weapons[previewIndex];

            if (sr != null && w.weaponSprite != null)
                sr.sprite = w.weaponSprite;

            if (firePoint != null)
                firePoint.localPosition = w.firePointPosition;
        };
    #endif
    }

    // Klik kanan atau titik tiga di header Gun (Script) untuk memakai ini
        [ContextMenu("Simpan Fire Point ke Kartu")]
    private void SaveFirePointToCard()
    {
    #if UNITY_EDITOR
        if (weapons == null || previewIndex >= weapons.Length || weapons[previewIndex] == null) return;

        WeaponData w = weapons[previewIndex];
        UnityEditor.Undo.RecordObject(w, "Simpan Fire Point");
        w.firePointPosition = firePoint.localPosition;
        UnityEditor.EditorUtility.SetDirty(w);
        UnityEditor.AssetDatabase.SaveAssets();

        Debug.Log("Fire point " + w.weaponName + " disimpan: " + w.firePointPosition);
    #endif
    }

// Titik merah penanda fire point di Scene
    private void OnDrawGizmos()
    {
        if (firePoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(firePoint.position, 0.03f);
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

        // hentikan animasi senjata sebelumnya
        if (animRoutine != null)
        {
            StopCoroutine(animRoutine);
            animRoutine = null;
        }

        // gambar diam
        if (sr != null && Current.weaponSprite != null)
            sr.sprite = Current.weaponSprite;

        // pindahkan fire point ke ujung laras senjata ini
        firePoint.localPosition = Current.firePointPosition;
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

        bool wantsToFire = Current.automatic
            ? Input.GetMouseButton(0)
            : Input.GetMouseButtonDown(0);

        if (!wantsToFire || Time.time < nextFireTime) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        for (int i = 0; i < Current.bulletsPerShot; i++)
        {
            float offset = Random.Range(-Current.spreadAngle / 2f, Current.spreadAngle / 2f);

            GameObject bullet = Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.Euler(0f, 0f, baseAngle - 90f + offset)
            );

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

        // mainkan animasi tembak
        if (sr != null && Current.shootFrames != null && Current.shootFrames.Length > 0)
        {
            if (animRoutine != null) StopCoroutine(animRoutine);
            animRoutine = StartCoroutine(PlayShootAnimation());
        }

        nextFireTime = Time.time + Current.fireRate;
    }

    private IEnumerator PlayShootAnimation()
    {
        WeaponData w = Current;
        Sprite[] frames = w.shootFrames;

        // jangan lebih lama dari jeda tembakan, supaya senjata cepat tetap terlihat rapi
        float frameTime = Mathf.Min(1f / w.animationFPS, w.fireRate / frames.Length);

        for (int i = 0; i < frames.Length; i++)
        {
            sr.sprite = frames[i];
            yield return new WaitForSeconds(frameTime);
        }

        // kembali ke gambar diam
        if (w.weaponSprite != null)
            sr.sprite = w.weaponSprite;

        animRoutine = null;
    }

    private IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(flashDuration);
        muzzleFlash.SetActive(false);
    }
}