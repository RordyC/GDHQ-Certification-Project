using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Slider _playerHealthBarSlider;

    [SerializeField]
    private Slider _smoothPlayerHealthSlider;

    [SerializeField]
    private float _currentPlayerHealth = 100;

    private float _smoothHealth = 100f;

    [SerializeField]
    private GameObject _pauseMenuPanel = null;

    [SerializeField]
    private GameObject _deathScreenPanel = null;

    private bool _isPlayerDead = false;

    [SerializeField]
    private Animator _deathScreenAnimator = null;

    [SerializeField]
    private Animator _waveTextAnimator = null;

    [SerializeField]
    private TextMeshProUGUI _waveText = null;

    public void SetHealth(int health)
    {
        _playerHealthBarSlider.value = health;
        _currentPlayerHealth = health;
    }

    private bool _isPaused = false;

    private void Update()
    {
        float y = Mathf.MoveTowards(_smoothHealth, _currentPlayerHealth, Time.deltaTime * 10);
        _smoothHealth = y;
        _smoothPlayerHealthSlider.value = y;

        if (Input.GetKeyDown(KeyCode.Escape) && _isPlayerDead == false)
        {
            if (_isPaused == true)
            {
                Resume();
            }
            else
            {
                Time.timeScale = 0f;
                _isPaused = true;
                _pauseMenuPanel.SetActive(true);
            }
        }

    }

    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        _isPaused = false;
        _pauseMenuPanel.SetActive(false);
    }

    public void PlayerDead()
    {
        _deathScreenPanel.SetActive(true);
        _isPlayerDead = true;
        _deathScreenAnimator.SetTrigger("Play");
    }

    public void UpdateWaveText(int wave)
    {
        _waveTextAnimator.SetTrigger("Play");
        _waveText.text = "Wave " + wave;
    }

}
