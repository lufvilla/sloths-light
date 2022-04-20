using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems
{
    public class SceneLoaderSystem : MonoBehaviour
    {
        private bool _isLoading = false;

        private static SceneLoaderSystem _instance;

        private static SceneLoaderSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("SceneLoaderSystem");
                    _instance = go.AddComponent<SceneLoaderSystem>();
                    DontDestroyOnLoad(_instance);
                }
                return _instance;
            }
        }

        public static void Load(SceneID scene)
        {
            Instance.LoadScene(scene);
        }

        private void LoadScene(SceneID scene)
        {
            if(_isLoading) return;
            _isLoading = true;
            
            StartCoroutine(LoadScene((int)scene));
        }

        private IEnumerator LoadScene(int sceneIndex)
        {
            yield return SceneManager.LoadSceneAsync(sceneIndex);

            _isLoading = false;
        }
    }
    
    public enum SceneID
    {
        Menu = 0,
        Game
    }
}