using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _monkeyBalls0 = null;

    [SerializeField]
    private GameObject _explosionPrefab = null;

    [SerializeField]
    private int _damage = 2;
    // Start is called before the first frame update
    void Start()
    {
        _monkeyBalls0.AddForce(Vector3.up * Random.Range(700,900));
        _monkeyBalls0.AddForce(Vector3.right * Random.Range(400, 600));
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(_monkeyBalls0.velocity);
        if (transform.position.y < -20)
        {
            Destroy(this.gameObject);
        }

        if (transform.rotation.x > 0)
        {
            _monkeyBalls0.AddForce(Vector3.down * 25);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.transform.GetComponent<Enemy>().Damage(_damage);
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion,1f);
            Destroy(this.gameObject);
        }
    }
}
