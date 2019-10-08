using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace lesson2409
{
    class Program
    {
        static void Main(string[] args)
        {
            // var str = "Иванов Иван Иванович";
            Console.WriteLine("Hello World!");
            var first_names = new List<string>();
            var last_names = new List<string>();
            var patr_names = new List<string>();


            using (StreamReader file = new StreamReader("test.txt"))
            {
                string line;
                while((line = file.ReadLine()) != null)
                {
                    first_names.Add(line.Split(' ')[1]);
                    last_names.Add(line.Split(' ')[0]);
                    patr_names.Add(line.Split(' ')[2]);
                    Console.WriteLine(line);
                    
                }
            }
            
            var students = CreateStudents(first_names.ToArray(), last_names.ToArray(), patr_names.ToArray(), 10);



            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;
            for(int i = 0; i < students.Length; i++)
            {
                var avg = students[i].AverageRating;
                if (avg < min) min = avg;
                if (avg > max) max = avg;
            }

            var delta = max - min;
            var min_threshold = min + 0.1*delta;
            var max_threshold = max - 0.1*delta;
            var last = new List<Student>();
            var best = new List<Student>();
            foreach (var stud in students)
            {
                var avg_rating = stud.AverageRating;
                if (avg_rating > max_threshold) 
                    best.Add(stud);
                else if (avg_rating < min_threshold)
                    last.Add(stud);
            }
            
        }

        static Student[] CreateStudents(string[] Names, string[] LastNames, string[] Patronimycs, int Count)
        {
            var result = new Student[Count];
            var rnd = new Random();
            for(int i=0; i<Count; i++)
            {
                // var first_name = Names[rnd.Next(0, Names.Length)];
                // var last_name = LastNames[rnd.Next(0, LastNames.Length)];
                // var patr_name = Patronimycs[rnd.Next(0, Patronimycs.Length)];
                result[i] = new Student(Names[rnd.Next(0, Names.Length)],
                                        LastNames[rnd.Next(0, LastNames.Length)],
                                        Patronimycs[rnd.Next(0, Patronimycs.Length)]);
                result[i].Rating = GetRandomRating(5, 2, 5);

                result[i].visualize();
                Console.WriteLine();
                
            }
            return result;
        }
        static double[] GetRandomRating(int num, double MinR, double MaxR)
        {
            var rnd = new Random();
            double[] ratings = new double[num];
            for(int i=0; i<num; i++)
            {
                ratings[i] = MinR + rnd.NextDouble()*(MaxR - MinR);
            }
            return ratings;
        }
    }



    class Student
    {
        public string FirstName;
        public string LastName;
        public string Patronimyc;

        public double[] Rating;

        public double AverageRating
        {
            get
            {
                if (Rating == null || Rating.Length == 0) return double.NaN;
                // double sum = 0;
                // for(int i = 0; i < Rating.Length; i++)
                //     sum += Rating[i];
                // return sum / Rating.Length;
                return Rating.Average();
            }
        }

        public Student(string f_name, string last_name, string patr)
        {
            FirstName = f_name;
            LastName = last_name;
            Patronimyc = patr;
        }
        public void visualize()
        {
            Console.WriteLine(FirstName + " " + Patronimyc + " " + LastName);
            Console.WriteLine("Ratings:");
            for (int i = 0; i < Rating.Length; i++)
            {
                Console.WriteLine(Rating[i]);
            }
            Console.WriteLine("Average Rating: " + AverageRating);
        }
    }




}
