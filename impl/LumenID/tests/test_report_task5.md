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
7. Check data on database using MySQL Shell for VSCode extension.
8. Resend same data.
9. Check email conflict detected.

# Result
- [x]: Register action
- [x]: Base data generation
- [x]: Account conflict detect
