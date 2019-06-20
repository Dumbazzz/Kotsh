using System;
using System.Drawing;
using System.Linq;
using ColorConsole = Colorful.Console;

namespace Kotsh.Modules.Console
{
    /// <summary>
    /// Console class allows to push messages in the console
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Core instance
        /// </summary>
        private Manager core;

        /// <summary>
        /// Store the core instance
        /// </summary>
        /// <param name="core">Kotsh instance</param>
        public Input(Manager core)
        {
            // Store the core
            this.core = core;
        }

        /// <summary>
        /// Ask a question to get a string
        /// </summary>
        /// <param name="message">Question/Value to ask</param>
        /// <returns>User response as String</returns>
        public String AskString(String message)
        {
            // Display header
            ColorConsole.Write(core.Console.GetHeader("INPUT"), Color.Orange);

            // Display question
            ColorConsole.Write(message + " ", Color.WhiteSmoke);

            // Return result
            return System.Console.ReadLine();
        }

        /// <summary>
        /// Ask a question to get a number
        /// </summary>
        /// <param name="message">Question/Value to ask</param>
        /// <returns>User response as int</returns>
        private int AskInteger(String message)
        {
            // Display header
            ColorConsole.Write(core.Console.GetHeader("INPUT"), Color.OrangeRed);

            // Display question
            ColorConsole.Write(message + " ", Color.WhiteSmoke);

            // Get result
            String result = System.Console.ReadLine();

            // Return parsed int
            return int.Parse(result);
        }

        public int AskIntegerWithLimit(String message, int min, int max)
        {
            // Ask integer
            int number = this.AskInteger(message);

            // Check number
            while (!Enumerable.Range(min, max).Contains(number))
            {
                number = this.AskInteger(message);
            }

            // Return filtered integer
            return number;
        }


        /// <summary>
        /// Display a list with possibles choices
        /// This is a helper method for askChoice()
        /// </summary>
        /// <param name="message">Message to introduce choices</param>
        /// <param name="choices">List of possible choices</param>
        private void DisplayChoices(String message, String[] choices)
        {
            // Display header
            ColorConsole.Write(core.Console.GetHeader("INPUT"), Color.DarkOrange);

            // Display question
            ColorConsole.WriteLine(message, Color.WhiteSmoke);

            // Display all choices
            for (int i = 0; i < choices.Length; i++)
            {
                ColorConsole.WriteLine(" | " + i + " -> " + choices[i], Color.Gray);
            }
        }

        /// <summary>
        /// Display choices (using displayChoices() helper) and ask for a choice
        /// </summary>
        /// <param name="message">Message to introduce choices</param>
        /// <param name="choices">List of possible choices</param>
        /// <returns>String with choice</returns>
        public String AskChoice(String message, String[] choices)
        {
            // Display choices
            this.DisplayChoices(message, choices);

            // Get response
            int response = this.AskInteger("Type your choice:");

            // Check result
            while (response > choices.Length || response < 0)
            {
                // Ask question
                response = this.AskInteger("Wrong choice, type another choice:");
            }

            // Return response
            return choices[response];
        }
    }
}
