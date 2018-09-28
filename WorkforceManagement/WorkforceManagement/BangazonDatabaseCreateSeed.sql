
IF (OBJECT_ID('dbo.FK_ComputerEmployee', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.EmployeeComputer DROP CONSTRAINT FK_ComputerEmployee
END

IF (OBJECT_ID('dbo.FK_EmployeeComp', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.EmployeeComputer DROP CONSTRAINT FK_EmployeeComp
END

IF (OBJECT_ID('dbo.FK_TrainingProgram', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.EmployeeTraining DROP CONSTRAINT FK_TrainingProgram
END

IF (OBJECT_ID('dbo.FK_EmployeeTraining', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.EmployeeTraining DROP CONSTRAINT FK_EmployeeTraining
END

IF (OBJECT_ID('dbo.FK_DepartmentEmployee', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.Employee DROP CONSTRAINT FK_DepartmentEmployee
END

IF (OBJECT_ID('dbo.FK_CustomerProduct', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.Product DROP CONSTRAINT FK_CustomerProduct
END

IF (OBJECT_ID('dbo.FK_ProductTypeProduct', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.Product DROP CONSTRAINT FK_ProductTypeProduct
END

IF (OBJECT_ID('dbo.FK_OrderProductOrder', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.ProductOrder DROP CONSTRAINT FK_OrderProductOrder
END

IF (OBJECT_ID('dbo.FK_ProductProductOrder', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.ProductOrder DROP CONSTRAINT FK_ProductProductOrder
END

IF (OBJECT_ID('dbo.FK_CustomerPaymentOrder', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.[Order] DROP CONSTRAINT FK_CustomerPaymentOrder
END

IF (OBJECT_ID('dbo.FK_CustomerOrder', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.[Order] DROP CONSTRAINT FK_CustomerOrder
END

IF (OBJECT_ID('dbo.FK_PaymentTypeCustomer', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.CustomerPayment DROP CONSTRAINT FK_PaymentTypeCustomer
END

IF (OBJECT_ID('dbo.FK_CustomerPayment', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE dbo.CustomerPayment DROP CONSTRAINT FK_CustomerPayment
END

DROP TABLE IF EXISTS Department;
DROP TABLE IF EXISTS Employee;
DROP TABLE IF EXISTS EmployeeComputer;
DROP TABLE IF EXISTS EmployeeTraining;
DROP TABLE IF EXISTS [Order];
DROP TABLE IF EXISTS PaymentType;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS ProductOrder;
DROP TABLE IF EXISTS ProductType;
DROP TABLE IF EXISTS TrainingProgram;
DROP TABLE IF EXISTS Computer;
DROP TABLE IF EXISTS Customer;
DROP TABLE IF EXISTS CustomerPayment;



CREATE TABLE Computer (
  ComputerId          INTEGER NOT NULL PRIMARY KEY IDENTITY,
  DatePurchased       DATE NOT NULL,
  DateDecommissioned  DATE,
  Working             BIT NOT NULL,
  ModelName           varchar(80) NOT NULL,
  Manufacturer        varchar(80) NOT NULL
);
CREATE TABLE TrainingProgram (
  TrainingProgramId   INTEGER NOT NULL PRIMARY KEY IDENTITY,
  ProgramName         varchar(80) NOT NULL,
  StartDate           DATE NOT NULL,
  EndDate             DATE NOT NULL,
  MaximumAttendees    INTEGER NOT NULL
);
CREATE TABLE Department (
  DepartmentId        INTEGER NOT NULL PRIMARY KEY IDENTITY,
  DepartmentName      varchar(80) NOT NULL,
  ExpenseBudget       INTEGER NOT NULL
);
CREATE TABLE Employee (
  EmployeeId          INTEGER NOT NULL PRIMARY KEY IDENTITY,
  FirstName           varchar(30) NOT NULL,
  LastName            varchar(30) NOT NULL,
  Email               varchar(50) NOT NULL,
  Supervisor          BIT NOT NULL,
  StartDate           DATE NOT NULL,
  EndDate			  DATE,
  DepartmentId        INTEGER NOT NULL,
  CONSTRAINT FK_DepartmentEmployee FOREIGN KEY(DepartmentId)REFERENCES Department(DepartmentId)
);
CREATE TABLE EmployeeTraining (
  EmployeeTrainingId  INTEGER NOT NULL PRIMARY KEY IDENTITY,
  TrainingProgramId   INTEGER NOT NULL,
  EmployeeId          INTEGER NOT NULL,
  CONSTRAINT FK_TrainingProgram FOREIGN KEY(TrainingProgramId)REFERENCES TrainingProgram(TrainingProgramId),
  CONSTRAINT FK_EmployeeTraining FOREIGN KEY(EmployeeId)REFERENCES Employee(EmployeeId)
);
CREATE TABLE EmployeeComputer (
  EmployeeComputerId  INTEGER NOT NULL PRIMARY KEY IDENTITY,
  DateAssigned        DATE NOT NULL,
  DateReturned        DATE,
  ComputerId          INTEGER NOT NULL,
  EmployeeId          INTEGER NOT NULL,
  CONSTRAINT FK_ComputerEmployee FOREIGN KEY(ComputerId)REFERENCES Computer(ComputerId),
  CONSTRAINT FK_EmployeeComp FOREIGN KEY(EmployeeId)REFERENCES Employee(EmployeeId)
);
CREATE TABLE Customer (
  CustomerId          INTEGER NOT NULL PRIMARY KEY IDENTITY,
  FirstName           varchar(30) NOT NULL,
  LastName            varchar(30)  NOT NULL,
  Email               varchar(50) NOT NULL,
  Address             varchar(80) NOT NULL,
  City                varchar(30) NOT NULL,
  State               varchar(2) NOT NULL,
  AcctCreationDate    DATE NOT NULL,
  LastLogin           DATE NOT NULL
);
CREATE TABLE ProductType (
  ProductTypeId       INTEGER NOT NULL PRIMARY KEY IDENTITY,
  ProductTypeName     varchar(30) NOT NULL
);
CREATE TABLE PaymentType (
  PaymentTypeId       INTEGER NOT NULL PRIMARY KEY IDENTITY,
  PaymentTypeName     varchar(15) NOT NULL
);
CREATE TABLE CustomerPayment (
  CustomerPaymentId   INTEGER NOT NULL PRIMARY KEY IDENTITY,
  CardNumber          BIGINT NOT NULL,
  CcvCode             varchar(3) NOT NULL,
  ExpirationDate      varchar(5) NOT NULL,
  PaymentTypeId       INTEGER NOT NULL,
  CustomerId          INTEGER NOT NULL,
  CONSTRAINT FK_CustomerPayment FOREIGN KEY(CustomerId)REFERENCES Customer(CustomerId),
  CONSTRAINT FK_PaymentTypeCustomer FOREIGN KEY(PaymentTypeId)REFERENCES PaymentType(PaymentTypeId)
);
CREATE TABLE [Order] (
  OrderId             INTEGER NOT NULL PRIMARY KEY IDENTITY,
  CustomerId          INTEGER NOT NULL,
  CustomerPaymentId       INTEGER,
  CONSTRAINT FK_CustomerOrder FOREIGN KEY(CustomerId)REFERENCES Customer(CustomerId),
  CONSTRAINT FK_CustomerPaymentOrder FOREIGN KEY(CustomerPaymentId)REFERENCES CustomerPayment(CustomerPaymentId)
);
CREATE TABLE Product (
  ProductId           INTEGER NOT NULL PRIMARY KEY IDENTITY,
  Price               DECIMAL(4, 2) NOT NULL,
  Title               varchar(55) NOT NULL,
  Description         varchar(400) NOT NULL,
  CustomerId          INTEGER NOT NULL,
  Quantity            INTEGER NOT NULL,
  ProductTypeId       INTEGER NOT NULL,
  CONSTRAINT FK_CustomerProduct FOREIGN KEY(CustomerId)REFERENCES Customer(CustomerId),
  CONSTRAINT FK_ProductTypeProduct FOREIGN KEY(ProductTypeId)REFERENCES ProductType(ProductTypeId)
);
CREATE TABLE ProductOrder (
  ProductOrderId      INTEGER NOT NULL PRIMARY KEY IDENTITY,
  OrderId             INTEGER NOT NULL,
  ProductId           INTEGER NOT NULL,
  CONSTRAINT FK_OrderProductOrder FOREIGN KEY(OrderId)REFERENCES [Order](OrderId),
  CONSTRAINT FK_ProductProductOrder FOREIGN KEY(ProductId)REFERENCES Product(ProductId)
);
INSERT INTO Computer(DatePurchased, DateDecommissioned, Working, ModelName, Manufacturer)
VALUES 
(
 '2017/10/11', 
 null,
1,
'XPS',
'Dell');
INSERT INTO Computer(DatePurchased, DateDecommissioned, Working, ModelName, Manufacturer)
VALUES( 
 '2016/04/01', 
'2017/04/15', 
0,
'ThinkPad',
'Lenovo');
INSERT INTO TrainingProgram(ProgramName,StartDate,EndDate,MaximumAttendees)
VALUES( 
 'Learn To Type', 
'2018/09/20', 
'2018/09/27', 
23);
INSERT INTO TrainingProgram(ProgramName,StartDate,EndDate, MaximumAttendees)
VALUES(  
 'Begining React', 
'2017/02/14', 
'2017/03/01', 
14);
INSERT INTO Department(DepartmentName,ExpenseBudget)
VALUES( 
 'CodeRockstars', 
140234);
INSERT INTO Department(DepartmentName,ExpenseBudget)
VALUES( 
 'IT', 
23400);
INSERT INTO Employee(FirstName,LastName,Email, StartDate, Supervisor, DepartmentId)
VALUES( 
 'William', 
 'Kimball', 
'wkkimball043@gmail.com',
'2015/10/01', 
1,
1);
INSERT INTO Employee(FirstName,LastName,Email, StartDate, Supervisor, DepartmentId)
VALUES(  
 'Robert', 
 'Leedy', 
'rleedy@gmail.com',
'2010/04/25', 
0,
2);
INSERT INTO EmployeeTraining(TrainingProgramId,EmployeeId)
VALUES(  
1,
2);
INSERT INTO EmployeeTraining(TrainingProgramId,EmployeeId)
VALUES(  
2,
1);
INSERT INTO EmployeeComputer(DateAssigned,DateReturned,EmployeeId,ComputerId)
VALUES( 
'2017/02/14', 
'2017/03/01', 
2,
1);
INSERT INTO EmployeeComputer(DateAssigned,DateReturned,EmployeeId,ComputerId)
VALUES(  
'2017/10/07', 
'2017/03/01', 
1,
2);
INSERT INTO Customer(FirstName,LastName,Email, Address,City,State,AcctCreationDate,LastLogin)
VALUES(  
 'Sathvik', 
 'Reddy', 
'sr@gmail.com',
'123 Main St.',
'Nashville',
'TN',
'2014/03/01', 
'2018/03/01');
INSERT INTO Customer(FirstName,LastName,Email, Address,City,State,AcctCreationDate,LastLogin)
VALUES(  
 'Natasha', 
 'Cox', 
'ncox@gmail.com',
'123 Side St.',
'Nashville',
'TN',
'2012/01/14', 
'2018/07/23');
INSERT INTO ProductType(ProductTypeName)
VALUES(
'KnitCap');
INSERT INTO ProductType(ProductTypeName)
VALUES(
'Craft Thing');
INSERT INTO PaymentType ( PaymentTypeName)
VALUES(
'Visa');
INSERT INTO PaymentType(PaymentTypeName)
VALUES(
'MasterCard');
INSERT INTO CustomerPayment (CardNumber,CcvCode,ExpirationDate,PaymentTypeId,CustomerId)
VALUES(
6798620123,
'345',
'03/19',
2,
2);
INSERT INTO CustomerPayment(CardNumber,CcvCode,ExpirationDate,PaymentTypeId,CustomerId)
VALUES(
12343211432,
'213',
'06/23',
1,
1);
INSERT INTO [Order](CustomerPaymentId,CustomerId)
VALUES(
2,
2);
INSERT INTO [Order](CustomerId)
VALUES(
1);
INSERT INTO Product(Price,Title,Description,Quantity,CustomerId,ProductTypeId)
VALUES(
23.40,
'Cap',
'Warm Winter Cap', 
1,
2,
1);
INSERT INTO Product (Price,Title,Description,Quantity,CustomerId,ProductTypeId)
VALUES(
13.23,
'Painting',
'Painting of the ocean', 
2,
1,
2);
INSERT INTO ProductOrder(OrderId, ProductId)
VALUES(
1,
2);
INSERT INTO ProductOrder(OrderId, ProductId)
VALUES(
1,
2);
INSERT INTO Computer(DatePurchased, DateDecommissioned, Working, ModelName, Manufacturer)
VALUES( 
 '2018/12/11', 
 null,
3,
'Pro',
'Mac');
INSERT INTO TrainingProgram(ProgramName,StartDate,EndDate, MaximumAttendees)
VALUES(  
 'Mastering SQL', 
'2018/07/12', 
'2018/07/13', 
5);
INSERT INTO Department(DepartmentName,ExpenseBudget)
VALUES( 
 'Sales', 
24000);
INSERT INTO Employee(FirstName,LastName,Email, StartDate, Supervisor, DepartmentId)
VALUES(  
 'Seth', 
 'Dana', 
'sd@gmail.com',
'2013/01/13', 
0,
3);
INSERT INTO EmployeeTraining(TrainingProgramId,EmployeeId)
VALUES( 
3,
3);
INSERT INTO EmployeeComputer(DateAssigned,DateReturned,EmployeeId,ComputerId)
VALUES(  
'2017/02/14', 
'2018/03/01', 
3,
3);
INSERT INTO Customer(FirstName,LastName,Email, Address,City,State,AcctCreationDate,LastLogin)
VALUES( 
 'Walter', 
 'White', 
'ww@gmail.com',
'123 5th St.',
'Nashville',
'TN',
'2016/03/01', 
'2018/03/01');
INSERT INTO ProductType(ProductTypeName)
VALUES(
'Poem');
INSERT INTO PaymentType (PaymentTypeName)
VALUES(
'Discover');
INSERT INTO CustomerPayment(CardNumber,CcvCode,ExpirationDate,PaymentTypeId,CustomerId)
VALUES(
12343512432,
'133',
'06/33',
3,
3)
INSERT INTO [Order](CustomerPaymentId,CustomerId)
VALUES(
3,
3);
INSERT INTO Product (Price,Title,Description,Quantity,CustomerId,ProductTypeId)
VALUES(
21,
'Love Poem',
'Heart achingly beautiful poem', 
3,
3,
3);
INSERT INTO ProductOrder (OrderId, ProductId)
VALUES(
3,
3);
