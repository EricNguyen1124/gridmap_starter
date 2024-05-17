using Godot;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public enum COMBATANT_COMMANDS {ATTACK, GUARD, SKILL, ITEM};

public class Combatant: ICombatant
{
    public string CombatantName { get; set; }
    public float Health { get; set; }
    public float Mana { get; set; }
	public float Speed { get; set; }
    public float Attack { get; set; }
	public bool PlayerControlled { get; set; }
    public List<Skill> Skills { get; set; }
}

public class Skill
{
    public string Name;
    public Action<ICombatant, ICombatant> Action;
}

public static class SkillList
{
    public static List<Skill> AllSkills = new()
    {
        new Skill {
            Name = "Big Strike",
            Action = (self, target) => {
                target.Health -= self.Attack * 2;
                self.Mana -= 10;
            }
        }
    };
}

