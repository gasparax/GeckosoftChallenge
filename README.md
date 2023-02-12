# GeckosoftChallenge
REST API to manage the persistence of images uploaded by anonymous users.

The solution was developed using the ASP.NET framework. 

The solution is based on the MVC paradigm.



The main components are the controller (`ImageController.cs`), for handling API calls, and the repository (`ImageRepository.cs`), useful to encapsulate the persistence logic.


## How to run
```
git clone https://github.com/gasparax/GeckosoftChallenge
```
### Start the application
```
cd GeckosoftChallenge
```
```
dotnet run --project GeckosoftChallenge
```
### Run the tests
```
dotnet test
```