import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../../providers/car_control_provider.dart';
import '../../utils/constants.dart';

class FlashIntensitySlider extends StatelessWidget {
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
              Icon(Icons.flash_off, color: Colors.grey),
              Expanded(
                child: Slider(
                  value: carControlProvider.flashIntensity.toDouble(),
                  min: 0,
                  max: 100,
                  divisions: 4,
                  label: carControlProvider.flashIntensity.round().toString(),
                  onChanged: (value) {
                    carControlProvider.setFlashIntensity(value.round());
                  },
                ),
              ),
              Icon(Icons.flash_on, color: Colors.yellow),
            ],
          ),
        );
      },
    );
  }
}
