FROM microsoft/dotnet:2.2-sdk

RUN apt-get update
RUN apt-get install -y gnupg git procps

# language server
RUN curl -sL https://deb.nodesource.com/setup_11.x | bash -
RUN apt-get install -y nodejs

WORKDIR /
RUN git clone https://github.com/georgewfraser/fsharp-language-server
WORKDIR /fsharp-language-server

RUN npm install
RUN dotnet build -c Release
RUN echo "#/bin/bash" > /usr/local/bin/fls
RUN echo "dotnet /fsharp-language-server/src/FSharpLanguageServer/bin/Release/netcoreapp2.0/FSharpLanguageServer.dll" >> /usr/local/bin/fls
RUN chmod a+x /usr/local/bin/fls
# Hack based on https://github.com/fsprojects/fsharp-language-server/issues/49#issuecomment-482853772
RUN ln -s /usr/share/dotnet/sdk/2.2.203/Current /usr/share/dotnet/sdk/2.2.203/15.0

ARG PROJECT_DIR

ADD . $PROJECT_DIR
WORKDIR $PROJECT_DIR
