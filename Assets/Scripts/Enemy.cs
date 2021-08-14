using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private enum Type
    {
        MIG29,
        SU25,
        NONE,
        DRONE,
    }

    [SerializeField]
    private Type _type = Type.MIG29;

    [Header("General")]

    [SerializeField]
    private int _health = 5;

    [SerializeField]
    private float _speed = 15;

    [SerializeField]
    private GameObject _model;

    private Material _mat;

    [SerializeField]
    private float _flashTime = 0f;

    [SerializeField]
    private Color _flashColor = Color.black;

    [SerializeField]
    private GameObject _miniExplosionPrefab = null;

    [SerializeField]
    private ParticleSystem _explosionPrefab = null;

    private Rigidbody _rb;

    private bool _isDead = false;

    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip[] _damageSounds;

    [SerializeField]
    private AudioClip[] _deathSounds;


    [SerializeField]
    private GameObject _fireBall = null;

    private CameraShake _cameraShake;

    [Header("MIG29")]

    [SerializeField]
    private EnemyMissile _rightMissile, _leftMissile;

    private Vector3 _target;

    private float _lerpSpeed = 1f;

    private bool _intro = true;

    [Header("SU25")]

    [SerializeField]
    private float _followSpeed = 5f;

    [SerializeField]
    private int _attacks = 3;

    [SerializeField]
    private Transform _playerTransform = null;

    [SerializeField]
    private ParticleSystem _rightLMG, _leftLMG, _rightFlash, _leftFlash;

    [SerializeField]
    private GameObject _gunFireHitbox = null;

    private bool _isShooting = false;

    [Header("DRONE")]

    [SerializeField]
    private int _powerupToDrop = 0;

    [SerializeField]
    private GameObject[] _powerups;

    [SerializeField]
    private GameObject _homingMissilePrefab = null;

    [SerializeField]
    private float _missileFireRate = 4f;

    private float _nextMissileFire;
    // Start is called before the first frame update
    void Start()
    {
        _mat = _model.transform.GetComponent<MeshRenderer>().material;
        _mat.EnableKeyword("_EMISSION");

        _rb = transform.GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("RB is NULL!");
        }

        _cameraShake = GameObject.Find("Main Camera").transform.GetComponent<CameraShake>();

        if (_cameraShake == null)
        {
            Debug.LogError("Camera Shaker is NULL!");
        }

        _playerTransform = GameObject.Find("Player").GetComponent<Transform>();

        if (_playerTransform == null)
        {
            Debug.LogError("Player transform is NULL!");
        }

        _audioSource = transform.GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Audio SOurce is NULL!");
        }

        _target = transform.position + new Vector3(-20, 0, 0);

        switch (_type)
        {
            case Type.MIG29:
                StartCoroutine(MIG29MovementRoutine());
                break;
            case Type.SU25:
                StartCoroutine(SU25MovementRoutine());
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (_type)
        {
            case Type.MIG29:
                MIGMovement();
                break;
            case Type.SU25:
                SU25Movement();
                break;
            case Type.DRONE:
                DRONEMovement();
                break;
        }

        if (_flashTime > 0)
        {
            _flashColor.r = _flashTime;
            _flashColor.g = _flashTime;
            _flashColor.b = _flashTime;

            _mat.SetColor("_EmissionColor", _flashColor);
            _flashTime -= Time.deltaTime * 5;

            if (_flashTime < 0)
            {
                _flashTime = 0;
            }
        }

        if (transform.position.x < -55)
        {
            Destroy(this.gameObject);
        }
    }

    private void MIGMovement()
    {
        if (_isDead == false )
        {
            //transform.Rotate(Vector3.right, Time.deltaTime * 120);
            if (_intro == true)
            {
                transform.position = Vector3.Lerp(transform.position, _target, Time.deltaTime * _lerpSpeed);
                if (transform.position == _target)
                {
                    _intro = false;
                }
            }
            else
            {
                transform.Translate(Vector3.left * Time.deltaTime * _speed);
            }
        }

        //transform.Translate(Vector3.left* Time.deltaTime * 10);
    }

    private void SU25Movement()
    {
        if (_isDead == false && _attacks > 0)
        {
            if (transform.position.x != 24)
            {
                float newXPos = Mathf.MoveTowards(transform.position.x, 24, Time.deltaTime * _followSpeed);
                transform.position = new Vector3(newXPos, transform.position.y, transform.position.z);
            }

            float newYPos = Mathf.SmoothStep(transform.position.y, _playerTransform.position.y, Time.deltaTime * _followSpeed);
            transform.position = new Vector3(transform.position.x, newYPos, transform.position.z);
        }
        else if (_attacks <= 0)
        {
            transform.Translate(Vector3.forward* Time.deltaTime * _speed);
        }
    }

    private void DRONEMovement()
    {
        if (Time.time >_nextMissileFire)
        {
            _nextMissileFire = Time.time + _missileFireRate;
            Instantiate(_homingMissilePrefab, transform.position, Quaternion.Euler(0, -90, 0));
        }

        if (_isDead == false)
        {
            //transform.Rotate(Vector3.right, Time.deltaTime * 120);
            if (_intro == true)
            {
                transform.position = Vector3.Lerp(transform.position, _target, Time.deltaTime * _lerpSpeed);
                if (transform.position == _target)
                {
                    _intro = false;
                }
            }
            else
            {
                transform.Translate(Vector3.left * Time.deltaTime * _speed);
            }
        }
    }

    public void Damage(int damageAmount)
    {
        if (_isDead)
            return;

        float randomX = Random.Range(-5f, 5f);
        float randomY = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-5f, 0f);

        var randomClip = Random.Range(0, _damageSounds.Length);

        AudioSource.PlayClipAtPoint(_damageSounds[randomClip], transform.position);

        _health -= damageAmount;
        _flashTime = 0.9f;
        GameObject explosion = Instantiate(_miniExplosionPrefab, transform.position + new Vector3(randomX, randomY, randomZ), Quaternion.identity);
        Destroy(explosion,1f);
        if (_health <= 0 && _isDead == false)
        {
            Death();
        }
    }

    public void SetPowerupDrop(int drop)
    {
        _powerupToDrop = drop;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isDead)
                return;

            other.transform.GetComponent<Player>().Damage(1);
            Death();
        }
    }

    private void Death()
    {
        StartCoroutine(_cameraShake.Shake(0.2f, 0.4f));

        var random = Random.onUnitSphere;
        _isDead = true;

        _rb.useGravity = true;

        var randomClip = Random.Range(0, _deathSounds.Length);

        _audioSource.clip = _deathSounds[randomClip];
        _audioSource.Play();

        if (_type == Type.MIG29 || _type == Type.NONE)
        {
            _rb.AddTorque(random * 100);
            _rb.AddForce(Vector3.right * 250);
        }


        if (_type == Type.SU25)
        {
            _gunFireHitbox.SetActive(false);
            _rb.AddForce(Vector3.up * 1000);
            _rb.AddTorque(random * 50);
        }

        if (_type == Type.DRONE)
        {
            Instantiate(_powerups[_powerupToDrop], transform.position, Quaternion.Euler(-21,160,-2));
            _rb.AddTorque(random * 100);
            _rb.AddForce(Vector3.right * 250);
        }


        if (_explosionPrefab != null)
        {
            _explosionPrefab.Play();
            _explosionPrefab.time = 0.5f;
        }

        if (_fireBall != null)
        {
            _fireBall.SetActive(true);
        }

        Destroy(this.gameObject, 6f);
    }

    IEnumerator MIG29MovementRoutine()
    {
        yield return new WaitForSeconds(2.5f);
        if (_isDead == false)
            _rightMissile.Activate();
        yield return new WaitForSeconds(1f);
        if (_isDead == false)
            _leftMissile.Activate();
        yield return new WaitForSeconds(1f);
        _intro = false;
    }

    IEnumerator SU25MovementRoutine()
    {
        var oldSpeed = _followSpeed;
        yield return new WaitForSeconds(2f);
        while (_attacks > 0)
        {
            yield return new WaitForSeconds(5f);
            _isShooting = true;
            yield return new WaitForSeconds(1f);
            if (_isDead == false)
            {
                _followSpeed = 2;
                _gunFireHitbox.SetActive(true);
                _rightLMG.Play();
                _leftLMG.Play();
                _leftFlash.Play();
                _rightFlash.Play();
            }
            yield return new WaitForSeconds(3f);
            _isShooting = false;
            if (_isDead == false)
            {
                _followSpeed = oldSpeed;
                _gunFireHitbox.SetActive(false);
                _rightLMG.Stop();
                _leftLMG.Stop();
                _leftFlash.Stop();
                _rightFlash.Stop();
            }
            _attacks--;
        }

    }
}
