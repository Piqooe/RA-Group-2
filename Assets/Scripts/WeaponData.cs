using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite weaponSprite;        // gambar senjata saat diam

    [Header("Tembakan")]
    public int damage = 1;
    public float fireRate = 0.3f;
    public float bulletSpeed = 10f;
    public float bulletLifetime = 3f;

    [Header("Pola Tembakan")]
    public int bulletsPerShot = 1;
    public float spreadAngle = 0f;
    public bool automatic = true;

    [Header("Posisi Ujung Laras")]
    public Vector2 firePointPosition;  // posisi peluru keluar

    [Header("Animasi Tembak")]
    public Sprite[] shootFrames;       // frame animasi tembak (boleh kosong)
    public float animationFPS = 20f;   // kecepatan animasi

    [Header("Lainnya")]
    public int pierce = 0;
    public float shakeMagnitude = 0.1f;
}