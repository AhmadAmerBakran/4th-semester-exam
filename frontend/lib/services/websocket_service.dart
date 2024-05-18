import 'dart:async';
import 'dart:collection';
import 'dart:convert';
import 'dart:typed_data';
import 'package:web_socket_channel/web_socket_channel.dart';

class WebSocketService {
  late WebSocketChannel _channel;
  final Function(String) _onMessageReceived;
  final Function(Uint8List) _onBinaryMessageReceived;
  final Queue<Uint8List> _frameBuffer = Queue<Uint8List>();
  final int _bufferSize = 5;
  late Timer _frameTimer;

  WebSocketService(String url, this._onMessageReceived, this._onBinaryMessageReceived) {
    _channel = WebSocketChannel.connect(Uri.parse(url));
    _channel.stream.listen((message) {
      if (message is String) {
        _onMessageReceived(message);
      } else if (message is List<int>) {
        _addFrameToBuffer(Uint8List.fromList(message));
      }
    });
    _frameTimer = Timer.periodic(Duration(milliseconds: 100), (_) => _displayFrameFromBuffer());
  }

  void _addFrameToBuffer(Uint8List frame) {
    if (_frameBuffer.length >= _bufferSize) {
      _frameBuffer.removeFirst();
    }
    _frameBuffer.addLast(frame);
  }

  void _displayFrameFromBuffer() {
    if (_frameBuffer.isNotEmpty) {
      final frame = _frameBuffer.removeFirst();
      _onBinaryMessageReceived(frame);
    }
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
    _frameTimer.cancel();
    _channel.sink.close();
  }
}
