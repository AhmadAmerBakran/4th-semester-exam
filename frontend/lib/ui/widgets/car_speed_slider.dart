import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/car_control_provider.dart';
import '../../utils/constants.dart';

class CarSpeedSlider extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Consumer<CarControlProvider>(
      builder: (context, carControlProvider, child) {
        return Container(
          width: MediaQuery.of(context).size.width > 800
              ? kWebWidth
              : kMobileWidth,
          child: Row(
            children: [
              Icon(Icons.speed, color: Colors.grey),
              Expanded(
                child: Slider(
                  value: carControlProvider.carSpeed.toDouble(),
                  min: 0,
                  max: 255,
                  divisions: 15,
                  label: carControlProvider.carSpeed.round().toString(),
                  onChanged: (value) {
                    carControlProvider.setCarSpeed(value.round());
                  },
                ),
              ),
              Icon(Icons.speed, color: Colors.red),
            ],
          ),
        );
      },
    );
  }
}