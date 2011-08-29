using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FantasyEngineData;
using FantasyEngineData.Battles;
using FantasyEngineData.Items;

namespace FantasyEngine.Classes.Battles
{
    public struct BattleAction
    {
        public enum eKind
        {
            WAIT,
            ATTACK,
            MAGIC,
            ITEM,
            GUARD
        }

        public eKind Kind;
        public Cursor Target;
        public int skillId;
        public BaseItem Item;

        public BattleAction(Game game, eKind kind = eKind.WAIT, int skillId = -1, BaseItem item = null)
        {
            this.Kind = kind;
            this.Target = null;
            this.skillId = skillId;
            this.Item = item;
        }
    }

    public struct CTBTurn : IComparable<CTBTurn>
    {
        public int counter;
        public int rank;
        public int tickSpeed;
        public Battler battler;

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(object obj)
        {
            if (obj is CTBTurn)
            {
                CTBTurn other = (CTBTurn)obj;
                //    return other.pActor == null || (this.pActor != null && this.counter <= other.counter);
                if (this.battler == other.battler)
                    return this.counter == other.counter;

                return false;
            }

            return base.Equals(obj);
        }

        public override string ToString()
        {
            return battler + " [C:" + counter + "]";
        }

        #region IComparable<CTBTurn> Membres

        public int CompareTo(CTBTurn other)
        {
            //return other.pActor == null || (this.pActor != null && this.counter <= other.counter);
            if (this.battler == other.battler)
                return 0;

            if (other.battler == null)
                return -1;

            if (this.battler == null)
                return 1;

            return this.counter.CompareTo(other.counter);
        }

        #endregion
    }

    public class Battle : Scene
    {
        public const int MAX_CTB = 16;
        public const int MAX_ENEMY = Player.MAX_ACTOR;

        private readonly string[] playerCommands = { "Attack", "Magic", "Item", "Guard", "Run" };
        private readonly string[] partyCommand = { "Fight", "Escape" };
        private const string MISS = "MISS";

        #region Fields
        private Texture2D _BattleBack;
        private Song _BattleMusic;

        private Command _PlayerCommand;
        private Command _PartyCommand;
        private Window _HelpWindow;
        private Window _StatusWindow;
        private Window _MessageWindow;
        private Window _CTBWindow;
        private Window _ResultWindow;
        private ItemSelection _ItemSelection;

        private int _CTBWindowScrollY;

        private int _BattleTurn = 0;
        private int _Phase;
        private int _PhaseStep;
        private int _AnimationWait = 0;

        private List<CTBTurn> _OrderBattle;
        private Cursor _Target = null;
        private Battler[] _TargetBattler = new Battler[Player.MAX_ACTOR + MAX_ENEMY];

        private int _ActiveBattlerIndex;
        private BattleAction _CurrentAction;

        private bool _CanEscape = true;
        private bool _CanLose = true;
        private int _Exp = 0;
        private int _Gold = 0;
        private List<BaseItem> _Treasure = new List<BaseItem>();
        #endregion Fields

        /// <summary>
        /// Get the Battler who is taking an action.
        /// </summary>
        /// <returns></returns>
        private Battler getActiveBattler()
        {
            return _ActiveBattlerIndex < Player.MAX_ACTOR ? _Actors[_ActiveBattlerIndex] : _Enemies[_ActiveBattlerIndex - Player.MAX_ACTOR];
        }
        private void setActiveBattler()
        {
            for (int i = 0; i < Player.MAX_ACTOR; i++)
            {
                if (_Actors[i] == _OrderBattle[0].battler)
                    _ActiveBattlerIndex = i;
            }

            for (int i = 0; i < MAX_ENEMY; i++)
            {
                if (_Enemies[i] == _OrderBattle[0].battler)
                    _ActiveBattlerIndex = Player.MAX_ACTOR + i;
            }
        }

        /// <summary>
        /// Determine battle Win/Loss results.
        /// </summary>
        /// <returns></returns>
        private bool Judge()
        {
            foreach (Battler actor in _Actors)
            {
                //TODO: Pas sur, mais je ne pense pas que ca marche pour plus qu'un actor.
                if (actor != null && actor.IsDead)
                {
                    BattleEnd(_CanLose ? 1 : 2);
                    return true;
                }
            }

            foreach (Battler enemy in _Enemies)
            {
                if (enemy != null && !enemy.IsDead)
                {
                    return false;
                }
            }

            // Start after battle phase (win)
            StartPhase5();
            return true;
        }

