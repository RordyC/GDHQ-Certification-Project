using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    [SerializeField]
    private float _startSpeed = 100f;

    [SerializeField]
    private int _damage = 1;

    [SerializeField]
    private ParticleSystem _thruster = null;

    [SerializeField]
    private GameObject _explosion = null;

    private bool _activated = false;

    // Update is called once per frame
    void Update()
    {
        if (_activated == true)
        {

            transform.Translate(Vector3.right * Time.deltaTime * _startSpeed);

            if (_startSpeed < 50)
                _startSpeed += _startSpeed * Time.deltaTime * 5;

        }

        if (transform.position.x < -35)
        {
            Destroy(this.gameObject);
        }
    }

    public void Activate()
    {
        _activated = true;
        transform.parent = null;
        _thruster.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.GetComponent<Player>().Damage(_damage);
            GameObject explosion = Instantiate(_explosion, transform.position, Quaternion.identity);
            explosion.transform.GetComponent<ParticleSystem>().time = 0.5f;
            Destroy(explosion, 1f);
            Destroy(this.gameObject);
        }
    }
}
