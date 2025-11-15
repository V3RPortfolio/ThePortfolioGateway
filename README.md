# Ocelot API Gateway

This is a sample microservice architecture that uses API Gateway for authentication and consists of a service (ProductService). Both of the applications are .Net 6 application. The details of different services are given below.

### 1. Database.Product

This is a SQLProj project that contains the schema of the database. All tables, stored procedures, views, etc. are created here and is published to the database from here.

**Important files**

- Database.Product.publish.xml: This file contains the connectionstring of the database.
 
**Publishing a database (Using vscode extension)**

- Open SQL Server (at least 2019) and create a new database named 'ocelot'.
- Right click on the project 'Database.Product' in Visual Studio 2022 and click on 'Publish'.
- A popup will appear. Click on 'Load Profile' button and select the file 'Database.Product.publish.xml'.
- Click on the 'Publish' button and the schema will get updated in the database.


**Publishing the database (Linux)**

To run the sqlproj applications in Linux server, please use Database.Product.linux.sqlproj:

```bash
dotnet clean Database.Product.linux.sqlproj
dotnet build Database.Product.linux.sqlproj
dotnet tool install -g microsoft.sqlpackage
sqlpackage \
  /Action:Publish \
  /SourceFile:"bin/Debug/net8.0/Database.Authentication.dacpac" \
  /TargetConnectionString:"Server=localhost,1433;Database=portfolio_auth;User ID=sa;Password=Admin@123;TrustServerCertificate=True;Encrypt=False;Authentication=SqlPassword;"
```


**Changing database connectionstring**

- Open the file 'Database.Product.publish.xml'.
- In 'TargetConnectionString' section update the connectionstring of the database.
 
### 2. Database.Log

This is a SQLProj project that contains the schema of the database. All tables, stored procedures, views, etc. are created here and is published to the database from here.

**Important files**

- Database.Log.publish.xml: This file contains the connectionstring of the database.
 
**Publishing a database**

- Open SQL Server (at least 2019) and create a new database named 'ocelot_log'.
- Right click on the project 'Database.Log' in Visual Studio 2022 and click on 'Publish'.
- A popup will appear. Click on 'Load Profile' button and select the file 'Database.Product.publish.xml'.
- Click on the 'Publish' button and the schema will get updated in the database.


**Changing database connectionstring**

- Open the file 'Database.Product.publish.xml'.
- In 'TargetConnectionString' section update the connectionstring of the database.

### 3. ProductService

This is a .Net 6 Web Api application. Users and other applications communicate with this service using the ReST api. Swagger is attached to the service to test the API.

**Changing database connectionstring**

If you are running the application using Docker:

- Navigate to 'appsettings.docker.json' and update the connectionstring in 'Product' section.

If you are using builtin server of Visual Studio 2022:

- Navigate to 'appsettings.Development.json' and update the connectionstring in 'Product' section.


