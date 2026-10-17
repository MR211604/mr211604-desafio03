FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Pedidos.Entities/Pedidos.Entities.csproj Pedidos.Entities/
COPY Pedidos.DAL/Pedidos.DAL.csproj Pedidos.DAL/
COPY Pedidos.BL/Pedidos.BL.csproj Pedidos.BL/
COPY mr211604-desafio03/Pedidos.API.csproj mr211604-desafio03/
RUN dotnet restore mr211604-desafio03/Pedidos.API.csproj

COPY Pedidos.Entities/ Pedidos.Entities/
COPY Pedidos.DAL/ Pedidos.DAL/
COPY Pedidos.BL/ Pedidos.BL/
COPY mr211604-desafio03/ mr211604-desafio03/
RUN dotnet publish mr211604-desafio03/Pedidos.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Pedidos.API.dll"]
