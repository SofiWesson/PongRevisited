﻿using Raylib_cs;
using System;
using System.Numerics;
using System.Linq;

namespace Pong
{
    class Application
    {
        GameStateManager gameStateManager = null;

        Vector2 aspectRatio = new Vector2(16, 9);

        static int windowSize = 120;
        static string windowName = "Pong";

        int windowWidth;
        int windowHeight;

        bool shouldQuit = false;

        public int GetWindowSize()
        {
            return windowSize;
        }

        public int GetWindowWidth()
        {
            return windowWidth;
        }

        public int GetWindowHeight()
        {
            return windowHeight;
        }

        public void Run()
        {
            windowWidth = windowSize * (int)aspectRatio.X;
            windowHeight = windowSize * (int)(aspectRatio.Y);

            Raylib.InitWindow(windowWidth, windowHeight, windowName);
            Raylib.SetTargetFPS(60);

            Load();

            while (!shouldQuit)
            {
                Update();
                Draw();
            }

            Raylib.CloseWindow();
        }

        void Load()
        {
            gameStateManager = new GameStateManager();
            gameStateManager.SetState("Menu", new MenuState(this));
            gameStateManager.SetState("Game", new GameState(this));
            gameStateManager.SetState("Pause", new PauseState(this));
            gameStateManager.SetState("Quit", new QuitState(this));

            gameStateManager.PushState("Menu");
        }

        void Unload()
        {

        }

        void Update()
        {
            gameStateManager.Update();
        }

        void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.RAYWHITE);

            gameStateManager.Draw();

