using Godot;
using Interfaces;
using System;
using System.Collections.Generic;
using Classes.Combatant;
using System.Linq;

namespace Scenes.Enemy
{
    public enum ENEMY_BEHAVIOR { NORMAL }

    public partial class Enemy : Node3D, ICombatant
    {
        public Func<List<ICombatant>, (ICombatant, COMBATANT_COMMANDS, string)> MakeTurnDecision { get { return GetBehavior(Behavior); } }
        public string CombatantName { get; set; }
        public float Health { get; set; }
        public float Mana { get; set; }
        public float Speed { get; set; }
        public float Attack { get; set; }
        public bool PlayerControlled { get; set; }
        public List<Skill> Skills { get; set; }
        public ENEMY_BEHAVIOR Behavior { get; set; } = ENEMY_BEHAVIOR.NORMAL;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }
        
        private Func<List<ICombatant>, (ICombatant, COMBATANT_COMMANDS, string)> GetBehavior(ENEMY_BEHAVIOR behavior) 
        {
            Dictionary<ENEMY_BEHAVIOR, Func<List<ICombatant>, (ICombatant, COMBATANT_COMMANDS, string)>> BehaviorLibrary = new() 
            { 
                { ENEMY_BEHAVIOR.NORMAL, 
                    (combatants) => {
                        if (Health < 1.0f) { GD.Print("I'M DYING!!"); }
                        return (combatants.First(c => c.CombatantName != CombatantName), COMBATANT_COMMANDS.ATTACK, null);
                    }
                }
            };

            return BehaviorLibrary[behavior];
        }
    }
}
