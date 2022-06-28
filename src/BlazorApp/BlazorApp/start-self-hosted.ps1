# just an example of an alternative way to start, now using compose
dapr run `
 --app-id blazorapp `
 --app-port 5000 `
 --dapr-http-port 3500 `
 --components-path ../../dapr/components `
 dotnet run 
