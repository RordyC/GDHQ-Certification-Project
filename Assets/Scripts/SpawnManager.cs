using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private UIManager _uiManager = null;

    [SerializeField]
    private GameObject _mig29Prefab;

    private Vector3 _mig29SpawnRot = new Vector3(-20,0,0);
    private Vector3 _su25SpawnRot = new Vector3(0, -90, 0);

    [SerializeField]
    private GameObject _su25Prefab;

    [SerializeField]
    private GameObject _DRONEPrefab;

    public void SpawnMIG29()
    {
        Instantiate(_mig29Prefab, transform.position + new Vector3(0, Random.Range(-15f, 15)), Quaternion.Euler(_mig29SpawnRot));
        Debug.Log("Spawning a MIG29!");
    }

    public void SpawnSU25()
    {
        Instantiate(_su25Prefab, transform.position + new Vector3(0, Random.Range(-15f, 15),0), Quaternion.Euler(_su25SpawnRot));
    }

    public void SpawnDRONE(int powerupToSpawn)
    {
        GameObject drone = Instantiate(_DRONEPrefab, transform.position + new Vector3(0, Random.Range(-15f, 15), 0), Quaternion.Euler(_su25SpawnRot));
        drone.transform.GetComponent<Enemy>().SetPowerupDrop(powerupToSpawn);
    }

    public void UpdateWaves(int wave)
    {
        _uiManager.UpdateWaveText(wave);
    }
}
