using Raylib_cs;
using System;
using System.Numerics;

namespace Pong
{
    class Ball
    {
        Vector2 m_position;
        Vector2 m_direction;
        float m_radius;
        float m_speed;
        Color m_color;

        // Setters
        public void SetPosition(Vector2 a_position) { m_position = a_position; }
        public void SetDirection(Vector2 a_direction) { m_direction = a_direction; }
        public void SetRadius(float a_radius) { m_radius = a_radius; }
        public void SetSpeed(float a_speed) { m_speed = a_speed; }
        public void SetColor(Color a_color) { m_color = a_color; }

        // Getters
        public Vector2 GetPosition() { return m_position; }
        public Vector2 GetDirection() { return m_direction; }
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
        public void SetCorner(Vector2 a_center) { m_center = a_center; }

        // Getters
        public Vector2 GetPosition() { return m_position; }
        public Vector2 GetSize() { return m_size; }
        public Color GetColor() { return m_color; }
        public KeyboardKey GetUpKey() { return m_upKey; }
        public KeyboardKey GetDownKey() { return m_downKey; }
        public float GetSpeed() { return m_speed; }
        public int GetScore() { return m_score;}
        public Vector2 GetCorner() { return m_center; }
    }

    class Program
    {
        Vector2 m_aspectRatio = new Vector2(16, 9);
        
        // 1080p is 120
        // 4k is 256
        static int m_windowSize = 120;
        static string m_windowName = "Pong";

        int m_windowWidth;
        int m_windowHeight;

        Ball m_ball = new Ball();
        Paddle m_leftPaddle = new Paddle();
        Paddle m_rightPaddle = new Paddle();

        static void Main(string[] args)
        {
            Program p = new Program();

            p.RunProgram();
        }

        void RunProgram()
        {
            m_windowWidth = m_windowSize * (int)m_aspectRatio.X;
            m_windowHeight = m_windowSize * (int)(m_aspectRatio.Y);

            Raylib.InitWindow(m_windowWidth, m_windowHeight, m_windowName);
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
            // Set ball values
            m_ball.SetDirection(new Vector2(0.707f, 0.707f));
            m_ball.SetRadius(35f);
            m_ball.SetSpeed(7f);
            m_ball.SetColor(Color.WHITE);

            // Set left paddle values
            m_leftPaddle.SetSize(new Vector2(30, 300));
            m_leftPaddle.SetColor(Color.BLUE);
            m_leftPaddle.SetUpKey(KeyboardKey.KEY_W);
            m_leftPaddle.SetDownKey(KeyboardKey.KEY_S);
            m_leftPaddle.SetSpeed(10f);
            m_leftPaddle.SetScore(0);

            // Set right paddle values
            m_rightPaddle.SetSize(new Vector2(30, 300));
            m_rightPaddle.SetColor(Color.RED);
            m_rightPaddle.SetUpKey(KeyboardKey.KEY_UP);
            m_rightPaddle.SetDownKey(KeyboardKey.KEY_DOWN);
            m_rightPaddle.SetSpeed(10f);
            m_rightPaddle.SetScore(0);

            // set initial position
            ResetPos();
        }
        
        void Update()
        {
            MovePaddles();
            MoveBall();
            // Updates corner positions for both paddles
            UpdateCorners();

            BallPaddleCollision(m_leftPaddle);
            BallPaddleCollision(m_rightPaddle);
        }

        void UpdateCorners()
        {
            m_leftPaddle.SetCorner(
                new Vector2(
                    m_leftPaddle.GetPosition().X - (m_leftPaddle.GetSize().X / 2),
                    m_leftPaddle.GetPosition().Y - (m_leftPaddle.GetSize().Y / 2)
                ));

            m_rightPaddle.SetCorner(
                new Vector2(
                    m_rightPaddle.GetPosition().X - (m_rightPaddle.GetSize().X / 2),
                    m_rightPaddle.GetPosition().Y - (m_rightPaddle.GetSize().Y / 2)
                ));
        }

