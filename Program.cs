using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;


using System.Text;

namespace lesson2409
{
    class Program
    {

        static bool keepFiles = false;
        static void Main(string[] args)
        {

            TestStudent();
            TestWC();

        }

        static void TestWC()
        {
            Console.WriteLine("\n------\nТест счётчика слов\n------\n");
            string file_name = "voyna-i-mir-tom-1.txt";
            Console.WriteLine(file_name);
            var voyna_words = WordCounter(file_name);
            WriteWordCountsToCsv("warpeace-words.csv", voyna_words);
            if (!keepFiles)
                ClearExt(".csv");
        }

        static Dictionary<string, int> WordCounter(string file_name)
        {
            var file = new FileInfo(file_name);
            if(!file.Exists)
            {
                Console.Write("Файла не существует");
                return new Dictionary<string, int>();
            }
            Console.WriteLine("Размер файла {0}", file.Length);
            var words_dict = new Dictionary<string, int>();
            using (var reader = new StreamReader(file_name, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.Length == 0) 
                        continue;
                    var words = line.Split(' ');
                    foreach (var word in words)
                    {
                        var w = word.Trim().Trim("/,-.?!][)(;:1234567890'".ToCharArray()).ToLower();
                        if (w == null || w.Length == 0)
                            continue;
                        if (!words_dict.ContainsKey(w))
                            words_dict.Add(w,0);
                        words_dict[w]++;
                    }
                }
            }
            Console.WriteLine("Количество слов: {0}", words_dict.Count);
            return words_dict;
        }

        public static void WriteWordCountsToCsv(string FileName, Dictionary<string, int> dict_to_write)
        {
            using (var file = new StreamWriter(FileName))
            {
                file.WriteLine("Word;Count");
                foreach (KeyValuePair<string,int> item in dict_to_write.OrderByDescending(key=> key.Value))
                { 
                     file.WriteLine("{0};{1}", item.Key, item.Value);
                }
            }
        }

        static void TestStudent()
        {
            Console.WriteLine("\n------\nТест класса Student\n------\n");
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
            if (!keepFiles)
            {
                ClearExt();
                ClearExt(".csv");
            }
        }

        static void ClearExt(string extension=".xml")
        {
            foreach (string file in Directory.GetFiles(".", "*" + extension).Where(item => item.EndsWith(extension)))
            {
                Console.WriteLine(file+" deleted");
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
