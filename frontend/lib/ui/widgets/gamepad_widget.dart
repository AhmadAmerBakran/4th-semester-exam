import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/car_control_provider.dart';

class GamepadWidget extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    final carControlProvider = Provider.of<CarControlProvider>(context, listen: false);

    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        GestureDetector(
          onTapDown: (_) => carControlProvider.sendCommand('car/control', '1'),
          onTapUp: (_) => carControlProvider.sendCommand('car/control', '0'),
          child: AnimatedContainer(
            duration: Duration(milliseconds: 200),
            height: 60,
            width: 60,
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
                height: 60,
                width: 60,
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
                height: 60,
                width: 60,
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
            height: 60,
            width: 60,
            decoration: BoxDecoration(
              color: Colors.blue,
              borderRadius: BorderRadius.circular(30),
              boxShadow: [BoxShadow(color: Colors.black26, blurRadius: 10)],
            ),
            child: Icon(Icons.arrow_downward, size: 40, color: Colors.white),
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