**Swagger Endpoint: [http://localhost:8006/swagger/index.html](http://localhost:8006/swagger/index.html)**

### 4. AuthenticationService

This is a .Net 6 Web Api application. Users and other applications communicate with this service using the ReST api. Swagger is attached to the service to test the API.

This application uses 'Ocelot' to act as the API Gateway to other microservices. Clients send request to this endpoint first and the requests are then routed to appropriate microservice. It communicates with other microservices using ReST api.

**Changing database connectionstring**

If you are running the application using Docker:

- Navigate to 'appsettings.docker.json' and update the connectionstring in 'Product' section.

If you are using builtin server of Visual Studio 2022:

- Navigate to 'appsettings.Development.json' and update the connectionstring in 'Product' section.


**Swagger Endpoint: [http://localhost:8005/swagger/index.html](http://localhost:8005/swagger/index.html)**


### 5. Package.Database

This is a .Net class library that contains the boilerplate code for EntityFramework and Dapper. Since multiple services will use this same boilerplate code, all this code has been created in a separate repository. All other services will use this package. It will remove code duplication and make the process of integrating dapper and EF in service easier.

## Running application locally (Using Visual Studio Server)

1. Open the solution using Visual Studio 2022. You should have .Net 6 runtime installed.
2. Right click on the solution and select 'Properties'.
3. Select 'Common Properties' > 'Startup Project' and click on 'Multiple startup projects'.
4. Set 'Action' value of 'AuthenticationService' and 'ProductService' from 'None' to 'Start'. Set 'docker-compose' to 'None'.
5. Click on 'Apply' and 'Ok'.
6. Create a new database named 'ocelot' in SQL Server.
7. Change the connectionstring in 'appsettings.development.json' of 'AuthenticationService' and 'ProductService' to point to the local SQL Server.
8. Open the file 'Database.Product.publish.xml' file in Database.Product project and change the connectionstring to point to the local SQL Server.
9. Right click on the project 'Database.Product' and publish the database. This should populate the new database with tables and stored procedures.
10. Open the database with SQL Studio Management Server and run the stored procedure: ```exec BulkCreateRandomProducts 1000```. This will populate the Product table with random 1000 products.
11. Click on the 'Start' button in Visual Studio 2022 to start all the microservice applications. You can then use the swagger urls provided above to test the endpoints.


## Running application using Docker

1. Install Docker Desktop on your machine.
2. Right click on the solution and select 'Properties'.
3. Select 'Common Properties' > 'Startup Project' and click on 'Single startup project' and select 'docker-compose'.
4. Click on 'Apply' and 'Ok'.
5. Create a new database named 'ocelot' in SQL Server.
6. Change the connectionstring in 'appsettings.docker.json' of 'AuthenticationService' and 'ProductService' to point to the local SQL Server. You should provide the value of 'Data Source' as 'host.docker.internal' and you should enable password login for the datbase. In the connectionstring you should replace the username and password.
7. Open the file 'Database.Product.publish.xml' file in Database.Product project and change the connectionstring to point to the local SQL Server.
8. Right click on the project 'Database.Product' and publish the database. This should populate the new database with tables and stored procedures.
9. Open the database with SQL Studio Management Server and run the stored procedure: ```exec BulkCreateRandomProducts 1000```. This will populate the Product table with random 1000 products.
10. Click on 'Docker Compose' run button in Visual Studio 2022 to start all the microservice applications. You can then use the swagger urls provided above to test the endpoints.

## Running MSSQL Server with docker

To run MSSQL Server, perform the following tasks:

1. Login to docker container using the following command using:

```bash
docker exec -it e69e056c702d "bash"
```

2. Once inside the container, connect locally with sqlcmd by using its full path.

```bash
/opt/mssql-tools18/bin/sqlcmd -No -S localhost -U sa -P 'Admin@123'
```

**Note: Newer versions of sqlcmd are secure by default. For more information about connection encryption, see sqlcmd utility for Windows, and Connecting with sqlcmd for Linux and macOS. If the connection doesn't succeed, you can add the -No option to sqlcmd to specify that encryption is optional, not mandatory.**

3. Run the following command within the SQL Server terminal to ensure that SQL Server is running.

```bash
SELECT @@VERSION
```

4. Check existing databases using the following command:

```bash
EXEC sp_databases;
GO
```

5. Create a database using the following command:

```bash
/opt/mssql-tools18/bin/sqlcmd -No -S localhost -U sa -P 'Admin@123' -Q 'CREATE DATABASE [ocelot];'
```

## Ocelot.json

The files ocelot.json and ocelot.docker.json in the 'AuthenticationService' project contains the mapping to route requests to different services. The documentation for Ocelot can be found at [https://ocelot.readthedocs.io/en/latest/introduction/bigpicture.html](https://ocelot.readthedocs.io/en/latest/introduction/bigpicture.html)


## Upgrading packages with .Net Target Framework

```bash
# check outdated packages for all projects
dotnet list package --outdated

# (optional) install an updater tool
dotnet tool install --global dotnet-outdated-tool

# run the tool to update top-level package references automatically
dotnet outdated -u

# or update specific packages manually
dotnet add ProductService/ProductService.csproj package Microsoft.Extensions.Hosting --version 8.0.0

# restore and build
dotnet restore
dotnet build
```

### Building Application

```bash
dotnet build Package.Database
dotnet build AuthenticationService 
```