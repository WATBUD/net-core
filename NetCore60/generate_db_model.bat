@echo off
dotnet ef dbcontext scaffold "Server=127.0.0.1;Database=csharp_db;User=louis005;Password=louis005;" Pomelo.EntityFrameworkCore.MySql -o Models --force --context ApplicationDbContext
pause
