using System;
using System.Collections.Generic;
using System.Linq;

namespace FantasyEngineData
{
    public class Monster : BaseJob, ICloneable
    {
        public float StrenghtPlus { get; set; }

        // Dans Drop, il y a des règles plus souples pour donner
        // le Gold et les Treasures en fonction de plusieurs paramètres du
        // Monster courant comme le Level.
        public Drop Drop { get; set; }

        public Monster()
        {
            Drop = new Drop();
        }

        public Monster(Job Job)
        {
            // Calculer le base exp.

            Drop = new Drop();
        }

        #region ICloneable Membres
        public object Clone()
        {
            return MemberwiseClone();
        }
        #endregion
    }
}
