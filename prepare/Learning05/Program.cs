using System;

class Program
{
    static void Main(string[] args)
    {
        Square square = new Square("red", 5);
        Console.WriteLine($"Square - Color: {square.GetColor()}, Area: {square.GetArea()}");

        Rectangle rectangle = new Rectangle("blue", 4, 6);
        Console.WriteLine($"Rectangle - Color: {rectangle.GetColor()}, Area: {rectangle.GetArea()}");

        Circle circle = new Circle("green", 3);
        Console.WriteLine($"Circle - Color: {circle.GetColor()}, Area: {circle.GetArea()}");

        List<Shape> shapes = new List<Shape>();
        shapes.Add(new Square("red", 5));
        shapes.Add(new Rectangle("blue", 4, 6));
        shapes.Add(new Circle("green", 3));

        foreach (Shape shape in shapes)
        {
            Console.WriteLine($"Shape - Color: {shape.GetColor()}, Area: {shape.GetArea()}");
        }
    }
}
