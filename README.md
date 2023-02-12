# GeckosoftChallenge
REST API to manage the persistence of images uploaded by anonymous users.

The solution was developed using the ASP.NET framework. 

The solution is based on the MVC paradigm.



The main components are the controller (`ImageController.cs`), for handling API calls, and the repository (`ImageRepository.cs`), useful to encapsulate the persistence logic.

## API End points

[GET]
https://localhost:5000/api/Image/all

[POST]
https://localhost:5000/api/Image
`body:{image:<imageFile>}`

The POST response with a JSON containing the file name and an ID used for the DELETE and PUT requests.

[DELETE]
https://localhost:5000/api/Image/id

[PUT]
https://localhost:5000/api/Image/id
`body:{"id":<id>, "height":<new height>, width:<new width>}`

## How to run
```
git clone https://github.com/gasparax/GeckosoftChallenge
```
```
cd GeckosoftChallenge
```
### Start the application
```
dotnet run --project GeckosoftChallenge
```
The API can be manually tested through swagger interface:
```
https://localhost:5000/swagger/index.html
```
### Run the tests
```
dotnet test
```