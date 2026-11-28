using Dapper;
using ET_Training.Models;
using Microsoft.Data.SqlClient;

namespace ET_Training.Data;

public class TrainingRepository
{
    private readonly string _cs;

    public TrainingRepository(IConfiguration config)
    {
        _cs = config.GetConnectionString("TrainingDb")
              ?? throw new InvalidOperationException("ConnectionStrings:TrainingDb is missing in appsettings.json");
    }

    private SqlConnection Open() => new SqlConnection(_cs);

    // ---------- Sites ----------
    public async Task<List<Site>> GetSites()
    {
        using var c = Open();
        return (await c.QueryAsync<Site>("SELECT Id_site, SiteName, Country FROM dbo.tbl_Site ORDER BY SiteName")).ToList();
    }
    public async Task SaveSite(Site x)
    {
        using var c = Open();
        if (x.Id_site == 0)
            await c.ExecuteAsync("INSERT INTO dbo.tbl_Site (SiteName, Country) VALUES (@SiteName, @Country)", x);
        else
            await c.ExecuteAsync("UPDATE dbo.tbl_Site SET SiteName=@SiteName, Country=@Country WHERE Id_site=@Id_site", x);
    }
    public async Task DeleteSite(int id)
    {
        using var c = Open();
        await c.ExecuteAsync("DELETE FROM dbo.tbl_Site WHERE Id_site=@id", new { id });
    }

    // ---------- Functions ----------
    public async Task<List<FunctionItem>> GetFunctions()
    {
        using var c = Open();
        return (await c.QueryAsync<FunctionItem>("SELECT Id_Function, FunctionName FROM dbo.tbl_function ORDER BY FunctionName")).ToList();
    }
    public async Task SaveFunction(FunctionItem x)
    {
        using var c = Open();
        if (x.Id_Function == 0)
            await c.ExecuteAsync("INSERT INTO dbo.tbl_function (FunctionName) VALUES (@FunctionName)", x);
        else
            await c.ExecuteAsync("UPDATE dbo.tbl_function SET FunctionName=@FunctionName WHERE Id_Function=@Id_Function", x);
    }
    public async Task DeleteFunction(int id)
    {
        using var c = Open();
        await c.ExecuteAsync("DELETE FROM dbo.tbl_function WHERE Id_Function=@id", new { id });
    }

    // ---------- Categories ----------
    public async Task<List<Category>> GetCategories()
    {
        using var c = Open();
        return (await c.QueryAsync<Category>("SELECT Id_Category, CategoryName FROM dbo.tbl_category ORDER BY CategoryName")).ToList();
    }
    public async Task SaveCategory(Category x)
    {
        using var c = Open();
        if (x.Id_Category == 0)
            await c.ExecuteAsync("INSERT INTO dbo.tbl_category (CategoryName) VALUES (@CategoryName)", x);
        else
            await c.ExecuteAsync("UPDATE dbo.tbl_category SET CategoryName=@CategoryName WHERE Id_Category=@Id_Category", x);
    }
    public async Task DeleteCategory(int id)
    {
        using var c = Open();
        await c.ExecuteAsync("DELETE FROM dbo.tbl_category WHERE Id_Category=@id", new { id });
    }

    // ---------- Courses ----------
    public async Task<List<Course>> GetCourses()
    {
        using var c = Open();
        return (await c.QueryAsync<Course>(
            @"SELECT co.ID_Course, co.CourseName, co.Id_Category, ca.CategoryName
              FROM dbo.tbl_courses co
              JOIN dbo.tbl_category ca ON ca.Id_Category = co.Id_Category
              ORDER BY co.CourseName")).ToList();
    }
    public async Task SaveCourse(Course x)
    {
        using var c = Open();
        if (x.ID_Course == 0)
            await c.ExecuteAsync("INSERT INTO dbo.tbl_courses (CourseName, Id_Category) VALUES (@CourseName, @Id_Category)", x);
        else
            await c.ExecuteAsync("UPDATE dbo.tbl_courses SET CourseName=@CourseName, Id_Category=@Id_Category WHERE ID_Course=@ID_Course", x);
    }
    public async Task DeleteCourse(int id)
    {
        using var c = Open();
        await c.ExecuteAsync("DELETE FROM dbo.tbl_courses WHERE ID_Course=@id", new { id });
    }

    // ---------- Employees ----------
    public async Task<List<Employee>> GetEmployees()
    {
        using var c = Open();
        return (await c.QueryAsync<Employee>(
            @"SELECT e.ID_employe, e.[Name], e.Id_site, e.EmailAddress, e.Username, e.Id_Function,
                     e.Date_emp, e.Date_Leave, e.IsActive, s.SiteName, f.FunctionName
              FROM dbo.tbl_employe e
              JOIN dbo.tbl_Site s ON s.Id_site = e.Id_site
              JOIN dbo.tbl_function f ON f.Id_Function = e.Id_Function
              ORDER BY e.[Name]")).ToList();
    }
    public async Task SaveEmployee(Employee x)
    {
        using var c = Open();
        if (x.ID_employe == 0)
            await c.ExecuteAsync(
                @"INSERT INTO dbo.tbl_employe ([Name], Id_site, EmailAddress, Username, Id_Function, Date_emp, Date_Leave, IsActive)
                  VALUES (@Name, @Id_site, @EmailAddress, @Username, @Id_Function, @Date_emp, @Date_Leave, @IsActive)", x);
        else
            await c.ExecuteAsync(
                @"UPDATE dbo.tbl_employe SET [Name]=@Name, Id_site=@Id_site, EmailAddress=@EmailAddress, Username=@Username,
                         Id_Function=@Id_Function, Date_emp=@Date_emp, Date_Leave=@Date_Leave, IsActive=@IsActive
                  WHERE ID_employe=@ID_employe", x);
    }
    public async Task DeleteEmployee(int id)
    {
        using var c = Open();
        await c.ExecuteAsync("DELETE FROM dbo.tbl_employe WHERE ID_employe=@id", new { id });
    }

    /// <summary>Turns SQL errors into a short readable message.</summary>
    public static string Explain(Exception ex)
    {
        if (ex is SqlException s)
        {
            return s.Number switch
            {
                2601 or 2627 => "Already exists (duplicate value).",
                547 => "Cannot do this: the row is linked to other data (foreign key) or a date check failed.",
                515 => "A required field is empty.",
                _ => "SQL error: " + s.Message
            };
        }
        return ex.Message;
    }
}
