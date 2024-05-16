import 'dart:convert';
import 'package:web_socket_channel/web_socket_channel.dart';

class WebSocketService {
  late WebSocketChannel _channel;
  final Function(String) _onMessageReceived;
  final Function(List<int>) _onBinaryMessageReceived;

  WebSocketService(String url, this._onMessageReceived, this._onBinaryMessageReceived) {
    _channel = WebSocketChannel.connect(Uri.parse(url));

    _channel.stream.listen((message) {
      if (message is String) {
        _onMessageReceived(message);
      } else if (message is List<int>) {
        _onBinaryMessageReceived(message);
      }
    });
  }

  void sendMessage(Map<String, dynamic> message) {
    _channel.sink.add(jsonEncode(message));
  }

  void sendCarControlCommand(String topic, String command) {
    sendMessage({
      'eventType': 'ClientWantsToControlCar',
      'Topic': topic,
      'Command': command,
    });
  }

  void sendSignIn(String nickName) {
    sendMessage({
      'eventType': 'ClientWantsToSignIn',
      'NickName': nickName,
    });
  }

  void sendSignOut() {
    sendMessage({
      'eventType': 'ClientWantsToSignOut',
    });
  }

  void sendReceiveNotifications() {
    sendMessage({
      'eventType': 'ClientWantsToReceiveNotifications',
    });
  }

  void sendGetCarLog() {
    sendMessage({
      'eventType': 'ClientWantsToGetCarLog',
    });
  }

  void close() {
    _channel.sink.close();
  }
}
