using System.Text;
using InventoryService;
using TMPro;
using UnityEngine;

namespace UI
{
    public class InventoryPanelUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text contentText;
        [SerializeField] private GameObject panelRoot;

        private PlayerInventoryController _playerInventory;

        public void BindLocalPlayer(PlayerInventoryController inventory)
        {
            _playerInventory = inventory;
        }

        private void Update()
        {
            if (panelRoot != null && !panelRoot.activeSelf) return;
            if (_playerInventory == null || contentText == null) return;

            var builder = new StringBuilder();
            builder.AppendLine($"Money: ${_playerInventory.Money}");
            contentText.text = builder.ToString();
        }

        public void TogglePanel()
        {
            if (panelRoot != null) panelRoot.SetActive(!panelRoot.activeSelf);
        }
    }
}
