import 'package:flutter/material.dart';
import '../models/user.dart';
import '../services/websocket_service.dart';

enum LightState { off, on, auto }

class CarControlProvider with ChangeNotifier {
  final WebSocketService webSocketService;
  LightState _lightState = LightState.off;
  int _flashIntensity = 0;
  int _carSpeed = 136;
  List<String> _notifications = [];
  int _unreadCount = 0;
  bool _notificationsEnabled = true;

  CarControlProvider({required this.webSocketService});

  LightState get lightState => _lightState;
  int get flashIntensity => _flashIntensity;
  int get carSpeed => _carSpeed;
  List<String> get notifications => _notifications;
  int get unreadCount => _unreadCount;
  bool get notificationsEnabled => _notificationsEnabled;

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

  void cycleLightState() {
    _lightState = _lightState == LightState.off
        ? LightState.on
        : _lightState == LightState.on
        ? LightState.auto
        : LightState.off;

    final command = _lightState == LightState.on
        ? 'on'
        : _lightState == LightState.off
        ? 'off'
        : 'auto';

    sendCommand('car/led/control', command);
  }

  void signIn(User user) {
    webSocketService.sendSignIn(user.nickname);
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

  void setCarSpeed(int value) {
    _carSpeed = value;
    sendCommand('car/speed', value.toString());
    notifyListeners();
  }

  void addNotification(String notification) {
    if (_notificationsEnabled && notification.startsWith('Notification on')) {
      _notifications.add(notification);
      _unreadCount++;
      notifyListeners();
    }
  }

  void clearUnreadCount() {
    _unreadCount = 0;
    notifyListeners();
  }

  void toggleNotifications(bool isEnabled) {
    _notificationsEnabled = isEnabled;
    notifyListeners();
  }
}