        /// <summary>
        /// Battle Ends.
        /// </summary>
        /// <param name="result">Results (0:Win 1:Lose 2:Escape)</param>
        private void BattleEnd(int result)
        {
            switch (result)
            {
                case 0: // Win
                    {
                        int nbActor = 0;
                        foreach (Battler actor in _Actors)
                        {
                            if (actor != null && !actor.IsDead)
                                nbActor++;
                        }

                        foreach (Battler actor in _Actors)
                        {
                            if (actor != null && !actor.IsDead)
                            {
                                int oldLevel = actor.Level;
                                actor.Exp += _Exp / nbActor;
#if DEBUG
                                actor.Exp += 40;
#endif
                                if (actor.Level != oldLevel)
                                    Scene.AddSubScene(new FantasyEngine.Classes.Menus.LevelUpScene(Game, actor));
                                Player.GamePlayer.Inventory.Gold += _Gold / nbActor;
                                Player.GamePlayer.Inventory.AddRange(_Treasure);
                            }
                        }
                    }
                    break;

                case 1: // Lose
                    // Gameover screen
                    // Wakeup in the inn.
                    break;
            }

            //Remove battle states.
            foreach (Battler actor in _Actors)
            {
            }

            /*
                # Call battle callback
                if $game_temp.battle_proc != nil
                  $game_temp.battle_proc.call(result)
                  $game_temp.battle_proc = nil
                end
            */

            _Phase = 6;
        }


        /// <summary>
        /// Start Pre-battle phase.
        /// </summary>
        public void StartPhase1()
        {
            _Phase = 1;

            InitCTBCounters();

            SetBattlerPositions();

            // Start party command phase
            StartPhase3();
        }

        /// <summary>
        /// Initialise CTB counters for the first time.
        /// </summary>
        private void InitCTBCounters()
        {
            //1-Calcul les ICV, ajoute le plus petit et garde les restes
            List<CTBTurn> tempCTB = new List<CTBTurn>(Player.MAX_ACTOR + MAX_ENEMY);

            //Get ICVs
            for (int i = 0; i < Player.MAX_ACTOR; i++)
            {
                if (_Actors[i] == null)
                    continue;

                _Actors[i].CalculateICV();
                CTBTurn turn = new CTBTurn();
                turn.battler = _Actors[i];
                turn.rank = 3;
                turn.counter = turn.battler.Counter;
                turn.tickSpeed = turn.battler.getTickSpeed();
                tempCTB.Add(turn);
            }

            for (int i = 0; i < MAX_ENEMY; i++)
            {
                if (_Enemies[i] == null)
                    continue;

                _Enemies[i].CalculateICV();
                CTBTurn turn = new CTBTurn();
                turn.battler = _Enemies[i];
                turn.rank = 3;
                turn.counter = turn.battler.Counter;
                turn.tickSpeed = turn.battler.getTickSpeed();
                tempCTB.Add(turn);
            }

            //Sort ICVs
            tempCTB.Sort();

            //Keep lowest
            if (tempCTB[0].battler == null)
                return;
            _OrderBattle.Add(tempCTB[0]);

            //2-Calcul le NCV de celui ajouté, ajoute le plus petit et garde les restes
            for (int i = 1; i < MAX_CTB; i++)
            {
                //Get Next CV
                CTBTurn turn = tempCTB[0];
                turn.counter += turn.battler.getCounterValue(turn.rank);
                tempCTB[0] = turn;

                //Sort CVs
                tempCTB.Sort();

                //Keep lowest
                _OrderBattle.Add(tempCTB[0]);
                tempCTB[0].battler.Counter = tempCTB[0].counter;
            }

            setActiveBattler();
        }

        /// <summary>
        /// Take all battler and set their position.
        /// </summary>
        private void SetBattlerPositions()
        {
            for (int i = 0; i < Player.MAX_ACTOR; i++)
                if (_Actors[i] != null)
                    _Actors[i].BattlerPosition = new Vector2(320, 160 + 40 * i);

            for (int i = 0; i < MAX_ENEMY; i++)
                if (_Enemies[i] != null)
                    _Enemies[i].BattlerPosition = new Vector2(100, 160 + 40 * i);
        }

        /// <summary>
        /// Start Party Command phase.
        /// </summary>
        private void StartPhase2()
        {
            _Phase = 2;

            //Set actor to non-selecting

            _PartyCommand.Enabled = true;
            _PartyCommand.Visible = true;

            _PlayerCommand.Enabled = false;
            _PlayerCommand.Visible = false;
        }

        /// <summary>
        /// Start Actor command phase.
        /// </summary>
        private void StartPhase3()
        {
            _Phase = 3;

            // Determine win/loss situation
            if (Judge())
            {
                // If won or lost: end method
                return;
            }

            for (int i = 0; i < _TargetBattler.Length; i++)
            {
                _TargetBattler[i] = null;
            }

            _BattleTurn++;

            foreach (Battler actor in _Actors)
            {
                if (getActiveBattler() == actor)
                {
                    SetupCommandWindow();
                    return;
                }
            }

            foreach (Battler enemy in _Enemies)
            {
                if (getActiveBattler() == enemy)
                {
                    _CurrentAction = enemy.AIChooseAction(Game, _Enemies, _Actors);
                    StartPhase4();
                    return;
                }
            }
        }

