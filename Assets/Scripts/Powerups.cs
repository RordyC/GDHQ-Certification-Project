using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerups : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 100f;

    [SerializeField]
    private int _powerupID = 0;

    [SerializeField]
    private GameObject _collectedEffect = null;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up,  _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _collectedEffect.SetActive(true);
            Destroy(_collectedEffect, 2.5f);
            _collectedEffect.transform.parent = null;
            other.transform.GetComponent<Player>().UnlockWeapon(_powerupID);
            Destroy(this.gameObject);
        }
    }
}
