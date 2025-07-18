using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Baerhous.Games.Towerfall
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField] private Button srvrBtn;
        [SerializeField] public Button hostBtn;
        [SerializeField] private Button clientBtn;


        private void Awake()
        {
            srvrBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartServer(); });
            hostBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartHost(); });
            clientBtn.onClick.AddListener(() => { NetworkManager.Singleton.StartClient(); });
        }
    }
}
