FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

# Copy everything and restore as distinct layers
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /App
COPY --from=build-env /App/out .
# For added security, you can opt out of the diagnostic pipeline. 
# ENV DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "QueueService.dll", "--launch-profile http"]