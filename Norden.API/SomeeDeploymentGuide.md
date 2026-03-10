## Norden.API Deployment to Somee.com

Because Somee.com is a shared Windows hosting environment, deploying a .NET 9 API requires you to map the files correctly, update the SQL Server database connection string, and ensure the paths are properly routed.

### Phase 1: Set Up the MS SQL Database on Somee

1. Log into your **Somee.com** dashboard and navigate to **User -> MS SQL**.
2. Create a new Microsoft SQL Server Database (e.g., `NordenDb`).
3. Once created, click on the database to view its connection string. 
   * It will look something like this: \`Server=workstation.somee.com;Database=NordenDb;User Id=YourName_SQLLogin_1;Password=YourPassword;\`
4. Copy this precise connection string! We need to place it in the published configuration.

### Phase 2: Update the Appsettings & Publish Folder

I have already generated your final Production-ready code into the `publish` directory:
**`F:\Norden\Norden.API\publish`**

1. Go to `F:\Norden\Norden.API\publish` and open `appsettings.json` (or `appsettings.Production.json` if it generated one) in any text editor.
2. Find the \`"ConnectionStrings": { "DefaultConnection": "..." }\` section.
3. Replace the existing local SQL connection string with the **Somee.com Connection String** you copied in Phase 1.
4. Save the file.
5. Highlight all the files and folders inside the `publish` directory, right-click, and select **Compress to ZIP file**. Name it something like `NordenAPI_Deploy.zip`.

### Phase 3: Set Up the Web Application on Somee

1. Go back to your Somee.com dashboard. Navigate to **User -> Websites**.
2. Click **Create Website**.
3. Choose the `.NET Framework Version` -> Select **.NET 9 (Core)** (Or the highest available .NET Core version if 9 is not explicitly listed, though ASP.NET Core apps generally bundle their own CLR instructions or run self-contained).
4. Give your site a title (e.g., `nordenapi.somee.com`).

### Phase 4: Upload and Extract

1. Open the File Manager for your new Website on Somee.com.
2. Click the **Upload** button and upload the `NordenAPI_Deploy.zip` file you created in Phase 2.
3. Once uploaded, select the ZIP file and choose the **Extract** or **Unzip** option in the file manager.
4. Extract all files directly into the root folder (usually `\wwwroot` or the base path presented by Somee). 
   *(Make sure the `web.config` and the `.dll` files sit right at the root of the site, not inside a subfolder!)*

### Phase 5: Test the Production API

Because the `Program.cs` is configured to run database migrations automatically on startup (`context.Database.Migrate();`), **the very first time** you access the API online, it will connect to your empty Somee SQL database, automatically construct all the tables, and invoke `SeedData.SeedAsync` to populate your Admin User, Products, and Categories.

1. Navigate to: \`https://YOUR_SOMEE_URL/swagger/index.html\`
2. The Swagger UI should appear! You can test the endpoints live on the internet.
