using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using FantasyEngineData.Skills;
using FantasyEngineData.Entities;

namespace FantasyEngine.Classes.Battles
{
    public class SkillSelection : Command
    {
        private Character _Actor;
        private List<Skill> list = new List<Skill>();

        public Character Actor
        {
            get { return _Actor; }
            set
            {
                _Actor = value;
                RefreshChoices();
            }
        }

        public Skill SkillSelected { get { return list[CursorPosition]; } }

        public SkillSelection(Game game, int width, int height)
            : base(game, width, new string[] { "" }, 2)
        {
            Rectangle.Height = height;
        }

        /// <summary>
        /// Refresh the list of items.
        /// </summary>
        public void RefreshChoices()
        {
            int height = Rectangle.Height;
            list.Clear();
            foreach (var skill in Actor.Skills)
            {
                int maxLevel;
                if (skill.Level > 0 && skill.IsUsable(Actor, out maxLevel))
                {
                    Skill skillClone = (Skill)skill.Clone();
                    skillClone.Level = maxLevel;
                    list.Add(skillClone);
                }
            }
            Choices = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                Choices[i] = list[i].Name + ": " + list[i].MPCost;
            }
            Rectangle.Height = height;
        }
    }
}
