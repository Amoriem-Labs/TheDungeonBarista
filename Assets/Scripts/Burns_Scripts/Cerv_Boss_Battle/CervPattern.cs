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
    public class CervPattern : MonoBehaviour
    {
        // private references
        private EntityData _entityData;
        private Vector2 _homePos = Vector2.zero;
        private float _cooldownTimerCurrent;
        private SpriteRenderer _renderer;
        private Vector2 _lastTeleportPosition;
        private List<Vector2> validPoints = new List<Vector2>();
        private Collider2D hitbox;
        // for attacks that depend on hp percentages
        private bool percentageAttack = false;
        private bool triggered75 = false;
        private bool triggered50 = false;
        private bool triggered25 = false;
        private bool triggered10 = false;

        // public changeable references
        [Header("Teleport Settings")]
        public Vector2 Range = new Vector2(100, 100);
        public float CooldownTimerMax = 5f;

        [Header("Teleport Area")]
        public Collider2D teleportArea;
        public int pointsToGenerate = 100;

        [Header("Timing")]
        public float DisappearDelay = 0.3f;
        public float ReappearDelay = 0.2f;

        [Header("Attack Percentage")]
        // I'm not sure, but I don't actually think these are percentages!!!
        public float PAOE = 0.6f;
        public float PTopDown = 0.25f;
        public float PTrap = 0.15f;

        [Header("AOE Attack")]
        public GameObject AOEprojectile;
        public float projectSpeed = 8f;
        public float angle = 15f; 
        public float waveDelay = 0.2f;

        [Header("Top-Down Attack")]
        public Vector2 lineAttackPosition;
        public GameObject TDprojectile;
        public int numberOfProjectiles = 12;
        public float spacing = 1.0f;

        [Header("Center Attack")]
        public Vector2 Center;
        public int waveCount = 3;
        public float timeBetweenWaves = 0.3f;

        [Header("Trapping Attack")]
        public GameObject Traps;
        public int numberOfTraps = 20;


        private void Awake()
        {
            _entityData = GetComponent<EntityData>();
            _renderer = GetComponent<SpriteRenderer>();
            hitbox = GetComponent<Collider2D>(); 
            SetHomePoint();
            _cooldownTimerCurrent = CooldownTimerMax;
            GenerateValidPoints();
        }

        // on update perform the basic attacks and then check hp percentage
        private void Update()
        {
            WanderUpdate();
            CheckHealthTriggers();
        }

        public void SetHomePoint()
        {
            _homePos = transform.position;
        }

        // used for the teleportation - creates a list of points that are valid for teleportation
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

            // check if any points are found
            if (validPoints.Count == 0)
            {
                Debug.LogError("No valid teleport points found");
            }
        }

        // checks for hp percentages and triggers attacks/possibly events based on them
        private void CheckHealthTriggers()
        {
            float hpPercent = _entityData.CurrentHealth / _entityData.MaxHealth;

            if (!triggered75 && hpPercent <= 0.75f)
            {
                triggered75 = true;
                StartCoroutine(CenterAttack());
            }

            if (!triggered50 && hpPercent <= 0.50f)
            {
                triggered50 = true;
                StartCoroutine(CenterAttack());
            }

            if (!triggered25 && hpPercent <= 0.25f)
            {
                triggered25 = true;
                StartCoroutine(CenterAttack());
            }

            if (!triggered10 && hpPercent <= 0.10f)
            {
                triggered10 = true;
                StartCoroutine(CenterAttack());
            }
        }

        // the standard update that chooses what attack based on a scaling system
        public void WanderUpdate()
        {
            if (percentageAttack) return;

            _cooldownTimerCurrent -= Time.deltaTime;

            if (_cooldownTimerCurrent <= 0)
            {
                ChooseAttack();
                _cooldownTimerCurrent = CooldownTimerMax;
            }
        }

        // dicatates what attack to perform and then starts that coroutine
        private void ChooseAttack()
        {
            float Pall = PAOE + PTopDown + PTrap;
            float value = Random.Range(0f, Pall);

            if (value < PAOE)
            {
                StartCoroutine(TeleportRoutine());
            }
            else if (value < PAOE + PTopDown)
            {
                StartCoroutine(LineAttackRoutine());
            }
            else 
            {
                StartCoroutine(TrapAttackRoutine());
            }
        }

        // his standard teleport routine, then calls for his AOE attack on landing
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

        // actual teleport function
        private void TeleportToRandomPoint()
        {
        if (validPoints.Count == 0)
            return;

        Vector2 chosenPoint = validPoints[Random.Range(0, validPoints.Count)];

        transform.position = chosenPoint;
        _lastTeleportPosition = chosenPoint;
        }

        // AOE attack, creates 2 waves of projectiles (prefabs) that shoot at offset angles
        private IEnumerator AOEattack()
        {
            // First wave
            FireProjectiles(0f);

            yield return new WaitForSeconds(waveDelay);

            // Second wave
            FireProjectiles(angle);
        }

        // actual AOE attack, creates the projectiles and shoots them at a given velocity (always check rotations are correct)
        private void FireProjectiles(float angleStart)
        {
            int projectileCount = 4;
            float Steps = 360f / projectileCount;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = angleStart + (Steps * i);
            float angleRad = angle * Mathf.Deg2Rad;

            Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

            Quaternion rotation = Quaternion.Euler(0, 0, angle - 90f);

            GameObject proj = Instantiate(AOEprojectile, transform.position, rotation);

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * projectSpeed;
            }
        }
        }

        // call for the line attack where they go to the top of the screen and shoot projectiles downward 
        private IEnumerator LineAttackRoutine()
        {
            LineAttack();
            yield return null;
        }

        // actual line attack (currently shoots same projectile) - similar to previous AOE attack
        private void LineAttack()
        {
            MoveToPosition(lineAttackPosition);

            float totalWidth = (numberOfProjectiles - 1) * spacing;
            float startX = transform.position.x - totalWidth / 2f;

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                Vector2 spawnPos = new Vector2(
                    startX + i * spacing,
                    transform.position.y
                );

                Quaternion rotation = Quaternion.Euler(0, 0, 180f); 

                GameObject proj = Instantiate(TDprojectile, spawnPos, rotation);

                Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.down * projectSpeed;
                }
            }
        }  

        // standard move 
        private void MoveToPosition(Vector2 target)
        {
            transform.position = target;
        }

        // center attack which is triggered by HP percentages. The boss is not able to be hit during this attack (disabled hitbox)
        private IEnumerator CenterAttack()
        {
            percentageAttack = true;
            if (hitbox != null)
                hitbox.enabled = false;
        
            // insert animation of screaming or something here
            yield return new WaitForSeconds(2f);
            MoveToPosition(Center);

            // inserted for players reactions and animations
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < waveCount; i++)
            {
                float offset = i * angle;

                FireProjectiles(offset);

                yield return new WaitForSeconds(timeBetweenWaves);
            }
            if (hitbox != null)
                hitbox.enabled = false;
            percentageAttack = false;
        }

        private IEnumerator TrapAttackRoutine()
        {
            Debug.Log("Starting Trap Attack!");
            TrapAttack();
            yield return null;
        }

        private void TrapAttack()
        {
            if (teleportArea == null)
                return;

            Bounds bounds = teleportArea.bounds;

            for (int i = 0; i < numberOfTraps; i++)
            {
                Vector2 spawnPoint = Vector2.zero;
                bool foundValid = false;

                int attempts = 0;
                int maxAttempts = 20;

                // Try to find a valid point inside the collider
                while (!foundValid && attempts < maxAttempts)
                {
                    Vector2 testPoint = new Vector2(
                        Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y)
                    );

                    if (teleportArea.OverlapPoint(testPoint))
                    {
                        spawnPoint = testPoint;
                        foundValid = true;
                    }

                    attempts++;
                }

                if (foundValid)
                {
                    Instantiate(Traps, spawnPoint, Quaternion.identity);
                }
            }
        }

    }
}