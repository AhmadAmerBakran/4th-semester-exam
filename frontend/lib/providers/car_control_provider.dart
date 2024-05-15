import 'package:flutter/material.dart';
import '../services/websocket_service.dart';

enum LightState { off, on, auto }

class CarControlProvider with ChangeNotifier {
  final WebSocketService webSocketService;
  LightState _lightState = LightState.off;

  CarControlProvider({required this.webSocketService});

  LightState get lightState => _lightState;

  void sendCommand(String topic, String command) {
    webSocketService.sendCarControlCommand(topic, command);
    if (topic == 'car/led/control') {
      if (command == 'on') {
        _lightState = LightState.on;
      } else if (command == 'off') {
        _lightState = LightState.off;
      } else if (command == 'auto') {
        _lightState = LightState.auto;
      }
      notifyListeners();
    }
  }

  void signIn(String nickName) {
    webSocketService.sendSignIn(nickName);
  }

  void signOut() {
    webSocketService.sendSignOut();
  }

  void receiveNotifications() {
    webSocketService.sendReceiveNotifications();
  }

  void getCarLog() {
    webSocketService.sendGetCarLog();
  }
}