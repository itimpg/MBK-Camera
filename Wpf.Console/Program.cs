using System;
using System.IO;
using System.Linq;
using static Mbk.Helper.Converter;
using static System.Console;

namespace Wpf.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] texts = File.ReadAllText(@"D:\mbk\counting.txt")
                 .Split(new[] { "--myboundary" }, StringSplitOptions.RemoveEmptyEntries);

            var countings =
                texts.Select(x => x.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(x => x.Length > 1)
                    .Select(x => new 
                    {
                        Gmt = ConvertToTime(x[3].Split(',')[5]),
                        DateTime = ConvertToDateTime(string.Join(" ", x[3].Split(',').Take(2))),
                        RawData = string.Join(Environment.NewLine, x.Skip(4)),
                        Population = x.Skip(4).Sum(a =>
                        {
                            var items = a.Split(',').Select(decimal.Parse).ToArray();
                            return items[4] - items[5];
                        })
                    }).ToArray();

            ReadLine();
        }
    }
}
