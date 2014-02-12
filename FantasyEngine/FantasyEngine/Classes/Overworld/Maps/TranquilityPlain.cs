using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using FantasyEngine.Classes.Battles;
using FantasyEngineData.Entities;

namespace FantasyEngine.Classes.Overworld.Maps
{
	public sealed class TranquilityPlainFactory : IMapFactory
	{
		public Map CreateMap(Game game)
		{
			return new TranquilityPlain(game);
		}
	}

	public sealed class TranquilityPlain : Map
	{
		public TranquilityPlain(Game game)
			: base(game, "Tranquility Plain")
		{
			BackgroundMusic = Game.Content.Load<Song>(@"Audios\Musics\Village");
			Encounters.Add(new Encounter(game.Content.Load<Monster>(@"Monsters\Goblin"), 2, 100));
			Encounters.Add(new Encounter(new Encounter.MonsterLevel[] {
					new Encounter.MonsterLevel(Game.Content.Load<Monster>(@"Monsters\Goblin"), 1),
					new Encounter.MonsterLevel(Game.Content.Load<Monster>(@"Monsters\Goblin"), 1)
				}, 100));
			BattleBackName = "battleback_grass";
		}

		public static void Battle_OnEnter(EventArgs e, Event eve, GameTime gameTime)
		{
			Encounter encounter = Player.GamePlayer.Map.Encounters[0];
			Battle battle = new Battle(eve.Game, Player.GamePlayer.Map.BattleBackName);
			battle._Enemies[0] = new Battler(eve.Game, encounter.Monsters[0].Monster, encounter.Monsters[0].Level + 1);
			battle._Enemies[0].Name = battle._Enemies[0].CurrentJob.JobName + "1";
			battle.StartPhase1();
			Scene.ChangeMainScene(battle);
		}
	}
}
