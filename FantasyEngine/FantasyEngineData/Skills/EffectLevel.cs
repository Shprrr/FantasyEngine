using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using FantasyEngineData.Items;

namespace FantasyEngineData.Skills
{
    public class EffectLevel : Effect
    {
        #region Properties
        /// <summary>
        /// Level of the effect.
        /// </summary>
        [ContentSerializerIgnore()]
        public int Level { get; set; }

        /// <summary>
        /// Add this number to the Value of the effect.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public float AddValueByLevel { get; set; }
        /// <summary>
        /// Coefficient to the squared level for the Value of the effect.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public float QuadraticCoefficientValueLevel { get; set; }
        /// <summary>
        /// Add this number to the Multiplier of the effect.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public float AddMultiplierByLevel { get; set; }
        /// <summary>
        /// Coefficient to the squared level for the Multiplier of the effect.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public float QuadraticCoefficientMultiplierLevel { get; set; }
        /// <summary>
        /// Add this number to the HitPourc of the effect.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public float AddHitPourcByLevel { get; set; }
        /// <summary>
        /// Coefficient to the squared level for the HitPourc of the effect.
        /// </summary>
        [ContentSerializer(Optional = true)]
        public float QuadraticCoefficientHitPourcLevel { get; set; }
        #endregion Properties

        public EffectLevel()
            : base()
        {
            Level = 1;
        }

        /// <summary>
        /// Calculates the effect for the specified level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public Effect EffectForLevel(int level)
        {
            Effect effect = new Effect(Type);

            effect.Value = (int)((QuadraticCoefficientValueLevel * Math.Pow(level, 2)) + (AddValueByLevel * level) + Value);
            effect.Multiplier = (float)((QuadraticCoefficientMultiplierLevel * Math.Pow(level, 2)) + (AddMultiplierByLevel * level) + Multiplier);
            effect.HitPourc = (int)((QuadraticCoefficientHitPourcLevel * Math.Pow(level, 2)) + (AddHitPourcByLevel * level) + HitPourc);

            return effect;
        }

        /// <summary>
        /// Calculates the effect on the current level.
        /// </summary>
        /// <returns></returns>
        public Effect EffectForLevel() { return EffectForLevel(Level); }

        public override string ToString()
        {
            return EffectForLevel().ToString();
        }
    }
}
