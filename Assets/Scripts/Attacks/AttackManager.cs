using System.Collections.Generic;
using UnityEngine;

namespace Weltraumsknecht.Attacks
{

    /// <summary>
    /// Passes through update calls for all attacks.
    /// Doing this in a separate script means the attacks can continue even if the Attacker is destroyed (e.g. for projectiles with effects on landing)
    /// </summary>
    public class AttackManager : MonoBehaviour
    {
        public static AttackManager Instance
        { get; private set; }

        HashSet<Attack> activeAttacks;

        void Awake()
        {
            activeAttacks = new();
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            foreach (Attack attack in activeAttacks)
            {
                attack.Update();
            }

            activeAttacks.RemoveWhere(a => !a.IsActive());
        }

        public void AddAttack(Attack attack)
        {
            activeAttacks.Add(attack);
        }

        public void RemoveAttack(Attack attack)
        {
            activeAttacks.Remove(attack);
        }

        private void OnDisable()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
