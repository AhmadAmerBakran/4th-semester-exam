## Project Name
**Car Control**

## Purpose
The purpose of the Car Control project is to control a car connected to two ESP32 devices:

- **ESP32 Device**: Controls the movement of the car, lights, and sensors.
- **ESP32-CAM**: Streams video and controls the camera's integrated flashlight.

The entire system uses MQTT and WebSocket connections for communication.

## Technologies Used
- **Backend**: Developed using C# 8.0.x with the following NuGets:
  - "Fleck" Version="1.2.0"
  - "uldahlalex.websocket.boilerplate" Version="1.6.0"
  - "Npgsql.DependencyInjection" Version="8.0.2"
  - "Npgsql" Version="8.0.2"
  - "Dapper" Version="2.1.35"
  - "MQTTnet" Version="4.3.3.952"

- **Frontend**: Developed using Flutter with the following packages:
  - cupertino_icons: ^1.0.2
  - provider: ^6.1.2
  - web_socket_channel: ^3.0.0
  - animated_button: ^0.2.0
  - flutter_animate: ^4.5.0
  - carousel_slider: ^4.2.1
  - image_gallery_saver: ^2.0.3
  - permission_handler: ^11.3.1
  - google_fonts: ^6.2.1

- **IoT**: ESP32 & ESP32-CAM and other sensors and components such as:
  - HC-SR04 Ultrasonic Distance Module
  - Servo Micro Motor 9G SG92R 2.5KG
  - L298N Motor Driver Module
  - Two DC motors

## Project Overview
This project is a 3 in 1 exam project focusing on the concepts we learned in our 4th semester. It emphasizes WebSocket connections and real-time communication, developing mobile applications using Flutter & Dart, and connecting non-internet devices with the internet using microcontrollers and MQTT broker. Therefore, no authentication or authorization is implemented or focused on in this project. The specific focus areas include:

#### Backend:
- Using WebSocket connections for full-duplex communication instead of HTTP requests and responses.
- Automating WebSocket to be triggered on certain commands (events).

#### Frontend:
- Developed using Flutter and Dart.
- Utilizing the widget tree structure.
- Implementing stateless and stateful widgets.
- Using providers for state management.
- Handling permissions.

#### IoT:
- Integrating devices with microcontrollers.
- Using MQTT broker to send commands, trigger certain variables, get data from the device, and aggregate data.

## How It Works

#### 1. Backend and ESP32 Devices Initialization
- When the backend runs, it opens a WebSocket server.
- Once we run the device (in our case, a car toy), the ESP32-CAM connects to the WebSocket server and both ESP devices connect to an MQTT broker (in our case, Flespi).

#### 2. Frontend Connection
- Once the app is opened, it establishes a connection with the backend over WebSocket.

#### 3. Command Flow
- The frontend does not interact directly with the ESP devices.
- The frontend sends commands (e.g., movement buttons, sliders, control buttons) to the backend via WebSocket.
- The backend receives these commands and sends them to the MQTT broker.
- The MQTT broker forwards the commands to the car, as the ESP devices have subscribed to the relevant topics.
- The car performs the commands (e.g., movement, auto-drive, lights on/off/auto, flash intensity, video streaming).
- Notifications from the car are sent to the broker, which forwards them to the backend.
- The backend sends notifications to the frontend if requested by the user.

#### 4. Database
- All notifications, commands sent to topics, commands coming from relevant topics, user IDs, and timestamps are registered in the database.

## Frontend Details
- Developed using Dart and Flutter.
- Two builds are available: one for mobile and one for web.
- When the frontend is turned on, it connects to the WebSocket server.
- Once connected, the user can enter a nickname and take control of the car as described above.
#### NOTE: The there may be a very small delay (around 5 frames) in video streaming due to buffer mechanism we have implemented to ensure smooth video when receiving the video stream

## Getting Started

#### Visit this site to take control over the car
[Car Control Web App](https://car-control-5dc32.web.app/)

#### If you choose to run the project by visiting the site, please make sure that the ESP devices are connected.

#### Responsible teachers can send us mails on school mail for turning on the car at the time they want.

#### [Ahmad Amer Bakran](mailto:ahmbak01@easv365.dk)
#### [Mahmoud Eybo](mailto:maheyb01@easv365.dk)

## Review the code

#### Clone the Repository
```sh
git clone https://github.com/AhmadAmerBakran/4th-semester-exam.git
```

#### Note: If you clone the code the frontend would connect with the deployed backend, so no need to run the backend, or you can change the constant file in frontend/utils/constants.dart to include your ip address to make the frontend communicate with the local backend.

#### If you choose to run the project locally you won't be able to communicate with the car, but only testing the mechanism of the commands flow due to some environment variables that you tippically don't have xd




## Running the deployed Application
1. Turn on the car and ensure both ESP devices connect to the MQTT broker.
2. Visit the site [Car Control Web App](https://car-control-5dc32.web.app/)
3. Enter a nickname (at least two charachters or numbers)
4. Hit start button and you are good to go

## Contributors
The project was developed by:
- [Ahmad Amer Bakran](https://github.com/AhmadAmerBakran)
- [Mahmoud Eybo](https://github.com/Hozaneybo)
