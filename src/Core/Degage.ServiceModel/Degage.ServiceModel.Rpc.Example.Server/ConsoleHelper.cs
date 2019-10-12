using System;
using System.Collections.Generic;
using System.Text;

namespace Degage.ServiceModel.Rpc.Example
{
    public static class ConsoleHelper
    {
        private static ConsoleColor _OldForeColor = Console.ForegroundColor;
        public static void SetForeColor(ConsoleColor color)
        {
            _OldForeColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }
        public static void RecoverForeColor()
        {

            Console.ForegroundColor = _OldForeColor;
        }
        public static void NewLine()
        {
            Console.WriteLine("-----------------------------------------------------------------------------------");
        }
        public static void ShowTextInfo(String text, ConsoleColor color = ConsoleColor.White)
        {
            SetForeColor(color);
            Console.WriteLine(text);
            RecoverForeColor();
        }
        public static void ShowPlayerInfo(PlayerInfo info, ConsoleColor color = ConsoleColor.Green)
        {
            var text = $"\tName:{info.Name} , Age:{info.Age}";
            ShowTextInfo(text, color);
        }
    }
}
