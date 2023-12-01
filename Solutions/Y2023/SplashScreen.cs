using AdventOfCode.Framework;
namespace AdventOfCode.Y2023;

class SplashScreenImpl : SplashScreen
{
    public override void Show()
    {
        WriteFiglet("Advent of code 2023", Spectre.Console.Color.Yellow);
        Write(0x886655, "                                                            \n                                       ");
        Write(0x886655, "                     \n                                                            \n                 ");
        Write(0x886655, "                                           \n                                                        ");
        Write(0x886655, "    \n                                                            \n                                  ");
        Write(0x886655, "                          \n                                                            \n            ");
        Write(0x886655, "                                                \n                                                   ");
        Write(0x886655, "         \n                                                            \n                             ");
        Write(0x886655, "                               \n                                                            \n       ");
        Write(0x886655, "                                                     \n                                              ");
        Write(0x886655, "              \n                                                            \n                        ");
        Write(0x886655, "                                    \n                                                            \n  ");
        Write(0x886655, "                                                          \n                                         ");
        Write(0x886655, "                   \n                                                            \n                   ");
        Write(0x886655, "                                         \n                                                          ");
        Write(0x886655, "  \n               ----@");
        Write(0x888888, "             *                             2\n             ");
        Write(0xffff66, "* ");
        Write(0x886655, "! /^\\                                          ");
        Write(0x888888, " 1 ");
        Write(0xffff66, "**\n           \n");
        
        Console.WriteLine();
    }
}