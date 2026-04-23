using System.Collections;
using UnityEngine;
using System.Collections.Generic;

//=================================================================================
    // File: CervPattern.cs
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
        private Animator anim;
        private Animator headAnim;
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

        [Header("Arena Scaling")]
        [Tooltip("Scales attack sizes/offsets to match a resized arena. Use ~4.35 if the arena was previously scaled down to 0.23.")]
        public float ArenaScaleMultiplier = 1f;

        [Header("Timing")]
        public float DisappearDelay = 0.8f;
        public float ReappearDelay = 0.2f;
        public float AttackDelayAfterReappear = 0.1f;

        [Header("Attack Percentage")]
        // I'm not sure, but I don't actually think these are percentages!!!
        public float PAOE = 0.5f;
        public float PTopDown = 0.25f;
        public float PTrap = 0.15f;
        public float PHead = 0.1f;

        [Header("AOE Attack")]
        public GameObject AOEprojectile;
        public float projectSpeed = 8f;
        public float angle = 15f; 
        public float waveDelay = 0.2f;

        [Header("Top-Down Attack")]
        public Vector2 lineAttackPosition;
        public GameObject TDwarning;
        public GameObject TDbeam;
        public int numberOfProjectiles = 12;
        public float spacing = 3.0f;
        public float warningDuration = 1.0f;
        public float beamDuration = 0.9f;

        [Header("Center Attack")]
        public Vector2 Center;
        public float CenterAngle = 20f; 
        public int waveCount = 10;
        public float timeBetweenWaves = 0.2f;

        [Header("Trapping Attack")]
        public GameObject Traps;
        public int numberOfTraps = 20;

        [Header("Head Attack")]
        public GameObject CervHead;
        public Transform StartPosition;
        public GameObject Headwarning;
        public GameObject Headbeam;
        public float HeadSpeed=5f;
        public float headDuration=5f;
        public float HeadBeamInterval = 0.75f;
        public float HeadBobAmplitude = 2f;

        [Header("Enrage Settings")]
        public float enragedCooldownMultiplier = 0.5f;
        public float enragedWarningMultiplier = 0.5f;
        public float enragedProjectileMultiplier = 1.5f; 
        // To start with
        private float currentProjectileMultiplier = 1f;
        private float currentWarningMultiplier = 1f;
        private bool _headBeamInProgress = false;

        private float Scaled(float value) => value * ArenaScaleMultiplier;

        private Bounds? TryGetArenaBounds()
        {
            if (teleportArea == null)
                return null;

            return teleportArea.bounds;
        }

        private void Awake()
        {
            anim = GetComponent<Animator>();
            headAnim = CervHead.GetComponent<Animator>();
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
                EnterEnrage();
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
            float Pall = PAOE + PTopDown + PTrap + PHead;
            float value = Random.Range(0f, Pall);

            if (value < PAOE)
            {
                StartCoroutine(TeleportRoutine());
            }
            else if (value < PAOE + PTopDown)
            {
                StartCoroutine(LineAttackRoutine());
            }
            else if (value < PAOE + PTopDown + PTrap)
            {
                StartCoroutine(TrapAttackRoutine());
            }
            else
            {
                StartCoroutine(HeadAttack());
            }
        }

        // his standard teleport routine, then calls for his AOE attack on landing
        private IEnumerator TeleportRoutine()
        {
            // Disappear [insert animation]
            anim.SetBool("isMelting", true);
            yield return new WaitForSeconds(DisappearDelay);

            if (_renderer != null)
                _renderer.enabled = false;

            anim.SetBool("isMelting", false);

            // Teleport [insert animation]
            TeleportToRandomPoint();

            yield return new WaitForSeconds(ReappearDelay);

            // Reappear [insert animation]
            if (_renderer != null)
                _renderer.enabled = true;

            anim.SetTrigger("meltUp");
            // Call for AOE attack
            if (AttackDelayAfterReappear > 0f)
                yield return new WaitForSeconds(AttackDelayAfterReappear);
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

            Quaternion rotation = Quaternion.Euler(0, 0, angle);

            GameObject proj = Instantiate(AOEprojectile, transform.position, rotation);

            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * projectSpeed * currentProjectileMultiplier;
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
            anim.SetTrigger("Spray");

            float scaledSpacing = Scaled(spacing);
            float totalWidth = (numberOfProjectiles - 1) * scaledSpacing;
            float startX = transform.position.x - totalWidth / 2f;

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                Vector2 origin = new Vector2(
                    startX + i * scaledSpacing,
                    transform.position.y
                );
                Vector2 warningPos = new Vector2(
                    startX + i * scaledSpacing,
                    transform.position.y - Scaled(3f)
                );
                StartCoroutine(ShootBeam(origin, warningPos));
            }
        }  

        private IEnumerator ShootBeam(Vector2 origin, Vector2 warningPos)
        {
            // warning pillar
            GameObject pillar = Instantiate(TDwarning, warningPos, Quaternion.identity);
            Collider2D pillarCol = pillar.GetComponent<Collider2D>();
            if (pillarCol != null)
                pillarCol.isTrigger = true;

            yield return new WaitForSeconds(warningDuration * currentWarningMultiplier);

            float warningHeight = 0f;
            var arenaBounds = TryGetArenaBounds();
            if (arenaBounds.HasValue)
            {
                // cover from the attack origin down to the bottom of the arena
                warningHeight = Mathf.Max(0.5f, origin.y - arenaBounds.Value.min.y);
            }
            else
            {
                SpriteRenderer warnSR = pillar.GetComponent<SpriteRenderer>();
                if (warnSR != null)
                    warningHeight = warnSR.bounds.size.y;
            }
            Destroy(pillar);

            // spawn visual spikes as before (no colliders needed on these)
            float spikeSpacing = Scaled(0.5f);
            int spikeCount = Mathf.CeilToInt(warningHeight / spikeSpacing);
            List<GameObject> spawned = new List<GameObject>();
            Vector2 adjustedOrigin = origin + Vector2.down * Scaled(1.2f);

            for (int i = 0; i < spikeCount; i++)
            {
                Vector2 spawnPos = adjustedOrigin + Vector2.down * (i * spikeSpacing);
                GameObject spike = Instantiate(TDbeam, spawnPos, Quaternion.identity);
                spawned.Add(spike);
                yield return new WaitForSeconds(0.02f);
            }

            // create one big trigger collider covering the whole column
            GameObject hitbox = new GameObject("SpikeHitbox");
            hitbox.transform.position = adjustedOrigin + Vector2.down * (warningHeight / 2f);
            hitbox.layer = gameObject.layer;

            Rigidbody2D rb = hitbox.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;

            BoxCollider2D box = hitbox.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(Scaled(0.5f), Mathf.Max(0.5f, warningHeight - Scaled(1f))); // match spike column width/height

            SpikeLife spikeLife = hitbox.AddComponent<SpikeLife>();
            spikeLife.damageAmount = 1;

            yield return new WaitForSeconds(beamDuration);

            Destroy(hitbox);
            foreach (var s in spawned)
                if (s != null) Destroy(s);
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
            anim.SetTrigger("Spray");
            yield return new WaitForSeconds(2f);
            MoveToPosition(Center);

            // inserted for players reactions and animations
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < waveCount; i++)
            {
                float offset = i * CenterAngle;

                FireProjectiles(offset);

                yield return new WaitForSeconds(timeBetweenWaves);
            }
            if (hitbox != null)
                hitbox.enabled = true;
            percentageAttack = false;
        }

        private IEnumerator TrapAttackRoutine()
        {
            TrapAttack();
            yield return null;
        }

        private void TrapAttack()
        {


            if (teleportArea == null)
            {
                Debug.LogError("Teleport area missing!");
                return;
            }

            if (Traps == null)
            {
                Debug.LogError("Trap prefab missing!");
                return;
            }

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

        private IEnumerator HeadAttack()
        {

            CervHead.transform.position = new Vector3( StartPosition.position.x, StartPosition.position.y, 0f);
            Debug.Log("Starting Head Attack!");
            // not actually a percentage attack, but turns on so other attacks are not also triggered
            percentageAttack = true;
            if (_renderer != null)
                _renderer.enabled = false;
                
            if (hitbox != null)
                hitbox.enabled = false;

            CervHead.SetActive(true);
            CervHead.transform.position = StartPosition.position;
            
            headAnim.SetTrigger("emerge");


            float endTime = Time.time + headDuration;
            float nextBeamTime = Time.time;
            float elapsed = 0f;
            float baseY = StartPosition.position.y;
            float baseX = StartPosition.position.x;          

            while (elapsed < headDuration)
            {

                elapsed += Time.deltaTime;
                // orignally used ping pong but this seems to be smoother with sine (got online)
                float yOffset = Mathf.Sin(Time.time * HeadSpeed) * HeadBobAmplitude;

                CervHead.transform.position = new Vector3(baseX, baseY + yOffset, 0f);

                if (!_headBeamInProgress && Time.time >= nextBeamTime)
                {
                    nextBeamTime = HeadBeamInterval;
                    Debug.Log("Next beam time is " + nextBeamTime);
                    StartCoroutine(ShootBeamHeadWrapper(CervHead.transform));
                }

                yield return null;
            }

            headAnim.SetTrigger("retract");
            yield return new WaitForSeconds(0.8f);

            CervHead.SetActive(false);
             if (_renderer != null)
                _renderer.enabled = true;

            if (hitbox != null)
                hitbox.enabled = true;

            percentageAttack = false;

        }

        private IEnumerator ShootBeamHeadWrapper(Transform headTransform)
        {
            _headBeamInProgress = true;
            yield return StartCoroutine(ShootBeamHead(headTransform));
            _headBeamInProgress = false;
        }

        private IEnumerator ShootBeamHead(Transform headTransform)
        {
            Vector3 offsetWarning = new Vector3(Scaled(-4f), Scaled(-0.4f), 0f);
            Vector3 offsetBeam = new Vector3(Scaled(0.2f), Scaled(-0.4f), 0f);

            GameObject warning = Instantiate(
                Headwarning,
                headTransform.position + offsetWarning,
                Quaternion.Euler(0, 0, 90f)
            );

            warning.transform.SetParent(headTransform);
            float warningTime = warningDuration * currentWarningMultiplier;
            yield return new WaitForSeconds(warningTime);

            Destroy(warning);

            headAnim.SetTrigger("attack");
            yield return new WaitForSeconds(0.7f);

            GameObject beam = Instantiate(
                Headbeam,
                headTransform.position + offsetBeam,
                Quaternion.identity
            );

            // disable the prefab's built-in collider - visuals only
            CapsuleCollider2D prefabCol = beam.GetComponent<CapsuleCollider2D>();
            if (prefabCol != null)
                prefabCol.enabled = false;

            SpriteRenderer sr = beam.GetComponent<SpriteRenderer>();
            sr.drawMode = SpriteDrawMode.Tiled;

            float height = Scaled(0.2f);

            float beamLength = Scaled(20f);
            var arenaBounds = TryGetArenaBounds();
            if (arenaBounds.HasValue)
            {
                // extend to the right edge of the arena
                beamLength = Mathf.Max(Scaled(2f), arenaBounds.Value.max.x - beam.transform.position.x);
            }
            float duration = 0.1f;
            float t = 0f;

            // grow sprite visually as before
            sr.size = new Vector2(0f, height);

            // create hitbox at full size, parented to headTransform directly
            GameObject hitboxObj = new GameObject("BeamHitbox");
            hitboxObj.layer = gameObject.layer;

            Rigidbody2D rb = hitboxObj.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;

            BoxCollider2D box = hitboxObj.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(beamLength, Scaled(0.5f));
            box.offset = Vector2.zero; // centered on the object itself

            SpikeLife spikeLife = hitboxObj.AddComponent<SpikeLife>();
            spikeLife.damageAmount = 1;
            spikeLife.damageCooldown = 0.5f;

            // position it so it starts at the beam origin and extends right
            // beamLength/2 offset centers the box starting from the spawn point
            hitboxObj.transform.position = new Vector3(
                beam.transform.position.x + beamLength / 2f,
                beam.transform.position.y,
                beam.transform.position.z
            );

            hitboxObj.transform.SetParent(headTransform); // follows head movement
            hitboxObj.transform.localScale = Vector3.one;

            while (t < duration)
            {
                t += Time.deltaTime;
                float lerp = t / duration;
                float currentWidth = Mathf.Lerp(0f, beamLength, lerp);
                sr.size = new Vector2(currentWidth, height);
                yield return null;
            }

            sr.size = new Vector2(beamLength, height);
            beam.transform.SetParent(headTransform);

            yield return new WaitForSeconds(beamDuration);

            Destroy(beam);
            Destroy(hitboxObj); // destroy separately now since it's not a child of beam
        }

        private void EnterEnrage()
        {
            // faster attacks
            CooldownTimerMax *= enragedCooldownMultiplier;
            HeadBeamInterval *= enragedCooldownMultiplier*1/8;

            // faster projectiles
            currentProjectileMultiplier = enragedProjectileMultiplier;

            // smaller warning beam time
            currentWarningMultiplier = enragedWarningMultiplier;

            // insert some animation here, for now just turns red
            if (_renderer != null)
                _renderer.color = Color.red;
        }

    }
}
