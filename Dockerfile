FROM mcr.microsoft.com/dotnet/core/sdk:2.2

ADD src /srv
WORKDIR /srv

RUN dotnet restore
CMD dotnet run
