using System.Collections.Generic;
using System.Linq;
using Classes.Combatant;
using Scenes.Enemy;

namespace Classes.Database
{
    public enum SKILLS 
    { 
        STRIKE, 
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
                        target.Health -= self.Attack * 2;
                        self.Mana -= 10;
                    }
                }
            }
        };

        public static Dictionary<ENEMIES, Enemy> EnemyLibrary = new()
        {
            {
                ENEMIES.BLOB, 
                new Enemy {
                    CombatantName = "Blob",
                    Health = 7,
                    Mana = 3,
                    Speed = 5,
                    Attack = 2,
                    Level = 1,
                    Skills = new List<Skill>() {
                        SkillLibrary[SKILLS.STRIKE]
                    },
                    Behavior = ENEMY_BEHAVIOR.NORMAL
                }
            },
            {
                ENEMIES.SKELETON, 
                new Enemy {
                    CombatantName = "Skeleton",
                    Health = 5,
                    Mana = 2,
                    Speed = 8,
                    Attack = 1,
                    Level = 1,
                    Skills = new List<Skill>() {
                        SkillLibrary[SKILLS.STRIKE]
                    },
                    Behavior = ENEMY_BEHAVIOR.NORMAL
                }
            }
        };
    }
}