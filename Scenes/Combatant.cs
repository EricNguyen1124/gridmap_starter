using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class Combatant
{
    public string Name;
    public float Health;
    public float Mana;
	public float Speed;
    public float Attack;
	public bool PlayerControlled;
    public List<Skill> skills;

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

