using System;
using System.Collections.Generic;
using System.Linq;

namespace FantasyEngineData.Skills
{
    public static class SkillManager
    {
        private static Dictionary<string, BaseSkill> baseSkills = new Dictionary<string, BaseSkill>();

        public static void Load(params BaseSkill[] baseSkills)
        {
            foreach (BaseSkill baseSkill in baseSkills)
            {
                AddBaseSkill(baseSkill);
            }
        }

        public static void AddBaseSkill(BaseSkill baseSkill)
        {
            if (!baseSkills.ContainsKey(baseSkill.Name))
            {
                baseSkills.Add(baseSkill.Name, baseSkill);
            }
        }

        public static BaseSkill GetBaseSkill(string skillName)
        {
            if (baseSkills.ContainsKey(skillName))
            {
                return (BaseSkill)baseSkills[skillName];
            }
            return null;
        }
    }
}
