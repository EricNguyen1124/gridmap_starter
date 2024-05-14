using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public enum COMBATANT_COMMANDS {ATTACK, GUARD, SKILL, ITEM};

public partial class Combatant
{
    public string Name;
    public float Health;
    public float Mana;
	public float Speed;
    public float Attack;
	public bool PlayerControlled;
    public List<Skill> skills;

    public void TakeTurn(COMBATANT_COMMANDS command, Combatant target, string specifier = null)
    {
        switch (command)
        {
            case COMBATANT_COMMANDS.ATTACK:
                target.Health -= this.Attack;
                break;
            case COMBATANT_COMMANDS.SKILL:
                var skill = skills.Single(s => s.Name == specifier);
                skill.action(this, target);
                break;
            case COMBATANT_COMMANDS.GUARD:
            default:
                break;
        }
    }
}

public class Skill
{
    public string Name;
    public Action<Combatant, Combatant> action;
}

public static class SkillList
{
    public static List<Skill> AllSkills = new()
    {
        new Skill {
            Name = "Big Strike",
            action = (self, target) => {
                target.Health -= self.Attack * 2;
                self.Mana -= 10;
            }
        }
    };
}

