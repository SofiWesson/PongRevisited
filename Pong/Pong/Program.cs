using Raylib_cs;
using System;
using System.Numerics;

namespace Pong
{
    class Ball
    {
        Vector2 m_position;
        Vector2 m_direction;
        Vector2 m_center;
        float m_radius;
        float m_speed;
        Color m_color;

        // Setters
        public void SetPosition(Vector2 a_position) { m_position = a_position; }
        public void SetDirection(Vector2 a_direction) { m_direction = a_direction; }
        public void SetCenter(Vector2 a_center) { m_center = a_center; }
        public void SetRadius(float a_radius) { m_radius = a_radius; }
        public void SetSpeed(float a_speed) { m_speed = a_speed; }
        public void SetColor(Color a_color) { m_color = a_color; }

        // Getters
        public Vector2 GetPosition() { return m_position; }
        public Vector2 GetDirection() { return m_direction; }
        public Vector2 GetCenter() { return m_center; }
        public float GetRadius() { return m_radius; }
        public float GetSpeed() { return m_speed; }
        public Color GetColor() { return m_color;}
    }

    class Paddle
    {
        Vector2 m_position;
        Vector2 m_size;
        Vector2 m_center;
        Color m_color;
        KeyboardKey m_upKey;
        KeyboardKey m_downKey;
        float m_speed;
        int m_score;

        // Setters
        public void SetPosition(Vector2 a_pos) { m_position = a_pos; }
        public void SetSize(Vector2 a_size) { m_size = a_size; }
        public void SetColor(Color a_color) { m_color = a_color; }
        public void SetUpKey(KeyboardKey a_upKey) { m_upKey = a_upKey; }
        public void SetDownKey(KeyboardKey a_downKey) { m_downKey = a_downKey; }
        public void SetSpeed(float a_speed) { m_speed = a_speed; }
        public void SetScore(int a_score) { m_score = a_score;}
        public void SetCenter(Vector2 a_center) { m_center = a_center; }

        // Getters
        public Vector2 GetPosition() { return m_position; }
        public Vector2 GetSize() { return m_size; }
        public Color GetColor() { return m_color; }
        public KeyboardKey GetUpKey() { return m_upKey; }
        public KeyboardKey GetDownKey() { return m_downKey; }
        public float GetSpeed() { return m_speed; }
        public int GetScore() { return m_score;}
        public Vector2 GetCenter() { return m_center; }
    }

    class Program
    {
        Vector2 m_aspectRatio = new Vector2(16, 9);
        
        // 1080p is 120
        // 4k is 256
        static int m_windowSize = 120;
        static string m_windowName = "Pong";

        Ball ball = new Ball();
        Paddle leftPaddle = new Paddle();
        Paddle rightPaddle = new Paddle();

        static void Main(string[] args)
        {
            Program p = new Program();

            p.RunProgram();
        }

        void RunProgram()
        {
            int windowWidth = m_windowSize * (int)m_aspectRatio.X;
            int windowHeight = m_windowSize * (int)(m_aspectRatio.Y);

            Raylib.InitWindow(windowWidth, windowHeight, m_windowName);
            Raylib.SetTargetFPS(60);

            LoadGame();

            while (!Raylib.WindowShouldClose())
            {
                Update();
                Draw();
            }

            Raylib.WindowShouldClose();
        }

        void LoadGame()
        {
            int windowWidth = m_windowSize * (int)m_aspectRatio.X;
            int windowHeight = m_windowSize * (int)(m_aspectRatio.Y);

            // Set ball values
            ball.SetPosition(new Vector2(windowWidth / 2, windowHeight / 2));
            ball.SetDirection(new Vector2(0.707f, 0.707f));
            ball.SetRadius(35f);
            ball.SetSpeed(5f);
            ball.SetColor(Color.WHITE);

            // Set left paddle values
            leftPaddle.SetPosition(new Vector2(30, windowHeight / 2));
            leftPaddle.SetSize(new Vector2(30, 300));
            leftPaddle.SetColor(Color.BLUE);
            leftPaddle.SetUpKey(KeyboardKey.KEY_W);
            leftPaddle.SetDownKey(KeyboardKey.KEY_S);
            leftPaddle.SetSpeed(10f);
            leftPaddle.SetScore(0);

            // Set right paddle values
            rightPaddle.SetPosition(new Vector2(windowWidth - 30, windowHeight / 2));
            rightPaddle.SetSize(new Vector2(30, 300));
            rightPaddle.SetColor(Color.RED);
            rightPaddle.SetUpKey(KeyboardKey.KEY_UP);
            rightPaddle.SetDownKey(KeyboardKey.KEY_DOWN);
            rightPaddle.SetSpeed(10f);
            rightPaddle.SetScore(0);
        }
        
        void Update()
        {
            // Updates centers of both paddles and ball
            UpdateCenters();
            MovePaddles();
            MoveBall();
        }

        void UpdateCenters()
        {
            leftPaddle.SetCenter(
                new Vector2(
                    leftPaddle.GetPosition().X - (leftPaddle.GetSize().X / 2),
                    leftPaddle.GetPosition().Y - (leftPaddle.GetSize().Y / 2)
                ));

            rightPaddle.SetCenter(
                new Vector2(
                    rightPaddle.GetPosition().X - (rightPaddle.GetSize().X / 2),
                    rightPaddle.GetPosition().Y - (rightPaddle.GetSize().Y / 2)
                ));

            ball.SetCenter(
                new Vector2(
                    ball.GetPosition().X - (ball.GetRadius() / 2),
                    ball.GetPosition().Y - (ball.GetRadius() / 2)
                ));
        }

        void MovePaddles()
        {
            // Move left paddle up and down
            if (Raylib.IsKeyDown(leftPaddle.GetUpKey()))
                leftPaddle.SetPosition(new Vector2(leftPaddle.GetPosition().X, leftPaddle.GetPosition().Y - leftPaddle.GetSpeed()));
            if (Raylib.IsKeyDown(leftPaddle.GetDownKey()))
                leftPaddle.SetPosition(new Vector2(leftPaddle.GetPosition().X, leftPaddle.GetPosition().Y + leftPaddle.GetSpeed()));

            // Move right paddle up and down
            if (Raylib.IsKeyDown(rightPaddle.GetUpKey()))
                rightPaddle.SetPosition(new Vector2(rightPaddle.GetPosition().X, rightPaddle.GetPosition().Y - rightPaddle.GetSpeed()));
            if (Raylib.IsKeyDown(rightPaddle.GetDownKey()))
                rightPaddle.SetPosition(new Vector2(rightPaddle.GetPosition().X, rightPaddle.GetPosition().Y + rightPaddle.GetSpeed()));
        }

        void MoveBall()
        {

        }

        void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            Raylib.DrawRectangleV(leftPaddle.GetCenter(), leftPaddle.GetSize(), leftPaddle.GetColor());
            Raylib.DrawRectangleV(rightPaddle.GetCenter(), rightPaddle.GetSize(), rightPaddle.GetColor());

            Raylib.DrawCircleV(ball.GetCenter(), ball.GetRadius(), ball.GetColor());

            Raylib.EndDrawing();
        }
    }
}