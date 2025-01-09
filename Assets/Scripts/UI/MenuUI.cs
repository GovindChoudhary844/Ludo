using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ludo.UI
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown ddPlayer;
        [SerializeField] private TMP_Dropdown ddGoti;

        public void OnPlay()
        {
            var pd = FindObjectOfType<PersistentData>();
            if (pd != null)
            {
                pd.SetData(ddPlayer.value, ddGoti.value);
            }
            else
            {
                Debug.Log("Persistent not found!");
            }

            SceneManager.LoadScene("Game");
        }
    }
}