        private void SetupCommandWindow()
        {
            // Disable party command window
            _PartyCommand.Enabled = false;
            _PartyCommand.Visible = false;
            // Enable actor command window
            _PlayerCommand.Enabled = true;
            _PlayerCommand.Visible = true;
            // Set index to 0
            // Note: They may want to keep position of the last action.
            _PlayerCommand.CursorPosition = 0;
        }

        /// <summary>
        /// Start the select of the target with a default target.
        /// </summary>
        /// <param name="defaultValue">First position of the target cursor</param>
        private void StartTargetSelection(eTargetType defaultValue)
        {
            //Make cursor
            _Target = new Cursor(Game, _Actors, _Enemies, defaultValue, _ActiveBattlerIndex);

            _PlayerCommand.Enabled = false;
        }

        private void EndTargetSelection()
        {
            _Target = null;
        }

        private void StartItemSelection()
        {
            _ItemSelection.Enabled = true;
            _ItemSelection.Visible = true;

            _PlayerCommand.Enabled = false;
        }

        private void EndItemSelection()
        {
            _ItemSelection.Enabled = false;
            _ItemSelection.Visible = false;

            if (_CurrentAction.Item is Item)
                StartTargetSelection(((Item)_CurrentAction.Item).DefaultTarget);
        }

        /// <summary>
        /// Start Main phase.
        /// </summary>
        private void StartPhase4()
        {
            _Phase = 4;

            _PlayerCommand.Enabled = false;
            _PlayerCommand.Visible = false;

            _PhaseStep = 1;
            Phase4Step1();
        }
        /// <summary>
        /// Action preparation.
        /// </summary>
        private void Phase4Step1()
        {
            //Hide Help window

            //Determine win/loss
            if (Judge())
            {
                // If won or lost: end method
                Scene.ChangeMainScene(new Overworld.Overworld(Game));
                return;
            }

            //Init animations

            //Turn damage

            //Natural removal of states

            _PhaseStep = 2;
            Phase4Step2();
        }
        /// <summary>
        /// Start action.
        /// </summary>
        private void Phase4Step2()
        {
            switch (_CurrentAction.Kind)
            {
                case BattleAction.eKind.ATTACK:
                    //Set animation id

                    _CurrentAction.Target.getTargetBattler(_TargetBattler);
                    for (int i = 0; i < Player.MAX_ACTOR + MAX_ENEMY; i++)
                        if (_TargetBattler[i] != null)
                            _TargetBattler[i].Attacked(getActiveBattler());
                    break;

                case BattleAction.eKind.MAGIC:
                    break;

                case BattleAction.eKind.ITEM:
                    //Set animation id

                    int nbTarget = 0;
                    _CurrentAction.Target.getTargetBattler(_TargetBattler);
                    for (int i = 0; i < Player.MAX_ACTOR + MAX_ENEMY; i++)
                        if (_TargetBattler[i] != null)
                            nbTarget++;

                    for (int i = 0; i < Player.MAX_ACTOR + MAX_ENEMY; i++)
                        if (_TargetBattler[i] != null)
                            _TargetBattler[i].Used(getActiveBattler(), _CurrentAction.Item, nbTarget);

                    if (getActiveBattler().IsActor)
                    {
                        Player.GamePlayer.Inventory.Drop(_CurrentAction.Item);
                        _ItemSelection.RefreshChoices();
                    }
                    break;

                case BattleAction.eKind.GUARD:
                    break;

                case BattleAction.eKind.WAIT:
                    break;
            }

            _PhaseStep = 3;
            Phase4Step3();
        }
        /// <summary>
        /// Animation for action performer.
        /// </summary>
        private void Phase4Step3()
        {
            //Set animation of attacker
            _AnimationWait = 30;
        }
        /// <summary>
        /// Animation for target.
        /// </summary>
        private void Phase4Step4()
        {
            //Set animation of target

            //Animation has at least 8 frames, regardless of its length
            _AnimationWait = 30;
        }
        /// <summary>
        /// Damage display.
        /// </summary>
        private void Phase4Step5()
        {
            //Display damage

            _AnimationWait = 30;
        }

        /// <summary>
        /// Start After Battle phase.
        /// </summary>
        private void StartPhase5()
        {
            _Phase = 5;
            _PhaseStep = 1;

            // Play the victory song.
            Song victory = Game.Content.Load<Song>(@"Audios\Musics\Victory");
            MediaPlayer.Stop();
            MediaPlayer.Play(victory);
            MediaPlayer.IsRepeating = false;

            // Get Exp, Gold and Items from dead enemies.
            foreach (Battler enemy in _Enemies)
            {
                if (enemy == null)
                    continue;

                _Exp += enemy.ExpToGive();
                _Gold += enemy.GoldToGive;
                _Treasure.AddRange(enemy.Treasure);
            }

            // Wait 100 frames.
            _AnimationWait = 30;

            // Show the result.
        }

