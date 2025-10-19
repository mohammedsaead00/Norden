# PowerShell script to test database connection
Write-Host "Testing NordenAPI Database Connection..." -ForegroundColor Green

try {
    # Test if SQL Server is running
    $sqlService = Get-Service -Name "MSSQL*" -ErrorAction SilentlyContinue
    if ($sqlService) {
        Write-Host "SQL Server services found:" -ForegroundColor Yellow
        $sqlService | ForEach-Object { Write-Host "  - $($_.Name): $($_.Status)" }
    } else {
        Write-Host "No SQL Server services found. Please install SQL Server Express." -ForegroundColor Red
        exit 1
    }
    
    # Test connection using .NET
    Add-Type -AssemblyName System.Data.SqlClient
    $connectionString = "Server=localhost\SQLEXPRESS;Integrated Security=true;Connection Timeout=5;"
    
    try {
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        Write-Host "✅ Successfully connected to SQL Server!" -ForegroundColor Green
        
        # Check if NordenDB exists
        $command = $connection.CreateCommand()
        $command.CommandText = "SELECT name FROM sys.databases WHERE name = 'NordenDB'"
        $result = $command.ExecuteScalar()
        
        if ($result) {
            Write-Host "✅ NordenDB database exists!" -ForegroundColor Green
            
            # Get table count
            $command.CommandText = "USE NordenDB; SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"
            $tableCount = $command.ExecuteScalar()
            Write-Host "📊 Tables in NordenDB: $tableCount" -ForegroundColor Cyan
            
            # List tables
            $command.CommandText = "USE NordenDB; SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"
            $reader = $command.ExecuteReader()
            Write-Host "📋 Available tables:" -ForegroundColor Cyan
            while ($reader.Read()) {
                Write-Host "  - $($reader[0])" -ForegroundColor White
            }
            $reader.Close()
        } else {
            Write-Host "❌ NordenDB database does not exist. Creating it..." -ForegroundColor Yellow
            
            # Create database
            $command.CommandText = "CREATE DATABASE NordenDB"
            $command.ExecuteNonQuery()
            Write-Host "✅ NordenDB database created!" -ForegroundColor Green
        }
        
        $connection.Close()
    } catch {
        Write-Host "❌ Failed to connect to SQL Server: $($_.Exception.Message)" -ForegroundColor Red
    }
    
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nTo view your database, use one of these methods:" -ForegroundColor Yellow
Write-Host "1. SQL Server Management Studio (SSMS)" -ForegroundColor White
Write-Host "2. Azure Data Studio" -ForegroundColor White
Write-Host "3. Visual Studio Code with SQL Server extension" -ForegroundColor White
Write-Host "4. Run the create_database.sql script in SSMS" -ForegroundColor White
