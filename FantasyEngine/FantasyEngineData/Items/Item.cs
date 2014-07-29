using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FantasyEngineData.Battles;
using Effect = FantasyEngineData.Effects.Effect;

namespace FantasyEngineData.Items
{
	public class Item : BaseItem
	{
		public const string TYPE_CONSUMABLE = "Consumable";

		public eTargetType DefaultTarget { get; set; }

		#region Constructors
		public Item()
			: base()
		{

		}

		public Item(string name, string type, Texture2D icon, int price, float weight, string description, string allowableJobs, Effect effect, eTargetType defaultTarget)
			: base(name, type, icon, price, weight, description, allowableJobs, effect)
		{
			DefaultTarget = defaultTarget;
		}
		#endregion Constructors

		#region Abstract Method Region
		public override object Clone()
		{
			Item item = new Item(
				Name,
				Type,
				Icon,
				Price,
				Weight,
				Description,
				AllowableJobs,
				Effect,
				DefaultTarget);
			return item;
		}
		#endregion Abstract Method Region
	}
}
