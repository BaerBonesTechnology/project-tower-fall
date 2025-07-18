using System;
using System.IO;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Baerhous.Games.Towerfall
{
    public class HostWorldManager : MonoBehaviour
    {
        [SerializeField] private Button hostGameBtn;
        public GameObject worldNamePanel;
        public TMP_InputField worldNameInput;
        [SerializeField] private Button confirmWorldBtn;
        public string worldSceneName = "GameScene";

        private void Start()
        {
            worldNamePanel.SetActive(false);

            hostGameBtn.onClick.AddListener(ShowWorldNamePanel);
            confirmWorldBtn.onClick.AddListener(OnConfirmWorld);
        }
        void ShowWorldNamePanel()
        {
            worldNamePanel.SetActive(true);
        }

        private void OnConfirmWorld()
        {
            string worldName = worldNameInput.text.Trim();
            if (string.IsNullOrEmpty(worldName))
            {
                Debug.LogWarning("World name is empty...");
                return;
            }
            string worldPath = Path.Combine(Application.persistentDataPath, "worldsaves", worldName);
            if (!Directory.Exists(worldPath))
            {
                Directory.CreateDirectory(worldPath);
                SaveWorldMetaData(worldPath, worldName);
            }

            NetworkManager.Singleton.StartHost();

            SceneManager.LoadScene(worldSceneName);
        }




        void SaveWorldMetaData(string path, string worldName)
        {
            WorldMeta meta = new WorldMeta
            {
                name = worldName,
                createdAt = System.DateTime.Now.ToString("s")
            };
            string json = JsonUtility.ToJson(meta,true);
            File.WriteAllText(Path.Combine(path, "worldData.json"), json);
        }


        [System.Serializable]
        public class WorldMeta
        {
            public string name;
            public string createdAt;
        }
    }
}