        /// <summary>
        /// Try to escape.
        /// </summary>
        private void Escape()
        {
            //From FF3
            //(Run Away) : DD=0 DM=0 %Chance to Run = 100 - Hit%
            //(Hit%) : (Weapon Hit%) + (Agility/4) + (JLevel/4)
            //(Run Away for monster ) : IF((Lowest character level) - (Enemy level) > 15) THEN %Chance to Run = 100 - Monster Hit%

            //Calculate enemy agility average
            int enemies_agi = 0;
            int enemies_number = 0;
            for (int i = 0; i < MAX_ENEMY; i++)
            {
                Character pEnemy = _Enemies[i];
                if (pEnemy == null)
                    continue;

                if (!pEnemy.IsDead)
                {
                    enemies_agi += pEnemy.Agility;
                    enemies_number += 1;
                }
            }
            if (enemies_number > 0)
            {
                enemies_agi /= enemies_number;
            }

            //Calculate actor agility average
            int actors_agi = 0;
            int actors_number = 0;
            for (int i = 0; i < Player.MAX_ACTOR; i++)
            {
                Character pActor = _Actors[i];
                if (pActor == null)
                    continue;

                if (!pActor.IsDead)
                {
                    actors_agi += pActor.Agility;
                    actors_number += 1;
                }
            }
            if (actors_number > 0)
            {
                actors_agi /= actors_number;
            }

            //Determine if escape is successful
            //rand(100) < 50 * actors_agi / enemies_agi
            bool success = new Random().Next(0, 100) < 50 * actors_agi / enemies_agi;

            if (success)
            {
                BattleEnd(2);
            }
            else
            {
                //Next turn
                StartPhase4();
            }
        }

        private void CalculateCTB()
        {
            // Remove active counter on all other counters.
            int counterLastTurn = _OrderBattle[0].counter;
            for (int i = 0; i < _OrderBattle.Count; i++)
            {
                CTBTurn turn = _OrderBattle[i];
                turn.counter -= counterLastTurn;
                _OrderBattle[i] = turn;
            }

            // Calculate the last CTBTurn to replace the one that will go out.

            //1-Calcul les NCV et ajoute le plus petit
            List<CTBTurn> tempCTB = new List<CTBTurn>(Player.MAX_ACTOR + MAX_ENEMY);

            //Get NCVs
            for (int i = 0; i < Player.MAX_ACTOR; i++)
            {
                if (_Actors[i] == null)
                    continue;

                _Actors[i].Counter -= counterLastTurn;
                CTBTurn turn = new CTBTurn();
                turn.battler = _Actors[i];
                turn.rank = 3;
                turn.counter = turn.battler.Counter + turn.battler.getCounterValue(turn.rank);
                turn.tickSpeed = turn.battler.getTickSpeed();
                tempCTB.Add(turn);
            }

            for (int i = 0; i < MAX_ENEMY; i++)
            {
                if (_Enemies[i] == null)
                    continue;

                _Enemies[i].Counter -= counterLastTurn;
                CTBTurn turn = new CTBTurn();
                turn.battler = _Enemies[i];
                turn.rank = 3;
                turn.counter = turn.battler.Counter + turn.battler.getCounterValue(turn.rank);
                turn.tickSpeed = turn.battler.getTickSpeed();
                tempCTB.Add(turn);
            }

            //Sort NCVs
            tempCTB.Sort();

            //Keep lowest
            _OrderBattle.Add(tempCTB[0]);
            tempCTB[0].battler.Counter = tempCTB[0].counter;

            _OrderBattle.RemoveAt(0);
            setActiveBattler();
        }

        public Battler[] _Actors = new Battler[Player.MAX_ACTOR];
        public Battler[] _Enemies = new Battler[MAX_ENEMY];

        public Battle(Game game, string battleBackName)
            : base(game)
        {
            //Init textures
            _BattleBack = Game.Content.Load<Texture2D>(@"Images\Battlebacks\" + battleBackName);

            _BattleMusic = Game.Content.Load<Song>(@"Audios\Musics\Battle");
            MediaPlayer.Stop();
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_BattleMusic);

            //Prepare battlers
            for (int i = 0; i < Player.MAX_ACTOR; i++)
            {
                if (Player.GamePlayer.Actors[i] != null)
                    _Actors[i] = new Battler(Game, Player.GamePlayer.Actors[i]);
            }

            _OrderBattle = new List<CTBTurn>(MAX_CTB);

            //Making windows
            _PlayerCommand = new Command(Game, 160, playerCommands);
            _PlayerCommand.ChangeOffset(640 - 160, 320);
            _PlayerCommand.Enabled = false;
            _PlayerCommand.Visible = false;

            _PartyCommand = new Command(Game, 640, partyCommand);
            _PartyCommand.Enabled = false;
            _PartyCommand.Visible = false;

            _HelpWindow = new Window(Game, 0, 0, 640, 48);
            _HelpWindow.Visible = false;

            _StatusWindow = new Window(Game, 0, 320, 640, 160);
            _MessageWindow = new Window(Game, 0, 320, 480, 160);
            _MessageWindow.Visible = false;

            _CTBWindow = new Window(Game, 640 - 160, _HelpWindow.Rectangle.Bottom,
                160, 480 - _HelpWindow.Rectangle.Bottom - 160);
            _CTBWindowScrollY = 0;

            _ItemSelection = new ItemSelection(Game, 480, 160);
            _ItemSelection.ChangeOffset(80, 320);
            _ItemSelection.Enabled = false;
            _ItemSelection.Visible = false;

            _ResultWindow = new Window(Game, 0, 0, 640, 480);
            _ResultWindow.Visible = false;

            GameMain.cameraMatrix = Matrix.Identity;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.Black);