            Raylib.EndDrawing();
        }

        public GameStateManager GetGameStateManager()
        {
            return gameStateManager;
        }

        public void Quit()
        {
            shouldQuit = true;
        }
    }

    class GameStateManager
    {
        protected IDictionary<string, IGameState> states = new Dictionary<string, IGameState>();
        protected List<IGameState> stack = new List<IGameState>();
        protected List<Action> commands = new List<Action>();

        public void Unload()
        {
            
        }

        public void Update()
        {
            for (int i = 0; i < commands.Count; i++)
                commands[i].Invoke();
            commands.Clear();

            stack.Last().Update();
        }

        public void Draw()
        {
            for (int i = 0; i < stack.Count; i++)
            {
                stack[i].Draw();
            }
        }

        public void SetState(string name, IGameState state)
        {
            commands.Insert(commands.Count, () =>
            {
                states[name] = state;
            });
        }

        public void PushState(string name)
        {
            commands.Insert(commands.Count, () =>
            {
                if (states.ContainsKey(name))
                {
                    states[name].Unload();
                }

                stack.Insert(stack.Count, states[name]);

                if (states.ContainsKey(name))
                {
                    states[name].Load();
                }

            });
        }

        public void PopState()
        {
            commands.Insert(commands.Count, () =>
            {
                stack.RemoveAt(stack.Count - 1);
            });
        }
    }

    class IGameState
    {
        virtual public void Load() { }
        virtual public void Unload() { }
        virtual public void Update() { }
        virtual public void Draw() { }
    }

    class MenuState : IGameState
    {
        enum Selector
        {
            START,
            Game,
            Quit,
            END
        }

        Application app;

        string playText = "Play";
        string quitText = "Quit";
        string selectorText = "> ";

        Selector selected = Selector.Game;

        public MenuState(Application app)
        {
            this.app = app;
        }

        override public void Load()
        {

        }

        override public void Unload()
        {
            
        }

        public override void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))
                selected++;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
                selected--;

            if (selected == Selector.START)
                selected = Selector.END - 1;
            if (selected == Selector.END)
                selected = Selector.START + 1;

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
            {
                app.GetGameStateManager().PopState();
                app.GetGameStateManager().PushState(selected.ToString());
            }
        }

        public override void Draw()
        {
            float windowSize = app.GetWindowSize();

            Raylib.ClearBackground(Color.BLACK);
            Raylib.DrawText("Pong", (int)(0.2f * windowSize), (int)(0.2f * windowSize), (int)(1.4f * windowSize), Color.LIGHTGRAY);
            Raylib.DrawText(
                selected == Selector.Game ? selectorText + playText : playText,
                (int)(0.2f * windowSize),
                (int)(2.8f * windowSize),
                (int)(1f * windowSize),
                Color.LIGHTGRAY);
            Raylib.DrawText(
                selected == Selector.Quit ? selectorText + quitText : quitText,
                (int)(0.2f * windowSize),
                (int)(4f * windowSize),
                (int)(1f * windowSize),
                Color.LIGHTGRAY);
        }
    }

    class QuitState : IGameState
    {
        Application app;

        public QuitState(Application app)
        {
            this.app = app;
        }

        override public void Load()
        {
            app.Quit();
        }

        override public void Unload()
        {

        }

        override public void Update()
        {

        }

        override public void Draw()
        {

        }
    }

    class Ball
    {
        Vector2 position;
        Vector2 direction;
        float radius;
        float speed;
        Color color;

        // Setters
        public void SetPosition(Vector2 a_position) { position = a_position; }
        public void SetDirection(Vector2 a_direction) { direction = a_direction; }
        public void SetRadius(float a_radius) { radius = a_radius; }
        public void SetSpeed(float a_speed) { speed = a_speed; }
        public void SetColor(Color a_color) { color = a_color; }

        // Getters
        public Vector2 GetPosition() { return position; }
        public Vector2 GetDirection() { return direction; }
        public float GetRadius() { return radius; }
        public float GetSpeed() { return speed; }
        public Color GetColor() { return color;}
    }

    class Paddle
    {
        Vector2 position;
        Vector2 size;
        Vector2 corner;
        Color color;
        KeyboardKey upKey;
        KeyboardKey downKey;
        float speed;
        int score;

        // Setters
        public void SetPosition(Vector2 a_pos) { position = a_pos; }
        public void SetSize(Vector2 a_size) { size = a_size; }
        public void SetColor(Color a_color) { color = a_color; }
        public void SetUpKey(KeyboardKey a_upKey) { upKey = a_upKey; }
        public void SetDownKey(KeyboardKey a_downKey) { downKey = a_downKey; }
        public void SetSpeed(float a_speed) { speed = a_speed; }
        public void SetScore(int a_score) { score = a_score;}
        public void SetCorner(Vector2 a_center) { corner = a_center; }

        // Getters
        public Vector2 GetPosition() { return position; }
        public Vector2 GetSize() { return size; }
        public Color GetColor() { return color; }
        public KeyboardKey GetUpKey() { return upKey; }
        public KeyboardKey GetDownKey() { return downKey; }
        public float GetSpeed() { return speed; }
        public int GetScore() { return score;}
        public Vector2 GetCorner() { return corner; }
    }

    class GameState : IGameState
    {
        Application app;

        Ball ball = new Ball();
        Paddle leftPaddle = new Paddle();
        Paddle rightPaddle = new Paddle();

        bool debugEnabled = false;

        int windowSize;
        int windowWidth;
        int windowHeight;

        public GameState(Application app)
        {
            this.app = app;
        }

        override public void Load()
        {
            windowSize = app.GetWindowSize();
            windowWidth = app.GetWindowWidth();
            windowHeight = app.GetWindowHeight();

            // Set ball values
            ball.SetDirection(new Vector2(0.707f, 0.707f));
            ball.SetRadius(0.29f * windowSize);
            ball.SetSpeed(0.07f * windowSize);
            ball.SetColor(Color.WHITE);

            // Set left paddle values
            leftPaddle.SetSize(new Vector2(.23f * windowSize, 2.5f * windowSize));
            leftPaddle.SetColor(Color.BLUE);
            leftPaddle.SetUpKey(KeyboardKey.KEY_W);
            leftPaddle.SetDownKey(KeyboardKey.KEY_S);
            leftPaddle.SetSpeed(.07f * windowSize);
            leftPaddle.SetScore(0);

            // Set right paddle values
            rightPaddle.SetSize(new Vector2(.23f * windowSize, 2.5f * windowSize));
            rightPaddle.SetColor(Color.RED);
            rightPaddle.SetUpKey(KeyboardKey.KEY_UP);
            rightPaddle.SetDownKey(KeyboardKey.KEY_DOWN);
            rightPaddle.SetSpeed(.07f * windowSize);
            rightPaddle.SetScore(0);

            // set initial position
            ResetPos();
        }

        override public void Unload()
        {

        }

        public override void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_F11))
            {
                if (Raylib.IsWindowState(ConfigFlags.FLAG_FULLSCREEN_MODE))
                    Raylib.ClearWindowState(ConfigFlags.FLAG_FULLSCREEN_MODE);
                else
                    Raylib.SetWindowState(ConfigFlags.FLAG_FULLSCREEN_MODE);
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_F1))
                debugEnabled = !debugEnabled;

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_P))
            {
                app.GetGameStateManager().PushState("Pause");
            }

            MovePaddle(leftPaddle);
            MovePaddle(rightPaddle);
            MoveBall();
            // Updates corner positions for both paddles
            UpdateCorners();

            BallPaddleCollision(leftPaddle);
            BallPaddleCollision(rightPaddle);
        }

        void UpdateCorners()
        {
            leftPaddle.SetCorner(
                new Vector2(
                    leftPaddle.GetPosition().X - (leftPaddle.GetSize().X / 2),
                    leftPaddle.GetPosition().Y - (leftPaddle.GetSize().Y / 2)
                ));

            rightPaddle.SetCorner(
                new Vector2(
                    rightPaddle.GetPosition().X - (rightPaddle.GetSize().X / 2),
                    rightPaddle.GetPosition().Y - (rightPaddle.GetSize().Y / 2)
                ));
        }

        void MovePaddle(Paddle paddle)
        {
            // Move paddle up and down
            if (Raylib.IsKeyDown(paddle.GetUpKey()))
                paddle.SetPosition(new Vector2(paddle.GetPosition().X, paddle.GetPosition().Y - paddle.GetSpeed()));
            if (Raylib.IsKeyDown(paddle.GetDownKey()))
                paddle.SetPosition(new Vector2(paddle.GetPosition().X, paddle.GetPosition().Y + paddle.GetSpeed()));

            // Keep paddle on screen
            if (paddle.GetPosition().Y - (paddle.GetSize().Y / 2) < 0)
                paddle.SetPosition(new Vector2(paddle.GetPosition().X, paddle.GetPosition().Y + paddle.GetSpeed()));
            if (paddle.GetPosition().Y + (paddle.GetSize().Y / 2) > windowHeight)
                paddle.SetPosition(new Vector2(paddle.GetPosition().X, paddle.GetPosition().Y - paddle.GetSpeed()));
        }

        void MoveBall()
        {
            ball.SetPosition(ball.GetPosition() + ball.GetDirection() * ball.GetSpeed());

            // ball bounce off left of screen
            if (ball.GetPosition().X - ball.GetRadius() < 0)
            {
                rightPaddle.SetScore(rightPaddle.GetScore() + 1);
                ResetPos();
            }

            // ball bounce off right of screen
            if (ball.GetPosition().X + ball.GetRadius() > windowWidth)
            {
                leftPaddle.SetScore(leftPaddle.GetScore() + 1);
                ResetPos();
            }

            // ball bounce off top of screen
            if (ball.GetPosition().Y - ball.GetRadius() < 0)
                ball.SetDirection(new Vector2(ball.GetDirection().X, -ball.GetDirection().Y));

            // ball bounce off bottom of screen
            if (ball.GetPosition().Y + ball.GetRadius() > windowHeight)
                ball.SetDirection(new Vector2(ball.GetDirection().X, -ball.GetDirection().Y));
        }

        void BallPaddleCollision(Paddle a_paddle)
        {
            float top = a_paddle.GetPosition().Y - (a_paddle.GetSize().Y / 2);
            float bottom = a_paddle.GetPosition().Y + (a_paddle.GetSize().Y / 2);
            float left = a_paddle.GetPosition().X - (a_paddle.GetSize().X * 2);
            float right = a_paddle.GetPosition().X + (a_paddle.GetSize().X * 2);

            if (ball.GetPosition().Y > top &&
                ball.GetPosition().Y < bottom &&
                ball.GetPosition().X > left &&
                ball.GetPosition().X < right)

                ball.SetDirection(new Vector2(-ball.GetDirection().X, ball.GetDirection().Y));
        }

        void ResetPos()
        {
            ball.SetPosition(new Vector2(windowWidth / 2, windowHeight / 2));
            leftPaddle.SetPosition(new Vector2(.23f * windowSize, windowHeight / 2));
            rightPaddle.SetPosition(new Vector2(windowWidth - (.23f * windowSize), windowHeight / 2));
        }

        public override void Draw()
        {
            Raylib.ClearBackground(Color.BLACK);

            Raylib.DrawRectangleV(leftPaddle.GetCorner(), leftPaddle.GetSize(), leftPaddle.GetColor());
            Raylib.DrawRectangleV(rightPaddle.GetCorner(), rightPaddle.GetSize(), rightPaddle.GetColor());

            Raylib.DrawCircleV(ball.GetPosition(), ball.GetRadius(), ball.GetColor());

            Raylib.DrawText(leftPaddle.GetScore().ToString(), (int)(.40f * windowSize), (int)(.25f * windowSize), (int)(0.5 * windowSize), Color.LIGHTGRAY);
            Raylib.DrawText(rightPaddle.GetScore().ToString(), windowWidth - (int)(.65f * windowSize), (int)(.25f * windowSize), (int)(0.5 * windowSize), Color.LIGHTGRAY);

            // Debug
            if (debugEnabled)
            {
                Raylib.DrawCircleV(rightPaddle.GetCorner(), 0.05f * windowSize, Color.YELLOW);
                Raylib.DrawCircleV(rightPaddle.GetPosition(), 0.05f * windowSize, Color.YELLOW);
                Raylib.DrawCircleV(leftPaddle.GetCorner(), 0.05f * windowSize, Color.YELLOW);
                Raylib.DrawCircleV(leftPaddle.GetPosition(), 0.05f * windowSize, Color.YELLOW);
                Raylib.DrawCircleV(ball.GetPosition(), 0.05f * windowSize, Color.YELLOW);
            }
        }
    }

    class PauseState : IGameState
    {
        enum Selector
        {
            START,
            RESUME,
            LEAVE,
            END
        }

        Application app;

        string resumeText = "Resume";
        string leaveText = "Leave";
        string selectorText = "> ";

        Selector selected = Selector.RESUME;

        int windowSize;

        Color clearGray = new Color(255, 255, 255, 128);

        public PauseState(Application app)
        {
            this.app = app;
        }

        public override void Load()
        {
            windowSize = app.GetWindowSize();
            selected = Selector.RESUME;
        }

        public override void Unload()
        {
            
        }

        public override void Update()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_P))
            {
                app.GetGameStateManager().PopState();
            }

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))
                selected++;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
                selected--;

            if (selected == Selector.START)
                selected = Selector.END - 1;
            if (selected == Selector.END)
                selected = Selector.START + 1;

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER) && selected == Selector.RESUME)
                app.GetGameStateManager().PopState();
            else if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER) && selected == Selector.LEAVE)
            {
                app.GetGameStateManager().PopState();
                app.GetGameStateManager().PopState();
                app.GetGameStateManager().PushState("Menu");
            }
        }

        public override void Draw()
        {
            Raylib.DrawRectangle(0, 0, app.GetWindowWidth(), app.GetWindowHeight(), clearGray);
            Raylib.DrawText("Paused", (int)(0.2f * windowSize), (int)(0.2f * windowSize), (int)(1.4f * windowSize), Color.WHITE);
            Raylib.DrawText(
                selected == Selector.RESUME ? selectorText + resumeText : resumeText,
                (int)(0.2f * windowSize),
                (int)(2.8f * windowSize),
                (int)(1f * windowSize),
                Color.WHITE);
            Raylib.DrawText(
                selected == Selector.LEAVE ? selectorText + leaveText : leaveText,
                (int)(0.2f * windowSize),
                (int)(4f * windowSize),
                (int)(1f * windowSize),
                Color.WHITE);
        }
    }

    class Program
    {
        Vector2 aspectRatio = new Vector2(16, 9);
        
        // 720p is 45
        // 1080p is 120
        // 4k is 240
        static int windowSize = 120;
        static string windowName = "Pong";

        int windowWidth;
        int windowHeight;

        Ball ball = new Ball();
        Paddle leftPaddle = new Paddle();
        Paddle rightPaddle = new Paddle();

        bool debugEnabled = false;

        static void Main(string[] args)
        {
            Application app = new Application();
            app.Run();
        }
    }
}