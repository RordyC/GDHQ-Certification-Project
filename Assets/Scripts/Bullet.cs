using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _startSpeed = 100f;

    private float _verticalSpeed = 4f;

    private float _holdTime = 0.3f;

    [SerializeField]
    private int _damage = 1;

    [SerializeField]
    private ParticleSystem _thruster = null;

    [SerializeField]
    private GameObject _explosion = null;

    private AudioSource _audio;

    // Update is called once per frame

    private void Start()
    {
        _audio = transform.GetComponent<AudioSource>();
        if (_audio == null)
        {
            Debug.LogError("Audiosource is NULL!");
        }

        _audio.Play();
    }

    void Update()
    {
        if (_holdTime > 0)
        {
            _holdTime -= Time.deltaTime;

            if (_holdTime <= 0)
            {
                _verticalSpeed = -0.75f;
                _thruster.Play();
                transform.parent = null;
            }
        }
        else
        {
            transform.Translate(Vector3.forward * Time.deltaTime * _startSpeed);

            if (_startSpeed < 100)
                _startSpeed += _startSpeed * Time.deltaTime * 5;
        }

        transform.Translate(Vector3.down * Time.deltaTime * _verticalSpeed);

        if (transform.position.x > 200)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject explosion = Instantiate(_explosion, transform.position, Quaternion.identity);
            explosion.transform.GetComponent<ParticleSystem>().time = 0.4f;
            Destroy(explosion, 1f);
            other.transform.GetComponent<Enemy>().Damage(_damage);
            Destroy(this.gameObject);
        }
    }

}
