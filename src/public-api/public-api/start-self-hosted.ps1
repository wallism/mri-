# just an example of an alternative way to start, now using compose
dapr run `
 --app-id public-api `
 --app-port 5200 `
 --dapr-http-port 3501 `
 --components-path ../../dapr/components `
 dotnet run 
