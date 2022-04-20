using System.Collections;
using Systems;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Player Settings")]
    public float rotationSpeed = 1f;
    public float controllerSpeed = 5f;

    [Header("Sounds Settings")]
    public AudioClip[] deathSounds;
    public AudioClip breathSound;

    [Header("Debug")]
    public bool godMode = false;
    
    private Camera _camera;
    private AudioSource _source;

    private bool _isDead = false;

	private void Start ()
    {
        _camera = Camera.main;
        _source = GetComponent<AudioSource>();

        StartCoroutine(Breath());
    }

    private Vector3 _currentPosition;
    private void Update ()  
    {
        if (!_isDead)
        {
            _currentPosition = transform.position;

            float axis = Input.GetAxis("Horizontal");

            if (Mathf.Abs(axis) > float.Epsilon)
                _currentPosition += Vector3.right * axis * Time.deltaTime * controllerSpeed;
            
            // Clamp VR or Controller
            transform.position = new Vector3(
                Mathf.Clamp(_currentPosition.x + (_camera.transform.localRotation.z * rotationSpeed * Time.deltaTime * -1), -3f, 3f), 
                _currentPosition.y, 
                _currentPosition.z);
        }
    }

    // @TODO: Move this to an audio system.
    private IEnumerator Breath()
    {
        yield return new WaitForSeconds(Random.Range(1, 3));
        _source.clip = breathSound;
        _source.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_isDead || godMode) return;
        _isDead = true;
        
        // @TODO: Move this to an audio system.
        _source.Stop();
        _source.clip = deathSounds[Random.Range(0, deathSounds.Length)];
        _source.Play();
        GameEvents.DispatchOnPlayerDies();
    }
}