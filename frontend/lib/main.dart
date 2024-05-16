import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'providers/car_control_provider.dart';
import 'providers/user_provider.dart';
import 'services/websocket_service.dart';
import 'ui/screens/car_control_screen.dart';
import 'ui/screens/login_screen.dart';
import 'utils/constants.dart';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        ChangeNotifierProvider(create: (_) => UserProvider()),
        ChangeNotifierProvider(
          create: (context) => CarControlProvider(
            webSocketService: WebSocketService(
              WEBSOCKET_URL,
                  (message) {
                print('WebSocket message: $message');
              },
                  (binaryMessage) {
                WidgetsBinding.instance.addPostFrameCallback((_) {
                  // Process binary message here
                });
              },
            ),
          ),
        ),
      ],
      child: MaterialApp(
        title: 'Car Control App',
        theme: ThemeData(
          primarySwatch: Colors.blue,
        ),
        initialRoute: '/',
        routes: {
          '/': (context) => LoginScreen(),
          '/carControl': (context) => CarControlScreen(),
        },
      ),
    );
  }
}
