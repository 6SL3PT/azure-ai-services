# Slip Intelligence

## Secret setup

1. Initiate user-secrets:
   ```
   dotnet user-secrets init
   ```
1. Add DI Endpoint:
   ```
   dotnet user-secrets set "Azure:DocumentIntelligence:Endpoint" "<YOUR_ENDPOINT>"
   ```
1. Add DI API Key:
   ```
   dotnet user-secrets set "Azure:DocumentIntelligence:ApiKey" "<YOUR_API_KEY>"
   ```
1. Add Blob Storage connection string:
   ```
   dotnet user-secrets set "Azure:BlobStorage:ConnectionString" "<YOUR_CONNECTION_STRING>"
   ```
