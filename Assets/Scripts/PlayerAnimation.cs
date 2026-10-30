using UnityEngine;

[System.Serializable]
public class DirectionFrames
{
    public Sprite[] idle;
    public Sprite[] walk;

    [Header("Senjata untuk arah ini")]
    public Vector2 gunOffset;   // posisi tangan (relatif ke Player)
    public bool gunBehind;      // centang = senjata di belakang badan
}

[DefaultExecutionOrder(-10)]
public class PlayerAnimation : MonoBehaviour
{
    [Header("Wajib diisi")]
    public SpriteRenderer body;            // objek badan (yang punya Sprite Renderer)
    public SpriteRenderer weaponRenderer;  // WeaponSprite (anak Gun)
    public Transform gun;                  // objek Gun (anak Player)

    [Header("Kecepatan animasi")]
    public float idleFps = 6f;
    public float walkFps = 12f;

    [Header("Frame per arah: 0 kanan-atas, 1 atas, 2 kiri-atas, 3 kiri-bawah, 4 bawah, 5 kanan-bawah")]
    public DirectionFrames[] directions = new DirectionFrames[6];

    [Header("Preview di Edit Mode (tanpa Play)")]
    [Range(0, 5)] public int previewDirection = 0;

    [Header("Folder aset (untuk Isi Frame Otomatis)")]
    public string idleFolder = "Assets/Assets/The Male adventurer - Free/Idle";
    public string walkFolder = "Assets/Assets/The Male adventurer - Free/Walk";

    private Vector3 lastPos;
    private float lastMoveTime = -10f;
    private bool wasMoving;
    private float timer;
    private int frame;
    private bool warned;

    private void Start()
    {
        lastPos = transform.position;
    }

    private void Update()
    {
        // 1. Sedang jalan? Dilihat dari perpindahan posisi
        if ((transform.position - lastPos).sqrMagnitude > 0.000001f)
            lastMoveTime = Time.time;
        lastPos = transform.position;
        bool isMoving = Time.time - lastMoveTime < 0.1f;

        // 2. Arah hadap mengikuti mouse (6 sektor, masing-masing 60 derajat)
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mouse - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;
        int dirIndex = Mathf.FloorToInt(angle / 60f) % 6;

        if (body == null || directions == null || dirIndex >= directions.Length || directions[dirIndex] == null)
            return;

        DirectionFrames d = directions[dirIndex];

        // 3. Posisi senjata sesuai arah badan
        if (gun != null)
        {
            Vector3 p = gun.localPosition;
            gun.localPosition = new Vector3(d.gunOffset.x, d.gunOffset.y, p.z);
        }

        if (weaponRenderer != null)
            weaponRenderer.sortingOrder = body.sortingOrder + (d.gunBehind ? -1 : 1);

        // 4. Pilih animasi
        Sprite[] frames = isMoving ? d.walk : d.idle;

        if (frames == null || frames.Length == 0)
        {
            if (!warned)
            {
                Debug.LogWarning("PlayerAnimation: frame kosong untuk arah " + dirIndex +
                                 ". Jalankan 'Isi Frame Otomatis' atau isi manual.");
                warned = true;
            }
            return;
        }

        if (isMoving != wasMoving)
        {
            frame = 0;
            timer = 0f;
            wasMoving = isMoving;
        }

        timer += Time.deltaTime;
        float step = 1f / (isMoving ? walkFps : idleFps);
        if (timer >= step)
        {
            timer -= step;
            frame++;
        }

        body.sprite = frames[frame % frames.Length];
        body.flipX = false;
    }

    // ================= BAGIAN EDITOR (tanpa Play) =================

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (Application.isPlaying) return;

        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this == null) return;
            ApplyPreview();
        };
#endif
    }

