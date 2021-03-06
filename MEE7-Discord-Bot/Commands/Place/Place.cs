﻿using Discord;
using MEE7.Backend;
using MEE7.Backend.HelperFunctions;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MEE7.Commands
{
    public class Place : Command
    {
        readonly string filePath = $"Commands{Path.DirectorySeparatorChar}Place{Path.DirectorySeparatorChar}place.png";
        const int placeSize = 500;
        const int pixelSize = 10;
        readonly PlaceCommand[] subCommands;

        public Place() : base("place", "Basically just r/place", false, true)
        {
            subCommands = new PlaceCommand[] {
            new PlaceCommand("print", "Prints the canvas without this annoying help message.",
                (string[] split, IMessageChannel Channel) => { return true; },
                (IMessage commandmessage, string filePath, string[] split) => { DiscordNETWrapper.SendFile(filePath, commandmessage.Channel).Wait(); }),
            new PlaceCommand("drawPixel", "Draws the specified color to the specified place(0 - " + (placeSize / pixelSize - 1) + ", 0 - " + (placeSize / pixelSize - 1) +
            ")\neg. " + Prefix + CommandLine + " drawPixel 10,45 Red",
                (string[] split, IMessageChannel Channel) => {

                    int X, Y;
                    if (split.Length != 4)
                    {
                        DiscordNETWrapper.SendText("I need 3 arguments to draw!", Channel).Wait();
                        return false;
                    }

                    try
                    {
                        string[] temp = split[2].Split(',');
                        X = Convert.ToInt32(temp[0]);
                        Y = Convert.ToInt32(temp[1]);
                    }
                    catch
                    {
                        DiscordNETWrapper.SendText("I don't understand those coordinates, fam!", Channel).Wait();
                        return false;
                    }

                    if (X >= (placeSize / pixelSize) || Y >= (placeSize / pixelSize))
                    {
                        DiscordNETWrapper.SendText("The picture is only " + (placeSize / pixelSize) + "x" + (placeSize / pixelSize) + " big!\nTry smaller coordinates.", Channel).Wait();
                        return false;
                    }

                    System.Drawing.Color brushColor = System.Drawing.Color.FromName(split[3]);

                    if (brushColor.R == 0 && brushColor.G == 0 && brushColor.B == 0 && split[3].ToLower() != "black")
                    {
                        DiscordNETWrapper.SendText("I dont think I know that color :thinking:", Channel).Wait();
                        return false;
                    }

                    return true;

                },
                (IMessage commandmessage, string filePath, string[] split) => {

                    string[] temps = split[2].Split(',');
                    int X = Convert.ToInt32(temps[0]);
                    int Y = Convert.ToInt32(temps[1]);

                    Bitmap temp;
                    System.Drawing.Color brushColor = System.Drawing.Color.FromName(split[3]);
                    using (FileStream stream = new FileStream(filePath, FileMode.Open))
                        temp = (Bitmap)Bitmap.FromStream(stream);

                    using (Graphics graphics = Graphics.FromImage(temp))
                    {
                        graphics.FillRectangle(new SolidBrush(brushColor), new Rectangle(X * pixelSize, Y * pixelSize, pixelSize, pixelSize));
                    }

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        temp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                    DiscordNETWrapper.SendFile(filePath, commandmessage.Channel, "Succsessfully drawn!").Wait();
                }),
            new PlaceCommand("drawCircle", "Draws a circle in some color, in the given size and in the given coordinates(0 - " + (placeSize - 1) + ", 0 - " + (placeSize - 1) +
            ")\neg. " + Prefix + CommandLine + " drawCircle 100,450 Red 25",
                (string[] split, IMessageChannel Channel) => {

                    int X, Y, S;
                    if (split.Length != 5)
                    {
                        DiscordNETWrapper.SendText("I need 4 arguments to draw!", Channel).Wait();
                        return false;
                    }

                    try
                    {
                        string[] temp = split[2].Split(',');
                        X = Convert.ToInt32(temp[0]);
                        Y = Convert.ToInt32(temp[1]);
                    }
                    catch
                    {
                        DiscordNETWrapper.SendText("I don't understand those coordinates, fam!", Channel).Wait();
                        return false;
                    }

                    //if (X >= placeSize || Y >= placeSize)
                    //{
                    //    DiscordNETWrapper.SendText("The picture is only " + placeSize + "x" + placeSize + " big!\nTry smaller coordinates.", Channel);
                    //    return false;
                    //}

                    System.Drawing.Color brushColor = System.Drawing.Color.FromName(split[3]);

                    if (brushColor.R == 0 && brushColor.G == 0 && brushColor.B == 0 && split[3].ToLower() != "black")
                    {
                        DiscordNETWrapper.SendText("I dont think I know that color :thinking:", Channel).Wait();
                        return false;
                    }

                    try
                    {
                        S = Convert.ToInt32(split[4]);
                    }
                    catch
                    {
                        DiscordNETWrapper.SendText("I don't understand that size, fam!", Channel).Wait();
                        return false;
                    }

                    if (S > 100)
                    {
                        DiscordNETWrapper.SendText("Thats a little big, don't ya think?", Channel).Wait();
                        return false;
                    }

                    return true;

                },
                (IMessage commandmessage, string filePath, string[] split) => {

                    string[] temps = split[2].Split(',');
                    int X = Convert.ToInt32(temps[0]);
                    int Y = Convert.ToInt32(temps[1]);

                    int S = Convert.ToInt32(split[4]);

                    Bitmap temp;
                    System.Drawing.Color brushColor = System.Drawing.Color.FromName(split[3]);
                    using (FileStream stream = new FileStream(filePath, FileMode.Open))
                        temp = (Bitmap)Bitmap.FromStream(stream);

                    using (Graphics graphics = Graphics.FromImage(temp))
                    {
                        graphics.FillPie(new SolidBrush(brushColor), new Rectangle(X - S, Y - S, S * 2, S * 2), 0, 360);
                    }

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        temp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                    DiscordNETWrapper.SendFile(filePath, commandmessage.Channel, "Succsessfully drawn!").Wait();

                }),
            new PlaceCommand("drawRekt", "Draws a rectangle in some color and in the given coordinates(0 - " + (placeSize - 1) + ", 0 - " + (placeSize - 1) +
            ") and size\neg. " + Prefix + CommandLine + " drawRekt 100,250 Red 200,100",
                (string[] split, IMessageChannel Channel) => {

                    int X, Y, W, H;
                    if (split.Length < 5)
                    {
                        DiscordNETWrapper.SendText("I need 4 arguments to draw!", Channel).Wait();
                        return false;
                    }

                    try
                    {
                        string[] temp = split[2].Split(',');
                        X = Convert.ToInt32(temp[0]);
                        Y = Convert.ToInt32(temp[1]);
                    }
                    catch
                    {
                        DiscordNETWrapper.SendText("I don't understand those coordinates, fam!", Channel).Wait();
                        return false;
                    }

                    try
                    {
                        string[] temp = split[4].Split(',');
                        W = Convert.ToInt32(temp[0]);
                        H = Convert.ToInt32(temp[1]);
                    }
                    catch
                    {
                        DiscordNETWrapper.SendText("I don't understand that size, fam!", Channel).Wait();
                        return false;
                    }

                    if (W + H > 500)
                    {
                        DiscordNETWrapper.SendText("Thats a little big, don't ya think?", Channel).Wait();
                        return false;
                    }

                    System.Drawing.Color brushColor = System.Drawing.Color.FromName(split[3]);

                    if (brushColor.R == 0 && brushColor.G == 0 && brushColor.B == 0 && split[3].ToLower() != "black")
                    {
                        DiscordNETWrapper.SendText("I dont think I know that color :thinking:", Channel).Wait();
                        return false;
                    }

                    return true;

                },
                (IMessage commandmessage, string filePath, string[] split) => {

                    string[] temps = split[2].Split(',');
                    int X = Convert.ToInt32(temps[0]);
                    int Y = Convert.ToInt32(temps[1]);
                    temps = split[4].Split(',');
                    int W = Convert.ToInt32(temps[0]);
                    int H = Convert.ToInt32(temps[1]);

                    Bitmap temp;
                    System.Drawing.Color brushColor = System.Drawing.Color.FromName(split[3]);
                    using (FileStream stream = new FileStream(filePath, FileMode.Open))
                        temp = (Bitmap)Bitmap.FromStream(stream);

                    using (Graphics graphics = Graphics.FromImage(temp))
                    {
                        graphics.FillRectangle(new SolidBrush(brushColor), new Rectangle(X, Y, W, H));
                    }

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        temp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                    DiscordNETWrapper.SendFile(filePath, commandmessage.Channel, "Succsessfully drawn!").Wait();

                }),
            new PlaceCommand("drawString", "Draws a string in some color and in the given coordinates(0 - " + (placeSize - 1) + ", 0 - " + (placeSize - 1) +
            ")\neg. " + Prefix + CommandLine + " drawString 100,250 Red OwO what dis",
                (string[] split, IMessageChannel Channel) => {

                    int X, Y;
                    if (split.Length < 5)
                    {
                        DiscordNETWrapper.SendText("I need 4 arguments to draw!", Channel).Wait();
                        return false;
                    }

                    try
                    {
                        string[] temp = split[2].Split(',');
                        X = Convert.ToInt32(temp[0]);
                        Y = Convert.ToInt32(temp[1]);
                    }
                    catch
                    {
                        DiscordNETWrapper.SendText("I don't understand those coordinates, fam!", Channel).Wait();
                        return false;
                    }

                    //if (X >= placeSize || Y >= placeSize)
                    //{
                    //    DiscordNETWrapper.SendText("The picture is only " + placeSize + "x" + placeSize + " big!\nTry smaller coordinates.", Channel);
                    //    return false;
                    //}

                    System.Drawing.Color brushColor = System.Drawing.Color.FromName(split[3]);

                    if (brushColor.R == 0 && brushColor.G == 0 && brushColor.B == 0 && split[3].ToLower() != "black")
                    {
                        DiscordNETWrapper.SendText("I dont think I know that color :thinking:", Channel).Wait();
                        return false;
                    }

                    return true;

                },
                (IMessage commandmessage, string filePath, string[] split) => {

                    string[] temps = split[2].Split(',');
                    int X = Convert.ToInt32(temps[0]);
                    int Y = Convert.ToInt32(temps[1]);

                    Bitmap temp;
                    System.Drawing.Color brushColor = System.Drawing.Color.FromName(split[3]);
                    using (FileStream stream = new FileStream(filePath, FileMode.Open))
                        temp = (Bitmap)Bitmap.FromStream(stream);

                    using (Graphics graphics = Graphics.FromImage(temp))
                    {
                        graphics.DrawString(string.Join(" ", split.Skip(4).ToArray()), new Font("Comic Sans", 16), new SolidBrush(brushColor), new PointF(X, Y) );
                    }

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        temp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

                    DiscordNETWrapper.SendFile(filePath, commandmessage.Channel, "Succsessfully drawn!").Wait();

                }) };
        }

        class PlaceCommand
        {
            public delegate bool ConditionCheck(string[] split, IMessageChannel Channel);
            public delegate void Execution(IMessage commandmessage, string filePath, string[] split);
            public string command, desc;
            public ConditionCheck check;
            public Execution execute;

            public PlaceCommand(string command, string desc, ConditionCheck check, Execution execute)
            {
                this.command = command;
                this.desc = desc;
                this.check = check;
                this.execute = execute;
            }
        }

        public override void Execute(IMessage message)
        {
            string[] split = message.Content.Split(new char[] { ' ', '\n' });
            if (split.Length == 1)
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithColor(0, 128, 255);
                foreach (PlaceCommand Pcommand in subCommands)
                {
                    Embed.AddFieldDirectly(Prefix + CommandLine + " " + Pcommand.command, Pcommand.desc);
                }
                Embed.WithDescription("Place Commands:");
                DiscordNETWrapper.SendEmbed(Embed, message.Channel).Wait();

                DiscordNETWrapper.SendFile(filePath, message.Channel).Wait();
            }
            else
            {
                foreach (PlaceCommand Pcommand in subCommands)
                {
                    if (split[1] == Pcommand.command && Pcommand.check(split, message.Channel))
                    {
                        Pcommand.execute(message, filePath, split);
                        break;
                    }
                }
            }
        }
    }
}
