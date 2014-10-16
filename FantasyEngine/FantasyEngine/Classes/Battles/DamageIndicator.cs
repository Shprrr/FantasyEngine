using System;
using Microsoft.Xna.Framework;
using FantasyEngineData.Effects;

namespace FantasyEngine.Classes.Battles
{
	/// <summary>
	/// Show and apply damage on a Battler.
	/// </summary>
	public class DamageIndicator : DrawableGameComponent
	{
		private const string MISS = "MISS";

		public Battler Target { get; set; }
		public Damage Damage { get; set; }
		public bool ShowMultiplier { get; set; }
		public TimeSpan AnimationWait { get; private set; }

		public DamageIndicator(Game game, Battler target, Damage damage, bool showMultiplier = true)
			: base(game)
		{
			Target = target;
			Damage = damage;
			ShowMultiplier = showMultiplier;
			AnimationWait = new TimeSpan(0, 0, 0, 1);
		}

		public override void Draw(GameTime gameTime)
		{
			if (!Visible)
				return;

			base.Draw(gameTime);

			if (AnimationWait <= TimeSpan.Zero)
			{
				Visible = false;
				Damage.ApplyDamage(Target);
				return;
			}

			if (Target != null)
			{
				//TODO: Miss si status en double et aucun damage sinon les damages sont affiché, sans changer le status
				int totalDamage = Math.Abs(Damage.Value);
				string damageText = Damage.Multiplier == 0 ? MISS :
					(ShowMultiplier ? Damage.Multiplier + " hit" + (Damage.Multiplier > 1 ? "s" : "") : "") +
					Environment.NewLine + totalDamage;

				Color color = Damage.Value > 0 ? new Color(0xFF, 0x80, 0x80, 0xFF) : new Color(0x80, 0xFF, 0x80, 0xFF);
				if (Damage.Multiplier == 0) color = new Color(0x80, 0x80, 0x80, 0xFF);

				GameMain.spriteBatchGUI.DrawString(GameMain.font, damageText,
					new Vector2(Target.BattlerPosition.X, Target.BattlerPosition.Y - 12),
					color);
			}

			AnimationWait -= gameTime.ElapsedGameTime;
		}
	}
}
