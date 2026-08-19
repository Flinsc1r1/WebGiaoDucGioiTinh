FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

# Thêm biến môi trường ép buộc .NET chạy ổn định trên container Linux
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "WebGiaoDucGioiTinh.dll"]