using Godot;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Classes.Combatant
{
    public enum COMBATANT_COMMANDS {ATTACK, GUARD, SKILL, ITEM};

    public class Combatant: ICombatant
    {
        public int Id { get; set; }
        public string CombatantName { get; set; }
        public float MaxHealth { get; set; }
        public float Health { get; set; }
        public float MaxMana { get; set; }
        public float Mana { get; set; }
        public float Speed { get; set; }
        public float Attack { get; set; }
        public bool PlayerControlled { get; set; }
        public bool Fainted { get; set; } = false;
        public int Level { get; set;}
        public List<Skill> Skills { get; set; }
    }

    public class Skill
    {
        public string Name;
        public Action<ICombatant, ICombatant> Action;
        public float Cost;
    }
}