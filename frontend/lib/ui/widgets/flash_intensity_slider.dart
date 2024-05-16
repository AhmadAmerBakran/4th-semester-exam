import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/car_control_provider.dart';

class FlashIntensitySlider extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Consumer<CarControlProvider>(
      builder: (context, carControlProvider, child) {
        return Slider(
          value: carControlProvider.flashIntensity.toDouble(),
          min: 0,
          max: 100,
          divisions: 4,
          label: carControlProvider.flashIntensity.round().toString(),
          onChanged: (value) {
            carControlProvider.setFlashIntensity(value.round());
          },
        );
      },
    );
  }
}
