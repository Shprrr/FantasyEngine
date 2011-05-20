using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FantasyEngineData.Battles;

namespace FantasyEngine.Classes.Battles
{
    public class Cursor : DrawableGameComponent
    {
        private int _Index = 0;
        private int _IndexCourant = 0;
        private eTargetType _Target = eTargetType.NONE;

        private Battler[] _Actors = new Battler[Player.MAX_ACTOR];
        private Battler[] _Enemies = new Battler[Battle.MAX_ENEMY];

        public Cursor(Game game, Battler[] Actors, Battler[] Enemies, eTargetType Target, int IndexCourant)
            : base(game)
        {
            _IndexCourant = IndexCourant;
            _Target = Target;

            for (int i = 0; i < Player.MAX_ACTOR; i++)
                _Actors[i] = Actors[i];

            for (int i = 0; i < Battle.MAX_ENEMY; i++)
                _Enemies[i] = Enemies[i];
        }

        private byte frame = 0;
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            frame = (byte)((frame + 1) % 8);

            Color alpha = Color.White * (frame < 4 ? 0.5f : 1);
            switch (_Target)
            {
                case eTargetType.SINGLE_ENEMY:
                    //Dessiner un cursor sur l'enemy sélectionné
                    GameMain.spriteBatch.Draw(GameMain.cursor,
                        new Vector2(_Enemies[_Index].BattlerPosition.X + _Enemies[_Index].BattlerSprite.TileWidth + 8,
                            _Enemies[_Index].BattlerPosition.Y + (_Enemies[_Index].BattlerSprite.TileHeight / 2)),
                        null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
                    break;

                case eTargetType.MULTI_ENEMY:
                    //Dessiner un cursor en alpha sur chacun des ennemis
                    for (int i = 0; i < Battle.MAX_ENEMY; i++)
                        if (_Enemies[i] != null)
                            GameMain.spriteBatch.Draw(GameMain.cursor,
                                new Vector2(_Enemies[i].BattlerPosition.X + _Enemies[i].BattlerSprite.TileWidth + 8,
                                    _Enemies[i].BattlerPosition.Y + (_Enemies[i].BattlerSprite.TileHeight / 2)),
                                null, alpha, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
                    break;

                case eTargetType.SINGLE_PARTY:
                    //Dessiner un cursor sur l'actor sélectionné
                    GameMain.spriteBatch.Draw(GameMain.cursor, new Vector2(_Actors[_Index].BattlerPosition.X - 8,
                        _Actors[_Index].BattlerPosition.Y + (_Actors[_Index].BattlerSprite.TileHeight / 2)), Color.White);
                    break;

                case eTargetType.MULTI_PARTY:
                    //Dessiner un cursor en alpha sur chacun des actors
                    for (int i = 0; i < Player.MAX_ACTOR; i++)
                        if (_Actors[i] != null)
                            GameMain.spriteBatch.Draw(GameMain.cursor, new Vector2(_Actors[i].BattlerPosition.X - 8,
                                _Actors[i].BattlerPosition.Y + (_Actors[i].BattlerSprite.TileHeight / 2)), alpha);
                    break;

                case eTargetType.SELF:
                    //Dessiner un cursor sur l'actor courant
                    if (_IndexCourant < Player.MAX_ACTOR)
                        GameMain.spriteBatch.Draw(GameMain.cursor, new Vector2(_Actors[_IndexCourant].BattlerPosition.X - 8,
                            _Actors[_IndexCourant].BattlerPosition.Y + (_Actors[_IndexCourant].BattlerSprite.TileHeight / 2)), Color.White);
                    else
                        GameMain.spriteBatch.Draw(GameMain.cursor,
                            new Vector2(_Enemies[_IndexCourant].BattlerPosition.X + _Enemies[_IndexCourant].BattlerSprite.TileWidth + 8,
                                _Enemies[_IndexCourant].BattlerPosition.Y + (_Enemies[_IndexCourant].BattlerSprite.TileHeight / 2)),
                            null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
                    break;

                case eTargetType.ALL:
                    //Dessiner un cursor en alpha sur chacun des actors et des ennemis
                    for (int i = 0; i < Player.MAX_ACTOR; i++)
                        if (_Actors[i] != null)
                            GameMain.spriteBatch.Draw(GameMain.cursor, new Vector2(_Actors[i].BattlerPosition.X - 8,
                                _Actors[i].BattlerPosition.Y + (_Actors[i].BattlerSprite.TileHeight / 2)), alpha);

                    for (int i = 0; i < Battle.MAX_ENEMY; i++)
                        if (_Enemies[i] != null)
                            GameMain.spriteBatch.Draw(GameMain.cursor,
                                new Vector2(_Enemies[i].BattlerPosition.X + _Enemies[i].BattlerSprite.TileWidth + 8,
                                    _Enemies[i].BattlerPosition.Y + (_Enemies[i].BattlerSprite.TileHeight / 2)),
                                null, alpha, 0, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
                    break;

                //case eTargetType.NONE:
                default:
                    //Rien dessiner
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Input.UpdateInput(gameTime))
                return;

            if (Input.keyStateHeld.IsKeyDown(Keys.Down))
            {
                //Si on peut descendre
                if (_Target == eTargetType.SINGLE_PARTY)
                {
                    if (_Actors[_Index + 1] != null)
                        _Index++;
                    else
                        _Index = 0;
                    Input.PutDelay(Keys.Down);
                }
                else if (_Target == eTargetType.SINGLE_ENEMY)
                {
                    if (_Enemies[_Index + 1] != null)
                        _Index++;
                    else
                        _Index = 0;
                    Input.PutDelay(Keys.Down);
                }

                return;
            }

            if (Input.keyStateHeld.IsKeyDown(Keys.Up))
            {
                //Si on peut monter
                if (_Target == eTargetType.SINGLE_PARTY)
                {
                    if (_Index != 0)
                        _Index--;
                    else
                        _Index = Player.MAX_ACTOR;
                    Input.PutDelay(Keys.Up);

                    //Valider l'index le plus haut
                    do
                    {
                        _Index--;
                    }
                    while (_Actors[_Index] == null);
                }
                else if (_Target == eTargetType.SINGLE_ENEMY)
                {
                    if (_Index != 0)
                        _Index--;
                    else
                        _Index = Battle.MAX_ENEMY;
                    Input.PutDelay(Keys.Up);

                    //Valider l'index le plus haut
                    do
                    {
                        _Index--;
                    }
                    while (_Enemies[_Index] == null);
                }

                return;
            }

            if (Input.keyStateDown.IsKeyDown(Keys.Left))
            {
                //Changer de target type
                switch (_Target)
                {
                    case eTargetType.MULTI_ENEMY:
                        _Target = eTargetType.ALL;
                        break;

                    case eTargetType.SINGLE_ENEMY:
                        _Target = eTargetType.MULTI_ENEMY;
                        break;

                    case eTargetType.SINGLE_PARTY:
                        _Target = eTargetType.SINGLE_ENEMY;
                        break;

                    case eTargetType.MULTI_PARTY:
                        _Target = eTargetType.SINGLE_PARTY;
                        break;

                    case eTargetType.ALL:
                        _Target = eTargetType.MULTI_PARTY;
                        break;
                }

                return;
            }

            if (Input.keyStateDown.IsKeyDown(Keys.Right))
            {
                //Changer de target type
                switch (_Target)
                {
                    case eTargetType.ALL:
                        _Target = eTargetType.MULTI_ENEMY;
                        break;

                    case eTargetType.MULTI_ENEMY:
                        _Target = eTargetType.SINGLE_ENEMY;
                        break;

                    case eTargetType.SINGLE_ENEMY:
                        _Target = eTargetType.SINGLE_PARTY;
                        break;

                    case eTargetType.SINGLE_PARTY:
                        _Target = eTargetType.MULTI_PARTY;
                        break;

                    case eTargetType.MULTI_PARTY:
                        _Target = eTargetType.ALL;
                        break;
                }

                return;
            }
        }

        public void getTargetBattler(Battler[] mapTargetBattler)
        {
            //Clear old values
            for (int i = 0; i < mapTargetBattler.Length; i++)
            {
                mapTargetBattler[i] = null;
            }

            switch (_Target)
            {
                case eTargetType.SINGLE_ENEMY:
                    mapTargetBattler[Player.MAX_ACTOR + _Index] = _Enemies[_Index];
                    break;

                case eTargetType.MULTI_ENEMY:
                    for (int i = 0; i < Battle.MAX_ENEMY; i++)
                        mapTargetBattler[Player.MAX_ACTOR + i] = _Enemies[i];
                    break;

                case eTargetType.SINGLE_PARTY:
                    mapTargetBattler[_Index] = _Actors[_Index];
                    break;

                case eTargetType.MULTI_PARTY:
                    for (int i = 0; i < Player.MAX_ACTOR; i++)
                        mapTargetBattler[i] = _Actors[i];
                    break;

                case eTargetType.SELF:
                    if (_IndexCourant < Player.MAX_ACTOR)
                        mapTargetBattler[_IndexCourant] = _Actors[_IndexCourant];
                    else
                        mapTargetBattler[_IndexCourant] = _Enemies[_IndexCourant - Player.MAX_ACTOR];
                    break;

                case eTargetType.ALL:
                    for (int i = 0; i < Player.MAX_ACTOR; i++)
                        mapTargetBattler[i] = _Actors[i];

                    for (int i = 0; i < Battle.MAX_ENEMY; i++)
                        mapTargetBattler[Player.MAX_ACTOR + i] = _Enemies[i];
                    break;

                default:
                    break;
            }
        }
    }
}