#if UNITY_EDITOR
    private void ApplyPreview()
    {
        if (directions == null || previewDirection >= directions.Length || directions[previewDirection] == null)
            return;

        DirectionFrames d = directions[previewDirection];

        if (body != null && d.idle != null && d.idle.Length > 0)
            body.sprite = d.idle[0];

        if (gun != null)
        {
            Vector3 p = gun.localPosition;
            gun.localPosition = new Vector3(d.gunOffset.x, d.gunOffset.y, p.z);

            // putar senjata ke tengah sektor supaya mirip saat Play
            float centerAngle = previewDirection * 60f + 30f;
            gun.rotation = Quaternion.Euler(0f, 0f, centerAngle);

            Vector3 s = gun.localScale;
            bool left = Mathf.Cos(centerAngle * Mathf.Deg2Rad) < 0f;
            s.y = left ? -Mathf.Abs(s.y) : Mathf.Abs(s.y);
            gun.localScale = s;
        }

        if (weaponRenderer != null && body != null)
            weaponRenderer.sortingOrder = body.sortingOrder + (d.gunBehind ? -1 : 1);
    }
#endif

    [ContextMenu("1. Salin Posisi Gun ke Semua Arah")]
    private void CopyGunToAll()
    {
#if UNITY_EDITOR
        if (gun == null)
        {
            Debug.LogWarning("Isi dulu kolom Gun.");
            return;
        }

        EnsureDirections();
        UnityEditor.Undo.RecordObject(this, "Salin Posisi Gun");

        for (int i = 0; i < 6; i++)
        {
            directions[i].gunOffset = gun.localPosition;
            directions[i].gunBehind = i <= 2;   // arah atas: senjata di belakang badan
        }

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log("Posisi Gun disalin ke semua arah: " + gun.localPosition);
#endif
    }

    [ContextMenu("2. Isi Frame Otomatis")]
    private void AutoFill()
    {
#if UNITY_EDITOR
        string[] names = { "right_up", "up", "left_up", "left_down", "down", "right_down" };
        EnsureDirections();
        UnityEditor.Undo.RecordObject(this, "Isi Frame Otomatis");

        for (int i = 0; i < 6; i++)
        {
            directions[i].idle = LoadFrames(idleFolder + "/idle_" + names[i] + ".png");
            directions[i].walk = LoadFrames(walkFolder + "/walk_" + names[i] + ".png");
        }

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log("Isi Frame Otomatis selesai. Posisi senjata yang sudah kamu atur tidak berubah.");
#endif
    }

    [ContextMenu("3. Simpan Posisi Gun untuk Arah Ini")]
    private void SaveGunForDirection()
    {
#if UNITY_EDITOR
        if (gun == null)
        {
            Debug.LogWarning("Isi dulu kolom Gun.");
            return;
        }

        EnsureDirections();
        UnityEditor.Undo.RecordObject(this, "Simpan Posisi Gun");

        directions[previewDirection].gunOffset = gun.localPosition;

        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log("Posisi Gun arah " + previewDirection + " disimpan: " + gun.localPosition);
#endif
    }

#if UNITY_EDITOR
    private void EnsureDirections()
    {
        if (directions == null || directions.Length != 6)
        {
            DirectionFrames[] old = directions;
            directions = new DirectionFrames[6];

            if (old != null)
            {
                for (int i = 0; i < Mathf.Min(6, old.Length); i++)
                    directions[i] = old[i];
            }
        }

        for (int i = 0; i < 6; i++)
        {
            if (directions[i] == null)
                directions[i] = new DirectionFrames();
        }
    }

    private Sprite[] LoadFrames(string path)
    {
        Object[] all = UnityEditor.AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
        var list = new System.Collections.Generic.List<Sprite>();

        foreach (Object o in all)
        {
            if (o is Sprite s) list.Add(s);
        }

        list.Sort((a, b) => FrameNumber(a.name).CompareTo(FrameNumber(b.name)));

        if (list.Count == 0)
            Debug.LogWarning("Tidak ada frame di: " + path + " (nama file salah, atau belum di-Slice?)");

        return list.ToArray();
    }

    private int FrameNumber(string spriteName)
    {
        int u = spriteName.LastIndexOf('_');
        if (u >= 0 && int.TryParse(spriteName.Substring(u + 1), out int n)) return n;
        return 0;
    }
#endif
}