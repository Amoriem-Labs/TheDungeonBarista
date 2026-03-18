using System.Collections;
using UnityEngine;
using System.Collections.Generic;

//=================================================================================
    // File: CervWander.cs
    // Author: Nathan B 
    // Description: The basic movement for base Cervitus, teleportation. It is largely based 
    // on an imported online script and then modified.
    // I'm also convinced there is a simpler way to do this without generating points
    // but I was experimenting and it works so I'm not going to bother :)
//=================================================================================


namespace TDB
{
    public class CervWander : MonoBehaviour
    {
        private CervData _entityData;
        private Vector2 _homePos = Vector2.zero;

        [Header("Teleport Settings")]
        public Vector2 Range = new Vector2(100, 100);
        public float CooldownTimerMax = 5f;

        [Header("Teleport Area")]
        public Collider2D teleportArea;

        [Header("Timing")]
        public float DisappearDelay = 0.3f;
        public float ReappearDelay = 0.2f;

        [Header("AOE Attack")]
        public GameObject projectile;
        public float projectSpeed = 8f;
        public float angle = 15f; 
        public float waveDelay = 0.2f;

        private float _cooldownTimerCurrent;

        private SpriteRenderer _renderer;
        private Vector2 _lastTeleportPosition;
        public float MinTeleportDistance = 2f;
        private List<Vector2> validPoints = new List<Vector2>();
        public int pointsToGenerate = 100;

        private void Awake()
        {
            _entityData = GetComponent<CervData>();
            _renderer = GetComponent<SpriteRenderer>();

            SetHomePoint();
            _cooldownTimerCurrent = CooldownTimerMax;
            GenerateValidPoints();
        }

        private void Update()
        {
            WanderUpdate();
        }

        public void SetHomePoint()
        {
            _homePos = transform.position;
        }

        private void GenerateValidPoints()
        {
            if (teleportArea == null)
                return;

            Bounds bounds = teleportArea.bounds;

            validPoints.Clear();

            int attempts = 0;
            int maxAttempts = pointsToGenerate * 5;

            while (validPoints.Count < pointsToGenerate && attempts < maxAttempts)
            {
                Vector2 testPoint = new Vector2(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y)
                );

                if (teleportArea.OverlapPoint(testPoint))
                {
                    validPoints.Add(testPoint);
                    Debug.DrawLine(testPoint, testPoint + Vector2.up * 0.5f, Color.red, 2f);
                }

                attempts++;
            }

            if (validPoints.Count == 0)
            {
                Debug.LogError("No valid teleport points found");
            }
        }

        public void WanderUpdate()
        {
            _cooldownTimerCurrent -= Time.deltaTime;

            if (_cooldownTimerCurrent <= 0)
            {
                StartCoroutine(TeleportRoutine());
                _cooldownTimerCurrent = CooldownTimerMax;
            }
        }

        private IEnumerator TeleportRoutine()
        {
            // Disappear [insert animation]
            if (_renderer != null)
                _renderer.enabled = false;

            yield return new WaitForSeconds(DisappearDelay);

            // Teleport [insert animation]
            TeleportToRandomPoint();

            yield return new WaitForSeconds(ReappearDelay);

            // Reappear [insert animation]
            if (_renderer != null)
                _renderer.enabled = true;

            // Call for AOE attack
            StartCoroutine(AOEattack());
        }


        private void TeleportToRandomPoint()
        {
        if (validPoints.Count == 0)
            return;

        Vector2 chosenPoint = validPoints[Random.Range(0, validPoints.Count)];

        transform.position = chosenPoint;
        _lastTeleportPosition = chosenPoint;
        }

        private IEnumerator AOEattack()
        {
            // First wave
            FireProjectiles(0f);

            yield return new WaitForSeconds(waveDelay);

            // Second wave
            FireProjectiles(angle);
        }

        private void FireProjectiles(float angleStart)
        {
            int projectileCount = 4;
            float Steps = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = angleStart + (Steps * i);
            float angleRad = angle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

            GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity);

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * projectSpeed;
            }
        }
        }
    }
}