#Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

RUN mkdir /app
COPY . /app
RUN cd /app && ls -l
RUN mkdir /build

#Artefact
RUN cd /app && dotnet publish WebApi/WebApi.csproj --self-contained true -r linux-x64 -o /build

#Final image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

#Environment Variables
ENV ASPNETCORE_URLS=https://+:____cfg_https_port____;http://+:____cfg_http_port____
ENV ASPNETCORE_ENVIRONMENT=____cfg_aspnetcore_environment____
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=____cfg_certificate_path____
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=____cfg_certificate_password____

RUN echo $ASPNETCORE_ENVIRONMENT
RUN echo $ASPNETCORE_URLS
RUN echo $ASPNETCORE_Kestrel__Certificates__Default__Path
RUN echo $ASPNETCORE_Kestrel__Certificates__Default__Password

RUN mkdir /https
WORKDIR /build
COPY --from=build /build .

RUN mkdir /var/log/ms_order
RUN cd /https && ls -l
RUN cd /build && ls -l

#CMD tail -f /dev/null
ENTRYPOINT ["./MicroserviceOrder"]