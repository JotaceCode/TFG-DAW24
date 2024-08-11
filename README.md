# 🧹 BROOM
# 🦄 C# ASP.NET CORE 8 WEB API + ANGULAR 17

+ Jose Carlos Vélez
  - https://github.com/JotaceCode
    
+ David Mendoza
  - https://github.com/eldavid1996
    
+ Esteban Dominguez
  - https://github.com/EDB1827
    
## TABLE OF CONTENTS

* [About the project](#-about-the-project)
* [Features](#%EF%B8%8F-features)
* [Installation](#%EF%B8%8F-installation-develop)

## 🔥 About the project

This project has been created with the objective of do more easy the organization of room cleaning tasks in hotels.

This project includes an API Gateway application with Ocelot for the management of the 'User', 'Room' and 'Task' entities.
``ASP.NET Core 8 Web API`` was used with C#.
``'BackEnd'`` directory.

``MongoDB`` and ``SQL Server`` Docker containers used for data persistence.


Additionally, a web application made in ``Angular 17`` with standalone components to communicate with the API.
``'FrontEnd'`` directory.

## ✔️ Features

✅ JWT Authentication

✅ Users Roles

✅ Ocelot Gateway

## ⚙️ Installation (develop)

**1º Clone this repository**

       https://github.com/eldavid1996/FCT_Limpieza

**2º FrontEnd - Install modules**

        npm install; ng serve --o

**3º BackEnd - Https & DataBases**
   
   - Open the project with Visual Studio

   - Install the .NET Develop Certificate

   - Install SQL Server Docker Container:


        ``docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=StrongPassword1!" -p 1433:1433  --name SQLServerSecurity -d mcr.microsoft.com/mssql/server``

   - Start project ``'Services.API.Security'`` as first project with Docker Container started for auto-execute migrations in the docker database
   - Install MongoDB Docker Container:


         docker run -d -p 27017:27017 --name MongoDBHotel mongo

   - Start project ``'Services.API.Hotel'`` as first project with Docker Container started for auto-set unique index in collections

## ⭐️ GIVE IT A STAR

If you found this Implementation useful for your Projects, please give it a star. Thank you!
