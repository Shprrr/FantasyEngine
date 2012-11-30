using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FantasyEngine.Classes.Overworld
{
    public class NPC : Sprite
    {
        public enum eAction
        {
            Stay,
            Moving,
            Blocked,
            Talking
        }

        private Window _MessageWindow;
        private int _MessageWindowBottomY;
        private string[] _Message = new string[0];
        private int _MessageIndex;
        private Thread _MessageThread;
        private eDirection _InitialDirection;

        public string Name { get; set; }
        public eAction Action { get; private set; }
        public bool RegainDirectionAfterTalk { get; set; }
        public int Step { get; set; }

        public NPC(Game game, string name, string charsetName, Rectangle tileSize, Vector2 position)
            : base(game, charsetName, tileSize, position, OVERWORLD_SIZE)
        {
            _MovePxPerMillisecond = 0.06f;
            Name = name;
            RegainDirectionAfterTalk = false;

            int height = (4 * GameMain.font.LineSpacing) + (Window.Tileset.TileHeight * 2);
            _MessageWindowBottomY = Game.GraphicsDevice.Viewport.Height - height;
            _MessageWindow = new Window(Game, 0, _MessageWindowBottomY,
                Game.GraphicsDevice.Viewport.Width, height);
            _MessageWindow.Enabled = false;
            _MessageWindow.Visible = false;
        }

        public NPC(Game game, string name, string charsetName, Rectangle tileSize, Vector2 position, eDirection direction)
            : this(game, name, charsetName, tileSize, position)
        {
            _InitialDirection = direction;
            Direction = direction;
        }

        public NPC(Game game, string name, string charsetName, Rectangle tileSize, Vector2 position, eDirection direction, bool regainDirection)
            : this(game, name, charsetName, tileSize, position, direction)
        {
            RegainDirectionAfterTalk = regainDirection;
        }

        public override string ToString()
        {
            return Name;
        }

        public void DrawGUI(GameTime gameTime)
        {
            DrawTalk(gameTime);
        }

        private byte frame = 0;
        private void DrawTalk(GameTime gameTime)
        {
            if (!_MessageWindow.Visible)
                return;

            frame = (byte)((frame + 1) % 16);

            _MessageWindow.Offset = GameMain.spriteBatchGUI.CameraOffset;
            _MessageWindow.Draw(gameTime);

            GameMain.spriteBatchGUI.Scissor(_MessageWindow.InsideBound);

            GameMain.spriteBatchGUI.DrawString(GameMain.font, _Message[_MessageIndex],
                new Vector2(_MessageWindow.InsideBound.X, _MessageWindow.InsideBound.Y) + GameMain.spriteBatchGUI.CameraOffset, Color.White);

            GameMain.spriteBatchGUI.ScissorReset();

            if (frame < 12)
                GameMain.spriteBatchGUI.Draw(Window.NextDialog,
                    new Vector2(_MessageWindow.InsideBound.Right - Window.NextDialog.Width,
                        _MessageWindow.Rectangle.Bottom - Window.NextDialog.Height * 2) + GameMain.spriteBatchGUI.CameraOffset,
                    Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            RaiseOnMoving(gameTime);

            if (_MessageWindow.Enabled && Input.keyStateDown.IsKeyDown(Keys.Enter))
            {
                _MessageIndex++;
                if (_MessageIndex >= _Message.Length)
                {
                    _MessageWindow.Enabled = false;
                    _MessageWindow.Visible = false;
                    if (RegainDirectionAfterTalk)
                        Direction = _InitialDirection;
                    Player.GamePlayer.Hero.Enabled = true;
                    _MessageThread.Interrupt();
                    Action = eAction.Stay;
                }
                Input.CatchKeys(Keys.Enter);
            }
        }

        public void Stay()
        {
            Action = eAction.Stay;
        }

        public void Talk(string message)
        {
            List<string> messages = new List<string>();
            string messageFit = string.Empty;
            int nbLines = 0;
            string[] words = message.Split(' ');
            string lastLine = string.Empty;
            StringBuilder line = new StringBuilder(500);

            // Format the message to fit in the screen.
            for (int i = 0; i < words.Length; i++)
            {
                if (i != 0)
                    line.Append(" ");
                line.Append(words[i]);
                if (GameMain.font.MeasureString(line).X > _MessageWindow.InsideBound.Width)
                {
                    nbLines++;
                    messageFit += lastLine + (nbLines != 4 ? Environment.NewLine : string.Empty);
                    if (nbLines == 4)
                    {
                        messages.Add(messageFit);
                        messageFit = string.Empty;
                        nbLines = 0;
                    }
                    line.Length = 0;
                    line.Append(words[i]);
                }
                lastLine = line.ToString();
            }
            messageFit += lastLine;
            messages.Add(messageFit);

            _Message = messages.ToArray();
            _MessageIndex = 0;
            _MessageWindow.Enabled = true;
            _MessageWindow.Visible = true;
            Player.GamePlayer.Hero.Enabled = false;
            Action = eAction.Talking;
            _MessageThread = Thread.CurrentThread;
            try
            {
                Thread.Sleep(Timeout.Infinite);
            }
            catch (ThreadInterruptedException)
            {
            }
        }

        public void Move(GameTime gameTime, eDirection direction, int maxStep = int.MaxValue)
        {
            Vector2 newOffset;

            if (CheckCollision(gameTime, direction, out newOffset, maxStep))
            {
                Position += newOffset;
                Step += (int)newOffset.Length();

                if (Action != eAction.Moving)
                    _MovingTime = gameTime.TotalGameTime;
                Action = eAction.Moving;
            }
            else
            {
                _MovingTime = TimeSpan.Zero;
                Action = eAction.Blocked;
            }
        }

        #region Events
        public delegate void TalkingHandler(EventArgs e, NPC npc);
        public event TalkingHandler Talking;
        public virtual void RaiseOnTalking()
        {
            // Raise the event by using the () operator.
            if (Talking != null)
                Talking(new EventArgs(), this);
        }

        public delegate void MovingHandler(EventArgs e, NPC npc, GameTime gameTime);
        public event MovingHandler Moving;
        public virtual void RaiseOnMoving(GameTime gameTime)
        {
            // Raise the event by using the () operator.
            if (Moving != null)
                Moving(new EventArgs(), this, gameTime);
        }
        #endregion Events
    }
}
