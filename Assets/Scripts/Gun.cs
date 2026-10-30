using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public SpriteRenderer sr;          // tarik WeaponSprite ke sini

    [Header("Daftar Senjata (isi dengan kartu)")]
    public WeaponData[] weapons;
    private int currentIndex = 0;

    [Header("Muzzle Flash")]
    public GameObject muzzleFlash;
    public float flashDuration = 0.05f;

    [Header("Camera Shake")]
    public float shakeDuration = 0.1f;

    [Header("Preview di Edit Mode")]
    [Range(0, 3)] public int previewIndex = 0;
    [HideInInspector] public int lastPreviewIndex = -1;

    private float nextFireTime;
    private Coroutine animRoutine;
    private Vector3 baseLocalPos;

    private WeaponData Current => weapons[currentIndex];

    private void Awake()
    {
        baseLocalPos = transform.localPosition;

        if (sr == null)
        {
            Transform t = transform.Find("WeaponSprite");
            if (t != null) sr = t.GetComponent<SpriteRenderer>();
        }
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

    // ---------- GANTI SENJATA ----------
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

        if (animRoutine != null)
        {
            StopCoroutine(animRoutine);
            animRoutine = null;
        }

        currentIndex = index;

        if (sr != null && Current.weaponSprite != null)
            sr.sprite = Current.weaponSprite;

        firePoint.localPosition = Current.firePointPosition;
    }

    // ---------- ARAH SENJATA ----------
    private void RotateToMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // menghadap kiri atau kanan ditentukan dari tengah badan Player
        Vector3 center = transform.parent != null ? transform.parent.position : transform.position;
        bool facingLeft = mousePosition.x < center.x;

        // putar senjata ke arah mouse
        Vector2 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        Vector3 scale = transform.localScale;
        scale.y = facingLeft ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
        transform.localScale = scale;
    }

    // ---------- TEMBAK ----------
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

            Projectile pr = bullet.GetComponent<Projectile>();
            pr.damage = Current.damage;
            pr.speed = Current.bulletSpeed;
            pr.lifeTime = Current.bulletLifetime;
            pr.pierce = Current.pierce;
        }

        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(shakeDuration, Current.shakeMagnitude);

        if (muzzleFlash != null)
            StartCoroutine(ShowMuzzleFlash());

        if (sr != null && Current.shootFrames != null && Current.shootFrames.Length > 0)
        {
            if (animRoutine != null) StopCoroutine(animRoutine);
            animRoutine = StartCoroutine(PlayShootAnimation());
        }

        nextFireTime = Time.time + Current.fireRate;
    }

    // ---------- ANIMASI & EFEK ----------
    private IEnumerator PlayShootAnimation()
    {
        WeaponData w = Current;
        Sprite[] frames = w.shootFrames;

        float frameTime = Mathf.Min(1f / w.animationFPS, w.fireRate / frames.Length);

        for (int i = 0; i < frames.Length; i++)
        {
            sr.sprite = frames[i];
            yield return new WaitForSeconds(frameTime);
        }

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

    // ---------- PREVIEW DI EDIT MODE ----------
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

    private void OnDrawGizmos()
    {
        if (firePoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(firePoint.position, 0.03f);
    }
}