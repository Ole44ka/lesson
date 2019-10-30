using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace lesson2409
{
    public class Student
    {
        public string FirstName;
        public string LastName;
        public string Patronimyc;

        public int[] Rating;

        public double AverageRating
        {
            get
            {
                if (Rating == null || Rating.Length == 0) return double.NaN;
                return Rating.Average();
            }
        }
        public Student()
        {
            FirstName = "";
            LastName = "";
            Patronimyc = "";
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

        public static void WriteToCsv(string FileName, Student[] students)
        {
            using (var file = new StreamWriter(FileName))
            {
                file.WriteLine("LastName;FirstName;Patronymic;AvgRating;Ratings");
                for (var i = 0; i < students.Length; i++)
                {
                    var ratings = string.Join(";", students[i].Rating);
                    file.WriteLine("{0};{1};{2};{3};{4}",
                                   students[i].LastName,
                                   students[i].FirstName,
                                   students[i].Patronimyc,
                                   students[i].AverageRating,
                                   ratings);
                }
            }
        }
        public static Student[] ReadCsv(string FileName)
        {
            var result = new List<Student>();
            using (var file = new StreamReader(FileName))
            {
                file.ReadLine();
                while (!file.EndOfStream)
                {
                    var line = file.ReadLine();
                    var values = line.Split(";");
                    var student = new Student(values[1], values[0], values[2]);
                    var ratings = new int[values.Length - 4];
                    for (var i = 1; i < ratings.Length; i++)
                    {
                        ratings[i] = int.Parse(values[i + 3]);
                    }
                    student.Rating = ratings;
                    result.Add(student);
                }
            }
            return result.ToArray();
        }

        public static Student ReadStudentXml(string f_name)
        {
            var file = new StreamReader(f_name);
            var serializer = new XmlSerializer(typeof(Student));
            Student stud = (Student)serializer.Deserialize(file);
            file.Close();
            return stud;
        }

        public static Student[] ReadStudentArrayXml(string f_name, string arr_name)
        {
            var file = new StreamReader(f_name);
            var serializer = new XmlSerializer(typeof(Student[]), new XmlRootAttribute(arr_name));
            Student[] students = (Student[])serializer.Deserialize(file);
            file.Close();
            return students;
        }

        public static void WriteArrToXml(Student[] arr, string f_name, string arr_name = "")
        {
            XmlSerializer serializer = new XmlSerializer(arr.GetType());
            if (arr_name != "")
            {
                serializer = new XmlSerializer(arr.GetType(), new XmlRootAttribute(arr_name));
            }
            using (TextWriter file = new StreamWriter(f_name))
            {
                serializer.Serialize(file, arr);
            }

        }

        public static void WriteEachToXml(Student[] arr, string file_prefix)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Student));
                string f_name = file_prefix + "_" + i.ToString() + ".xml";
                TextWriter writer = new StreamWriter(f_name);
                Console.WriteLine(f_name);
                serializer.Serialize(writer, arr[i]);
                writer.Close();
            }
        }

        public static (List<Student>, List<Student>) GetBestAndLast(Student[] students, double proc=0.1)
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
            var min_threshold = min + proc * delta;
            var max_threshold = max - proc * delta;
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
            return (last, best);
        }

    }
    
}