            // Background
            spriteBatch.Draw(_BattleBack, new Vector2(0, _HelpWindow.Rectangle.Bottom), Color.White);
            spriteBatch.Draw(_BattleBack, new Vector2(_BattleBack.Width, _HelpWindow.Rectangle.Bottom), Color.White);
            spriteBatch.Draw(_BattleBack, new Vector2(_BattleBack.Width * 2, _HelpWindow.Rectangle.Bottom), Color.White);

            for (int i = 0; i < Player.MAX_ACTOR; i++)
            {
                if (_Actors[i] == null || _Actors[i].IsDead)
                    continue;

                Color color = _Actors[i] == _TargetBattler[i] ? new Color(0xFF, 0, 0, 0xFF) : Color.White;
                if (_Actors[i].BattlerSprite is Tileset)
                    spriteBatch.Draw(_Actors[i].BattlerSprite.texture, _Actors[i].BattlerPosition, _Actors[i].BattlerSprite.GetSourceRectangle(0), color);
                else
                    spriteBatch.Draw(_Actors[i].BattlerSprite.texture, _Actors[i].BattlerPosition, color);
            }

            for (int i = 0; i < MAX_ENEMY; i++)
            {
                if (_Enemies[i] == null || _Enemies[i].IsDead)
                    continue;

                Color color = _Enemies[i] == _TargetBattler[Player.MAX_ACTOR + i] ? new Color(0xFF, 0, 0, 0xFF) : Color.White;
                if (_Enemies[i].BattlerSprite is Tileset)
                    spriteBatch.Draw(_Enemies[i].BattlerSprite.texture, _Enemies[i].BattlerPosition, _Enemies[i].BattlerSprite.GetSourceRectangle(0), color);
                else
                    spriteBatch.Draw(_Enemies[i].BattlerSprite.texture, _Enemies[i].BattlerPosition, color);
            }

            if (_Target != null)
                _Target.Draw(gameTime);

            _PartyCommand.Draw(gameTime);
            _HelpWindow.Draw(gameTime);

            DrawStatusWindow(gameTime);

            _MessageWindow.Draw(gameTime);
            _PlayerCommand.Draw(gameTime);

            DrawCTBWindow(gameTime);

            _ItemSelection.Draw(gameTime);

            if (_Phase == 4 && _PhaseStep == 3)
            {
                switch (_CurrentAction.Kind)
                {
                    case BattleAction.eKind.ATTACK:
                        //spriteBatch.DrawString(GameMain.font, "Sword swing !", new Vector2(0, 200), Color.White);
                        if (getActiveBattler().RightHand != null)
                            spriteBatch.DrawString(GameMain.font,
                                getActiveBattler().RightHand.Name + " swing !", new Vector2(0, 200), Color.White);

                        if (getActiveBattler().LeftHand != null)
                            spriteBatch.DrawString(GameMain.font,
                                getActiveBattler().LeftHand.Name + " swing !", new Vector2(0, 220), Color.White);

                        if (getActiveBattler().RightHand == null && getActiveBattler().LeftHand == null)
                            spriteBatch.DrawString(GameMain.font, "Barehand swing !", new Vector2(0, 200), Color.White);
                        break;

                    case BattleAction.eKind.MAGIC:
                        break;

                    case BattleAction.eKind.ITEM:
                        spriteBatch.DrawString(GameMain.font, _CurrentAction.Item.Name + " is used !", new Vector2(0, 200), Color.White);
                        break;

                    case BattleAction.eKind.GUARD:
                        break;

                    case BattleAction.eKind.WAIT:
                        break;
                }

                _AnimationWait--;
                if (_AnimationWait < 0)
                {
                    _PhaseStep = 4;
                    Phase4Step4();
                }
            }

