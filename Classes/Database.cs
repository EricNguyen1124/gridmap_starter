using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Classes.Combatant;
using Godot;
using Interfaces;
using Scenes.Enemy;

namespace Classes.Database
{
    public enum SKILLS 
    { 
        STRIKE,
        FIREBALL,
        COUNT 
    }

    public enum ENEMIES
    {
        BLOB,
        SKELETON,
        COUNT
    }

    public static class Database
    {
        public static Dictionary<SKILLS, Skill> SkillLibrary = new()
        {
            {
                SKILLS.STRIKE, 
                new Skill {
                    Name = "Strike",
                    Action = (self, target) => {
                        target.Health -= (float)Math.Round(self.Attack * 1.5f);
                    },
                    Cost = 2.0f
                }
            },
            {
                SKILLS.FIREBALL, 
                new Skill {
                    Name = "Fireball",
                    Action = (self, target) => {
                        target.Health -= 5 + (self.Level * 3);
                    },
                    Cost = 5.0f
                }
            }
        };

        public static Dictionary<ENEMIES, Enemy> EnemyLibrary = new()
        {
            {
                ENEMIES.BLOB, 
                new Enemy {
                    CombatantName = "Blob",
                    MaxHealth = 7,
                    Mana = 8,
                    Speed = 5,
                    Attack = 2,
                    Level = 1,
                    Skills = new List<Skill>() {
                        SkillLibrary[SKILLS.STRIKE]
                    },
                    Behavior = ENEMY_BEHAVIOR.NORMAL,
                    SpriteTexture = LoadEnemySprite("blob.png")
                }
            },
            {
                ENEMIES.SKELETON, 
                new Enemy {
                    CombatantName = "Skeleton",
                    MaxHealth = 5,
                    Mana = 2,
                    Speed = 8,
                    Attack = 1,
                    Level = 1,
                    Skills = new List<Skill>() {
                        SkillLibrary[SKILLS.STRIKE]
                    },
                    Behavior = ENEMY_BEHAVIOR.NORMAL,
                    SpriteTexture = LoadEnemySprite("skeleton.png")
                }
            }
        };

        public static Dictionary<ENEMY_BEHAVIOR, Func<ICombatant, List<ICombatant>, (ICombatant, COMBATANT_COMMANDS, string)>> EnemyBehaviorLibrary = new()
        {
            {
                ENEMY_BEHAVIOR.NORMAL,
                (self, combatants) => {
                    var random = new Random();

                    var viableTargets = combatants.Where(c => c.Id != self.Id && c.PlayerControlled).ToList();
                    var index = random.Next(viableTargets.Count);
                    var randomTarget = viableTargets[index];

                    if (random.NextDouble() < 0.2f)
                    {
                        return (randomTarget, COMBATANT_COMMANDS.SKILL, "Strike");
                    }
                    else
                    {
                        return (randomTarget, COMBATANT_COMMANDS.ATTACK, null);
                    }
                }
            }
        };

        private static Texture2D LoadEnemySprite(string fileName)
        {
            string enemySpritePath = "res://assets/Enemies/";
            return GD.Load<Texture2D>(enemySpritePath + fileName);
        }
    }
}