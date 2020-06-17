using System;

public struct Food
{
    public string Name { get; set; }
    public double Price { get; set; }
    public override string ToString()
    {
        return $"Food(Name={Name}, Price={Price})";
    }
}