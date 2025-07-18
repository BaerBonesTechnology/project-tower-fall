using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Baerhous.Games.Towerfall
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private Button playOnline;
        [SerializeField] private Button hostLocal;
        [SerializeField] private Button options;
        [SerializeField] private Button createWorld;


        private void Awake()
        {
            playOnline.onClick.AddListener(() => {});
            hostLocal.onClick.AddListener(() => SceneManager.LoadScene("LocalHost"));
            options.onClick.AddListener(() => {});
        }
    }
}
