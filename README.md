# Keg Monitor

Keg monitoring and management system using an MQTT broker and IoT smart scales to report keg weight values and changes. Compliment a kegerator with this system to display what beer is on tap, how much beer remains within each keg, and detect when a beer is poured.

Massive shoutout and credit to Matt over at the [Troubled Brewing YouTube Channel](https://www.youtube.com/@TroubleBrewing/featured) for the [instructional video](https://www.youtube.com/watch?v=QF1B8yD9jy4) on building the smart scales that helped jumpstart this project. 

## Getting Started

Getting the full keg monitoring application and dependences up and running will require some hardware, minor technical and networking knowledge, and some beer of course.

### Prerequisites

* Keg(s) in a kegerator connected to a tap
* Smart scales using load cells with amplifier and microchip
  * Search online for 50kg Load Cell (pack of 4) with HX711 Amplifier
  * Wood plate on which to install load cells. See this [instructional video](https://www.youtube.com/watch?v=QF1B8yD9jy4) for building instructions and IoT setup.
* Linux machine to host application
  * It is recommended to run Linux and the commands below will be for Linux distributions; however, Windows can be supported running Docker for Windows or using WSL.
  * [Docker](https://www.docker.com/) or other flavor of running containers

### Installation

First, clone the repository or copy out the [docker-compose.yml](/docker-compose.yml). 

```
git clone https://github.com/sethsteenken/keg-monitor.git
```

Create new directory and copy docker-compose.yml.

```
mkdir server
cp docker-compose.yml /server/docker-compose.yml
```

Create an .env file in this directory. Note: you will not see the file in the directory.

```
touch .env
```

Set .env with the following content. Fill in the empty environment variables with values related to your setup. Feel free to change any of these defaults.
```
MQTT_HEALTH_PROBE_USER=healthcheck_user
MQTT_HEALTH_PROBE_PASSWORD=<password for health probe for the mqtt broker>
POSTGRES_USER=<postgres user>
POSTGRES_PASSWORD=<postgres password>
POSTGRES_VERSION=15.4
KEGMONITOR_CONN_STRING="Host=postgresql;Database=keg-monitor;Username=<postgres user>;Password=<postgres password>"
KEGMONITOR_DOMAIN=<custom local domain or IP address for the web application>
KEGMONITOR_MQTT_PASSWORD=<password for the mqtt broker app user>
TIMEZONE=America/New_York
```

Deploy the application and dependent containers using [Docker Compose](https://docs.docker.com/compose/)

```
sudo docker compose up -d
```

The containers should deploy successfully, but may result in unhealthy or stopped status. Perform the following setup steps:

Setup steps:

* Run commands on the MQTT broker container to establish the following:
  * health probe user healthcheck_user
  * web app user keg_monitor_web_user
  * client KM_Web_Sub
  * See [documentation for the MQTT broker](https://mosquitto.org/)
