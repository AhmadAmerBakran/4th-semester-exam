import 'package:flutter/material.dart';
import '../services/websocket_service.dart';

enum LightState { off, on, auto }

class CarControlProvider with ChangeNotifier {
  final WebSocketService webSocketService;
  LightState _lightState = LightState.off;
  int _flashIntensity = 0;

  CarControlProvider({required this.webSocketService});

  LightState get lightState => _lightState;
  int get flashIntensity => _flashIntensity;

  void sendCommand(String topic, String command) {
    webSocketService.sendCarControlCommand(topic, command);
    if (topic == 'car/led/control') {
      _lightState = command == 'on'
          ? LightState.on
          : command == 'off'
          ? LightState.off
          : LightState.auto;
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

  void startStream() {
    webSocketService.sendCarControlCommand('cam/control', 'start');
  }

  void stopStream() {
    webSocketService.sendCarControlCommand('cam/control', 'stop');
  }

  void setFlashIntensity(int value) {
    _flashIntensity = value;
    sendCommand('cam/flash', value.toString());
    notifyListeners();
  }
}
