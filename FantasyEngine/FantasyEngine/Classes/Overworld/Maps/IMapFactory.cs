using Microsoft.Xna.Framework;

namespace FantasyEngine.Classes.Overworld.Maps
{
	public interface IMapFactory
	{
		/// <summary>
		/// Creates a map by loading all the information
		/// </summary>
		Map CreateMap(Game game);
	}
}
