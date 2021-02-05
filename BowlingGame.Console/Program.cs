using System;
using BowlingGame.Domain;

namespace BowlingGame.Console
{
    internal class Program
    {
        private static void Main()
        {
            Game game = null;
            WriteLine_Instructions("Welcome to Michael Nikiforos' Bowling Game!");
            string input;
            do
            {
                WriteLine_Instructions("Press the key...\t\t To...");
                WriteLine_Instructions("\t[N]\t\t\t Start a new Game");
                //WriteLine_Instructions("\t[R]\t\t\t Roll the ball ");
                //WriteLine_Instructions("\t[S]\t\t\t Show your score ");
                WriteLine_Instructions("\t[X]\t\t\t Exit the game ");
                Write_Instructions("\nEnter selection -> ");
                input = System.Console.ReadKey().KeyChar.ToString().ToUpper();
                WriteLine_Instructions("\n");
                try
                {
                    switch (input)
                    {
                        case "N":
                            WriteLine_ResponseToInput("You pressed N to start a new game.");
                            StartNewGame(ref game);
                            //break;
                            //case "R":
                            //WriteLine_ResponseToInput("You pressed R to take your turn and roll the ball.");
                            //if (game == null) throw new InvalidGameState("You haven't started a game yet!");
                            PlayGame(game);
                            //case "S":
                            //    WriteLine_ResponseToInput("You pressed S to show your score.");
                            //    ShowScore(game);
                            break;
                        case "X":
                            WriteLine_ResponseToInput("You pressed  X to exit the game.");
                            break;
                        default:
                            WriteLine_ResponseToInput("You chose an invalid selection. Please try again.");
                            break;
                    }
                }
                catch (InvalidRollState rex)
                {
                    WriteLine_ValidationError(
                        $"Oops! You tried to test the limits of this game, but this validation error was caught!\nThe error is: {rex.Message}\n\n");
                }
                catch (InvalidGameState gex)
                {
                    WriteLine_ValidationError(
                        $"Oops! You tried to test the limits of this game, but this validation error was caught!\nThe error is: {gex.Message}\n\n");
                }
                catch (InvalidFrameState gex)
                {
                    WriteLine_ValidationError(
                        $"Oops! You tried to test the limits of this frame, but this validation error was caught!\nThe error is: {gex.Message}\n\n");
                }
                catch
                {
                    WriteLine_ValidationError("An unexpected error has occurred. Game will terminate");
                    throw;
                }
            } while (input != "X");
        }

        private static void PlayGame(Game game)
        {
            Random r = new Random();
            do
            {
                try
                {
                    Write_Instructions($"Enter the number of pins knock downed down for Frame #{ game.CurrentFrame.FrameNumber}, Roll #{game.CurrentFrame.Rolls.Count + 1} or press enter for random number ->");
                    var rollInput = System.Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(rollInput))
                    {
                        game.Roll(r.Next(0, game.CurrentFrame.NumOfRemainingPins + 1));
                    }
                    else if (int.TryParse(rollInput, out var pinsKnockedOver))
                    {
                        game.Roll(pinsKnockedOver);
                        //ShowScore(game);
                    }
                    else
                    { WriteLine_ValidationError("You did not add a valid number. \n");}
                }

                catch (InvalidRollState rex)
                {
                    WriteLine_ValidationError($"{rex.Message}\n");
                }
                catch (InvalidGameState gex)
                {
                    WriteLine_ValidationError($"{gex.Message}\n");
                }
                catch (InvalidFrameState gex)
                {
                    WriteLine_ValidationError($"{gex.Message}\n");
                }
            } while (!game.IsGameCompleted);

        }

