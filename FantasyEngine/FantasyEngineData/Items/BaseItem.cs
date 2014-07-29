using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FantasyEngineData.Entities;
using Effect = FantasyEngineData.Effects.Effect;

namespace FantasyEngineData.Items
{
	public enum Hands { One, Two }
	public enum ArmorLocation { Body, Head, Arms, Feet }

	public abstract class BaseItem
	{
		public const int NAME_LENGTH = 30;

		#region Field Region
		string name;
		string type;
		int price;
		float weight;
		bool equipped = false;
		#endregion

		#region Property Region
		public string Name
		{
			get { return name; }
			set
			{
				if (value.Length >= NAME_LENGTH)
					value = value.Remove(NAME_LENGTH);
				name = value;
			}
		}
		public string Type
		{
			get { return type; }
			set { type = value; }
		}

		[ContentSerializerIgnore()]
		public Texture2D Icon { get; set; }

		[ContentSerializer(ElementName = "Icon")]
		public string IconName { get; set; }

		public int Price
		{
			get { return price; }
			set { price = value; }
		}
		public float Weight
		{
			get { return weight; }
			protected set { weight = value; }
		}

		public string Description { get; set; }

		public string AllowableJobs { get; set; }

		[ContentSerializer(Optional = true)]
		public Effect Effect { get; set; }

		[ContentSerializerIgnore()]
		public bool IsEquiped
		{
			get { return equipped; }
			set { if (!(this is Item)) equipped = value; }
		}
		#endregion

		#region Constructor Region
		public BaseItem()
		{

		}

		public BaseItem(string name, string type, Texture2D icon, int price, float weight, string description, string allowableJobs, Effect effect)
		{
			Name = name;
			Type = type;
			Icon = icon;
			Price = price;
			Weight = weight;
			Description = description;
			AllowableJobs = allowableJobs;
			Effect = effect;
		}
		#endregion

		#region Methods
		public abstract object Clone();

		/// <summary>
		/// Determine if the job is allowed to use this item.
		/// </summary>
		/// <param name="job">Job who wants to use this item</param>
		/// <returns></returns>
		public bool IsAllowed(BaseJob job)
		{
			if (AllowableJobs == "*")
				return true;

			string[] allowableJobs = AllowableJobs.Trim().Split(' ');

			foreach (string allowableJob in allowableJobs)
			{
				if (job.JobAbbreviation == allowableJob)
					return true;
			}

			return false;
		}

		public override string ToString()
		{
			string itemString = "";
			itemString += Name + ", ";
			itemString += Type + ", ";
			itemString += Price.ToString() + ", ";
			itemString += Weight.ToString();
			return itemString;
		}

		public override bool Equals(object obj)
		{
			if (obj is BaseItem)
				return Name == ((BaseItem)obj).Name;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		#endregion Methods

		public static bool operator ==(BaseItem item1, BaseItem item2)
		{
			if (((object)item1) == null)
				return ((object)item2) == null;
			if (((object)item2) == null)
				return ((object)item1) == null;
			return item1.Equals(item2);
		}

		public static bool operator !=(BaseItem item1, BaseItem item2) { return !(item1 == item2); }
	}

	public class BaseItemContentReader : ContentTypeReader<BaseItem>
	{
		protected override BaseItem Read(ContentReader input, BaseItem existingInstance)
		{
			BaseItem baseItem = existingInstance;

			if (baseItem == null)
			{
				//baseItem = new BaseItem();
			}

			baseItem.Name = input.ReadString();
			baseItem.Type = input.ReadString();
			string iconName = input.ReadString();
			baseItem.Price = input.ReadInt32();
			//baseItem.Weight = input.ReadInt32();
			baseItem.Description = input.ReadString();
			baseItem.AllowableJobs = input.ReadString();
			baseItem.Effect = input.ReadObject<Effect>();

			baseItem.Icon = input.ContentManager.Load<Texture2D>(@"Images\Menus\" + iconName);

			return baseItem;
		}
	}
}
