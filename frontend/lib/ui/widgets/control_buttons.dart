import 'package:flutter/material.dart';
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
      children: [
        ElevatedButton(
          onPressed: () {
            carControlProvider.startStream();
            onStartStream();
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
            onStopStream();
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
          onPressed: carControlProvider.cycleLightState,
          child: Text(
            getLightButtonText(carControlProvider.lightState),
            style: TextStyle(fontSize: 18),
          ),
          style: ElevatedButton.styleFrom(
            backgroundColor: Colors.orange,
            padding: EdgeInsets.symmetric(horizontal: 20, vertical: 15),
            shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(30)),
            elevation: 10,
          ),
        ),
      ],
    );
  }
}