using UnityEngine;

namespace GunService
{
    [CreateAssetMenu(fileName = "WeaponDefinition", menuName = "Game/Weapon Definition")]
    public class WeaponDefinition : ScriptableObject
    {
        public string WeaponId;
        public float Damage = 20f;
        public float Range = 50f;
        public float FireRate = 4f;
        public AudioClip FireSound;
    }
}
