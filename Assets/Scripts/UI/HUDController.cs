using HealthService;
using InteractionService;
using InventoryService;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private TMP_Text interactPromptText;

        private HealthComponent _playerHealth;
        private PlayerInventoryController _playerInventory;

        public void BindLocalPlayer(HealthComponent health, PlayerInventoryController inventory)
        {
            _playerHealth = health;
            _playerInventory = inventory;
        }

        private void OnEnable()
        {
            InteractionActionManager.onPromptChanged += SetInteractPrompt;
        }

        private void OnDisable()
        {
            InteractionActionManager.onPromptChanged -= SetInteractPrompt;
        }

        private void Update()
        {
            if (_playerHealth != null && healthText != null)
                healthText.text = $"HP: {_playerHealth.CurrentHealth:0}/{_playerHealth.MaxHealth:0}";

            if (_playerInventory != null && moneyText != null)
                moneyText.text = $"${_playerInventory.Money}";
        }

        private void SetInteractPrompt(string prompt)
        {
            if (interactPromptText == null) return;
            interactPromptText.text = prompt;
            interactPromptText.gameObject.SetActive(!string.IsNullOrEmpty(prompt));
        }
    }
}
