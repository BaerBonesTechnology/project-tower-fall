using System;
using UnityEngine;

namespace Baerhous.Games.Towerfall.Game.Player
{
    public class SpellbookController : MonoBehaviour
    {
        public GameObject[] SpellsGO;
        
        private bool spellbookLoaded;
        private Spell[] _spells;

        private void Awake()
        {
            PopulateSpellBook();
        }

        private void PopulateSpellBook()
        {
            
        }
    }
    
    
    public abstract class Spell : MonoBehaviour
    {
        [Header("Spell Data")]
        public SpellInfo spellInfo;

        public abstract void ActivateSpell();
    }

    [CreateAssetMenu(fileName = "Spells", menuName = "Spells/Create Spell Info")]
    public class SpellInfo : ScriptableObject
    {
        [Header("Spell Data")]
        [Tooltip("Name of Spell")]
        [SerializeField]
        public string name;

        public string description;

        [Tooltip("Base Spell Power")]
        [SerializeField]
        public float attackPower;

        [Tooltip("Spell Cooldown in seconds")]
        [SerializeField] 
        public int spellCooldown;

        [Tooltip("Spell Unlocked for Character")]
        [SerializeField] public bool isUnlocked;
        
        [Header("Spell Animation")]
        [Tooltip("Spell Controller Animation")]
        public Animation spellActivationAnimation;

        [Tooltip("Spell Particles")] public ParticleSystem spellParticles;

    }
}