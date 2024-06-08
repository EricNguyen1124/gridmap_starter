using System.Collections.Generic;
using System.Linq;
using Godot;
using Classes.Combatant;

namespace Interfaces {
    public interface ICombatant 
    {
        int Id { get; set; }
        string CombatantName { get; set; }
        float MaxHealth { get; set; }
        float Health { get; set; }
        float MaxMana { get; set; }
        float Mana { get; set; }
        float Speed { get; set; }
        float Attack { get; set; }
        bool PlayerControlled { get; set; }
        bool Fainted { get; set; }
        public int Level { get; set;}
        List<Skill> Skills { get; set;}

        // Probably need to make this a list of targets instead
        public void TakeTurn(ICombatant target, COMBATANT_COMMANDS command, string specifier = null)
        {
            GD.Print($"{CombatantName} does {command} to {target.CombatantName}!");
            switch (command)
            {
                case COMBATANT_COMMANDS.ATTACK:
                    target.Health -= this.Attack;
                    GD.Print($"and does {this.Attack} damage!");
                    break;
                case COMBATANT_COMMANDS.SKILL:
                    var skill = Skills.Single(s => s.Name == specifier);
                    GD.Print($"and performs a {skill.Name}!");
                    skill.Action(this, target);
                    break;
                case COMBATANT_COMMANDS.GUARD:
                default:
                    break;
            }

            GD.Print($"{target.CombatantName}'s health is now {target.Health}");
        }

        public void ResetHealth()
        {
            Health = MaxHealth;
        }
    }
}