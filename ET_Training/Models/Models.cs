namespace ET_Training.Models;

public class Site
{
    public int Id_site { get; set; }
    public string SiteName { get; set; } = "";
    public string Country { get; set; } = "";
}

public class FunctionItem
{
    public int Id_Function { get; set; }
    public string FunctionName { get; set; } = "";
}

public class Category
{
    public int Id_Category { get; set; }
    public string CategoryName { get; set; } = "";
}

public class Course
{
    public int ID_Course { get; set; }
    public string CourseName { get; set; } = "";
    public int Id_Category { get; set; }
    public string? CategoryName { get; set; }
}

public class EmployeeCourse
{
    public int ID_employe { get; set; }
    public int ID_Course { get; set; }
    public DateTime Date_affect { get; set; }
}

public class Employee
{
    public int ID_employe { get; set; }
    public string Name { get; set; } = "";
    public int Id_site { get; set; }
    public string? EmailAddress { get; set; }
    public string Username { get; set; } = "";
    public int Id_Function { get; set; }
    public DateTime Date_emp { get; set; } = DateTime.Today;
    public DateTime? Date_Leave { get; set; }
    public bool IsActive { get; set; } = true;
    public string? SiteName { get; set; }
    public string? FunctionName { get; set; }
}
