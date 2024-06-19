using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public static void Main()
    {
        var people = new List<Person>{
            new Person("Iker", 18, 1.80f),
            new Person("Joan", 19, 1.60f),
            new Person("Martin", 20, 1.67f)
        };

        var result = people.Where(x => x.Edad > 18 & x.Altura > 1.60f)
            .Select(x => new {
                Nombre = x.Nombre,
                ProximaEdad = x.Edad + 1
            })
            .ToList();

        foreach (var r in result)
        {
            Console.WriteLine("{0} - {1}", r.Nombre, r.ProximaEdad);
        }
    }
}

public class Result
{
    public string Name { get; set; }
    public float NextAge { get; set; }
}

public class Person
{
    public string Nombre { get; set; }
    public int Edad { get; set; }
    public float Altura { get; set; }

    public Person(string nombre, int edad, float altura)
    {
        // PascalCase: Clases, Métodos, Interfaces
        // camelCase: Variables. Parámetros
        // snake_case: MI_CONSTANTE
        this.Nombre = nombre;
        this.Edad = edad;
        this.Altura = altura;

    }
}