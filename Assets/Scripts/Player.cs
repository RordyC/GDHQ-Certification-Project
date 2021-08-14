using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 25f;

    [SerializeField]
    private int _health = 100;

    [SerializeField]
    private int _maxHealth = 100;

    [SerializeField]
    private ParticleSystem _Rthruster = null;
    [SerializeField]
    private ParticleSystem _Lthruster = null;

    [SerializeField]
    private ParticleSystem _bulletTracers = null;

    [SerializeField]
    private ParticleSystem _miniGunFlash = null;

    [SerializeField]
    private ParticleSystem _rocketArtilleryFlash = null;

    [SerializeField]
    private Transform _modelTransform = null;

    [SerializeField]
    private GameObject _missilePrefab = null;

    [SerializeField]
    private GameObject _rocketPrefab = null;

    [SerializeField]
    private Transform _rocketSpawnPos = null;

    [SerializeField]
    private GameObject _rocketModel = null;

    [SerializeField]
    private ParticleSystem _deathExplosion = null;

    [SerializeField]
    private GameObject _deathFireBall = null;

    [SerializeField]
    private Rigidbody _rightWingRB, _leftWingRB;

    [SerializeField]
    private float _fireRate = 0.15f;
    private float _nextFire = -1;

    private float _enemyDamageRate = 0.15f;
    private float _nextDamage = -1f;

    private UIManager _uiManager;

    private CameraShake _cameraShake;
    private Rigidbody _rb;
    private bool _isDead = false;

    [Header("MISSILES")]

    [SerializeField]
    private bool _canShootMissiles = true;

    [SerializeField]
    private float _missileFireRate = 1f;
    private float _nextMissileFire = -1;

    [Header("HELLFIRE ROCKET ARTILLERY")]

    [SerializeField]
    private bool _canShootRockets = true;

    [SerializeField]
    private float _rocketFireRate = 0.5f;
    private float _nextRocketFire = -1;

    [SerializeField]
    private int _maxRockets = 15;
    private int _currentRockets = 15;

    [SerializeField]
    private float _rocketCooldown = 7f;
    private bool _reloadingRockets = false;

    [Header("Audio")]

    [SerializeField]
    private AudioClip _rocketAudioClip = null;

    //private Animator _animator;

    // Start is called before the first frame update
    void Start()
    { /*
        _animator = transform.GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator is NULL!");
        }
        */

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI MANAGER IS NULL!");
        }

        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        if (_cameraShake == null)
        {
            Debug.LogError("CAMERA SHAKER IS NULL!");
        }

        _rb = transform.GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("Rigidbody is NULL!");
        }

        _miniGunFlash.Stop();
        _rocketArtilleryFlash.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead)
            return;

        Movement();

        CheckForEnemy();

        if (_canShootRockets == true)
        {
            _rocketModel.SetActive(true);
            RocketBehavior();
        }
        else
        {
            _rocketModel.SetActive(false);
        }

        float tiltAngleZ = 10f * Input.GetAxis("Vertical");
        float tiltAngleX = 20f * Input.GetAxis("Horizontal");
        var target = Quaternion.Euler(tiltAngleX, 0, tiltAngleZ);
        _modelTransform.rotation = Quaternion.Slerp(_modelTransform.rotation, target, Time.deltaTime * 10);
    }

    private void Movement()
    {
        var rightThruster = _Rthruster.main;
        var leftThruster = _Lthruster.main;

        float xDir = Input.GetAxis("Horizontal");
        float yDir = Input.GetAxis("Vertical");

        Vector3 velocity = new Vector3 (xDir, yDir, 0);
        transform.Translate(velocity * _speed * Time.deltaTime);
        if (xDir > 0)
        {
            rightThruster.startSizeXMultiplier = 1 + xDir;
            rightThruster.startSizeYMultiplier = 2 + (xDir * 2);
            rightThruster.startSizeZMultiplier = 1 + xDir;

            leftThruster.startSizeXMultiplier = 1 + xDir;
            leftThruster.startSizeYMultiplier = 2 + (xDir * 2);
            leftThruster.startSizeZMultiplier = 1 + xDir;
        }
        else
        {
            rightThruster.startSizeXMultiplier = 1;
            rightThruster.startSizeYMultiplier = 2;
            rightThruster.startSizeZMultiplier = 1;


            leftThruster.startSizeXMultiplier = 1;
            leftThruster.startSizeYMultiplier = 2;
            leftThruster.startSizeZMultiplier = 1;
        }

        if (transform.position.y < -20 || transform.position.y > 20 || transform.position.x < -38 || transform.position.x > 36)
        {
            transform.position = new Vector3(-15, 0, 0);
            Damage(2);
        }

    }

    private void CheckForEnemy()
    {
        RaycastHit hit;

        Debug.DrawRay(_modelTransform.position, _modelTransform.right * 100, Color.red);

        if (Physics.Raycast(transform.position, _modelTransform.right, out hit, 100f))
        {
            if (hit.transform.CompareTag("Player"))
                return;

            if (Time.time > _nextFire)
            {
                if (hit.transform.tag == "Enemy")
                {
                    hit.transform.GetComponent<Enemy>().Damage(1);
                }

                if (hit.transform.tag == "Missile")
                {
                    hit.transform.GetComponent<HomingMissile>().Damage();
                }

                _nextFire = Time.time + _fireRate;

                var bullet = _bulletTracers.emission;
                bullet.enabled = true;
                _miniGunFlash.Play();
            }

            if (_canShootMissiles == true && Time.time > _nextMissileFire)
            {
                GameObject missile = Instantiate(_missilePrefab, transform.position, Quaternion.Euler(0, 90, 0));
                missile.transform.parent = this.transform;
                _nextMissileFire = Time.time + _missileFireRate;
            }

        }
        else
        {
            var bullet = _bulletTracers.emission;
            bullet.enabled = false;
            _rocketArtilleryFlash.Stop();
            _miniGunFlash.Stop();
        }
    }

    private void RocketBehavior()
    {
        if (Time.time > _nextRocketFire && _currentRockets > 0)
        {
            _currentRockets--;
            Instantiate(_rocketPrefab, _rocketSpawnPos.position + new Vector3(0, Random.Range(-0.3f, 0.3f), 0), Quaternion.Euler(-32, 90, 0));
            _nextRocketFire = Time.time + _rocketFireRate;
            _rocketArtilleryFlash.Play();
            //AudioSource.PlayClipAtPoint(_rocketAudioClip,transform.position);
        }

        if (_currentRockets <= 0 && _reloadingRockets == false)
        {
            _rocketArtilleryFlash.Stop();
            _reloadingRockets = true;
            StartCoroutine(RocketReload());
        }
    }

    private IEnumerator RocketReload()
    {
        yield return new WaitForSeconds(_rocketCooldown);
        _currentRockets = _maxRockets;
        _reloadingRockets = false;
    }

    public void UnlockWeapon(int weaponID)
    {
        if (weaponID == 0)
        {
            _canShootMissiles = true;
        }
        else if (weaponID == 1)
        {
            _canShootRockets = true;
        }
    }

    public void Damage(int damageAmount)
    {
        StartCoroutine(_cameraShake.Shake(0.2f, 0.2f));
        if (_health > 0)
        {
            _health -= damageAmount;

            if (_health <= 0)
                Death();
        }
        _uiManager.SetHealth(_health);
    }

    private void Death()
    {
        _isDead = true;
        _rb.useGravity = true;

        _uiManager.PlayerDead();

        _deathExplosion.Play();
        _deathFireBall.SetActive(true);
        _deathExplosion.time = 0.5f;

        var random = Random.onUnitSphere;
        _rb.AddTorque(random * 100);
        _rb.AddForce(Vector3.left * 250);

        _rightWingRB.useGravity = true;
        _leftWingRB.useGravity = true;

        _rightWingRB.transform.parent = null;
        _leftWingRB.transform.parent = null;

        _rightWingRB.AddForce(RandomOnSphere() * 500);
        _leftWingRB.AddForce(RandomOnSphere() * 500);

        _rightWingRB.AddTorque(RandomOnSphere() * 250);
        _leftWingRB.AddTorque(RandomOnSphere() * 250);
    }

    private Vector3 RandomOnSphere()
    {
        var random = Random.onUnitSphere;
        return random;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("EnemyFire"))
        {
            if (Time.time > _nextDamage)
            {
                _nextDamage = Time.time + _enemyDamageRate;
                Damage(3);
            }
        }
    }
}
