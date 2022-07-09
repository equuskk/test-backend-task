# Запуск приложения:
* В докере:
```
docker build -t test-back .
docker run -dp 8080:8080 test-back
```
* При наличии установленного net6:
```
cd .\TestBackendTask.App\
dotnet run
```
После этого проект будет запущен на 8080 порту: http://localhost:8080