import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/car_control_provider.dart';
import '../widgets/gamepad_widget.dart';
import '../widgets/lamp_widget.dart';

class CarControlScreen extends StatelessWidget {
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
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            LampWidget(),
            SizedBox(height: 20),
            GamepadWidget(),
          ],
        ),
      ),
    );
  }
}