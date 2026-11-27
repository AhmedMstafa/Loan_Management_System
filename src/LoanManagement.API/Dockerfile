# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Loan_Management_System.slnx", "./"]
COPY ["src/LoanManagement.Domain/LoanManagement.Domain.csproj", "src/LoanManagement.Domain/"]
COPY ["src/LoanManagement.Application/LoanManagement.Application.csproj", "src/LoanManagement.Application/"]
COPY ["src/LoanManagement.Infrastructure/LoanManagement.Infrastructure.csproj", "src/LoanManagement.Infrastructure/"]
COPY ["src/LoanManagement.API/LoanManagement.API.csproj", "src/LoanManagement.API/"]

RUN dotnet restore "src/LoanManagement.API/LoanManagement.API.csproj"

COPY . .
WORKDIR "/src/src/LoanManagement.API"
RUN dotnet publish "LoanManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LoanManagement.API.dll"]
