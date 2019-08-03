﻿
﻿English      <a href="./POYA/README/README.zh-CN.md">中文</a>   
**Hello, here is POYA project, I'm Larry**  
**❤ Welcome!! ❤**  
>In fact, my definition of this project is vague, but I have implemented the function of file storage and article publishing in the project, and I would like to add many functions in the future. If you like my project and want to contribute your code (<a href="#2">succinctly</a>) **>_**   
####  null  
***  
>This is a **C#/.NET Core MVC** project   
(I'm a beginner, bored and uninterested? Actually, it's **[easy](https://docs.microsoft.com/en-us/aspnet/?view=aspnetcore-2.2#pivot=core "easy")**)  
**. . .** , There should be a lot of things to write here, I don't write it much😅, I think you must know a lot more than I do
#### 0  
***  
>Rename the **appsettings.json.text** 
> file 
>(<a href="./POYA/">where?</a>) to **appsettings.json** after **git clone** it    
The contents of **appsettings.json** are:  
```json
{
  "ConnectionStrings": { 
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=aspnet-POYA-0E28E843-176D-49F3-9739-6D5E6F1BC3F5;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "EmailSender": {
    "userName": "xxxxxx@xxxxxxxx.xxxx",
    "host": "xxxxxx.xxxxx.xxxx",
    "password": "xxxxx",
    "port": 0,
    "enableSsl": false
  },
  "ErrorLogHandle": {
    "ReceiveLogEmailAddress": "xxxxxxx@xxx.xxx"
  },
  "Administration": {
    "AdminEmail": "xxxxxxxxx@xxx.xxx"
  },
  "IsInitialized":false
} 
```     
>✔ **DefaultConnection >_** The connection string of database, the above is the connection string of the MSSQL, the  connection string is **"DataSource=app.db"** if you use **SQLite**. You can customize your connection string and modify **services.AddDbContext**  in **Startup.cs** file accordingly  

✔**EmailSender>_** It is the configuration of the mail service(you need to change its value in order to the project to work properly), it plays a key role in user registration and user receiving notifications      

✔**ReceiveLogEmailAddress** This e-mail address is used to receive error messages    
✔**AdminEmail** The email of administrator(the original administrator), This administrator can add developers and content review administrators, and more. . .     
✔**IsInitialized** Initializing the ApplicationDbContext in the Controller is  simple relatively, so "IsInitialized" is used to indicate whether the application is initialized(you don't need to change it manually, but you should make a migratioin before the first **dotnet run**, although exceptions are ignored)


#### 1(inessential)    
***  
>You can try it after your appsettings.json is 👌 **>_**  
**dotnet build**  
**dotnet run**

#### <span id="2">2(succinctly)</span>   

```powershell    
#sh/ps1
cd yourdir
git clone https://github.com/larryw3i/POYA.git
cd POYA
cd POYA
cp appsettings.json.txt appsettings.json
#modify appsettings.json
dotnet build
dotnet run
```     

#### . . .

 ####  ∞    
***     
>Copyright (c) 2019 Larry Wei    
Licensed under the <a href="./LICENSE">MIT</a>  License