        private static void WriteLine_ValidationError(string v)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(v);
        }

        private static void WriteLine_ResponseToInput(string v)
        {
            System.Console.ForegroundColor = ConsoleColor.Blue;
            System.Console.WriteLine(v);
        }

        private static void WriteLine_Instructions(string s)
        {
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine(s);
        }
        private static void Write_Instructions(string s)
        {
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.Write(s);
        }
        private static void StartNewGame(ref Game game)
        {
            game =
                GameBuilder.Create()
                    .ConfigureGameEventResponses((g, gameEvent) =>
                    {
                        switch (gameEvent)
                        {
                            case StartedGameEvent _:
                                WriteLine_ResponseToEvent("You started a new game. Have fun!");
                                break;

                            case AdvanceFrameGameEvent x:
                                WriteLine_ResponseToEvent(
                                    $"The frame is advancing from frame {x.FrameNumber} to frame {x.NewFrameNumber} .");
                                break;

                            case EndGameEvent _:
                                WriteLine_ResponseToEvent("The game has ended. Below is your final score.");
                                ShowScore(g);
                                WriteLine_Instructions("");
                                break;
                        }
                    })
                    .ConfigureFrameEventResponses((g, frameEvent) =>
                    {
                        switch (frameEvent)
                        {
                            case StartOfFirstRollFrameEvent x:
                                //System.Console.WriteLine($"You start frame {x.FrameNumber} with a clean slate.");
                                break;

                            case StartOfSecondRollFrameEvent _ when g.CurrentFrame.FrameNumber < 10:
                                WriteLine_ResponseToEvent(
                                    "You didn't get a strike on your first roll, but you hope to make up for it by getting a spare.");
                                break;
                            case StartOfThirdRollFrameEvent _:
                                WriteLine_ResponseToEvent(
                                    "You managed to get a third roll in the last frame.  Make this one count!");
                                break;
                            case RollFrameEvent x:
                                WriteLine_ResponseToEvent(
                                    "You grab the bowling ball, release the ball, and watch the ball hurdling towards the pins...");
                                System.Console.WriteLine(x.PinsKnockedOver == 0
                                    ? "Gutter ball! That was tough luck"
                                    : $"You manage to knock {x.PinsKnockedOver} pins down. Good job.");
                                break;

                            case StrikeFrameEvent _:
                                WriteLine_ResponseToEvent(
                                    "Wait a minute...was that a strike?!? Great job!");
                                break;

                            case SpareFrameEvent _:
                                WriteLine_ResponseToEvent("Yes! You managed to wrangle up a spare!");
                                break;

                            case EndFrameEvent x:
                                WriteLine_ResponseToEvent(
                                    $"Frame {x.FrameNumber} has ended with a total of {x.PinsKnockedOver} pins knocked over. Your score so far  is {g.GetScore()}");
                                break;
                        }

                        //System.Console.WriteLine();
                    })
                    .Build();
        }

        private static void WriteLine_ResponseToEvent(string v)
        {
            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine(v);
        }

        private static void ShowScore(Game game)
        {
            WriteLine_Instructions("Frame #\tRoll 1\tRoll 2\tRoll 3\tFrame Total");
            WriteLine_Instructions("=======\t======\t======\t======\t===========");
            game.Frames
                .ForEach(f =>
                {
                    Write_Instructions($"{f.FrameNumber}\t");
                    
                    switch (f.Rolls.Count)
                    {
                        case 1:
                            Write_Instructions($"{f.Rolls[0].PinsKnockedOver}\t\t\t\t{f.GetScore()}");
                            break;
                        case 2:
                            Write_Instructions($"{f.Rolls[0].PinsKnockedOver}\t{f.Rolls[1].PinsKnockedOver}\t\t\t{f.GetScore()}");
                            break;
                        case 3:
                            Write_Instructions($"{f.Rolls[0].PinsKnockedOver}\t{f.Rolls[1].PinsKnockedOver}\t{f.Rolls[1].PinsKnockedOver}\t\t{f.GetScore()}");
                            break;
                    }

                    WriteLine_Instructions("");
                });
            WriteLine_Instructions("===========================================");
            WriteLine_Instructions($"Total Game Score:\t\t\t{game.GetScore()}");

        }
    }
}