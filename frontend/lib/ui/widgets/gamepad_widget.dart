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
              child: _buildButton(constraints, Icons.arrow_upward),
            ),
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                GestureDetector(
                  onTapDown: (_) => carControlProvider.sendCommand('car/control', '6'),
                  onTapUp: (_) => carControlProvider.sendCommand('car/control', '0'),
                  child: _buildButton(constraints, Icons.arrow_back),
                ),
                SizedBox(width: 20),
                GestureDetector(
                  onTapDown: (_) => carControlProvider.sendCommand('car/control', '5'),
                  onTapUp: (_) => carControlProvider.sendCommand('car/control', '0'),
                  child: _buildButton(constraints, Icons.arrow_forward),
                ),
              ],
            ),
            GestureDetector(
              onTapDown: (_) => carControlProvider.sendCommand('car/control', '2'),
              onTapUp: (_) => carControlProvider.sendCommand('car/control', '0'),
              child: _buildButton(constraints, Icons.arrow_downward),
            ),
          ],
        );
      },
    );
  }

  Widget _buildButton(BoxConstraints constraints, IconData icon) {
    return AnimatedContainer(
      duration: Duration(milliseconds: 200),
      height: constraints.maxWidth > 400 ? 80 : 60,
      width: constraints.maxWidth > 400 ? 80 : 60,
      decoration: BoxDecoration(
        color: Colors.blue,
        borderRadius: BorderRadius.circular(30),
        boxShadow: [BoxShadow(color: Colors.black26, blurRadius: 10)],
      ),
      child: Icon(icon, size: 40, color: Colors.white),
    );
  }
}
