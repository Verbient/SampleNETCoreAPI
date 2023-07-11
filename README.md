# SampleNETCoreAPI
A Sample Web API using SQL Server, Dapper, GenericController, DTO, Services, Models and Tests


-----CREATED BY SUJOY ADHIKARY--------

SuperAdmin Login Credentials:
 superadmin1@mail.com/superadmin2@mail.com  Password:Admin@1234
 
Admin Login Credentials:
 admin3@mail.com/admin4@mail.com  Password:Admin@1234

Customer Login Credentials:
 customer5@mail.com - customer94@mail.com  Password:Admin@1234
 
Key features:

1) Loosely coupled and testable
2) DTO layer is used for Data transfers in API. It removes dependency from the Model layer, 
   wherever necessary
3) JWTMiddleware  is used as the method of Authorization
4) Custom Authorization is used, which is very flexible.
5) Custom Exception Filter is used which has the ability to display a BadRequest error 
   from any part of the Applicaiton 
6) Password is Salted and Hashed using `Rfc2898DeriveBytes`
7) SQL server(attached mdf file) is used as Database. There are useful Storedprocs like InitialCreate, 
   DatabaseSeed...
8) Dapper Micro ORM is used, which is extremely fast and SQL friendly
9) GenericController is used, which makes some of the Controllers extremely lightweight. Reflection is 
   used to dynamically map the DTO with the Database-Models and generate SQL Queries. It opens an immense
   possibility of Customization
10)UnitTests also shows the capability of Generics. It is just the starting point of further customizations
   and creating reusable Generic Test Classes. It can Mock IHttpContextAccessor and Can Simulate Logins,
   thus making it capable of performing some integration tests too.
11)EFDemo is a Demo helper project which helps to generate some the Model class using Migration 

Disadvantages:

1) SQL Server (mdf) file is used and is attached for portability. This is only good for this Test project.
   In realtime, we would prefer an AWS RDS database, hosted in some server; and in development phase we 
   can use SQL Server Localhost database. We can also use Secret Manager to store database configurations

2) IntegrationTest won't run as the Database path is not static.

3) CustOrder and CustOrderDetail can have many more useful endpoints. The can have Cascade DELETE relation.
   Further Delete error can have proper message, using better Exception handling. 
   Omited due to time constraints.

4) There can be test cases for Services and DAL layer too
