import 'dart:async';
import 'dart:collection';
import 'dart:convert';
import 'dart:typed_data';
import 'package:web_socket_channel/web_socket_channel.dart';
import '../models/events.dart';

class WebSocketService {
  static final WebSocketService _singleton = WebSocketService._internal();

  late WebSocketChannel _channel;
  final Queue<Uint8List> _frameBuffer = Queue<Uint8List>();
  final int _bufferSize = 5;
  late Timer _frameTimer;
  final int _reconnectDelay = 5;

  final StreamController<String> _messageStreamController = StreamController<String>.broadcast();
  final StreamController<Uint8List> _binaryMessageStreamController = StreamController<Uint8List>.broadcast();

  factory WebSocketService() {
    return _singleton;
  }

  WebSocketService._internal();

  Stream<String> get messageStream => _messageStreamController.stream;
  Stream<Uint8List> get binaryMessageStream => _binaryMessageStreamController.stream;

  void init(String url, Function(String) onMessageReceived, Function(Uint8List) onBinaryMessageReceived) {
    _connect(url, onMessageReceived, onBinaryMessageReceived);
  }

  void _connect(String url, Function(String) onMessageReceived, Function(Uint8List) onBinaryMessageReceived) {
    _channel = WebSocketChannel.connect(Uri.parse(url));
    _channel.stream.listen(
          (message) {
        if (message is String) {
          try {
            onMessageReceived(message);
            _messageStreamController.add(message);
          } catch (e) {
            print("Error parsing message: $e");
          }
        } else if (message is List<int>) {
          final binaryMessage = Uint8List.fromList(message);
          _addFrameToBuffer(binaryMessage);
          _binaryMessageStreamController.add(binaryMessage);
        }
      },
      onDone: () => _reconnect(url, onMessageReceived, onBinaryMessageReceived),
      onError: (error) => _reconnect(url, onMessageReceived, onBinaryMessageReceived),
    );
    _frameTimer = Timer.periodic(Duration(milliseconds: 100), (_) => _displayFrameFromBuffer(onBinaryMessageReceived));
  }

  void _reconnect(String url, Function(String) onMessageReceived, Function(Uint8List) onBinaryMessageReceived) {
    print('Connection lost. Attempting to reconnect in $_reconnectDelay seconds...');
    Future.delayed(Duration(seconds: _reconnectDelay), () {
      print('Reconnecting...');
      _connect(url, onMessageReceived, onBinaryMessageReceived);
    });
  }

  void _addFrameToBuffer(Uint8List frame) {
    if (_frameBuffer.length >= _bufferSize) {
      _frameBuffer.removeFirst();
    }
    _frameBuffer.addLast(frame);
  }

  void _displayFrameFromBuffer(Function(Uint8List) onBinaryMessageReceived) {
    if (_frameBuffer.isNotEmpty) {
      final frame = _frameBuffer.removeFirst();
      onBinaryMessageReceived(frame);
    }
  }

  void sendMessage(BaseEvent event) {
    _channel.sink.add(jsonEncode(event.toJson()));
  }

  void sendCarControlCommand(String topic, String command) {
    sendMessage(CarControlCommand(eventType: 'ClientWantsToControlCar', topic: topic, command: command));
  }

  void sendSignIn(String nickName) {
    sendMessage(SignInEvent(eventType: 'ClientWantsToSignIn', nickName: nickName));
  }

  void sendSignOut() {
    sendMessage(SignOutEvent(eventType: 'ClientWantsToSignOut'));
  }

  void sendReceiveNotifications() {
    sendMessage(ReceiveNotificationsEvent(eventType: 'ClientWantsToReceiveNotifications'));
  }

  void sendGetCarLog() {
    sendMessage(GetCarLogEvent(eventType: 'ClientWantsToGetCarLog'));
  }

  void close() {
    _frameTimer.cancel();
    //_channel.sink.close();
  }

  void sendAICommand(String command) {
    final event = CarControlCommand(
      eventType: 'ClientWantsToControlCar', // Ensure this matches the backend event type
      topic: 'car/control', // This is the MQTT topic for car control
      command: command, // This will be the command processed by the backend AI service
    );
    sendMessage(event);
  }


}
