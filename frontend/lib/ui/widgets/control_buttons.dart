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
}
