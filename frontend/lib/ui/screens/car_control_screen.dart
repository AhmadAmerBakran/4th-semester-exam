import 'dart:async';
import 'dart:typed_data';
import 'dart:ui' as ui;
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/car_control_provider.dart';
import '../../services/websocket_service.dart';
import '../widgets/flash_intensity_slider.dart';
import '../widgets/stream_widget.dart';
import '../widgets/control_buttons.dart';
import '../widgets/gamepad_widget.dart';
import '../widgets/lamp_widget.dart';

class CarControlScreen extends StatefulWidget {
  @override
  _CarControlScreenState createState() => _CarControlScreenState();
}

class _CarControlScreenState extends State<CarControlScreen> {
  static const String streamUrl = "ws://192.168.0.165:8181/stream";
  late WebSocketService _webSocketService;
  ui.Image? _currentImage;

  @override
  void initState() {
    super.initState();
    _connectToWebSocket();
  }

  void _onMessageReceived(String message) {
    print("Received text message: $message");
  }

  void _onBinaryMessageReceived(Uint8List message) async {
    final decodedImage = await _decodeImageFromList(message);
    setState(() {
      _currentImage = decodedImage;
    });
  }

  Future<ui.Image> _decodeImageFromList(Uint8List list) async {
    final Completer<ui.Image> completer = Completer();
    ui.decodeImageFromList(list, (ui.Image img) {
      completer.complete(img);
    });
    return completer.future;
  }

  void _connectToWebSocket() {
    _webSocketService = WebSocketService(
      streamUrl,
      _onMessageReceived,
      _onBinaryMessageReceived,
    );
    print("Connected to WebSocket");
  }

  void _disconnectFromWebSocket() {
    _webSocketService.close();
    print("Disconnected from WebSocket");
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('Car Control'),
        actions: [
          IconButton(
            icon: Icon(Icons.notifications),
            onPressed: () {
              context.read<CarControlProvider>().receiveNotifications();
            },
          ),
          IconButton(
            icon: Icon(Icons.history),
            onPressed: () {
              context.read<CarControlProvider>().getCarLog();
            },
          ),
          IconButton(
            icon: Icon(Icons.exit_to_app),
            onPressed: () {
              context.read<CarControlProvider>().signOut();
              Navigator.pushReplacementNamed(context, '/');
            },
          ),
        ],
      ),
      body: LayoutBuilder(
        builder: (context, constraints) {
          return Padding(
            padding: const EdgeInsets.all(20.0),
            child: Column(
              children: [
                if (_currentImage != null)
                  StreamWidget(currentImage: _currentImage!),
                SizedBox(height: 20),
                Expanded(
                  child: constraints.maxWidth > 600
                      ? _buildWebLayout()
                      : _buildMobileLayout(),
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildMobileLayout() {
    return Column(
      children: [
        GamepadWidget(),
        SizedBox(height: 20),
        Text('Flash Intensity'),
        FlashIntensitySlider(),
        Expanded(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              LampWidget(),
              SizedBox(height: 20),
              ControlButtons(),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildWebLayout() {
    return Row(
      children: [
        Expanded(
          child: Column(
            children: [
              GamepadWidget(),
              SizedBox(height: 20),
              Text('Flash Intensity'),
              FlashIntensitySlider(),
            ],
          ),
        ),
        Expanded(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              LampWidget(),
              SizedBox(height: 20),
              ControlButtons(),
            ],
          ),
        ),
      ],
    );
  }

  @override
  void dispose() {
    _webSocketService.close();
    super.dispose();
  }
}