            if (_Phase == 4 && _PhaseStep == 4)
            {
                switch (_CurrentAction.Kind)
                {
                    case BattleAction.eKind.ATTACK:
                        //spriteBatch.DrawString(GameMain.font, "Sword hitted !", new Vector2(0, 220), Color.White);
                        if (getActiveBattler().RightHand != null)
                            spriteBatch.DrawString(GameMain.font,
                                getActiveBattler().RightHand.Name + " hitted !", new Vector2(0, 200), Color.White);

                        if (getActiveBattler().LeftHand != null)
                            spriteBatch.DrawString(GameMain.font,
                                getActiveBattler().LeftHand.Name + " hitted !", new Vector2(0, 220), Color.White);

                        if (getActiveBattler().RightHand == null && getActiveBattler().LeftHand == null)
                            spriteBatch.DrawString(GameMain.font, "Barehand hitted !", new Vector2(0, 200), Color.White);
                        break;

                    case BattleAction.eKind.MAGIC:
                        break;

                    case BattleAction.eKind.ITEM:
                        spriteBatch.DrawString(GameMain.font, _CurrentAction.Item.Name + " hitted !", new Vector2(0, 200), Color.White);
                        break;

                    case BattleAction.eKind.GUARD:
                        break;

                    case BattleAction.eKind.WAIT:
                        break;
                }

                _AnimationWait--;
                if (_AnimationWait < 0)
                {
                    _PhaseStep = 5;
                    Phase4Step5();
                }
            }

            if (_Phase == 4 && _PhaseStep == 5)
            {
                switch (_CurrentAction.Kind)
                {
                    case BattleAction.eKind.ATTACK:
                    case BattleAction.eKind.MAGIC:
                    case BattleAction.eKind.ITEM:
                        for (int i = 0; i < Player.MAX_ACTOR + MAX_ENEMY; i++)
                            if (_TargetBattler[i] != null)
                            {
                                int totalMultiplier = _TargetBattler[i].multiplierRH + _TargetBattler[i].multiplierLH;
                                int totalDamage = _TargetBattler[i].damageRH + _TargetBattler[i].damageLH;
                                string damage = totalMultiplier == 0 ? MISS :
                                    (_CurrentAction.Kind != BattleAction.eKind.ITEM ?
                                        totalMultiplier + " hit" + (totalMultiplier > 1 ? "s" : "") : "") +
                                    Environment.NewLine + totalDamage;
                                spriteBatch.DrawString(GameMain.font, damage,
                                    new Vector2(_TargetBattler[i].BattlerPosition.X,
                                        _TargetBattler[i].BattlerPosition.Y - 12),
                                    new Color(0xFF, 0x80, 0x80, 0xFF));
                            }
                        break;

                    case BattleAction.eKind.GUARD:
                        break;

                    case BattleAction.eKind.WAIT:
                        break;
                }

                _AnimationWait--;
                if (_AnimationWait < 0)
                {
                    for (int i = 0; i < Player.MAX_ACTOR + MAX_ENEMY; i++)
                        if (_TargetBattler[i] != null)
                        {
                            _TargetBattler[i].GiveDamage();
                        }

                    // Change the active battler for the next one.
                    CalculateCTB();

                    StartPhase3();
                }
            } // if (_Phase == 4 && _Phase4Step == 5)

            if (_Phase == 5)
            {
                if (_PhaseStep == 1)
                {
                    _AnimationWait--;
                    if (_AnimationWait < 0)
                    {
                        _ResultWindow.Visible = true;
                        _PhaseStep = 2;
                    }
                }

                if (_PhaseStep == 2)
                {
                    DrawResultWindow(gameTime);
                }
            }
        }

