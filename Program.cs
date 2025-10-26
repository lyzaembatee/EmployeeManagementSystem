using EmployeeManagementSystems.Models.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add SQLite Database
builder.Services.AddDbContext<DashboardContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DashboardContext>();
    context.Database.EnsureCreated();
    
    // Seed initial data if needed
    if (!context.Users.Any())
    {
        // Add initial admin user (Password: "admin123")
        context.Users.Add(new EmployeeManagementSystems.Models.Entities.User
        {
            Name = "Admin Manager",
            Email = "admin@company.com",
            PasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=",
            Role = "Manager",
            Position = "System Administrator",
            Department = "IT"
        });
        
        // Add sample employee (Password: "employee123")
        context.Users.Add(new EmployeeManagementSystems.Models.Entities.User
        {
            Name = "John Employee",
            Email = "employee@company.com",
            PasswordHash = "JdVa0lOjhlv0HKBVOVpKNeYMN3xTYaZIZNqRQRh2r7Q=",
            Role = "Employee",
            Position = "Software Developer",
            Department = "IT"
        });
        context.SaveChanges();
        
        // Add employee records
        var manager = context.Users.First(u => u.Role == "Manager");
        var employeeUser = context.Users.First(u => u.Role == "Employee");
        
        context.Employees.Add(new EmployeeManagementSystems.Models.Entities.Employee
        {
            UserId = employeeUser.Id,
            EmployeeId = "EMP0001",
            HireDate = DateTime.UtcNow.AddMonths(-6),
            PerformanceScore = 85,
            AttendancePercentage = 95,
            TotalPoints = 450
        });
        
        context.SaveChanges();
    }
}

app.Run();