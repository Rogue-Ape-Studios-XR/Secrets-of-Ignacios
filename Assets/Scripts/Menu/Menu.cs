using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace RogueApeStudios.SecretsOfIgnacios.Menu
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] UnityEvent PressEvent;
        [SerializeField] string Tag;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag))
            {
                PressEvent.Invoke();
            }
        }

        public void Play()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
