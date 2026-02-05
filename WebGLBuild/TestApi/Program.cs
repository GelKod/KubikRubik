using Npgsql;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors();

// Get connection string from appsettings.json or use default
string connStr = builder.Configuration.GetConnectionString("DefaultConnection")
                 ?? "Host=localhost;Database=postgres;Username=postgres;Password=admin";

// Register
app.MapPost("/auth/register", async (RegisterRequest req) =>
{
    if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
    {
        return Results.BadRequest("Требуется имя пользователя и пароль");
    }

    var passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
    var userId = Guid.NewGuid();

    try
    {
        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        // Start transaction
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            // Insert user
            var cmd = new NpgsqlCommand(@"
                INSERT INTO users (id, username, password_hash)
                VALUES (@id, @username, @hash)
            ", conn, transaction);

            cmd.Parameters.AddWithValue("id", userId);
            cmd.Parameters.AddWithValue("username", req.Username);
            cmd.Parameters.AddWithValue("hash", passwordHash);

            await cmd.ExecuteNonQueryAsync();

            // Insert initial stats
            var statsCmd = new NpgsqlCommand(@"
                INSERT INTO user_stats (user_id, wins)
                VALUES (@user_id, 0)
            ", conn, transaction);

            statsCmd.Parameters.AddWithValue("user_id", userId);
            await statsCmd.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
            return Results.Ok(new { ok = true, userId = userId });
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    catch (PostgresException ex) when (ex.SqlState == "23505")
    {
        return Results.Conflict("Пользователь с таким именем уже существует");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex.Message);
        return Results.Problem("Ошибка сервера при регистрации");
    }
});

// Login
app.MapPost("/auth/login", async (LoginRequest req) =>
{
    if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
    {
        return Results.BadRequest("Требуется имя пользователя и пароль");
    }

    try
    {
        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(@"
            SELECT id, password_hash
            FROM users
            WHERE username = @username
        ", conn);

        cmd.Parameters.AddWithValue("username", req.Username);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return Results.Unauthorized();
        }

        var userId = reader.GetGuid(0);
        var hash = reader.GetString(1);

        if (!BCrypt.Net.BCrypt.Verify(req.Password, hash))
        {
            return Results.Unauthorized();
        }

        return Results.Ok(new
        {
            ok = true,
            userId = userId
        });
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(ex.Message);
        return Results.Problem("Ошибка сервера при входе");
    }
});

// Optional: Get user stats
app.MapGet("/user/{userId}/stats", async (Guid userId) =>
{
    try
    {
        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = new NpgsqlCommand(@"
            SELECT wins FROM user_stats WHERE user_id = @user_id
        ", conn);
        cmd.Parameters.AddWithValue("user_id", userId);

        var wins = await cmd.ExecuteScalarAsync();
        if (wins == null) return Results.NotFound();

        return Results.Ok(new { userId, wins = (int)wins });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();

record RegisterRequest(string Username, string Password);
record LoginRequest(string Username, string Password);
