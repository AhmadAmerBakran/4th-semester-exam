import 'dart:typed_data';

import 'package:flutter/material.dart';
import '../models/user.dart';
import '../services/websocket_service.dart';


class CarControlProvider with ChangeNotifier {
  final WebSocketService webSocketService;


  CarControlProvider({required this.webSocketService});


  void sendCommand(String topic, String command) {
    webSocketService.sendCarControlCommand(topic, command);
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

  void reconnect(String url, Function(String) onMessageReceived, Function(Uint8List) onBinaryMessageReceived) {
    webSocketService.init(url, onMessageReceived, onBinaryMessageReceived);
  }

  void sendAICommand(String command) {
    webSocketService.sendAICommand(command);
  }
}
