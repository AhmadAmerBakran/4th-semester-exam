import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/car_control_provider.dart';

class GamepadWidget extends StatefulWidget {
  @override
  _GamepadWidgetState createState() => _GamepadWidgetState();
}

class _GamepadWidgetState extends State<GamepadWidget> {
  @override
  Widget build(BuildContext context) {
    final carControlProvider = Provider.of<CarControlProvider>(context, listen: false);

    return LayoutBuilder(
      builder: (context, constraints) {
        return Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            GestureDetector(
              onTapDown: (_) => carControlProvider.sendCommand('car/control', '1'),
              onTapUp: (_) => carControlProvider.sendCommand('car/control', '0'),
              child: AnimatedContainer(
                duration: Duration(milliseconds: 200),
                height: constraints.maxWidth > 400 ? 80 : 60,
                width: constraints.maxWidth > 400 ? 80 : 60,
                decoration: BoxDecoration(
                  color: Colors.blue,
                  borderRadius: BorderRadius.circular(30),
                  boxShadow: [BoxShadow(color: Colors.black26, blurRadius: 10)],
                ),
                child: Icon(Icons.arrow_upward, size: 40, color: Colors.white),
              ),
            ),
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                GestureDetector(
                  onTapDown: (_) => carControlProvider.sendCommand('car/control', '6'),
                  onTapUp: (_) => carControlProvider.sendCommand('car/control', '0'),
                  child: AnimatedContainer(
                    duration: Duration(milliseconds: 200),
                    height: constraints.maxWidth > 400 ? 80 : 60,
                    width: constraints.maxWidth > 400 ? 80 : 60,
                    decoration: BoxDecoration(
                      color: Colors.blue,
                      borderRadius: BorderRadius.circular(30),
                      boxShadow: [BoxShadow(color: Colors.black26, blurRadius: 10)],
                    ),
                    child: Icon(Icons.arrow_back, size: 40, color: Colors.white),
                  ),
                ),
                SizedBox(width: 20),
                GestureDetector(
                  onTapDown: (_) => carControlProvider.sendCommand('car/control', '5'),
                  onTapUp: (_) => carControlProvider.sendCommand('car/control', '0'),
                  child: AnimatedContainer(
                    duration: Duration(milliseconds: 200),
                    height: constraints.maxWidth > 400 ? 80 : 60,
                    width: constraints.maxWidth > 400 ? 80 : 60,
                    decoration: BoxDecoration(
                      color: Colors.blue,
                      borderRadius: BorderRadius.circular(30),
                      boxShadow: [BoxShadow(color: Colors.black26, blurRadius: 10)],
                    ),
                    child: Icon(Icons.arrow_forward, size: 40, color: Colors.white),
                  ),
                ),
              ],
            ),
            GestureDetector(
              onTapDown: (_) => carControlProvider.sendCommand('car/control', '2'),
              onTapUp: (_) => carControlProvider.sendCommand('car/control', '0'),
              child: AnimatedContainer(
                duration: Duration(milliseconds: 200),
                height: constraints.maxWidth > 400 ? 80 : 60,
                width: constraints.maxWidth > 400 ? 80 : 60,
                decoration: BoxDecoration(
                  color: Colors.blue,
                  borderRadius: BorderRadius.circular(30),
                  boxShadow: [BoxShadow(color: Colors.black26, blurRadius: 10)],
                ),
                child: Icon(Icons.arrow_downward, size: 40, color: Colors.white),
              ),
            ),
          ],
        );
      },
    );
  }
}
