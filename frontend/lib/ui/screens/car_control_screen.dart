import 'dart:async';
import 'dart:typed_data';
import 'dart:ui' as ui;
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/car_control_provider.dart';
import '../../services/websocket_service.dart';
import '../widgets/gamepad_widget.dart';
import '../widgets/lamp_widget.dart';
import '../../utils/constants.dart';

class CarControlScreen extends StatefulWidget {
  @override
  _CarControlScreenState createState() => _CarControlScreenState();
}

class _CarControlScreenState extends State<CarControlScreen> {
  static const String streamUrl = "ws://192.168.0.165:8181/stream";
  late WebSocketService _webSocketService;
  ui.Image? _currentImage;
  Uint8List? _nextImageData;

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
    final carControlProvider = Provider.of<CarControlProvider>(context);

    return Scaffold(
      appBar: AppBar(
        title: Text('Car Control'),
        actions: [
          IconButton(
            icon: Icon(Icons.notifications),
            onPressed: () {
              carControlProvider.receiveNotifications();
            },
          ),
          IconButton(
            icon: Icon(Icons.history),
            onPressed: () {
              carControlProvider.getCarLog();
            },
          ),
          IconButton(
            icon: Icon(Icons.exit_to_app),
            onPressed: () {
              carControlProvider.signOut();
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
                  Container(
                    width: constraints.maxWidth > kWebWidth ? kWebWidth : kMobileWidth,
                    height: constraints.maxHeight > kWebHeight ? kWebHeight : kMobileHeight,
                    decoration: BoxDecoration(
                      border: Border.all(color: Colors.blueAccent),
                      borderRadius: BorderRadius.circular(10),
                    ),
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(10),
                      child: CustomPaint(
                        painter: FramePainter(_currentImage!),
                      ),
                    ),
                  ),
                SizedBox(height: 20),
                Expanded(
                  child: constraints.maxWidth > kWebWidth
                      ? _buildWebLayout(carControlProvider)
                      : _buildMobileLayout(carControlProvider),
                ),
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildMobileLayout(CarControlProvider carControlProvider) {
    return Column(
      children: [
        GamepadWidget(),
        SizedBox(height: 20),
        Text('Flash Intensity'),
        Slider(
          value: carControlProvider.flashIntensity.toDouble(),
          min: 0,
          max: 100,
          divisions: 5,
          label: carControlProvider.flashIntensity.round().toString(),
          onChanged: (value) {
            carControlProvider.setFlashIntensity(value.round());
          },
        ),
        SizedBox(height: 20),
        Expanded(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              LampWidget(),
              SizedBox(height: 20),
              _buildControlButtons(carControlProvider),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildWebLayout(CarControlProvider carControlProvider) {
    return Row(
      children: [
        Expanded(
          child: Column(
            children: [
              GamepadWidget(),
              SizedBox(height: 20),
              Text('Flash Intensity'),
              Slider(
                value: carControlProvider.flashIntensity.toDouble(),
                min: 0,
                max: 100,
                divisions: 5,
                label: carControlProvider.flashIntensity.round().toString(),
                onChanged: (value) {
                  carControlProvider.setFlashIntensity(value.round());
                },
              ),
            ],
          ),
        ),
        Expanded(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              LampWidget(),
              SizedBox(height: 20),
              _buildControlButtons(carControlProvider),
            ],
          ),
        ),
      ],
    );
  }

  Widget _buildControlButtons(CarControlProvider carControlProvider) {
    return Column(
      children: [
        ElevatedButton(
          onPressed: () {
            carControlProvider.startStream();
            _connectToWebSocket();
          },
          child: Text('Start Stream'),
          style: ElevatedButton.styleFrom(
            padding: EdgeInsets.symmetric(horizontal: 20, vertical: 15),
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(30)),
          ),
        ),
        SizedBox(height: 20),
        ElevatedButton(
          onPressed: () {
            carControlProvider.stopStream();
            _disconnectFromWebSocket();
          },
          child: Text('Stop Stream'),
          style: ElevatedButton.styleFrom(
            padding: EdgeInsets.symmetric(horizontal: 20, vertical: 15),
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(30)),
          ),
        ),
        SizedBox(height: 20),
        ElevatedButton(
          onPressed: () => carControlProvider.sendCommand('car/control', '7'),
          child: Text('Auto Drive', style: TextStyle(fontSize: 18)),
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.green,
            padding: EdgeInsets.symmetric(horizontal: 20, vertical: 15),
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(30)),
            elevation: 10,
          ),
        ),
        SizedBox(height: 20),
        ElevatedButton(
          onPressed: () => carControlProvider.sendCommand('car/led/control', 'on'),
          child: Text('Turn On Lights', style: TextStyle(fontSize: 18)),
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.orange,
            padding: EdgeInsets.symmetric(horizontal: 20, vertical: 15),
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(30)),
            elevation: 10,
          ),
        ),
        SizedBox(height: 20),
        ElevatedButton(
          onPressed: () => carControlProvider.sendCommand('car/led/control', 'off'),
          child: Text('Turn Off Lights', style: TextStyle(fontSize: 18)),
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.black12,
            padding: EdgeInsets.symmetric(horizontal: 20, vertical: 15),
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(30)),
            elevation: 10,
          ),
        ),
        SizedBox(height: 20),
        ElevatedButton(
          onPressed: () => carControlProvider.sendCommand('car/led/control', 'auto'),
          child: Text('Auto Light Mode', style: TextStyle(fontSize: 18)),
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.purple,
            padding: EdgeInsets.symmetric(horizontal: 20, vertical: 15),
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(30)),
            elevation: 10,
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

class FramePainter extends CustomPainter {
  final ui.Image image;

  FramePainter(this.image);

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint();
    canvas.drawImage(image, Offset.zero, paint);
  }

  @override
  bool shouldRepaint(FramePainter oldDelegate) {
    return oldDelegate.image != image;
  }
}
