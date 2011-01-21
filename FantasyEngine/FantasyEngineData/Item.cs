using System;
using System.Collections.Generic;
using System.Linq;

namespace FantasyEngineData
{
    public class Item
    {
        public const int NAME_LENGTH = 30;

        private string _Name;

        /// <summary>
        /// Name of the item.
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                if (value.Length >= NAME_LENGTH)
                    value = value.Remove(NAME_LENGTH);
                _Name = value;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
