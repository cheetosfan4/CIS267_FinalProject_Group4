using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {
    public AudioClip[] music;
    private AudioSource musicPlayer;
    private string currentScene;

    private void Start() {
        musicPlayer = GetComponent<AudioSource>();
        currentScene = SceneManager.GetActiveScene().name;
        musicPlayer.volume = 0.75f;
    }

    public void playSceneMusic() {
        if (musicPlayer == null) {
            musicPlayer = GetComponent<AudioSource>();
        }
        currentScene = SceneManager.GetActiveScene().name;
        musicPlayer.Stop();
        if (currentScene == "LevelOne") {
            musicPlayer.clip = music[1];
        }
        else if (currentScene == "LevelTwo") {
            musicPlayer.clip = music[2];
        }
        else if (currentScene == "LevelThree") {
            musicPlayer.clip = music[3];
        }
        else if (currentScene == "MainMenu") {
            musicPlayer.clip = music[3];
        }
        else {
            musicPlayer.clip = music[0];
        }
        musicPlayer.Play();
    }

    public void playBossMusic() {
        musicPlayer.Stop();
        musicPlayer.clip = music[4];
        musicPlayer.Play();
    }
    public void playFinalBossMusic() {
        musicPlayer.Stop();
        musicPlayer.clip = music[5];
        musicPlayer.Play();
    }
}