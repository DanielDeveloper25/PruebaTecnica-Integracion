
## Prueba tecnica API

1. Clonar proyecto
2. Abrir la solucion
3. Cambiar las credenciales en el ```appSetting.json```
4. Correr el siguiente comando en el package managment console para levantar la base de datos
```
Update-Database
```
5. Para iniciar el programa utilice los siguientes comandos
```
1. cd PruebaTecnicaAPI
2. dotnet run
```
6. Para probar el programa visite ```http://localhost:5277/swagger```
7. Y para realizar los test
```
1. cd RegistrationTest
2. dotnet test
```
