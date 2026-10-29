using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite weaponSprite;        // gambar senjata (boleh dikosongin dulu)

    [Header("Tembakan")]
    public int damage = 1;
    public float fireRate = 0.3f;      // jeda antar tembakan (detik)
    public float bulletSpeed = 10f;
    public float bulletLifetime = 3f;

    [Header("Pola Tembakan")]
    public int bulletsPerShot = 1;     // shotgun isi lebih dari 1
    public float spreadAngle = 0f;     // sebaran peluru (derajat)
    public bool automatic = true;      // true = tahan klik terus nembak

    [Header("Lainnya")]
    public int pierce = 0;             // berapa musuh bisa ditembus
    public float shakeMagnitude = 0.1f;
}