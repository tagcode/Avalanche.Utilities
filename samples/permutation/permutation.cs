using System;
using Avalanche.Utilities;

public class permutation
{
    // Rename to "Main", or run from Main.
    public static void Run(string[] args)
    {
        {
            PermutationSetup permutation = new PermutationSetup();

            // Add property "Color"
            permutation.Add("Color", "Red");
            permutation.Add("Color", "Green");
            permutation.Add("Color", "Blue");

            // Add property "Number"
            permutation.Add("Number", "10");
            permutation.Add("Number", "100");
            permutation.Add("Number", "1000");

            // Add property "Size"
            permutation.Add("Size", "Small");
            permutation.Add("Size", "Medium");
            permutation.Add("Size", "Large");

            foreach (Scenario scenario in permutation.Scenarios)
            {
                string color = scenario["Color"]!.Name;
                string number = scenario["Number"]!.Name;
                string size = scenario["Size"]!.Name;

                Console.WriteLine($"Color={color}, Number={number}, Size={size}");
            }
        }

        {
            PermutationSetup permutation = new PermutationSetup();

            // Add property "Color"
            permutation.Add("Color", "Red", null, run => Color.Red, run => { /*finalizer here*/ });
            permutation.Add("Color", "Green", null, run => Color.Green);
            permutation.Add("Color", "Blue", null, run => Color.Blue);

            // Add property "Number"
            permutation.Add("Number", "10", null, run => 10);
            permutation.Add("Number", "100", null, run => 100);
            permutation.Add("Number", "1000", null, run => 1000);

            // Add property "Size"
            permutation.Add("Size", "Small", null, run => Size.Small);
            permutation.Add("Size", "Medium", null, run => Size.Medium);
            permutation.Add("Size", "Large", null, run => Size.Large);

            foreach (Scenario scenario in permutation.Scenarios)
            {
                using (Run run = scenario.Run().Initialize())
                {
                    Color color = (Color)run["Color"]!;
                    int number = (int)run["Number"]!;
                    Size size = (Size)run["Size"]!;

                    Console.WriteLine($"Color={color}, Number={number}, Size={size}");
                }
            }
        }

    }

    enum Color { Red, Green, Blue }
    enum Size { Small, Medium, Large }
}


