import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/car_control_provider.dart';

class LampWidget extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    final lightState = Provider.of<CarControlProvider>(context).lightState;

    Color lampColor;
    String label;
    switch (lightState) {
      case LightState.on:
        lampColor = Colors.yellow;
        label = "On";
        break;
      case LightState.auto:
        lampColor = Colors.yellow.withOpacity(0.5);
        label = "Auto";
        break;
      case LightState.off:
      default:
        lampColor = Colors.grey;
        label = "Off";
        break;
    }

    return LayoutBuilder(
      builder: (context, constraints) {
        return Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.lightbulb,
              size: constraints.maxWidth > 400 ? 120 : 100,
              color: lampColor,
            ),
            Text(
              label,
              style: TextStyle(fontSize: constraints.maxWidth > 400 ? 22 : 18),
            ),
          ],
        );
      },
    );
  }
}
