using System.Data;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Connection string placeholders: fill these in
// NOTE: Prefer Windows Auth locally if possible; see the alt connection string below.
var connectionString =
    "Server=dotnet.reynolds.edu;Database=ITD132_04;User Id=ITD132_04;Password=u9m*YnJlLAEU@JFjFP2P;TrustServerCertificate=True;Encrypt=False;";

// Alternative (Windows Authentication) example:
// var connectionString =
//     "Server=YOUR_SERVERNAME;Database=ContactDemo;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;";

builder.Services.AddSingleton(new DbConfig(connectionString));

var app = builder.Build();

// Serve static files from wwwroot (so / loads index.html if default files enabled)
app.UseDefaultFiles();   // enables index.html as default
app.UseStaticFiles();    // serves wwwroot content

// POST endpoint that receives form data and inserts into SQL Server
app.MapPost("/contact", async (HttpRequest request, DbConfig db) =>
{
    // Form posts as application/x-www-form-urlencoded by default.
    var form = await request.ReadFormAsync();

    var fullName = (string?)form["fullName"];
    var email    = (string?)form["email"];
    var subject  = (string?)form["subject"];
    var message  = (string?)form["message"];

    // Basic validation
    if (string.IsNullOrWhiteSpace(fullName) ||
        string.IsNullOrWhiteSpace(email) ||
        string.IsNullOrWhiteSpace(subject) ||
        string.IsNullOrWhiteSpace(message))
    {
        return Results.BadRequest("All fields are required.");
    }

    const string sql = @"
INSERT INTO dbo.ContactMessages (FullName, Email, Subject, Message)
VALUES (@FullName, @Email, @Subject, @Message);";

    try
    {
        await using var conn = new SqlConnection(db.ConnectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar, 100) { Value = fullName.Trim() });
        cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 255) { Value = email.Trim() });
        cmd.Parameters.Add(new SqlParameter("@Subject", SqlDbType.NVarChar, 150) { Value = subject.Trim() });
        cmd.Parameters.Add(new SqlParameter("@Message", SqlDbType.NVarChar) { Value = message.Trim() });

        await cmd.ExecuteNonQueryAsync();

        // Simple success page
        return Results.Content("""
            <!doctype html>
            <html><head><meta charset="utf-8"><title>Thanks</title></head>
            <body style="font-family: Arial; margin: 2rem;">
              <h2>Message received ✅</h2>
              <p>Your message was saved to the database.</p>
              <p><a href="/">Back to form</a></p>
            </body></html>
        """, "text/html");
    }
    catch (Exception ex)
    {
        // For class/demo, returning the error is helpful; for production, log it instead.
        return Results.Problem(detail: ex.Message);
    }
});

app.Run();

record DbConfig(string ConnectionString);


