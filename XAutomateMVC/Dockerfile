FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /srv/www/XAutomate

# Copy csproj and restore as distinct layers
#COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
#ARG source
#RUN apk add docker
WORKDIR /srv/www/XAutomate
RUN apt-get update
RUN  apt-get install python3 -y
RUN apt-get install python3-pip -y
RUN   pip3 install robotframework==3.2.2
RUN  pip3 install robotframework-seleniumlibrary
RUN  pip3 install robotframework-reportportal-ng
#`RUN  pip3 install robotframework-ride
RUN  pip3 install -U robotframework-databaselibrary
RUN pip3 install pymysql
RUN apt-get  install git -y
RUN pip3 install git+https://github.com/reportportal/agent-Python-RobotFramework.git
RUN pip3 install robotframework-listenerlibrary
#RUN  git clone https://github.com/pyexcel/pyexcel.git
#RUN cd pyexcel
RUN pip3 install robotframework-excellib
RUN pip3 install robotframework-excellib
RUN pip3 install pyexcel
RUN apt-get install python3.7-dev -y
RUN apt-get install python3-mysqldb -y
RUN apt-get install default-mysql-client -y
#RUN  apt-get install mysql-client
RUN apt-get install default-libmysqlclient-dev -y
RUN apt-get install libssl-dev -y
#RUN apt-get install libmysqlclient-dev -y
RUN  pip3 install mysqlclient
RUN pip3 install dbbot-sqlalchemy
#RUN  systemctl start docker
#RUN systemctl enable docker
COPY  --from=build-env /srv/www/XAutomate/out  .

ENTRYPOINT ["dotnet", "XAutomateMVC.dll"]
