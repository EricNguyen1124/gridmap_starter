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
        public int Id { get; set; }
        public string CombatantName { get; set; }
        public float Health { get; set; }
        public float Mana { get; set; }
        public float Speed { get; set; }
        public float Attack { get; set; }
        public bool PlayerControlled { get; set; } = false;
        public int Level { get; set;}
        public List<Skill> Skills { get; set; }
        public ENEMY_BEHAVIOR Behavior { get; set; } = ENEMY_BEHAVIOR.NORMAL;

        public Texture2D SpriteTexture { get; set; }
        private Sprite3D Sprite { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Sprite = GetNode<Sprite3D>("Sprite3D");
            Sprite.Texture = SpriteTexture;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

        public void SetEnemyProperties(Enemy enemy)
        {
            CombatantName = enemy.CombatantName;
            Health = enemy.Health;
            Mana = enemy.Mana;
            Speed = enemy.Speed;
            Attack = enemy.Attack;
            Level = enemy.Level;
            Skills = enemy.Skills;
            Behavior = enemy.Behavior;
            SpriteTexture = enemy.SpriteTexture;
        }
        
        private Func<List<ICombatant>, (ICombatant, COMBATANT_COMMANDS, string)> GetBehavior(ENEMY_BEHAVIOR behavior) 
        {
            Dictionary<ENEMY_BEHAVIOR, Func<List<ICombatant>, (ICombatant, COMBATANT_COMMANDS, string)>> BehaviorLibrary = new() 
            { 
                { ENEMY_BEHAVIOR.NORMAL, 
                    (combatants) => {
                        var otherCombatants = GetAllCombatantsExceptSelf(combatants);

                        var random = new Random();
                        var index = random.Next(otherCombatants.Count);

                        if (random.NextDouble() < 0.2f)
                        {
                            return (otherCombatants[index], COMBATANT_COMMANDS.SKILL, "Strike");
                        }
                        else
                        {
                            return (otherCombatants[index], COMBATANT_COMMANDS.ATTACK, null);
                        }
                    }
                }
            };

            return BehaviorLibrary[behavior];
        }

        private List<ICombatant> GetAllCombatantsExceptSelf(List<ICombatant> combatants)
        {
            return combatants.Where(c => c.Id != Id).ToList();
        }
    }
}