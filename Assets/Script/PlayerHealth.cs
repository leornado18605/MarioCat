using Unity.Cinemachine;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 5;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float hitCooldown = 0.5f;
    [SerializeField] private int respawnIndex = 0;

    private Transform[] respawnPoints;
    private HealthUI ui;
    private Rigidbody2D rb;
    private PlayerController controller;

    private CinemachineCamera vcam;

    private int   hp;
    private float lastHitTime;
    private bool  initialized;
    public  int   CurrentHp => hp;

    private void Awake()
    {
        ui         = Object.FindFirstObjectByType<HealthUI>();
        rb         = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();

        var rm = Object.FindFirstObjectByType<RespawnManager>();
        respawnPoints = rm ? rm.Points : null;

        vcam = Object.FindFirstObjectByType<CinemachineCamera>();
        RebindCamera();

        if (!initialized)
        {
            hp          = maxHp;
            initialized = true;
        }

        if (ui) ui.SetHearts(hp);
        ClampRespawnIndex();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(enemyTag)) Hit();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag)) Hit();
    }

    private void Hit()
    {
        if (Time.time - lastHitTime < hitCooldown) return;
        lastHitTime = Time.time;

        if (hp <= 0) return;
        hp = Mathf.Max(0, hp - 1);
        if (ui) ui.SetHearts(hp);

        if (hp > 0) Respawn();
        else Lose();
    }

    private void Respawn()
    {
        RebindCamera();
        var p = GetCurrentRespawnPoint();
        if (p) transform.position = p.position;
        if (rb) rb.linearVelocity = Vector2.zero;
        RebindCamera();
    }

    private void Lose()
    {
        if (ui) ui.ShowLose();
        if (controller) controller.Die();
        if (rb) rb.linearVelocity = Vector2.zero;
    }


    private void RebindCamera()
    {
        if (!vcam) vcam = Object.FindFirstObjectByType<CinemachineCamera>();
        if (vcam) vcam.Follow = transform;
    }

    public void SetRespawnIndex(int index)
    {
        respawnIndex = index;
        ClampRespawnIndex();
    }

    private Transform GetCurrentRespawnPoint()
    {
        if (respawnPoints == null || respawnPoints.Length == 0) return null;
        ClampRespawnIndex();
        return respawnPoints[respawnIndex];
    }

    private void ClampRespawnIndex()
    {
        if (respawnPoints == null || respawnPoints.Length == 0) { respawnIndex = 0; return; }
        respawnIndex = Mathf.Clamp(respawnIndex, 0, respawnPoints.Length - 1);
    }

    public void ResetPlayer()
    {
        hp          = maxHp;
        lastHitTime = 0f;

        if (ui) ui.SetHearts(hp);

        if (controller)
        {
            controller.enabled = true;
            controller.Revive();  //reset isDead
        }

        if (rb)
        {
            rb.linearVelocity  = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        var anim = GetComponentInChildren<Animator>();
        if (anim)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        var p                     = GetCurrentRespawnPoint();
        if (p) transform.position = p.position;
    }

}