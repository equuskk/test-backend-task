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

# Эндпоинты:
* `POST http://localhost:8080/report/user_statistics`. Присутствует валидация на корректный guid (`userId`), начальная дата (`fromDate`) должна быть меньше конечной даты (`toDate`). Пример тела запроса:
```json
{
    "userId": "65cee085-24bc-47e4-8f92-40edfe1a6198",
    "fromDate": "2022-07-08T08:05:50.394877+00:00",
    "toDate": "2022-07-10T08:05:51.394877+00:00"
}
```
* `GET http://localhost:8080/report/info?id={{report_guid}}`. Пример запроса:

`http://localhost:8080/report/info?id=a36c6643-25eb-4a1e-932c-675daeb79599`
