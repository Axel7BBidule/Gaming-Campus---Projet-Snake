using Raylib_CsLo;
using System.Numerics;

public static class Menu
{
    public static void Main()
    {
        Raylib.InitWindow(800, 600, "Snake Memory");
        Raylib.SetTargetFPS(60);
        Raylib.InitAudioDevice();
        
        var songManager = new SongManager(
            SongManager.DefaultAppleSoundPath,
            SongManager.DefaultClickSoundPath,
            SongManager.DefaultThemeMusicPath
        );
        ((SongManager)songManager).PlayTheme();
        songManager.SetThemeVolume(0.2f);

        int state = 0;
        float fade = 0f;
        float fadeTimer = 0f;
        int selected = 0;
        int difficultySelected = 0;
        int colorCount = 4;


        SnakeGame? snakeGame = null;

        while (!Raylib.WindowShouldClose())
        {
            ((SongManager)songManager).UpdateTheme();
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.RAYWHITE);


            switch (state)
            {
                case 0: // Debut Fondu
                    fade += 0.01f;
                    Raylib.DrawText("Projet Axel CANUET", 300, 280, 20, Raylib.Fade(Raylib.DARKGRAY, fade));
                    if (fade >= 1f)
                    {
                        state = 1;
                        fadeTimer = 0f;
                    }
                    break;

                case 1: // Fin Fondu
                    fadeTimer += Raylib.GetFrameTime();
                    Raylib.DrawText("Projet Axel CANUET", 300, 280, 20, Raylib.DARKGRAY);
                    if (fadeTimer >= 3f)
                        state = 2;
                    break;

                case 2: // Menu Principal
                    Raylib.DrawText("Snake Memory", 280, 100, 60, Raylib.DARKGRAY);
                    Raylib.DrawText("Menu Principal", 280, 200, 30, Raylib.DARKGRAY);

                    
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) selected = (selected + 1) % 2;
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) selected = (selected + 1) % 2;

                    Color playColor = selected == 0 ? Raylib.RED : Raylib.DARKGRAY;
                    Color quitColor = selected == 1 ? Raylib.RED : Raylib.DARKGRAY;

                    Raylib.DrawText("Jouer", 370, 270, 24, playColor);
                    Raylib.DrawText("Quitter", 370, 320, 24, quitColor);

                    if ((Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER) && selected == 0))
                    {
                        songManager.PlayClickSound();
                        state = 3;
                    }

                    if ((Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER) && selected == 1))
                    {
                        songManager.PlayClickSound();
                        ((SongManager)songManager).StopTheme();
                        songManager.Unload();
                        Raylib.CloseAudioDevice();
                        Raylib.CloseWindow();
                        return;
                    }
                    break;

                case 3: // Choix difficulté
                    Raylib.DrawText("Choisissez la difficulté", 270, 180, 32, Raylib.DARKGRAY);

                    
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) difficultySelected = (difficultySelected + 1) % 3;
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) difficultySelected = (difficultySelected + 2) % 3;

                    Color easyColor = difficultySelected == 0 ? Raylib.RED : Raylib.DARKGRAY;
                    Color mediumColor = difficultySelected == 1 ? Raylib.RED : Raylib.DARKGRAY;
                    Color hardColor = difficultySelected == 2 ? Raylib.RED : Raylib.DARKGRAY;

                    Raylib.DrawText("Facile (4 couleurs)", 330, 260, 24, easyColor);
                    Raylib.DrawText("Moyen (8 couleurs)", 330, 320, 24, mediumColor);
                    Raylib.DrawText("Difficile (12 couleurs)", 330, 380, 24, hardColor);

                    bool selectAction = Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER);

                    if (selectAction)
                    {
                        songManager.PlayClickSound();

                        if (difficultySelected == 0)
                            colorCount = 4;
                        else if (difficultySelected == 1)
                            colorCount = 8;
                        else
                            colorCount = 12;

                        snakeGame = new SnakeGame(colorCount, songManager);
                        state = 4;
                    }

                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))
                        state = 2;
                    break;

                case 4: // Lancement du jeu
                    snakeGame?.Update();
                    snakeGame?.Draw();

                    if (snakeGame != null)
                    {
                        if (snakeGame.State == GameState.Restart)
                            snakeGame.Reset();
                        else if (snakeGame.State == GameState.Menu)
                        {
                            state = 2;
                            snakeGame = null;
                            songManager.PlayClickSound();
                        }
                    }
                    break;
            }

            Raylib.EndDrawing();
        }

        ((SongManager)songManager).StopTheme();
        songManager.Unload();
        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }
}