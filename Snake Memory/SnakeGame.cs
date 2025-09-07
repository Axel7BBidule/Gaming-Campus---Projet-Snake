using Raylib_CsLo;
using System.Numerics;
using System.Collections.Generic;

public class SnakeGame
{
    private Grid grid;
    private Snake snake;
    private SongManager songManager;

    private List<Apple> apples = new();
    private List<AppleColor> objective = new();
    private int objectiveIndex = 0;

    private Raylib_CsLo.Color[] appleColors;
    private string[] appleColorNames;
    private int colorCount;

    private float moveTimer = 1;
    private float moveInterval = .2f;

    private const int InitialHealth = 2;
    private int health = InitialHealth;
    public int HealthActual => health;

    public GameState State { get; private set; } = GameState.Playing;
    private GameState previousState = GameState.Playing;
    public int Score { get; private set; } = 0;




    public SnakeGame(int colorCount , SongManager? songManager = null)
    {
        this.colorCount = colorCount;
        grid = new Grid(20, 20);
        snake = new Snake(new Vector2(10, 10));
        if (songManager == null)
            throw new ArgumentNullException("songManager");
        this.songManager = songManager;

        appleColors = GenerateColors(colorCount);
        appleColorNames = GenerateColorNames(colorCount);

        Reset();
    }

