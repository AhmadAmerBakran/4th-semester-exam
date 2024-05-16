import 'package:flutter/material.dart';
import 'package:frontend/ui/widgets/styled_button_widget.dart';
import 'package:provider/provider.dart';
import '../../providers/car_control_provider.dart';

class ControlButtons extends StatelessWidget {
  final VoidCallback onStartStream;
  final VoidCallback onStopStream;

  ControlButtons({required this.onStartStream, required this.onStopStream});

  @override
  Widget build(BuildContext context) {
    final carControlProvider = Provider.of<CarControlProvider>(context);
    String getLightButtonText(LightState state) {
      switch (state) {
        case LightState.on:
          return 'Auto Light Mode';
        case LightState.auto:
          return 'Turn Off Lights';
        case LightState.off:
        default:
          return 'Turn On Lights';
      }
    }

    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        CustomIconButton(
          icon: Icons.drive_eta,
          onTap: () => carControlProvider.sendCommand('car/control', '7'),
          color: Colors.blue,
          size: 60,
        ),
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            CustomIconButton(
              icon: Icons.play_arrow,
              onTap: () {
                carControlProvider.startStream();
                onStartStream();
              },
              color: Colors.green,
              size: 60,
            ),
            SizedBox(width: 20),
            CustomIconButton(
              icon: Icons.stop,
              onTap: () {
                carControlProvider.stopStream();
                onStopStream();
              },
              color: Colors.red,
              size: 60,
            ),
          ],
        ),
        CustomIconButton(
          icon: Icons.lightbulb,
          onTap: carControlProvider.cycleLightState,
          color: Colors.orange,
          size: 60,
        ),
      ],
    );
  }
}