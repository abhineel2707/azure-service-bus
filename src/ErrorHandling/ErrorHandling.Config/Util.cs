﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ErrorHandling.Config
{
    public static class Utils
    {
        public static void WriteLine(string text,ConsoleColor color)
        {
            var tempColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = tempColor;
        }
    }
}
