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
            // формирование набора имен, фамилий и отчеств из файла
            var first_names = new List<string>();
            var last_names = new List<string>();
            var patr_names = new List<string>();
            using (StreamReader file = new StreamReader("test.txt"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    first_names.Add(line.Split(' ')[1]);
                    last_names.Add(line.Split(' ')[0]);
                    patr_names.Add(line.Split(' ')[2]);
                    Console.WriteLine(line);

                }
            }

            // создание массива студентов со случайными именами из заданного набора
            var students = CreateStudents(first_names.ToArray(), last_names.ToArray(), patr_names.ToArray(), 100);

            // Получение списков лучших и худших студентов(10% по умолчанию, второй параметр)
            var res = Student.GetBestAndLast(students);
            List<Student> last = res.Item1;
            List<Student> best = res.Item2;

            // Запись и чтение csv
            Student.WriteToCsv("students.csv", students);
            var students1 = Student.ReadCsv("students.csv");
            foreach (var stud in students1)
            {
                Console.WriteLine("----------------");
                stud.visualize();
            }

            // Запись студентов в Xml-файлы
            Student.WriteArrToXml(students1, "studs.xml", "");
            Student.WriteArrToXml(best.ToArray(), "best_studs.xml", "BestStudents");
            Student.WriteArrToXml(last.ToArray(), "last_studs.xml", "LastStudents");
            Student.WriteEachToXml(students1, "t");

            // Проверка функций чтения xml
            Console.WriteLine("---x-x-x--STUDENT1--x-x-x-x-");
            var student1 = Student.ReadStudentXml("t_1.xml");
            student1.visualize();
            var best_studs = Student.ReadStudentArrayXml("last_studs.xml", "LastStudents");
            Console.WriteLine("Худшие студенты:");
            foreach (var stud in best_studs)
            {
                Console.WriteLine("-----");
                stud.visualize();
            }
            Console.WriteLine("-------------------------------");
            SplitIntoGroups(students, 10);
            // Удаление Xml и csv
            ClearExt();
            ClearExt(".csv");

        }

        static void ClearExt(string extension=".xml")
        {
            foreach (string file in Directory.GetFiles(".", "*" + extension).Where(item => item.EndsWith(extension)))
            {
                Console.WriteLine(file);
                File.Delete(file);
            }
        }

        static Student[] CreateStudents(string[] Names, string[] LastNames, string[] Patronimycs, int Count)
        {
            var result = new Student[Count];
            var rnd = new Random();
            for (int i = 0; i < Count; i++)
            {
                result[i] = new Student(Names[rnd.Next(0, Names.Length)],
                                        LastNames[rnd.Next(0, LastNames.Length)],
                                        Patronimycs[rnd.Next(0, Patronimycs.Length)]);
                result[i].Rating = GetRandomRating(5, 2, 6);
            }
            return result;
        }
        
        static int[] GetRandomRating(int num, int MinR, int MaxR)
        {
            var rnd = new Random();
            int[] ratings = new int[num];
            for (int i = 0; i < num; i++)
            {
                ratings[i] = (int)(MinR + rnd.NextDouble() * (MaxR - MinR));
            }
            return ratings;
        }

        static void SplitIntoGroups(Student[] students, int num_groups)
        {
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;
            for (int i = 0; i < students.Length; i++)
            {
                var avg = students[i].AverageRating;
                if (avg < min) min = avg;
                if (avg > max) max = avg;
            }
            var delta = max - min;
            var step = delta / num_groups;
            for (int i = 1; i < num_groups + 1; i++)
            {
                var group = new List<Student>();
                foreach (var stud in students)
                {
                    if ((stud.AverageRating >= (min + (i-1)*step)) && (stud.AverageRating < (min + i*step)))
                    {
                        group.Add(stud);
                    } else if ((i == num_groups) && (stud.AverageRating == max)) {
                        group.Add(stud);
                    }
                    string group_name = "group" + i.ToString();
                    Student.WriteToCsv(group_name + ".csv", group.ToArray());
                }
            }
        }
    }


}
