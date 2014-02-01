using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using FantasyEngine.Classes.Battles;

namespace FantasyEngine.Classes.Overworld.Maps
{
    public class TranquilityPlain
    {
        public static void Battle_OnEnter(EventArgs e, Event eve, GameTime gameTime)
        {
            Map.Encounter mob = Player.GamePlayer.Map.Encounters[0];
            Battle battle = new Battle(eve.Game, "battleback_grass");
            battle._Enemies[0] = new Battler(eve.Game, mob.Monster, mob.Level + 1);
            battle._Enemies[0].Name = battle._Enemies[0].CurrentJob.JobName + "1";
            battle.StartPhase1();
            Scene.ChangeMainScene(battle);
        }
    }
}
