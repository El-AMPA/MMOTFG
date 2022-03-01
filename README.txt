Para ejecutar el servidor sql en docker:
1.Ejecutar docker
2.usar 

docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=Password' -e 'MSSQL_PID=Express' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2017-latest-ubuntu

sustituyendo password por una contraseña valida (+ de 8 caracteres, con simbolos/mayusculas y números)