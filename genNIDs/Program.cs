using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace genNIDs {
    class Program {
        /// <summary>
        /// Wrie a message to the user and exit.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">Arguments if any.</param>
        static void Exit(string message, params object[] args) {
            if (args != null) Console.WriteLine(message, args);
            else Console.WriteLine(message);
            Console.ReadLine();
            Environment.Exit(0);
        }

        static void Main(string[] args) {
            Console.WriteLine("TT  Generate NIDs Tool  TT\nusage: genNIDs <YOUR_FUNCTION_OFFSETS_FILE>\n");
            if (args.Length > 1) Exit("To many Arguments !");
            if (args.Length <= 0) Exit("To less Arguments !");
            if (!args[0].Contains(".txt")) Exit("Please feed me with a text file.");

            try {
                FileInfo fi = new FileInfo(args[0]);
                if (fi.Length > 5242880) Exit("File to long !");

                string currDir = Directory.GetCurrentDirectory();
                if (File.Exists(currDir + @"\conv_nids.txt")) File.Delete(currDir + @"\conv_nids.txt");
                File.Create(currDir + @"\conv_nids.txt").Close();

                string[] toConvert = File.ReadAllLines(args[0]);
                if (!toConvert[0].Contains("imagebase==")) Exit("Can't find imagebase offset !\nPlace 'imagebase==OFFSET_OF_IMAGEBASE' into the first line of your text file.");

                long kernbase = 0;
                if (!long.TryParse(toConvert[0].Replace("imagebase==", ""), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out kernbase)) Exit("Can't parse imagebase !");

                foreach (string line in toConvert) {
                    if (line.Contains(":")) {
                        long nid = 0;
                        string[] split = line.Split(':');
                        if (split.Length != 2) {
                            Console.WriteLine("Something went wrong !\nThis line resulted into to much variables.");
                            Console.WriteLine("Will skip this one.");
                        } else {
                            if (!long.TryParse(split[1], NumberStyles.HexNumber, CultureInfo.CurrentCulture, out nid)) Console.WriteLine("Couldn't parse offset {0}, will skip {1}", split[1], split[0]);
                            else File.AppendAllLines(currDir + @"\conv_nids.txt", new string[] { "#define " + split[0] + " 0x" + (nid - kernbase).ToString("X2") });
                        }
                    }
                }
                Exit("Done !\nCheck your nid file.\nby yours cfwprpht");
            } catch (IOException io) { Exit("Error: Some IO Exception happend !\n" + io.ToString()); }

        }
    }
}