        void MovePaddles()
        {
            // Move left paddle up and down
            if (Raylib.IsKeyDown(m_leftPaddle.GetUpKey()))
                m_leftPaddle.SetPosition(new Vector2(m_leftPaddle.GetPosition().X, m_leftPaddle.GetPosition().Y - m_leftPaddle.GetSpeed()));
            if (Raylib.IsKeyDown(m_leftPaddle.GetDownKey()))
                m_leftPaddle.SetPosition(new Vector2(m_leftPaddle.GetPosition().X, m_leftPaddle.GetPosition().Y + m_leftPaddle.GetSpeed()));

            // Move right paddle up and down
            if (Raylib.IsKeyDown(m_rightPaddle.GetUpKey()))
                m_rightPaddle.SetPosition(new Vector2(m_rightPaddle.GetPosition().X, m_rightPaddle.GetPosition().Y - m_rightPaddle.GetSpeed()));
            if (Raylib.IsKeyDown(m_rightPaddle.GetDownKey()))
                m_rightPaddle.SetPosition(new Vector2(m_rightPaddle.GetPosition().X, m_rightPaddle.GetPosition().Y + m_rightPaddle.GetSpeed()));
        }

        void MoveBall()
        {
            m_ball.SetPosition(m_ball.GetPosition() + m_ball.GetDirection() * m_ball.GetSpeed());

            // ball bounce off left of screen
            if (m_ball.GetPosition().X - m_ball.GetRadius() < 0)
            {
                m_rightPaddle.SetScore(m_rightPaddle.GetScore() + 1);
                ResetPos();
            }

            // ball bounce off right of screen
            if (m_ball.GetPosition().X + m_ball.GetRadius() > m_windowWidth)
            {
                m_leftPaddle.SetScore(m_leftPaddle.GetScore() + 1);
                ResetPos();
            }

            // ball bounce off top of screen
            if (m_ball.GetPosition().Y - m_ball.GetRadius() < 0)
                m_ball.SetDirection(new Vector2(m_ball.GetDirection().X, -m_ball.GetDirection().Y));

            // ball bounce off bottom of screen
            if (m_ball.GetPosition().Y + m_ball.GetRadius() > m_windowHeight)
                m_ball.SetDirection(new Vector2(m_ball.GetDirection().X, -m_ball.GetDirection().Y));
        }

        void BallPaddleCollision(Paddle a_paddle)
        {
            float top = a_paddle.GetPosition().Y - (a_paddle.GetSize().Y / 2);
            float bottom = a_paddle.GetPosition().Y + (a_paddle.GetSize().Y / 2);
            float left = a_paddle.GetPosition().X - (a_paddle.GetSize().X * 2);
            float right = a_paddle.GetPosition().X + (a_paddle.GetSize().X * 2);

            if (m_ball.GetPosition().Y > top &&
                m_ball.GetPosition().Y < bottom &&
                m_ball.GetPosition().X > left &&
                m_ball.GetPosition().X < right)

                m_ball.SetDirection(new Vector2(-m_ball.GetDirection().X, m_ball.GetDirection().Y));
        }

        void ResetPos()
        {
            m_ball.SetPosition(new Vector2(m_windowWidth / 2, m_windowHeight / 2));
            m_leftPaddle.SetPosition(new Vector2(30, m_windowHeight / 2));
            m_rightPaddle.SetPosition(new Vector2(m_windowWidth - 30, m_windowHeight / 2));
        }

        void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            Raylib.DrawText(m_leftPaddle.GetScore().ToString(), 30, 30, 40, Color.LIGHTGRAY);
            Raylib.DrawText(m_rightPaddle.GetScore().ToString(), m_windowWidth - 50, 30, 40, Color.LIGHTGRAY);

            Raylib.DrawRectangleV(m_leftPaddle.GetCorner(), m_leftPaddle.GetSize(), m_leftPaddle.GetColor());
            Raylib.DrawRectangleV(m_rightPaddle.GetCorner(), m_rightPaddle.GetSize(), m_rightPaddle.GetColor());

            Raylib.DrawCircleV(m_ball.GetPosition(), m_ball.GetRadius(), m_ball.GetColor());

            // Debug
            // Raylib.DrawCircleV(m_rightPaddle.GetCorner(), 5f, Color.YELLOW);
            // Raylib.DrawCircleV(m_rightPaddle.GetPosition(), 5f, Color.YELLOW);
            // Raylib.DrawCircleV(m_leftPaddle.GetCorner(), 5f, Color.YELLOW);
            // Raylib.DrawCircleV(m_leftPaddle.GetPosition(), 5f, Color.YELLOW);
            // Raylib.DrawCircleV(m_ball.GetPosition(), 5f, Color.YELLOW);

            Raylib.EndDrawing();
        }
    }
}