**Migrate Database**

go into solution directory - `Homework/`
```
dotnet ef migrations add InitialMigrations --project DAL --startup-project WebApp
```

**Apply Changes**
```
dotnet ef database update --project DAL --startup-project WebApp
```

**Scaffold Controllers**

go into webapp project folder `Homework/WebApp/`
```
dotnet aspnet-codegenerator controller -name CaesarsController          -actions -m  Domain.Caesar          -dc DAL.ApplicationDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name VigeneresController        -actions -m  Domain.Vigenere        -dc DAL.ApplicationDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name DiffieHellmansController   -actions -m  Domain.DiffieHellman   -dc DAL.ApplicationDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -name RsasController             -actions -m  Domain.Rsa             -dc DAL.ApplicationDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
```
