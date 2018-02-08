﻿using System;
using System.Globalization;

namespace Trabo
{
    internal class Program
    {
        private static Bot bot;

        private static void Main(string[] args)
        {
            // To avoid decimal parse issues
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

            bot = new Bot();
            bot.Updated += BotOnUpdated;
            bot.Start();

            Console.ReadLine();
        }

        private static void BotOnUpdated()
        {
            if (bot.DeltaWithComission > 1m)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if (bot.DeltaWithComission > 1.5m)
                Console.ForegroundColor = ConsoleColor.Green;
                
            Console.WriteLine($"{DateTime.Now} - SMA[{bot.MovingAverage.Count}] = {bot.MovingAverage.Mean:F5} BIDASK = {bot.LastOrderbook.Bids[0].Price:F4}/{bot.LastOrderbook.Asks[0].Price:F4} DELTA = {bot.Delta:F1} DELTAcom = {bot.DeltaWithComission:F1}");

            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}