    public void Update()
    {
        // Gestion de la musique selon l'état du jeu
        if ((State == GameState.GameOver || State == GameState.Victory) && previousState != State)
        {
            songManager.StopTheme();
        }
        if ((State == GameState.Playing && previousState != State) || (State == GameState.Menu && previousState != State))
        {
            songManager.PlayTheme();
            songManager.SetThemeVolume(0.2f);
        }

        previousState = State;

        if (State == GameState.GameOver || State == GameState.Victory)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_T)) State = GameState.Menu;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_R)) State = GameState.Restart;
            return;
        }

        // Contrôles
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) snake.ChangeDirection(new Vector2(0, -1));
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) snake.ChangeDirection(new Vector2(0, 1));
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT)) snake.ChangeDirection(new Vector2(-1, 0));
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_RIGHT)) snake.ChangeDirection(new Vector2(1, 0));

        // Déplacement
        moveTimer = moveTimer + Raylib.GetFrameTime();
        if (moveTimer >= moveInterval)
        {
            moveTimer = 0f;
            Vector2 Head = snake.Body[0] + snake.Direction;

            // Vérifie si le serpent touche un mur
            if (Head.X < 0 || Head.Y < 0 || Head.X >= grid.Size || Head.Y >= grid.Size)
            {
                State = GameState.GameOver;
                return;
            }

            // Vérifie si le serpent se mord lui-même
            bool bodyIsTouch = false;
            for (int i = 0; i < snake.Body.Count; i++)
            {
                if (snake.Body[i].Equals(Head))
                {
                    bodyIsTouch = true;
                    break;
                }
            }
            if (bodyIsTouch)
            {
                State = GameState.GameOver;
                return;
            }

            // Ajoute la nouvelle tête
            snake.Body.Insert(0, Head);

            // Vérifie si le serpent mange une pomme
            int appleIdx = -1;
            for (int i = 0; i < apples.Count; i++)
            {
                if (apples[i].Position.Equals(Head))
                {
                    appleIdx = i;
                    break;
                }
            }

            if (appleIdx != -1)
            {
                var eatenApple = apples[appleIdx];
                if (objectiveIndex < objective.Count && eatenApple.Color == objective[objectiveIndex])
                {
                    songManager.PlayAppleSound();
                    apples.RemoveAt(appleIdx);
                    objectiveIndex = objectiveIndex + 1;
                    Score = Score + 1;

                    if (objectiveIndex == objective.Count)
                        State = GameState.Victory;
                    else
                        SpawnAppleOfColor(objective[objectiveIndex]);
                }
                else
                {
                    health = health - 1;
                    if (health <= 0)
                        State = GameState.GameOver;
                    else
                    {
                        Reset();
                        health = 1;
                    }
                }
            }
            else
            {
               
                snake.Body.RemoveAt(snake.Body.Count - 1);
            }
        }
    }

    public void Draw()
    {
        grid.Draw();

        foreach (var apple in apples)
            apple.Draw(grid.CellSize, GetColor(apple.Color));

        snake.Draw(grid.CellSize);

        // Objectif
        int startY = 40;
        Raylib.DrawText("Ordre des pommes :", grid.Size * grid.CellSize + 30, startY, 20, Raylib.DARKGRAY);
        for (int i = 0; i < objective.Count; i++)
        {
            Raylib_CsLo.Color c = GetColor(objective[i]);
            string txt = appleColorNames[(int)objective[i]];
            int y = startY + 40 + i * 40;
            Raylib.DrawText(txt, grid.Size * grid.CellSize + 60, y, 24, c);
            if (i == objectiveIndex)
                Raylib.DrawRectangle(grid.Size * grid.CellSize + 30, y - 5, 10, 30, c);
        }

        // PV et fin de partie
        bool isEnd = State == GameState.GameOver || State == GameState.Victory;
        if (isEnd)
        {
            Raylib.DrawRectangle(0, 0, grid.Size * grid.CellSize, grid.Size * grid.CellSize, Raylib.Fade(Raylib.RAYWHITE, 0.7f));
            Raylib.DrawText($"PV : {health}", 200, startY + 400, 28, Raylib.RED);
            string msg = State == GameState.Victory ? "Victoire !" : "Game Over";
            Raylib.DrawText(msg, 300, 250, 40, Raylib.MAROON);
            Raylib.DrawText($"Score : {Score}", 300, 300, 32, Raylib.DARKBLUE);
            Raylib.DrawText("R - Rejouer", 300, 350, 28, Raylib.DARKGRAY);
            Raylib.DrawText("T - Menu", 300, 390, 28, Raylib.DARKGRAY);
        }
        else
        {
            Raylib.DrawText($"PV : {health}", 200, startY + 400, 28, Raylib.RED);
        }
    }

    public void Reset()
    {
        snake = new Snake(new Vector2(10, 10));
        Score = 0;
        objectiveIndex = 0;
        State = GameState.Playing;
        health = InitialHealth;
        GenerateObjective();
        SpawnApples();
    }

    private void GenerateObjective()
    {
        var rand = new System.Random();
        objective.Clear();
        for (int i = 0; i < colorCount; i++)
            objective.Add((AppleColor)rand.Next(0, colorCount));
    }

    private void SpawnApples()
    {
        apples.Clear();
        var rand = new System.Random();
        HashSet<Vector2> used = new();
        for (int i = 0; i < objective.Count; i++)
        {
            Vector2 pos;
            do
            {
                pos = new Vector2(rand.Next(0, grid.Size), rand.Next(0, grid.Size));
            } while (snake.Body.Contains(pos) || used.Contains(pos));
            used.Add(pos);
            apples.Add(new Apple(pos, objective[i]));
        }
    }

    private void SpawnAppleOfColor(AppleColor color)
    {
        var rand = new System.Random();
        Vector2 pos;
        do
        {
            pos = new Vector2(rand.Next(0, grid.Size), rand.Next(0, grid.Size));
        } while (snake.Body.Contains(pos) || apples.Exists(a => a.Position.Equals(pos)));
        apples.Add(new Apple(pos, color));
    }

    private Raylib_CsLo.Color GetColor(AppleColor color) => appleColors[(int)color];

    private Raylib_CsLo.Color[] GenerateColors(int count)
    {
        Raylib_CsLo.Color[] allColors = {
            Raylib.RED, Raylib.BLUE, Raylib.GREEN, Raylib.BLACK,
            Raylib.YELLOW, Raylib.VIOLET, Raylib.ORANGE, Raylib.PINK,
            Raylib.SKYBLUE, Raylib.BROWN, Raylib.GRAY, Raylib.WHITE
        };
        Raylib_CsLo.Color[] result = new Raylib_CsLo.Color[count];
        for (int i = 0; i < count; i++)
            result[i] = allColors[i % allColors.Length];
        return result;
    }

    private string[] GenerateColorNames(int count)
    {
        string[] allNames = {
            "Rouge", "Bleu", "Vert", "Noir",
            "Jaune", "Violet", "Orange", "Rose",
            "Cyan", "Marron", "Gris", "Blanc"
        };
        string[] result = new string[count];
        for (int i = 0; i < count; i++)
            result[i] = allNames[i % allNames.Length];
        return result;
    }
}