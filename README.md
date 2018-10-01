# WorkforceManagement
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
In Visual Studio right click on ```WorkforceManagement``` and select ```Add -> New Item...```
when the window pops up select ```Data``` underneath ```ASP.NET Core``` and choose ```JSON File``` and name it ```appsettings.json``` then click ```add```
then open ```SSMS``` and copy the contents of the ```Server name``` text box and paste where it says ```INSERT_DATABASE_CONNECTION_HERE```
then replace ```INSERT_DATABASE_NAME``` with the name of your database that you've created. 

## Starting this Project

Clone the Obedient Lobsters WorkforceManagement repo onto your machine. ```cd``` into that directory and open the project in Visual Studio Code.
Make sure the database is built on your local machine using the contents of ```BangazonDatabaseSeedBuild``` in the project root directory (see database setup above for instructions).
Link Visual Studio Code to that database by going to ```View``` and selecting the ```SQL Server Object Explorer```. Open that up and press the ```add SQL server``` button (looks like a column with a green plus). Then select ```local``` and pick the option that matches your local server.

If all went correctly, your database should be connected, and you can then run the project.
To run the project, press the green "play" triangle that is above the code editor, roughly in the middle.



# Human Resources

## Departments

### View All
	The great folks in the Human Resources Department can view all Departments when the Department Naviagation Link is clicked. The Department view shows links for "Create new Department" under the page title and "details" link to the right of each department.

### Add A New Department
	Human Resources can add a new department by clicking Create New link on the View all Departments view. The link will show a form with an input field requesting the new department name and a submit button. Once submitted they will be rerouted back to view all department page and the new department is also listed.

### Details/View one Department
	Click on the word details on and individual department to see a list of that department's employees.

## Employee 

### Employee Index/List View
    To see the Employee Index view, click on the Employee tab in the navbar in the upper portion of the screen (while the app is running). If all is working correctly, you should see a nicely formatted table that has a column for First Name Last Name and Department Name. Each of these columns should be filled with the corresponding information that is sourced from the database. On the right of each row, for each Employee, you should see a hyperlink for ```Detail``` and ```Edit```. Above the First Name column, there should be a hyperlink for ```Create New```. 

## Computer


### Details/View One Computer
	Click on the link "Details" one an indiviual computer in the Index view. The browser will show the details of the selected computer. The details will include the computer id, Date of purchase of the computer, date the computer was decommissioned if applicable, operation status as a checkbox, model and manufacturer.
  