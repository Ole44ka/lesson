using System;
using System.IO;
using System.Collections.Generic;

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
            
            var students = CreateStudents(first_names.ToArray(), last_names.ToArray(), patr_names.ToArray(), 200);
            
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
        }
        return result;
    }
    }



    class Student
    {
        public string FirstName;
        public string LastName;
        public string Patronimyc;

        public Student(string f_name, string last_name, string patr)
        {
            FirstName = f_name;
            LastName = last_name;
            Patronimyc = patr;
        }
    }




}