        private void DrawStatusWindow(GameTime gameTime)
        {
            _StatusWindow.Draw(gameTime);

            GameMain.Scissor(_StatusWindow.InsideBound);

            int x, y, right, height;

            Texture2D pixel = new Texture2D(Game.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[1] { new Color(255, 255, 255) });

            //// Test Statut 2
            //x = _StatusWindow.Rectangle.Left + (_StatusWindow.Rectangle.Width / 4);
            //y = _StatusWindow.Rectangle.Top;
            //right = x + (_StatusWindow.Rectangle.Width / 4);
            //height = _StatusWindow.Rectangle.Height;

            //Rectangle pos = new Rectangle(x, y, Window.Tileset.TileWidth, height);
            //spriteBatch.Draw(Window.Tileset.texture, pos, Window.Tileset.GetSourceRectangle(3), Color.White);

            //spriteBatch.DrawString(GameMain.font, "ABCDEFGHIJ", new Vector2(x + 12, y + 8), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            //spriteBatch.DrawString(GameMain.font, "L100", new Vector2(x + 84, y + 20), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            //spriteBatch.DrawString(GameMain.font, "HP:", new Vector2(x + 12, y + 36), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            //spriteBatch.Draw(pixel, new Rectangle(x + 60, y + 38, 60, 8), new Color(0, 255, 0));
            //spriteBatch.DrawString(GameMain.font, "9999/9999", new Vector2(x + 24, y + 48), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            //spriteBatch.DrawString(GameMain.font, "MP:", new Vector2(x + 12, y + 64), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            //spriteBatch.Draw(pixel, new Rectangle(x + 60, y + 66, 60, 8), new Color(0, 255, 0));
            //spriteBatch.DrawString(GameMain.font, "9999/9999", new Vector2(x + 24, y + 76), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            ////spriteBatch.DrawString(GameMain.font, "Statut:", new Vector2(x +  12, y + 104), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            //spriteBatch.DrawString(GameMain.font, "NormalXY", new Vector2(x + 12, y + 96), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);

            //pos = new Rectangle(right - Window.Tileset.TileWidth, y, Window.Tileset.TileWidth, height);
            //spriteBatch.Draw(Window.Tileset.texture, pos, Window.Tileset.GetSourceRectangle(5), Color.White); //Bord Droite


            x = _StatusWindow.Rectangle.Left;
            y = _StatusWindow.Rectangle.Top;
            right = x + (_StatusWindow.Rectangle.Width / 4);
            height = _StatusWindow.Rectangle.Height;
            for (int i = 0; i < Player.MAX_ACTOR; i++)
            {
                if (i != 0)
                {
                    Rectangle pos = new Rectangle(x, y, Window.Tileset.TileWidth, height);
                    spriteBatch.Draw(Window.Tileset.texture, pos, Window.Tileset.GetSourceRectangle(3), Color.White); //Bord Gauche
                }

                if (_Actors[i] != null)
                {
                    spriteBatch.DrawString(GameMain.font,
                        _Actors[i].Name,
                        new Vector2(x + 12, y + 8), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
                    spriteBatch.DrawString(GameMain.font,
                        _Actors[i].Level < 100 ? "Lv" + _Actors[i].Level : "L" + _Actors[i].Level,
                        new Vector2(x + 84, y + 20), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);

                    spriteBatch.DrawString(GameMain.font,
                        "HP:",
                        new Vector2(x + 12, y + 36), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
                    //TODO: Changer la grandeur d'apres le ratio
                    float ratio = 1;
                    spriteBatch.Draw(pixel, new Rectangle(x + 60, y + 38, (int)(60 * ratio), 8),
                        new Color(255 - 255 * ratio, 255 * ratio, 0));
                    spriteBatch.DrawString(GameMain.font,
                        _Actors[i].Hp + "/" + _Actors[i].MaxHp,
                        new Vector2(x + 24, y + 48), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);

                    spriteBatch.DrawString(GameMain.font,
                        "MP:",
                        new Vector2(x + 12, y + 64), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
                    //TODO: Changer la grandeur d'apres le ratio
                    ratio = 1;
                    spriteBatch.Draw(pixel, new Rectangle(x + 60, y + 66, (int)(60 * ratio), 8),
                        new Color(255 - 255 * ratio, 255 * ratio, 0));
                    spriteBatch.DrawString(GameMain.font,
                        _Actors[i].Mp + "/" + _Actors[i].MaxMp,
                        new Vector2(x + 24, y + 76), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);

                    spriteBatch.DrawString(GameMain.font,
                        _Actors[i].Statut.ToString(),
                        new Vector2(x + 12, y + 96), Color.White, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
                }

                if (i != Player.MAX_ACTOR - 1)
                {
                    Rectangle pos = new Rectangle(right - Window.Tileset.TileWidth, y, Window.Tileset.TileWidth, height);
                    spriteBatch.Draw(Window.Tileset.texture, pos, Window.Tileset.GetSourceRectangle(5), Color.White); //Bord Droite
                }

                x += _StatusWindow.Rectangle.Width / 4;
                right = x + (_StatusWindow.Rectangle.Width / 4);
            }

            GameMain.ScissorReset();
        }

        private void DrawCTBWindow(GameTime gameTime)
        {
            //TODO: Ajouter les flèches pour indiquer le scroll.
            _CTBWindow.Draw(gameTime);

            GameMain.Scissor(_CTBWindow.InsideBound);

            for (int i = 0; i < MAX_CTB; i++)
            {
                /*GRRLIB_Rectangle(640 - 160 + 8, mpHelpWindow.Rectangle.Bottom + 8 + 32 * i, 
                    160 - 16, 32, clrNormal, false);*/

                //TODO: Dessiner le rectangle counter
                spriteBatch.DrawString(GameMain.font, "C:" + _OrderBattle[i].counter,
                    new Vector2(_CTBWindow.Rectangle.Left + 8,
                        _CTBWindow.Rectangle.Top + 8 + _CTBWindowScrollY + 32 * i),
                    Color.White);

                //TODO: Dessiner la face
                spriteBatch.DrawString(GameMain.font, _OrderBattle[i].battler.Name,
                    new Vector2(_CTBWindow.Rectangle.Left + 8 + 6 * 16,
                        _CTBWindow.Rectangle.Top + 8 + _CTBWindowScrollY + 32 * i),
                    Color.White);
            }
            GameMain.ScissorReset();
        }

        private void DrawResultWindow(GameTime gameTime)
        {
            _ResultWindow.Draw(gameTime);

            spriteBatch.DrawString(GameMain.font, "Exp. gained : " + _Exp, new Vector2(16, 16), Color.White);
            spriteBatch.DrawString(GameMain.font, "Gold gained : " + _Gold, new Vector2(16, 16 + GameMain.font.LineSpacing), Color.White);

            Rectangle pos = new Rectangle(_ResultWindow.Rectangle.X + Window.Tileset.TileWidth, _ResultWindow.Rectangle.Y + 24 + (GameMain.font.LineSpacing * 2),
                _ResultWindow.Rectangle.Width - (Window.Tileset.TileWidth * 2), Window.Tileset.TileHeight);
            spriteBatch.Draw(Window.Tileset.texture, pos, Window.Tileset.GetSourceRectangle(1), Color.White); //Bord Haut

            int x = 16;
            int y = 32 + (GameMain.font.LineSpacing * 2) + Window.Tileset.TileHeight;
            foreach (BaseItem item in _Treasure)
            {
                spriteBatch.DrawString(GameMain.font, item.Name, new Vector2(x, y), Color.White);
                y += GameMain.font.LineSpacing;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _PlayerCommand.Update(gameTime);
            //return;

            _PartyCommand.Update(gameTime);
            //return;

            if (_Target != null)
                _Target.Update(gameTime);
            //return;

            _ItemSelection.Update(gameTime);

            if (_Phase == 6 && CurrentScene == this) //WaitEnd
            {
                // Return to map
                MediaPlayer.Stop();
                Scene.ChangeMainScene(new Overworld.Overworld(Game));
                return;
            }

            if (Input.keyStateDown.IsKeyDown(Keys.Enter))
            {
                switch (_Phase)
                {
                    case 2: //Party Command
                        switch (_PartyCommand.CursorPosition)
                        {
                            case 0: //Fight
                                StartPhase3();
                                return;

                            case 1: //Escape
                                if (_CanEscape)
                                    Escape();
                                return;
                        }
                        break;

                    case 3: //Actor Command
                        if (_PlayerCommand.Enabled)
                        {
                            switch (_PlayerCommand.CursorPosition)
                            {
                                case 0: //Attack
                                    //Set current action
                                    _CurrentAction = new BattleAction(Game, BattleAction.eKind.ATTACK);

                                    //Select the target
                                    StartTargetSelection(eTargetType.SINGLE_ENEMY);
                                    return;

                                case 1: //Magic
                                    return;

                                case 2: //Item
                                    //Set current action
                                    _CurrentAction = new BattleAction(Game, BattleAction.eKind.ITEM);

                                    //Select the item
                                    StartItemSelection();
                                    return;

                                case 3: //Guard
                                    _CurrentAction = new BattleAction(Game, BattleAction.eKind.GUARD);

                                    StartPhase4();
                                    return;

                                case 4: //Run
                                    if (_CanEscape)
                                    {
                                        _CurrentAction = new BattleAction(Game, BattleAction.eKind.WAIT);
                                        Escape();
                                    }
                                    return;
                            }
                        } // if (_PlayerCommand.Enabled)
                        else if (_Target != null)
                        {
                            _CurrentAction.Target = _Target;
                            EndTargetSelection();

                            //Next
                            StartPhase4();
                            return;
                        }
                        else if (_ItemSelection.Enabled)
                        {
                            _CurrentAction.Item = _ItemSelection.ItemSelected;
                            EndItemSelection();
                        }
                        break;

                    case 5: //Result Command
                        BattleEnd(0);
                        break;
                } // switch (_Phase)
            } //if(wpaddown & CONTROL_ACCEPT)

            if (Input.keyStateDown.IsKeyDown(Keys.Back))
            {
                switch (_Phase)
                {
                    case 3: //Actor Command
                        if (_Target != null)
                        {
                            EndTargetSelection();

                            switch (_PlayerCommand.CursorPosition)
                            {
                                case 0: //Attack
                                    _PlayerCommand.Enabled = true;
                                    break;
                                case 2: //Item
                                    StartItemSelection();
                                    break;
                            }
                            return;
                        }
                        else if (_ItemSelection.Enabled)
                        {
                            _CurrentAction.Item = null;
                            EndItemSelection();
                            _PlayerCommand.Enabled = true;
                            return;
                        }
                        break;

                    case 5: //Result Command
                        BattleEnd(0);
                        break;
                }
            }

            if (Input.keyStateHeld.IsKeyDown(Keys.PageUp))
            {
                //Scroll le CTB
                _CTBWindowScrollY += 4;

                if (_CTBWindowScrollY > 0)
                    _CTBWindowScrollY = 0;
                return;
            }

            if (Input.keyStateHeld.IsKeyDown(Keys.PageDown))
            {
                //Scroll le CTB
                _CTBWindowScrollY -= 4;

                if (_CTBWindowScrollY < -(32 * MAX_CTB - _CTBWindow.Rectangle.Height))
                    _CTBWindowScrollY = -(32 * MAX_CTB - _CTBWindow.Rectangle.Height);
                return;
            }
        }
    }
}
