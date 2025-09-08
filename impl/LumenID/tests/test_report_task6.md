# How to test
1. Run mysql cluster with docker.
    ```ps1
    cd ./containers/mysql
    start.ps1
    ```
2. Run backend server.
    ```ps1
    cd ./src/LumenID.Backend
    dotnet run
    ```
3. Run frontend server.
    ```ps1
    cd ./src/LumenID.Frontend
    dotnet run
    ```
4. Open frontend page.
    ```ps1
    ...
    info: Microsoft.Hosting.Lifetime[14]
        Now listening on: http://localhost:PORT <-- ctrl + click here
    info: Microsoft.Hosting.Lifetime[0]
        Application started. Press Ctrl+C to shut down.
    info: Microsoft.Hosting.Lifetime[0]
    ...
    ```
5. Go to `register` page.
6. Input email and password, then press register button.
7. Check redirect to login page.
8. In login page, insert same data for 5.
9. Press login button.
10. Check successfully get session data from backend.

# Result
- [x]: Login action
- [x]: Base data generation
- [x]: Generate session per login button click
