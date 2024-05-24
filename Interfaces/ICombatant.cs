using System.Collections.Generic;
using System.Linq;
using Godot;
using Classes.Combatant;

namespace Interfaces {
    public interface ICombatant 
    {
        string CombatantName { get; set; }
        float Health { get; set; }
        float Mana { get; set; }
        float Speed { get; set; }
        float Attack { get; set; }
        bool PlayerControlled { get; set; }
        List<Skill> Skills { get; set;}

        // Probably need to make this a list of targets instead
        public void TakeTurn(ICombatant target, COMBATANT_COMMANDS command, string specifier = null)
        {
            GD.Print(CombatantName + " TURN");

            switch (command)
            {
                case COMBATANT_COMMANDS.ATTACK:
                    target.Health -= this.Attack;
                    break;
                case COMBATANT_COMMANDS.SKILL:
                    var skill = Skills.Single(s => s.Name == specifier);
                    skill.Action(this, target);
                    break;
                case COMBATANT_COMMANDS.GUARD:
                default:
                    break;
            }

            GD.Print($"Target {target.CombatantName}: {target.Health}");
        }
    }
}