using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

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
            Student.WriteToCsv("students.csv", students);


            var students1 = Student.ReadCsv("students.csv");
            foreach (var stud in students1)
            {
                Console.WriteLine("----------------");
                stud.visualize();
            }
            

            WriteArrToXml(students1, "studs.xml", "");
            WriteArrToXml(best.ToArray(), "best_studs.xml", "BestStudents");
            WriteArrToXml(last.ToArray(), "last_studs.xml", "LastStudents");
            WriteEachToXml(students1, "t");


            Console.WriteLine("---x-x-x--STUDENT1--x-x-x-x-");
            var student1 = ReadStudentXml("t_1.xml");
            student1.visualize();

            // Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
            // using (var file = new StreamReader("studs.xml"))
            // {
            //     var ser = new XmlSerializer(typeof(Student []));
            //     var studs = (Student [])ser.Deserialize(file);
            //     studs[1].visualize();
            // }

            var best_studs = ReadStudentArrayXml("last_studs.xml", "LastStudents");
            foreach (var stud in best_studs)
            {
                Console.WriteLine("XXXXXXXXXXXXOOOOOOOOOOOO");
                stud.visualize();
            }


            foreach (string file in Directory.GetFiles(".", "*.xml").Where(item => item.EndsWith(".xml")))
            {
                Console.WriteLine(file);
                File.Delete(file);
            }


        }

        static Student ReadStudentXml(string f_name)
        {
            var file = new StreamReader(f_name);
            var serializer = new XmlSerializer(typeof(Student));
            Student stud = (Student)serializer.Deserialize(file);
            file.Close();
            return stud;
        }

        static Student[] ReadStudentArrayXml(string f_name, string arr_name)
        {
            var file = new StreamReader(f_name);
            var serializer = new XmlSerializer(typeof(Student[]), new XmlRootAttribute(arr_name));
            Student[] students = (Student[])serializer.Deserialize(file);
            file.Close();
            return students;
        }

        static void WriteArrToXml(Student[] arr, string f_name, string arr_name="")
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

        static void WriteEachToXml(Student[] arr, string file_prefix)
        {
            for(int i = 0; i < arr.Length; i++)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Student));
                string f_name = file_prefix + "_" + i.ToString() + ".xml";
                TextWriter writer = new StreamWriter(f_name);
                Console.WriteLine(f_name);
                serializer.Serialize(writer, arr[i]);
                writer.Close();
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

                // result[i].visualize();
                Console.WriteLine();
                
            }
            return result;
        }
        static int[] GetRandomRating(int num, int MinR, int MaxR)
        {
            var rnd = new Random();
            int[] ratings = new int[num];
            for(int i=0; i<num; i++)
            {
                ratings[i] =  (int)(MinR + rnd.NextDouble()*(MaxR - MinR));
            }
            return ratings;
        }
    }



    // [XmlRoot("Student")]
    public class Student
    {
        public string FirstName;
        public string LastName;
        public string Patronimyc;

        public int[] Rating;

        public static void WriteToCsv(string FileName, Student[] students)
        {
            using (var file = new StreamWriter(FileName))
            {
                file.WriteLine("LastName;FirstName;Patronymic;Ratings");
                for (var i=0; i < students.Length; i++)
                {
                    var ratings = string.Join(";", students[i].Rating);
                    file.WriteLine("{0};{1};{2};{3}",
                                   students[i].LastName,
                                   students[i].FirstName,
                                   students[i].Patronimyc,
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
                    var ratings = new int[values.Length - 3];
                    for (var i = 0; i < ratings.Length; i++)
                    {
                        ratings[i] = int.Parse(values[i + 3]);
                    }
                    student.Rating = ratings;
                    result.Add(student);
                }
            }
            return result.ToArray();
        }

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
        public Student()
        {
            FirstName = "";
            LastName = "";
            Patronimyc = "";
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
