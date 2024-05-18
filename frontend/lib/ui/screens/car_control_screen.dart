import 'dart:async';
import 'dart:io';
import 'dart:ui' as ui;
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:provider/provider.dart';
import '../../providers/car_control_provider.dart';
import '../../providers/user_provider.dart';
import '../../services/websocket_service.dart';
import '../widgets/Aanimated_background.dart';
import '../widgets/animated_app_bar.dart';
import '../widgets/flash_intensity_slider.dart';
import '../widgets/notification_list_widget.dart';
import '../widgets/stream_container_widget.dart';
import '../widgets/control_buttons.dart';
import '../widgets/gamepad_widget.dart';
import '../../utils/constants.dart';

class CarControlScreen extends StatefulWidget {
  @override
  _CarControlScreenState createState() => _CarControlScreenState();
}

class _CarControlScreenState extends State<CarControlScreen> {
  static const String streamUrl = WEBSOCKET_URL;
  late WebSocketService _webSocketService;
  ui.Image? _currentImage;
  bool _isStreaming = false;

  @override
  void initState() {
    super.initState();
    print("CarControlScreen initialized");
    _setOrientation();
    _initWebSocket();
  }

  void _setOrientation() {
    if (!kIsWeb && Platform.isAndroid) {
      SystemChrome.setPreferredOrientations([DeviceOrientation.landscapeLeft]);
    }
  }

  void _onMessageReceived(String message) {
    print("Received text message: $message");
    if (message.startsWith("Notification on")) {
      context.read<CarControlProvider>().addNotification(message);
    }
  }

  void _onBinaryMessageReceived(Uint8List message) async {
    print("Received binary message of length: ${message.length}");
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

  void _initWebSocket() {
    _webSocketService = WebSocketService(
      streamUrl,
      _onMessageReceived,
      _onBinaryMessageReceived,
    );
    context.read<CarControlProvider>().receiveNotifications();
  }

  void _startStream() {
    print("Starting stream...");
    setState(() {
      _isStreaming = true;
    });
    print("Connected to WebSocket");
  }

  void _stopStream() {
    print("Stopping stream...");
    _webSocketService.close();
    setState(() {
      _isStreaming = false;
      _currentImage = null;
    });
    print("Disconnected from WebSocket");
  }

  @override
  Widget build(BuildContext context) {
    final userProvider = Provider.of<UserProvider>(context);
    return Scaffold(
      body: Stack(
        children: [
          Positioned.fill(
            child: AnimatedBackground(),
          ),
          Column(
            children: [
              AnimatedAppBar(
                title: userProvider.user?.nickname ?? 'Car Control',
                actions: [
                  Consumer<CarControlProvider>(
                    builder: (context, carControlProvider, child) {
                      return Stack(
                        children: [
                          IconButton(
                            icon: Icon(Icons.notifications),
                            onPressed: () {
                              context.read<CarControlProvider>().clearUnreadCount();
                              showModalBottomSheet(
                                context: context,
                                builder: (context) => NotificationList(),
                              );
                            },
                          ),
                          if (carControlProvider.unreadCount > 0)
                            Positioned(
                              right: 8,
                              top: 8,
                              child: Container(
                                padding: EdgeInsets.all(2),
                                decoration: BoxDecoration(
                                  color: Colors.red,
                                  borderRadius: BorderRadius.circular(12),
                                ),
                                constraints: BoxConstraints(
                                  minWidth: 16,
                                  minHeight: 16,
                                ),
                                child: Center(
                                  child: Text(
                                    '${carControlProvider.unreadCount}',
                                    style: TextStyle(
                                      color: Colors.white,
                                      fontSize: 10,
                                    ),
                                    textAlign: TextAlign.center,
                                  ),
                                ),
                              ),
                            ),
                        ],
                      );
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
              Expanded(
                child: LayoutBuilder(
                  builder: (context, constraints) {
                    return constraints.maxWidth > 800
                        ? _buildWebLayout()
                        : _buildMobileLayout();
                  },
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }


  Widget _buildMobileLayout() {
    return Column(
      children: [
        Expanded(
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            children: [
              Expanded(child: GamepadWidget()),
              Expanded(
                child: StreamContainer(
                  isStreaming: _isStreaming,
                  currentImage: _currentImage,
                ),
              ),
              Expanded(
                child: ControlButtons(
                  onStartStream: _startStream,
                  onStopStream: _stopStream,
                ),
              ),
            ],
          ),
        ),
        FlashIntensitySlider(),
      ],
    );
  }

  Widget _buildWebLayout() {
    return Column(
      children: [
        Expanded(
          child: StreamContainer(
            isStreaming: _isStreaming,
            currentImage: _currentImage,
          ),
        ),
        FlashIntensitySlider(),
        Expanded(
          child: Row(
            children: [
              Expanded(
                child: GamepadWidget(),
              ),
              Expanded(
                child: ControlButtons(
                  onStartStream: _startStream,
                  onStopStream: _stopStream,
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }

  @override
  void dispose() {
    if (_isStreaming) {
      _webSocketService.close();
    }
    SystemChrome.setPreferredOrientations([
      DeviceOrientation.portraitUp,
      DeviceOrientation.portraitDown,
      DeviceOrientation.landscapeLeft,
      DeviceOrientation.landscapeRight,
    ]);
    super.dispose();
  }
}
