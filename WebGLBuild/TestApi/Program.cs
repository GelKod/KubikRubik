using Npgsql;

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

string connStr ="Host=localhost;Database=postgres;Username=postgres;Password=admin";

app.MapPost("/auth/register", async (RegisterRequest req) =>
{
    if (string.IsNullOrWhiteSpace(req.Username) ||
        string.IsNullOrWhiteSpace(req.Password))
    {
        return Results.BadRequest("Username and password required");
    }

    var passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
    var userId = Guid.NewGuid();

    try
    {
        await using var conn = new NpgsqlConnection(connStr);
        await conn.OpenAsync();

        // users
        var cmd = new NpgsqlCommand(@"
            INSERT INTO users (id, username, password_hash)
            VALUES (@id, @username, @hash)
        ", conn);

        cmd.Parameters.AddWithValue("id", userId);
        cmd.Parameters.AddWithValue("username", req.Username);
        cmd.Parameters.AddWithValue("hash", passwordHash);

        await cmd.ExecuteNonQueryAsync();

        // user_stats
        var statsCmd = new NpgsqlCommand(@"
            INSERT INTO user_stats (user_id, wins)
            VALUES (@user_id, 0)
        ", conn);

        statsCmd.Parameters.AddWithValue("user_id", userId);
        await statsCmd.ExecuteNonQueryAsync();

        return Results.Ok(new { ok = true });
    }
    catch (PostgresException ex) when (ex.SqlState == "23505")
    {
        return Results.Conflict("Username already exists");
    }
});


app.MapPost("/auth/login", async (LoginRequest req) =>
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
        return Results.Unauthorized();

    var userId = reader.GetGuid(0);
    var hash = reader.GetString(1);

    if (!BCrypt.Net.BCrypt.Verify(req.Password, hash))
        return Results.Unauthorized();

    return Results.Ok(new
    {
        ok = true,
        userId = userId
    });
});

app.Run();


record RegisterRequest(string Username, string Password);
record LoginRequest(string Username, string Password);