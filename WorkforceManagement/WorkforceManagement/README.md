﻿# WorkforceManagement
Welcome to **Bangazon!** The new virtual marketplace. This marketplace allows customers to buy and sell their products through a single page application web page and its data is tracked through a powerful, hand crafted and solely dedicated API. 

## Table of Contents
(content to be filled)
## Software Requirements
Sql Server Manangment Studio
Visual Studio Community 2017
Google Chrome
## Enitity Relationship Diagram
![ERD](/images/bangazonv3.png)


## Database Setup
In Visual Studio right click on ```BangazonAPI``` and select ```Add -> New Item...```
when the window pops up select ```Data``` underneath ```ASP.NET Core``` and choose ```JSON File``` and name it ```appsettings.json``` then click ```add```
then open ```SSMS``` and copy the contents of the ```Server name``` text box and paste where it says ```INSERT_DATABASE_CONNECTION_HERE```
then replace ```INSERT_DATABASE_NAME``` with the name of your database that you've created. 

## Starting this Project

Clone the Obedient Lobsters WorkforceManagement repo onto your machine. ```cd``` into that directory and open the project in Visual Studio Code.
Make sure the database is built on your local machine using the contents of ```BangazonDatabaseSeedBuild``` in the project root directory (see database setup above for instructions).
Link Visual Studio Code to that database by going to ```View``` and selecting the ```SQL Server Object Explorer```. Open that up and press the ```add SQL server``` button (looks like a column with a green plus). Then select ```local``` and pick the option that matches your local server.

If all went correctly, your database should be connected, and you can then run the project.
To run the project, press the green "play" triangle that is above the code editor, roughly in the middle.

