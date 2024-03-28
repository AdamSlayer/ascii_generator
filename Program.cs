using System.Drawing;

#nullable disable warnings // 'x is not null here'
#pragma warning disable CA1416 // 'x is only supported on windows'


class Program
{
    static void Main()
    {
        //Console.WriteLine(TwoClosestConsoleColors(Color.FromArgb(255,0,255,127)));
        Console.WriteLine("Press any key to continue\n\n");
        Console.ReadKey();

        string imagePath = "../obama.jpg";
        int width = Console.WindowWidth - 2;
        bool frame = false;

            FileStream stream = new FileStream(imagePath, FileMode.Open);
            Image originalImage = Image.FromStream(stream);

            float ratio = (float)originalImage.Width / (float)originalImage.Height;
            int height = (int)(width / 2 / ratio);

            Bitmap bmp = new Bitmap(originalImage, new Size(width, height));

            string result = "";

            // add the top of the frame
            if (frame)
            {
                result += "1";
                for (int i = 0; i < width; i++) { result += "-"; }
                result += "2";
                result += "\n";
            }

            for (int y = 0; y < height; y++)
            {
                // left part of the frame
                if (frame) { result += "|"; }
                for (int x = 0; x < width; x++)
                {
                    //char character = ColorToChar(bmp.GetPixel(x, y));
                    char character = ColorToCharWithConsoleColor(bmp.GetPixel(x, y));
                    Console.Write(character); // Print character immediately after color is set
                    result += character;
                }
                // right part of the frame
                if (frame) { result += "|"; }
                result += "\n";
                Console.Write("\n");
            }

            // Reset console color after printing
            Console.ResetColor();

            // add the bottom of the frame
            if (frame)
            {
                result += "3";
                for (int i = 0; i < width; i++) { result += "-"; }
                result += "4";
                result += "\n";
            }
            Console.WriteLine(result); // Print result string to console
            Console.WriteLine($"Image size: {width}x{height}");



    }

    static (ConsoleColor, ConsoleColor, int) TwoClosestConsoleColors(Color inputColor)
    {
        // Ignore alpha channel
        inputColor = Color.FromArgb(inputColor.R, inputColor.G, inputColor.B);

        // Define the array of ConsoleColor values
        ConsoleColor[] consoleColors = (ConsoleColor[])Enum.GetValues(typeof(ConsoleColor));
        var colorTuples = new List<(Color, ConsoleColor)>();
        foreach (ConsoleColor consoleColor in consoleColors)
        {
            // Convert ConsoleColor to Color
            colorTuples.Add((Color.FromName(consoleColor.ToString()), consoleColor));
        }

        ConsoleColor closestColor1 = ConsoleColor.Black;
        ConsoleColor closestColor2 = ConsoleColor.Black;
        int bestI = 0;
        double closestDistance = double.MaxValue;

        // Loop through each ConsoleColor value
        for (int i = 0; i < 15; i++)
        {
            var firstColorStrength = 17 * i; // second color strength is 255 - firstColorStrength
            foreach (var colorTuple1 in colorTuples)
            {
                foreach (var colorTuple2 in colorTuples)
                {
                    var color1Rgb = colorTuple1.Item1;
                    var color2Rgb = colorTuple2.Item1;
                    Color totalColor = Color.FromArgb(255,
                        (color1Rgb.R * firstColorStrength + color2Rgb.R * (255 - firstColorStrength)) / 255,
                        (color1Rgb.G * firstColorStrength + color2Rgb.G * (255 - firstColorStrength)) / 255,
                        (color1Rgb.B * firstColorStrength + color2Rgb.B * (255 - firstColorStrength)) / 255
                    );
                    // Calculate the distance between the two colors
                    double distance = Math.Sqrt(Math.Pow((float)(inputColor.R - totalColor.R) * 1.0, 2) +
                                                Math.Pow((float)(inputColor.G - totalColor.G) * 1.3, 2) +
                                                Math.Pow((float)(inputColor.B - totalColor.B) * 0.7, 2));

                    // Update closestColors if this color is closer than the current closest color
                    if (distance < closestDistance)
                    {
                        closestColor1 = colorTuple1.Item2;
                        closestColor2 = colorTuple2.Item2;
                        bestI = i;
                        closestDistance = distance;
                    }
                }
            }
        }

        return (closestColor1, closestColor2, bestI);
    }

    static char ColorToChar(Color color)
    {
        // Calculate grayscale value
        double grayscale = (0.3 * color.R + 0.55 * color.G + 0.15 * color.B);

        // Map the grayscale value to a character
        string chars = " .:+lj%@";
        int index = (int)(grayscale * ((float)chars.Length - 1.0) / 255.0);
        return chars[index = Math.Max(0, Math.Min(index, chars.Length - 1))];
    }

    static char ColorToCharWithConsoleColor(Color color)
    {
        var result = TwoClosestConsoleColors(color);
        if (result.Item3 < 8)
        {
            Console.ForegroundColor = result.Item1;
            Console.BackgroundColor = result.Item2;
            return " .:+lj%@"[result.Item3];
        }
        else
        {
            {
                Console.ForegroundColor = result.Item2;
                Console.BackgroundColor = result.Item1;
                return " .:+lj%@"[15 - result.Item3];
            }
        }
    }
}
