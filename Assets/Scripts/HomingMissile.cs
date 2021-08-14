using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10f;

    [SerializeField]
    private float _maxSpeed = 15f;

    [SerializeField]
    private float _rotateSpeed = 100f;

    [SerializeField]
    private float _maxRotSpeed = 100f;

    [SerializeField]
    private int _damage = 1;

    [SerializeField]
    private int _health = 5;

    [SerializeField]
    private GameObject _explosion = null;

    private Transform _playerTransform;

    [SerializeField]
    private Transform _model = null;

    private Rigidbody _rb;

    private bool _follow = true;

    private Material _mat;

    private float _flashTime = 0f;

    [SerializeField]
    private Color _flashColor = Color.black;

    [SerializeField]
    private AudioClip _clip;
    // Start is called before the first frame update
    void Start()
    {
        _playerTransform = GameObject.Find("Player").transform;
        _rb = transform.GetComponent<Rigidbody>();

        _mat = _model.transform.GetComponent<MeshRenderer>().material;
        _mat.EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < _playerTransform.position.x)
        {
            _follow = false;
        }

        if (transform.position.y < -20.5 || transform.position.x < -35.5f)
        {
            GameObject explosion = Instantiate(_explosion, transform.position, Quaternion.identity);
            explosion.transform.GetComponent<ParticleSystem>().time = 0.5f;
            Destroy(explosion, 1f);
            Destroy(this.gameObject);
        }

        if (_speed < _maxSpeed)
        {
            _speed += _speed * Time.deltaTime * 4;
        }

        if (_rotateSpeed < _maxRotSpeed)
        {
            _rotateSpeed += _rotateSpeed * Time.deltaTime * 4;
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

    public void Damage()
    {
        _flashTime = 0.9f;
        _health--;
        if (_health <= 0)
        {
            Death();
        }

        AudioSource.PlayClipAtPoint(_clip, transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.GetComponent<Player>().Damage(_damage);
            Death();
        }
    }

    private void Death()
    {
        GameObject explosion = Instantiate(_explosion, transform.position, Quaternion.identity);
        explosion.transform.GetComponent<ParticleSystem>().time = 0.5f;
        Destroy(explosion, 1f);
        Destroy(this.gameObject);
    }

    private void FixedUpdate()
    {
        _rb.velocity = transform.forward * _speed;

        Vector2 dir = _playerTransform.position - _rb.position;

        dir.Normalize();

        float rotAmount = Vector3.Cross(dir, transform.forward).z;

        _rb.angularVelocity = new Vector3(0,0, -rotAmount * _rotateSpeed);
    }
}
