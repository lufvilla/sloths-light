using Systems;
using UnityEngine;

public class MenuController : MonoBehaviour {


    [Header("Sounds")]
    public AudioClip buttonClick;
    public AudioClip introSound;

    private Animation _animation;
    private AudioSource _source;

    private void Start()
    {
        _animation = GetComponent<Animation>();
        _source = GetComponent<AudioSource>();
    }

    public void StartAnimation()
    {
        _source.clip = buttonClick;
        _source.Play();
        _animation.Play();
    }

    public void StartGame()
    {
        SceneLoaderSystem.Load(SceneID.Game);
    }

    public void PlaySound()
    {
        _source.clip = introSound;
        _source.Play();
    }
}
